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
	public class LoggedSystemuserService : LoggedService, ISystemuserService
	{
		private SystemuserService Service { get; set; }

		#region .ctor
		public LoggedSystemuserService(IServiceContext context) : base(context)
		{
			Service = new SystemuserService(context);
		}

		public LoggedSystemuserService(IOrganizationService service) : this(new ServiceContextBase(service))
		{
		}
		#endregion

		public Boolean IsActiveUser(EntityReference  userRef)
		{
			#region Parameters check
			if (userRef == null)
			{
				throw new ArgumentNullException("userRef");
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("IsActiveUser", "Start: userRef = {0}", userRef);

			var returnValue = Service.IsActiveUser( userRef);

			Log("IsActiveUser", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}
	}
}
