using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using LessCode.EFCore.StronglyTypedId;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace LessCode.EFCore.StronglyTypedIdGenerator.UnitTests
{
    public class StronglyTypedIdCodeGeneratorShould
    {
        [Fact]
        public void Generate_IdClass_Successfully_When_HasStronglyTypedId_Is_Parameterless_And_NoReference_To_EFCore()
        {
            string sourceCode = @"
                namespace EntitiesProject2
                {
                    [HasStronglyTypedId]
                    public record Cat
                    {
                        public CatId Id { get; set; }
                        public string Name { get; set; }
                    }
                }";
            Compilation inputCompilation = CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(sourceCode) });

            StronglyTypedIdCodeGenerator sut = new StronglyTypedIdCodeGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(sut);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Severity.Should().Be(DiagnosticSeverity.Warning);
            diagnostics[0].GetMessage().Should().Match("*Assembly Microsoft.EntityFrameworkCore is not added into the project*ValueConverter types will not be automatically generated*Please add reference Microsoft.EntityFrameworkCore to*or write the ValueConverter types manually*");
            GeneratorRunResult generatorResult = driver.GetRunResult().Results[0];
            generatorResult.GeneratedSources.Should().HaveCount(1);
            generatorResult.Exception.Should().BeNull();

            //assert the namespace
            var generatedIdClassSyntaxTree = generatorResult.GeneratedSources.Single().SyntaxTree;
            var root = generatedIdClassSyntaxTree.GetRoot();
            var namespaceDeclaration = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().SingleOrDefault();
            namespaceDeclaration.Should().NotBeNull();
            namespaceDeclaration.Name.ToString().Should().Be("EntitiesProject2");

            //assert the struct
            var structDeclaration = namespaceDeclaration.DescendantNodes().OfType<StructDeclarationSyntax>().SingleOrDefault();
            structDeclaration.Should().NotBeNull();
            structDeclaration.Identifier.Text.Should().Be("CatId");
            structDeclaration.Modifiers.Any(SyntaxKind.ReadOnlyKeyword).Should().BeTrue();

            //assert the two constructors
            structDeclaration.Should().HasPublicConstructor();
            structDeclaration.Should().HasPublicConstructor("long");

            //assert the property "Vaulue"
            structDeclaration.Should().HasProperty("Value", "long");

            //assert the two Converter fields(Converter1 and Converter2)
            var fieldDeclarations = structDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>().ToArray();
            fieldDeclarations.Should().HaveCountGreaterThanOrEqualTo(2);
            fieldDeclarations[0].Declaration.Variables.Single().Identifier.Text.Should().Match("Converter*");
            fieldDeclarations[1].Declaration.Variables.Single().Identifier.Text.Should().Match("Converter*");

            //assert the two methods "ToString", "GetHashCode" and "Equals"
            structDeclaration.Should().HasMethod("ToString");
            structDeclaration.Should().HasMethod("GetHashCode");
            structDeclaration.Should().HasMethod("Equals");

            //assert the operators
            structDeclaration.Should().HasOperator("==");
            structDeclaration.Should().HasOperator("!=");
            structDeclaration.Should().HasOperator(">");
            structDeclaration.Should().HasOperator("<");
            structDeclaration.Should().HasOperator(">=");
            structDeclaration.Should().HasOperator("<=");
        }

        [Fact]
        public void Generate_IdValueConverter_Successfully_WithReference_To_EFCore()
        {
            string sourceCode = @"
                namespace EntitiesProject2
                {
                    [HasStronglyTypedId]
                    public record Cat
                    {
                        public CatId Id { get; set; }
                        public string Name { get; set; }
                    }
                }";
            Compilation inputCompilation = CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(sourceCode) },
                references:new MetadataReference[] { MetadataReference.CreateFromFile(typeof(DbContext).Assembly.Location)});

            StronglyTypedIdCodeGenerator sut = new StronglyTypedIdCodeGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(sut);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
            outputCompilation.SyntaxTrees.Should().HaveCount(3);
            diagnostics.Should().HaveCount(0);
            GeneratorRunResult generatorResult = driver.GetRunResult().Results[0];
            generatorResult.GeneratedSources.Should().HaveCount(2);
            generatorResult.Exception.Should().BeNull();

            generatorResult.GeneratedSources[0].SyntaxTree.GetRoot().DescendantNodes().OfType<StructDeclarationSyntax>().Single().Identifier.Text.Should().Be("CatId");
            //assert the IdValueConverter.cs

            var idValueConverterRootNode =  generatorResult.GeneratedSources[1].SyntaxTree.GetRoot();

            //assert the namespace
            var namespaceDeclaration = idValueConverterRootNode.DescendantNodes().OfType<NamespaceDeclarationSyntax>().SingleOrDefault();
            namespaceDeclaration.Should().NotBeNull();
            namespaceDeclaration.Name.ToString().Should().Be("EntitiesProject2");

            //assert the class
            var idValueConverterNode = namespaceDeclaration.DescendantNodes().OfType<ClassDeclarationSyntax>().SingleOrDefault();
            idValueConverterNode.Identifier.Text.Should().Be("CatIdValueConverter");
            idValueConverterNode.Modifiers.Any(SyntaxKind.PublicKeyword).Should().BeTrue();

            //assert the attribute of [StronglyTypedIdValueConverter(typeof(CatId))]
            idValueConverterNode.AttributeLists.Should().HaveCount(1);
            idValueConverterNode.AttributeLists[0].Attributes.Should().HaveCount(1);
            idValueConverterNode.AttributeLists[0].Attributes[0].Name.ToString().Should().Be("StronglyTypedIdValueConverter");
            idValueConverterNode.AttributeLists[0].Attributes[0].ArgumentList.Arguments.Should().HaveCount(1);
            idValueConverterNode.AttributeLists[0].Attributes[0].ArgumentList.Arguments[0].Expression.ToString().Should().Be("typeof(CatId)");

            //assert the base class
            idValueConverterNode.BaseList.Types.Should().HaveCount(1);
            idValueConverterNode.BaseList.Types[0].Type.ToString().Should().Be("ValueConverter<CatId, long>");
        }
    }
}