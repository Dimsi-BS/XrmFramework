namespace XrmFramework
{
    // ReSharper disable once UnusedTypeParameter
    public sealed class CustomApiInArgument<T> : CustomApiArgument
    {
        public CustomApiInArgument(string name, CustomApiArgumentType type, bool isSerialized, string description, string displayName, string logicalEntityName, bool isOptional) 
        : base(true, name, type, isSerialized, description, displayName, logicalEntityName, isOptional) { }
    }

    // ReSharper disable once UnusedTypeParameter
    public sealed class CustomApiOutArgument<T> : CustomApiArgument
    {
        public CustomApiOutArgument(string name, CustomApiArgumentType type, bool isSerialized, string description, string displayName, string logicalEntityName, bool isOptional) 
        : base(false, name, type, isSerialized, description, displayName, logicalEntityName, isOptional) { }
    }

    public abstract class CustomApiArgument
    {
        public bool IsInArgument { get; }
        public string ArgumentName { get; }
        public CustomApiArgumentType ArgumentType { get; }
        public bool IsSerializedArgument { get; }
        public string Description { get; }
        public string DisplayName { get; }
        public string LogicalEntityName { get; }
        public bool IsOptional { get; }

        protected CustomApiArgument(bool isInArgument, string name, CustomApiArgumentType type, bool isSerialized, string description, string displayName, string logicalEntityName, bool isOptional)
        {
            IsInArgument = isInArgument;
            ArgumentName = name;
            ArgumentType = type;
            IsSerializedArgument = isSerialized;
            Description = description;
            DisplayName = displayName;
            LogicalEntityName = logicalEntityName;
            IsOptional = isOptional;
        }
    }
}