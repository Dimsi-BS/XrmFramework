using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Visitors;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace XrmFramework.Azure.Functions.Worker.Extensions.OpenApi;

public class CustomDocumentHelper(
    RouteConstraintFilter filter,
    IOpenApiSchemaAcceptor acceptor,
    IServiceProvider serviceProvider)
    : IDocumentHelper
{
    private readonly IDocumentHelper _internalDocumentHelper = new DocumentHelper(filter, acceptor);

    public string FilterRouteConstraints(string route)
    {
        return _internalDocumentHelper.FilterRouteConstraints(route);
    }

    public OpenApiPathItem GetOpenApiPath(string path, OpenApiPaths paths)
    {
        return _internalDocumentHelper.GetOpenApiPath(path, paths);
    }

    public OpenApiRequestBody? GetOpenApiRequestBody(MethodInfo element, NamingStrategy namingStrategy, VisitorCollection collection, OpenApiVersionType version = OpenApiVersionType.V2)
    {
        IEnumerable<OpenApiRequestBodyAttribute> customAttributes = element.GetCustomAttributes<OpenApiRequestBodyAttribute>(false);

        var isGetMethod = element.GetParameters().SelectMany(param => param.GetCustomAttribute<HttpTriggerAttribute>()?.Methods ?? []).Any(m => string.Compare(m, "get", StringComparison.OrdinalIgnoreCase) == 0);

        var bodyType = element.GetParameters().FirstOrDefault(p => typeof(IBaseRequest).IsAssignableFrom(p.ParameterType))?.ParameterType;
        var openApiRequestBodyAttributes = customAttributes.ToList();
        if (isGetMethod || (!openApiRequestBodyAttributes.Any() && bodyType == null))
        {
            return null;
        }

        if (!openApiRequestBodyAttributes.Any())
        {
            openApiRequestBodyAttributes = new List<OpenApiRequestBodyAttribute>
            {
                new("application/json", bodyType)
            };
        }

        Dictionary<string, OpenApiMediaType> dictionary = openApiRequestBodyAttributes.Where(p => !p.Deprecated).ToDictionary(p => p.ContentType, p => p.ToOpenApiMediaType(namingStrategy, collection, version));
        if (dictionary.Any())
        {
            return new OpenApiRequestBody
            {
                Content = dictionary,
                Required = openApiRequestBodyAttributes[0].Required,
                Description = openApiRequestBodyAttributes[0].Description
            };
        }

        return null;
    }

    [Obsolete("This method is obsolete and will be removed in the next major version. Please use GetOpenApiRequestBody(MethodInfo, NamingStrategy, VisitorCollection, OpenApiVersionType) instead.")]
    public OpenApiResponses GetOpenApiResponseBody(MethodInfo element, NamingStrategy namingStrategy = null)
    {
        return _internalDocumentHelper.GetOpenApiResponseBody(element, namingStrategy);
    }

    public OpenApiResponses GetOpenApiResponses(MethodInfo element, NamingStrategy namingStrategy, VisitorCollection collection, OpenApiVersionType version = OpenApiVersionType.V2)
    {
        var first = from p in element.GetCustomAttributes<OpenApiResponseWithBodyAttribute>(inherit: false)
                    where !p.Deprecated
                    select new
                    {
                        p.StatusCode,
                        Response = p.ToOpenApiResponse(namingStrategy, null, version)
                    };
        var second = from p in element.GetCustomAttributes<OpenApiResponseWithoutBodyAttribute>(inherit: false)
                     select new
                     {
                         p.StatusCode,
                         Response = p.ToOpenApiResponse(namingStrategy)
                     };

        if (!first.Any() && !second.Any())
        {
            var third = from p in element
                    .GetParameters()
                    .Where(param => typeof(IBaseRequest).IsAssignableFrom(param.ParameterType))
                    .SelectMany(param => param.ParameterType.GetInterfaces())
                    .Where(i => i.IsAssignableTo(typeof(IBaseRequest)) && i != typeof(IBaseRequest))
                    .Select(i =>
                    {
                        OpenApiResponse response;
                        if (i != typeof(IRequest))
                        {
                            response = new OpenApiResponseWithBodyAttribute(System.Net.HttpStatusCode.OK, "application/json", i.GenericTypeArguments.Single()).ToOpenApiResponse(namingStrategy, collection, version);
                        }
                        else
                        {
                            response = new OpenApiResponseWithoutBodyAttribute(System.Net.HttpStatusCode.OK).ToOpenApiResponse(namingStrategy);
                        }
                        return response;
                    })

                        select new
                        {
                            StatusCode = System.Net.HttpStatusCode.OK,
                            Response = p
                        };

            second = second.Concat(third);
        }

        var fullCollection = first.Concat(second);

        if (fullCollection.All(c => c.StatusCode != System.Net.HttpStatusCode.BadRequest))
        {
            var temp = from p in element
                    .GetParameters()
                    .Where(param => typeof(IBaseRequest).IsAssignableFrom(param.ParameterType))
                    .Where(param => param.ParameterType.GetInterfaces().Any(i => i.IsAssignableTo(typeof(IBaseRequest)) && i != typeof(IBaseRequest)))
                    .Select(param =>
                    {
                        var parameterType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.FullName == param.ParameterType.FullName);

                        var validator = serviceProvider.GetService(typeof(IValidator<>).MakeGenericType(parameterType));

                        if (validator != null)
                        {
                            var attribute = new OpenApiResponseWithBodyAttribute(System.Net.HttpStatusCode.BadRequest, "application/json", typeof(ValidationErrors));
                            attribute.Description = "Bad request";

                            return attribute.ToOpenApiResponse(namingStrategy, collection, version);
                        }
                        return null;
                    })
                    .Where(a => a != null)

            select new
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Response = p
            };

            fullCollection = fullCollection.Concat(temp);
        }

        return fullCollection.ToDictionary(p => ((int)p.StatusCode).ToString(), p => p.Response).ToOpenApiResponses();
    }

    public Dictionary<string, OpenApiSchema> GetOpenApiSchemas(List<MethodInfo> elements, NamingStrategy namingStrategy, VisitorCollection collection)
    {
        IEnumerable<Type> first = from p in elements.SelectMany(p => p.GetCustomAttributes<OpenApiRequestBodyAttribute>(inherit: false))
                                  select p.BodyType;
        IEnumerable<Type> second = from p in elements.SelectMany(p => p.GetCustomAttributes<OpenApiResponseWithBodyAttribute>(inherit: false))
                                   select p.BodyType;


        IEnumerable<Type> third = from p in elements
                                  .Where(p => p.GetCustomAttribute<FunctionAttribute>() != null && p.GetCustomAttribute<OpenApiOperationAttribute>() != null)
                                  .Where(p => !p.GetCustomAttributes<OpenApiRequestBodyAttribute>(inherit: false).Any())
                                  .SelectMany(p => p.GetParameters())
                                  .Where(param => typeof(IBaseRequest).IsAssignableFrom(param.ParameterType))
                                  .Select(param => param)
                                  select p.ParameterType;


        IEnumerable<Type> fourth = from p in elements
                                  .Where(p => p.GetCustomAttribute<FunctionAttribute>() != null && p.GetCustomAttribute<OpenApiOperationAttribute>() != null)
                                  .Where(p => !p.GetCustomAttributes<OpenApiResponseWithBodyAttribute>(inherit: false).Any())
                                  .SelectMany(p => p.GetParameters())
                                  .Where(param => typeof(IBaseRequest).IsAssignableFrom(param.ParameterType))
                                  .SelectMany(param => param.ParameterType.GetInterfaces())
                                 .Where(i => i.IsAssignableTo(typeof(IBaseRequest)) && i != typeof(IRequest) && i != typeof(IBaseRequest))
                                   select p.GenericTypeArguments.Single();

        IEnumerable<Type> validators = new[] { typeof(ValidationErrors) };

        List<Type> list = (from p in (from p in first.Union(second).Union(third).Union(fourth).Union(validators)
                                      select (!p.IsOpenApiArray() && !p.IsOpenApiDictionary()) ? p : p.GetOpenApiSubType()).Distinct()
                           where !p.IsSimpleType()
                           where p.IsReferentialType()
                           where !typeof(Array).IsAssignableFrom(p)
                           select p).ToList();
        Dictionary<string, OpenApiSchema> dictionary = new Dictionary<string, OpenApiSchema>();
        Dictionary<string, OpenApiSchema> schemas = new Dictionary<string, OpenApiSchema>();
        Dictionary<string, Type> dictionary2 = new Dictionary<string, Type>();
        foreach (Type item in list)
        {
            string openApiReferenceId = item.GetOpenApiReferenceId(item.IsOpenApiDictionary(), item.IsOpenApiArray(), namingStrategy);
            if (!dictionary2.ContainsKey(openApiReferenceId))
            {
                dictionary2.Add(openApiReferenceId, item);
            }
        }

        acceptor.Types = dictionary2;
        acceptor.RootSchemas = dictionary;
        acceptor.Schemas = schemas;
        acceptor.Accept(collection, namingStrategy);
        return (from p in schemas.Concat(dictionary.Where(p => !Enumerable.Contains(schemas.Keys, p.Key))).Distinct()
                where p.Key.ToUpperInvariant() != "OBJECT"
                orderby p.Key
                select p).ToDictionary(p => p.Key, delegate (KeyValuePair<string, OpenApiSchema> p)
                {
                    p.Value.Title = null;
                    return p.Value;
                });
    }

    public List<OpenApiSecurityRequirement> GetOpenApiSecurityRequirement(MethodInfo element, NamingStrategy namingStrategy = null)
    {
        return _internalDocumentHelper.GetOpenApiSecurityRequirement(element, namingStrategy);
    }

    [Obsolete("This method is obsolete and will be removed in the next major version. Please use GetOpenApiSecurityRequirement(MethodInfo, NamingStrategy) instead.")]
    public Dictionary<string, OpenApiSecurityScheme> GetOpenApiSecuritySchemes()
    {
        return _internalDocumentHelper.GetOpenApiSecuritySchemes();
    }

    public Dictionary<string, OpenApiSecurityScheme> GetOpenApiSecuritySchemes(List<MethodInfo> elements, NamingStrategy namingStrategy = null)
    {
        return _internalDocumentHelper.GetOpenApiSecuritySchemes(elements, namingStrategy);
    }
}

[JsonObject]
public class ValidationErrors
{
    [JsonProperty("errors")]
    public ICollection<Error> Errors { get; set; }
}

[JsonObject]
public class Error
{
    [JsonProperty("errorCode")]
    public string ErrorCode { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("property")]
    public string Property { get; set; }
}
