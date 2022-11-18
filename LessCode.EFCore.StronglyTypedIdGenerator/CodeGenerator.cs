using LessCode.EFCore.StronglyTypedIdGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                if (semanticModel == null)
                {
                    continue;
                }
                var namespaceNodes = syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<NamespaceDeclarationSyntax>();
                foreach (var namespaceNode in namespaceNodes)
                {
                    string nsValue = ((IdentifierNameSyntax)namespaceNode.Name).Identifier.Text;
                    var classDefs = namespaceNode.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>();
                    foreach(var classDef in classDefs)
                    {
                        ProcessClass(context, semanticModel, classDef);
                    }
                }
            }
        }

        private void ProcessClass(GeneratorExecutionContext context,SemanticModel semanticModel, ClassDeclarationSyntax classDef)
        {            
            var symbol = semanticModel.GetDeclaredSymbol(classDef);
            if (!(symbol is INamedTypeSymbol)) return;
            INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)symbol;
            var hasStronglyTypedIdAttr = namedTypeSymbol.GetAttributes().SingleOrDefault(t=>t.AttributeClass.ToString()== "System.HasStronglyTypedIdAttribute");
            var args = hasStronglyTypedIdAttr.ConstructorArguments;
            if(args.Length<=0)
            {
                string ns = namedTypeSymbol.ContainingNamespace.Name;
                string className = namedTypeSymbol.Name;
                IdModel model = new IdModel() { ClassName=className,NameSpace=ns,DateType="long"};
                IdClassTemplate idClassTemplate = new IdClassTemplate();
                idClassTemplate.Model = model;
                context.AddSource(className + "Id.generated.cs", idClassTemplate.TransformText());

                /*
                StringBuilder sbIdClass = new StringBuilder();
                sbIdClass.Append("namespace ").Append(ns).AppendLine("{");
                sbIdClass.Append("public class ").Append(className).AppendLine("Id {");
                sbIdClass.AppendLine("}");
                sbIdClass.AppendLine("}");
                context.AddSource(className + "Id.generated.cs", sbIdClass.ToString());*/
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
        }
    }
}