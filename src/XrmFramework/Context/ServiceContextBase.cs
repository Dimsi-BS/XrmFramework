// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework
{
    public class ServiceContextBase : IServiceContext
    {
        public LogServiceMethod LogServiceMethod => Logger.LogWithMethodName;

        public ServiceContextBase(IOrganizationService service)
        {
            if (Logger == null)
            {
                LoggerFactory.GetLogger(this, Console.WriteLine);
            }
            AdminOrganizationService = service;
            OrganizationService = service;
            CorrelationId = Guid.NewGuid();

            var response = (WhoAmIResponse)service.Execute(new WhoAmIRequest());
            UserId = response.UserId;
            InitiatingUserId = response.UserId;
            BusinessUnitRef = new EntityReference("businessunit", response.BusinessUnitId);

            var organization = service.Retrieve("organization", response.OrganizationId, new ColumnSet("name"));
            OrganizationName = organization.GetAttributeValue<string>("name");
        }

        public ServiceContextBase(IOrganizationService service, LogMethod log) : this(service)
        {
            Logger = LoggerFactory.GetLogger(this, log);
        }

        public ServiceContextBase(IOrganizationService service, IOrganizationService adminService)
            : this(service)
        {
            AdminOrganizationService = adminService;
        }

        public ServiceContextBase(IOrganizationService service, IOrganizationService adminService, LogMethod log)
            : this(service, log)
        {
            AdminOrganizationService = adminService;
        }

        public Microsoft.Xrm.Sdk.IOrganizationService AdminOrganizationService
        {
            get;
            private set;
        }

        public Guid CorrelationId { get; }

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

        public string OrganizationName { get; set; }

        public ILogger Logger { get; }

        public void Log(string message, params object[] paramsObject)
        {
            Logger.Log(message, paramsObject);
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
