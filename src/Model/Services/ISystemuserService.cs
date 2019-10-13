// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;

namespace Plugins
{
    /// <summary>
    /// All signatures of SystemUser services methods
    /// </summary>
    public interface ISystemUserService : IService
    {

        /// <summary>
        /// Check if user is active or not
        /// </summary>
        /// <param name="userRef"></param>
        /// <returns>
        /// True : user is Active
        /// False : user is Inactive
        /// </returns>
        bool IsActiveUser(EntityReference userRef);
    }
}
