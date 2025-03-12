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
            Compilation inputCompilation = CSharpCompilation.Create(null,
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
            var classDeclaration = namespaceDeclaration.DescendantNodes().OfType<ClassDeclarationSyntax>().SingleOrDefault();
            classDeclaration.Should().NotBeNull();
            classDeclaration.Identifier.Text.Should().Be("CatId");
            classDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword).Should().BeTrue();

            //assert the two constructors
            classDeclaration.HasPublicConstructor().Should().BeTrue();
            classDeclaration.HasPublicConstructor("long").Should().BeTrue();

            //assert the property "Vaulue"
            classDeclaration.HasProperty("Value", "long").Should().BeTrue();

            //assert the two Converter fields(Converter1 and Converter2)
            var fieldDeclarations = classDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>().ToArray();
            fieldDeclarations.Should().HaveCountGreaterThanOrEqualTo(2);
            fieldDeclarations[0].Declaration.Variables.Single().Identifier.Text.Should().Match("Converter*");
            fieldDeclarations[1].Declaration.Variables.Single().Identifier.Text.Should().Match("Converter*");

            //assert the two methods "ToString", "GetHashCode" and "Equals"
            classDeclaration.HasMethod("ToString").Should().BeTrue();
            classDeclaration.HasMethod("GetHashCode").Should().BeTrue();
            classDeclaration.HasMethod("Equals");

            //assert the operators
            classDeclaration.HasOperator("==").Should().BeTrue();
            classDeclaration.HasOperator("!=").Should().BeTrue();
            classDeclaration.HasOperator(">").Should().BeTrue();
            classDeclaration.HasOperator("<").Should().BeTrue();
            classDeclaration.HasOperator(">=").Should().BeTrue();
            classDeclaration.HasOperator("<=").Should().BeTrue();
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
            Compilation inputCompilation = CSharpCompilation.Create(null,
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

            generatorResult.GeneratedSources[0].SyntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single().Identifier.Text.Should().Be("CatId");
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

        [Fact]
        public void Generate_IdClass_Successfully_Using_File_Scoped_Namespaces()
        {
            string sourceCode = @"
                namespace EntitiesProject2;
                [HasStronglyTypedId]
                public record Cat
                {
                    public CatId Id { get; set; }
                    public string Name { get; set; }
                }";
            Compilation inputCompilation = CSharpCompilation.Create(null,
                new[] { CSharpSyntaxTree.ParseText(sourceCode) });

            StronglyTypedIdCodeGenerator sut = new StronglyTypedIdCodeGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(sut);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
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
            var classDeclaration = namespaceDeclaration.DescendantNodes().OfType<ClassDeclarationSyntax>().SingleOrDefault();
            classDeclaration.Should().NotBeNull();
            classDeclaration.Identifier.Text.Should().Be("CatId");
        }

        [Theory]
        [InlineData("sbyte")]
        [InlineData("byte")]
        [InlineData("short")]
        [InlineData("ushort")]
        [InlineData("int")]
        [InlineData("uint")]
        [InlineData("long")]
        [InlineData("ulong")]
        [InlineData("Int64")]
        [InlineData("System.Int64")]
        [InlineData("Int32")]
        [InlineData("System.Int32")]
        public void Generate_IdClass_Successfully_When_With_IntegralType(string idType)
        {            
            string sourceCode = $@"
                namespace EntitiesProject2
                {{
                    [HasStronglyTypedId(typeof({idType}))]
                    public class Cat
                    {{
                        public CatId Id {{ get; set; }}
                        public string Name {{ get; set; }}
                    }}
                }}";
            Compilation inputCompilation = CSharpCompilation.Create(null, syntaxTrees: new[] { CSharpSyntaxTree.ParseText(sourceCode) });

            StronglyTypedIdCodeGenerator sut = new StronglyTypedIdCodeGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(sut);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
            GeneratorRunResult generatorResult = driver.GetRunResult().Results[0];
            generatorResult.Exception.Should().BeNull();

            //assert the namespace
            var generatedIdClassSyntaxTree = generatorResult.GeneratedSources.Single().SyntaxTree;
            var root = generatedIdClassSyntaxTree.GetRoot();
            var namespaceDeclaration = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().SingleOrDefault();

            //assert the struct
            var classDeclaration = namespaceDeclaration.DescendantNodes().OfType<ClassDeclarationSyntax>().SingleOrDefault();

            //assert the two constructors
            classDeclaration.HasPublicConstructor().Should().BeTrue();
            classDeclaration.HasPublicConstructor(idType).Should().BeTrue();

            //assert the property "Vaulue"
            classDeclaration.HasProperty("Value", idType).Should().BeTrue();
            //assert the operators
            classDeclaration.HasOperator("==").Should().BeTrue();
            classDeclaration.HasOperator("!=").Should().BeTrue();
            classDeclaration.HasOperator(">").Should().BeTrue();
            classDeclaration.HasOperator("<").Should().BeTrue();
            classDeclaration.HasOperator(">=").Should().BeTrue();
            classDeclaration.HasOperator("<=").Should().BeTrue();
        }

        [Theory]
        [InlineData("string")]
        [InlineData("String")]
        [InlineData("System.String")]
        public void Generate_IdClass_Successfully_When_With_StringId(string idType)
        {
            string sourceCode = $@"
                namespace EntitiesProject2
                {{
                    [HasStronglyTypedId(typeof({idType}))]
                    public class Cat
                    {{
                        public CatId Id {{ get; set; }}
                        public string Name {{ get; set; }}
                    }}
                }}";
            Compilation inputCompilation = CSharpCompilation.Create(null, syntaxTrees: new[] { CSharpSyntaxTree.ParseText(sourceCode) });

            StronglyTypedIdCodeGenerator sut = new StronglyTypedIdCodeGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(sut);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
            GeneratorRunResult generatorResult = driver.GetRunResult().Results[0];
            generatorResult.Exception.Should().BeNull();

            //assert the namespace
            var generatedIdClassSyntaxTree = generatorResult.GeneratedSources.Single().SyntaxTree;
            var root = generatedIdClassSyntaxTree.GetRoot();
            var namespaceDeclaration = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().SingleOrDefault();

            var classDeclaration = namespaceDeclaration.DescendantNodes().OfType<ClassDeclarationSyntax>().SingleOrDefault();

            //assert the two constructors
            classDeclaration.HasPublicConstructor().Should().BeTrue();
            classDeclaration.HasPublicConstructor(idType).Should().BeTrue();

            //assert the property "Vaulue"
            classDeclaration.HasProperty("Value", idType).Should().BeTrue();

            //assert the operators
            classDeclaration.HasOperator("==").Should().BeTrue();
            classDeclaration.HasOperator("!=").Should().BeTrue();
            classDeclaration.HasOperator(">").Should().BeFalse();
            classDeclaration.HasOperator("<").Should().BeFalse();
            classDeclaration.HasOperator(">=").Should().BeFalse();
            classDeclaration.HasOperator("<=").Should().BeFalse();
        }

        [Theory]
        [InlineData("Guid")]
        [InlineData("System.Guid")]
        public void Generate_IdClass_Successfully_When_With_GuidId(string idType)
        {
            string sourceCode = $@"
                namespace EntitiesProject2
                {{
                    [HasStronglyTypedId(typeof({idType}))]
                    public class Cat
                    {{
                        public CatId Id {{ get; set; }}
                        public string Name {{ get; set; }}
                    }}
                }}";
            Compilation inputCompilation = CSharpCompilation.Create(null, syntaxTrees: new[] { CSharpSyntaxTree.ParseText(sourceCode) });

            StronglyTypedIdCodeGenerator sut = new StronglyTypedIdCodeGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(sut);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
            GeneratorRunResult generatorResult = driver.GetRunResult().Results[0];
            generatorResult.Exception.Should().BeNull();
            //assert the namespace
            var generatedIdClassSyntaxTree = generatorResult.GeneratedSources.Single().SyntaxTree;
            var root = generatedIdClassSyntaxTree.GetRoot();
            var namespaceDeclaration = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().SingleOrDefault();

            var classDeclaration = namespaceDeclaration.DescendantNodes().OfType<ClassDeclarationSyntax>().SingleOrDefault();

            //assert the two constructors
            classDeclaration.HasPublicConstructor().Should().BeTrue();
            classDeclaration.HasPublicConstructor(idType).Should().BeTrue();

            //assert the property "Vaulue"
            classDeclaration.HasProperty("Value", idType).Should().BeTrue();

            //assert the operators
            classDeclaration.HasOperator("==").Should().BeTrue();
            classDeclaration.HasOperator("!=").Should().BeTrue();
            classDeclaration.HasOperator(">").Should().BeTrue();
            classDeclaration.HasOperator("<").Should().BeTrue();
            classDeclaration.HasOperator(">=").Should().BeTrue();
            classDeclaration.HasOperator("<=").Should().BeTrue();
        }
        
        //https://github.com/yangzhongke/LessCode.EFCore.StronglyTypedId/issues/9
        [Fact]
        public void Generate_IdClass_Successfully_When_Multiple_Attributes()
        {
            string sourceCode = @"
                using Microsoft.EntityFrameworkCore;
                namespace EntitiesProject2
                {
                    [HasStronglyTypedId]
                    [Index(nameof(String))]
                    public record Cat
                    {
                        public CatId Id { get; set; }
                        public string Name { get; set; }
                    }
                }";
            Compilation inputCompilation = CSharpCompilation.Create(null,
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
        }
    }
}