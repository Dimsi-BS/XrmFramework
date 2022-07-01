#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using XrmFramework.Analyzers.Extensions;
using XrmFramework.Analyzers.Helpers;

namespace XrmFramework.Analyzers.Generators
{
    [Generator]
    public class LoggedServicesSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var interfaceDeclaration = context.SyntaxProvider
                .CreateSyntaxProvider(IsIServiceSyntax, FilterIsAssignableToIService)
                .Where(s => s is not null).Collect();

            var serviceImplementationDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(IsIServiceClassSyntax, FilterIsAssignableToIService)
                .Where(s => s is not null).Collect();

            var interfacesAndImplementations = serviceImplementationDeclarations.Combine(interfaceDeclaration);

            context.RegisterSourceOutput(interfacesAndImplementations, (spc, source) => Execute(source.Left, source.Right, spc));
        }

        private void Execute(ImmutableArray<INamedTypeSymbol?> implementations, ImmutableArray<INamedTypeSymbol?> services, SourceProductionContext context)
        {
            var distinctServices = services.Distinct(SymbolEqualityComparer.Default).Cast<INamedTypeSymbol>().ToList();

            var generator = new LoggedServiceCodeGenerator();

            foreach (var service in distinctServices)
            {
                var code = generator.Generate(service);

                var typeNamespace = service.ContainingNamespace.IsGlobalNamespace
                       ? null
                       : $"{service.ContainingNamespace}.";

                context.AddSource($"{typeNamespace}{service.Name}.logged.cs", code);
            }


            var content = new InternalDependencyGenerator().GetInternalDependencyFileContent(services, implementations);
            context.AddSource("DependencyInjection.cs", SourceText.From(content, Encoding.UTF8));
        }

        private bool IsIServiceSyntax(SyntaxNode node, CancellationToken cancellationToken)
        {
            if (node is not InterfaceDeclarationSyntax interfaceSyntax)
            {
                return false;
            }

            return interfaceSyntax.Identifier.Text == "IService" ||
                interfaceSyntax.BaseList != null && interfaceSyntax.BaseList.Types.Any();
        }

        private bool IsIServiceClassSyntax(SyntaxNode node, CancellationToken cancellationToken)
        {
            if (node is not ClassDeclarationSyntax classDeclaration)
            {
                return false;
            }

            if (classDeclaration.Modifiers.All(s => !s.IsKind(SyntaxKind.PublicKeyword)) ||
                classDeclaration.Modifiers.Any(s => s.IsKind(SyntaxKind.AbstractKeyword)))
            {
                return false;
            }

            if (!classDeclaration.BaseList?.Types.Any() ?? true)
            {
                return false;
            }

            return true;
        }

        private INamedTypeSymbol? FilterIsAssignableToIService(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            var baseTypeDeclarationSyntax = (BaseTypeDeclarationSyntax)context.Node;

            var symbol = context.SemanticModel.GetDeclaredSymbol(baseTypeDeclarationSyntax, cancellationToken);

            if (symbol == null)
            {
                return null;
            }

            if (!symbol.IsAssignableToIService(baseTypeDeclarationSyntax is ClassDeclarationSyntax))
            {
                return null;
            }

            return symbol;
        }
    }
}
