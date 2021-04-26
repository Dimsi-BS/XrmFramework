// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
#if TOOLING
using System;
using System.Configuration;
using System.Net;
using Microsoft.Xrm.Tooling.Connector;

namespace XrmFramework.DeployUtils
{
    public static class ConnectionHelper
    {
        public static DisposableOrganizationService GetCrmServiceClientFromConnectionString(string connectionString)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            return new DisposableOrganizationService(new CrmServiceClient(connectionString));
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

#endif