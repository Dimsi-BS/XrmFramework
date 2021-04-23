// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Configuration;
using System.Net;
using Microsoft.Xrm.Sdk;
using XrmFramework.Utils;
#if NETCOREAPP3_1
using Microsoft.PowerPlatform.Dataverse.Client;
#else
using System.ServiceModel.Description;
#endif

namespace XrmFramework
{
    public static class ConnectionHelper
    {
        public static DisposableOrganizationService GetCrmServiceClientFromConnectionString(string connectionString)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            
#if NETCOREAPP3_1
            return new DisposableOrganizationService(new ServiceClient(connectionString));
#else
            var cs = ConnectionStringParser.Parse(connectionString);

            var client = new ManagedTokenOrganizationServiceProxy(new Uri(new Uri(cs.Url), "/XRMServices/2011/Organization.svc"), new ClientCredentials { UserName = { UserName = cs.Username, Password = cs.Password } });

            return new DisposableOrganizationService(client);
#endif
        }

        public static DisposableOrganizationService GetCrmServiceClient(string connectionStringName)
        {
            string connectionString;

            try
            {
                connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            }
            catch (Exception)
            {
                connectionString = ConfigurationManager.AppSettings[connectionStringName];
            }

            return GetCrmServiceClientFromConnectionString(connectionString);
        }
    }
}
