// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.IdentityModel.Tokens;
//using System.DirectoryServices.AccountManagement;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace WebApiTrainingSupport.CRM_SDK
{
    internal sealed class ManagedTokenOrganizationServiceProxy : OrganizationServiceProxy
    {
        private readonly AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService> _proxyManager;

        public ManagedTokenOrganizationServiceProxy(Uri serviceUri, ClientCredentials userCredentials)
            : base(serviceUri, null, userCredentials, null)
        {
            this._proxyManager = new AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService>(this);
        }

        public ManagedTokenOrganizationServiceProxy(IServiceManagement<IOrganizationService> serviceManagement,
            SecurityTokenResponse securityTokenRes)
            : base(serviceManagement, securityTokenRes)
        {
            this._proxyManager = new AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService>(this);
        }

        public ManagedTokenOrganizationServiceProxy(IServiceManagement<IOrganizationService> serviceManagement,
            ClientCredentials userCredentials)
            : base(serviceManagement, userCredentials)
        {
            this._proxyManager = new AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService>(this);
        }

        protected override void AuthenticateCore()
        {
            this._proxyManager.PrepareCredentials();
            base.AuthenticateCore();
        }

        protected override void ValidateAuthentication()
        {
            this._proxyManager.RenewTokenIfRequired();
            base.ValidateAuthentication();
        }

        protected override Guid CreateCore(Entity entity)
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
        }

        protected override void UpdateCore(Entity entity)
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
        }

        protected override void AssociateCore(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
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
        }

        protected override void DisassociateCore(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
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
        }

        protected override void DeleteCore(string entityName, Guid id)
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
        }

        protected override OrganizationResponse ExecuteCore(OrganizationRequest request)
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
        }

        protected override Entity RetrieveCore(string entityName, Guid id, ColumnSet columnSet)
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
        }

        protected override EntityCollection RetrieveMultipleCore(QueryBase query)
        {
            try
            {
                return base.RetrieveMultipleCore(query);
            }
            catch (Exception ex) when (ex is SecurityTokenValidationException || ex is ExpiredSecurityTokenException || ex is SecurityAccessDeniedException || ex is SecurityNegotiationException)
            {
                ValidateAuthentication();
                return base.RetrieveMultipleCore(query);
            }
        }
    }
}