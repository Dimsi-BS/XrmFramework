using Microsoft.Xrm.Sdk;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Converters
{
    /// <summary>
    /// Handles the conversion from a <see cref="ICrmComponent"/> to an <see cref="Entity"/> to be deployed on the Crm
    /// </summary>
    public interface ICrmComponentConverter
    {
        /// <summary>
        /// Converts a <see cref="ICrmComponent"/> to the appropriate <see cref="Entity"/>
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        Entity ToRegisterComponent(ICrmComponent component);

        /// <summary>
        /// Converts a <see cref="ICrmComponent"/> to the <see cref="Entity"/> to send in an UPDATE.
        /// Identical to <see cref="ToRegisterComponent"/> for every component whose columns are all
        /// writable after creation, which is all of them but the plugin package - whose name and
        /// version the platform refuses to see again.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        Entity ToUpdateComponent(ICrmComponent component);
    }
}
