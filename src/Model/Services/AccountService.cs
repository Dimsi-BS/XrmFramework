// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugins
{
    /// <summary>
    /// All Services related to Account 
    /// </summary>
    public class AccountService : DefaultService, IAccountService
    {
        #region .ctor

        public AccountService(IServiceContext context) : base(context)
        {
        }

        #endregion

    }
}
