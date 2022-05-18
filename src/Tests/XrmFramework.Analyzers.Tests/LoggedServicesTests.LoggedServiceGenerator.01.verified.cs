//HintName: XrmFramework.IService.logged.cs
using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using XrmFramework;

namespace XrmFramework
{
    [DebuggerStepThrough, CompilerGenerated]
    public class LoggedIService : LoggedServiceBase, IService
    {

        #region .ctor
        public LoggedIService(IServiceContext context, IService service) : base(context, service)
        {
        }
        #endregion

        public  global::System.Guid Create(global::Microsoft.Xrm.Sdk.Entity entity)
        {
            #region Parameters check
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Create), "Start: entity = {0}", entity);

            var returnValue = Service.Create(entity);

            Log(nameof(Create), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  void Update(global::Microsoft.Xrm.Sdk.Entity entity)
        {
            #region Parameters check
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Update), "Start: entity = {0}", entity);

            Service.Update(entity);

            Log(nameof(Update), "End : duration = {0}", sw.Elapsed);
        }
    }
}
