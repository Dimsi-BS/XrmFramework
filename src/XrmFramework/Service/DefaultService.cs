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

#if INTERNAL_NEWTONSOFT
using Newtonsoft.Json.Xrm;
#else
using Newtonsoft.Json;
#endif

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

        protected Microsoft.Xrm.Sdk.EntityReference BusinessUnitRef => _context.BusinessUnitRef;

        protected Guid UserId => _context.UserId;

        protected string OrganizationName => _context.OrganizationName;

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

        public void Delete(Microsoft.Xrm.Sdk.EntityReference objectReference, Guid callerId)
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

            Delete(new Microsoft.Xrm.Sdk.EntityReference(logicalName, id), callerId);
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

            Delete(new Microsoft.Xrm.Sdk.EntityReference(logicalName, id), useAdmin);
        }

        public void Delete(Microsoft.Xrm.Sdk.EntityReference objectReference, bool useAdmin = false)
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

        public void AssignEntity(Microsoft.Xrm.Sdk.EntityReference objectReference, Microsoft.Xrm.Sdk.EntityReference ownerRef)
        {
#region Parameters check
            if (objectReference == null)
            {
                throw new ArgumentNullException(nameof(objectReference));
            }
#endregion

            AdminOrganizationService.Execute(new AssignRequest
            {
                Assignee = ownerRef ?? new Microsoft.Xrm.Sdk.EntityReference("systemuser", InitiatingUserId),
                Target = objectReference
            });
        }

        public void SetState(Microsoft.Xrm.Sdk.EntityReference objectRef, int stateCode, int statusCode, bool useAdmin = false)
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

        public void Share(Microsoft.Xrm.Sdk.EntityReference objectRef, Microsoft.Xrm.Sdk.EntityReference assigneeRef, AccessRights accessRights)
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

        public void UnShare(Microsoft.Xrm.Sdk.EntityReference objectRef, Microsoft.Xrm.Sdk.EntityReference revokeeRef, Microsoft.Xrm.Sdk.EntityReference callerRef = null)
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

        private Entity RetrieveInternal(Microsoft.Xrm.Sdk.EntityReference objectRef, ColumnSet cs)
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

            return RetrieveInternal(new Microsoft.Xrm.Sdk.EntityReference(entityName, id), new ColumnSet(columns));
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

            return RetrieveInternal(new Microsoft.Xrm.Sdk.EntityReference(entityName, id), new ColumnSet(allColumns));
        }

        public Entity Retrieve(Microsoft.Xrm.Sdk.EntityReference objectRef, params string[] columns)
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


        public Entity Retrieve(Microsoft.Xrm.Sdk.EntityReference objectRef, bool allColumns)
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

        public T GetById<T>(Microsoft.Xrm.Sdk.EntityReference entityReference) where T : IBindingModel, new()
        {
            return AdminOrganizationService.GetById<T>(entityReference);
        }

        public T Upsert<T>(T model, bool isAdmin = false) where T : IBindingModel, new()
        {
            var service = isAdmin ? AdminOrganizationService : OrganizationService; 

            return service.Upsert(model);
        }


        public void AddUsersToTeam(Microsoft.Xrm.Sdk.EntityReference teamRef, params Microsoft.Xrm.Sdk.EntityReference[] userRefs)
        {
            if(userRefs.Length == 0)
            {
                return;
            }

            var request = new AddMembersTeamRequest
            {
                MemberIds = userRefs.Select(u => u.Id).ToArray(),
                TeamId = teamRef.Id
            };

            AdminOrganizationService.Execute(request);
        }

        public void RemoveUsersFromTeam(Microsoft.Xrm.Sdk.EntityReference teamRef, params Microsoft.Xrm.Sdk.EntityReference[] userRefs)
        {
            var request = new RemoveMembersTeamRequest
            {
                MemberIds = userRefs.Select(u => u.Id).ToArray(),
                TeamId = teamRef.Id
            };

            AdminOrganizationService.Execute(request);
        }

        public void AddToQueue(Guid queueId, Microsoft.Xrm.Sdk.EntityReference target)
        {
            var request = new AddToQueueRequest
            {
                Target = target,
                DestinationQueueId = queueId
            };

            AdminOrganizationService.Execute(request);
        }

        public Microsoft.Xrm.Sdk.EntityReference GetDefaultCurrencyRef()
        {
            var query = new QueryExpression("transactioncurrency");
            query.Criteria.AddCondition("isocurrencycode", ConditionOperator.Equal, "EUR");

            return AdminOrganizationService.RetrieveAll(query).Select(e => e.ToEntityReference()).FirstOrDefault();
        }

        public void Merge(Microsoft.Xrm.Sdk.EntityReference target, Guid subordonate, Entity content)
        {
            var request = new MergeRequest
            {
                Target = target,
                SubordinateId = subordonate,
                UpdateContent = content,
                PerformParentingChecks = true
            };

            AdminOrganizationService.Execute(request);
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

        public ICollection<Guid> GetUserRoleIds(Microsoft.Xrm.Sdk.EntityReference userRef)
        {
            var query = new QueryExpression(RoleDefinition.EntityName);
            query.ColumnSet.AddColumns(RoleDefinition.Columns.RoleTemplateId, RoleDefinition.Columns.ParentRootRoleId);

            var userRoleLink = query.AddLink(SystemUserRolesDefinition.EntityName, RoleDefinition.Columns.Id, SystemUserRolesDefinition.Columns.RoleId);
            var userLink = userRoleLink.AddLink(SystemUserDefinition.EntityName, SystemUserRolesDefinition.Columns.SystemUserId, SystemUserDefinition.Columns.Id);

            userLink.LinkCriteria.AddCondition(SystemUserDefinition.Columns.Id, ConditionOperator.Equal, userRef.Id);

            return AdminOrganizationService.RetrieveMultiple(query).Entities
                .Select(r =>
                    r.Contains(RoleDefinition.Columns.RoleTemplateId) ?
                        r.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>(RoleDefinition.Columns.RoleTemplateId).Id :
                        r.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>(RoleDefinition.Columns.ParentRootRoleId).Id
                        ).ToList();
        }


        public void AddRoleToUserOrTeam(Microsoft.Xrm.Sdk.EntityReference userOrTeamRef, string parentRootRoleIdOrTemplateId)
        {
            Microsoft.Xrm.Sdk.EntityReference businessUnitRef;

            if (userOrTeamRef.LogicalName == SystemUserDefinition.EntityName)
            {
                businessUnitRef = Retrieve(userOrTeamRef, SystemUserDefinition.Columns.BusinessUnitId).GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>(SystemUserDefinition.Columns.BusinessUnitId);
            }
            else
            {
                businessUnitRef = Retrieve(userOrTeamRef, TeamDefinition.Columns.BusinessUnitId).GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>(TeamDefinition.Columns.BusinessUnitId);
            }

            var roleRef = GetRoleRefForBusinessUnit(businessUnitRef, parentRootRoleIdOrTemplateId);


            if (userOrTeamRef.LogicalName == SystemUserDefinition.EntityName)
            {
                AdminOrganizationService.Associate(SystemUserDefinition.EntityName, userOrTeamRef.Id, new Microsoft.Xrm.Sdk.Relationship(RoleDefinition.ManyToManyRelationships.systemuserroles_association),
                    new EntityReferenceCollection { roleRef });
            }
            else
            {
                AdminOrganizationService.Associate(TeamDefinition.EntityName, userOrTeamRef.Id, new Microsoft.Xrm.Sdk.Relationship(RoleDefinition.ManyToManyRelationships.teamroles_association),
                    new EntityReferenceCollection { roleRef });
            }
        }

        private Microsoft.Xrm.Sdk.EntityReference GetRoleRefForBusinessUnit(Microsoft.Xrm.Sdk.EntityReference businessUnitRef, string parentRootRoleIdOrTemplateId)
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

        public ICollection<Microsoft.Xrm.Sdk.EntityReference> GetTeamMemberRefs(Microsoft.Xrm.Sdk.EntityReference teamRef)
        {
            var queryMembers = new QueryExpression(SystemUserDefinition.EntityName);
            queryMembers.ColumnSet.AddColumn(SystemUserDefinition.Columns.Id);
            var linkMembers = queryMembers.AddLink(SystemUserDefinition.TeamMembershipRelationName, SystemUserDefinition.Columns.Id, SystemUserDefinition.Columns.Id);
            var linkTeam = linkMembers.AddLink(TeamDefinition.EntityName, TeamDefinition.Columns.Id, TeamDefinition.Columns.Id);

            linkTeam.LinkCriteria.AddCondition(TeamDefinition.Columns.Id, ConditionOperator.Equal, teamRef.Id);

            return AdminOrganizationService.RetrieveAll(queryMembers, false).Select(e => e.ToEntityReference()).ToList();
        }

        public void AssociateRecords(Microsoft.Xrm.Sdk.EntityReference objectRef, Microsoft.Xrm.Sdk.Relationship relationName, params Microsoft.Xrm.Sdk.EntityReference[] entityReferences)
        {
            var collec = new EntityReferenceCollection();
            collec.AddRange(entityReferences);

            AdminOrganizationService.Associate(objectRef.LogicalName, objectRef.Id, relationName, collec);
        }

        public TVariable GetEnvironmentVariable<TVariable>(string schemaName)
        {
            var variableObject = GetEnvironmentVariable(typeof(TVariable), schemaName);

            if (variableObject == null)
            {
                return default(TVariable);
            }

            return (TVariable) variableObject;
        }

        protected object GetEnvironmentVariable(Type objectType, string schemaName)
        {
            var queryVariable = BindingModelHelper.GetRetrieveAllQuery<EnvironmentVariable>();
            queryVariable.Criteria.AddCondition(EnvironmentVariableDefinition.Columns.SchemaName, ConditionOperator.Equal, schemaName);

            var linkValue = queryVariable.AddLink(EnvironmentVariableValueDefinition.EntityName, EnvironmentVariableDefinition.Columns.Id, EnvironmentVariableValueDefinition.Columns.EnvironmentVariableDefinitionId, JoinOperator.LeftOuter);
            linkValue.EntityAlias = EnvironmentVariableValueDefinition.EntityName;
            linkValue.Columns.AddColumn(EnvironmentVariableValueDefinition.Columns.Value);

            var variable = AdminOrganizationService.RetrieveAll<EnvironmentVariable>(queryVariable).FirstOrDefault();

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
    }
}
