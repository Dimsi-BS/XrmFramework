using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Deploy;
using Moq;

namespace Plugins.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PluginCommonTests : TestClass
    {
        [TestMethod]
        public void InstantiateAllPlugins()
        {
            ObjectHelper<Plugin>.ApplyCode(new Type[] { }, null, null);
        }

        [TestMethod]
        public void StepCoverage()
        {
            ObjectHelper<Plugin>.ApplyCode(new Type[] { }, null, (plugin, type, sb) =>
            {
                var hasError = false;
                var declaredSteps = plugin.Steps;

                var testedSteps = GetTestedSteps(plugin);
                var comparer = new StepComparer();

                var notTestedSteps = declaredSteps.Except(testedSteps, comparer);
                var notDeclaredSteps = testedSteps.Except(declaredSteps, comparer);

                if (notTestedSteps.Any() || notDeclaredSteps.Any())
                {
                    hasError = true;
                    sb.AppendFormat("\r\nPlugin {0} :\r\n", plugin.GetType().Name);
                    if (notTestedSteps.Any())
                    {
                        sb.AppendLine("\tNot tested steps :");
                        foreach (var step in notTestedSteps)
                        {
                            sb.AppendFormat("\t\t- {0} {1} {2} {3}\r\n", step.EntityName, step.Message, step.Mode, step.Stage);
                        }
                    }
                    if (notDeclaredSteps.Any())
                    {
                        sb.AppendLine("\tNot declared steps :");
                        foreach (var step in notDeclaredSteps)
                        {
                            sb.AppendFormat("\t\t- {0} {1} {2} {3}\r\n", step.EntityName, step.Message, step.Mode, step.Stage);
                        }
                    }
                }
                return hasError;
            }, false);
        }

        [TestMethod]
        public void PublicMethodNullArguments()
        {
            ObjectHelper<Plugin>.ApplyCode(new Type[] { }, null, (plugin, type, sb) =>
            {
                var hasError = false;
                var declaredSteps = plugin.Steps;

                foreach (var step in declaredSteps)
                {
                    var listParamValues = new List<object>();

                    var parameters = step.Method.GetParameters();

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        listParamValues.Clear();
                        var nullParameterName = parameters.ElementAt(i).Name;
                        for (var j = 0; j < parameters.Length; j++)
                        {
                            var param = parameters.ElementAt(j);
                            if (i == j)
                            {
                                listParamValues.Add(null);
                            }
                            else
                            {
                                if (typeof(IPluginContext).IsAssignableFrom(param.ParameterType))
                                {
                                    listParamValues.Add(new MockPluginContext(Messages.Create, Stages.PostOperation, Modes.Asynchronous, "test"));
                                }
                                else if (typeof(IService).IsAssignableFrom(param.ParameterType))
                                {
                                    var mockServiceType = typeof(Mock<>).MakeGenericType(param.ParameterType);

                                    dynamic obj = mockServiceType.GetConstructor(new Type[] { }).Invoke(new object[] { });
                                    listParamValues.Add((object)obj.Object);
                                }
                            }
                        }
                        var message = string.Format("Method {0}.{1} does not raise ArgumentNullException for parameter {2}", plugin.GetType().Name, step.Method.Name, nullParameterName);
                        AssertException<ArgumentNullException>(() =>
                        {
                            step.Method.Invoke(plugin, listParamValues.ToArray());
                        }, (e) => { StringAssert.Contains(e.Message, nullParameterName, message); }, message);
                    }
                }

                return hasError;
            }, true);
        }

        private ICollection<Step> GetTestedSteps(Plugin plugin)
        {
            var allTestTypes = this.GetType().Assembly.GetTypes();

            var genericType = typeof(PluginTestClass<>);

            var pluginTestType = genericType.MakeGenericType(plugin.GetType());

            var testClasses = allTestTypes.Where(t => pluginTestType.IsAssignableFrom(t));

            var steps = new List<Step>();

            foreach (var testClass in testClasses)
            {
                var testMethods = testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(TestMethodAttribute), false).FirstOrDefault() != null);

                foreach (var testMethod in testMethods)
                {
                    var context = (PluginContextAttribute)testMethod.GetCustomAttributes(typeof(PluginContextAttribute), false).FirstOrDefault();

                    if (context == null)
                    {
                        continue;
                    }

                    if (steps.Count(s => s.Stage == context.Stage && s.Message == context.Message && s.EntityName == context.EntityName) == 0)
                    {
                        steps.Add(new Step(plugin, context.Message, context.Stage, context.Mode, context.EntityName));
                    }
                }
            }
            return steps;
        }

        protected override void InitializeTest(IServiceContext context)
        {
        }
    }
}
