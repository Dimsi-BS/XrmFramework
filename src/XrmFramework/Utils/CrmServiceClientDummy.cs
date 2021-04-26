using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

// ReSharper disable once CheckNamespace
namespace Microsoft.Xrm.Tooling.Connector {
    public class CrmServiceClient : IOrganizationService
    {
        public CrmServiceClient(string connectionString) {
            
        }
        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public Guid Create(Entity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(string entityName, Guid id)
        {
            throw new NotImplementedException();
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            throw new NotImplementedException();
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            throw new NotImplementedException();
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            throw new NotImplementedException();
        }

        public void Update(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}