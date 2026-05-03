using VerifyNUnit;
// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;


namespace XrmFramework.Generators.Tests;

/// <summary>
/// Utilitaires partagés pour les tests de générateurs de source Roslyn.
/// </summary>
public static class TestHelper
{
    /// <summary>
    /// Compile le code source fourni, exécute le générateur <typeparamref name="TGenerator"/>
    /// et vérifie la sortie par snapshot avec Verify.
    /// </summary>
    /// <typeparam name="TGenerator">Type du générateur incrémental à tester.</typeparam>
    /// <param name="source">Code source C# servant de compilation d'entrée.</param>
    /// <param name="additionalTexts">Fichiers additionnels passés au générateur (ex. .pluginsession.json).</param>
    public static Task Verify<TGenerator>(
        string source,
        params (string path, string content)[] additionalTexts)
        where TGenerator : IIncrementalGenerator, new()
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "GeneratorTests",
            syntaxTrees: new[] { syntaxTree },
            references: new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            });

        var generator = new TGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        if (additionalTexts.Length > 0)
        {
            var texts = additionalTexts
                .Select(t => (Microsoft.CodeAnalysis.AdditionalText)new InMemoryAdditionalText(t.path, t.content))
                .ToArray();

            driver = driver.AddAdditionalTexts(ImmutableArray.Create(texts));
        }

        driver = driver.RunGenerators(compilation);

        return Verifier
            .Verify(driver)
            .UseDirectory("TestData");
    }

    // ──────────────────────────────────────────────
    //  Helper : AdditionalText en mémoire
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
