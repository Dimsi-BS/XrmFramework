// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Moq;

namespace Plugins.Tests.Objects
{
    [ExcludeFromCodeCoverage]
    public class MockServiceProvider : Mock<IServiceProvider>, IServiceProvider
    {
        public MockPluginExecutionContext PluginExecutionContext { get; } = new MockPluginExecutionContext();

        public Mock<ITracingService> TracingService { get; } = new Mock<ITracingService>();

        public Mock<IOrganizationServiceFactory> OrganizationServiceFactory { get; } = new Mock<IOrganizationServiceFactory>();

        public MockServiceProvider()
        {
            Setup(s => s.GetService(typeof(IPluginExecutionContext))).Returns(PluginExecutionContext);
            Setup(s => s.GetService(typeof(ITracingService))).Returns(TracingService.Object);
            Setup(s => s.GetService(typeof(IOrganizationServiceFactory))).Returns(PluginExecutionContext.OrganizationServiceFactory.Object);
        }

        public object GetService(Type serviceType) => Object.GetService(serviceType);
    }
}
