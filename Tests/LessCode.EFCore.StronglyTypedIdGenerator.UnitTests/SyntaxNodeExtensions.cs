using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LessCode.EFCore.StronglyTypedIdGenerator.UnitTests
{
    public static class SyntaxNodeExtensions
    {
        public static bool HasOperator(this SyntaxNode syntaxNode, string operatorName)
        {
            var operatorDeclarations = syntaxNode.DescendantNodes().OfType<OperatorDeclarationSyntax>();
            var items = operatorDeclarations.Select(e => e.OperatorToken.Text).ToArray();
            return operatorDeclarations.Any(o => o.OperatorToken.Text == operatorName);
        }

        public static bool HasMethod(this SyntaxNode syntaxNode, string methodName)
        {
            var methodDeclarations = syntaxNode.DescendantNodes().OfType<MethodDeclarationSyntax>().ToArray();
            return methodDeclarations.Any(m => m.Identifier.Text == methodName);
        }

        public static bool HasProperty(this SyntaxNode syntaxNode, string propertyName, string propertyTypeName)
        {
            var propertyDeclarationSyntaxValue = syntaxNode.DescendantNodes().OfType<PropertyDeclarationSyntax>().SingleOrDefault(e => e.Identifier.ValueText == propertyName);
            if(propertyDeclarationSyntaxValue == null)
            {
                return false;
            }
            return propertyDeclarationSyntaxValue.Type.ToFullString().Trim()==propertyTypeName;
        }

        public static bool HasPublicConstructor(this SyntaxNode syntaxNode, params string[] argumentTypeNames)
        {
            var constructorDeclarations = syntaxNode.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
            return constructorDeclarations.Any(c=>c.Modifiers.Any(SyntaxKind.PublicKeyword)
            && c.ParameterList.Parameters.Select(p=>p.Type.ToString()).SequenceEqual(argumentTypeNames));
        }
    }
}
