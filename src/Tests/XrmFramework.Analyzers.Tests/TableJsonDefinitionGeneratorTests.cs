using System.Threading.Tasks;
using XrmFramework.Analyzers.Generators;
using Xunit;

namespace XrmFramework.Analyzers.Tests;

public class TableJsonDefinitionGeneratorTests
{
	[Fact]
	public async Task CalculateTableFiles()
	{
		// The source code to test
		var source = @"";

		// Pass the source code to our helper and snapshot test the output
		await TestHelper.Verify<TableJsonDefinitionGenerator>(source,
			("Account.table", TableFiles.Account)
			);

	}
}
