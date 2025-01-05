using FluentAssertions.Primitives;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LessCode.EFCore.StronglyTypedIdGenerator.UnitTests
{
    public static class CodeAnalysisAssertionExtensions
    {
        public static AndConstraint<ObjectAssertions> HasOperator(this ObjectAssertions assertions, string operatorName)
        {
            SyntaxNode syntaxNode = (SyntaxNode)assertions.Subject;
            var operatorDeclarations = syntaxNode.DescendantNodes().OfType<OperatorDeclarationSyntax>();
            operatorDeclarations.Should().Contain(o => o.OperatorToken.Text == operatorName);
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> HasMethod(this ObjectAssertions assertions, string methodName)
        {
            SyntaxNode syntaxNode = (SyntaxNode)assertions.Subject;
            var methodDeclarations = syntaxNode.DescendantNodes().OfType<MethodDeclarationSyntax>().ToArray();
            methodDeclarations.Should().Contain(m => m.Identifier.Text == methodName);
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> HasProperty(this ObjectAssertions assertions, string propertyName, string propertyTypeName)
        {
            SyntaxNode syntaxNode = (SyntaxNode)assertions.Subject;
            var propertyDeclarationSyntaxValue = syntaxNode.DescendantNodes().OfType<PropertyDeclarationSyntax>().SingleOrDefault(e => e.Identifier.ValueText == propertyName);
            propertyDeclarationSyntaxValue.Should().NotBeNull();
            propertyDeclarationSyntaxValue.Type.ToFullString().Trim().Should().Be(propertyTypeName);
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> HasPublicConstructor(this ObjectAssertions assertions, params string[] argumentTypeNames)
        {
            SyntaxNode syntaxNode = (SyntaxNode)assertions.Subject;
            var constructorDeclarations = syntaxNode.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
            constructorDeclarations.Should().Contain(c=>c.Modifiers.Any(SyntaxKind.PublicKeyword)
            && c.ParameterList.Parameters.Select(p=>p.Type.ToString()).SequenceEqual(argumentTypeNames));
            return new AndConstraint<ObjectAssertions>(assertions);
        }
    }
}
