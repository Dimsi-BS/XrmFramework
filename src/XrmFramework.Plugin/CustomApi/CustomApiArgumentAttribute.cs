using System;

namespace XrmFramework
{
    // ReSharper disable once UnusedTypeParameter
    public sealed class CustomApiInArgument<T> : CustomApiArgument
    {
        public CustomApiInArgument(string name, CustomApiArgumentType type, bool isSerialized) : base(name, type, isSerialized) { }
    }

    // ReSharper disable once UnusedTypeParameter
    public sealed class CustomApiOutArgument<T> : CustomApiArgument
    {
        public CustomApiOutArgument(string name, CustomApiArgumentType type, bool isSerialized) : base(name, type, isSerialized) { }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class CustomApiArgumentAttribute : Attribute
    {
        public string DisplayName { get; }
        public string Description { get; }

        protected CustomApiArgumentAttribute(string displayName = null, string description = null)
        {
            DisplayName = displayName;
            Description = description;
        }
    }
}