using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Deploy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;

namespace Plugins.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceCommonTests : TestClass
    {
        [TestMethod]
        public void InstantiateAllServices()
        {
            var mockOrganizationService = new Mock<IOrganizationService>();

            ObjectHelper<DefaultService>.ApplyCode(new Type[] { typeof(IServiceContext) }, new object[] { new MockServiceContext(mockOrganizationService.Object) }, null);
        }

        [TestMethod]
        public void ServiceMethodTestCoverage()
        {
            var mockOrganizationService = new Mock<IOrganizationService>();

            ObjectHelper<DefaultService>.ApplyCode(new Type[] { typeof(IServiceContext) }, new object[] { new MockServiceContext(mockOrganizationService.Object) }, (service, serviceType, sb) =>
            {
                if (serviceType == null)
                {
                    serviceType = typeof(IService);
                }

                var hasError = false;
                var declaredMethods = serviceType.GetMethods().Select(m => m.Name).ToList();

                var testedMethods = GetTestedMethods(service);

                var notTestedMethods = declaredMethods.Except(testedMethods);
                var notDeclaredMethods = testedMethods.Except(declaredMethods);

                if (notTestedMethods.Any() || notDeclaredMethods.Any())
                {
                    hasError = true;
                    sb.AppendFormat("\r\nService {0} :\r\n", service.GetType().Name);
                    if (notTestedMethods.Any())
                    {
                        sb.AppendLine("\tNot tested methods :");
                        foreach (var methodName in notTestedMethods)
                        {
                            sb.AppendFormat("\t\t- {0}\r\n", methodName);
                        }
                    }
                    if (notDeclaredMethods.Any())
                    {
                        sb.AppendLine("\tNot declared methods :");
                        foreach (var methodName in notDeclaredMethods)
                        {
                            sb.AppendFormat("\t\t- {0}\r\n", methodName);
                        }
                    }
                }
                return hasError;
            }, false);
        }

        private ICollection<string> GetTestedMethods(DefaultService service)
        {
            var allTestTypes = this.GetType().Assembly.GetTypes();

            var genericType = typeof(ServiceTestClass<>);

            var serviceTestType = genericType.MakeGenericType(service.GetType());

            var testClasses = allTestTypes.Where(t => serviceTestType.IsAssignableFrom(t));

            var steps = new List<string>();

            foreach (var testClass in testClasses)
            {
                var testMethods = testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(TestMethodAttribute), false).FirstOrDefault() != null);

                foreach (var testMethod in testMethods)
                {
                    var methodAttribute = testMethod.GetCustomAttributes(typeof(ServiceMethodNameAttribute), false).FirstOrDefault() as ServiceMethodNameAttribute;

                    if (methodAttribute != null && !string.IsNullOrEmpty(methodAttribute.MethodName))
                    {
                        steps.Add(methodAttribute.MethodName);
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
