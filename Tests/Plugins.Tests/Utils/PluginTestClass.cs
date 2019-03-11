using Deploy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.Tests
{
    public abstract class PluginTestClass<T> : TestClass where T : Plugin, new()
    {
        protected MockPluginContext PluginContext { get; private set; }

        protected T Plugin { get; private set; }

        protected sealed override void InitializeTest(IServiceContext context)
        {
            Plugin = new T();

            var testType = GetType().Assembly.GetType(TestContext.FullyQualifiedTestClassName);

            var method = testType.GetMethod(TestContext.TestName);

            var contextAttribute = method.GetCustomAttributes(typeof(PluginContextAttribute), false).FirstOrDefault();

            Assert.IsNotNull(contextAttribute, "A test on a plugin must declare a PluginContextAttribute");

            var a = contextAttribute as PluginContextAttribute;

            PluginContext = new MockPluginContext(a.Message, a.Stage, a.Mode, a.EntityName);

            InitializeServices();
        }

        protected abstract void InitializeServices();
    }
}
