using System;
using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using XrmFramework.Analyzers.Generators;

namespace XrmFramework.Analyzers.Tests;

[UsesVerify]
public class TableSourceFileGeneratorTests
{
    private static byte[] LoadFixture(string fileName)
        => File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", fileName));

    [Fact]
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
