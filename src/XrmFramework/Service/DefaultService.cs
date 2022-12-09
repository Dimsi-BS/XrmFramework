// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using XrmFramework.BindingModel;
using XrmFramework.Definitions;
using XrmFramework.Model;
using Newtonsoft.Json;

namespace XrmFramework
{
    public partial class DefaultService : IService
    {
        private readonly IServiceContext _context;

        protected LogServiceMethod Log { get; }

        public DefaultService(IServiceContext context)
        {
            _context = context;
            Log = context.LogServiceMethod;
        }

        protected IOrganizationService OrganizationService => _context.OrganizationService;

        protected IOrganizationService AdminOrganizationService => _context.AdminOrganizationService;

        protected EntityReference BusinessUnitRef => _context.BusinessUnitRef;

        protected Guid UserId => _context.UserId;

        protected string OrganizationName => _context.OrganizationName;

        protected Guid InitiatingUserId => _context.InitiatingUserId;

        protected Guid CorrelationId => _context.CorrelationId;

        public Guid Create(Entity entity, bool useAdmin = false, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var service = GetService(useAdmin);

            var request = new CreateRequest { Target = entity };

          return Execute<CreateRequest, CreateResponse>(service, request, bypassCustomPluginExecution).id;
        }

        public Guid Create(Entity entity, Guid callerId, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var service = GetService(callerId);

            var request = new CreateRequest { Target = entity };
            
            return Execute<CreateRequest, CreateResponse>(service, request, bypassCustomPluginExecution).id;
        }

        public UpsertResponse Upsert(Entity entity, bool useAdmin = false, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var service = GetService(useAdmin);

            var request = new UpsertRequest { Target = entity };

            return Execute<UpsertRequest, UpsertResponse>(service, request, bypassCustomPluginExecution);
        }

        public UpsertResponse Upsert(Entity entity, Guid callerId, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var service = GetService(callerId);

            var request = new UpsertRequest { Target = entity };

            return Execute<UpsertRequest, UpsertResponse>(service, request, bypassCustomPluginExecution);
        }

        public void Update(Entity entity, bool useAdmin = false, bool bypassCustomPluginExecution = false)
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

            var request = new UpdateRequest { Target = entity };

            Execute<UpdateRequest, UpdateResponse>(service, request, bypassCustomPluginExecution);
        }

        public void Update(Entity entity, Guid callerId, bool bypassCustomPluginExecution = false)
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

            var request = new UpdateRequest { Target = entity };

            Execute<UpdateRequest, UpdateResponse>(service, request, bypassCustomPluginExecution);
        }

        public void Delete(EntityReference objectReference, Guid callerId, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (objectReference == null)
            {
                throw new ArgumentNullException(nameof(objectReference));
            }
            #endregion

            var service = GetService(callerId);

            var request = new DeleteRequest { Target = objectReference };

            Execute<DeleteRequest, DeleteResponse>(service, request, bypassCustomPluginExecution);
        }

        public void Delete(string logicalName, Guid id, Guid callerId, bool bypassCustomPluginExecution = false)
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

            Delete(new EntityReference(logicalName, id), callerId, bypassCustomPluginExecution);
        }

        public void Delete(string logicalName, Guid id, bool useAdmin = false, bool bypassCustomPluginExecution = false)
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

            Delete(new EntityReference(logicalName, id), useAdmin, bypassCustomPluginExecution);
        }

        public void Delete(EntityReference objectReference, bool useAdmin = false, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (objectReference == null)
            {
                throw new ArgumentNullException(nameof(objectReference));
            }
            #endregion

            var service = GetService(useAdmin);

            var request = new DeleteRequest { Target = objectReference };

            Execute<DeleteRequest, DeleteResponse>(service, request, bypassCustomPluginExecution);
        }

        public void AssignEntity(EntityReference objectReference, EntityReference ownerRef, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (objectReference == null)
            {
                throw new ArgumentNullException(nameof(objectReference));
            }
            #endregion

            Execute<AssignRequest, AssignResponse>(AdminOrganizationService, new AssignRequest
            {
                Assignee = ownerRef ?? new EntityReference("systemuser", InitiatingUserId),
                Target = objectReference
            }, bypassCustomPluginExecution);
        }

        public void SetState(EntityReference objectRef, int stateCode, int statusCode, bool useAdmin = false, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (objectRef == null)
            {
                throw new ArgumentNullException(nameof(objectRef));
            }
            #endregion

            var service = GetService(useAdmin);

            var request = new SetStateRequest
            {
                EntityMoniker = objectRef,
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statusCode)
            };

            Execute<SetStateRequest, SetStateResponse>(service, request, bypassCustomPluginExecution);
        }

        public void Share(EntityReference objectRef, EntityReference assigneeRef, AccessRights accessRights, bool bypassCustomPluginExecution = false)
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

            var request = new GrantAccessRequest
            {
                Target = objectRef,
                PrincipalAccess = new PrincipalAccess
                {
                    AccessMask = accessRights,
                    Principal = assigneeRef
                }
            };

            Execute<GrantAccessRequest, GrantAccessResponse>(AdminOrganizationService, request, bypassCustomPluginExecution);
        }

        public void UnShare(EntityReference objectRef, EntityReference revokeeRef, EntityReference callerRef = null, bool bypassCustomPluginExecution = false)
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

            var request = new RevokeAccessRequest
            {
                Target = objectRef,
                Revokee = revokeeRef
            };

            Execute<RevokeAccessRequest, RevokeAccessResponse>(AdminOrganizationService, request, bypassCustomPluginExecution);
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

            var optionsetSelectedText = string.Empty;

            var retrieveOptionSetRequest = new RetrieveOptionSetRequest { Name = optionsetName };

            //Execute the request
            var retrieveOptionSetResponse = (RetrieveOptionSetResponse)OrganizationService.Execute(retrieveOptionSetRequest);

            //Access the retrieved OptionSetMetaData
            var retrievedOptionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;

            //Get the current option lists for the retrieved attribute
            foreach (var optionMetadata in retrievedOptionSetMetadata.Options)
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

        public T Upsert<T>(T model, bool isAdmin = false, bool bypassCustomPluginExecution = false) where T : IBindingModel, new()
        {
            var service = isAdmin ? AdminOrganizationService : OrganizationService;

            return service.Upsert(model, new UpsertSettings { DisablePluginsExecution = bypassCustomPluginExecution });
        }


        public void AddUsersToTeam(EntityReference teamRef, params EntityReference[] userRefs)
        {
            AddUsersToTeam(teamRef, false, userRefs);
        }

        public void AddUsersToTeam(EntityReference teamRef, bool bypassCustomPluginExecution, params EntityReference[] userRefs)
        {
            if (userRefs.Length == 0)
            {
                return;
            }

            var request = new AddMembersTeamRequest
            {
                MemberIds = userRefs.Select(u => u.Id).ToArray(),
                TeamId = teamRef.Id
            };

            Execute<AddMembersTeamRequest, AddMembersTeamResponse>(AdminOrganizationService, request, bypassCustomPluginExecution);
        }

        public void RemoveUsersFromTeam(EntityReference teamRef, params EntityReference[] userRefs)
        {
            RemoveUsersFromTeam(teamRef, false, userRefs);
        }

        public void RemoveUsersFromTeam(EntityReference teamRef, bool bypassCustomPluginExecution, params EntityReference[] userRefs)
        {
            var request = new RemoveMembersTeamRequest
            {
                MemberIds = userRefs.Select(u => u.Id).ToArray(),
                TeamId = teamRef.Id
            };

            Execute<RemoveMembersTeamRequest, RemoveMembersTeamResponse>(AdminOrganizationService, request, bypassCustomPluginExecution);
        }

        public void AddToQueue(Guid queueId, EntityReference target, bool bypassCustomPluginExecution = false)
        {
            var request = new AddToQueueRequest
            {
                Target = target,
                DestinationQueueId = queueId
            };

            Execute<AddToQueueRequest, AddToQueueResponse>(AdminOrganizationService, request, bypassCustomPluginExecution);
        }

        public EntityReference GetDefaultCurrencyRef()
        {
            var query = new QueryExpression("transactioncurrency");
            query.Criteria.AddCondition("isocurrencycode", ConditionOperator.Equal, "EUR");

            return AdminOrganizationService.RetrieveAll(query).Select(e => e.ToEntityReference()).FirstOrDefault();
        }

        public void Merge(EntityReference target, Guid subordonate, Entity content, bool bypassCustomPluginExecution = false)
        {
            var request = new MergeRequest
            {
                Target = target,
                SubordinateId = subordonate,
                UpdateContent = content,
                PerformParentingChecks = true
            };

            Execute<MergeRequest, MergeResponse>(AdminOrganizationService, request, bypassCustomPluginExecution);
            
        }

        public bool UserHasOneRoleOf(Guid userId, params string[] roleIdTxts) =>
            UserHasOneRoleOf(userId, roleIdTxts?.Select(t => new Guid(t)).ToArray());

        public bool UserHasOneRoleOf(Guid userId, params Guid[] roleIds)
        {
            var query = new QueryExpression(SystemUserDefinition.EntityName);
            query.ColumnSet.AddColumn(SystemUserDefinition.Columns.Id);
            query.Criteria.AddCondition(SystemUserDefinition.Columns.Id, ConditionOperator.Equal, userId);

            var userRoleLink = query.AddLink(SystemUserRolesDefinition.EntityName, SystemUserDefinition.Columns.Id, SystemUserRolesDefinition.Columns.SystemUserId);
            var roleLink = userRoleLink.AddLink(RoleDefinition.EntityName, SystemUserRolesDefinition.Columns.RoleId, RoleDefinition.Columns.Id);
            roleLink.LinkCriteria.FilterOperator = LogicalOperator.Or;

            foreach (var roleId in roleIds)
            {
                roleLink.LinkCriteria.AddCondition(RoleDefinition.Columns.ParentRootRoleId, ConditionOperator.Equal, roleId);
                roleLink.LinkCriteria.AddCondition(RoleDefinition.Columns.RoleTemplateId, ConditionOperator.Equal, roleId);
            }

            return AdminOrganizationService.RetrieveMultiple(query).Entities.Any();
        }

        public ICollection<Guid> GetUserRoleIds(EntityReference userRef)
        {
            var query = new QueryExpression(RoleDefinition.EntityName);
            query.ColumnSet.AddColumns(RoleDefinition.Columns.RoleTemplateId, RoleDefinition.Columns.ParentRootRoleId);

            var userRoleLink = query.AddLink(SystemUserRolesDefinition.EntityName, RoleDefinition.Columns.Id, SystemUserRolesDefinition.Columns.RoleId);
            var userLink = userRoleLink.AddLink(SystemUserDefinition.EntityName, SystemUserRolesDefinition.Columns.SystemUserId, SystemUserDefinition.Columns.Id);

            userLink.LinkCriteria.AddCondition(SystemUserDefinition.Columns.Id, ConditionOperator.Equal, userRef.Id);

            return AdminOrganizationService.RetrieveMultiple(query).Entities
                .Select(r =>
                    r.Contains(RoleDefinition.Columns.RoleTemplateId) ?
                        r.GetAttributeValue<EntityReference>(RoleDefinition.Columns.RoleTemplateId).Id :
                        r.GetAttributeValue<EntityReference>(RoleDefinition.Columns.ParentRootRoleId).Id
                        ).ToList();
        }


        public void AddRoleToUserOrTeam(EntityReference userOrTeamRef, string parentRootRoleIdOrTemplateId, bool bypassCustomPluginExecution = false)
        {
            EntityReference businessUnitRef;

            if (userOrTeamRef.LogicalName == SystemUserDefinition.EntityName)
            {
                businessUnitRef = Retrieve(userOrTeamRef, SystemUserDefinition.Columns.BusinessUnitId).GetAttributeValue<EntityReference>(SystemUserDefinition.Columns.BusinessUnitId);
            }
            else
            {
                businessUnitRef = Retrieve(userOrTeamRef, TeamDefinition.Columns.BusinessUnitId).GetAttributeValue<EntityReference>(TeamDefinition.Columns.BusinessUnitId);
            }

            var roleRef = GetRoleRefForBusinessUnit(businessUnitRef, parentRootRoleIdOrTemplateId);


            if (userOrTeamRef.LogicalName == SystemUserDefinition.EntityName)
            {
                AssociateRecords(userOrTeamRef, new Microsoft.Xrm.Sdk.Relationship(RoleDefinition.ManyToManyRelationships.systemuserroles_association), bypassCustomPluginExecution, roleRef);

                AdminOrganizationService.Associate(SystemUserDefinition.EntityName, userOrTeamRef.Id, new Microsoft.Xrm.Sdk.Relationship(RoleDefinition.ManyToManyRelationships.systemuserroles_association),
                    new EntityReferenceCollection { roleRef });
            }
            else
            {
                AssociateRecords(userOrTeamRef, new Microsoft.Xrm.Sdk.Relationship(RoleDefinition.ManyToManyRelationships.teamroles_association), bypassCustomPluginExecution, roleRef);
            }
        }

        private EntityReference GetRoleRefForBusinessUnit(EntityReference businessUnitRef, string parentRootRoleIdOrTemplateId)
        {
            var queryRole = new QueryExpression(RoleDefinition.EntityName);
            var filterParent = queryRole.Criteria.AddFilter(LogicalOperator.Or);
            filterParent.AddCondition(RoleDefinition.Columns.RoleTemplateId, ConditionOperator.Equal, new Guid(parentRootRoleIdOrTemplateId));
            filterParent.AddCondition(RoleDefinition.Columns.ParentRootRoleId, ConditionOperator.Equal, new Guid(parentRootRoleIdOrTemplateId));

            queryRole.Criteria.AddCondition(RoleDefinition.Columns.BusinessUnitId, ConditionOperator.Equal, businessUnitRef.Id);

            return AdminOrganizationService.RetrieveMultiple(queryRole).Entities.Select(e => e.ToEntityReference()).FirstOrDefault();
        }

        public bool UserHasRole(Guid userId, Guid parentRoleId)
        {
            return UserHasOneRoleOf(userId, parentRoleId);
        }

        public Entity ToEntity<T>(T model) where T : IBindingModel
        {
            return model.ToEntity(OrganizationService);
        }

        public ICollection<EntityReference> GetTeamMemberRefs(EntityReference teamRef)
        {
            var queryMembers = new QueryExpression(SystemUserDefinition.EntityName);
            queryMembers.ColumnSet.AddColumn(SystemUserDefinition.Columns.Id);
            var linkMembers = queryMembers.AddLink(SystemUserDefinition.TeamMembershipRelationName, SystemUserDefinition.Columns.Id, SystemUserDefinition.Columns.Id);
            var linkTeam = linkMembers.AddLink(TeamDefinition.EntityName, TeamDefinition.Columns.Id, TeamDefinition.Columns.Id);

            linkTeam.LinkCriteria.AddCondition(TeamDefinition.Columns.Id, ConditionOperator.Equal, teamRef.Id);

            return AdminOrganizationService.RetrieveAll(queryMembers, false).Select(e => e.ToEntityReference()).ToList();
        }

        public void AssociateRecords(EntityReference objectRef, Microsoft.Xrm.Sdk.Relationship relationName, params EntityReference[] entityReferences)
        {
            AssociateRecords(objectRef, relationName, false, entityReferences);
        }

        public void AssociateRecords(EntityReference objectRef, Microsoft.Xrm.Sdk.Relationship relationName, bool bypassCustomPluginExecution, params EntityReference[] entityReferences)
        {
            var collec = new EntityReferenceCollection();
            collec.AddRange(entityReferences);

            var request = new AssociateRequest
            {
                Target = objectRef,
                Relationship = relationName,
                RelatedEntities = new EntityReferenceCollection(collec.ToList())
            };

            Execute<AssociateRequest, AssociateResponse>(AdminOrganizationService, request,
                bypassCustomPluginExecution);
        }

        public TVariable GetEnvironmentVariable<TVariable>(string schemaName)
        {
            var variableObject = GetEnvironmentVariable(typeof(TVariable), schemaName);

            if (variableObject == null)
            {
                return default(TVariable);
            }

            return (TVariable)variableObject;
        }

        protected object GetEnvironmentVariableValue(Type objectType, EnvironmentVariable variable)
        {
            if (variable == null)
            {
                return null;
            }

            switch (variable.Type)
            {
                case EnvironmentVariableType.String:
                    if (objectType != typeof(string))
                    {
                        throw new ArgumentException($"The environment variable is of type String GetEnvironmentVariable must be called with a string Type argument");
                    }

                    return variable.Value;
                case EnvironmentVariableType.Number:
                    if (objectType != typeof(int))
                    {
                        throw new ArgumentException($"The environment variable is of type Integer GetEnvironmentVariable must be called with a int Type argument");
                    }

                    return int.Parse(variable.Value);
                case EnvironmentVariableType.Boolean:
                    if (objectType != typeof(bool))
                    {
                        throw new ArgumentException($"The environment variable is of type Boolean GetEnvironmentVariable must be called with a bool Type argument");
                    }

                    return bool.Parse(variable.Value);
                case EnvironmentVariableType.Json:
                    return JsonConvert.DeserializeObject(variable.Value, objectType);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected ICollection<EnvironmentVariable> GetEnvironmentVariables(params string[] schemaNames)
        {
            if (schemaNames == null || schemaNames.Length == 0)
            {
                return new List<EnvironmentVariable>();
            }

            var queryVariable = BindingModelHelper.GetRetrieveAllQuery<EnvironmentVariable>();

            queryVariable.Criteria.FilterOperator = LogicalOperator.Or;

            foreach (var schemaName in schemaNames) {
                queryVariable.Criteria.AddCondition(EnvironmentVariableDefinition.Columns.SchemaName, ConditionOperator.Equal, schemaName);
            }

            var linkValue = queryVariable.AddLink(EnvironmentVariableValueDefinition.EntityName, EnvironmentVariableDefinition.Columns.Id, EnvironmentVariableValueDefinition.Columns.EnvironmentVariableDefinitionId, JoinOperator.LeftOuter);
            linkValue.EntityAlias = EnvironmentVariableValueDefinition.EntityName;
            linkValue.Columns.AddColumn(EnvironmentVariableValueDefinition.Columns.Value);

            return AdminOrganizationService.RetrieveAll<EnvironmentVariable>(queryVariable);
        }

        protected object GetEnvironmentVariable(Type objectType, string schemaName)
        {
            var variable = GetEnvironmentVariables(schemaName).FirstOrDefault();

            return GetEnvironmentVariableValue(objectType, variable);
        }

        private TResponse Execute<TRequest, TResponse>(IOrganizationService service, TRequest request,
            bool bypassCustomPluginExecution)
            where TRequest : OrganizationRequest
            where TResponse : OrganizationResponse
        {

            if (bypassCustomPluginExecution)
            {
                request["BypassCustomPluginExecution"] = true;
            }

            return (TResponse)service.Execute(request);
        }
    }
}
