using LessCode.EFCore.StronglyTypedIdGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics;
using System.Linq;

namespace LessCode.EFCore.StronglyTypedId
{
    [Generator]
    class CodeGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                //GetSemanticModel() and DescendantNodesAndSelf() are less-performant,
                //so check the source code first to improve the performance of SourceGenerator
                if (syntaxTree.TryGetText(out var sourceText)&&
                    !sourceText.ToString().Contains("[HasStronglyTypedId"))
                {
                    continue;
                }
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                if (semanticModel == null)
                {
                    continue;
                }
                //[HasStronglyTypedId] can be applied to class, record, interface or struct.
                //TypeDeclarationSyntax is the base class of ClassDeclarationSyntax, InterfaceDeclarationSyntax, RecordDeclarationSyntax,and StructDeclarationSyntax.
                var classDefs = syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<TypeDeclarationSyntax>();
                foreach (var classDef in classDefs)
                {
                    ProcessClass(context, semanticModel, classDef);
                }
            }
        }

        private static bool HasEFCoreReference(GeneratorExecutionContext context)
        {
            return context.Compilation.ReferencedAssemblyNames.Any(r => r.Name == "Microsoft.EntityFrameworkCore");
        }

        private void ProcessClass(GeneratorExecutionContext context,SemanticModel semanticModel, TypeDeclarationSyntax classDef)
        {            
            var symbol = semanticModel.GetDeclaredSymbol(classDef);
            if (!(symbol is INamedTypeSymbol)) return;
            INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)symbol;
            var hasStronglyTypedIdAttr = namedTypeSymbol.GetAttributes()
                .SingleOrDefault(t => t.AttributeClass.Name == nameof(HasStronglyTypedIdAttribute));
            if (hasStronglyTypedIdAttr == null) return;
            var args = hasStronglyTypedIdAttr.ConstructorArguments;
            Debug.Assert(args.Length <= 1);
            string idDataType;
            if(args.Length<=0)//[HasStronglyTypedId]
            {
                idDataType = "long";
            }
            else
            {
                var arg0 = args.Single();
                switch(arg0.Kind)
                {
                    case TypedConstantKind.Type://[HasStronglyTypedId(typeof(Guid))]
                        idDataType = arg0.Value.ToString();
                        break;
                    case TypedConstantKind.Primitive://[HasStronglyTypedId("System.Int64")]
                        idDataType = arg0.Value.ToString();
                        break;
                    default:
                        throw new InvalidOperationException($"unsupported {arg0.Kind}");
                }
            }
            
            string ns = namedTypeSymbol.ContainingSymbol.ToDisplayString();
            string className = namedTypeSymbol.Name;
            IdModel model = new IdModel() { ClassName = className, NameSpace = ns, DataType = idDataType };
            var idClassTemplate = new IdClassTemplate();
            idClassTemplate.Model = model;
            string codeIdClass = idClassTemplate.TransformText();
            context.AddSource(className + "Id.generated.cs", codeIdClass);
            if(HasEFCoreReference(context))
            {
                var idValueConverterTemplate = new IdValueConverterTemplate();
                idValueConverterTemplate.Model = model;
                string codeValueConverter = idValueConverterTemplate.TransformText();
                context.AddSource(className + "IdValueConverter.generated.cs", codeValueConverter);
            }
            else
            {
                var projectName = context.Compilation.AssemblyName;
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("ZK001", "EF Core", $"Assembly Microsoft.EntityFrameworkCore is not added into the project '{projectName}', so ValueConverter types will not be automatically generated. Please add reference Microsoft.EntityFrameworkCore to '{projectName}' or write the ValueConverter types manually.", "", DiagnosticSeverity.Warning, true), null));
            }            
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //System.Diagnostics.Debugger.Launch();
        }
    }
}