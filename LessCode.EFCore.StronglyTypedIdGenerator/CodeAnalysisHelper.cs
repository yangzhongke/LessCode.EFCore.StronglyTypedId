using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LessCode.EFCore.StronglyTypedIdGenerator
{
    public static class CodeAnalysisHelper
    {
        public static string GetTypeName(TypeSyntax typeSyntax)
        {
            if (typeSyntax is IdentifierNameSyntax identifierName)
            {
                // Simple identifiers
                return identifierName.Identifier.Text;
            }
            else if (typeSyntax is QualifiedNameSyntax qualifiedName)
            {
                // Fully qualified names
                return qualifiedName.ToFullString();
            }
            else if (typeSyntax is PredefinedTypeSyntax predefinedType)
            {
                // Predefined types like int, string
                return predefinedType.Keyword.Text;
            }
            else if (typeSyntax is NullableTypeSyntax nullableType)
            {
                // Nullable types
                return GetTypeName(nullableType.ElementType) + "?";
            }
            else if (typeSyntax is GenericNameSyntax genericName)
            {
                // Generic types like List<int>
                var typeArguments = genericName.TypeArgumentList.Arguments
                    .Select(GetTypeName)
                    .ToArray();
                return genericName.Identifier.Text + "<" + string.Join(", ", typeArguments) + ">";
            }
            else if (typeSyntax is ArrayTypeSyntax arrayType)
            {
                // Array types like int[]
                return GetTypeName(arrayType.ElementType) + "[]";
            }
            else if (typeSyntax is PointerTypeSyntax pointerType)
            {
                // Pointer types like int*
                return GetTypeName(pointerType.ElementType) + "*";
            }
            else if (typeSyntax is TupleTypeSyntax tupleType)
            {
                // Tuples like (int, string)
                var elements = tupleType.Elements
                    .Select(e => GetTypeName(e.Type))
                    .ToArray();
                return "(" + string.Join(", ", elements) + ")";
            }

            // Fallback for unhandled cases
            return typeSyntax.ToString();
        }

        /// <summary>
        /// Resolves a type name or an C# alias name to a runtime Type using Roslyn. Example type names: 
        /// "int"->"System.Int32"
        /// "string"->"System.String"
        /// "System.Int32"->"System.Int32"
        /// </summary>
        /// <param name="typeNameOrAliasName">type name(like System.Int32) or alias name(like int)</param>
        /// <returns></returns>
        public static Type ResolveTypeWithRoslyn(string typeNameOrAliasName)
        {
            // Minimal C# program that declares a variable of the given type
            string code = $"using System; class Dummy {{ {typeNameOrAliasName} dummyVariable; }}";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CSharpCompilation compilation = CSharpCompilation.Create(null,
                new[] { tree },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );
            var root = tree.GetRoot();
            var variableDeclaration = root.DescendantNodes()
                .OfType<VariableDeclarationSyntax>()
                .Single();
            var typeSyntax = variableDeclaration.Type;
            SemanticModel semanticModel = compilation.GetSemanticModel(tree);
            var typeSymbol = semanticModel.GetSymbolInfo(typeSyntax).Symbol;
            string typeFullName = typeSymbol.ContainingNamespace + "." + typeSymbol.MetadataName;
            return Type.GetType(typeFullName);
        }
    }
}
