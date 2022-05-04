using Microsoft.Xrm.Sdk;

namespace XrmFramework.DeployUtils.Model
{
    /// <summary>
    /// Additional layer of <see cref="ICrmComponent"/> used for
    /// <see cref="Deploy.CustomApiRequestParameter"/> and
    /// <see cref="Deploy.CustomApiResponseProperty"/>
    /// </summary>
    public interface ICustomApiComponent : ICrmComponent
    {
        /// <summary>The unique name of the component</summary>
        new string UniqueName { get; set; }

        /// <summary>The description of the CustomApiComponent</summary>
        string Description { get; set; }

        /// <summary>The DisplayName of the CustomApiComponent</summary>
        string DisplayName { get; set; }

        /// <summary>Indicates whether or not the component is optional, for <see cref="Deploy.CustomApiRequestParameter"/> only</summary>
        bool IsOptional { get; set; }

        /// <summary>The Type of the CustomApiComponent</summary>
        OptionSetValue Type { get; set; }

        /// <summary>The Name of the CustomApiComponent</summary>
        string Name { get; set; }
    }
}