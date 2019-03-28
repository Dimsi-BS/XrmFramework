// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins
{
    public abstract class DebrayablePlugin : Plugin
    {
        #region .ctor
        protected DebrayablePlugin(string unsecuredConfig, string securedConfig) : base(unsecuredConfig, securedConfig) { }

        protected DebrayablePlugin() : base() { }
        #endregion

        protected bool CanExecute(IPluginContext context)
        {
            var canExecute = true;
            if (context.IsCreate() || context.IsUpdate())
            {
                var target = context.GetInputParameter<Entity>(InputParameters.Target);

                if (target.Contains("pchmcs_disableplugins") && target.GetAttributeValue<bool>("pchmcs_disableplugins"))
                {
                    canExecute = false;
                }
            }

            return canExecute;
        }
    }
}
