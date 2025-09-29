using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using XrmFramework;
using XrmFramework.UnitTests;
using XrmFramework.UnitTests.Plugin;

namespace TestProject1XrmFramework.Plugin.UnitTests.Tests;

[TestClass]
public partial class DemoPluginTests : PluginTestClass<DemoPlugin>
{
    [TestMethod]
    //[PluginStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, "account", nameof(DemoPlugin.MaMethode))]
    public void DemoPlugin_Should_Initialize_With_Correct_Configuration()
    {
        // Arrange
        var plugin = new DemoPlugin(null, null);

        var result = new List<Entity>();

        MockOrganizationService mockService = new MockOrganizationService();
        mockService.RetrieveAll(b => b
                .Query(qb => qb
                    .EntityName("account")
                    .Columns(c => c.IsAllColumns())
                .Criteria(c => c.AtLeastOneCondition("toto", ConditionOperator.Equal, "titi")))
                .Returns(result)
        );


    }

}

public class DemoPlugin(string unsecuredConfig, string securedConfig)
    : XrmFramework.Plugin(unsecuredConfig, securedConfig)
{
    // This is a placeholder for the actual plugin implementation.
    // In a real scenario, this class would contain the logic for the plugin.
    protected override void AddSteps()
    {
        
    }
    
    public void MaMethode(IPluginContext pluginContext)
    {
        
    }
}
