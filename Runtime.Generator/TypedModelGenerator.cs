using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NodeApi.Runtime.Generator
{

    [Generator]
    public class AutoNotifyGenerator : ISourceGenerator
    {
        private const string attributeText = @"
using System;

namespace AutoGenerate
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    [System.Diagnostics.Conditional(""AutoGenerateGenerator_DEBUG"")]
    sealed class AutoGenerateAttribute : Attribute
    {
        public AutoGenerateAttribute()
        {
        }
    }
}
";

        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute source
            context.RegisterForPostInitialization((i) => i.AddSource("AutoNotifyAttribute.g.cs", attributeText));

            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver 
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
                return;

            // get the added attribute, and INotifyPropertyChanged
            INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName("AutoGenerate.AutoGenerateAttribute")
                ?? throw new Exception("Attribute symbol not found");
            INamedTypeSymbol nullableSymbol = context.Compilation.GetTypeByMetadataName("System.Nullable")
                ?? throw new Exception("Attribute symbol not found");

            HashSet<string> nameTable = new HashSet<string>();
            HashSet<string> uniqueFileNames = new HashSet<string>();
            // group the fields by class, and generate the source
            foreach (INamedTypeSymbol structSymbol in receiver.Structs)
            {
                (string structSource, string fileName) = ProcessStruct(structSymbol, uniqueFileNames, nameTable, context, nullableSymbol);
                if (!string.IsNullOrEmpty(fileName))
                {
                    context.AddSource($"{fileName}AutoGen.g.cs", SourceText.From(structSource, Encoding.UTF8));
                }
            }

            string source = ProcessNameTable(nameTable);
            context.AddSource($"NameTable_AutoGen.g.cs", SourceText.From(source, Encoding.UTF8));
        }

        private (string structSource, string fileName) ProcessStruct(
            INamedTypeSymbol structSymbol,
            HashSet<string> uniqueFileNames,
            HashSet<string> nameTable,
            GeneratorExecutionContext context,
            INamedTypeSymbol nullableSymbol)
        {
            if (!structSymbol.ContainingSymbol.Equals(structSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                throw new Exception("Struct must be in a namespace");
            }

            string namespaceName = structSymbol.ContainingNamespace.ToDisplayString();
            string structName = structSymbol.Name;
            string fileName = structName + "_";
            if (structSymbol.IsGenericType)
            {
                structName += "<" + string.Join(", ", structSymbol.TypeParameters.Select(t => t.Name)) + ">";
                fileName += string.Join("_", structSymbol.TypeParameters.Select(t => t.Name)) + "_";
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
    public partial struct {structName}
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

            foreach (var interfaceSymbol in structSymbol.AllInterfaces)
            {
                if (interfaceSymbol.Name == NameTable.IJSValueHolder.Text)
                {
                    continue;
                }

                foreach (var member in interfaceSymbol.GetMembers())
                {
                    if (member is IPropertySymbol propertySymbol)
                    {
                        bool isWritable = propertySymbol.SetMethod != null;
                        string typeName = propertySymbol.Type.ToDisplayString();

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
                            string parameterType = propertySymbol.Parameters[0].Type.ToDisplayString();
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

                        nameTable.Add(methodSymbol.Name);

                        string methodText = methodSymbol.DeclaringSyntaxReferences[0].GetSyntax().GetText().ToString();
                        methodText = methodText.Trim();
                        methodText = methodText.Substring(0, methodText.Length - 1);
                        string returnTypeName = methodSymbol.ReturnType.ToDisplayString();
                        if (methodSymbol.Name == "New")
                        {
                            string args = string.Join(", ", methodSymbol.Parameters.Select(p => p.Name));
                            source.Append($@"
        public {methodText}
            => ({returnTypeName})_value.CallAsConstructor({args});
");
                        }
                        else if (methodSymbol.Name == "Call")
                        {
                            string args = string.Join(", ", methodSymbol.Parameters.Select(p => p.Name));
                            args = args.Length > 0 ? ", " + args : "";
                            source.Append($@"
        public {methodText}
            => ({returnTypeName})_value.Call(_value{args});
");
                        }
                        else
                        {
                            string args = string.Join(", ", methodSymbol.Parameters.Select(p => p.Name));
                            args = args.Length > 0 ? ", " + args : "";
                            source.Append($@"
        public {methodText}
            => ({returnTypeName})_value.CallMethod(NameTable.{methodSymbol.Name}{args});
");
                        }
                    }
                }
            }

            source.Append(@"
    }
}
");

            return (source.ToString(), fileName);
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

        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        class SyntaxReceiver : ISyntaxContextReceiver
        {
            public List<IFieldSymbol> Fields { get; } = new List<IFieldSymbol>();

            public List<INamedTypeSymbol> JSValueInterfaces { get; } = new List<INamedTypeSymbol>();
            public List<INamedTypeSymbol> Structs { get; } = new List<INamedTypeSymbol>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (context.Node is InterfaceDeclarationSyntax interfaceDeclarationSyntax)
                {
                    INamedTypeSymbol interfaceSymbol = context.SemanticModel.GetDeclaredSymbol(interfaceDeclarationSyntax)
                        ?? throw new Exception("semantic node not found");
                    AddIJSValueHolderDerived(interfaceSymbol);
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

            private void AddIJSValueHolderDerived(INamedTypeSymbol interfaceSymbol)
            {
                if (IsIJSValueHolderDerived(interfaceSymbol))
                {
                    JSValueInterfaces.Add(interfaceSymbol);
                }
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
    }
}
