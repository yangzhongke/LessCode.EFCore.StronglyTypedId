using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq.Expressions;

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
        public static Type ResolveTypeFromName(string typeNameOrAliasName)
        {
            Type type = ResolveTypeFromPredefinedAlias(typeNameOrAliasName);
            if (type != null)
            {
                return type;
            }
            type = Type.GetType(typeNameOrAliasName);
            if(type == null)
            {
                type = Type.GetType("System."+typeNameOrAliasName);
            }
            if(type == null)
            {
                throw new ArgumentException(nameof(typeNameOrAliasName),$"{typeNameOrAliasName} not found");
            }
            return type;
        }

        private static Type ResolveTypeFromPredefinedAlias(string alias)
        {
            switch (alias)
            {
                case "object": return typeof(object);
                case "string": return typeof(string);
                case "bool": return typeof(bool);
                case "byte": return typeof(byte);
                case "sbyte": return typeof(sbyte);
                case "short": return typeof(short);
                case "ushort": return typeof(ushort);
                case "int": return typeof(int);
                case "uint": return typeof(uint);
                case "long": return typeof(long);
                case "ulong": return typeof(ulong);
                case "float": return typeof(float);
                case "double": return typeof(double);
                case "decimal": return typeof(decimal);
                case "char": return typeof(char);
                case "void": return typeof(void);
                default: return null;
            }
        }

        public static bool SupportsCompare(Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type);
        }

        public static bool SupportsCompareToType(Type type, Type otherType)
        {
			return typeof(IComparable<>).MakeGenericType(otherType).IsAssignableFrom(type);
		}

        public static bool SupportsBinaryOperator(Type type, ExpressionType operation)
        {
            try
            {
                // Create two parameters of the given type
                var left = Expression.Parameter(type, "left");
                var right = Expression.Parameter(type, "right");

                // Try to create a binary expression for the specified operator
                var binaryExpression = Expression.MakeBinary(operation, left, right);

                // If we successfully create the expression, the operator exists
                return binaryExpression != null;
            }
            catch(InvalidOperationException)
            {                
                // If an exception occurs, the operator is not supported
                return false;
            }
        }
    }
}
