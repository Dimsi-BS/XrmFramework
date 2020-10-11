using Microsoft.Xrm.Sdk;
using Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plugins.LoggedServices
{
    public class LoggedAccountService : LoggedService, IAccountService
    {
        protected new IAccountService Service => (IAccountService) base.Service;

        #region .ctor
        public LoggedAccountService(IServiceContext context, IAccountService service) : base(context, service)
        {
        }
        #endregion

        public ICollection<EntityReference> GetSubContactRefs(EntityReference accountRef)
        {
            #region Parameters check
            if (accountRef == null)
            {
                throw new ArgumentNullException(nameof(accountRef));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log("GetSubContactRefs", "Start: accountRef = {0}", accountRef);

            var returnValue = Service.GetSubContactRefs(accountRef);

            Log("GetSubContactRefs", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }
    }
}
