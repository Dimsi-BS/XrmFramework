using System;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    public class DiffComponent
    {
        public ICrmComponent Component { get; set; }
        public Guid OriginalId { get; set; }
        public Guid OriginalParentId { get; set; }
        public RegistrationState DiffResult { get; set; }

        public DiffComponent(ICrmComponent component, RegistrationState state)
        {
            Component = component;
            DiffResult = state;
        }
        public DiffComponent()
        {
        }

    }
}
