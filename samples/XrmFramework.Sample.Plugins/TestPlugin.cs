using System;

namespace XrmFramework.Sample.Plugins
{
    internal class TestPlugin(string unsecuredConfig, string securedConfig) : Plugin(unsecuredConfig, securedConfig)
    {
        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, "account", nameof(MaMethode));
        }

        public void MaMethode(IPluginContext context)
        {
            // throw new InvalidPluginExecutionException("An error occurred in the test plugin.");
            Console.WriteLine("MaMethode was executed successfully!");
        }
}
