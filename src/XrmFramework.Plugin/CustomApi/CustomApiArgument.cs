namespace XrmFramework
{
    public sealed class CustomApiInputAttribute : CustomApiArgumentAttribute
    {
        public CustomApiInputAttribute(string displayName = null, string description = null) : base(displayName, description)
        {
        }
    }

    public sealed class CustomApiOutputAttribute : CustomApiArgumentAttribute
    {
        public CustomApiOutputAttribute(string displayName = null, string description = null) : base(displayName, description)
        {
        }
    }

    public abstract class CustomApiArgument
    {
        public string ArgumentName { get; }
        public CustomApiArgumentType ArgumentType { get; }
        public bool IsSerializedArgument { get; }

        protected CustomApiArgument(string name, CustomApiArgumentType type, bool isSerialized)
        {
            ArgumentName = name;
            ArgumentType = type;
            IsSerializedArgument = isSerialized;
        }
    }
}