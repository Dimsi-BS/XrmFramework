using Microsoft.Azure.Functions.Worker.Extensions.OpenApi;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Visitors;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using XrmFramework.Azure.Functions.Worker.Extensions.OpenApi.Extensions;

namespace XrmFramework.Azure.Functions.Worker.Extensions.OpenApi;


// ReSharper disable once UnusedType.Global
public class CustomOpenApiHttpTriggerContext(
    IOpenApiConfigurationOptions? configOptions = null,
    IOpenApiHttpTriggerAuthorization? httpTriggerAuthorization = null,
    IOpenApiCustomUIOptions? uiOptions = null)
    : OpenApiHttpTriggerContext(configOptions, httpTriggerAuthorization, uiOptions)
{
    public override VisitorCollection GetVisitorCollection()
    {
        var visitors = base.GetVisitorCollection()
            .SwitchTypeVisitor<ListObjectTypeVisitor, CustomListObjectTypeVisitor>()
            .SwitchTypeVisitor<DictionaryObjectTypeVisitor, CustomDictionaryObjectTypeVisitor>();

        return visitors;
    }
};

public class CustomListObjectTypeVisitor(VisitorCollection visitorCollection) 
    : ListObjectTypeVisitor(visitorCollection)
{
    public override bool IsVisitable(Type type)
        => base.IsVisitable(type) && !type.IsDataCollectionType();

    public override void Visit(IAcceptor acceptor, KeyValuePair<string, Type> type, NamingStrategy namingStrategy, params Attribute[] attributes)
    {
        var attributeList = attributes.ToList();

        var openApiSchemaVisibilityAttribute = attributeList.OfType<OpenApiSchemaVisibilityAttribute>().FirstOrDefault(attributes => attributes != null);

        if (openApiSchemaVisibilityAttribute?.Visibility == Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums.OpenApiVisibilityType.Internal)
        {
            return;
        }

        var jsonPropertyAttribute = attributeList.OfType<JsonPropertyAttribute>().FirstOrDefault(attributes => attributes != null);

        if (jsonPropertyAttribute == null)
        {
            return;
        }

        base.Visit(acceptor, type, namingStrategy, attributes);
    }
}

public sealed class CustomDictionaryObjectTypeVisitor(VisitorCollection visitorCollection) : DictionaryObjectTypeVisitor(visitorCollection)
{
    public override bool IsVisitable(Type type) 
        => base.IsVisitable(type) || type.IsDataCollectionType();

    public override void Visit(IAcceptor acceptor, KeyValuePair<string, Type> type, NamingStrategy namingStrategy, params Attribute[] attributes)
    { 
        if (acceptor is not IOpenApiSchemaAcceptor openApiSchemaAcceptor)
        {
            return;
        }

        openApiSchemaAcceptor.Schemas.Add(type.Key, PayloadVisit(type.Value, namingStrategy));
    }

    public override OpenApiSchema PayloadVisit(Type type, NamingStrategy namingStrategy)
    {
        if (type.IsDataCollectionType())
        {
            if (type.BaseType!.GenericTypeArguments.Length == 2)
            {
                return VisitorCollection.PayloadVisit(typeof(Dictionary<,>).MakeGenericType(type.BaseType.GenericTypeArguments), namingStrategy);
            }
            else
            {
                return VisitorCollection.PayloadVisit(typeof(List<>).MakeGenericType(type.BaseType.GenericTypeArguments), namingStrategy);
            }
        }

        return base.PayloadVisit(type, namingStrategy); 
    }
}
