// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using System;
using System.Configuration;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using XrmConnectionTooling;

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

            var client = new ManagedTokenOrganizationServiceProxy(new Uri(new Uri(cs.Url), "/XRMServices/2011/Organization.svc"), new ClientCredentials { UserName = { UserName = cs.Username, Password = cs.Password } });

            return client;
        }

        public static OrganizationServiceProxy GetCrmServiceClient(string connectionStringName)
        {
            return GetCrmServiceClientFromConnectionString(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString);
        }
    }
}
