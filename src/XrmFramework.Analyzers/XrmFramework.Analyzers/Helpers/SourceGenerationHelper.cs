namespace XrmFramework.Analyzers.Helpers
{
    public static class SourceGenerationHelper
    {
        public const string EnumExtensionsAttribute = @"
namespace XrmFramework
{
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    public class EnumExtensionsAttribute : System.Attribute
    {
    }
}";
    }
}
