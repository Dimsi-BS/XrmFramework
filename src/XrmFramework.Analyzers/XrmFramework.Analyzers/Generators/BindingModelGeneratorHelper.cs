using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using XrmFramework.Analyzers.Utils;

namespace XrmFramework.Generators
{
    public static class BindingModelGeneratorHelper
    {

        public static SyntaxNode GenerateCode(SyntaxTree syntaxTree, SemanticModel semanticModel, IBindingModelSyntaxGenerator generator)
        {
            var namespaceDeclarations = syntaxTree.GetRoot().DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            return generator.AddUsings(syntaxTree.GetCompilationUnitRoot()
                .ReplaceNodes(namespaceDeclarations, (n1, n2) => ComputeNewNamespaceDeclarationNode(n1, semanticModel, generator)))
                .CheckAndAddUsing("System.Diagnostics")
                .CheckAndAddUsing("System.CodeDom.Compiler")
                .WithAdditionalAnnotations(Formatter.Annotation);
        }

        /// <summary>
        ///   Takes a namespace declaration and returns a new namespace declaration containing only
        ///   the ServiceContract interfaces, converted to an asynchronous version
        /// </summary>
        /// <param name="originalNamespace">The namespace declaration to replace</param>
        /// <param name="semanticModel"></param>
        /// <returns></returns>
        private static NamespaceDeclarationSyntax ComputeNewNamespaceDeclarationNode(NamespaceDeclarationSyntax originalNamespace, SemanticModel semanticModel, IBindingModelSyntaxGenerator generator)
        {
            var classDeclarations =
                originalNamespace.DescendantNodes().OfType<ClassDeclarationSyntax>();

            var newNamespaceDeclaration = originalNamespace.WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>());

            foreach (var classDeclaration in classDeclarations)
            {
                var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

                var modelDefinition = ModelDefinition.GetModelDefinition(classSymbol);

                if (modelDefinition == null)
                {
                    continue;
                }

                var newClassDeclaration = SyntaxFactory.ClassDeclaration(classDeclaration.Identifier.Text).WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.PartialKeyword)));

                newClassDeclaration = newClassDeclaration.AddMembers(GetProtectedFillEntityMethodDeclaration(modelDefinition));

                var toEntityMethodDeclaration = GetPublicToEntityMethodDeclaration(modelDefinition);

                if (toEntityMethodDeclaration != null)
                {
                    newClassDeclaration = newClassDeclaration.AddMembers(toEntityMethodDeclaration);
                }

                if (!modelDefinition.IsAbstract)
                {
                    newClassDeclaration = newClassDeclaration.AddMembers(
                        GetToBindingModelMethodDeclaration(modelDefinition, generator));
                }

                newNamespaceDeclaration = newNamespaceDeclaration.AddMembers(newClassDeclaration);
            }

            return newNamespaceDeclaration;
        }


        private static MemberDeclarationSyntax GetToBindingModelMethodDeclaration(ModelDefinition modelDefinition, IBindingModelSyntaxGenerator generator)
        {
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(modelDefinition.Name), "ToBindingModel")
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                .AddParameter("record", "Entity");

            var syntaxGenerator = generator.GetSyntaxGenerator();

            var body = SyntaxFactory.Block()
                .AddStatements(SyntaxFactory.ParseStatement("if (record == null)\r\n{\r\nreturn null;\r\n}\r\n").WithAdditionalAnnotations(Formatter.Annotation))
                .AddStatements(syntaxGenerator.GetAllSubEntitySyntaxes(modelDefinition).ToArray())
                .AddStatements(SyntaxFactory.ParseStatement($"var model = new {modelDefinition.Name}();\r\n\r\n"))
                .AddStatements(GetToBindingModelMappings(modelDefinition, syntaxGenerator).ToArray())
                .AddStatements(SyntaxFactory.ParseStatement("return model;\r\n"));

            return method.WithBody(body).WithAdditionalAnnotations(Formatter.Annotation);
        }

        private static MemberDeclarationSyntax GetPublicToEntityMethodDeclaration(ModelDefinition modelDefinition)
        {
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("Entity"), "ToEntity")
                .AddParameter("service", "IOrganizationService")
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

            var body = SyntaxFactory.Block();

            if (modelDefinition.ParentModel != null)
            {
                return null;
            }

            body = body
                .AddStatements(SyntaxFactory.ParseStatement($"var record = new Entity({modelDefinition.EntityDefinition.Name}.EntityName, Id);\r\n\r\n"))
                .AddStatements(SyntaxFactory.ParseStatement("FillEntity(service, record);\r\n"))
                .AddStatements(SyntaxFactory.ParseStatement("return record;\r\n"));

            return method.WithBody(body).WithAdditionalAnnotations(Formatter.Annotation);
        }

        private static MemberDeclarationSyntax GetProtectedFillEntityMethodDeclaration(ModelDefinition modelDefinition)
        {
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "FillEntity")
                .AddParameter("service", "IOrganizationService")
                .AddParameter("record", "Entity")
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)));

            var body = SyntaxFactory.Block();

            if (modelDefinition.ParentModel != null)
            {
                method = method.AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
                body = body.AddStatements(SyntaxFactory.ParseStatement($"base.FillEntity(service, record);\r\n\r\n"));
            }
            else
            {
                method = method.AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword));
            }

            body = body
                .AddStatements(GetToEntityMappings(modelDefinition).ToArray());

            return method.WithBody(body).WithAdditionalAnnotations(Formatter.Annotation);
        }

        private static ICollection<StatementSyntax> GetToEntityMappings(ModelDefinition modelDefinition)
        {
            var list = new List<StatementSyntax>();

            foreach (var attribute in modelDefinition.Attributes)
            {
                if (!attribute.IsValidForUpdate)
                {
                    continue;
                }

                var statementsSyntax = attribute.GetFillEntitySyntax();


                list.AddRange(statementsSyntax);
            }

            return list;
        }

        private static ICollection<StatementSyntax> GetToBindingModelMappings(ModelDefinition modelDefinition, SubEntitySyntaxGenerator syntaxGenerator)
        {
            var list = new List<StatementSyntax>();

            foreach (var attribute in modelDefinition.AllAttributes)
            {
                var statementsSyntax = attribute.GetToBindingModelSyntax(syntaxGenerator);

                list.AddRange(statementsSyntax);
            }

            return list;
        }
    }
}
