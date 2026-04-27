using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Xrm.Sdk;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;
using XrmFramework.Analyzers.Model;

namespace XrmFramework.Analyzers.Tests
{
    public static class TestHelper
    {
        public static Task Verify<TGenerator>(string source, params (string path, byte[] content)[] additionalTexts) where TGenerator : IIncrementalGenerator, new()
        {
            // Parse the provided string into a C# syntax tree.
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            // Build a Roslyn compilation around it. We bring in mscorlib and Microsoft.Xrm.Sdk
            // so that generators can resolve common framework types.
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: new[] {
                    MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Entity).Assembly.Location)
                });

            // Wrap any (path, content) tuple so the generator can read it through
            // context.AdditionalTextsProvider, exactly like a project-level <AdditionalFiles>.
            var additionalFiles = ImmutableArray
                .CreateRange(additionalTexts.Select(a => (AdditionalText)new TableAdditionalText(a)));

            GeneratorDriver driver = CSharpGeneratorDriver
                .Create(new TGenerator())
                .AddAdditionalTexts(additionalFiles);

            driver = driver.RunGenerators(compilation);

            return Verifier
                .Verify(driver)
                .UseDirectory("TestData");
        }
    }
}
