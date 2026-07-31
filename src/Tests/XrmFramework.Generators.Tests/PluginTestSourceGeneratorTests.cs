using NUnit.Framework;
// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;


using XrmFramework.RemoteDebugger.Generator;

namespace XrmFramework.Generators.Tests;

/// <summary>
/// Tests for the <see cref="PluginTestSourceGenerator"/> source generator.
/// Uses Verify to compare the generated output against snapshots.
/// </summary>

[TestFixture]
public class PluginTestSourceGeneratorTests
{
    // ──────────────────────────────────────────────
    //  Without .pluginsession.json files
    // ──────────────────────────────────────────────

    [Test]
    public Task Generator_WithNoAdditionalFiles_ProducesNoOutput()
    {
        const string source = "// empty";

        return TestHelper.Verify<PluginTestSourceGenerator>(source);
    }

    // ──────────────────────────────────────────────
    //  With a minimal .pluginsession.json file
    // ──────────────────────────────────────────────

    [Test]
    public Task Generator_WithSingleSessionFile_GeneratesTestClass()
    {
        const string source = "// empty";

        // Minimal JSON simulating a captured remote debugging session
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
    //  Compilation without errors
    // ──────────────────────────────────────────────

    [Test]
    public void Generator_WithNoAdditionalFiles_CompilationHasNoDiagnostics()
    {
        const string source = "// empty";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "DiagnosticsTest",
            syntaxTrees: [syntaxTree],
            references:
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            ]);

        var generator = new PluginTestSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGenerators(compilation);

        var result = driver.GetRunResult();

        Assert.IsEmpty(result.Diagnostics);
    }
}
