using System.Threading.Tasks;
using VerifyXunit;
using XrmFramework.Analyzers.Generators;
using Xunit;

namespace XrmFramework.Analyzers.Tests;

[UsesVerify]
public class TableSourceFileGeneratorTests
{
    [Fact]
    public async Task CalculateTableFiles()
    {
        // The source code to test
        var source = @"";

        // Pass the source code to our helper and snapshot test the output
        await TestHelper.Verify<TableSourceFileGenerator>(source,
            ("Account.table", TableFiles.Account),
            ("Contratdelocation.table", TableFiles.Contratdelocation),
            ("OptionSet.table", TableFiles.OptionSet),
            ("Particulier.table", TableFiles.Particulier)
            );

    }
}
