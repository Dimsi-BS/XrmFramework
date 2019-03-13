// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace Plugins.Tests.Objects
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PluginTests : TestClass
    {
        private TestPlugin Plugin { get; set; }

        private MockServiceProvider Provider { get; set; }
        protected override void InitializeTest(IServiceContext context)
        {
            Plugin = new TestPlugin();
            Provider = new MockServiceProvider();
        }

        private class TestPlugin : Plugin
        {
            protected override void AddSteps()
            {
                AddStep(Stages.PreValidation, Messages.Create, Modes.Synchronous, "test", "Action");
            }

            public void Action(IPluginContext context, IService service)
            {
                MethodLaunched = true;
            }

            public bool MethodLaunched { get; private set; }
        }

        [TestMethod]
        public void PluginServiceProviderNull()
        {
            AssertException<ArgumentNullException>(() =>
            {
                Plugin.Execute(null);
            });
        }

        [TestMethod]
        public void PluginServiceProvider()
        {
            Provider.PluginExecutionContext.Stage = (int)Stages.PreValidation;
            Provider.PluginExecutionContext.MessageName = "Create";
            Provider.PluginExecutionContext.PrimaryEntityName = "test";

            Provider.PluginExecutionContext.InputParameters["Target"] = new Entity();

            Plugin.Execute(Provider);

            Assert.IsTrue(Plugin.MethodLaunched);
        }
    }
}
