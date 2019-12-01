// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Model;
using Plugins;

namespace Contoso.Plugins
{
    public class AccountPlugin : Plugin
    {
        /// <summary>
        /// AddSteps allows to generate SdkMessageProcessing items.
        /// </summary>
        protected override void AddSteps()
        {
        }

        public AccountPlugin(string unsecuredConfig, string securedConfig) : base(unsecuredConfig, securedConfig)
        {
        }
    }
}
