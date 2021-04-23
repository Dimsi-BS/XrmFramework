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
        Guid Create(Entity entity, bool useAdmin = false);

        UpsertResponse Upsert(Entity entity, bool useAdmin = false);

        void Update(Entity entity, bool useAdmin = false);

        void Delete(string logicalName, Guid id, bool useAdmin = false);

        Guid Create(Entity entity, Guid callerId);

        UpsertResponse Upsert(Entity entity, Guid callerId);

        void Update(Entity entity, Guid callerId);

        void Delete(string logicalName, Guid id, Guid callerId);

        void Delete(Microsoft.Xrm.Sdk.EntityReference objectReference, bool useAdmin = false);

        void Delete(Microsoft.Xrm.Sdk.EntityReference objectReference, Guid callerId);

        void AssignEntity(Microsoft.Xrm.Sdk.EntityReference objectReference, [Nullable] Microsoft.Xrm.Sdk.EntityReference ownerRef);

        void AddUsersToTeam(Microsoft.Xrm.Sdk.EntityReference teamRef, params Microsoft.Xrm.Sdk.EntityReference[] userRefs);

        void RemoveUsersFromTeam(Microsoft.Xrm.Sdk.EntityReference teamRef, params Microsoft.Xrm.Sdk.EntityReference[] userRefs);

        void AddToQueue(Guid queueId, Microsoft.Xrm.Sdk.EntityReference target);

        void Merge(Microsoft.Xrm.Sdk.EntityReference target, Guid subordonate, Entity content);

        [Obsolete("Use Update instead.")]
        void SetState(Microsoft.Xrm.Sdk.EntityReference objectRef, int stateCode, int statusCode, bool useAdmin = false);

        void Share(Microsoft.Xrm.Sdk.EntityReference objectRef, Microsoft.Xrm.Sdk.EntityReference assignee, AccessRights accessRights);

        void UnShare(Microsoft.Xrm.Sdk.EntityReference objectRef, Microsoft.Xrm.Sdk.EntityReference revokee, [Nullable] Microsoft.Xrm.Sdk.EntityReference callerRef = null);

        Entity Retrieve(string entityName, Guid id, params string[] columns);

        Entity Retrieve(string entityName, Guid id, bool allColumns);

        Entity Retrieve(Microsoft.Xrm.Sdk.EntityReference objectRef, params string[] columns);

        Entity Retrieve(Microsoft.Xrm.Sdk.EntityReference objectRef, bool allColumns);

        string GetOptionSetNameFromValue(string optionsetName, int optionsetValue);

        string GetOptionSetNameFromValue<T>(int optionsetValue);

        T GetById<T>(Guid id) where T : IBindingModel, new();

        T GetById<T>(Microsoft.Xrm.Sdk.EntityReference entityReference) where T : IBindingModel, new();

        T Upsert<T>(T model, bool isAdmin = false) where T : IBindingModel, new();

        bool UserHasRole(Guid userId, Guid parentRoleId);

        bool UserHasOneRoleOf(Guid userId, params Guid[] parentRoleIds);

        bool UserHasOneRoleOf(Guid userId, params string[] parentRoleIds);

        ICollection<Guid> GetUserRoleIds(Microsoft.Xrm.Sdk.EntityReference userRef);

        Entity ToEntity<T>(T model) where T : IBindingModel;

        ICollection<Microsoft.Xrm.Sdk.EntityReference> GetTeamMemberRefs(Microsoft.Xrm.Sdk.EntityReference teamRef);

        void AssociateRecords(Microsoft.Xrm.Sdk.EntityReference objectRef, Microsoft.Xrm.Sdk.Relationship relationName, params Microsoft.Xrm.Sdk.EntityReference[] entityReferences);

        TVariable GetEnvironmentVariable<TVariable>(string schemaName);

        void AddRoleToUserOrTeam(Microsoft.Xrm.Sdk.EntityReference userOrTeamRef, string parentRootRoleIdOrTemplateId);
    }
}
