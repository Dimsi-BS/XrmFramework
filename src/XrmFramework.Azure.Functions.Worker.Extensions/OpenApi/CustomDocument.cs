using System.Dynamic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Filters;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Visitors;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using XrmFramework.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using YamlDotNet.Serialization;
using StringExtensions = Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions.StringExtensions;

namespace XrmFramework.Azure.Functions.Worker.Extensions.OpenApi;

public class CustomDocument : IDocument
{
    private readonly IDocumentHelper? _helper;
    private readonly IModelMetadataProvider? _modelMetadataProvider;
    private NamingStrategy? _strategy;

    private VisitorCollection? _collection;

    private IHttpRequestDataObject? _req;

    public OpenApiDocument OpenApiDocument { get; private set; } = null!;

    //
    // Résumé :
    //     Initializes a new instance of the Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Document
    //     class.
    public CustomDocument(IDocumentHelper helper, IModelMetadataProvider modelMetadataProvider)
    {
        _helper = helper.ThrowIfNullOrDefault();
        _modelMetadataProvider = modelMetadataProvider;
    }

    public CustomDocument(OpenApiDocument openApiDocument)
    {
        OpenApiDocument = openApiDocument;
    }

    public IDocument InitialiseDocument()
    {
        OpenApiDocument = new OpenApiDocument
        {
            Components = new OpenApiComponents()
        };
        return this;
    }

    public IDocument AddMetadata(OpenApiInfo info)
    {
        OpenApiDocument.Info = info;
        return this;
    }

    public IDocument AddServer(IHttpRequestDataObject req, string routePrefix, IOpenApiConfigurationOptions? options = null)
    {
        _req = req;
        string value = (string.IsNullOrWhiteSpace(routePrefix) ? string.Empty : ("/" + routePrefix));
        string baseUrl = $"{_req.GetScheme(options)}://{_req.Host}{value}";
        OpenApiServer item = new OpenApiServer
        {
            Url = baseUrl
        };
        if (options.IsNullOrDefault())
        {
            OpenApiDocument.Servers = new List<OpenApiServer> { item };
            return this;
        }

        List<OpenApiServer> list = options!.Servers.Where(p => p.Url.TrimEnd('/') != baseUrl.TrimEnd('/')).ToList();
        if (!list.Any())
        {
            list.Insert(0, item);
        }

        if (options.IncludeRequestingHostName && list.All(p => p.Url.TrimEnd('/') != baseUrl.TrimEnd('/')))
        {
            list.Insert(0, item);
        }

        OpenApiDocument.Servers = list;
        return this;
    }

    public IDocument AddNamingStrategy(NamingStrategy strategy)
    {
        _strategy = strategy.ThrowIfNullOrDefault();
        return this;
    }

    public IDocument AddVisitors(VisitorCollection collection)
    {
        _collection = collection.ThrowIfNullOrDefault();
        return this;
    }

    public IDocument Build(string assemblyPath, OpenApiVersionType version = OpenApiVersionType.V2)
    {
        Assembly assembly = Assembly.Load(assemblyPath);
        return Build(assembly, version);
    }

    public IDocument Build(Assembly assembly, OpenApiVersionType version = OpenApiVersionType.V2)
    {
        if (_strategy.IsNullOrDefault())
        {
            _strategy = new DefaultNamingStrategy();
        }

        OpenApiPaths openApiPaths = new OpenApiPaths();
        string[] tags = StringExtensions.ToArray(_req.Query["tag"]);
        List<MethodInfo> httpTriggerMethods = Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions.DocumentHelperExtensions.GetHttpTriggerMethods(_helper, assembly, tags);
        foreach (MethodInfo item in httpTriggerMethods)
        {
            HttpTriggerAttribute httpTriggerAttribute = Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions.DocumentHelperExtensions.GetHttpTriggerAttribute(_helper, item);
            if (httpTriggerAttribute.IsNullOrDefault())
            {
                continue;
            }

            FunctionAttribute functionNameAttribute = Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions.DocumentHelperExtensions.GetFunctionNameAttribute(_helper, item);
            if (functionNameAttribute.IsNullOrDefault())
            {
                continue;
            }

            string httpEndpoint = _helper.GetHttpEndpoint(functionNameAttribute, httpTriggerAttribute);
            if (!httpEndpoint.IsNullOrWhiteSpace())
            {
                OperationType httpVerb = _helper.GetHttpVerb(httpTriggerAttribute);
                OpenApiPathItem openApiPath = _helper!.GetOpenApiPath(httpEndpoint, openApiPaths);
                IDictionary<OperationType, OpenApiOperation> operations = openApiPath.Operations;
                OpenApiOperation openApiOperation = _helper.GetOpenApiOperation(item, functionNameAttribute, httpVerb);
                if (!openApiOperation.IsNullOrDefault())
                {
                    openApiOperation.Security = _helper.GetOpenApiSecurityRequirement(item, _strategy);
                    openApiOperation.Parameters = _helper.GetOpenApiParameters(item, httpTriggerAttribute, _strategy, _collection, version, httpVerb, _modelMetadataProvider);
                    openApiOperation.RequestBody = _helper.GetOpenApiRequestBody(item, _strategy, _collection, version);
                    openApiOperation.Responses = _helper.GetOpenApiResponses(item, _strategy, _collection, version);
                    operations[httpVerb] = openApiOperation;
                    openApiPath.Operations = operations;
                    openApiPaths[httpEndpoint] = openApiPath;
                }
            }
        }

        OpenApiDocument.Paths = openApiPaths;
        OpenApiDocument.Components.Schemas = _helper!.GetOpenApiSchemas(httpTriggerMethods, _strategy, _collection);
        OpenApiDocument.Components.SecuritySchemes = _helper.GetOpenApiSecuritySchemes(httpTriggerMethods, _strategy);
        return this;
    }

    public IDocument ApplyDocumentFilters(DocumentFilterCollection collection)
    {
        foreach (IDocumentFilter documentFilter in collection.ThrowIfNullOrDefault().DocumentFilters)
        {
            documentFilter.Apply(_req, OpenApiDocument);
        }

        return this;
    }

    public async Task<string> RenderAsync(OpenApiSpecVersion version, OpenApiFormat format)
    {
        return await Task.Factory.StartNew(() => Render(version, format)).ConfigureAwait(continueOnCapturedContext: false);
    }

    private string Render(OpenApiSpecVersion version, OpenApiFormat format)
    {
        string? text;
        using (StringWriter stringWriter = new StringWriter())
        {
            OpenApiDocument.Serialise(stringWriter, version, OpenApiFormat.Json);
            text = stringWriter.ToString();
        }

        string? result;
        using (StringWriter stringWriter2 = new StringWriter())
        {
            OpenApiDocument.Serialise(stringWriter2, version, OpenApiFormat.Yaml);
            result = stringWriter2.ToString();
        }

        if (version != 0)
        {
            if (format != 0)
            {
                return result;
            }

            return text;
        }

        var descriptionPropertyName = "description";
        
        JObject? jObject = JsonConvert.DeserializeObject<JObject>(text);
        foreach (JObject item in (from p in (from p in jObject!.DescendantsAndSelf()
                         where p.Type == JTokenType.Property && (p as JProperty).Name == "parameters"
                         select p).SelectMany(p => p.Values<JArray>().SelectMany(q => q!.Children<JObject>()))
                     where p.Value<string>("in") == null
                     where p.Value<string>(descriptionPropertyName) != null
                     where p.Value<string>(descriptionPropertyName).Contains("[formData]")
                     select p).ToList())
        {
            item["in"] = "formData";
            item[descriptionPropertyName] = item.Value<string>(descriptionPropertyName)!.Replace("[formData]", string.Empty);
        }

        string text2 = JsonConvert.SerializeObject(jObject, Formatting.Indented);
        if (format == OpenApiFormat.Json)
        {
            return text2;
        }

        ExpandoObjectConverter expandoObjectConverter = new ExpandoObjectConverter();
        ExpandoObject? graph = JsonConvert.DeserializeObject<ExpandoObject>(text2, expandoObjectConverter);
        return new SerializerBuilder().Build().Serialize(graph!);
    }
}
