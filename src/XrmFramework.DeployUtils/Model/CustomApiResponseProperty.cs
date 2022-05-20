using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{

    /// <summary>
    /// Component of a <see cref="CustomApi"/>, defines an output
    /// </summary>
    /// <seealso cref="XrmFramework.DeployUtils.Model.ICustomApiComponent" />
    /// <seealso cref="XrmFramework.DeployUtils.Model.ICrmComponent" />
    public class CustomApiResponseProperty : ICustomApiComponent
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string EntityTypeName => CustomApiResponsePropertyDefinition.EntityName;
        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public string Description { get; set; }
        public string DisplayName { get; set; }
        public bool IsOptional { get; set; }
        public OptionSetValue Type { get; set; }
        public string Name { get; set; }

        public string UniqueName { get; set; }
        public int Rank => 2;
        public bool DoAddToSolution => true;
        public bool DoFetchTypeCode => true;

        #region ICrmComponent dummy Implementation
        public IEnumerable<ICrmComponent> Children => new List<ICrmComponent>();
        public void AddChild(ICrmComponent child) => throw new ArgumentException("CustomApiResponseProperty doesn't take children");

        public void CleanChildrenWithState(RegistrationState state)
        {
        }
        #endregion
    }
}
