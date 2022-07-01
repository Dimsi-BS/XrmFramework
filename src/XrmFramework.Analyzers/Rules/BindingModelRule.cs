// ReSharper disable ArrangeObjectCreationWhenTypeEvident

namespace XrmFramework.Analyzers
{/*
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BindingModelRuleAnalyzer : DiagnosticAnalyzer // CodeFixProvider
    {
        private static readonly LocalizableString Xrm0100Title = new LocalizableResourceString(nameof(Resources.Xrm0100_Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0100MessageFormat = new LocalizableResourceString(nameof(Resources.Xrm0100_MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0100Description = new LocalizableResourceString(nameof(Resources.Xrm0100_Description), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Syntax";

        private static readonly LocalizableString Xrm0101Title = new LocalizableResourceString(nameof(Resources.Xrm0101_Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0101MessageFormat = new LocalizableResourceString(nameof(Resources.Xrm0101_MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0101Description = new LocalizableResourceString(nameof(Resources.Xrm0101_Description), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Xrm0100 = new DiagnosticDescriptor(DiagnosticIds.Xrm0100Id, Xrm0100Title, Xrm0100MessageFormat, Category, DiagnosticSeverity.Error, true, description: Xrm0100Description);

        private static readonly DiagnosticDescriptor Xrm0101 = new DiagnosticDescriptor(DiagnosticIds.Xrm0101Id, Xrm0101Title, Xrm0101MessageFormat, Category, DiagnosticSeverity.Error, true, description: Xrm0101Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Xrm0100, Xrm0101);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeBindingModelClass, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethodCall, SyntaxKind.InvocationExpression);

            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var namedType = (INamedTypeSymbol) context.Symbol;

            if (namedType.AllInterfaces.All(i => i.Name != "IBindingModel"))
            {
                return;
            }

            if (!namedType.TryGetCrmEntityAttribute(out _))
            {
                return;
            }

            if (!HasToEntityMethod(namedType))
            {
                var locations = namedType.DeclaringSyntaxReferences.Select(s => ((ClassDeclarationSyntax) s.GetSyntax()).Identifier.GetLocation()).ToList();

                Diagnostic diagnostic;

                if (locations.Count() == 1)
                {
                    diagnostic = Diagnostic.Create(Xrm0101, locations.Single());
                }
                else
                {
                    diagnostic = Diagnostic.Create(Xrm0101, locations.First(), locations.Skip(1));
                }

                context.ReportDiagnostic(diagnostic);
            }
        }

        private bool HasToEntityMethod(INamedTypeSymbol symbol)
        {
            if (symbol.SpecialType == SpecialType.System_Object)
            {
                return false;
            }

            return HasToEntityMethod(symbol.BaseType) || symbol.GetMembers("ToEntity").Any();
        }

        private void AnalyzeMethodCall(SyntaxNodeAnalysisContext context)
        {
            var invocationNode = (InvocationExpressionSyntax) context.Node;

            if (!(invocationNode.Expression is MemberAccessExpressionSyntax memberAccessSyntax))
            {
                return;
            }

            if (!(memberAccessSyntax.Name is GenericNameSyntax genericNameSyntax))
            {
                return;
            }

            if (genericNameSyntax.Identifier.Text != "GetById")
            {
                return;
            }


            if (genericNameSyntax.TypeArgumentList.Arguments.Count != 1)
            {
                return;
            }

            var typeSyntax = genericNameSyntax.TypeArgumentList.Arguments.Single();

            if (!(typeSyntax is IdentifierNameSyntax identifierNameSyntax))
            {
                return;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(invocationNode).Symbol;

            if (symbol == null || !(symbol is IMethodSymbol methodSymbol))
            {
                return;
            }

            var typeArgument = methodSymbol.TypeArguments.Single();

            if (!typeArgument.GetMembers("GetRetrieveRequest").Any())
            {
                //var diag = Diagnostic.Create(Xrm0101, identifierNameSyntax.Identifier.GetLocation(), identifierNameSyntax.Identifier.Text);
                //context.ReportDiagnostic(diag);
            }
        }

        private static void AnalyzeBindingModelClass(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is ClassDeclarationSyntax classDeclaration)) return;

            var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);

            if (classSymbol.AllInterfaces.All(i => i.Name != "IBindingModel"))
            {
                return;
            }

            var modifiers = classDeclaration.Modifiers;

            if (modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword)))
            {
                return;
            }

            if (modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            {
                return;
            }

            var diag = Diagnostic.Create(Xrm0100, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier.Text);
            context.ReportDiagnostic(diag);
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BindingModelRuleAnalyzer)), Shared]
    public class BindingModelRuleCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIds.Xrm0100Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var title = "Make the class partial";

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;// Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(title, c => AddPartial(context.Document, root, declaration, c), equivalenceKey: title),
                diagnostic);

        }

        private Task<Document> AddPartial(Document document, SyntaxNode root, ClassDeclarationSyntax typeDecl, CancellationToken cancellationToken)
        {
            var newClassDeclaration = typeDecl.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

            var newRoot = root.ReplaceNode(typeDecl, newClassDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return Task.FromResult(newDocument);
        }
    }
*/
}
