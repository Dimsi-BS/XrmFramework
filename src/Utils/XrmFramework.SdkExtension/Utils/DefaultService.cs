// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Plugins
{
    public partial class DefaultService : IService
    {
        private readonly IServiceContext _context;

        protected Logger Log { get; }

        public DefaultService(IServiceContext context)
        {
            _context = context;
            Log = context.Logger;
        }

        protected IOrganizationService OrganizationService => _context.OrganizationService;

        protected IOrganizationService AdminOrganizationService => _context.AdminOrganizationService;

        protected EntityReference BusinessUnitRef => _context.BusinessUnitRef;

        protected Guid UserId => _context.UserId;

        protected Guid InitiatingUserId => _context.InitiatingUserId;

        protected Guid CorrelationId => _context.CorrelationId;

        protected void ThrowInvalidPluginException(string messageId, params object[] args)
        {
            _context.ThrowInvalidPluginException(messageId, args);
        }

        public Guid Create(Entity entity, bool useAdmin = false)
        {
            #region Parameters check
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var service = GetService(useAdmin);

            return service.Create(entity);
        }

        public Guid Create(Entity entity, Guid callerId)
        {
            #region Parameters check
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var service = GetService(callerId);

            return service.Create(entity);
        }

        public UpsertResponse Upsert(Entity entity, bool useAdmin = false)
        {
            #region Parameters check
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var service = GetService(useAdmin);

            var request = new UpsertRequest { Target = entity };

            return (UpsertResponse)service.Execute(request);
        }

        public UpsertResponse Upsert(Entity entity, Guid callerId)
        {
            #region Parameters check
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var service = GetService(callerId);

            var request = new UpsertRequest { Target = entity };

            return (UpsertResponse)service.Execute(request);
        }

        public void Update(Entity entity, bool useAdmin = false)
        {
            #region Parameters check
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (entity.Id == Guid.Empty)
            {
                throw new ArgumentNullException("entity.Id");
            }
            #endregion

            var service = GetService(useAdmin);

            service.Update(entity);
        }

        public void Update(Entity entity, Guid callerId)
        {
            #region Parameters check
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (entity.Id == Guid.Empty)
            {
                throw new ArgumentNullException("entity.Id");
            }
            #endregion

            var service = GetService(callerId);

            service.Update(entity);
        }

        public void Delete(EntityReference objectReference, Guid callerId)
        {
            #region Parameters check
            if (objectReference == null)
            {
                throw new ArgumentNullException(nameof(objectReference));
            }
            #endregion

            var service = GetService(callerId);

            service.Execute(new DeleteRequest { Target = objectReference });
        }

        public void Delete(string logicalName, Guid id, Guid callerId)
        {
            #region Parameters check
            if (string.IsNullOrEmpty(logicalName))
            {
                throw new ArgumentNullException(nameof(logicalName));
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            #endregion

            Delete(new EntityReference(logicalName, id), callerId);
        }

        public void Delete(string logicalName, Guid id, bool useAdmin = false)
        {
            #region Parameters check
            if (string.IsNullOrEmpty(logicalName))
            {
                throw new ArgumentNullException(nameof(logicalName));
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            #endregion

            Delete(new EntityReference(logicalName, id), useAdmin);
        }

        public void Delete(EntityReference objectReference, bool useAdmin = false)
        {
            #region Parameters check
            if (objectReference == null)
            {
                throw new ArgumentNullException(nameof(objectReference));
            }
            #endregion

            var service = GetService(useAdmin);

            service.Execute(new DeleteRequest { Target = objectReference });
        }

        public void AssignEntity(EntityReference objectReference, EntityReference ownerRef)
        {
            #region Parameters check
            if (objectReference == null)
            {
                throw new ArgumentNullException(nameof(objectReference));
            }
            #endregion

            AdminOrganizationService.Execute(new AssignRequest
            {
                Assignee = ownerRef ?? new EntityReference("systemuser", InitiatingUserId),
                Target = objectReference
            });
        }

        public void SetState(EntityReference objectRef, int stateCode, int statusCode, bool useAdmin = false)
        {
            #region Parameters check
            if (objectRef == null)
            {
                throw new ArgumentNullException(nameof(objectRef));
            }
            #endregion

            var service = GetService(useAdmin);

            service.Execute(new SetStateRequest
            {
                EntityMoniker = objectRef,
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statusCode)
            });
        }

        public void Share(EntityReference objectRef, EntityReference assigneeRef, AccessRights accessRights)
        {
            #region Parameters check
            if (objectRef == null)
            {
                throw new ArgumentNullException(nameof(objectRef));
            }
            if (assigneeRef == null)
            {
                throw new ArgumentNullException(nameof(assigneeRef));
            }
            #endregion

            AdminOrganizationService.Execute(new GrantAccessRequest
            {
                Target = objectRef,
                PrincipalAccess = new PrincipalAccess
                {
                    AccessMask = accessRights,
                    Principal = assigneeRef
                }
            });
        }

        public void UnShare(EntityReference objectRef, EntityReference revokeeRef, EntityReference callerRef = null)
        {
            #region Parameters check
            if (objectRef == null)
            {
                throw new ArgumentNullException(nameof(objectRef));
            }
            if (revokeeRef == null)
            {
                throw new ArgumentNullException(nameof(revokeeRef));
            }
            #endregion

            var service = GetService(callerRef?.Id ?? Guid.Empty);

            service.Execute(new RevokeAccessRequest
            {
                Target = objectRef,
                Revokee = revokeeRef
            });
        }

        protected IOrganizationService GetService(Guid callerId)
        {
            if (callerId == Guid.Empty)
            {
                return GetService(true);
            }

            return _context.GetService(callerId);
        }

        protected IOrganizationService GetService(bool useAdmin)
        {
            var service = OrganizationService;

            if (useAdmin)
            {
                service = AdminOrganizationService;
            }
            return service;
        }

        private Entity RetrieveInternal(EntityReference objectRef, ColumnSet cs)
        {
            var request = new RetrieveRequest
            {
                Target = objectRef,
                ColumnSet = cs
            };

            return ((RetrieveResponse)AdminOrganizationService.Execute(request)).Entity;
        }

        public Entity Retrieve(string entityName, Guid id, params string[] columns)
        {
            #region Parameters check
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException(nameof(entityName));
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }
            if (columns.Length == 0)
            {
                throw new ArgumentNullException("columns.Length");
            }
            #endregion

            return RetrieveInternal(new EntityReference(entityName, id), new ColumnSet(columns));
        }


        public Entity Retrieve(string entityName, Guid id, bool allColumns)
        {
            #region Parameters check
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException(nameof(entityName));
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (!allColumns)
            {
                throw new ArgumentNullException(nameof(allColumns));
            }
            #endregion

            return RetrieveInternal(new EntityReference(entityName, id), new ColumnSet(allColumns));
        }

        public Entity Retrieve(EntityReference objectRef, params string[] columns)
        {
            #region Parameters check
            if (objectRef == null)
            {
                throw new ArgumentNullException(nameof(objectRef));
            }
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }
            if (columns.Length == 0)
            {
                throw new ArgumentNullException("columns.Length");
            }
            #endregion

            return RetrieveInternal(objectRef, new ColumnSet(columns));
        }


        public Entity Retrieve(EntityReference objectRef, bool allColumns)
        {
            #region Parameters check
            if (objectRef == null)
            {
                throw new ArgumentNullException(nameof(objectRef));
            }
            if (!allColumns)
            {
                throw new ArgumentNullException(nameof(allColumns));
            }
            #endregion

            return RetrieveInternal(objectRef, new ColumnSet(allColumns));
        }

        public string GetOptionSetNameFromValue(string optionsetName, int optionsetValue)
        {
            #region Parameters check
            if (string.IsNullOrEmpty(optionsetName))
            {
                throw new ArgumentNullException(nameof(optionsetName));
            }
            if (optionsetValue < 0)
            {
                throw new ArgumentNullException(nameof(optionsetValue));
            }
            #endregion

            string optionsetSelectedText = string.Empty;

            RetrieveOptionSetRequest retrieveOptionSetRequest = new RetrieveOptionSetRequest { Name = optionsetName };

            //Execute the request
            RetrieveOptionSetResponse retrieveOptionSetResponse = (RetrieveOptionSetResponse)OrganizationService.Execute(retrieveOptionSetRequest);

            //Access the retrieved OptionSetMetaData
            OptionSetMetadata retrievedOptionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;

            //Get the current option lists for the retrieved attribute
            foreach (OptionMetadata optionMetadata in retrievedOptionSetMetadata.Options)
            {
                if (optionMetadata.Value == optionsetValue)
                {
                    optionsetSelectedText = optionMetadata.Label.UserLocalizedLabel.Label;
                    break;
                }
            }

            return optionsetSelectedText;
        }

        public string GetOptionSetNameFromValue<T>(int optionsetValue)
        {
            #region Parameters check
            if (optionsetValue < 0)
            {
                throw new ArgumentNullException(nameof(optionsetValue));
            }
            #endregion

            var optionSetattribute = typeof(T).GetCustomAttributes(typeof(OptionSetDefinitionAttribute), false).FirstOrDefault() as OptionSetDefinitionAttribute;

            if (!typeof(T).IsEnum || optionSetattribute == null)
            {
                throw new InvalidPluginExecutionException("The type specified is not an OptionSet Enum type.");
            }

            return GetOptionSetNameFromValue(optionSetattribute.LogicalName, optionsetValue);
        }

        public T GetById<T>(Guid id) where T : IBindingModel, new()
        {
            return AdminOrganizationService.GetById<T>(id);
        }

        public T GetById<T>(EntityReference entityReference) where T : IBindingModel, new()
        {
            return AdminOrganizationService.GetById<T>(entityReference);
        }


        public void AddUsersToTeam(EntityReference teamRef, params EntityReference[] userRefs)
        {
            var request = new AddMembersTeamRequest
            {
                MemberIds = userRefs.Select(u => u.Id).ToArray(),
                TeamId = teamRef.Id
            };

            AdminOrganizationService.Execute(request);
        }

        public void RemoveUsersFromTeam(EntityReference teamRef, params EntityReference[] userRefs)
        {
            var request = new RemoveMembersTeamRequest
            {
                MemberIds = userRefs.Select(u => u.Id).ToArray(),
                TeamId = teamRef.Id
            };

            AdminOrganizationService.Execute(request);
        }
    }
}
