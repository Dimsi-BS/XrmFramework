using XrmFramework.BindingModel;

namespace XrmFramework
{
    public interface IBusinessProcess : IBindingModel
    {
        string Name { get; set; }

        int Duration { get; set; }

        Microsoft.Xrm.Sdk.EntityReference ActiveStageId { get; set; }
    }
}