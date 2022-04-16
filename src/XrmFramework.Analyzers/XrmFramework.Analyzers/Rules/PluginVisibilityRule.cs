using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable ArrangeObjectCreationWhenTypeEvident

namespace XrmFramework.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PluginVisibilityRuleAnalyzer : DiagnosticAnalyzer // CodeFixProvider
    {
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.PluginVisibilityTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.PluginVisibilityMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.PluginVisibilityDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Syntax";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.Xrm0003Id, Title, MessageFormat, Category, DiagnosticSeverity.Error, true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclaration)
            {
                var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);

                if (classSymbol == null || classSymbol.AllInterfaces.All(i => i.Name != "IPlugin"))
                {
                    return;
                }

                var allModifiers = new List<SyntaxToken>();

                foreach (var reference in classSymbol.DeclaringSyntaxReferences)
                {
                    var syntaxNode = reference.GetSyntax();

                    if (syntaxNode is ClassDeclarationSyntax classSyntaxNode)
                    {
                        allModifiers.AddRange(classSyntaxNode.Modifiers);
                    }
                }

                allModifiers.AddRange(classDeclaration.Modifiers);

                if (!allModifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                {
                    var diag = Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier.Text);
                    context.ReportDiagnostic(diag);
                }
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PluginVisibilityRuleCodeFixProvider)), Shared]
    public class PluginVisibilityRuleCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIds.Xrm0003Id);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var title = "Make class public";

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            if (root == null)
            {
                return;
            }

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;// Find the type declaration identified by the diagnostic.

            var token = root.FindToken(diagnosticSpan.Start).Parent;

            if (token == null)
            {
                return;
            }

            var declaration = token.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(title, _ => MakePublic(context.Document, root, declaration), title),
                diagnostic);

        }

        private Task<Solution> MakePublic(Document document, SyntaxNode root, ClassDeclarationSyntax typeDecl)
        {
            var modifiers = typeDecl.Modifiers;

            var listModifiers = new List<SyntaxToken> { SyntaxFactory.Token(SyntaxKind.PublicKeyword) };

            foreach (var token in modifiers)
            {
                if (token.IsKind(SyntaxKind.InternalKeyword) || token.IsKind(SyntaxKind.PrivateKeyword) || token.IsKind(SyntaxKind.ProtectedKeyword))
                {
                    continue;
                }
                listModifiers.Add(token);
            }

            var newModifiers = SyntaxFactory.TokenList(listModifiers);

            var newClassDeclaration = typeDecl.WithModifiers(newModifiers);

            var newRoot = root.ReplaceNode(typeDecl, newClassDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}
