// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using NUnit.Framework;

namespace XrmFramework.PluginInventory.Tests;

/// <summary>
///     Pins the inventory's behaviour when it cannot describe an assembly completely: it fails,
///     loudly, and writes no manifest.
/// </summary>
/// <remarks>
///     Both cases here used to be survivable, and that was the worst thing about them. A manifest is
///     the deployment's picture of the assembly, and the deployment treats a plugin, custom API or
///     workflow activity that is ABSENT from it as one that was deleted from the code - so it
///     unregisters it. An inventory that quietly omits what it could not load therefore does not
///     degrade gracefully; it silently tears down live registrations. Failing is the cheaper
///     outcome, every time.
/// </remarks>
[TestFixture]
public class InventoryStrictnessTests
{
    private static string BrokenFixture => Path.Combine(
        TestContext.CurrentContext.TestDirectory, "broken", "net48",
        "XrmFramework.PluginInventory.Fixtures.Broken.dll");

    /// <remarks>
    ///     Previously swallowed by a bare <c>catch</c> in <c>ExtractWorkflow</c>, which fell back to
    ///     the type's short name - so an activity whose action does not exist was registered in
    ///     Dataverse under a perfectly plausible friendly name, and nothing said otherwise.
    /// </remarks>
    [Test]
    public void An_Activity_Whose_Constructor_Throws_Fails_The_Inventory()
    {
        var result = InventoryHost.Run(BrokenFixture);

        Assert.Multiple(() =>
        {
            Assert.That(result.ExitCode, Is.Not.Zero, "The inventory should have failed.");
            Assert.That(result.Stdout, Is.Empty, "No manifest may be written when the inventory fails.");

            // The operator has to be able to find the offending type and the framework's own reason.
            Assert.That(result.Stderr, Does.Contain("BrokenActivity"));
            Assert.That(result.Stderr, Does.Contain("ThisMethodDoesNotExist"));
        });
    }

    /// <summary>
    ///     Removing the <c>System.Activities</c> stand-in must produce a hard error, not an assembly
    ///     that merely appears to have no workflow activities.
    /// </summary>
    /// <remarks>
    ///     Two things at once, which is why it is worth the file copying. It proves the stand-in is
    ///     genuinely what serves <c>System.Activities</c> - a test suite that passes with the shim
    ///     deleted would be telling us nothing - and it proves the type-load strictness works, since
    ///     this is the exact shape of the failure it exists for: the dependency that goes missing
    ///     takes out precisely the types that depend on it.
    /// </remarks>
    [Test]
    public void Without_The_Shim_The_Inventory_Fails_Instead_Of_Losing_The_Activities()
    {
        var host = Path.Combine(TestContext.CurrentContext.TestDirectory, "inventory-host");
        var crippled = Path.Combine(TestContext.CurrentContext.WorkDirectory, "inventory-host-no-shim");

        CopyDirectory(host, crippled);
        File.Delete(Path.Combine(crippled, "System.Activities.dll"));

        var result = InventoryHost.RunWithHost(
            Path.Combine(crippled, "XrmFramework.PluginInventory.dll"),
            InventoryHost.Fixture("net48"));

        Assert.Multiple(() =>
        {
            Assert.That(result.ExitCode, Is.Not.Zero, "The inventory should have failed.");
            Assert.That(result.Stdout, Is.Empty, "No manifest may be written when the inventory fails.");
            Assert.That(result.Stderr, Does.Contain("System.Activities"));
        });
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);

        foreach (var file in Directory.GetFiles(source))
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), overwrite: true);

        foreach (var directory in Directory.GetDirectories(source))
            CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
    }

    /// <remarks>
    ///     The fallback that must NOT have become an error: an activity that never sets a display
    ///     name is a supported, documented case, named after its type by System.Activities itself.
    /// </remarks>
    [Test]
    public void An_Activity_Without_A_DisplayName_Is_Still_Not_An_Error()
    {
        var json = InventoryHost.JsonOrFail(InventoryHost.Fixture("net48"));

        Assert.That(json, Does.Contain("NamelessActivity"));
    }
}
