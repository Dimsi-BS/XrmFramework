// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XrmFramework.Analyzers
{
    /// <summary>
    /// Code fix pour XRM0300.
    /// <list type="bullet">
    ///   <item>
    ///     <term>Plugin / IPlugin</term>
    ///     <description>
    ///       Ajoute <c>IDateTimeProvider dateTimeProvider</c> comme paramètre de la méthode
    ///       contenante et remplace <c>DateTime.Now/UtcNow</c> par <c>dateTimeProvider.Now/UtcNow</c>.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>IService — constructeur classique</term>
    ///     <description>
    ///       Ajoute <c>IDateTimeProvider dateTimeProvider</c> au constructeur, crée le champ
    ///       <c>private readonly IDateTimeProvider _dateTimeProvider;</c>, assigne dans le corps
    ///       du constructeur et remplace <c>DateTime.Now/UtcNow</c> par <c>_dateTimeProvider.Now/UtcNow</c>.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>IService — primary constructor</term>
    ///     <description>
    ///       Ajoute <c>IDateTimeProvider dateTimeProvider</c> au <c>ParameterList</c> de la classe,
    ///       crée le champ avec initialisation inline
    ///       <c>private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;</c>
    ///       et remplace <c>DateTime.Now/UtcNow</c> par <c>_dateTimeProvider.Now/UtcNow</c>.
    ///     </description>
    ///   </item>
    /// </list>
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DateTimeUsageCodeFixProvider)), Shared]
    public class DateTimeUsageCodeFixProvider : CodeFixProvider
    {
        private const string DateTimeProviderInterface = "IDateTimeProvider";
        private const string DefaultParamName          = "dateTimeProvider";
        private const string DefaultFieldName          = "_dateTimeProvider";

        public override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(DiagnosticIds.Xrm0300Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        // ── Registration ──────────────────────────────────────────────────────

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);
            if (root == null) return;

            var semanticModel = await context.Document
                .GetSemanticModelAsync(context.CancellationToken)
                .ConfigureAwait(false);
            if (semanticModel == null) return;

            var diagnostic    = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var memberAccess = root
                .FindToken(diagnosticSpan.Start)
                .Parent?
                .AncestorsAndSelf()
                .OfType<MemberAccessExpressionSyntax>()
                .FirstOrDefault();
            if (memberAccess == null) return;

            var memberName = memberAccess.Name.Identifier.Text; // "Now" ou "UtcNow"

            var classDecl = memberAccess
                .Ancestors()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();
            if (classDecl == null) return;

            var classSymbol = semanticModel.GetDeclaredSymbol(classDecl);
            if (classSymbol == null) return;

            bool isService = classSymbol.AllInterfaces.Any(i =>
                i.Name == "IService" &&
                i.ContainingNamespace?.ToDisplayString() == "XrmFramework");

            if (isService)
            {
                bool hasPrimaryCtor = classDecl.ParameterList != null;

                string title = hasPrimaryCtor
                    ? $"Ajouter IDateTimeProvider au primary constructor → {DefaultFieldName}.{memberName}"
                    : $"Injecter IDateTimeProvider dans le constructeur → {DefaultFieldName}.{memberName}";

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: title,
                        createChangedDocument: ct =>
                            ApplyServiceFixAsync(
                                context.Document, root, memberAccess, memberName, classDecl, ct),
                        equivalenceKey: $"XRM0300_Service_{memberName}"),
                    diagnostic);
            }
            else
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: $"Utiliser IDateTimeProvider.{memberName} (paramètre de méthode)",
                        createChangedDocument: ct =>
                            ApplyPluginFixAsync(
                                context.Document, root, memberAccess, memberName, ct),
                        equivalenceKey: $"XRM0300_Plugin_{memberName}"),
                    diagnostic);
            }
        }

        // ── Plugin fix : paramètre de méthode ────────────────────────────────

        private static Task<Document> ApplyPluginFixAsync(
            Document document,
            SyntaxNode root,
            MemberAccessExpressionSyntax memberAccess,
            string memberName,
            CancellationToken _)
        {
            var containingMethod = memberAccess
                .Ancestors()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault();

            string paramName  = DefaultParamName;
            bool   needsParam = true;

            if (containingMethod != null)
            {
                var existing = FindExistingProviderParam(containingMethod.ParameterList);
                if (existing != null)
                {
                    paramName  = existing.Identifier.Text;
                    needsParam = false;
                }
            }

            var newRoot = root.ReplaceNode(memberAccess, BuildMemberAccess(paramName, memberName, memberAccess));

            if (!needsParam || containingMethod == null)
                return Task.FromResult(document.WithSyntaxRoot(newRoot));

            var updatedMethod = FindUpdatedMethod(newRoot, containingMethod);
            if (updatedMethod == null) return Task.FromResult(document.WithSyntaxRoot(newRoot));

            newRoot = newRoot.ReplaceNode(
                updatedMethod,
                updatedMethod.WithParameterList(
                    updatedMethod.ParameterList.AddParameters(BuildParameter(DefaultParamName))));

            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }

        // ── Service fix : dispatche selon le type de constructeur ─────────────

        private static Task<Document> ApplyServiceFixAsync(
            Document document,
            SyntaxNode root,
            MemberAccessExpressionSyntax memberAccess,
            string memberName,
            ClassDeclarationSyntax classDeclaration,
            CancellationToken ct)
        {
            return classDeclaration.ParameterList != null
                ? ApplyServicePrimaryCtorFixAsync(document, root, memberAccess, memberName, classDeclaration, ct)
                : ApplyServiceRegularCtorFixAsync(document, root, memberAccess, memberName, classDeclaration, ct);
        }

        // ── Service fix — primary constructor ─────────────────────────────────
        //
        //  Avant :
        //    public class MyService(IServiceContext context) : DefaultService(context) { ... }
        //
        //  Après :
        //    public class MyService(IServiceContext context, IDateTimeProvider dateTimeProvider)
        //        : DefaultService(context)
        //    {
        //        private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
        //        ...
        //    }

        private static Task<Document> ApplyServicePrimaryCtorFixAsync(
            Document document,
            SyntaxNode root,
            MemberAccessExpressionSyntax memberAccess,
            string memberName,
            ClassDeclarationSyntax classDeclaration,
            CancellationToken _)
        {
            // Cherche un champ IDateTimeProvider déjà existant
            var existingField = FindExistingProviderField(classDeclaration);
            string fieldName    = existingField?.Declaration.Variables.First().Identifier.Text
                                  ?? DefaultFieldName;
            bool   needNewField = existingField == null;

            // 1. Remplace DateTime.Now/UtcNow → _dateTimeProvider.Now/UtcNow
            var newRoot = root.ReplaceNode(memberAccess, BuildMemberAccess(fieldName, memberName, memberAccess));

            if (!needNewField)
                return Task.FromResult(document.WithSyntaxRoot(newRoot));

            // 2. Retrouve la classe dans le nouveau root
            var updatedClass = FindUpdatedClass(newRoot, classDeclaration);
            if (updatedClass == null) return Task.FromResult(document.WithSyntaxRoot(newRoot));

            // Vérifie si le primary constructor a déjà un paramètre IDateTimeProvider
            var existingPrimaryParam = FindExistingProviderParam(updatedClass.ParameterList!);
            string paramName  = existingPrimaryParam?.Identifier.Text ?? DefaultParamName;
            bool   needsParam = existingPrimaryParam == null;

            // 3. Construit le champ avec initialisation inline :
            //      private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
            var newFieldDecl = BuildReadonlyFieldWithInitializer(DefaultFieldName, paramName);

            // 4. Insère le champ en première position dans les membres de la classe
            var newMembers = updatedClass.Members.Insert(0, newFieldDecl);
            var newClass   = updatedClass.WithMembers(newMembers);

            // 5. Si nécessaire, ajoute le paramètre au ParameterList de la classe
            if (needsParam)
            {
                newClass = newClass.WithParameterList(
                    newClass.ParameterList!.AddParameters(BuildParameter(DefaultParamName)));
            }

            newRoot = newRoot.ReplaceNode(updatedClass, newClass);
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }

        // ── Service fix — constructeur classique ──────────────────────────────
        //
        //  Avant :
        //    public class MyService : DefaultService
        //    {
        //        public MyService(IServiceContext context) : base(context) { }
        //        ...
        //    }
        //
        //  Après :
        //    public class MyService : DefaultService
        //    {
        //        private readonly IDateTimeProvider _dateTimeProvider;
        //
        //        public MyService(IServiceContext context, IDateTimeProvider dateTimeProvider)
        //            : base(context)
        //        {
        //            _dateTimeProvider = dateTimeProvider;
        //        }
        //        ...
        //    }

        private static Task<Document> ApplyServiceRegularCtorFixAsync(
            Document document,
            SyntaxNode root,
            MemberAccessExpressionSyntax memberAccess,
            string memberName,
            ClassDeclarationSyntax classDeclaration,
            CancellationToken _)
        {
            // Cherche un champ IDateTimeProvider déjà existant
            var existingField = FindExistingProviderField(classDeclaration);
            string fieldName    = existingField?.Declaration.Variables.First().Identifier.Text
                                  ?? DefaultFieldName;
            bool   needNewField = existingField == null;

            // 1. Remplace DateTime.Now/UtcNow → _dateTimeProvider.Now/UtcNow
            var newRoot = root.ReplaceNode(memberAccess, BuildMemberAccess(fieldName, memberName, memberAccess));

            if (!needNewField)
                return Task.FromResult(document.WithSyntaxRoot(newRoot));

            // 2. Retrouve la classe dans le nouveau root
            var updatedClass = FindUpdatedClass(newRoot, classDeclaration);
            if (updatedClass == null) return Task.FromResult(document.WithSyntaxRoot(newRoot));

            var ctor = updatedClass.Members
                .OfType<ConstructorDeclarationSyntax>()
                .FirstOrDefault();

            var newFieldDecl = BuildReadonlyField(DefaultFieldName);
            SyntaxList<MemberDeclarationSyntax> newMembers;

            if (ctor != null)
            {
                var existingCtorParam = FindExistingProviderParam(ctor.ParameterList);

                ConstructorDeclarationSyntax newCtor;
                if (existingCtorParam != null)
                {
                    // Paramètre déjà présent : ajoute uniquement l'assignation
                    newCtor = AppendAssignment(ctor, DefaultFieldName, existingCtorParam.Identifier.Text);
                }
                else
                {
                    newCtor = ctor
                        .WithParameterList(ctor.ParameterList.AddParameters(BuildParameter(DefaultParamName)));
                    newCtor = AppendAssignment(newCtor, DefaultFieldName, DefaultParamName);
                }

                // Insère le champ juste avant le constructeur
                int ctorIndex = updatedClass.Members.IndexOf(ctor);
                newMembers = updatedClass.Members
                    .Replace(ctor, newCtor)
                    .Insert(ctorIndex, newFieldDecl);
            }
            else
            {
                // Pas de constructeur : on ajoute uniquement le champ
                newMembers = updatedClass.Members.Insert(0, newFieldDecl);
            }

            newRoot = newRoot.ReplaceNode(updatedClass, updatedClass.WithMembers(newMembers));
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static FieldDeclarationSyntax? FindExistingProviderField(ClassDeclarationSyntax classDecl)
            => classDecl.Members
                .OfType<FieldDeclarationSyntax>()
                .FirstOrDefault(f =>
                    (f.Declaration.Type is IdentifierNameSyntax ins &&
                     ins.Identifier.Text == DateTimeProviderInterface) ||
                    (f.Declaration.Type is QualifiedNameSyntax qns &&
                     qns.Right.Identifier.Text == DateTimeProviderInterface));

        private static ParameterSyntax? FindExistingProviderParam(ParameterListSyntax? paramList)
        {
            if (paramList == null) return null;
            return paramList.Parameters.FirstOrDefault(p =>
                (p.Type is IdentifierNameSyntax ins && ins.Identifier.Text == DateTimeProviderInterface) ||
                (p.Type is QualifiedNameSyntax   qns && qns.Right.Identifier.Text == DateTimeProviderInterface));
        }

        private static MemberAccessExpressionSyntax BuildMemberAccess(
            string objectName, string memberName, MemberAccessExpressionSyntax trivia)
            => SyntaxFactory
                .MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(objectName),
                    SyntaxFactory.IdentifierName(memberName))
                .WithTriviaFrom(trivia);

        private static ParameterSyntax BuildParameter(string paramName)
            => SyntaxFactory
                .Parameter(SyntaxFactory.Identifier(paramName))
                .WithType(
                    SyntaxFactory.IdentifierName(DateTimeProviderInterface)
                        .WithTrailingTrivia(SyntaxFactory.Space));

        /// <summary>
        /// Construit <c>private readonly IDateTimeProvider _dateTimeProvider;</c>
        /// (sans initialiseur — pour les constructeurs classiques).
        /// </summary>
        private static FieldDeclarationSyntax BuildReadonlyField(string fieldName)
            => SyntaxFactory
                .FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName(DateTimeProviderInterface)
                            .WithTrailingTrivia(SyntaxFactory.Space),
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(fieldName)))))
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword)
                        .WithTrailingTrivia(SyntaxFactory.Space),
                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
                        .WithTrailingTrivia(SyntaxFactory.Space))
                .WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed)
                .WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed);

        /// <summary>
        /// Construit <c>private readonly IDateTimeProvider _dateTimeProvider = {paramName};</c>
        /// (avec initialiseur inline — pour les primary constructors).
        /// </summary>
        private static FieldDeclarationSyntax BuildReadonlyFieldWithInitializer(string fieldName, string paramName)
            => SyntaxFactory
                .FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName(DateTimeProviderInterface)
                            .WithTrailingTrivia(SyntaxFactory.Space),
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(fieldName))
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.IdentifierName(paramName))
                                    .WithLeadingTrivia(SyntaxFactory.Space)))))
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword)
                        .WithTrailingTrivia(SyntaxFactory.Space),
                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
                        .WithTrailingTrivia(SyntaxFactory.Space))
                .WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed)
                .WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed);

        private static ConstructorDeclarationSyntax AppendAssignment(
            ConstructorDeclarationSyntax ctor, string fieldName, string paramName)
        {
            if (ctor.Body == null) return ctor;

            var assignment = SyntaxFactory
                .ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName(fieldName),
                        SyntaxFactory.IdentifierName(paramName)))
                .WithLeadingTrivia(SyntaxFactory.ElasticTab)
                .WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed);

            return ctor.WithBody(ctor.Body.AddStatements(assignment));
        }

        private static MethodDeclarationSyntax? FindUpdatedMethod(
            SyntaxNode newRoot, MethodDeclarationSyntax original)
            => newRoot.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(m =>
                    m.Identifier.Text == original.Identifier.Text &&
                    m.SpanStart      == original.SpanStart);

        private static ClassDeclarationSyntax? FindUpdatedClass(
            SyntaxNode newRoot, ClassDeclarationSyntax original)
            => newRoot.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(c =>
                    c.Identifier.Text == original.Identifier.Text &&
                    c.SpanStart      == original.SpanStart);
    }
}
