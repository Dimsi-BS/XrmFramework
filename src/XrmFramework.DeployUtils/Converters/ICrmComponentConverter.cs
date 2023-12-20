using Microsoft.Xrm.Sdk;
using XrmFramework.DeployUtils.Model;
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
    }
}
