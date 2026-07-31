using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("XrmFramework.Analyzers.Test, PublicKey=0024000004800000940000000602000000240000525341310004000001000100514e5b42bacdbe31124ad199574b4550d639d4cc44e09b91d0e8f83c5034466dcfa5b9fcbce542367076648a2ad93a23e8d6c0c4634dc74847b83836afce964610cb2a0d45d65c5a642413688378f86a247acdd3a3f0ee60ed4a1ff45d8341adb7d6ce65f074a692faf58f27014cf4a51f205c9613b4b3460e676f2b98692bcc")]

namespace XrmFramework.Analyzers
{
    public static class DiagnosticIds
    {
        public const string Xrm0010Id = "XRM0010";
        public const string Xrm0002Id = "XRM0002";
        public const string Xrm0003Id = "XRM0003";
        public const string Xrm0011Id = "XRM0011";
        public const string Xrm0012Id = "XRM0012";
        public const string Xrm0013Id = "XRM0013";
        public const string Xrm0014Id = "XRM0014";
        public const string Xrm0100Id = "XRM0100";
        public const string Xrm0101Id = "XRM0101";
        public const string Xrm0102Id = "XRM0102";
        public const string Xrm0200Id = "XRM0200";
        public const string Xrm0300Id = "XRM0300";

        /// <summary>
        /// Base URL of the published analyzer documentation. Each rule has an anchor named
        /// after its lowercased id (e.g. <c>#xrm0010</c>).
        /// </summary>
        public const string HelpLinkBase = "https://github.com/Dimsi-BS/XrmFramework/blob/main/docs/Analyzers.md";

        /// <summary>
        /// Builds the documentation URL surfaced by the IDE (the diagnostic's <c>helpLinkUri</c>)
        /// for the given rule id, e.g. <c>HelpLink("XRM0010")</c> ->
        /// <c>.../docs/Analyzers.md#xrm0010</c>.
        /// </summary>
        public static string HelpLink(string ruleId) => $"{HelpLinkBase}#{ruleId.ToLowerInvariant()}";
    }
}
