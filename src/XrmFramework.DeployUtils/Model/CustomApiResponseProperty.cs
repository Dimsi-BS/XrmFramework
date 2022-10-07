using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{

    /// <summary>
    /// Component of a <see cref="CustomApi"/>, defines an output
    /// </summary>
    /// <seealso cref="XrmFramework.DeployUtils.Model.ICustomApiComponent" />
    /// <seealso cref="XrmFramework.DeployUtils.Model.ICrmComponent" />
    public class CustomApiResponseProperty : BaseCrmComponent, ICustomApiComponent
    {

        public string Description { get; set; }
        public string DisplayName { get; set; }
        public bool IsOptional { get; set; }
        public OptionSetValue Type { get; set; }
        public string Name { get; set; }

        #region ICrmComponent implementation
        public override string EntityTypeName => CustomApiResponsePropertyDefinition.EntityName;
        public override string UniqueName { get; set; }
        public override int Rank => 20;
        public override bool DoAddToSolution => true;
        public override bool DoFetchTypeCode => true;
        #endregion

        #region ICrmComponent dummy Implementation
        public override IEnumerable<ICrmComponent> Children => Enumerable.Empty<ICrmComponent>();
        public override void AddChild(ICrmComponent child) => throw new ArgumentException("CustomApiResponseProperty doesn't take children");
        protected override void RemoveChild(ICrmComponent child) { }
        #endregion
    }
}
