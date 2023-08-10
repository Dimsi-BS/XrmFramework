//HintName: ClientNamespace.Core.ISub3Service.logged.cs
#if !DISABLE_SERVICES
using ClientNamespace.Core;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using XrmFramework;

namespace ClientNamespace.Core
{
    [DebuggerStepThrough, CompilerGenerated]
    public class LoggedISub3Service : LoggedIService, ISub3Service
    {
        protected new ISub3Service Service => (ISub3Service) base.Service;

        #region .ctor
        public LoggedISub3Service(IServiceContext context, ISub3Service service) : base(context, service)
        {
        }
        #endregion
    }
}
#endif