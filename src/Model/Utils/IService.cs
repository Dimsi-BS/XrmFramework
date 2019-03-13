// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugins
{
    public interface IService
    {
        Guid Create(Entity entity, bool useAdmin = false);

        UpsertResponse Upsert(Entity entity, bool useAdmin = false);

        void Update(Entity entity, bool useAdmin = false);

        void Delete(string logicalName, Guid id, bool useAdmin = false);

        Guid Create(Entity entity, Guid callerId);

        UpsertResponse Upsert(Entity entity, Guid callerId);

        void Update(Entity entity, Guid callerId);

        void Delete(string logicalName, Guid id, Guid callerId);

        void Delete(EntityReference objectReference, bool useAdmin = false);

        void Delete(EntityReference objectReference, Guid callerId);

        void AssignEntity(EntityReference objectReference, [Nullable] EntityReference ownerRef);

        void AddUsersToTeam(EntityReference teamRef, params EntityReference[] userRefs);

        void RemoveUsersFromTeam(EntityReference teamRef, params EntityReference[] userRefs);

        [Obsolete("Use Update instead.")]
        void SetState(EntityReference objectRef, int stateCode, int statusCode, bool useAdmin = false);

        void Share(EntityReference objectRef, EntityReference assignee, AccessRights accessRights);

        void UnShare(EntityReference objectRef, EntityReference revokee, [Nullable] EntityReference callerRef = null);

        Entity Retrieve(string entityName, Guid id, params string[] columns);

        Entity Retrieve(string entityName, Guid id, bool allColumns);

        Entity Retrieve(EntityReference objectRef, params string[] columns);

        Entity Retrieve(EntityReference objectRef, bool allColumns);

        string GetOptionSetNameFromValue(string optionsetName, int optionsetValue);

        string GetOptionSetNameFromValue<T>(int optionsetValue);
    }
}
