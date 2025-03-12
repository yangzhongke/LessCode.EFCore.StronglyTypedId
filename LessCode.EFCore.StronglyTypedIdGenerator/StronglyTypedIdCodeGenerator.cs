using LessCode.EFCore.StronglyTypedIdGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LessCode.EFCore.StronglyTypedId
{
    [Generator]
    public class StronglyTypedIdCodeGenerator : ISourceGenerator
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
                //[HasStronglyTypedId] can be applied to class, record, interface or struct.
                //TypeDeclarationSyntax is the base class of ClassDeclarationSyntax, InterfaceDeclarationSyntax, RecordDeclarationSyntax,and StructDeclarationSyntax.
                var classDefs = syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<TypeDeclarationSyntax>();
                foreach (var classDef in classDefs)
                {
                    ProcessClass(context, classDef);
                }
            }
        }

        private static bool HasEFCoreReference(GeneratorExecutionContext context)
        {
            return context.Compilation.ReferencedAssemblyNames.Any(r => r.Name == "Microsoft.EntityFrameworkCore");
        }

        private void ProcessClass(GeneratorExecutionContext context, TypeDeclarationSyntax classDef)
        {
            string ns = ((BaseNamespaceDeclarationSyntax)classDef.Parent).Name.GetText().ToString();
            string className = classDef.Identifier.Text;
            var hasStronglyTypedIdAttr = classDef.DescendantNodes().OfType<AttributeSyntax>().Single(e=>e.Name.GetText().ToString()== nameof(HasStronglyTypedIdAttribute) || e.Name.GetText().ToString() == nameof(HasStronglyTypedIdAttribute).Replace("Attribute", ""));
            IEnumerable<AttributeArgumentSyntax> args = new AttributeArgumentSyntax[0]; 
            if (hasStronglyTypedIdAttr.ArgumentList != null)
                args = hasStronglyTypedIdAttr.ArgumentList.Arguments;
            Debug.Assert(args.Count() <= 1);
            string idDataType;
            if(args.Count() <= 0)//[HasStronglyTypedId]
            {
                idDataType = "long";
            }
            else
            {
                var arg0 = args.Single();
                TypeSyntax type = ((TypeOfExpressionSyntax)arg0.Expression).Type;
                idDataType = CodeAnalysisHelper.GetTypeName(type);
            }
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