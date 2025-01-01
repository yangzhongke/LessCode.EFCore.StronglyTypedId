using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Reflection;
using LessCode.EFCore.StronglyTypedId;
using FluentAssertions;

namespace LessCode.EFCore.StronglyTypedIdGenerator.UnitTests
{
    public class StronglyTypedIdCodeGeneratorShould
    {
        [Fact]
        public void Generate_Successfully_When_HasStronglyTypedId_Is_Parameterless_And_NoReference_To_EFCore()
        {
            // Create the 'input' compilation that the generator will act on
            Compilation inputCompilation = CreateCompilation(@"
namespace EntitiesProject2
{
    [HasStronglyTypedId]
    public record Cat
    {
        public CatId Id { get; set; }
        public string Name { get; set; }
    }
}");


            StronglyTypedIdCodeGenerator sut = new StronglyTypedIdCodeGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(sut);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
            diagnostics.IsEmpty.Should().BeFalse();
            outputCompilation.SyntaxTrees.Should().HaveCount(2);
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Severity.Should().Be(DiagnosticSeverity.Warning);
            diagnostics[0].GetMessage().Should().Match("*Assembly Microsoft.EntityFrameworkCore is not added into the project*ValueConverter types will not be automatically generated*Please add reference Microsoft.EntityFrameworkCore to*or write the ValueConverter types manually*");
            GeneratorDriverRunResult runResult = driver.GetRunResult();
            runResult.GeneratedTrees.Should().HaveCount(1);
            GeneratorRunResult generatorResult = runResult.Results[0];
            generatorResult.Generator.Should().Be(sut);
            generatorResult.GeneratedSources.Should().HaveCount(1);
            generatorResult.Exception.Should().BeNull();
        }

        private static Compilation CreateCompilation(string source)
            => CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }
}