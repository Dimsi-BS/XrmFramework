// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace XrmFramework.BindingModel
{
    /// <summary>
    /// Orchestrates upsert pipelines for <see cref="IXmlModel"/> graphs: builds the full set of
    /// requests, applies any custom behaviour, then executes them as transactions or ExecuteMultiple.
    /// </summary>
    internal static class BindingModelUpsertExecutor
    {
        private const int BatchSize = 200;

        // ---------------------------------------------------------------------
        // Request container
        // ---------------------------------------------------------------------

        public static RequestContainer BuildRequests(IXmlModel xmlModel, IOrganizationService service, bool disablePluginsExecution)
        {
            var container = new RequestContainer(disablePluginsExecution);
            FillContainer(xmlModel, service, container, extendedModel: null);
            return container;
        }

        private static void FillContainer(IXmlModel xmlModel, IOrganizationService service, RequestContainer container, IBindingModel extendedModel)
        {
            if (xmlModel == null)
            {
                return;
            }

            var modelDefinition = DefinitionCache.GetModelDefinition(xmlModel.GetType());

            if (xmlModel is IBindingModel model)
            {
                container.AddModel(model, service, extendedModel);
            }

            var upsertableProperties = modelDefinition.UpsertableAttributes;
            if (upsertableProperties.Count == 0)
            {
                return;
            }

            if (upsertableProperties.Count == 1)
            {
                FillFromProperty(xmlModel, upsertableProperties.Single(), service, container);
                return;
            }

            // Properties with an UpsertOrder run in order first, unordered ones afterward.
            var ordered = new SortedDictionary<int, AttributeDefinition>();
            var unordered = new List<AttributeDefinition>();

            foreach (var property in upsertableProperties)
            {
                if (property.UpsertOrder.HasValue)
                {
                    ordered.Add(property.UpsertOrder.Value, property);
                    continue;
                }

                // Skip non-updatable CRM-mapped properties.
                if (property.CrmMappingAttribute != null && !property.CrmMappingAttribute.IsValidForUpdate)
                {
                    continue;
                }

                unordered.Add(property);
            }

            foreach (var property in ordered.Values)
            {
                FillFromProperty(xmlModel, property, service, container);
            }

            foreach (var property in unordered)
            {
                FillFromProperty(xmlModel, property, service, container);
            }
        }

        private static void FillFromProperty(object model, AttributeDefinition property, IOrganizationService service, RequestContainer container)
        {
            if (property.CrmMappingAttribute != null && !property.CrmMappingAttribute.IsValidForUpdate)
            {
                return;
            }

            var extendedModel = property.IsExtendBindingModel ? model as IBindingModel : null;

            if (typeof(IBindingModel).IsAssignableFrom(property.PropertyType))
            {
                var bindingModel = (IBindingModel)property.GetValue(model);
                if (bindingModel == null)
                {
                    return;
                }

                if (property.IsExtendBindingModel && model is IBindingModel parentModel)
                {
                    bindingModel.Id = parentModel.Id;
                }

                FillContainer(bindingModel, service, container, extendedModel);
                return;
            }

            if (property.GetValue(model) is not IEnumerable values)
            {
                return;
            }

            foreach (IXmlModel value in values)
            {
                FillContainer(value, service, container, extendedModel);
            }
        }

        // ---------------------------------------------------------------------
        // Upsert entry points
        // ---------------------------------------------------------------------

        public static T UpsertFromDocument<T>(IOrganizationService service, XDocument doc, UpsertSettings settings) where T : IXmlModel
            => UpsertFromElement<T>(service, doc.Root, settings);

        public static T UpsertFromElement<T>(IOrganizationService service, XElement doc, UpsertSettings settings) where T : IXmlModel
        {
            var model = XmlBindingModelMapper.FromXElement(doc, typeof(T));
            return UpsertModel<T>(service, (T)model, settings);
        }

        public static T UpsertModel<T>(IOrganizationService service, T model, UpsertSettings settings) where T : IXmlModel
        {
            settings ??= new UpsertSettings();
            var type = typeof(T);

            if (TryApplyCustomBehaviour(service, type, model))
            {
                return model;
            }

            var requests = model.GetUpsertRequests(service, settings.DisablePluginsExecution);

            if (settings.UseTransactionRequest)
            {
                RunAsTransactions(service, requests);
            }
            else
            {
                RunAsExecuteMultiple(service, requests, settings.ContinueOnError);
            }

            return model;
        }

        private static bool TryApplyCustomBehaviour(IOrganizationService service, Type type, object model)
        {
            var behaviourAttribute = type.GetCustomAttribute<UpsertBehaviourAttribute>();
            var constructor = behaviourAttribute?.BehaviourType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                return false;
            }

            var behaviour = constructor.Invoke(Array.Empty<object>());
            var method = behaviourAttribute.BehaviourType.GetMethod("ApplyBehaviour");
            if (method == null)
            {
                return true; // behaviour attribute present but malformed: still skip default path (original behaviour).
            }

            method.Invoke(behaviour, new[] { service, model });
            return true;
        }

        private static void RunAsTransactions(IOrganizationService service, RequestContainer requests)
        {
            var skip = 0;
            while (requests.Count > skip)
            {
                var batch = requests.Skip(skip).Take(BatchSize).ToList();
                var responses = ExecuteTransactionRequests<UpsertResponse>(service, batch);

                for (var i = 0; i < batch.Count; i++)
                {
                    requests.UpdateIds(batch[i], responses[i].Target.Id);
                }

                skip += BatchSize;
            }
        }

        private static void RunAsExecuteMultiple(IOrganizationService service, RequestContainer requests, bool continueOnError)
        {
            var requestList = requests.ToList();
            var jobResult = service.ExecuteMultiple(requestList, r => r, "Update", BatchSize, continueOnError);

            foreach (var pair in jobResult.Responses)
            {
                var request = requestList[pair.Key];
                if (pair.Value is UpsertResponse response)
                {
                    requests.UpdateIds(request, response.Target.Id);
                }
            }

            if (jobResult.ErrorMessages.Any())
            {
                var errorContent = new StringBuilder();
                foreach (var errorMessage in jobResult.ErrorMessages)
                {
                    errorContent.AppendLine($"====> requête {errorMessage.Key} : {errorMessage.Value}\r\n");
                }
                throw new Exception(errorContent.ToString());
            }
        }

        private static IList<T> ExecuteTransactionRequests<T>(IOrganizationService service, IEnumerable<OrganizationRequest> requests) where T : OrganizationResponse
        {
            var request = new ExecuteTransactionRequest
            {
                Requests = new OrganizationRequestCollection(),
                ReturnResponses = true,
            };
            request.Requests.AddRange(requests);
            var response = (ExecuteTransactionResponse)service.Execute(request);

            return response.Responses.Cast<T>().ToList();
        }

        // ---------------------------------------------------------------------
        // Public ExecuteMultiple helper
        // ---------------------------------------------------------------------

        public static BindingModelHelper.JobResult ExecuteMultiple<T>(
            IOrganizationService service,
            IList<T> objects,
            Func<T, OrganizationRequest> requestBuilder,
            string message,
            int nbRequest,
            bool continueOnError)
        {
            var result = new BindingModelHelper.JobResult();

            if (objects.Count == 0)
            {
                return result;
            }

            var batches = new ExecuteMultipleBatcher<T>(service, objects, requestBuilder, message, nbRequest, continueOnError, result);
            batches.Run();

            return result;
        }

        /// <summary>
        /// Encapsulates the progress-reporting batch loop behind <see cref="ExecuteMultiple{T}"/>,
        /// which was previously a ~120-line inline method.
        /// </summary>
        private sealed class ExecuteMultipleBatcher<T>
        {
            private readonly IOrganizationService _service;
            private readonly IList<T> _objects;
            private readonly Func<T, OrganizationRequest> _requestBuilder;
            private readonly string _message;
            private readonly int _batchSize;
            private readonly bool _continueOnError;
            private readonly BindingModelHelper.JobResult _result;

            public ExecuteMultipleBatcher(
                IOrganizationService service,
                IList<T> objects,
                Func<T, OrganizationRequest> requestBuilder,
                string message,
                int batchSize,
                bool continueOnError,
                BindingModelHelper.JobResult result)
            {
                _service = service;
                _objects = objects;
                _requestBuilder = requestBuilder;
                _message = message;
                _batchSize = batchSize;
                _continueOnError = continueOnError;
                _result = result;
            }

            public void Run()
            {
                var stopwatch = Stopwatch.StartNew();
                Console.Write("Updating {0}...", _message);

                var offset = 0;
                var multipleRequest = new ExecuteMultipleRequest
                {
                    Settings = new ExecuteMultipleSettings
                    {
                        ContinueOnError = _continueOnError,
                        ReturnResponses = true,
                    },
                    Requests = new OrganizationRequestCollection(),
                };

                var createdRecord = 0;
                var updatedRecord = 0;
                var rejectedRecord = 0;

                while (offset < _objects.Count)
                {
                    multipleRequest.Requests.Clear();

                    var batch = _objects.Skip(offset).Take(_batchSize)
                        .Select(_requestBuilder)
                        .Select(PromoteCreateKeyAttributes);

                    multipleRequest.Requests.AddRange(batch);

                    var response = (ExecuteMultipleResponse)_service.Execute(multipleRequest);

                    var errorCount = 0;
                    if (response.IsFaulted)
                    {
                        errorCount = response.Responses.Count(r => r.Fault != null);
                        rejectedRecord += errorCount;
                    }

                    foreach (var res in response.Responses)
                    {
                        RecordResponse(res, ref createdRecord, ref updatedRecord);
                    }

                    var errorMessage = errorCount == 0 ? string.Empty : $" ({errorCount} erreurs)";
                    var remainingTime = TimeSpan.FromMilliseconds(
                        (stopwatch.ElapsedMilliseconds * _objects.Count / (offset + multipleRequest.Requests.Count))
                        - stopwatch.ElapsedMilliseconds);

                    Console.Write("\rUpdated {0}/{1} {2} in {3}{4} (ETA {5})",
                        offset + multipleRequest.Requests.Count, _objects.Count, _message,
                        stopwatch.Elapsed, errorMessage, remainingTime);

                    offset += _batchSize;
                }

                stopwatch.Stop();
                Console.WriteLine();

                _result.NbCreated = createdRecord;
                _result.NbUpdated = updatedRecord;
                _result.NbRejected = rejectedRecord;
            }

            /// <summary>
            /// CreateRequest doesn't accept KeyAttributes — they must be promoted to regular
            /// attributes first. Keeps original behaviour (mutates the entity in-place).
            /// </summary>
            private static OrganizationRequest PromoteCreateKeyAttributes(OrganizationRequest request)
            {
                if (request is not CreateRequest createRequest)
                {
                    return request;
                }

                var entity = createRequest.Target;
                for (var i = entity.KeyAttributes.Count - 1; i >= 0; i--)
                {
                    var keyName = entity.KeyAttributes.Keys.ElementAt(i);
                    entity[keyName] = entity.KeyAttributes[keyName];
                    entity.KeyAttributes.Remove(keyName);
                }
                return request;
            }

            private void RecordResponse(ExecuteMultipleResponseItem res, ref int createdRecord, ref int updatedRecord)
            {
                if (res.Response != null)
                {
                    _result.Responses.Add(new KeyValuePair<int, OrganizationResponse>(res.RequestIndex, res.Response));

                    switch (res.Response)
                    {
                        case UpsertResponse upsertResponse when upsertResponse.RecordCreated:
                            createdRecord++;
                            break;
                        case UpsertResponse _:
                            updatedRecord++;
                            break;
                        case UpdateResponse _:
                            updatedRecord++;
                            break;
                        case CreateResponse _:
                            createdRecord++;
                            break;
                    }
                    return;
                }

                if (res.Fault == null)
                {
                    return;
                }

                var message = res.Fault.InnerFault?.Message ?? BuildFaultMessage(res.Fault);
                _result.ErrorMessages.Add(new KeyValuePair<int, string>(res.RequestIndex, message));
            }

            private static string BuildFaultMessage(Microsoft.Xrm.Sdk.OrganizationServiceFault fault)
            {
                if (string.IsNullOrEmpty(fault.TraceText))
                {
                    return fault.Message;
                }
                return $"{fault.Message}\r\nDetail:\r\n{fault.TraceText}";
            }
        }
    }
}
