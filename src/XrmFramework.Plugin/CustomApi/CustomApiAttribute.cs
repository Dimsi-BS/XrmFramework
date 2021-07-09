using System;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CustomApiAttribute : Attribute
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public CustomApiBindingType BindingType { get; }
        public string BoundEntityLogicalName { get; set; }
        public bool IsFunction { get; set; }
        public bool IsPrivate { get; set; }
        public AllowedCustomProcessingStep AllowedCustomProcessing { get; set; }
        public string ExecutePrivilegeName { get; set; }
        public bool WorkflowSdkStepEnabled { get; set; }

        public CustomApiAttribute(
            CustomApiBindingType bindingType
        )
        {
            BindingType = bindingType;
        }
    }
}