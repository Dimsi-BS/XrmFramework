// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Moq;
using NUnit.Framework;

namespace XrmFramework.Tests.Service
{
    [TestFixture]
    public class LoggedServiceBaseTests
    {
        private class ConcreteLoggedService : LoggedServiceBase
        {
            public ConcreteLoggedService(IServiceContext context, IService service)
                : base(context, service) { }

            public IService ExposedService => Service;
            public LogServiceMethod ExposedLog => Log;
        }

        // ─────────────────────────────────────────────────────────────
        //  Constructor — property wiring
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Constructor_SetsServiceFromParameter()
        {
            var contextMock = BuildContextMock();
            var serviceMock = new Mock<IService>();

            var loggedService = new ConcreteLoggedService(contextMock.Object, serviceMock.Object);

            Assert.AreSame(serviceMock.Object, loggedService.ExposedService);
        }

        [Test]
        public void Constructor_SetsLogFromContext()
        {
            LogServiceMethod capturedLog = null;
            LogServiceMethod expectedLog = (_, __, ___) => { };

            var contextMock = BuildContextMock();
            contextMock.Setup(c => c.LogServiceMethod).Returns(expectedLog);

            var loggedService = new ConcreteLoggedService(contextMock.Object, new Mock<IService>().Object);

            Assert.AreSame(expectedLog, loggedService.ExposedLog);
        }

        [Test]
        public void Constructor_ServiceIsAccessibleFromSubclass()
        {
            var contextMock = BuildContextMock();
            var serviceMock = new Mock<IService>();

            var loggedService = new ConcreteLoggedService(contextMock.Object, serviceMock.Object);

            Assert.IsNotNull(loggedService.ExposedService);
        }

        [Test]
        public void Constructor_LogIsAccessibleFromSubclass()
        {
            LogServiceMethod expectedLog = (_, __, ___) => { };
            var contextMock = BuildContextMock();
            contextMock.Setup(c => c.LogServiceMethod).Returns(expectedLog);

            var loggedService = new ConcreteLoggedService(contextMock.Object, new Mock<IService>().Object);

            Assert.IsNotNull(loggedService.ExposedLog);
        }

        // ─────────────────────────────────────────────────────────────
        //  ILoggedService marker interface
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void LoggedServiceBase_ImplementsILoggedService()
        {
            var contextMock = BuildContextMock();
            var loggedService = new ConcreteLoggedService(contextMock.Object, new Mock<IService>().Object);

            Assert.IsInstanceOf<ILoggedService>(loggedService);
        }

        // ─────────────────────────────────────────────────────────────
        //  Helper
        // ─────────────────────────────────────────────────────────────

        private static Mock<IServiceContext> BuildContextMock()
        {
            var mock = new Mock<IServiceContext>();
            mock.Setup(c => c.LogServiceMethod).Returns((string _, string __, object[] ___) => { });
            mock.Setup(c => c.UserId).Returns(Guid.NewGuid());
            mock.Setup(c => c.InitiatingUserId).Returns(Guid.NewGuid());
            mock.Setup(c => c.CorrelationId).Returns(Guid.NewGuid());
            mock.Setup(c => c.OrganizationName).Returns("testorg");
            return mock;
        }
    }
}
