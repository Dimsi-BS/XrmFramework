// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using System;
using System.Configuration;
using System.Net;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using XrmConnectionTooling;
using XrmFramework.Common.Utils;

namespace Model
{
    public static class ConnectionHelper
    {
        public static IOrganizationService GetOrganizationService(string connectionStringName)
        {
            var client = GetCrmServiceClient(connectionStringName);
            return client;
        }

        public static OrganizationServiceProxy GetCrmServiceClientFromConnectionString(string connectionString)
        {
            var cs = ConnectionStringParser.Parse(connectionString);

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var client = new ManagedTokenOrganizationServiceProxy(new Uri(new Uri(cs.Url), "/XRMServices/2011/Organization.svc"), new ClientCredentials { UserName = { UserName = cs.Username, Password = cs.Password } });

            return client;
        }

        public static OrganizationServiceProxy GetCrmServiceClient(string connectionStringName)
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
