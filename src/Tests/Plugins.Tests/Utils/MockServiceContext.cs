// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
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
        private readonly ILogger _log;

        public MockServiceContext(IOrganizationService service)
        {
            _log = new DefaultLogHelper(service, this, (message, args) => { });
            AdminOrganizationService = service;
            OrganizationService = service;
        }

        public MockServiceContext(IOrganizationService service, LogMethod log)
        {
            _log = new DefaultLogHelper(service, this, log);
            AdminOrganizationService = service;
            OrganizationService = service;
        }

        public MockServiceContext(IOrganizationService service, IOrganizationService adminService)
        {
            _log = new DefaultLogHelper(adminService, this, (message, args) => { });
            AdminOrganizationService = adminService;
            OrganizationService = service;
        }

        public MockServiceContext(IOrganizationService service, IOrganizationService adminService, LogMethod log)
        {
            _log = new DefaultLogHelper(adminService, this, log);
            AdminOrganizationService = adminService;
            OrganizationService = service;
        }

        public IOrganizationService AdminOrganizationService
        {
            get;
            private set;
        }

        public IOrganizationService OrganizationService
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
        public string OrganizationName { get; set; }

        public LogServiceMethod LogServiceMethod => _log.LogWithMethodName;

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
            _log.Log(message, paramsObject);
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
