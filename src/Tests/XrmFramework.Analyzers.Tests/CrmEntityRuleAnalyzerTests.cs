// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;

namespace XrmFramework.Analyzers.Tests;

/// <summary>
/// XRM0200 — a binding model must name its table through the generated definition rather than
/// through a string literal.
///
/// The fixture exists mainly for one case: <c>[CrmEntity(typeof(AccountDefinition))]</c> is now
/// the recommended form, and nothing pinned down that the rule accepts it. It does, because it
/// only reports literals — but that was incidental, and a later tightening of the rule would
/// otherwise turn the recommended form into a build error.
/// </summary>
[TestFixture]
public class CrmEntityRuleAnalyzerTests
{
    private const string Stubs = """
using System;
using XrmFramework;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class CrmEntityAttribute : Attribute
    {
        public CrmEntityAttribute(string entityName) => EntityName = entityName;
        public CrmEntityAttribute(Type definitionType) { }
        public string EntityName { get; }
    }

    public interface IBindingModel { }
}

public static class AccountDefinition
{
    public const string EntityName = "account";
}
""";

    private static async Task<ImmutableArray<Diagnostic>> Analyze(string modelSource)
    {
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { CSharpSyntaxTree.ParseText(Stubs + modelSource) },
            references: new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Entity).Assembly.Location)
            });

        var withAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(new CrmEntityRuleAnalyzer()));

        return await withAnalyzers.GetAnalyzerDiagnosticsAsync();
    }

    [Test]
    public async Task StringLiteral_IsReported()
    {
        var diagnostics = await Analyze("""

[CrmEntity("account")]
public partial class AccountModel : IBindingModel { }
""");

        Assert.That(diagnostics.Select(d => d.Id), Does.Contain("XRM0200"));
    }

    [Test]
    public async Task DefinitionConstant_IsAccepted()
    {
        var diagnostics = await Analyze("""

[CrmEntity(AccountDefinition.EntityName)]
public partial class AccountModel : IBindingModel { }
""");

        Assert.That(diagnostics.Select(d => d.Id), Does.Not.Contain("XRM0200"));
    }

    /// <summary>The form the documentation now recommends must not raise the rule.</summary>
    [Test]
    public async Task TypeOfDefinition_IsAccepted()
    {
        var diagnostics = await Analyze("""

[CrmEntity(typeof(AccountDefinition))]
public partial class AccountModel : IBindingModel { }
""");

        Assert.That(diagnostics.Select(d => d.Id), Does.Not.Contain("XRM0200"),
            "typeof(SomeDefinition) is the recommended way to name the table and must be accepted");
    }

    /// <summary>
    /// The rule used to compare the whole attribute name, so a qualified usage slipped past it.
    /// </summary>
    [Test]
    public async Task StringLiteral_OnAQualifiedAttributeName_IsReported()
    {
        var diagnostics = await Analyze("""

[XrmFramework.CrmEntity("account")]
public partial class QualifiedModel : XrmFramework.IBindingModel { }
""");

        Assert.That(diagnostics.Select(d => d.Id), Does.Contain("XRM0200"));
    }
}
