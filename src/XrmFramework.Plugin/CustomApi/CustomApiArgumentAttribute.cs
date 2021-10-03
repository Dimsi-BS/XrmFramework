using System;

namespace XrmFramework
{
    public sealed class CustomApiInputAttribute : CustomApiArgumentAttribute
    {
    }

    public sealed class CustomApiOutputAttribute : CustomApiArgumentAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class CustomApiArgumentAttribute : Attribute
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public string LogicalEntityName { get; set; }

        public bool IsOptional { get; set; }
    }
}