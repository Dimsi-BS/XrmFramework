using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
#if !PLUGIN && !ON_PREMISE
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.Dataverse.Client;
#endif

namespace XrmFramework.BindingModel;

public static class IOrganizationServiceExtensions_GetById
{
    public static T GetById<T>(this IOrganizationService service, Guid id) where T : IBindingModel
    {
        var type = typeof(T);
        var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
        return (T)GetById(type, service, new EntityReference(definition.EntityName, id));
    }

    public static T GetById<T>(this IOrganizationService service, Guid id, Entity recordImage) where T : IBindingModel
    {
        var type = typeof(T);
        var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
        return (T)GetById(type, service, new EntityReference(definition.EntityName, id), recordImage);
    }

    public static T GetById<T>(this IOrganizationService service, EntityReference reference, Entity recordImage = null)
        where T : IBindingModel
    {
        var type = typeof(T);
        return (T)GetById(type, service, reference, recordImage);
    }

    public static IBindingModel GetById(Type type, IOrganizationService service, Guid id)
    {
        var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
        return GetById(type, service, new EntityReference(definition.EntityName, id));
    }


    public static bool TryGetById<T>(this IOrganizationService service, Guid id, out T result) where T : IBindingModel
    {
        var type = typeof(T);
        var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
        IBindingModel resultTemp;

        var isOk = TryGetById(type, service, new EntityReference(definition.EntityName, id), out resultTemp);
        result = (T)resultTemp;
        return isOk;
    }

    public static bool TryGetById<T>(this IOrganizationService service, EntityReference reference, out T result)
        where T : IBindingModel
    {
        var type = typeof(T);

        IBindingModel resultTemp;

        var isOk = TryGetById(type, service, reference, out resultTemp);
        result = (T)resultTemp;
        return isOk;
    }

    public static bool TryGetById(Type type, IOrganizationService service, Guid id, out IBindingModel result)
    {
        var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
        return TryGetById(type, service, new EntityReference(definition.EntityName, id), out result);
    }

    public static bool TryGetById(Type type, IOrganizationService service, EntityReference reference,
        out IBindingModel result)
    {
        bool bSuccess = false;
        try
        {
            result = GetById(type, service, reference);
            bSuccess = true;
        }
        catch (Exception)
        {
            result = null;
        }

        return bSuccess;
    }


#if !PLUGIN && !ON_PREMISE
    public static async Task<T> GetByIdAsync<T>(this IOrganizationServiceAsync service, Guid id)
        where T : IBindingModel
    {
        var type = typeof(T);
        var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
        return (T)await GetByIdAsync(type, service, new EntityReference(definition.EntityName, id));
    }

    public static async Task<T> GetByIdAsync<T>(this IOrganizationServiceAsync service, Guid id, Entity recordImage)
        where T : IBindingModel
    {
        var type = typeof(T);
        var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
        return (T)await GetByIdAsync(type, service, new EntityReference(definition.EntityName, id), recordImage);
    }

    public static async Task<T> GetByIdAsync<T>(this IOrganizationServiceAsync service, EntityReference reference,
        Entity recordImage) where T : IBindingModel
    {
        var type = typeof(T);
        return (T)await GetByIdAsync(type, service, reference, recordImage);
    }

    public static async Task<T> GetByIdAsync<T>(this IOrganizationServiceAsync service, EntityReference reference)
        where T : IBindingModel
    {
        var type = typeof(T);
        return (T)await GetByIdAsync(type, service, reference);
    }

    public static async Task<T> GetByIdAsync<T>(this IOrganizationServiceAsync2 service, Guid id,
        CancellationToken cancellationToken) where T : IBindingModel
    {
        var type = typeof(T);
        var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
        return (T)await GetByIdAsync(type, service, new EntityReference(definition.EntityName, id),
            cancellationToken);
    }

    public static async Task<T> GetByIdAsync<T>(this IOrganizationServiceAsync2 service, Guid id,
        CancellationToken cancellationToken, Entity recordImage)
        where T : IBindingModel
    {
        var type = typeof(T);
        var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
        return (T)await GetByIdAsync(type, service, new EntityReference(definition.EntityName, id),
            cancellationToken, recordImage);
    }

    public static async Task<T> GetByIdAsync<T>(this IOrganizationServiceAsync2 service, EntityReference reference,
        CancellationToken cancellationToken,
        Entity recordImage = null) where T : IBindingModel
    {
        var type = typeof(T);
        return (T)await GetByIdAsync(type, service, reference, cancellationToken, recordImage);
    }

    private static async Task<IBindingModel> GetByIdAsync(Type type, IOrganizationServiceAsync service, Guid id)
    {
        var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
        return await GetByIdAsync(type, service, new EntityReference(definition.EntityName, id));
    }

    private static async Task<IBindingModel> GetByIdAsync(Type type, IOrganizationServiceAsync2 service, Guid id,
        CancellationToken cancellationToken)
    {
        var definition = DefinitionCache.GetEntityDefinitionFromModelType(type);
        return await GetByIdAsync(type, service, new EntityReference(definition.EntityName, id), cancellationToken);
    }

    private static async Task<IBindingModel> GetByIdAsync(Type type, IOrganizationServiceAsync service,
        EntityReference reference, Entity recordImage = null)
    {
        return await GetByIdInternalAsync(type,
            async (request, _) => (RetrieveResponse)await service.ExecuteAsync(request), reference,
            CancellationToken.None, recordImage);
    }

    private static async Task<IBindingModel> GetByIdAsync(Type type, IOrganizationServiceAsync2 service,
        EntityReference reference, CancellationToken cancellationToken, Entity recordImage = null)
    {
        return await GetByIdInternalAsync(type,
            async (request, token) => (RetrieveResponse)await service.ExecuteAsync(request, token), reference,
            cancellationToken, recordImage);
    }

    private static async Task<IBindingModel> GetByIdInternalAsync(Type type,
        Func<RetrieveRequest, CancellationToken, Task<RetrieveResponse>> retrieveFuncAsync, EntityReference reference,
        CancellationToken
            cancellationToken, Entity recordImage = null)
    {
        var request = GetRetrieveRequest(type, reference);

        var response = await retrieveFuncAsync(request, cancellationToken);

        if (recordImage != null)
        {
            response.Entity.MergeWith(recordImage);
        }

        return response.Entity.ToBindingModel(type);
    }


    public static async Task<(bool found, IBindingModel result)> TryGetByIdAsync(Type type,
        IOrganizationServiceAsync service, EntityReference reference, CancellationToken cancellationToken)
    {
        bool bSuccess = false;
        IBindingModel result;
        try
        {
            result = await GetByIdAsync(type, service, reference);
            bSuccess = true;
        }
        catch (Exception)
        {
            result = null;
        }

        return (bSuccess, result);
    }

    public static async Task<(bool found, IBindingModel result)> TryGetByIdAsync(Type type,
        IOrganizationServiceAsync2 service, EntityReference reference, CancellationToken cancellationToken)
    {
        bool bSuccess = false;
        IBindingModel result;
        try
        {
            result = await GetByIdAsync(type, service, reference, cancellationToken);
            bSuccess = true;
        }
        catch (Exception)
        {
            result = null;
        }

        return (bSuccess, result);
    }

#endif


    private static IBindingModel GetById(Type type, IOrganizationService service, EntityReference reference,
        Entity recordImage = null)
    {
        var request = GetRetrieveRequest(type, reference);

        var response = (RetrieveResponse)service.Execute(request);

        if (recordImage != null)
        {
            response.Entity.MergeWith(recordImage);
        }

        return response.Entity.ToBindingModel(type);
    }


    public static RetrieveRequest GetRetrieveRequest(Type type, EntityReference reference)
    {
        var entityDefinition = DefinitionCache.GetEntityDefinitionFromModelType(type);

        var request = new RetrieveRequest
        {
            Target = reference,
            ColumnSet = new ColumnSet(),
            RelatedEntitiesQuery = new RelationshipQueryCollection()
        };

        FillRetrieveRequest(type, request, entityDefinition);

        return request;
    }

    private static void FillRetrieveRequest(Type type, RetrieveRequest request, EntityDefinition entityDefinition)
    {
        var modelDefinition = DefinitionCache.GetModelDefinition(type);

        foreach (var property in modelDefinition.CrmAttributes)
        {
            var mappingAttribute = property.CrmMappingAttribute;

            if (!request.ColumnSet.Columns.Contains(mappingAttribute.AttributeName))
            {
                request.ColumnSet.AddColumn(mappingAttribute.AttributeName);
            }

            if (!entityDefinition.IsLookupAttribute(mappingAttribute.AttributeName) ||
                property.CrmLookupAttribute == null && (property.ObjectType == typeof(Guid) ||
                                                        property.ObjectType == typeof(EntityReference)))
            {
                continue;
            }

            var definitionLookupAttributes =
                entityDefinition.GetCrmLookupAttributes(mappingAttribute.AttributeName).ToList();
            var lookupAttribute = property.CrmLookupAttribute;
            var isBindingModel = typeof(IBindingModel).IsAssignableFrom(property.PropertyType);

            var subModelDefinition = isBindingModel ? DefinitionCache.GetModelDefinition(property.PropertyType) : null;

            var targetEntityName = isBindingModel
                ? subModelDefinition.MainDefinition.EntityName
                : lookupAttribute?.TargetEntityName;

            if (definitionLookupAttributes.Count == 1 && targetEntityName == null)
            {
                targetEntityName = definitionLookupAttributes.Single().TargetEntityName;
            }

            foreach (var definitionLookupAttribute in definitionLookupAttributes)
            {
                if (string.IsNullOrEmpty(targetEntityName))
                {
                    targetEntityName = definitionLookupAttribute.TargetEntityName;
                }

                if (definitionLookupAttribute.TargetEntityName != targetEntityName)
                {
                    continue;
                }

                QueryExpression query;

                if (request.RelatedEntitiesQuery.Keys.All(r =>
                        r.SchemaName != definitionLookupAttribute.RelationshipName))
                {
                    query = new QueryExpression(targetEntityName);
                    request.RelatedEntitiesQuery.Add(
                        new Microsoft.Xrm.Sdk.Relationship(definitionLookupAttribute.RelationshipName)
                            { PrimaryEntityRole = Microsoft.Xrm.Sdk.EntityRole.Referencing }, query);
                }
                else
                {
                    query = (QueryExpression)request.RelatedEntitiesQuery
                        .First(r => r.Key.SchemaName == definitionLookupAttribute.RelationshipName).Value;
                }

                if (isBindingModel)
                {
                    BindingModelHelper.AddQueryFilter(property.PropertyType, null, query.ColumnSet, query.LinkEntities,
                        2, string.Empty);
                }
                else if (lookupAttribute != null)
                {
                    query.ColumnSet.AddColumn(lookupAttribute.AttributeName);
                }
            }
        }


        foreach (var property in modelDefinition.ExtendBindingAttributes)
        {
            FillRetrieveRequest(property.PropertyType, request, entityDefinition);
        }

        foreach (var property in modelDefinition.RelationshipAttributes)
        {
            if (property.IsCollectionProperty(out var bindingType))
            {
                var query = BindingModelHelper.GetRetrieveAllQuery(bindingType);

                var relationship = new Microsoft.Xrm.Sdk.Relationship(property.Relationship.SchemaName)
                {
                    PrimaryEntityRole = property.Relationship.PrimaryEntityRole == EntityRole.Referenced
                        ? Microsoft.Xrm.Sdk.EntityRole.Referenced
                        : Microsoft.Xrm.Sdk.EntityRole.Referencing
                };

                request.RelatedEntitiesQuery.Add(relationship, query);
            }
        }
    }
}
