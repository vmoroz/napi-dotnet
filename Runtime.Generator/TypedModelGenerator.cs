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
public class AutoNotifyGenerator : ISourceGenerator
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

        INamedTypeSymbol interfaceAttributeSymbol = context.Compilation.GetTypeByMetadataName("NodeApi.EcmaScript.TypedInterfaceAttribute")
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
            (string structSource, string fileName) = ProcessStruct(structSymbol, uniqueFileNames, nameTable, context);
            if (!string.IsNullOrEmpty(fileName))
            {
                context.AddSource($"{fileName}.g.cs", SourceText.From(structSource, Encoding.UTF8));
            }
        }

        {
            // Generate the name table
            string source = ProcessNameTable(nameTable);
            context.AddSource($"NameTable.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private (string structSource, string fileName) ProcessStruct(
        INamedTypeSymbol structSymbol,
        HashSet<string> uniqueFileNames,
        HashSet<string> nameTable,
        GeneratorExecutionContext context)
    {
        if (!structSymbol.ContainingSymbol.Equals(structSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
        {
            throw new Exception("Struct must be in a namespace");
        }

        var s = new SourceBuilder("    ");

        string namespaceName = structSymbol.ContainingNamespace.ToDisplayString();
        string structName = structSymbol.Name;
        string fileName = structName;
        if (structSymbol.IsGenericType)
        {
            structName += "<" + string.Join(", ", structSymbol.TypeParameters.Select(t => t.Name)) + ">";
            fileName += "_" + string.Join("_", structSymbol.TypeParameters.Select(t => t.Name));
        }
        if (Char.IsLower(fileName[0]))
        {
            fileName = "_" + fileName;
        }

        if (uniqueFileNames.Contains(fileName))
        {
            return ("", "");
        }

        uniqueFileNames.Add(fileName);

        s += $$"""
            namespace {{namespaceName}};

            public partial struct {{structName}}
            {
                private JSValue _value;

                public static explicit operator {{structName}}(JSValue value) => new {{structName}} { _value = value };
                public static implicit operator JSValue({{structName}} value) => value._value;

                // Map Undefined to Nullable
                public static explicit operator {{structName}}?(JSValue value)
                    => value.TypeOf() != JSValueType.Undefined ? ({{structName}})value : null;

                public static implicit operator JSValue({{structName}}? value)
                    => value is {{structName}} notNullValue ? notNullValue._value : JSValue.Undefined;

            """;
        s.IncreaseIndent();

        foreach (var interfaceSymbol in structSymbol.AllInterfaces)
        {
            foreach (var member in interfaceSymbol.GetMembers())
            {
                if (member is IPropertySymbol propertySymbol)
                {
                    if (propertySymbol.IsStatic) { continue; }
                    bool isReadonly = propertySymbol.SetMethod == null;
                    string typeName = ToDisplayString(propertySymbol.Type);
                    string propertyName = propertySymbol.Name;

                    if (propertySymbol.Parameters.Length == 0)
                    {
                        nameTable.Add(propertySymbol.Name);
                        if (isReadonly)
                        {
                            s += $$"""
                                public {{typeName}} {{propertyName}}
                                    => ({{typeName}})_value.GetProperty(NameTable.{{propertyName}});
                                """;
                        }
                        else
                        {
                            s += $$"""
                                public {{typeName}} {{propertyName}}
                                {
                                    get => ({{typeName}})_value.GetProperty(NameTable.{{propertyName}});
                                    set => _value.SetProperty(NameTable.{{propertyName}}, value);
                                }
                                """;
                        }
                    }
                    else if (propertySymbol.Parameters.Length == 1)
                    {
                        string parameterName = propertySymbol.Parameters[0].Name;
                        string parameterType = ToDisplayString(propertySymbol.Parameters[0].Type);
                        if (isReadonly)
                        {
                            s += $$"""
                                public {{typeName}} this[{{parameterType}} {{parameterName}}]
                                    => ({{typeName}})_value.GetProperty({{parameterName}});
                                """;
                        }
                        else
                        {
                            s += $$"""
                                public {{typeName}} this[{{parameterType}} {{parameterName}}]
                                {
                                    get => ({{typeName}})_value.GetProperty({{parameterName}});
                                    set => _value.SetProperty({{parameterName}}, value);
                                }
                                """;
                        }
                    }
                    s++;
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
                                typeContraints += $$"""
                                        where {{ToDisplayString(p)}} : struct, {{constraintTypes}}
                                    """;
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
                        s += $$"""
                            public {{returnTypeName}} New{{genericArgs}}({{parameters}}){{typeContraints}}
                                => ({{returnTypeName}})_value.CallAsConstructor({{args}});
                            """;
                    }
                    else if (methodName == "Call")
                    {
                        string args = string.Join(", ", methodSymbol.Parameters.Select(p => p.Name));
                        args = args.Length > 0 ? ", " + args : "";
                        s += $$"""
                            public {{returnTypeName}} Call{{genericArgs}}({{parameters}}){{typeContraints}}
                                => ({{returnTypeName}})_value.Call(JSValue.Undefined{{args}});
                            """;
                    }
                    else
                    {
                        string args = string.Join(", ", methodSymbol.Parameters.Select(p => p.Name));
                        args = args.Length > 0 ? ", " + args : "";
                        s += $$"""
                            public {{returnTypeName}} {{methodName}}{{genericArgs}}({{parameters}}){{typeContraints}}
                                => ({{returnTypeName}})_value.CallMethod(NameTable.{{methodName}}{{args}});
                            """;
                    }
                    s++;
                }
            }
        }

        s.DecreaseIndent();
        s += "}";

        var constructorInterface = structSymbol.AllInterfaces.SingleOrDefault(i => i.Name == NameTable.ITypedConstructor.Text);
        if (constructorInterface != null)
        {
            ITypeSymbol targetType = constructorInterface.TypeArguments[1];
            string targetName = targetType.Name;

            s += $$"""

                public partial struct {{targetName}}
                {
                """;
            s.IncreaseIndent();

            foreach (var interfaceSymbol in structSymbol.AllInterfaces)
            {
                foreach (var member in interfaceSymbol.GetMembers())
                {
                    if (member is IPropertySymbol propertySymbol)
                    {
                        if (propertySymbol.IsStatic)
                        {
                            continue;
                        }

                        bool isReadonly = propertySymbol.SetMethod == null;
                        string typeName = ToDisplayString(propertySymbol.Type);
                        string propertyName = propertySymbol.Name;

                        if (propertySymbol.Parameters.Length == 0)
                        {
                            nameTable.Add(propertySymbol.Name);

                            if (isReadonly)
                            {
                                s += $$"""
                                    public static {{typeName}} {{propertyName}}
                                        => ({{typeName}})((JSValue){{structName}}.Instance).GetProperty(NameTable.{{propertyName}});
                                    """;
                            }
                            else
                            {
                                s += $$"""
                                    public static {{typeName}} {{propertyName}}
                                    {
                                        get => ({{typeName}})((JSValue){{structName}}.Instance).GetProperty(NameTable.{{propertyName}});
                                        set => ((JSValue){{structName}}.Instance).SetProperty(NameTable.{{propertyName}}, value);
                                    }
                                    """;
                            }
                        }
                        else if (propertySymbol.Parameters.Length == 1)
                        {
                            continue;
                        }
                        s++;
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
                                    typeContraints += $$"""
                                            where {{ToDisplayString(p)}} : struct, {{constraintTypes}}
                                        """;
                                }
                            }
                        }

                        string parameters = string.Join(", ", methodSymbol.Parameters.Select(p => ToDisplayString(p.Type)
                            + " "
                            + p.Name
                            + (p.HasExplicitDefaultValue ? " = " + (p.ExplicitDefaultValue is object o ? o.ToString() : "null") : "")));
                        string returnTypeName = ToDisplayString(methodSymbol.ReturnType);
                        if (methodName == "New")
                        {
                            string args = string.Join(", ", methodSymbol.Parameters.Select(p => p.Name));
                            s += $$"""
                                public {{targetName}}{{genericArgs}}({{parameters}}){{typeContraints}}
                                    => _value = ((JSValue){{structName}}.Instance).CallAsConstructor({{args}});
                                """;
                        }
                        else if (methodName == "Call")
                        {
                            string args = string.Join(", ", methodSymbol.Parameters.Select(p => p.Name));
                            args = args.Length > 0 ? ", " + args : "";
                            s += $$"""
                                public static {{returnTypeName}} Call{{genericArgs}}({{parameters}}){{typeContraints}}
                                    => ({{returnTypeName}})((JSValue){{structName}}.Instance).Call(JSValue.Undefined{{args}});
                                """;
                        }
                        else
                        {
                            string args = string.Join(", ", methodSymbol.Parameters.Select(p => p.Name));
                            args = args.Length > 0 ? ", " + args : "";
                            s += $$"""
                                public static {{returnTypeName}} {{methodName}}{{genericArgs}}({{parameters}}){{typeContraints}}
                                    => ({{returnTypeName}})((JSValue){{structName}}.Instance).CallMethod(NameTable.{{methodName}}{{args}});
                                """;
                        }
                        s++;
                    }
                }
            }

            s.DecreaseIndent();
            s += "}";
        }


        return (s.ToString(), fileName);
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
        return typeSymbol.ToDisplayString();
    }

    private string ProcessNameTable(HashSet<string> nameTable)
    {
        List<string> sortedNames = nameTable.ToList();
        sortedNames.Sort();

        // begin building the generated source
        StringBuilder source = new StringBuilder(@"
namespace NodeApi.EcmaScript
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

                if (interfaceSymbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "NodeApi.EcmaScript.TypedInterfaceAttribute"))
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

/// <summary>
/// Naming constants used for analyzing and generating code.
/// </summary>
internal class NameTable
{
    public static readonly SyntaxToken IJSValueHolder = Identifier(nameof(IJSValueHolder));
    public static readonly SyntaxToken ITypedConstructor = Identifier(nameof(ITypedConstructor));
}
