// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using XrmFramework.DeployUtils.Factories;

namespace XrmFramework.PluginInventory.Tests;

/// <summary>
///     Inventories a REAL .NET Framework plugin assembly with the .NET-modern host, out of process,
///     exactly as <c>xrmframework deploy plugins</c> does.
/// </summary>
/// <remarks>
///     This is the fixture that makes cross-platform deployment a tested claim rather than a hope.
///     Everything it asserts depends on a net462/net48 assembly, its net4x <c>Microsoft.Xrm.Sdk</c>
///     and its workflow activities having loaded and RUN on a runtime that has neither .NET
///     Framework nor <c>System.Activities</c>.
///     <para>
///         The workflow assertions are the load-bearing ones. Without the <c>System.Activities</c>
///         stand-in, every <c>CodeActivity</c>-derived type fails to load - so the giveaway is not a
///         wrong display name but an assembly that reports no workflow activities at all.
///     </para>
/// </remarks>
[TestFixture]
public class CrossTargetFrameworkInventoryTests
{
    private static string[] TargetFrameworks() => InventoryHost.FixtureTargetFrameworks;

    private static JsonElement Manifest(string targetFramework) =>
        JsonDocument.Parse(InventoryHost.JsonOrFail(InventoryHost.Fixture(targetFramework))).RootElement;

    private static JsonElement Workflow(JsonElement manifest, string typeName) =>
        manifest.GetProperty("workflows").EnumerateArray()
            .Single(w => w.GetProperty("fullName").GetString()!.EndsWith("." + typeName));

    // ── The fixtures are built for every supported plugin target framework ──────────

    [Test]
    public void Both_Supported_Plugin_Target_Frameworks_Are_Covered()
        => Assert.That(TargetFrameworks(), Is.EquivalentTo(new[] { "net462", "net48" }));

    // ── Plugins and steps ───────────────────────────────────────────────────────────

    [TestCaseSource(nameof(TargetFrameworks))]
    public void Steps_Registered_In_A_Loop_Are_All_Reported(string targetFramework)
    {
        var plugin = Manifest(targetFramework).GetProperty("plugins").EnumerateArray().Single();

        // Two explicit AddStep calls plus three from a foreach. Static analysis of the source
        // would find one AddStep in that loop; only running the constructor yields three.
        var deleteSteps = plugin.GetProperty("steps").EnumerateArray()
            .Where(s => s.GetProperty("message").GetString() == "Delete")
            .Select(s => s.GetProperty("entityName").GetString())
            .ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(plugin.GetProperty("steps").EnumerateArray().Count(), Is.EqualTo(5));
            Assert.That(deleteSteps, Is.EquivalentTo(new[] { "contact", "lead", "opportunity" }));
        });
    }

    [TestCaseSource(nameof(TargetFrameworks))]
    public void Step_Attributes_Are_Transcribed(string targetFramework)
    {
        var steps = Manifest(targetFramework).GetProperty("plugins").EnumerateArray().Single()
            .GetProperty("steps").EnumerateArray().ToArray();

        var create = steps.Single(s => s.GetProperty("methodName").GetString() == "OnCreate");
        var update = steps.Single(s => s.GetProperty("methodName").GetString() == "OnUpdate");

        Assert.Multiple(() =>
        {
            Assert.That(create.GetProperty("stage").GetString(), Is.EqualTo("PreOperation"));
            Assert.That(create.GetProperty("mode").GetString(), Is.EqualTo("Synchronous"));
            Assert.That(create.GetProperty("preImage").GetProperty("attributes").EnumerateArray()
                .Select(a => a.GetString()), Is.EquivalentTo(new[] { "name", "accountnumber" }));
            Assert.That(create.GetProperty("postImage").GetProperty("allAttributes").GetBoolean(), Is.True);

            Assert.That(update.GetProperty("mode").GetString(), Is.EqualTo("Asynchronous"));
            Assert.That(update.GetProperty("order").GetInt32(), Is.EqualTo(42));
            Assert.That(update.GetProperty("filteringAttributes").EnumerateArray()
                .Select(a => a.GetString()), Is.EquivalentTo(new[] { "name", "telephone1" }));
        });
    }

    // ── Workflow activities: these only exist if the shim did its job ───────────────

    [TestCaseSource(nameof(TargetFrameworks))]
    public void Workflow_DisplayName_Computed_In_The_Constructor_Is_Reported(string targetFramework)
    {
        // Concatenated at construction time, so no static reading of the assembly could produce it.
        Assert.That(
            Workflow(Manifest(targetFramework), "ComputeScoreActivity").GetProperty("displayName").GetString(),
            Is.EqualTo("Compute account score"));
    }

    [TestCaseSource(nameof(TargetFrameworks))]
    public void Workflow_Without_A_DisplayName_Falls_Back_To_Its_Type_Name(string targetFramework)
    {
        Assert.That(
            Workflow(Manifest(targetFramework), "NamelessActivity").GetProperty("displayName").GetString(),
            Is.EqualTo("NamelessActivity"));
    }

    /// <remarks>
    ///     The single highest-value assertion about the stand-in. <c>CodeActivityMetadata</c> is a
    ///     STRUCT in the real System.Activities, and a signature blob spells a value type and a class
    ///     with different bytes. Declare it as a class and this activity's <c>CacheMetadata</c>
    ///     override matches nothing, so the activity fails to load - with a <c>TypeLoadException</c>
    ///     naming the user's type and no hint that System.Activities is involved.
    /// </remarks>
    [TestCaseSource(nameof(TargetFrameworks))]
    public void Activity_Overriding_CacheMetadata_Still_Loads(string targetFramework)
    {
        Assert.That(
            Workflow(Manifest(targetFramework), "CachingActivity").GetProperty("displayName").GetString(),
            Is.EqualTo("Caching activity"));
    }

    /// <remarks>
    ///     <c>EnableRulesActivityBase</c> carries <c>InArgument&lt;string&gt;</c> auto-properties, whose
    ///     backing fields the runtime resolves while laying the type out. It ships as source in a real
    ///     plugin project, so those generic argument types have to be loadable even for a plugin whose
    ///     own activities declare none.
    /// </remarks>
    [TestCaseSource(nameof(TargetFrameworks))]
    public void Activity_With_Framework_Supplied_Arguments_Loads(string targetFramework)
        => Assert.That(Workflow(Manifest(targetFramework), "EnableRulesFixture").ValueKind,
            Is.EqualTo(JsonValueKind.Object));

    // ── Custom APIs ─────────────────────────────────────────────────────────────────

    [TestCaseSource(nameof(TargetFrameworks))]
    public void Custom_Api_Arguments_Keep_Their_Types(string targetFramework)
    {
        var api = Manifest(targetFramework).GetProperty("customApis").EnumerateArray().Single();
        var arguments = api.GetProperty("arguments").EnumerateArray().ToArray();

        var enumArgument = arguments.Single(a => a.GetProperty("name").GetString() == "Kind");
        var output = arguments.Single(a => !a.GetProperty("isInArgument").GetBoolean());

        Assert.Multiple(() =>
        {
            Assert.That(api.GetProperty("bindingType").GetString(), Is.EqualTo("Entity"));
            Assert.That(api.GetProperty("boundEntityLogicalName").GetString(), Is.EqualTo("account"));
            Assert.That(enumArgument.GetProperty("isEnum").GetBoolean(), Is.True);
            Assert.That(output.GetProperty("typeFullName").GetString(), Is.EqualTo("System.Int32"));
        });
    }

    // ── The marker read back out of a net4x assembly ────────────────────────────────

    /// <remarks>
    ///     The packaging targets stamp <c>AssemblyMetadata</c> into the assembly and the engine reads
    ///     it back. Doing that across target frameworks - a net4x attribute read by a modern runtime,
    ///     its type reference travelling through the mscorlib facade - is only exercised here.
    /// </remarks>
    [TestCaseSource(nameof(TargetFrameworks))]
    public void Plugin_Package_Marker_Is_Read_Across_Target_Frameworks(string targetFramework)
        => Assert.That(Manifest(targetFramework).GetProperty("pluginPackage").GetBoolean(), Is.True);

    // ── Producer -> real consumer ───────────────────────────────────────────────────

    [TestCaseSource(nameof(TargetFrameworks))]
    public void RoundTrip_Through_The_Real_Reader(string targetFramework)
    {
        var json = InventoryHost.JsonOrFail(InventoryHost.Fixture(targetFramework));

        var plugin = PluginInventoryReader.ReadPlugins(json).Single();
        var workflows = PluginInventoryReader.ReadWorkflows(json);

        Assert.Multiple(() =>
        {
            Assert.That(plugin.Steps, Has.Count.EqualTo(5));
            Assert.That(workflows.Select(w => w.DisplayName), Contains.Item("Compute account score"));
            Assert.That(PluginInventoryReader.ReadUsesPluginPackage(json), Is.True);
        });
    }
}
