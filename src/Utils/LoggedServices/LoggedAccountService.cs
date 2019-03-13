// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
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
		private AccountService Service { get; set; }

		#region .ctor
		public LoggedAccountService(IServiceContext context) : base(context)
		{
			Service = new AccountService(context);
		}

		public LoggedAccountService(IOrganizationService service) : this(new ServiceContextBase(service))
		{
		}
		#endregion

		public ICollection<EntityReference> GetSubContactRefs(EntityReference  accountRef)
		{
			#region Parameters check
			if (accountRef == null)
			{
				throw new ArgumentNullException("accountRef");
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
