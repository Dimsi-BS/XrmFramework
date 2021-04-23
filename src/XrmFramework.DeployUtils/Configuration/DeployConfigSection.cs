// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Configuration;

namespace XrmFramework.DeployUtils.Configuration
{
    public class DeployConfigSection : ConfigurationSection
    {

        [ConfigurationProperty("pluginSolutionUniqueName", IsRequired = true)]
        public string PluginSolutionUniqueName
        {
            get { return (string)this["pluginSolutionUniqueName"]; }
            set { this["pluginSolutionUniqueName"] = value; }
        }

        [ConfigurationProperty("entitiesSolutionUniqueName", IsRequired = true)]
        public string EntitiesSolutionUniqueName
        {
            get { return (string)this["entitiesSolutionUniqueName"]; }
            set { this["entitiesSolutionUniqueName"] = value; }
        }

        [ConfigurationProperty("webResourcesSolutionUniqueName", IsRequired = true)]
        public string WebResourcesSolutionUniqueName
        {
            get { return (string)this["webResourcesSolutionUniqueName"]; }
            set { this["webResourcesSolutionUniqueName"] = value; }
        }

        public List<string> SolutionList
        {
            get
            {
                var list = new List<string>();
                list.Add(PluginSolutionUniqueName);

                if (!list.Contains(EntitiesSolutionUniqueName))
                {
                    list.Add(EntitiesSolutionUniqueName);
                }

                if (!list.Contains(WebResourcesSolutionUniqueName))
                {
                    list.Add(WebResourcesSolutionUniqueName);
                }

                return list;
            }
        }
    }
}
