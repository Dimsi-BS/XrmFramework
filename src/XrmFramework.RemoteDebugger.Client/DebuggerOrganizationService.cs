// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.RemoteDebugger.Common
{
    public class DebuggerOrganizationService : IOrganizationService
    {
        private readonly LocalServiceProvider.RequestHandler _onRequestSent;
        private RemoteDebugExecutionContext Context { get; }
        private Guid? UserId { get; }

        public DebuggerOrganizationService(RemoteDebugExecutionContext context, Guid? userId, LocalServiceProvider.RequestHandler onRequestSent)
        {
            _onRequestSent = onRequestSent;
            Context = context;
            UserId = userId;
        }

        public Guid Create(Entity entity)
        {
            return ((CreateResponse)Execute(new CreateRequest { Target = entity })).id;
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return ((RetrieveResponse)Execute(new RetrieveRequest { Target = new EntityReference(entityName, id), ColumnSet = columnSet})).Entity;
        }

        public void Update(Entity entity)
        {
            Execute(new UpdateRequest { Target = entity });
        }

        public void Delete(string entityName, Guid id)
        {
            Execute(new DeleteRequest { Target = new EntityReference(entityName, id) });
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            var message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Request, request, Context.Id)
            {
                UserId = UserId
            };

            var hybridResponse = _onRequestSent(message);

            var responseTemp = hybridResponse.GetOrganizationResponse();

            var assemblyQualifiedName = request.GetType().AssemblyQualifiedName;
            
            Debug.Assert(!string.IsNullOrWhiteSpace(assemblyQualifiedName));
            
            var typeName = assemblyQualifiedName.Replace("Request", "Response");

            var type = Type.GetType(typeName);
            
            Debug.Assert(type != null);

            var response = (OrganizationResponse)Activator.CreateInstance(type);

            response.Results = responseTemp.Results;

            return response;
        }

        public void Associate(string entityName, Guid entityId, Microsoft.Xrm.Sdk.Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            Execute(new AssociateRequest {  Target = new EntityReference(entityName, entityId), Relationship = relationship, RelatedEntities = relatedEntities});
        }

        public void Disassociate(string entityName, Guid entityId, Microsoft.Xrm.Sdk.Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            Execute(new DisassociateRequest { Target = new EntityReference(entityName, entityId), Relationship = relationship, RelatedEntities = relatedEntities });
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return ((RetrieveMultipleResponse)Execute(new RetrieveMultipleRequest { Query = query })).EntityCollection;
        }
    }
}