using System;
using System.Collections.Generic;
using System.Linq;
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

            HashSet<string> uniqueFileNames = new HashSet<string>();
            // group the fields by class, and generate the source
            foreach (INamedTypeSymbol structSymbol in receiver.Structs)
            {
                (string structSource, string fileName) = ProcessStruct(structSymbol, uniqueFileNames, context);
                if (!string.IsNullOrEmpty(fileName))
                {
                    context.AddSource($"{fileName}AutoGen.g.cs", SourceText.From(structSource, Encoding.UTF8));
                }
            }
        }

        private (string structSource, string fileName) ProcessStruct(INamedTypeSymbol structSymbol, HashSet<string> uniqueFileNames, GeneratorExecutionContext context)
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
    }}
}}
");

            return (source.ToString(), fileName);
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
