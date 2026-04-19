// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit;
using XrmFramework.RemoteDebugger.Generator;

namespace XrmFramework.Generators.Tests;

/// <summary>
/// Tests du générateur de source <see cref="PluginTestSourceGenerator"/>.
/// Utilise Verify pour comparer la sortie générée avec des snapshots.
/// </summary>
[UsesVerify]
public class PluginTestSourceGeneratorTests
{
    // ──────────────────────────────────────────────
    //  Sans fichiers .pluginsession.json
    // ──────────────────────────────────────────────

    [Fact]
    public Task Generator_WithNoAdditionalFiles_ProducesNoOutput()
    {
        const string source = "// empty";

        return TestHelper.Verify<PluginTestSourceGenerator>(source);
    }

    // ──────────────────────────────────────────────
    //  Avec un fichier .pluginsession.json minimal
    // ──────────────────────────────────────────────

    [Fact]
    public Task Generator_WithSingleSessionFile_GeneratesTestClass()
    {
        const string source = "// empty";

        // JSON minimal simulant une session de débogage distant capturée
        const string sessionJson = """
            {
              "pluginTypeName": "MyPlugin",
              "message": "Create",
              "stage": 20,
              "entityName": "contact",
              "capturedAt": "2025-01-01T00:00:00Z",
              "inputParameters": {},
              "preImages": {},
              "postImages": {}
            }
            """;

        return TestHelper.Verify<PluginTestSourceGenerator>(
            source,
            ("PluginTestSessions/MyPlugin_Create_contact.pluginsession.json", sessionJson));
    }

    // ──────────────────────────────────────────────
    //  Compilation sans erreurs
    // ──────────────────────────────────────────────

    [Fact]
    public void Generator_WithNoAdditionalFiles_CompilationHasNoDiagnostics()
    {
        const string source = "// empty";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "DiagnosticsTest",
            syntaxTrees: new[] { syntaxTree },
            references: new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            });

        var generator = new PluginTestSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGenerators(compilation);

        var result = driver.GetRunResult();

        Assert.Empty(result.Diagnostics);
    }
}
