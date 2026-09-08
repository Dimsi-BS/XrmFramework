// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using XrmFramework.DeployUtils.ModelSync;
using CoreModel = XrmFramework.Core.Model;

namespace XrmFramework.DeployUtils.Tests.ModelSync;

/// <summary>
/// <see cref="ModelSourceMigrator"/> against real files on disk — in particular the case a single
/// unit test on <see cref="BindingModelSourceRewriter"/> alone cannot exercise: a <c>.cs</c> file
/// declaring several of the migrated classes, possibly alongside content that is not migrated at all.
/// </summary>
[TestFixture]
public class ModelSourceMigratorTests
{
    private string _directory;

    [SetUp]
    public void SetUp()
    {
        _directory = Path.Combine(Path.GetTempPath(), "XrmFrameworkModelSyncTests_" + Path.GetRandomFileName());
        Directory.CreateDirectory(_directory);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, recursive: true);
    }

    private string WriteSource(string fileName, string content)
    {
        var path = Path.Combine(_directory, fileName);
        File.WriteAllText(path, content);
        return path;
    }

    /// <summary>
    /// The bug this locks in: a file holding several fully-mapped classes (each individually a
    /// <see cref="ModelSourceRewriteOutcome.Delete"/> in isolation) plus an unrelated class not among
    /// the migrated models must be converted to <c>*.partial.cs</c> — never deleted outright, and
    /// every migrated model must be found even though they share one file.
    /// </summary>
    [Test]
    public void MultipleMappedClassesInOneFile_AllStripped_FileKeptForUnrelatedContent()
    {
        WriteSource("CongeService.cs", """
            using XrmFramework;
            using XrmFramework.BindingModel;

            namespace MyProject.Core.Model
            {
                [CrmEntity(typeof(ConsultantDefinition))]
                public partial class ConsultantModel : BindingModelBase
                {
                    [CrmMapping(ConsultantDefinition.Columns.FullName)]
                    public string FullName { get; set; }
                }

                [CrmEntity(typeof(CongeDefinition))]
                public partial class CongeModel : BindingModelBase
                {
                    [CrmMapping(CongeDefinition.Columns.StartDate)]
                    public System.DateTime StartDate { get; set; }
                }

                public class CongeService
                {
                    public void Process() { }
                }
            }
            """);

        var models = new List<CoreModel>
        {
            new CoreModel { Name = "ConsultantModel", ModelNamespace = "MyProject.Core.Model" },
            new CoreModel { Name = "CongeModel", ModelNamespace = "MyProject.Core.Model" }
        };

        var migrator = new ModelSourceMigrator(_directory);
        var notFound = migrator.Migrate(models);

        Assert.AreEqual(0, notFound, "both classes share a file — neither should be reported as not found");

        var originalPath = Path.Combine(_directory, "CongeService.cs");
        var partialPath = Path.Combine(_directory, "CongeService.partial.cs");

        Assert.IsFalse(File.Exists(originalPath), "the original file must be replaced, not left in place");
        Assert.IsTrue(File.Exists(partialPath), "unrelated content (CongeService) survives, so the file must be kept as .partial.cs");

        var text = File.ReadAllText(partialPath);
        Assert.That(text, Does.Not.Contain("FullName"));
        Assert.That(text, Does.Not.Contain("StartDate"));
        Assert.That(text, Does.Contain("class CongeService"));
        Assert.That(text, Does.Contain("public void Process()"));
    }

    /// <summary>The simple case still works once the loop is file-centric: a lone fully-mapped class deletes its file.</summary>
    [Test]
    public void SingleFullyMappedClassAloneInFile_FileIsDeleted()
    {
        WriteSource("ContactModel.cs", """
            using XrmFramework;
            using XrmFramework.BindingModel;

            namespace MyProject.Core.Model
            {
                [CrmEntity(typeof(ContactDefinition))]
                public partial class ContactModel : BindingModelBase
                {
                    [CrmMapping(ContactDefinition.Columns.FullName)]
                    public string FullName { get; set; }
                }
            }
            """);

        var models = new List<CoreModel> { new CoreModel { Name = "ContactModel", ModelNamespace = "MyProject.Core.Model" } };

        var migrator = new ModelSourceMigrator(_directory);
        var notFound = migrator.Migrate(models);

        Assert.AreEqual(0, notFound);
        Assert.IsFalse(File.Exists(Path.Combine(_directory, "ContactModel.cs")));
        Assert.IsFalse(File.Exists(Path.Combine(_directory, "ContactModel.partial.cs")));
        Assert.IsTrue(Directory.GetFiles(_directory).Length == 0);
    }

    /// <summary>A model named in the list but present in no file is reported, without touching unrelated files.</summary>
    [Test]
    public void ModelNotPresentInAnyFile_IsReportedNotFound()
    {
        WriteSource("Unrelated.cs", """
            namespace MyProject.Core.Model
            {
                public class SomethingElse { }
            }
            """);

        var models = new List<CoreModel> { new CoreModel { Name = "GhostModel", ModelNamespace = "MyProject.Core.Model" } };

        var migrator = new ModelSourceMigrator(_directory);
        var notFound = migrator.Migrate(models);

        Assert.AreEqual(1, notFound);
        Assert.IsTrue(File.Exists(Path.Combine(_directory, "Unrelated.cs")));
    }
}
