// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System.Reflection;

namespace Plugins.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServicesInstantiationTests
    {
        [TestMethod]
        public void CheckAllServiceInterfaces()
        {
            var types = typeof(Plugin).Assembly.GetTypes();

            var serviceTypes = types.Where(t => (typeof(IService).IsAssignableFrom(t)) && t.IsPublic && t.IsInterface);

            foreach (Type type in serviceTypes)
            {
                foreach (var method in type.GetMethods())
                {
                    foreach (var parameter in method.GetParameters())
                    {
                        if (parameter.IsOut)
                        {
                           // Assert.Fail(string.Format("The parameter {0} of method {1}.{2} is an out parameter", parameter.Name, type.Name, method.Name));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TryInstantiateAllServices()
        {
            var types = typeof(Plugin).Assembly.GetTypes();

            var serviceTypes = types.Where(t => (typeof(DefaultService).IsAssignableFrom(t)) && t.IsPublic && !t.IsAbstract);

            foreach (Type type in serviceTypes)
            {
                var defaultConstructor = type.GetConstructor(new Type[] { typeof(IServiceContext) });
                if (defaultConstructor == null)
                {
                    Assert.Fail(string.Format("The Service {0} does not have a default constructor", type.Name));
                }

                Assert.IsFalse(defaultConstructor.GetParameters().Any(p => p.ParameterType == typeof(LocalContext)), string.Format("The service {0} must have a constructor taking a IServiceContext parameter", type.Name));
            }
        }
    }
}
