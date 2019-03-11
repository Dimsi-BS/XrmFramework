using Microsoft.Xrm.Sdk;

namespace Plugins
{
    /// <summary>
    /// All signatures of SystemUser services methods
    /// </summary>
    public interface ISystemuserService : IService
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
