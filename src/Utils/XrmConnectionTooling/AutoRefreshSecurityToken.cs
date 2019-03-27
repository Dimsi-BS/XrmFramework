// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Client;

namespace XrmConnectionTooling
{
    public sealed class AutoRefreshSecurityToken<TProxy, TService>
       where TProxy : ServiceProxy<TService>
       where TService : class
    {
        private readonly TProxy _proxy;

        /// <summary>
        /// Instantiates an instance of the proxy class
        /// </summary>
        /// <param name="proxy">Proxy that will be used to authenticate the user</param>
        public AutoRefreshSecurityToken(TProxy proxy)
        {
            _proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
        }

        /// <summary>
        /// Prepares authentication before authen6ticated
        /// </summary>
        public void PrepareCredentials()
        {
            if (null == _proxy.ClientCredentials)
            {
                return;
            }

            switch (_proxy.ServiceConfiguration.AuthenticationType)
            {
                case AuthenticationProviderType.ActiveDirectory:
                    _proxy.ClientCredentials.UserName.UserName = null;
                    _proxy.ClientCredentials.UserName.Password = null;
                    break;
                case AuthenticationProviderType.Federation:
                case AuthenticationProviderType.LiveId:
                    _proxy.ClientCredentials.Windows.ClientCredential = null;
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Renews the token (if it is near expiration or has expired)
        /// </summary>
        public void RenewTokenIfRequired()
        {
            if (null != _proxy.SecurityTokenResponse &&
                DateTime.UtcNow.AddMinutes(15) >= _proxy.SecurityTokenResponse.Response.Lifetime.Expires)
            {
                try
                {
                    _proxy.Authenticate();
                }
                catch (CommunicationException)
                {
                    if (null == _proxy.SecurityTokenResponse ||
                        DateTime.UtcNow >= _proxy.SecurityTokenResponse.Response.Lifetime.Expires)
                    {
                        throw;
                    }

                    // Ignore the exception 
                }
            }
        }
    }
}