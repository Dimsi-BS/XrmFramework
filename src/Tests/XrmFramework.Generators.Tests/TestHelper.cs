using VerifyNUnit;
// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;


namespace XrmFramework.Generators.Tests;

/// <summary>
/// Shared utilities for Roslyn source generator tests.
/// </summary>
public static class TestHelper
{
    /// <summary>
    /// Compiles the given source code, runs the <typeparamref name="TGenerator"/> generator,
    /// and verifies the output as a snapshot with Verify.
    /// </summary>
    /// <typeparam name="TGenerator">Type of the incremental generator to test.</typeparam>
    /// <param name="source">C# source code used as the input compilation.</param>
    /// <param name="additionalTexts">Additional files passed to the generator (e.g. .pluginsession.json).</param>
    public static Task Verify<TGenerator>(
        string source,
        params (string path, string content)[] additionalTexts)
        where TGenerator : IIncrementalGenerator, new()
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "GeneratorTests",
            syntaxTrees: [syntaxTree],
            references:
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Task).Assembly.Location)
            ]);

        var generator = new TGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        if (additionalTexts.Length > 0)
        {
            var texts = additionalTexts
                .Select(t => (Microsoft.CodeAnalysis.AdditionalText)new InMemoryAdditionalText(t.path, t.content))
                .ToArray();

            driver = driver.AddAdditionalTexts([..texts]);
        }

        driver = driver.RunGenerators(compilation);

        return Verifier
            .Verify(driver)
            .UseDirectory("TestData");
    }

    // ──────────────────────────────────────────────
    //  Helper: in-memory AdditionalText
    // ──────────────────────────────────────────────

    private sealed class InMemoryAdditionalText : Microsoft.CodeAnalysis.AdditionalText
    {
        private readonly string _path;
        private readonly string _content;

        public InMemoryAdditionalText(string path, string content)
        {
            _path = path;
            _content = content;
        }

        public override string Path => _path;

        public override Microsoft.CodeAnalysis.Text.SourceText? GetText(CancellationToken cancellationToken = default)
            => Microsoft.CodeAnalysis.Text.SourceText.From(_content);
    }
}
