//HintName: ClientNamespace.Core.ISubService.logged.cs
using ClientNamespace.Core;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using XrmFramework;

namespace ClientNamespace.Core
{
    [DebuggerStepThrough, CompilerGenerated]
    public class LoggedISubService : LoggedIService, ISubService
    {
        protected new ISubService Service => (ISubService) base.Service;

        #region .ctor
        public LoggedISubService(IServiceContext context, ISubService service) : base(context, service)
        {
        }
        #endregion
    }
}
