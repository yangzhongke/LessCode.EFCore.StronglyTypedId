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
                    case TypedConstantKind.Type:
                        idDataType = arg0.Value.ToString();
                        break;
                    case TypedConstantKind.Primitive:
                        idDataType = arg0.Value.ToString();
                        break;
                    default:
                        throw new InvalidOperationException($"unsupported {arg0.Kind}");
                }
            }
            string ns = namedTypeSymbol.ContainingNamespace.Name;
            string className = namedTypeSymbol.Name;
            IdModel model = new IdModel() { ClassName = className, NameSpace = ns, DataType = idDataType };
            IdClassTemplate idClassTemplate = new IdClassTemplate();
            idClassTemplate.Model = model;
            context.AddSource(className + "Id.generated.cs", idClassTemplate.TransformText());
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //System.Diagnostics.Debugger.Launch();
        }
    }
}