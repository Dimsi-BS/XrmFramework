using System;

namespace XrmFramework
{
    public sealed class CustomApiInputAttribute : CustomApiArgumentAttribute
    {
        public CustomApiInputAttribute(string displayName = null, string description = null, string logicalEntityName = null, bool isOptional = false) : base(displayName, description, logicalEntityName, isOptional)
        {
        }
    }

    public sealed class CustomApiOutputAttribute : CustomApiArgumentAttribute
    {
        public CustomApiOutputAttribute(string displayName = null, string description = null, string logicalEntityName = null) : base(displayName, description, logicalEntityName)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class CustomApiArgumentAttribute : Attribute
    {
        public string DisplayName { get; }
        public string Description { get; }

        public string LogicalEntityName { get; }

        public bool IsOptional { get; }

        protected CustomApiArgumentAttribute(string displayName = null, string description = null, string logicalEntityName = null, bool isOptional = false)
        {
            DisplayName = displayName;
            Description = description;
            LogicalEntityName = logicalEntityName;
            IsOptional = isOptional;
        }
    }
}