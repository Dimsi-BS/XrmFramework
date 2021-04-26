using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.DeployUtils
{
    public class DisposableOrganizationService : IDisposable, IOrganizationService
    {
        private readonly IOrganizationService _service;

        public DisposableOrganizationService(IOrganizationService service)
        {
            _service = service;
        }

        public Guid Create(Entity entity)
        {
            return _service.Create(entity);
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return _service.Retrieve(entityName, id, columnSet);
        }

        public void Update(Entity entity)
        {
            _service.Update(entity);
        }

        public void Delete(string entityName, Guid id)
        {
            _service.Delete(entityName, id);
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return _service.Execute(request);
        }

        public void Associate(string entityName, Guid entityId, Microsoft.Xrm.Sdk.Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _service.Associate(entityName, entityId, relationship, relatedEntities);
        }

        public void Disassociate(string entityName, Guid entityId, Microsoft.Xrm.Sdk.Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _service.Disassociate(entityName, entityId, relationship, relatedEntities);
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return _service.RetrieveMultiple(query);
        }

        public void Dispose()
        {
            if (_service is IDisposable disposable)
            {
                disposable?.Dispose();
            }
        }
    }
}
