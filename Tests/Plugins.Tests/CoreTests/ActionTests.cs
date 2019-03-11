using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Plugins.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ActionTests
    {
        class PluginTest1 : Plugin
        {
            protected override void AddSteps()
            {
                AddStep(Stages.PreOperation, Messages.Update, Modes.Synchronous, "test", "Machin");
            }
        }

        [TestMethod]
        public void ActionNotExist()
        {
            var ok = false;
            try
            {
                var pluginTest = new PluginTest1();
            }
            catch (InvalidPluginExecutionException)
            {
                ok = true;
            }

            Assert.IsTrue(ok, "It is possible to add a step for a non existing Action");
        }

        class PluginTest2 : Plugin
        {
            protected override void AddSteps()
            {
                AddStep(Stages.PostOperation, Messages.Create, Modes.Synchronous, "test", "Chose");
            }

            private void Chose(IPluginContext localContext)
            {
            }
        }

        class PluginTest3 : Plugin
        {
            protected override void AddSteps()
            {
                AddStep(Stages.PostOperation, Messages.Create, Modes.Synchronous, "test", "Chose");
            }

            public static void Chose(IPluginContext localContext)
            {
            }
        }
    
        [TestMethod]
        public void ActionNotPublic()
        {
            var ok = false;
            try
            {
                var pluginTest = new PluginTest2();
            }
            catch (InvalidPluginExecutionException)
            {
                ok = true;
            }

            Assert.IsTrue(ok, "It is possible to have a non public action");
        }

        [TestMethod]
        public void ActionStatic()
        {
            var ok = false;
            try
            {
                var pluginTest = new PluginTest3();
            }
            catch (InvalidPluginExecutionException)
            {
                ok = true;
            }

            Assert.IsTrue(ok, "It is possible to have a static action");
        }

        class PluginTest4 : Plugin
        {
            protected override void AddSteps()
            {
                AddStep(Stages.PostOperation, Messages.Create, Modes.Synchronous, "test", "Chose");
            }

            public void Chose(IPluginContext localContext)
            {
            }
        }


    }
}
