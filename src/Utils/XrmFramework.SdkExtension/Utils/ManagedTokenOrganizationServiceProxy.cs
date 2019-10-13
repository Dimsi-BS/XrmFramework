// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Threading;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace XrmConnectionTooling
{
    internal sealed class ManagedTokenOrganizationServiceProxy : OrganizationServiceProxy
    {
        private readonly AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService> _proxyManager;

        private const int RateLimitExceededErrorCode = -2147015902;
        private const int TimeLimitExceededErrorCode = -2147015903;
        private const int ConcurrencyLimitExceededErrorCode = -2147015898;

        private const int MaxRetries = 3;

        public ManagedTokenOrganizationServiceProxy(Uri serviceUri, ClientCredentials userCredentials)
            : base(serviceUri, null, userCredentials, null)
        {
            _proxyManager = new AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService>(this);
        }

        public ManagedTokenOrganizationServiceProxy(IServiceManagement<IOrganizationService> serviceManagement,
            SecurityTokenResponse securityTokenRes)
            : base(serviceManagement, securityTokenRes)
        {
            _proxyManager = new AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService>(this);
        }

        public ManagedTokenOrganizationServiceProxy(IServiceManagement<IOrganizationService> serviceManagement,
            ClientCredentials userCredentials)
            : base(serviceManagement, userCredentials)
        {
            _proxyManager = new AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService>(this);
        }

        protected override void AuthenticateCore()
        {
            _proxyManager.PrepareCredentials();
            base.AuthenticateCore();
        }

        protected override void ValidateAuthentication()
        {
            _proxyManager.RenewTokenIfRequired();
            base.ValidateAuthentication();
        }

        protected override Guid CreateCore(Entity entity)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    return base.CreateCore(entity);
                }
                catch (Exception ex) when (ex is SecurityTokenValidationException || ex is ExpiredSecurityTokenException || ex is SecurityAccessDeniedException || ex is SecurityNegotiationException)
                {
                    ValidateAuthentication();
                    return base.CreateCore(entity);
                }
                catch (FaultException<OrganizationServiceFault> e) when (IsTransientError(e) && ++retryCount < MaxRetries)
                {
                    ApplyDelay(e, retryCount);
                }
            }
        }

        protected override void UpdateCore(Entity entity)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    base.UpdateCore(entity);
                }
                catch (Exception ex) when (ex is SecurityTokenValidationException || ex is ExpiredSecurityTokenException || ex is SecurityAccessDeniedException || ex is SecurityNegotiationException)
                {
                    ValidateAuthentication();
                    base.UpdateCore(entity);
                }
                catch (FaultException<OrganizationServiceFault> e) when (IsTransientError(e) && ++retryCount < MaxRetries)
                {
                    ApplyDelay(e, retryCount);
                }
            }
        }

        protected override void AssociateCore(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    base.AssociateCore(entityName, entityId, relationship, relatedEntities);
                }
                catch (Exception ex) when (ex is SecurityTokenValidationException || ex is ExpiredSecurityTokenException || ex is SecurityAccessDeniedException || ex is SecurityNegotiationException)
                {
                    ValidateAuthentication();
                    base.AssociateCore(entityName, entityId, relationship, relatedEntities);
                }
                catch (FaultException<OrganizationServiceFault> e) when (IsTransientError(e) && ++retryCount < MaxRetries)
                {
                    ApplyDelay(e, retryCount);
                }
            }
        }

        protected override void DisassociateCore(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    base.DisassociateCore(entityName, entityId, relationship, relatedEntities);
                }
                catch (Exception ex) when (ex is SecurityTokenValidationException || ex is ExpiredSecurityTokenException || ex is SecurityAccessDeniedException || ex is SecurityNegotiationException)
                {
                    ValidateAuthentication();
                    base.DisassociateCore(entityName, entityId, relationship, relatedEntities);
                }
                catch (FaultException<OrganizationServiceFault> e) when (IsTransientError(e) && ++retryCount < MaxRetries)
                {
                    ApplyDelay(e, retryCount);
                }
            }
        }

        protected override void DeleteCore(string entityName, Guid id)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    base.DeleteCore(entityName, id);
                }
                catch (Exception ex) when (ex is SecurityTokenValidationException || ex is ExpiredSecurityTokenException || ex is SecurityAccessDeniedException || ex is SecurityNegotiationException)
                {
                    ValidateAuthentication();
                    base.DeleteCore(entityName, id);
                }
                catch (FaultException<OrganizationServiceFault> e) when (IsTransientError(e) && ++retryCount < MaxRetries)
                {
                    ApplyDelay(e, retryCount);
                }
            }
        }

        protected override OrganizationResponse ExecuteCore(OrganizationRequest request)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    return base.ExecuteCore(request);
                }
                catch (Exception ex) when (ex is SecurityTokenValidationException || ex is ExpiredSecurityTokenException || ex is SecurityAccessDeniedException || ex is SecurityNegotiationException)
                {
                    ValidateAuthentication();
                    return base.ExecuteCore(request);
                }
                catch (FaultException<OrganizationServiceFault> e) when (IsTransientError(e) && ++retryCount < MaxRetries)
                {
                    ApplyDelay(e, retryCount);
                }
            }
        }

        protected override Entity RetrieveCore(string entityName, Guid id, ColumnSet columnSet)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    return base.RetrieveCore(entityName, id, columnSet);
                }
                catch (Exception ex) when (ex is SecurityTokenValidationException || ex is ExpiredSecurityTokenException || ex is SecurityAccessDeniedException || ex is SecurityNegotiationException)
                {
                    ValidateAuthentication();
                    return base.RetrieveCore(entityName, id, columnSet);
                }
                catch (FaultException<OrganizationServiceFault> e) when (IsTransientError(e) && ++retryCount < MaxRetries)
                {
                    ApplyDelay(e, retryCount);
                }
            }
        }

        protected override EntityCollection RetrieveMultipleCore(QueryBase query)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    if (query is QueryExpression qe)
                    {
                        qe.NoLock = true;
                    }

                    return base.RetrieveMultipleCore(query);
                }
                catch (Exception ex) when (ex is SecurityTokenValidationException || ex is ExpiredSecurityTokenException || ex is SecurityAccessDeniedException || ex is SecurityNegotiationException)
                {
                    ValidateAuthentication();
                    return base.RetrieveMultipleCore(query);
                }
                catch (FaultException<OrganizationServiceFault> e) when (IsTransientError(e) && ++retryCount < MaxRetries)
                {
                    ApplyDelay(e, retryCount);
                }
            }
        }

        private static void ApplyDelay(FaultException<OrganizationServiceFault> e, int retryCount)
        {
            TimeSpan delay;
            if (e.Detail.ErrorCode == RateLimitExceededErrorCode)
            {
                // Use Retry-After delay when specified
                delay = (TimeSpan)e.Detail.ErrorDetails["Retry-After"];
            }
            else
            {
                // else use exponential backoff delay
                delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount));
            }

            Thread.Sleep(delay);
        }

        private static bool IsTransientError(FaultException<OrganizationServiceFault> ex)
        {
            // You can add more transient fault codes to retry here
            return ex.Detail.ErrorCode == RateLimitExceededErrorCode ||
                   ex.Detail.ErrorCode == TimeLimitExceededErrorCode ||
                   ex.Detail.ErrorCode == ConcurrencyLimitExceededErrorCode;
        }
    }
}