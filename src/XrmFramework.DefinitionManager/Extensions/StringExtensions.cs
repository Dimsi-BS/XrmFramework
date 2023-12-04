namespace XrmFramework.DefinitionManager.Extensions
{
    public static class StringExtensions
    {
        public static string EscapeQuotes(this string value)
            => value?.Replace("\"", "\\\"");
    }
}
