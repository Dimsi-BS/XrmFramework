// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using EntityReference = Microsoft.Xrm.Sdk.EntityReference;

namespace XrmFramework.BindingModel
{
    /// <summary>
    /// Facade exposing the public surface for binding-model conversion, query building, and upsert execution.
    /// </summary>
    /// <remarks>
    /// The actual implementations live in focused internal classes:
    /// <list type="bullet">
    /// <item><see cref="EntityToBindingModelMapper"/> — Entity → IBindingModel.</item>
    /// <item><see cref="BindingModelToEntityMapper"/> — IBindingModel → Entity.</item>
    /// <item><see cref="XmlBindingModelMapper"/> — XElement ↔ IXmlModel.</item>
    /// <item><see cref="DtoBindingModelMapper"/> — DTO ↔ IBindingModel.</item>
    /// <item><see cref="BindingModelQueryBuilder"/> — builds QueryExpressions from a model type.</item>
    /// <item><see cref="BindingModelUpsertExecutor"/> — upsert pipeline + ExecuteMultiple batching.</item>
    /// </list>
    /// Every previously-public signature on this class is preserved; callers need no changes.
    /// </remarks>
    public static class BindingModelHelper
    {
        // ---------------------------------------------------------------------
        // Entity <-> IBindingModel
        // ---------------------------------------------------------------------

        public static IEnumerable<T> ToBindingModel<T>(this IEnumerable<Entity> entity) where T : IBindingModel
            => EntityToBindingModelMapper.MapMany<T>(entity);

        public static T ToBindingModel<T>(this Entity entity) where T : IBindingModel
            => (T)EntityToBindingModelMapper.Map(entity, typeof(T));

        public static IBindingModel ToBindingModel(this Entity entity, Type type)
            => EntityToBindingModelMapper.Map(entity, type);

        public static EntityReference ToEntityReference<T>(this T model, IOrganizationService service) where T : IBindingModel
            => BindingModelToEntityMapper.ToEntityReference(model, service);

        public static Entity ToEntity(this IBindingModel bindingModel, IOrganizationService service, bool fillRelatedEntities = true)
            => BindingModelToEntityMapper.Map(bindingModel.GetType(), bindingModel, service, fillRelatedEntities);

        public static Entity ToEntity(Type type, object bindingModel, IOrganizationService service, bool fillRelatedEntities = true)
            => BindingModelToEntityMapper.Map(type, bindingModel, service, fillRelatedEntities);

        // ---------------------------------------------------------------------
        // QueryExpression building
        // ---------------------------------------------------------------------

        public static QueryExpression GetRetrieveAllQuery<T>() where T : IBindingModel
            => BindingModelQueryBuilder.BuildRetrieveAll(typeof(T));

        public static QueryExpression GetRetrieveAllQuery(Type bindingModelType)
            => BindingModelQueryBuilder.BuildRetrieveAll(bindingModelType);

        public static QueryExpression GetQueryToFilter(Type bindingModelType, Func<Relationship, LinkEntity, JoinOperator> filter)
            => BindingModelQueryBuilder.BuildFiltered(bindingModelType, filter);

        public static void AddQueryFilter(
            Type bindingModelType,
            Func<Relationship, LinkEntity, JoinOperator> filter,
            ColumnSet columnSet,
            DataCollection<LinkEntity> links,
            int depth = 1,
            string linkAlias = "")
            => BindingModelQueryBuilder.AppendFilter(bindingModelType, filter, columnSet, links, depth, linkAlias);

        // ---------------------------------------------------------------------
        // XElement <-> IXmlModel
        // ---------------------------------------------------------------------

        public static T ToBindingModel<T>(this XElement element) where T : IXmlModel
            => element == null ? default : (T)XmlBindingModelMapper.FromXElement(element, typeof(T));

        public static object ToBindingModel(this XElement element, Type type)
            => XmlBindingModelMapper.FromXElement(element, type);

        public static XElement ToXElement<T>(this T bindingModel) where T : IXmlModel
            => Equals(bindingModel, default(T)) ? null : XmlBindingModelMapper.ToXElement(bindingModel.GetType(), bindingModel);

        public static XElement ToXElement(Type type, object bindingModel)
            => XmlBindingModelMapper.ToXElement(type, bindingModel);

        // ---------------------------------------------------------------------
        // DTO <-> IBindingModel
        // ---------------------------------------------------------------------

        public static IBindingModel ToBindingModel(object dto, Type bindingType = null)
            => DtoBindingModelMapper.FromDto(dto, bindingType);

        public static Type GetCorrespondingBindingType(Type dtoType)
            => DtoBindingModelMapper.GetCorrespondingBindingType(dtoType);

        public static T ToDto<T>(IBindingModel model) where T : new()
            => DtoBindingModelMapper.ToDto<T>(model);

        public static U FromDto<T, U>(T dto) where T : new() where U : IBindingModel, new()
            => DtoBindingModelMapper.FromDtoStrict<T, U>(dto);

        // ---------------------------------------------------------------------
        // Upsert pipeline
        // ---------------------------------------------------------------------

        public static RequestContainer GetUpsertRequests(this IXmlModel xmlModel, IOrganizationService service, bool disablePluginsExecution = false)
            => BindingModelUpsertExecutor.BuildRequests(xmlModel, service, disablePluginsExecution);

        public static T Upsert<T>(this IOrganizationService service, XDocument doc, UpsertSettings settings = null) where T : IXmlModel
            => BindingModelUpsertExecutor.UpsertFromDocument<T>(service, doc, settings);

        public static T Upsert<T>(this IOrganizationService service, XElement doc, UpsertSettings settings = null) where T : IXmlModel
            => BindingModelUpsertExecutor.UpsertFromElement<T>(service, doc, settings);

        public static T Upsert<T>(this IOrganizationService service, T model, UpsertSettings settings = null) where T : IXmlModel
            => BindingModelUpsertExecutor.UpsertModel(service, model, settings);

        public static JobResult ExecuteMultiple<T>(
            this IOrganizationService service,
            IList<T> objects,
            Func<T, OrganizationRequest> RequestBuilder,
            string message,
            int nbRequest = 500,
            bool continueOnError = true)
            => BindingModelUpsertExecutor.ExecuteMultiple(service, objects, RequestBuilder, message, nbRequest, continueOnError);

        // ---------------------------------------------------------------------
        // Public companion type (kept for binary/source compatibility)
        // ---------------------------------------------------------------------

        public class JobResult
        {
            public int NbCreated { get; set; }

            public int NbUpdated { get; set; }

            public int NbRejected { get; set; }

            public ICollection<KeyValuePair<int, string>> ErrorMessages { get; } = new List<KeyValuePair<int, string>>();

            public ICollection<KeyValuePair<int, OrganizationResponse>> Responses { get; } = new List<KeyValuePair<int, OrganizationResponse>>();
        }
    }

    public class UpsertSettings
    {
        public bool UseTransactionRequest
        {
            get;
            set
            {
                field = value;
                if (value)
                {
                    ContinueOnError = false;
                }
            }
        } = true;

        public bool DisablePluginsExecution { get; set; }

        public bool ContinueOnError
        {
            get;
            set
            {
                field = value;
                if (value)
                {
                    UseTransactionRequest = false;
                }
            }
        }
    }
}
