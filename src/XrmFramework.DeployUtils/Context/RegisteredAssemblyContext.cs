using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Model;
using System.Reflection;
using Deploy;
using XrmFramework.DeployUtils.Service;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.DeployUtils.Context
{
    public class RegisteredAssemblyContext : IRegisteredAssemblyContext
    {        

        public RegisteredAssemblyContext( PluginAssembly assembly)
        {
            Assembly = assembly;
        }

        public PluginAssembly Assembly { get; set; }
        public Guid Id
        {
            get => Assembly.Id;
            set => Assembly.Id = value;
        }
        public string Name => Assembly.Name;
        public EntityReference EntityReference { get => Assembly.ToEntityReference(); }

        public ICollection<CustomApi> CustomApis { get; set; }
        public ICollection<CustomApiRequestParameter> CustomApiRequestParameters { get ; set; }
        public ICollection<CustomApiResponseProperty> CustomApiResponseProperties { get; set; }
        public ICollection<SdkMessageProcessingStep> Steps { get; set; }

        public ICollection<PluginType> PluginTypes { get; set; }      
        public ICollection<SdkMessageProcessingStepImage> ImageSteps { get; set; }


        public bool IsNull => Assembly == null;


    }
}
