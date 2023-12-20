using Microsoft.Xrm.Sdk;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Model
{
    /// <summary>
    /// Component of a <see cref="CustomApi"/>, defines an input
    /// </summary>
    /// <seealso cref="ICustomApiComponent" />
    /// <seealso cref="ICrmComponent" />
    public class CustomApiRequestParameter : BaseCrmComponent, ICustomApiComponent
    {
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public bool IsOptional { get; set; }
        public OptionSetValue Type { get; set; }
        public string Name { get; set; }

        #region ICrmComponent implementation
        public override string EntityTypeName => CustomApiRequestParameterDefinition.EntityName;
        public override string UniqueName { get; set; }
        public override int Rank => 20;
        public override bool DoAddToSolution => true;
        public override bool DoFetchTypeCode => true;
        #endregion

        #region ICrmComponent dummy implementation
        public override IEnumerable<ICrmComponent> Children => Enumerable.Empty<ICrmComponent>();

        public override void AddChild(ICrmComponent child) => throw new ArgumentException("CustomApiRequestParameter doesn't take children");

        protected override void RemoveChild(ICrmComponent child) { }
        #endregion
    }
}