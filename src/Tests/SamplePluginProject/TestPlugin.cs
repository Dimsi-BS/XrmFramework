using Microsoft.Xrm.Sdk;

namespace SamplePluginProject;


public class TestPlugin : Plugin
{
    public TestPlugin(string unsecuredConfig, string securedConfig) : base(unsecuredConfig, securedConfig)
    {
    }

    protected override void AddSteps()
    {
        AddStep(Stages.PostOperation, Messages.Create, Modes.Synchronous, "account", nameof(MyMethod));
    }

    public void MyMethod(IPluginContext context, IService service)
    {
        var account = context.GetInputParameter<Entity>(InputParameters.Target);

        var accountBis = service.Retrieve(account.ToEntityReference());

        var date = DateTime.Now;
    }
}