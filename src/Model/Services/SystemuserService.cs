// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Model;
using System;

namespace Plugins
{
    /// <summary>
    /// All system User service methods
    /// </summary>
    public class SystemUserService : DefaultService, ISystemUserService
    {
        #region .ctor

        public SystemUserService(IServiceContext context) : base(context)
        {
        }

        #endregion

        /// <summary>
        /// Check if user is disable
        /// </summary>
        /// <param name="userRef"></param>
        /// <returns>
        /// True : user is Active
        /// False : user is Inactive
        /// </returns>
        public bool IsActiveUser(EntityReference userRef)
        {
            //var user = OrganizationService.Retrieve(SystemUserDefinition.EntityName, userRef.Id,
            //                                        new ColumnSet(SystemUserDefinition.Columns.IsDisabled));

            //if (user != null && user.Attributes.Contains(SystemUserDefinition.Columns.IsDisabled))
            //    return !user.GetAttributeValue<bool>(SystemUserDefinition.Columns.IsDisabled);

            return false;
        }
    }

}

