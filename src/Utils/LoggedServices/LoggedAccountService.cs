using System;
using Model;
using Microsoft.Crm.Sdk.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.Diagnostics;

namespace Plugins
{
	public class LoggedAccountService : LoggedService, IAccountService
	{
		private IAccountService Service { get; set; }

		#region .ctor
		public LoggedAccountService(IServiceContext context, IAccountService service) : base(context, service)
		{
			Service = service;

		}
		#endregion

		public ICollection<EntityReference> GetSubContactRefs(EntityReference  accountRef)
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

			var returnValue = Service.GetSubContactRefs( accountRef);

			Log("GetSubContactRefs", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}
	}
}
