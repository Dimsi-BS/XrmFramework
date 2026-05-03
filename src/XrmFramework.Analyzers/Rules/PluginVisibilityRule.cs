using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
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

}
