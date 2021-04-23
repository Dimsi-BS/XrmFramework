using Microsoft.Xrm.Sdk;

namespace XrmFramework
{
    public static class EntityReferenceExtensions
    {
        public static Entity ToEntity(this EntityReference objectRef)
        {
            if (objectRef == null)
            {
                return null;
            }

            var entity = new Entity(objectRef.LogicalName)
            {
                Id = objectRef.Id
            };

            if (objectRef.KeyAttributes != null)
            {
                foreach (var key in objectRef.KeyAttributes.Keys)
                {
                    entity.KeyAttributes[key] = objectRef.KeyAttributes[key];
                }
            }

            return entity;
        }
    }
}
