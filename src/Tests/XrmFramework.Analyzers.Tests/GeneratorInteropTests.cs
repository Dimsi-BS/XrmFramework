// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using XrmFramework.Analyzers.Generators;
using XrmFramework.Analyzers.Model;

namespace XrmFramework.Analyzers.Tests;

/// <summary>
/// What one generator can see of another's output.
///
/// In a solution scaffolded from the templates, the <c>.table</c> files and the binding models
/// both live in the <c>.Core</c> project: <see cref="TableSourceFileGenerator"/> emits
/// <c>AccountDefinition</c> and <see cref="MappingSourceGenerator"/> looks for
/// <c>[CrmEntity(AccountDefinition.EntityName)]</c> in the same pass. Roslyn hands every
/// generator the same input compilation, so the question is whether the mapping generator can
/// resolve a constant the table generator is producing beside it.
///
/// <see cref="MappingSourceGeneratorTests"/> never answers this: it hand-writes its definition
/// classes as ordinary source.
/// </summary>
[TestFixture]
public class GeneratorInteropTests
{
    private static byte[] AccountTable
        => File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Account.table"));

    /// <summary>
    /// A hand-written binding model referencing the definition the table generator produces.
    /// Everything else it needs is stubbed inline, as in <see cref="MappingSourceGeneratorTests"/>.
    /// </summary>
    private const string BindingModelSource = """
using System;
using Microsoft.Xrm.Sdk;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class CrmEntityAttribute : Attribute
    {
        public CrmEntityAttribute(string entityName) => EntityName = entityName;
        public string EntityName { get; }
    }

    public interface IBindingModel { }

    namespace BindingModel
    {
        [AttributeUsage(AttributeTargets.Property)]
        public class CrmMappingAttribute : Attribute
        {
            public CrmMappingAttribute(string columnName) => ColumnName = columnName;
            public string ColumnName { get; }
        }
    }
}

// References AccountDefinition, which TableSourceFileGenerator emits from Account.table.
[XrmFramework.CrmEntity(AccountDefinition.EntityName)]
public partial class AccountModel : XrmFramework.IBindingModel
{
    [XrmFramework.BindingModel.CrmMapping(AccountDefinition.Columns.Name)]
    public string Name { get; set; }
}
""";

    /// <summary>The same model written with the typeof form.</summary>
    private const string TypeOfBindingModelSource = """
using System;
using Microsoft.Xrm.Sdk;

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

    namespace BindingModel
    {
        [AttributeUsage(AttributeTargets.Property)]
        public class CrmMappingAttribute : Attribute
        {
            public CrmMappingAttribute(string columnName) => ColumnName = columnName;
            public string ColumnName { get; }
        }
    }
}

[XrmFramework.CrmEntity(typeof(AccountDefinition))]
public partial class AccountModel : XrmFramework.IBindingModel
{
    [XrmFramework.BindingModel.CrmMapping(AccountDefinition.Columns.Name)]
    public string Name { get; set; }

    [XrmFramework.BindingModel.CrmMapping(AccountDefinition.Columns.AccountCategoryCode)]
    public int? Category { get; set; }
}
""";

    private static GeneratorDriverRunResult RunBoth(string source, params (string path, byte[] content)[] additionalTexts)
    {
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { CSharpSyntaxTree.ParseText(source) },
            references: new[]
            {
                MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Entity).Assembly.Location)
            });

        var additionalFiles = ImmutableArray.CreateRange(
            additionalTexts.Select(a => (AdditionalText)new TableAdditionalText(a)));

        return CSharpGeneratorDriver
            .Create(new TableSourceFileGenerator(), new MappingSourceGenerator())
            .AddAdditionalTexts(additionalFiles)
            .RunGenerators(compilation)
            .GetRunResult();
    }

    [Test]
    public void TableGenerator_EmitsTheDefinition()
    {
        var result = RunBoth(BindingModelSource, ("Model/Definitions/Account.table", AccountTable));

        var hintNames = result.Results.SelectMany(r => r.GeneratedSources).Select(s => s.HintName).ToList();

        Assert.That(hintNames, Does.Contain("Account.table.cs"));
    }

    /// <summary>
    /// The question this fixture exists for. A binding model whose <c>[CrmEntity]</c> argument is
    /// a constant from a generated definition class: does it get its mapping?
    /// </summary>
    [Test]
    public void MappingGenerator_ModelReferencingAGeneratedDefinition_EmitsMapping()
    {
        var result = RunBoth(BindingModelSource, ("Model/Definitions/Account.table", AccountTable));

        var mappingSources = result.Results
            .SelectMany(r => r.GeneratedSources)
            .Where(s => s.HintName.Contains("AccountModel"))
            .Select(s => s.HintName)
            .ToList();

        Assert.That(mappingSources, Is.Not.Empty,
            "MappingSourceGenerator produced nothing for a model whose [CrmEntity] argument comes "
            + "from a definition class generated in the same pass.");
    }

    [Test]
    public void MappingGenerator_TypeOfForm_EmitsMapping()
    {
        var result = RunBoth(TypeOfBindingModelSource, ("Model/Definitions/Account.table", AccountTable));

        var mapping = result.Results
            .SelectMany(r => r.GeneratedSources)
            .FirstOrDefault(s => s.HintName.Contains("AccountModel"));

        Assert.That(mapping.SourceText, Is.Not.Null, "no mapping was generated for the typeof form");

        var code = mapping.SourceText.ToString();

        Assert.That(code, Does.Contain("AccountDefinition.EntityName"),
            "the typeof form must still emit the definition constant, not a bare literal");
    }

    /// <summary>
    /// The column metadata has to come from the .table when the definition constant does not
    /// resolve, otherwise every column silently maps as a string.
    /// </summary>
    [Test]
    public void MappingGenerator_TypeOfForm_ReadsColumnTypesFromTheTable()
    {
        var result = RunBoth(TypeOfBindingModelSource, ("Model/Definitions/Account.table", AccountTable));

        var code = result.Results
            .SelectMany(r => r.GeneratedSources)
            .First(s => s.HintName.Contains("AccountModel"))
            .SourceText.ToString();

        Assert.That(code, Does.Contain("OptionSetValue"),
            "accountcategorycode is a Picklist in Account.table and must not be mapped as a string");
    }
}
