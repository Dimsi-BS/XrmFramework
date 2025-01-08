using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Visitors;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace XrmFramework.Azure.Functions.Worker.Extensions.OpenApi.Extensions;

public static class DocumentHelperExtensions
{
    public static List<OpenApiParameter> GetOpenApiParameters(this IDocumentHelper helper, MethodInfo element, HttpTriggerAttribute trigger, NamingStrategy namingStrategy, VisitorCollection collection, OpenApiVersionType version, OperationType operationType, IModelMetadataProvider modelMetadataProvider)
    {
        if (element.GetParameters().All(p => !p.ParameterType.IsAssignableTo(typeof(IBaseRequest))))
        {
            return helper.GetOpenApiParameters(element, trigger, namingStrategy, collection, version);
        }

        var requestType = element.GetParameters().FirstOrDefault(p => p.ParameterType.IsAssignableTo(typeof(IBaseRequest)))?.ParameterType;

        var metadatas = modelMetadataProvider.GetMetadataForProperties(requestType);

        var list = new List<OpenApiParameter>();

        foreach (var metadata in metadatas.Where(m => operationType is OperationType.Get or OperationType.Delete || (m is DefaultModelMetadata defaultModelMetadata && defaultModelMetadata.Attributes.PropertyAttributes.Any(p => p is FromRouteAttribute || p is FromQueryAttribute))))
        {
            list.Add(metadata.ToOpenApiParameter(namingStrategy, collection));
        }

        return list;
    }
}
