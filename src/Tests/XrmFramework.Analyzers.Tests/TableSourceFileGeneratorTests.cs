using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
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

        // load all the files from the Resources folder
        
        var files = 
            Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Resources"))
                .Where(f => f.EndsWith(".table"))
                .Select(f => (path: f, content: LoadFixture(f)))
                .ToArray();
        
        
        
        await TestHelper.Verify<TableSourceFileGenerator>(source, files);
    }
}
