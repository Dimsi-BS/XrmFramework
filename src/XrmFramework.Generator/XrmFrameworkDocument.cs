
namespace XrmFramework.Parser
{
    public class XrmFrameworkDocument
    {
        public XrmFrameworkDocument(XrmFrameworkTable feature, XrmFrameworkDocumentLocation documentLocation)
        {
            DocumentLocation = documentLocation;
        }

        public XrmFrameworkTable XrmFrameworkTable => (XrmFrameworkTable) Table;

        public XrmFrameworkDocumentLocation DocumentLocation { get; private set; }

        public string SourceFilePath => DocumentLocation?.SourceFilePath;
    }
}
