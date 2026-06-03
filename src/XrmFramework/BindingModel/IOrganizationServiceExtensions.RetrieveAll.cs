using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
#if !PLUGIN && !ON_PREMISE
using System.Threading;
using Microsoft.PowerPlatform.Dataverse.Client;
#endif

namespace XrmFramework.BindingModel;

public static class IOrganizationServiceExtensions_RetrieveAll
{
    public static IList<T> RetrieveAll<T>(this IOrganizationService service) where T : IBindingModel
    {
        var query = BindingModelHelper.GetRetrieveAllQuery<T>();
        return RetrieveAll(service, query).ToBindingModel<T>().ToList();
    }

    public static IList<T> RetrieveAll<T>(this IOrganizationService service, QueryExpression query,
        bool cleanLinks = false) where T : IBindingModel
    {
        return service.RetrieveAll(query, cleanLinks).ToBindingModel<T>().ToList();
    }

    public static IList<Entity> RetrieveAll(this IOrganizationService service, QueryExpression query,
        bool cleanLinks = false)
        => RetrieveAllInternal(query, cleanLinks, q => Task.FromResult(service.RetrieveMultiple(q))).GetAwaiter()
            .GetResult();

#if !PLUGIN && !ON_PREMISE
    public static async Task<IList<T>> RetrieveAllAsync<T>(this IOrganizationServiceAsync service)
        where T : IBindingModel
    {
        var query = BindingModelHelper.GetRetrieveAllQuery<T>();
        return (await service.RetrieveAllAsync(query)).ToBindingModel<T>().ToList();
    }

    public static async Task<IList<T>> RetrieveAllAsync<T>(this IOrganizationServiceAsync service,
        QueryExpression query, bool cleanLinks = false)
        where T : IBindingModel
    {
        return (await service.RetrieveAllAsync(query, cleanLinks)).ToBindingModel<T>().ToList();
    }

    public static async Task<IList<Entity>> RetrieveAllAsync(this IOrganizationServiceAsync service,
        QueryExpression query, bool cleanLinks = false)
        => await RetrieveAllInternal(query, cleanLinks, async q => await service.RetrieveMultipleAsync(q));

    public static async Task<IList<T>> RetrieveAllAsync<T>(this IOrganizationServiceAsync2 service,
        CancellationToken cancellationToken) where T : IBindingModel
    {
        var query = BindingModelHelper.GetRetrieveAllQuery<T>();
        return (await service.RetrieveAllAsync(query, cancellationToken)).ToBindingModel<T>().ToList();
    }

    public static async Task<IList<T>> RetrieveAllAsync<T>(this IOrganizationServiceAsync2 service,
        QueryExpression query, CancellationToken cancellationToken,
        bool cleanLinks = false) where T : IBindingModel
    {
        return (await service.RetrieveAllAsync(query, cancellationToken, cleanLinks)).ToBindingModel<T>().ToList();
    }

    public static async Task<IList<Entity>> RetrieveAllAsync(this IOrganizationServiceAsync2 service,
        QueryExpression query, CancellationToken cancellationToken,
        bool cleanLinks = false)
        => await RetrieveAllInternal(query, cleanLinks,
            async q => await service.RetrieveMultipleAsync(q, cancellationToken));
#endif

    private static async Task<IList<Entity>> RetrieveAllInternal(QueryExpression query, bool cleanLinks,
        Func<QueryExpression, Task<EntityCollection>> retrieveMultiple)
    {
        if (!query.TopCount.HasValue)
        {
            query.PageInfo = new PagingInfo { Count = 5000, PageNumber = 1 };
        }

        var result = new List<Entity>();

        EntityCollection ec;

        if (cleanLinks)
        {
            query.CleanLinks();
        }

        do
        {
            ec = await retrieveMultiple(query);
            Debug.WriteLine(
                $"Récupération de la page {query.PageInfo?.PageNumber} de {query.PageInfo?.Count} enregistrements.");

            result.AddRange(ec.Entities);

            if (query.PageInfo != null)
            {
                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = ec.PagingCookie;
            }
        } while (ec.MoreRecords);

        return result;
    }
}
