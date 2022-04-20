using System;
using System.Collections.Generic;

namespace XrmFramework.DeployUtils.Model
{
    public partial interface ICrmComponent
    {
        string UniqueName { get; }
        int Rank { get; }
        bool DoAddToSolution { get; }
        bool DoFetchTypeCode { get; }
        RegistrationState RegistrationState { get; set; }
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        string EntityTypeName { get; }
        IEnumerable<ICrmComponent> Children { get; }
        void AddChild(ICrmComponent child);
        void CleanChildrenWithState(RegistrationState state);
    }
}
