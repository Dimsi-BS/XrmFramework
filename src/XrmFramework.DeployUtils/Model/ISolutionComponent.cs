using System;
using System.Collections.Generic;

namespace XrmFramework.DeployUtils.Model
{
    public interface ISolutionComponent
    {
        string UniqueName { get; }
        RegistrationState RegistrationState { get; set; }
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        string EntityTypeName { get; }
        IEnumerable<ISolutionComponent> Children { get; }
        void AddChild(ISolutionComponent child);
    }
}
