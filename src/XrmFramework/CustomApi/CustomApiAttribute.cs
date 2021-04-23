using System;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CustomApiAttribute : Attribute
    {
        public string DisplayName { get; }
        public string Description { get; }
        public CustomApiBindingType BindingType { get; }
        public string BoundEntityLogicalName { get; }
        public bool IsFunction { get; }
        public bool IsPrivate { get; }
        public AllowedCustomProcessingStep AllowedCustomProcessing { get; }
        public string ExecutePrivilegeName { get; }

        public CustomApiAttribute(
            string displayName,
            CustomApiBindingType bindingType,
            bool isFunction,
            string description = null,
            string boundEntityLogicalName = null,
            bool isPrivate = false,
            AllowedCustomProcessingStep allowedCustomProcessing = AllowedCustomProcessingStep.None,
            string executePrivilegeName = null
        )
        {
            DisplayName = displayName;
            Description = description;
            BindingType = bindingType;
            BoundEntityLogicalName = boundEntityLogicalName;
            IsFunction = isFunction;
            IsPrivate = isPrivate;
            AllowedCustomProcessing = allowedCustomProcessing;
            ExecutePrivilegeName = executePrivilegeName;
        }
    }
}