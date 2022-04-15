using Microsoft.Xrm.Sdk;

namespace XrmFramework.DeployUtils.Model
{
    public interface ICustomApiComponent : ICrmComponent
    {
        new string UniqueName { get; set; }
        string Description { get; set; }
        string DisplayName { get; set; }
        string LogicalEntityName { get; }
        bool IsOptional { get; set; }
        OptionSetValue Type { get; set; }
        string Name { get; set; }
    }
}