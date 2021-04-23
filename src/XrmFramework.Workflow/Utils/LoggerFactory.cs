
#if STANDALONE
using System;

namespace XrmFramework.Workflow
{
    class LoggerFactory
    {
        internal static ILogger GetLogger(LocalContext localContext, Action<string, object[]> trace)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
