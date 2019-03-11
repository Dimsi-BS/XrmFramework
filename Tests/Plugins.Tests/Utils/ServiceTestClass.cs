using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace Plugins.Tests
{
    public abstract class ServiceTestClass<T> : TestClass where T : DefaultService
    {

        protected T Service { get; private set; }

        protected sealed override void InitializeTest(IServiceContext context)
        {
            var constructor = typeof(T).GetConstructor(new Type[] { typeof(IServiceContext) });
            List<object> arguments = new List<object>();

            if (constructor == null)
            {
                constructor = typeof(T).GetConstructor(new Type[] { typeof(IOrganizationService) });
                arguments.Add(OrganizationService);
            }
            else
            {
                arguments.Add(ServiceContext);
            }

            OrganizationService.Setup(s => s.Execute(It.IsAny<WhoAmIRequest>())).Returns(new WhoAmIResponse());

            Service = (T)constructor.Invoke(arguments.ToArray());
        }
    }
}
