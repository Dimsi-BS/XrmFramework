using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Plugins
{
    [ExcludeFromCodeCoverage]
    public class MockServiceContext : IServiceContext
    {
        private readonly LogHelper _log;

        public MockServiceContext(IOrganizationService service)
        {
            _log = new LogHelper((message, args) => { });
            AdminOrganizationService = service;
            OrganizationService = service;
        }

        public MockServiceContext(IOrganizationService service, TraceLogger log)
        {
            _log = new LogHelper(log);
            AdminOrganizationService = service;
            OrganizationService = service;
        }

        public MockServiceContext(IOrganizationService service, IOrganizationService adminService)
        {
            _log = new LogHelper((message, args) => { });
            AdminOrganizationService = adminService;
            OrganizationService = service;
        }

        public MockServiceContext(IOrganizationService service, IOrganizationService adminService, TraceLogger log)
        {
            _log = new LogHelper(log);
            AdminOrganizationService = adminService;
            OrganizationService = service;
        }

        public Microsoft.Xrm.Sdk.IOrganizationService AdminOrganizationService
        {
            get;
            private set;
        }

        public Microsoft.Xrm.Sdk.IOrganizationService OrganizationService
        {
            get;
            private set;
        }

        public EntityReference BusinessUnitRef
        {
            get;
            set;
        }

        public Guid CorrelationId { get; } = Guid.NewGuid();

        public Logger Logger => _log.Log;

        public Guid UserId
        {
            get;
            set;
        }

        public Guid InitiatingUserId
        {
            get;
            set;
        }

        public void Log(string message, params object[] paramsObject)
        {
            _log.LogMethod(message, paramsObject);
        }

        public void ThrowInvalidPluginException(string messageId, params object[] args)
        {
        }


        public IOrganizationService GetService(Guid userId)
        {
            return OrganizationService;
        }
    }
}
