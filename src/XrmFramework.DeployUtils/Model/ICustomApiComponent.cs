using Microsoft.Xrm.Sdk;
using System;

namespace Deploy
{
    public interface ICustomApiComponent
    {
        /// <summary>
        /// Unique identifier of the plug-in type associated with the step.
        /// </summary>
        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("customapiid")]
        public Microsoft.Xrm.Sdk.EntityReference CustomApiId {get; set;}

        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("uniquename")]
        public string UniqueName {get; set;}

        public Guid Id { get; set; }

        public Entity ToEntity();
        public EntityReference ToEntityReference();
    }
}