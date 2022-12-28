using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NodeApi.Generator;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NodeApi.Runtime.Generator;

[Generator]
public class TypedModelGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Register a syntax receiver that will be created for each generation pass
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // retrieve the populated receiver 
        if (!(context.SyntaxContextReceiver is SyntaxReceiver syntaxReceiver))
        {
            return;
        }

        INamedTypeSymbol interfaceAttributeSymbol = context.Compilation.GetTypeByMetadataName("NodeApi.TypedModel.TypedInterfaceAttribute")
            ?? throw new Exception("Symbol not found for TypedInterfaceAttribute");

        HashSet<string> nameTable = new HashSet<string>();
        HashSet<string> uniqueFileNames = new HashSet<string>();

        // Generate typed interfaces
        foreach (INamedTypeSymbol interfaceSymbol in syntaxReceiver.TypedInterfaces)
        {
            (string source, string fileName) = ProcessTypedInterface(interfaceSymbol, uniqueFileNames, nameTable, interfaceAttributeSymbol, context);
            if (!string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(fileName))
            {
                context.AddSource($"{fileName}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        foreach (INamedTypeSymbol structSymbol in syntaxReceiver.Structs)
        {
            var codeGenerator = new StructCodeGenerator(structSymbol, nameTable, context);
            string source = codeGenerator.Execute();
            if (!string.IsNullOrEmpty(codeGenerator.FileName))
            {
                context.AddSource($"{codeGenerator.FileName}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        {
            // Generate the name table
            string source = ProcessNameTable(nameTable);
            context.AddSource($"NameTable.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private (string structSource, string fileName) ProcessTypedInterface(
        INamedTypeSymbol interfaceSymbol,
        HashSet<string> uniqueFileNames,
        HashSet<string> nameTable,
        INamedTypeSymbol interfaceAttributeSymbol,
        GeneratorExecutionContext context)
    {
        if (!interfaceSymbol.ContainingSymbol.Equals(interfaceSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
        {
            throw new Exception($"Interface must be in a namespace: {interfaceSymbol.Name}");
        }

        // Get the name of the struct to be generated.
        AttributeData attributeData = interfaceSymbol.GetAttributes().Single(a => a.AttributeClass?.Equals(interfaceAttributeSymbol, SymbolEqualityComparer.Default) ?? false);
        TypedConstant overridenNameOpt = attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "Name").Value;
        string structName = ChooseName(interfaceSymbol.Name, overridenNameOpt);

        string namespaceName = interfaceSymbol.ContainingNamespace.ToDisplayString();
        string fileName = structName;
        string structBaseType = interfaceSymbol.Name;
        if (interfaceSymbol.IsGenericType)
        {
            if (interfaceSymbol.TypeParameters.Length != 1)
            {
                throw new Exception($"We do not support more than one generic type parameter: {interfaceSymbol.Name}.");
            }
            if (interfaceSymbol.TypeParameters[0].Name != "TSelf")
            {
                throw new Exception($"Generic type parameter name must be TSelf: {interfaceSymbol.Name}.");
            }
            structBaseType += "<" + structName + ">";
        }

        if (uniqueFileNames.Contains(fileName))
        {
            return ("", "");
        }

        uniqueFileNames.Add(fileName);

        // begin building the generated source
        StringBuilder source = new StringBuilder($@"
namespace {namespaceName}
{{
    public partial struct {structName} : {structBaseType}
    {{
        private JSValue _value;

        public static explicit operator {structName}(JSValue value) => new {structName} {{ _value = value }};
        public static implicit operator JSValue({structName} value) => value._value;

        // Map Undefined to Nullable
        public static explicit operator {structName}?(JSValue value)
            => value.TypeOf() != JSValueType.Undefined ? ({structName})value : null;
        public static implicit operator JSValue({structName}? value)
            => value is {structName} notNullValue ? notNullValue._value : JSValue.Undefined;
");

        foreach (var member in interfaceSymbol.GetMembers())
        {
            if (member is IPropertySymbol propertySymbol)
            {
                bool isWritable = propertySymbol.SetMethod != null;
                string typeName = ToDisplayString(propertySymbol.Type);

                if (propertySymbol.Parameters.Length == 0)
                {
                    nameTable.Add(propertySymbol.Name);

                    source.Append($@"
        public {typeName} {propertySymbol.Name}
        {{
            get => ({typeName})_value.GetProperty(NameTable.{propertySymbol.Name});");

                    if (isWritable)
                    {
                        source.Append($@"
            set => _value.SetProperty(NameTable.{propertySymbol.Name}, value);");
                    }

                    source.Append(@"
        }
");
                }
                else if (propertySymbol.Parameters.Length == 1)
                {
                    string parameterName = propertySymbol.Parameters[0].Name;
                    string parameterType = ToDisplayString(propertySymbol.Parameters[0].Type);
                    source.Append($@"
        public {typeName} this[{parameterType} {parameterName}]
        {{
            get => ({typeName})_value.GetProperty({parameterName});");

                    if (isWritable)
                    {
                        source.Append($@"
            set => _value.SetProperty({parameterName}, value);");
                    }

                    source.Append(@"
        }
");
                }
            }
            else if (member is IMethodSymbol methodSymbol)
            {
                if (methodSymbol.MethodKind != MethodKind.Ordinary)
                {
                    continue;
                }

                string methodName = methodSymbol.Name;
                nameTable.Add(methodName);

                string genericArgs = "";
                string typeContraints = "";
                if (methodSymbol.IsGenericMethod)
                {
                    genericArgs = "<" + string.Join(", ", methodSymbol.TypeParameters.Select(p => ToDisplayString(p))) + ">";

                    foreach (ITypeParameterSymbol p in methodSymbol.TypeParameters)
                    {
                        if (p.HasValueTypeConstraint)
                        {
                            string constraintTypes = string.Join(", ", p.ConstraintTypes.Select(c => ToDisplayString(c)));
                            typeContraints += $@"
            where {ToDisplayString(p)} : struct, {constraintTypes}";
                        }
                    }
                }

                string parameters = string.Join(", ", methodSymbol.Parameters.Select(p
                    => ToDisplayString(p.Type)
                    + " "
                    + p.Name
                    + (p.HasExplicitDefaultValue ? " = " + (p.ExplicitDefaultValue is object o ? o.ToString() : "null") : "")));
                string returnTypeName = ToDisplayString(methodSymbol.ReturnType);
                if (methodName == "New")
                {
                    string args = string.Join(", ", methodSymbol.Parameters.Select(p => p.Name));
                    source.Append($@"
        public {returnTypeName} {methodName}{genericArgs}({parameters}){typeContraints}
            => ({returnTypeName})_value.CallAsConstructor({args});
");
                }
                else if (methodName == "Call")
                {
                    string args = string.Join(", ", methodSymbol.Parameters.Select(p => p.Name));
                    args = args.Length > 0 ? ", " + args : "";
                    source.Append($@"
        public {returnTypeName} {methodName}{genericArgs}({parameters}){typeContraints}
            => ({returnTypeName})_value.Call(_value{args});
");
                }
                else
                {
                    string args = string.Join(", ", methodSymbol.Parameters.Select(p => p.Name));
                    args = args.Length > 0 ? ", " + args : "";
                    source.Append($@"
        public {returnTypeName} {methodName}{genericArgs}({parameters}){typeContraints}
            => ({returnTypeName})_value.CallMethod(NameTable.{methodName}{args});
");
                }
            }
        }

        source.Append(@"
    }
");
        if (interfaceSymbol.Interfaces.Length == 0)
        {
            source.Append($@"
    public partial interface {interfaceSymbol.Name} : IJSValueHolder<{structName}> {{ }}
");
        }

        source.Append(@"
}
");

        return (source.ToString(), fileName);
    }

    private string ChooseName(string interfaceName, TypedConstant overridenNameOpt)
    {
        if (!overridenNameOpt.IsNull && overridenNameOpt.Value is object value)
        {
            return value.ToString();
        }

        return interfaceName.TrimStart('I');
    }

    private string ToDisplayString(ITypeSymbol typeSymbol)
    {
        string typeName = typeSymbol.ToDisplayString();
        // If type name starts with a lower case letter, then prefix it with '@'
        int typeNameStart = typeName.LastIndexOf('.') + 1;
        if (typeNameStart > 0 && char.IsLower(typeName[typeNameStart]))
        {
            typeName = typeName.Insert(typeNameStart, "@");
        }
        return typeName;
    }

    private string ProcessNameTable(HashSet<string> nameTable)
    {
        List<string> sortedNames = nameTable.ToList();
        sortedNames.Sort();

        // begin building the generated source
        StringBuilder source = new StringBuilder(@"
namespace NodeApi.TypedModel
{
    public partial class NameTable
    {");

        foreach (var name in sortedNames)
        {
            source.Append($@"
        public static JSValue {name} => GetStringName(nameof({name}));");
        }

        source.Append(@"
    }
}
");

        return source.ToString();
    }

    // Created on demand before each generation pass
    class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<INamedTypeSymbol> TypedInterfaces { get; } = new List<INamedTypeSymbol>();
        public List<INamedTypeSymbol> Structs { get; } = new List<INamedTypeSymbol>();

        // Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is InterfaceDeclarationSyntax interfaceDeclarationSyntax
                && interfaceDeclarationSyntax.AttributeLists.Count > 0)
            {
                INamedTypeSymbol interfaceSymbol = context.SemanticModel.GetDeclaredSymbol(interfaceDeclarationSyntax)
                    ?? throw new Exception($"Semantic node not found for {interfaceDeclarationSyntax}");

                if (interfaceSymbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "NodeApi.TypedModel.TypedInterfaceAttribute"))
                {
                    TypedInterfaces.Add(interfaceSymbol);
                }
            }
            else if (context.Node is StructDeclarationSyntax structDeclarationSyntax)
            {
                INamedTypeSymbol structSymbol = context.SemanticModel.GetDeclaredSymbol(structDeclarationSyntax)
                    ?? throw new Exception("semantic node not found");
                AddStructToGenerate(structSymbol);
            }
        }

        private bool IsIJSValueHolderDerived(INamedTypeSymbol interfaceSymbol)
        {
            if (interfaceSymbol.Name == NameTable.IJSValueHolder.Text)
            {
                return false;
            }

            foreach (var parentInterface in interfaceSymbol.AllInterfaces)
            {
                if (parentInterface.Name == NameTable.IJSValueHolder.Text)
                {
                    return true;
                }
            }

            return false;
        }

        private void AddStructToGenerate(INamedTypeSymbol structSymbol)
        {
            if (IsIJSValueHolderDerived(structSymbol))
            {
                Structs.Add(structSymbol);
            }
        }
    }
}

public class StructCodeGenerator
{
    private INamedTypeSymbol _structSymbol;
    private HashSet<string> _nameTable;
    private GeneratorExecutionContext _context;
    private SourceBuilder _s;

    public string FileName { get; private set; } = "";

    public StructCodeGenerator(
        INamedTypeSymbol structSymbol,
        HashSet<string> nameTable,
        GeneratorExecutionContext context)
    {
        _structSymbol = structSymbol;
        _nameTable = nameTable;
        _context = context;
        _s = new SourceBuilder(indent: "    ");
    }

    public string Execute()
    {
        if (!_structSymbol.ContainingSymbol.Equals(_structSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
        {
            throw new Exception("Struct must be in a namespace");
        }

        string namespaceName = _structSymbol.ContainingNamespace.ToDisplayString();
        string structName = _structSymbol.Name;
        string fileName = structName;
        if (_structSymbol.IsGenericType)
        {
            structName += "<" + string.Join(", ", _structSymbol.TypeParameters.Select(t => t.Name)) + ">";
            fileName += "_" + string.Join("_", _structSymbol.TypeParameters.Select(t => t.Name));
        }

        if (char.IsLower(structName[0]))
        {
            structName = '@' + structName;
            fileName = "_" + fileName;
        }

        _s += $"namespace {namespaceName};";
        _s++;
        _s += $"public partial struct {structName}";
        _s += "{";
        _s += "private JSValue _value;";
        _s++;
        _s += $"public static explicit operator {structName}(JSValue value) => new {structName} {{ _value = value }};";
        _s += $"public static implicit operator JSValue({structName} value) => value._value;";
        _s++;
        _s += $"public static explicit operator {structName}?(JSValue value) => value.TypeOf() != JSValueType.Undefined ? ({structName})value : null;";
        _s += $"public static implicit operator JSValue({structName}? value) => value is {structName} notNullValue ? notNullValue._value : JSValue.Undefined;";
        _s++;

        foreach (INamedTypeSymbol interfaceSymbol in _structSymbol.AllInterfaces)
        {
            GenerateInterfaceMembers(interfaceSymbol);
        }

        _s += "}";

        var constructorInterface = _structSymbol.AllInterfaces.SingleOrDefault(i => i.Name == NameTable.ITypedConstructor.Text);
        if (constructorInterface != null)
        {
            ITypeSymbol targetType = constructorInterface.TypeArguments[1];
            string targetName = targetType.Name;

            _s++;
            _s += $"public partial struct {targetName}";
            _s += "{";

            foreach (var interfaceSymbol in _structSymbol.AllInterfaces)
            {
                GenerateStaticInterfaceMembers(structName, targetName, interfaceSymbol);
            }

            _s += "}";

            GenerateInstanceInGlobalCache(structName);
        }

        FileName = fileName;
        return _s.ToString();
    }

    private void GenerateInterfaceMembers(INamedTypeSymbol interfaceSymbol)
    {
        foreach (ISymbol member in interfaceSymbol.GetMembers())
        {
            if (member.IsStatic)
            {
                continue;
            }

            if (member is IPropertySymbol propertySymbol)
            {
                GenerateProperty(propertySymbol);
            }
            else if (member is IMethodSymbol methodSymbol)
            {
                GenerateMethod(methodSymbol);
            }
        }
    }

    private void GenerateInstanceInGlobalCache(string structName)
    {
        INamedTypeSymbol attributeSymbol = _context.Compilation.GetTypeByMetadataName("NodeApi.TypedModel.GenerateInstanceInGlobalCacheAttribute")
            ?? throw new Exception("Symbol not found for GenerateInstanceInGlobalCacheAttribute");
        AttributeData? attributeData = _structSymbol.GetAttributes().SingleOrDefault(a => a.AttributeClass?.Equals(attributeSymbol, SymbolEqualityComparer.Default) ?? false);
        if (attributeData is not null)
        {
            TypedConstant globalPropertyNameConst = attributeData.ConstructorArguments.FirstOrDefault();
            string globalPropertyName = globalPropertyNameConst.Value is object value ? value.ToString() : structName;

            _s++;
            _s += $"public partial struct {structName}";
            _s += "{";
            _s += $"public static {structName} Instance => GlobalCache.{globalPropertyName};";
            _s += "}";
            _s++;
            _s += $"public partial class GlobalCache";
            _s += "{";
            _s += $"public static {structName} {globalPropertyName} => ({structName})GetValue(CacheId.{globalPropertyName});";
            _s += $"private partial class CacheId {{ public static readonly CacheId {globalPropertyName} = new CacheId(nameof({globalPropertyName})); }}";
            _s += "}";
        }
    }

    private void GenerateStaticInterfaceMembers(string structName, string targetName, INamedTypeSymbol interfaceSymbol)
    {
        foreach (var member in interfaceSymbol.GetMembers())
        {
            if (member.IsStatic)
            {
                continue;
            }

            if (member is IPropertySymbol propertySymbol)
            {
                GenerateStaticProperty(propertySymbol, structName);
            }
            else if (member is IMethodSymbol methodSymbol)
            {
                GenerateStaticMethod(methodSymbol, structName, targetName);
            }
        }
    }

    private void GenerateProperty(IPropertySymbol propertySymbol)
    {
        bool isReadonly = propertySymbol.SetMethod == null;
        string propertyType = ToDisplayString(propertySymbol.Type);

        if (propertySymbol.Parameters.Length == 0)
        {
            string propertyName = propertySymbol.Name;
            _nameTable.Add(propertyName);
            if (isReadonly)
            {
                _s += $"public {propertyType} {propertyName}";
                _s.IncreaseIndent();
                _s += $"=> ({propertyType})_value.GetProperty(NameTable.{propertyName});";
                _s.DecreaseIndent();
            }
            else
            {
                _s += $"public {propertyType} {propertyName}";
                _s += "{";
                _s += $"get => ({propertyType})_value.GetProperty(NameTable.{propertyName});";
                _s += $"set => _value.SetProperty(NameTable.{propertyName}, value);";
                _s += "}";
            }
            _s++;
        }
        else if (propertySymbol.Parameters.Length == 1)
        {
            string parameterType = ToDisplayString(propertySymbol.Parameters[0].Type);
            string parameterName = propertySymbol.Parameters[0].Name;
            if (isReadonly)
            {
                _s += $"public {propertyType} this[{parameterType} {parameterName}]";
                _s.IncreaseIndent();
                _s += $"=> ({propertyType})_value.GetProperty({parameterName});";
                _s.DecreaseIndent();
            }
            else
            {
                _s += $"public {propertyType} this[{parameterType} {parameterName}]";
                _s += "{";
                _s += $"get => ({propertyType})_value.GetProperty({parameterName});";
                _s += $"set => _value.SetProperty({parameterName}, value);";
                _s += "}";
            }
            _s++;
        }
    }

    private void GenerateMethod(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.MethodKind != MethodKind.Ordinary)
        {
            return;
        }

        string methodName = methodSymbol.Name;
        _nameTable.Add(methodName);

        string returnType = ToDisplayString(methodSymbol.ReturnType);
        string parameters = GetParameters(methodSymbol);
        string typeParameters = GetTypeParameters(methodSymbol);
        string args = GetArgs(methodSymbol, methodName);

        _s += $"public {returnType} {methodName}{typeParameters}({parameters})";
        _s.IncreaseIndent();
        WriteTypeConstraints(methodSymbol);
        switch (methodName)
        {
            case "New": _s += $"=> ({returnType})_value.CallAsConstructor({args});"; break;
            case "Call": _s += $"=> ({returnType})_value.Call({args});"; break;
            default: _s += $"=> ({returnType})_value.CallMethod({args});"; break;
        };
        _s.DecreaseIndent();
        _s++;
    }

    private void WriteTypeConstraints(IMethodSymbol methodSymbol)
    {
        foreach (ITypeParameterSymbol typeParameter in methodSymbol.TypeParameters)
        {
            if (typeParameter.HasValueTypeConstraint)
            {
                string constraintTypes = string.Join(", ", typeParameter.ConstraintTypes.Select(t => ToDisplayString(t)));
                _s += $"where {ToDisplayString(typeParameter)} : struct, {constraintTypes}";
            }
        }
    }

    private string GetArgs(IMethodSymbol methodSymbol, string methodName)
    {
        return string.Join(", ", GetFirstArg().Union(methodSymbol.Parameters.Select(p => p.Name)));

        IEnumerable<string> GetFirstArg() => methodName switch
        {
            "New" => Enumerable.Empty<string>(),
            "Call" => Enumerable.Repeat("JSValue.Undefined", 1),
            _ => Enumerable.Repeat($"NameTable.{methodName}", 1)
        };
    }

    private string GetParameters(IMethodSymbol methodSymbol)
    {
        Func<IParameterSymbol, string?> getDefaultValue = p => p.HasExplicitDefaultValue ? p.ExplicitDefaultValue?.ToString() : null;
        Func<IParameterSymbol, string> assignDefault = p => (getDefaultValue(p) is string value) ? " = " + value : "";
        Func<IParameterSymbol, string> parameterString = p => $"{ToDisplayString(p.Type)} {p.Name}{assignDefault(p)}";
        string parameters = string.Join(", ", methodSymbol.Parameters.Select(p => parameterString(p)));
        return parameters;
    }

    private string GetTypeParameters(IMethodSymbol methodSymbol)
    {
        return (methodSymbol.IsGenericMethod)
            ? "<" + string.Join(", ", methodSymbol.TypeParameters.Select(p => ToDisplayString(p))) + ">"
            : "";
    }

    private void GenerateStaticProperty(IPropertySymbol propertySymbol, string constructorName)
    {
        if (propertySymbol.IsStatic || propertySymbol.Parameters.Length > 0)
        {
            return;
        }

        bool isReadonly = propertySymbol.SetMethod == null;
        string propertyType = ToDisplayString(propertySymbol.Type);
        string propertyName = propertySymbol.Name;
        _nameTable.Add(propertyName);

        if (isReadonly)
        {
            _s += $"public static {propertyType} {propertyName}";
            _s.IncreaseIndent();
            _s += $"=> ({propertyType})((JSValue){constructorName}.Instance).GetProperty(NameTable.{propertyName});";
            _s.DecreaseIndent();
        }
        else
        {
            _s += $"public static {propertyType} {propertyName}";
            _s += "{";
            _s += $"get => ({propertyType})((JSValue){constructorName}.Instance).GetProperty(NameTable.{propertyName});";
            _s += $"set => ((JSValue){constructorName}.Instance).SetProperty(NameTable.{propertyName}, value);";
            _s += "}";
        }
        _s++;
    }

    private void GenerateStaticMethod(IMethodSymbol methodSymbol, string constructorType, string targetName)
    {
        if (methodSymbol.MethodKind != MethodKind.Ordinary)
        {
            return;
        }

        string methodName = methodSymbol.Name;
        _nameTable.Add(methodName);

        string returnType = ToDisplayString(methodSymbol.ReturnType);
        string parameters = GetParameters(methodSymbol);
        string typeParameters = GetTypeParameters(methodSymbol);
        string args = GetArgs(methodSymbol, methodName);

        if (methodName == "New")
        {
            // Generate C# constructor instead of a static method.
            _s += $"public {targetName}{typeParameters}({parameters})";
        }
        else
        {
            _s += $"public static {returnType} {methodName}{typeParameters}({parameters})";
        }

        _s.IncreaseIndent();
        WriteTypeConstraints(methodSymbol);
        switch (methodName)
        {
            case "New": _s += $"=> _value = ((JSValue){constructorType}.Instance).CallAsConstructor({args});"; break;
            case "Call": _s += $"=> ({returnType})((JSValue){constructorType}.Instance).Call({args});"; break;
            default: _s += $"=> ({returnType})((JSValue){constructorType}.Instance).CallMethod({args});"; break;
        };
        _s.DecreaseIndent();
        _s++;
    }

    private string ToDisplayString(ITypeSymbol typeSymbol)
    {
        string typeName = typeSymbol.ToDisplayString();
        // If type name starts with a lower case letter, then prefix it with '@'
        int typeNameStart = typeName.LastIndexOf('.') + 1;
        if (typeNameStart > 0 && char.IsLower(typeName[typeNameStart]))
        {
            typeName = typeName.Insert(typeNameStart, "@");
        }
        return typeName;
    }
}

/// <summary>
/// Naming constants used for analyzing and generating code.
/// </summary>
internal class NameTable
{
    public static readonly SyntaxToken IJSValueHolder = Identifier(nameof(IJSValueHolder));
    public static readonly SyntaxToken ITypedConstructor = Identifier(nameof(ITypedConstructor));
}
