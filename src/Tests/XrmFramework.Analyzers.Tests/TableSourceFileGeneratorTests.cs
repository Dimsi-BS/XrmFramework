using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;


using XrmFramework.Analyzers.Generators;

namespace XrmFramework.Analyzers.Tests;


[TestFixture]
public class TableSourceFileGeneratorTests
{
    private static byte[] LoadFixture(string fileName)
        => File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", fileName));

    [Test]
    public async Task CalculateTableFiles()
    {
        // No C# user code is needed: TableSourceFileGenerator only consumes AdditionalTexts.
        var source = string.Empty;

        await TestHelper.Verify<TableSourceFileGenerator>(source,
            ("Account.table", LoadFixture("Account.table")),
            ("Contratdelocation.table", LoadFixture("Contratdelocation.table")),
            ("OptionSet.table", LoadFixture("OptionSet.table")),
            ("Particulier.table", LoadFixture("Particulier.table")));
    }
}
