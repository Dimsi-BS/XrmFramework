using Deploy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    public class LocalAssemblyContext : ILocalAssemblyContext
    {
        public LocalAssemblyContext()
        {

        }
        public LocalAssemblyContext(Type TPlugin)
        {
            Assembly = TPlugin.Assembly;

            CustomApis = new List<CustomApi>();
            CustomApiRequestParameters = new List<CustomApiRequestParameter>();
            CustomApiResponseProperties = new List<CustomApiResponseProperty>();
            Plugins = new List<Plugin>();
            Steps = new List<SdkMessageProcessingStep>();

        }
        public Assembly Assembly { get; set; }

        public IEnumerable<CustomApi> CustomApis { get; set; }
        public IEnumerable<CustomApiRequestParameter> CustomApiRequestParameters { get; set; }
        public IEnumerable<CustomApiResponseProperty> CustomApiResponseProperties { get; set; }
        public IEnumerable<Plugin> Plugins { get; set; }
        public IEnumerable<SdkMessageProcessingStep> Steps { get; set; }


        public string Name { get => Assembly.GetName().Name; }

        public PluginAssembly ToPluginAssembly()
        {
            var fullNameSplit = Assembly.FullName.Split(',');

            var name = fullNameSplit[0];
            var version = fullNameSplit[1].Substring(fullNameSplit[1].IndexOf('=') + 1);
            var culture = fullNameSplit[2].Substring(fullNameSplit[2].IndexOf('=') + 1);
            var publicKeyToken = fullNameSplit[3].Substring(fullNameSplit[3].IndexOf('=') + 1);
            var description = string.Format("{0} plugin assembly", name);

            var t = new PluginAssembly()
            {
                Name = name,
                SourceType = new OptionSetValue((int)pluginassembly_sourcetype.Database),
                IsolationMode = new OptionSetValue((int)pluginassembly_isolationmode.Sandbox),
                Culture = culture,
                PublicKeyToken = publicKeyToken,
                Version = version,
                Description = description,
                Content = Convert.ToBase64String(File.ReadAllBytes(Assembly.Location))
            };

            return t;
        }
        public PluginAssembly ToPluginAssembly(Guid registeredId)
        {
            return new PluginAssembly()
            {
                Id = registeredId,
                Content = Convert.ToBase64String(File.ReadAllBytes(Assembly.Location))
            };
        }

    }
}