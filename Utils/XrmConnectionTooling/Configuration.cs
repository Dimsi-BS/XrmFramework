using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Web;

namespace WebApiTrainingSupport.CRM_SDK
{
    public class Configuration
    {
        public String ServerAddress;
        public String OrganizationName;
        public Uri DiscoveryUri;
        public Uri OrganizationUri;
        public Uri HomeRealmUri = null;
        public ClientCredentials DeviceCredentials = null;
        public ClientCredentials Credentials = null;
        public AuthenticationProviderType EndpointType;
        public String UserPrincipalName;
        #region internal members of the class
        internal IServiceManagement<IOrganizationService> OrganizationServiceManagement;
        internal SecurityTokenResponse OrganizationTokenResponse;
        internal Int16 AuthFailureCount = 0;
        #endregion

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            Configuration c = (Configuration)obj;

            if (!this.ServerAddress.Equals(c.ServerAddress, StringComparison.InvariantCultureIgnoreCase))
                return false;
            if (!this.OrganizationName.Equals(c.OrganizationName, StringComparison.InvariantCultureIgnoreCase))
                return false;
            if (this.EndpointType != c.EndpointType)
                return false;
            if (null != this.Credentials && null != c.Credentials)
            {
                if (this.EndpointType == AuthenticationProviderType.ActiveDirectory)
                {

                    if (!this.Credentials.Windows.ClientCredential.Domain.Equals(
                        c.Credentials.Windows.ClientCredential.Domain, StringComparison.InvariantCultureIgnoreCase))
                        return false;
                    if (!this.Credentials.Windows.ClientCredential.UserName.Equals(
                        c.Credentials.Windows.ClientCredential.UserName, StringComparison.InvariantCultureIgnoreCase))
                        return false;

                }
                else if (this.EndpointType == AuthenticationProviderType.LiveId)
                {
                    if (!this.Credentials.UserName.UserName.Equals(c.Credentials.UserName.UserName,
                        StringComparison.InvariantCultureIgnoreCase))
                        return false;
                    if (!this.DeviceCredentials.UserName.UserName.Equals(
                        c.DeviceCredentials.UserName.UserName, StringComparison.InvariantCultureIgnoreCase))
                        return false;
                    if (!this.DeviceCredentials.UserName.Password.Equals(
                        c.DeviceCredentials.UserName.Password, StringComparison.InvariantCultureIgnoreCase))
                        return false;
                }
                else
                {

                    if (!this.Credentials.UserName.UserName.Equals(c.Credentials.UserName.UserName,
                        StringComparison.InvariantCultureIgnoreCase))
                        return false;

                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int returnHashCode = this.ServerAddress.GetHashCode()
                ^ this.OrganizationName.GetHashCode()
                ^ this.EndpointType.GetHashCode();
            if (null != this.Credentials)
            {
                if (this.EndpointType == AuthenticationProviderType.ActiveDirectory)
                    returnHashCode = returnHashCode
                        ^ this.Credentials.Windows.ClientCredential.UserName.GetHashCode()
                        ^ this.Credentials.Windows.ClientCredential.Domain.GetHashCode();
                else if (this.EndpointType == AuthenticationProviderType.LiveId)
                    returnHashCode = returnHashCode
                        ^ this.Credentials.UserName.UserName.GetHashCode()
                        ^ this.DeviceCredentials.UserName.UserName.GetHashCode()
                        ^ this.DeviceCredentials.UserName.Password.GetHashCode();
                else
                    returnHashCode = returnHashCode
                        ^ this.Credentials.UserName.UserName.GetHashCode();
            }
            return returnHashCode;
        }

    }
}