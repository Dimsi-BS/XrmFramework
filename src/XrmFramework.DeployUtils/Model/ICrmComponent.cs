using System;
using System.Collections.Generic;

namespace XrmFramework.DeployUtils.Model
{
    /// <summary>
    /// A Component of an Assembly to be deployed on the Crm,
    /// pretty much the local machine equivalent of an <see cref="Microsoft.Xrm.Sdk.Entity"/>
    /// </summary>
    public partial interface ICrmComponent
    {
        /// <summary>
        /// The unique name of the component
        /// </summary>
        string UniqueName { get; }

        /// <summary>
        /// The rank of the component, gives an idea of how many layers of other components this one depends on
        /// </summary>
        int Rank { get; }

        /// <summary>
        /// Indicates whether or not this components requires an <see cref="Microsoft.Crm.Sdk.Messages.AddSolutionComponentRequest"/>
        /// during export
        /// </summary>
        bool DoAddToSolution { get; }

        /// <summary>
        /// Indicates whether or not this components requires an EntityTypeCode
        /// during export
        /// </summary>
        bool DoFetchTypeCode { get; }

        /// <summary>
        /// Stores a custom state used for computing difference between Assemblies and for Deploy
        /// </summary>
        RegistrationState RegistrationState { get; set; }

        /// <summary>
        /// The Id of the component
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// The Id of the component this one depends on
        /// </summary>
        Guid ParentId { get; set; }

        /// <summary>
        /// The Logical Name this component is known as on the Crm
        /// </summary>
        string EntityTypeName { get; }

        /// <summary>
        /// Enumeration of the <see cref="ICrmComponent"/> that depend on this component
        /// </summary>
        IEnumerable<ICrmComponent> Children { get; }

        /// <summary>
        /// Adds a child to this component
        /// </summary>
        /// <remarks>
        /// Throws <see cref="ArgumentException"/> if this component doesn't accept the given <see cref="ICrmComponent"/>
        /// </remarks>
        /// <param name="child"></param>
        /// <exception cref="ArgumentException"/>
        void AddChild(ICrmComponent child);

        /// <summary>
        /// Removes recursively all children with a given <see cref="XrmFramework.RegistrationState"/><br/>
        /// A child and all its branch is kept if it or one member of its branch have a different <see cref="XrmFramework.RegistrationState"/>
        /// </summary>
        /// <param name="state"></param>
        void CleanChildrenWithState(RegistrationState state);
    }
}
