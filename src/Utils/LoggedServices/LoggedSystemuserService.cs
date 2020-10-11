using Microsoft.Xrm.Sdk;
using Plugins;
using System;
using System.Diagnostics;

namespace Plugins.LoggedServices
{
    public class LoggedSystemUserService : LoggedService, ISystemUserService
    {
        protected new ISystemUserService Service => (ISystemUserService) base.Service;

        #region .ctor
        public LoggedSystemUserService(IServiceContext context, ISystemUserService service) : base(context, service)
        {
        }
        #endregion

        public bool IsActiveUser(EntityReference userRef)
        {
            #region Parameters check
            if (userRef == null)
            {
                throw new ArgumentNullException(nameof(userRef));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log("IsActiveUser", "Start: userRef = {0}", userRef);

            var returnValue = Service.IsActiveUser(userRef);

            Log("IsActiveUser", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }
    }
}
