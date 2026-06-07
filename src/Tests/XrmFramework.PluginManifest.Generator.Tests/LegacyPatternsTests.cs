// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using System.Text.Json;
using NUnit.Framework;

namespace XrmFramework.PluginManifest.Generator.Tests;

/// <summary>
/// Le code legacy référence souvent des membres <c>static</c> (non <c>const</c>) — nom d'entité
/// via <c>XxxDefinition.EntityName</c>, message custom via <c>Messages.GetMessage("...")</c> ou
/// un membre statique. Le générateur doit les résoudre sans émettre XRMMAN001.
/// </summary>
[TestFixture]
public class LegacyPatternsTests
{
    private const string Source = @"
using XrmFramework;
namespace Legacy
{
    // Nom d'entité en static (non const) — pattern hérité.
    public static class ProjetDefinition { public static string EntityName = ""ftp_projet""; }

    // Message custom exposé via un membre statique initialisé par GetMessage.
    public static class CustomMessages
    {
        public static Messages Affecter { get; } = Messages.GetMessage(""ftp_affecter"");
    }

    public class AffectationProjetPlugin : Plugin
    {
        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, ProjetDefinition.EntityName, nameof(OnCreate));
            AddStep(Stages.PostOperation, Messages.GetMessage(""ftp_customaction""), Modes.Asynchronous, ProjetDefinition.EntityName, nameof(OnAction));
            AddStep(Stages.PostOperation, CustomMessages.Affecter, Modes.Synchronous, ProjetDefinition.EntityName, nameof(OnAffecter));
        }
        public void OnCreate(object ctx) {}
        public void OnAction(object ctx) {}
        public void OnAffecter(object ctx) {}
    }
}";

    private static JsonElement _root;
    private static Microsoft.CodeAnalysis.Diagnostic[] _diagnostics = null!;

    [OneTimeSetUp]
    public void RunGenerator()
    {
        var (json, diagnostics) = GeneratorTestHelper.Run(Source);
        _root = JsonDocument.Parse(json).RootElement.Clone();
        _diagnostics = diagnostics.ToArray();
    }

    private static JsonElement Step(string method)
        => _root.GetProperty("plugins").EnumerateArray()
            .Single(p => p.GetProperty("fullName").GetString() == "Legacy.AffectationProjetPlugin")
            .GetProperty("steps").EnumerateArray()
            .Single(s => s.GetProperty("methodName").GetString() == method);

    [Test]
    public void NoDiagnostic_AndAllStepsExtracted()
    {
        Assert.That(_diagnostics.Any(d => d.Id == "XRMMAN001"), Is.False,
            "Le code legacy (membres statiques) ne doit plus produire XRMMAN001.");
        var steps = _root.GetProperty("plugins").EnumerateArray()
            .Single(p => p.GetProperty("fullName").GetString() == "Legacy.AffectationProjetPlugin")
            .GetProperty("steps");
        Assert.That(steps.GetArrayLength(), Is.EqualTo(3));
    }

    [Test]
    public void StaticEntityName_IsResolvedToLiteral()
    {
        Assert.That(Step("OnCreate").GetProperty("entityName").GetString(), Is.EqualTo("ftp_projet"));
    }

    [Test]
    public void GetMessage_Literal_IsResolved()
    {
        Assert.That(Step("OnAction").GetProperty("message").GetString(), Is.EqualTo("ftp_customaction"));
    }

    [Test]
    public void StaticCustomMessageMember_IsResolvedToItsMessageString()
    {
        // CustomMessages.Affecter = Messages.GetMessage("ftp_affecter") → "ftp_affecter" (et non "Affecter").
        Assert.That(Step("OnAffecter").GetProperty("message").GetString(), Is.EqualTo("ftp_affecter"));
    }
}
