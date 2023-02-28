// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using XrmFramework.BindingModel;

namespace XrmFramework
{
    public partial interface IService
    {
        Guid Create(Entity entity, bool useAdmin = false, bool bypassCustomPluginExecution = false);

        UpsertResponse Upsert(Entity entity, bool useAdmin = false, bool bypassCustomPluginExecution = false);

        void Update(Entity entity, bool useAdmin = false, bool bypassCustomPluginExecution = false);

        void Delete(string logicalName, Guid id, bool useAdmin = false, bool bypassCustomPluginExecution = false);

        Guid Create(Entity entity, Guid callerId, bool bypassCustomPluginExecution = false);

        UpsertResponse Upsert(Entity entity, Guid callerId, bool bypassCustomPluginExecution = false);

        void Update(Entity entity, Guid callerId, bool bypassCustomPluginExecution = false);

        void Delete(string logicalName, Guid id, Guid callerId, bool bypassCustomPluginExecution = false);

        void Delete(EntityReference objectReference, bool useAdmin = false, bool bypassCustomPluginExecution = false);

        void Delete(EntityReference objectReference, Guid callerId, bool bypassCustomPluginExecution = false);

        void AssignEntity(EntityReference objectReference, [Nullable] EntityReference ownerRef, bool bypassCustomPluginExecution = false);

        void AddUsersToTeam(EntityReference teamRef, params EntityReference[] userRefs);

        void AddUsersToTeam(EntityReference teamRef, bool bypassCustomPluginExecution, params EntityReference[] userRefs);

        void RemoveUsersFromTeam(EntityReference teamRef, params EntityReference[] userRefs);

        void RemoveUsersFromTeam(EntityReference teamRef, bool bypassCustomPluginExecution, params EntityReference[] userRefs);

        void AddToQueue(Guid queueId, EntityReference target, bool bypassCustomPluginExecution = false);

        void Merge(EntityReference target, Guid subordonate, Entity content, bool bypassCustomPluginExecution = false);

        [Obsolete("Use Update instead.")]
        void SetState(EntityReference objectRef, int stateCode, int statusCode, bool useAdmin = false, bool bypassCustomPluginExecution = false);

        void Share(EntityReference objectRef, EntityReference assignee, AccessRights accessRights, bool bypassCustomPluginExecution = false);

        void UnShare(EntityReference objectRef, EntityReference revokee, [Nullable] EntityReference callerRef = null, bool bypassCustomPluginExecution = false);

        Entity Retrieve(string entityName, Guid id, params string[] columns);

        Entity Retrieve(string entityName, Guid id, bool allColumns);

        Entity Retrieve(EntityReference objectRef, params string[] columns);

        Entity Retrieve(EntityReference objectRef, bool allColumns);

        string GetOptionSetNameFromValue(string optionsetName, int optionsetValue);

        string GetOptionSetNameFromValue<T>(int optionsetValue);

        T GetById<T>(Guid id) where T : IBindingModel, new();

        T GetById<T>(EntityReference entityReference) where T : IBindingModel, new();

        T Upsert<T>(T model, bool isAdmin = false, bool bypassCustomPluginExecution = false) where T : IBindingModel, new();

        bool UserHasRole(Guid userId, Guid parentRoleId);

        bool UserHasOneRoleOf(Guid userId, params Guid[] parentRoleIds);

        bool UserHasOneRoleOf(Guid userId, params string[] parentRoleIds);

        ICollection<Guid> GetUserRoleIds(EntityReference userRef);

        Entity ToEntity<T>(T model) where T : IBindingModel;

        ICollection<EntityReference> GetTeamMemberRefs(EntityReference teamRef);

        void AssociateRecords(EntityReference objectRef, Microsoft.Xrm.Sdk.Relationship relationName, params EntityReference[] entityReferences);

        void AssociateRecords(EntityReference objectRef, Microsoft.Xrm.Sdk.Relationship relationName, bool bypassCustomPluginExecution, params EntityReference[] entityReferences);
        
        void AddRoleToUserOrTeam(EntityReference userOrTeamRef, string parentRootRoleIdOrTemplateId, bool bypassCustomPluginExecution = false);
    }
}
