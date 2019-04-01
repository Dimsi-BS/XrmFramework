using Microsoft.Xrm.Sdk;
using Model;

namespace Plugins
{
    public interface IBusinessProcess : IBindingModel
    {
        string Name { get; set; }

        int Duration { get; set; }

        EntityReference ActiveStageId { get; set; }
    }
}