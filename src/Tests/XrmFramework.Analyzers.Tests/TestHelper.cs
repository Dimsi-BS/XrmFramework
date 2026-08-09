using VerifyNUnit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using XrmFramework.Analyzers.Model;

namespace XrmFramework.Analyzers.Tests
{
    public static class TestHelper
    {
        /// <summary>
        /// Runs <typeparamref name="TGenerator"/> over the given AdditionalTexts and hands back the
        /// sources it produced, keyed by hint name.
        /// </summary>
        /// <remarks>
        /// Meant for the cases a snapshot would only obscure: a test that asserts one precise trait of
        /// the generated code says what it is about far better than a diff against a whole file.
        /// </remarks>
        public static IReadOnlyDictionary<string, string> Generate<TGenerator>(
            params (string path, string content)[] additionalTexts)
            where TGenerator : IIncrementalGenerator, new()
        {
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { CSharpSyntaxTree.ParseText(string.Empty) },
                references: new[] {
                    MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Entity).Assembly.Location)
                });

            var additionalFiles = ImmutableArray.CreateRange(
                additionalTexts.Select(a => (AdditionalText)new TableAdditionalText(a.path, a.content)));

            var driver = CSharpGeneratorDriver
                .Create(new TGenerator())
                .AddAdditionalTexts(additionalFiles)
                .RunGenerators(compilation);

            return driver.GetRunResult()
                         .Results
                         .SelectMany(result => result.GeneratedSources)
                         .ToDictionary(source => source.HintName, source => source.SourceText.ToString());
        }


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
