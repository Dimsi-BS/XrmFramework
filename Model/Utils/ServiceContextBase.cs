using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugins
{
    public class ServiceContextBase : IServiceContext
    {
        private LogHelper LogHelper {get;set;}

        public ServiceContextBase(IOrganizationService service)
        {

            LogHelper = new LogHelper(Console.WriteLine);
            AdminOrganizationService = service;
            OrganizationService = service;
            CorrelationId = Guid.NewGuid();

            var response = (WhoAmIResponse)service.Execute(new WhoAmIRequest());
            UserId = response.UserId;
            InitiatingUserId = response.UserId;
            BusinessUnitRef = new EntityReference("businessunit", response.BusinessUnitId);

         }

        public ServiceContextBase(IOrganizationService service, TraceLogger log) : this(service)
        {
            LogHelper = new LogHelper(log);
        }

        public ServiceContextBase(IOrganizationService service, IOrganizationService adminService)
            : this(service)
        {
            AdminOrganizationService = adminService;
        }

        public ServiceContextBase(IOrganizationService service, IOrganizationService adminService, TraceLogger log)
            : this(service)
        {
            LogHelper = new LogHelper(log);
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

        public Logger Logger
        {
            get { return LogHelper.Log; }
        }

        public void Log(string message, params object[] paramsObject)
        {
            LogHelper.LogMethod(message, paramsObject);
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
