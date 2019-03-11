using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using Model;
using Plugins;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Plugins
{
    public class LocalContext : IContext, IServiceContext
    {
        private readonly IList<object> _loadedServices = new List<object>();

        private IOrganizationService _adminService;

        private EntityReference _businessUnitRef;

        public EntityReference BusinessUnitRef => _businessUnitRef;

        protected LocalContext() { }

        protected LogHelper LogHelper { get; private set; }

        public Guid UserId => ExecutionContext.UserId;

        public Guid InitiatingUserId => ExecutionContext.InitiatingUserId;

        public Guid CorrelationId => ExecutionContext.CorrelationId;

        //public Guid CorrelationId => ExecutionContext.CorrelationId;

        public LocalContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            // Obtain the execution context service from the service provider.
            ExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the tracing service from the service provider.
            TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the Organization Service factory service from the service provider
            Factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            OrganizationService = Factory.CreateOrganizationService(ExecutionContext.UserId);

            _businessUnitRef = new EntityReference("businessunit", ExecutionContext.BusinessUnitId);

            LogHelper = new LogHelper(TracingService.Trace);
        }

        public LocalContext(CodeActivityContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Obtain the execution context service from the service provider.
            ExecutionContext = context.GetExtension<IWorkflowContext>();

            // Obtain the tracing service from the service provider.
            TracingService = context.GetExtension<ITracingService>();

            // Obtain the Organization Service factory service from the service provider
            Factory = context.GetExtension<IOrganizationServiceFactory>();

            // Use the factory to generate the Organization Service.
            OrganizationService = Factory.CreateOrganizationService(ExecutionContext.UserId);

            _businessUnitRef = new EntityReference("businessunit", ExecutionContext.BusinessUnitId);

            LogHelper = new LogHelper(TracingService.Trace);
        }

        protected LocalContext(LocalContext context, IPluginExecutionContext parentContext)
        {
            ExecutionContext = parentContext;
            TracingService = context.TracingService;
            Factory = context.Factory;
            OrganizationService = context.OrganizationService;
            _businessUnitRef = context._businessUnitRef;
            LogHelper = context.LogHelper;
        }

        private T GetService<T>() where T : IService
        {
            return new ServiceChannel<T>(this).Service;
        }

        public object GetService(Type type)
        {
            return ServiceManager.GetService(type, this);
        }

        public Logger Logger => LogHelper.Log;

        private IOrganizationServiceFactory Factory { get; set; }

        public IOrganizationService OrganizationService { get; protected set; }

        public IExecutionContext ExecutionContext { get; private set; }

        public ITracingService TracingService { get; protected set; }

        public IOrganizationService AdminOrganizationService => _adminService ?? (_adminService = Factory.CreateOrganizationService(null));

        public Messages MessageName
        {
            get
            {
                Messages message = Messages.Default;
                if (!string.IsNullOrEmpty(ExecutionContext.MessageName) && !Enum.TryParse(ExecutionContext.MessageName, out message))
                {
                    throw new InvalidPluginExecutionException($"The message name \"{ExecutionContext.MessageName}\" is not registered in the MessageName enum");
                }
                return message;
            }
        }

        //public virtual void Trace(string message)
        //{
        //    if (string.IsNullOrWhiteSpace(message) || TracingService == null)
        //    {
        //        return;
        //    }

        //    if (ExecutionContext == null)
        //    {
        //        TracingService.Trace(message);
        //    }
        //    else
        //    {
        //        TracingService.Trace(
        //            "{0}, Correlation Id: {1}, Initiating User: {2}",
        //            message,
        //            ExecutionContext.CorrelationId,
        //            ExecutionContext.InitiatingUserId);
        //    }
        //}

        public void Log(string message, params object[] args)
        {
            LogHelper.LogMethod(message, args);
        }


        #region Image Helpers
        public bool HasPreImage(string imageName)
        {
            return ExecutionContext.PreEntityImages.ContainsKey(imageName);
        }
        public virtual Entity GetPreImage(string imageName)
        {
            VerifyPreImage(imageName);
            return ExecutionContext.PreEntityImages[imageName];
        }
        public bool HasPostImage(string imageName)
        {
            return ExecutionContext.PostEntityImages.ContainsKey(imageName);
        }
        public virtual Entity GetPostImage(string imageName)
        {
            VerifyPostImage(imageName);
            return ExecutionContext.PostEntityImages[imageName];
        }
        protected void VerifyPreImage(string imageName)
        {
            VerifyImage(ExecutionContext.PreEntityImages, imageName, true);
        }

        protected void VerifyPostImage(string imageName)
        {
            VerifyImage(ExecutionContext.PostEntityImages, imageName, false);
        }

        private void VerifyImage(EntityImageCollection collection, string imageName, bool isPreImage)
        {
            if (!collection.Contains(imageName)
                || collection[imageName] == null)
            {
                throw new ArgumentNullException(imageName, $"{(isPreImage ? "PreImage" : "PostImage")} {imageName} does not exist in this context");
            }
        }
        #endregion

        #region Message/Stage/Mode Helpers
        public virtual bool IsCreate()
        {
            return IsMessage(Messages.Create);
        }

        public virtual bool IsUpdate()
        {
            return IsMessage(Messages.Update);
        }

        public virtual bool IsMessage(Messages message)
        {
            return ExecutionContext.MessageName == message.ToString();
        }

        public virtual bool IsSynchronous()
        {
            return IsMode(Modes.Synchronous);
        }

        public virtual bool IsAsynchronous()
        {
            return IsMode(Modes.Asynchronous);
        }

        private bool IsMode(Modes mode)
        {
            return Mode == mode;
        }
        #endregion

        #region Entitys
        public virtual void DumpSharedVariables()
        {
            if (ExecutionContext.SharedVariables != null)
            {
                var sb = new StringBuilder();

                foreach (var key in ExecutionContext.SharedVariables.Keys)
                {
                    var parameter = ExecutionContext.SharedVariables[key];

                    LogHelper.DumpObject(key, parameter);
                }
            }
        }


        public virtual void DumpInputParameters()
        {
            foreach (var inputParameter in ExecutionContext.InputParameters)
            {
                switch (inputParameter.Key)
                {
                    case "ExtensionData":
                    case "Parameters":
                    case "RequestId":
                    case "RequestName":
                        break;
                    default:
                        DumpInputParameter(inputParameter.Key);
                        break;
                }
            }
        }

        private void DumpInputParameter(InputParameters parameterName)
        {
            VerifyInputParameter(parameterName);

            var parameter = ExecutionContext.InputParameters[parameterName.ToString()];

            LogHelper.DumpObject(parameterName.ToString(), parameter);
        }

        private void DumpInputParameter(string parameterName)
        {
            var parameter = ExecutionContext.InputParameters[parameterName];
            LogHelper.DumpObject(parameterName, parameter);
        }


        #endregion

        #region Parameters Helpers
        public virtual T GetInputParameter<T>(InputParameters parameterName)
        {
            VerifyInputParameter(parameterName);

            if (typeof(T).IsEnum)
            {
                var value = (OptionSetValue)ExecutionContext.InputParameters[parameterName.ToString()];
                return (T)Enum.ToObject(typeof(T), value.Value);
            }
            else
            {
                return (T)ExecutionContext.InputParameters[parameterName.ToString()];
            }
        }

        public void SetInputParameter<T>(InputParameters parameterName, T parameterValue)
        {
            ExecutionContext.InputParameters[parameterName.ToString()] = parameterValue;
        }

        public virtual T GetOutputParameter<T>(OutputParameters parameterName)
        {
            return (T)ExecutionContext.OutputParameters[parameterName.ToString()];
        }

        public void SetOutputParameter<T>(OutputParameters parameterName, T parameterValue)
        {
            ExecutionContext.OutputParameters[parameterName.ToString()] = parameterValue;
        }

        protected void VerifyInputParameter(InputParameters parameterName)
        {
            if (!ExecutionContext.InputParameters.Contains(parameterName.ToString())
                || ExecutionContext.InputParameters[parameterName.ToString()] == null)
            {
                throw new ArgumentNullException(nameof(parameterName), $"InputParameter {parameterName} does not exist in this context");
            }
        }

        public bool HasSharedVariable(string variableName)
        {
            return ExecutionContext.SharedVariables.ContainsKey(variableName);
        }

        public void SetSharedVariable<T>(string variableName, T value)
        {
            if (typeof(T).IsEnum)
            {
                ExecutionContext.SharedVariables[variableName] = Enum.GetName(typeof(T), value);
            }
            else
            {
                ExecutionContext.SharedVariables[variableName] = value;
            }
        }

        public virtual T GetSharedVariable<T>(string variableName)
        {
            T value = default(T);

            if (ExecutionContext.SharedVariables.ContainsKey(variableName))
            {
                if (typeof(T).IsEnum)
                {
                    var valueTemp = (string)ExecutionContext.SharedVariables[variableName];

                    value = (T)Enum.Parse(typeof(T), valueTemp);
                }
                else
                {
                    value = (T)ExecutionContext.SharedVariables[variableName];
                }
            }

            return value;
        }

        #endregion

        #region User Error Message
        public virtual void ThrowInvalidPluginException(string messageName, params object[] formatArguments)
        {
            int orgLanguage = RetrieveOrganizationBaseLanguageCode(OrganizationService);
            int userLanguage = RetrieveUserUiLanguageCode(OrganizationService, ExecutionContext.InitiatingUserId);
            String orgResourceFile = GetResourceFileName(orgLanguage);
            String resourceFile = GetResourceFileName(userLanguage, orgResourceFile);

            XmlDocument messages = RetrieveXmlWebResourceByName(resourceFile);
            String message = RetrieveLocalizedStringFromWebResource(messages, messageName);
            if (formatArguments == null || formatArguments.Length == 0)
            {
                throw new InvalidPluginExecutionException(message);
            }
            else
            {
                throw new InvalidPluginExecutionException(string.Format(CultureInfo.InvariantCulture, message, formatArguments));
            }
        }

        private static string GetResourceFileName(int language, string defaultResourceName = null)
        {
            string resourceFile;
            switch (language)
            {
                case 1033:
                    resourceFile = "pchmcs_/xml/PluginMessages.en_US.xml";
                    break;
                case 1041:
                    resourceFile = "pchmcs_/xml/PluginMessages.ja_JP.xml";
                    break;
                case 1031:
                    resourceFile = "pchmcs_/xml/PluginMessages.de_DE.xml";
                    break;
                case 1036:
                    resourceFile = "pchmcs_/xml/PluginMessages.fr_FR.xml";
                    break;
                case 1034:
                    resourceFile = "pchmcs_/xml/PluginMessages.es_ES.xml";
                    break;
                case 1049:
                    resourceFile = "pchmcs_/xml/PluginMessages.ru_RU.xml";
                    break;
                default:
                    resourceFile = string.IsNullOrEmpty(defaultResourceName) ? "pchmcs_/xml/PluginMessages.en_US.xml" : defaultResourceName;
                    break;
            }
            return resourceFile;
        }

        private static int RetrieveOrganizationBaseLanguageCode(IOrganizationService service)
        {
            QueryExpression organizationEntityQuery = new QueryExpression("organization");
            organizationEntityQuery.NoLock = true;
            organizationEntityQuery.ColumnSet.AddColumn("languagecode");
            EntityCollection organizationEntities = service.RetrieveMultiple(organizationEntityQuery);
            return (int)organizationEntities[0].Attributes["languagecode"];
        }

        private static int RetrieveUserUiLanguageCode(IOrganizationService service, Guid userId)
        {
            QueryExpression userSettingsQuery = new QueryExpression("usersettings");
            userSettingsQuery.NoLock = true;
            userSettingsQuery.ColumnSet.AddColumns("uilanguageid", "systemuserid");
            userSettingsQuery.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, userId);
            EntityCollection userSettings = service.RetrieveMultiple(userSettingsQuery);
            if (userSettings.Entities.Count > 0)
            {
                return (int)userSettings.Entities[0]["uilanguageid"];
            }
            return 0;
        }

        private XmlDocument RetrieveXmlWebResourceByName(string webresourceSchemaName)
        {
            TracingService.Trace("Begin:RetrieveXmlWebResourceByName, webresourceSchemaName={0}", webresourceSchemaName);
            QueryExpression webresourceQuery = new QueryExpression("webresource")
            {
                NoLock = true
            };
            webresourceQuery.ColumnSet.AddColumn("content");
            webresourceQuery.Criteria.AddCondition("name", ConditionOperator.Equal, webresourceSchemaName);
            EntityCollection webresources = OrganizationService.RetrieveMultiple(webresourceQuery);
            TracingService.Trace("Webresources Returned from server. Count={0}", webresources.Entities.Count);
            if (webresources.Entities.Count > 0)
            {
                byte[] bytes = Convert.FromBase64String((string)webresources.Entities[0]["content"]);
                // The bytes would contain the ByteOrderMask. Encoding.UTF8.GetString() does not remove the BOM.
                // Stream Reader auto detects the BOM and removes it on the text
                XmlDocument document = new XmlDocument();
                document.XmlResolver = null;
                MemoryStream ms = new MemoryStream(bytes);
                using (StreamReader sr = new StreamReader(ms))
                {
                    document.Load(sr);
                }
                TracingService.Trace("End:RetrieveXmlWebResourceByName , webresourceSchemaName={0}", webresourceSchemaName);
                return document;
            }
            else
            {
                TracingService.Trace("{0} Webresource missing. Reinstall the solution", webresourceSchemaName);
                throw new InvalidPluginExecutionException($"Unable to locate the web resource {webresourceSchemaName}.");
            }
        }

        private string RetrieveLocalizedStringFromWebResource(XmlDocument resource, string resourceId)
        {
            XmlNode valueNode = resource.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "./root/data[@name='{0}']/value", resourceId));
            if (valueNode != null)
            {
                return valueNode.InnerText;
            }
            else
            {
                TracingService.Trace("No Node Found for {0} ", resourceId);
                throw new InvalidPluginExecutionException($"ResourceID {resourceId} was not found.");
            }
        }
        #endregion

        public Modes Mode
        {
            get
            {
                if (!Enum.IsDefined(typeof(Modes), ExecutionContext.Mode))
                {
                    throw new InvalidPluginExecutionException(string.Format("Mode {0} is not part of modes enum", ExecutionContext.Mode));
                }
                return (Modes)ExecutionContext.Mode;

            }
        }


        public IOrganizationService GetService(Guid userId)
        {
            return Factory.CreateOrganizationService(userId);
        }

        public void LogFields(Entity entity, params string[] fieldNames)
        {
            foreach (var fieldName in fieldNames)
            {
                if (entity.Contains(fieldName))
                {
                    LogHelper.DumpObject(fieldName, entity[fieldName]);
                }
                else
                {
                    Log("{0} not present in {1}", fieldName, entity.LogicalName);
                }
            }
        }
    }
}
