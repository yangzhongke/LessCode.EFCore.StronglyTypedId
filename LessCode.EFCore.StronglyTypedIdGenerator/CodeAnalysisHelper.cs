using System.Linq;
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
                return qualifiedName.Right.Identifier.Text;
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
    }
}
