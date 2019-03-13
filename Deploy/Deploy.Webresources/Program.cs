using XrmProject;

namespace Deploy.WebResources
{
    class Program
    {
        static void Main(string[] args)
        {
            WebResourceHelper.SyncWebResources(@"..\..\..\..\WebResources\", "Webresources");
        }
    }
}
