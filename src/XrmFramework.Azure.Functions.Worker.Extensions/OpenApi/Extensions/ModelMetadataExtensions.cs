using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Visitors;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace XrmFramework.Azure.Functions.Worker.Extensions.OpenApi.Extensions;

public static class ModelMetadataExtensions
{
    public static OpenApiParameter ToOpenApiParameter(this Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata metadata, NamingStrategy namingStrategy, VisitorCollection collection)
    {
        if (metadata is DefaultModelMetadata defaultModelMetadata)
        {
            OpenApiSchema openApiSchema = collection.PayloadVisit(metadata.ModelType, namingStrategy);
            
            if (metadata.ModelType.IsReferentialType() && !metadata.ModelType.IsOpenApiNullable() && !metadata.ModelType.IsOpenApiArray() && !metadata.ModelType.IsOpenApiDictionary())
            {
                OpenApiReference reference = new OpenApiReference
                {
                    Type = ReferenceType.Schema,
                    Id = metadata.ModelType.GetOpenApiReferenceId(isDictionary: false, isList: false, namingStrategy)
                };
                openApiSchema.Reference = reference;
            }

            var parameter = new OpenApiParameter
            {
                Name = namingStrategy.GetPropertyName(defaultModelMetadata.PropertyName, hasSpecifiedName: false),
                In = defaultModelMetadata.Attributes.PropertyAttributes.OfType<FromRouteAttribute>().Any() ? ParameterLocation.Path : ParameterLocation.Query,
                Required = defaultModelMetadata.IsRequired,
                Schema = openApiSchema
            };

            return parameter;
        }

        return null;
    }
}
