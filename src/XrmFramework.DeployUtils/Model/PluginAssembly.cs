using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace Deploy
{
    partial class PluginAssembly : ISolutionComponent
    {
        public RegistrationState RegistrationState { get; set; }

        public string EntityTypeName => PluginAssemblyDefinition.EntityName;

        public Entity ToRegisterComponent(IRegistrationContext context)
        {
            return this;
        }
    }
}
