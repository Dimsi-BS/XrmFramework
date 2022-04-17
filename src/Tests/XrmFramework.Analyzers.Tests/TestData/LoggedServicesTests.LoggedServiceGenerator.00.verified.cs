//HintName: ClientNamespace.Core.IAccountService.logged.cs
using ClientNamespace.Core;
using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using XrmFramework;

namespace ClientNamespace.Core
{
    [DebuggerStepThrough, CompilerGenerated]
    public class LoggedIAccountService : LoggedIService, IAccountService
    {
        protected new IAccountService Service => (IAccountService) base.Service;

        #region .ctor
        public LoggedIAccountService(IServiceContext context, IAccountService service) : base(context, service)
        {
        }
        #endregion

        public int GetSubRecordCount(EntityReference recordRef)
        {
            #region Parameters check
            if (recordRef == default)
            {
                throw new ArgumentNullException(nameof(recordRef));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(GetSubRecordCount), "Start: recordRef = {0}", recordRef);

            var returnValue = Service.GetSubRecordCount(recordRef);

            Log(nameof(GetSubRecordCount), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public async Task AsynchronousCall(IEnumerable<double> list)
        {
            #region Parameters check
            if (list == default)
            {
                throw new ArgumentNullException(nameof(list));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(AsynchronousCall), "Start: list = {0}", list);

            await Service.AsynchronousCall(list);

            Log(nameof(AsynchronousCall), "End : duration = {0}", sw.Elapsed);
        }

        public bool TryGetValue(string key, out int value)
        {
            #region Parameters check
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(TryGetValue), "Start: key = {0}", key);

            var returnValue = Service.TryGetValue(key, value);

            Log(nameof(TryGetValue), "End : duration = {0}: , value = {1}, returnValue = {2}", sw.Elapsed, value, returnValue);

            return returnValue;
        }

        public U GetValue<T, U>(T argument)
        {
            #region Parameters check
            if (Equals(argument, default(T)))
            {
                throw new ArgumentNullException(nameof(argument));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(GetValue), "Start: argument = {0}", argument);

            var returnValue = Service.GetValue<T, U>(argument);

            Log(nameof(GetValue), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public void StringParameter(string parameter)
        {
            #region Parameters check
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(StringParameter), "Start: parameter = {0}", parameter);

            Service.StringParameter(parameter);

            Log(nameof(StringParameter), "End : duration = {0}", sw.Elapsed);
        }

        public void StringParameterDefaultValue(string parameter = null)
        {
            #region Parameters check
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(StringParameterDefaultValue), "Start: parameter = {0}", parameter);

            Service.StringParameterDefaultValue(parameter = null);

            Log(nameof(StringParameterDefaultValue), "End : duration = {0}", sw.Elapsed);
        }
    }
}
