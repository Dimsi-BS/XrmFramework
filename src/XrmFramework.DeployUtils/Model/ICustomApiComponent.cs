using Microsoft.Xrm.Sdk;
using System;
using XrmFramework;

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

        Guid Id { get; set; }
        string Description { get; set; }
        string DisplayName { get; set; }
        string LogicalEntityName { get; set; }
        bool IsOptional { get; set; }
        OptionSetValue Type { get; set; }
        string Name { get; set; }
        RegistrationState RegistrationState { get; set; }
    }
}