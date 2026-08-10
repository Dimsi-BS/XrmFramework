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
    /// Code fix for XRM0300.
    /// <list type="bullet">
    ///   <item>
    ///     <term>Plugin / IPlugin</term>
    ///     <description>
    ///       Adds <c>IDateTimeProvider dateTimeProvider</c> as a parameter of the containing
    ///       method and replaces <c>DateTime.Now/UtcNow/Today</c> with <c>dateTimeProvider.Now/UtcNow/Today</c>.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>IService — regular constructor</term>
    ///     <description>
    ///       Adds <c>IDateTimeProvider dateTimeProvider</c> to the constructor, creates the field
    ///       <c>private readonly IDateTimeProvider _dateTimeProvider;</c>, assigns it in the
    ///       constructor body, and replaces <c>DateTime.Now/UtcNow/Today</c> with <c>_dateTimeProvider.Now/UtcNow/Today</c>.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>IService — primary constructor</term>
    ///     <description>
    ///       Adds <c>IDateTimeProvider dateTimeProvider</c> to the class's <c>ParameterList</c>,
    ///       creates the field with an inline initializer
    ///       <c>private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;</c>
    ///       and replaces <c>DateTime.Now/UtcNow/Today</c> with <c>_dateTimeProvider.Now/UtcNow/Today</c>.
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

            var memberName = memberAccess.Name.Identifier.Text; // "Now", "UtcNow" or "Today"

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
                    ? $"Add IDateTimeProvider to primary constructor -> {DefaultFieldName}.{memberName}"
                    : $"Inject IDateTimeProvider into constructor -> {DefaultFieldName}.{memberName}";

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
                        title: $"Use IDateTimeProvider.{memberName} (method parameter)",
                        createChangedDocument: ct =>
                            ApplyPluginFixAsync(
                                context.Document, root, memberAccess, memberName, ct),
                        equivalenceKey: $"XRM0300_Plugin_{memberName}"),
                    diagnostic);
            }
        }

        // ── Plugin fix: method parameter ────────────────────────────────

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

        // ── Service fix: dispatch based on constructor type ─────────────

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
        //  Before:
        //    public class MyService(IServiceContext context) : DefaultService(context) { ... }
        //
        //  After:
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
            // Look for an already existing IDateTimeProvider field
            var existingField = FindExistingProviderField(classDeclaration);
            string fieldName    = existingField?.Declaration.Variables.First().Identifier.Text
                                  ?? DefaultFieldName;
            bool   needNewField = existingField == null;

            // 1. Replace DateTime.Now/UtcNow/Today -> _dateTimeProvider.Now/UtcNow/Today
            var newRoot = root.ReplaceNode(memberAccess, BuildMemberAccess(fieldName, memberName, memberAccess));

            if (!needNewField)
                return Task.FromResult(document.WithSyntaxRoot(newRoot));

            // 2. Find the class again in the new root
            var updatedClass = FindUpdatedClass(newRoot, classDeclaration);
            if (updatedClass == null) return Task.FromResult(document.WithSyntaxRoot(newRoot));

            // Check whether the primary constructor already has an IDateTimeProvider parameter
            var existingPrimaryParam = FindExistingProviderParam(updatedClass.ParameterList!);
            string paramName  = existingPrimaryParam?.Identifier.Text ?? DefaultParamName;
            bool   needsParam = existingPrimaryParam == null;

            // 3. Build the field with an inline initializer:
            //      private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
            var newFieldDecl = BuildReadonlyFieldWithInitializer(DefaultFieldName, paramName);

            // 4. Insert the field as the first member of the class
            var newMembers = updatedClass.Members.Insert(0, newFieldDecl);
            var newClass   = updatedClass.WithMembers(newMembers);

            // 5. If necessary, add the parameter to the class's ParameterList
            if (needsParam)
            {
                newClass = newClass.WithParameterList(
                    newClass.ParameterList!.AddParameters(BuildParameter(DefaultParamName)));
            }

            newRoot = newRoot.ReplaceNode(updatedClass, newClass);
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }

        // ── Service fix — regular constructor ──────────────────────────────
        //
        //  Before:
        //    public class MyService : DefaultService
        //    {
        //        public MyService(IServiceContext context) : base(context) { }
        //        ...
        //    }
        //
        //  After:
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
            // Look for an already existing IDateTimeProvider field
            var existingField = FindExistingProviderField(classDeclaration);
            string fieldName    = existingField?.Declaration.Variables.First().Identifier.Text
                                  ?? DefaultFieldName;
            bool   needNewField = existingField == null;

            // 1. Replace DateTime.Now/UtcNow/Today -> _dateTimeProvider.Now/UtcNow/Today
            var newRoot = root.ReplaceNode(memberAccess, BuildMemberAccess(fieldName, memberName, memberAccess));

            if (!needNewField)
                return Task.FromResult(document.WithSyntaxRoot(newRoot));

            // 2. Find the class again in the new root
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
                    // Parameter already present: only add the assignment
                    newCtor = AppendAssignment(ctor, DefaultFieldName, existingCtorParam.Identifier.Text);
                }
                else
                {
                    newCtor = ctor
                        .WithParameterList(ctor.ParameterList.AddParameters(BuildParameter(DefaultParamName)));
                    newCtor = AppendAssignment(newCtor, DefaultFieldName, DefaultParamName);
                }

                // Insert the field just before the constructor
                int ctorIndex = updatedClass.Members.IndexOf(ctor);
                newMembers = updatedClass.Members
                    .Replace(ctor, newCtor)
                    .Insert(ctorIndex, newFieldDecl);
            }
            else
            {
                // No constructor: only add the field
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
        /// Builds <c>private readonly IDateTimeProvider _dateTimeProvider;</c>
        /// (without initializer — for regular constructors).
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
        /// Builds <c>private readonly IDateTimeProvider _dateTimeProvider = {paramName};</c>
        /// (with inline initializer — for primary constructors).
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
