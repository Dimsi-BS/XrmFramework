// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Plugins;

namespace Model
{
    public class RequestContainer : IEnumerable<UpsertRequest>
    {
        private Dictionary<Entity, List<IBindingModel>> ExistingRequests { get; } = new Dictionary<Entity, List<IBindingModel>>();

        public int Count => ExistingRequests.Keys.Count;

        public void AddModel(IBindingModel model, IOrganizationService service, IBindingModel extendedModel = null)
        {
            var entity = model.ToEntity(service);

            Entity existingEntity = null;

            if (extendedModel != null)
            {
                foreach (var tempEntity in ExistingRequests.Keys)
                {
                    if (ExistingRequests[tempEntity].Contains(extendedModel))
                    {
                        existingEntity = tempEntity;
                        break;
                    }
                }
            }

            if (existingEntity == null)
            {
                foreach (var tempEntity in ExistingRequests.Keys)
                {
                    if (entity.LogicalName != tempEntity.LogicalName)
                    {
                        continue;
                    }

                    if (entity.Id != Guid.Empty)
                    {
                        if (entity.Id == tempEntity.Id)
                        {
                            existingEntity = tempEntity;
                            break;
                        }
                    }
                    else if (entity.KeyAttributes.Any())
                    {
                        var isOk = true;
                        foreach (var key in entity.KeyAttributes.Keys)
                        {
                            var value = entity.KeyAttributes[key];

                            if (!tempEntity.KeyAttributes.ContainsKey(key) || tempEntity.KeyAttributes[key] != value)
                            {
                                isOk = false;
                                break;
                            }
                        }

                        if (!isOk)
                        {
                            continue;
                        }
                        existingEntity = tempEntity;
                        break;
                    }
                }
            }

            List<IBindingModel> list;

            if (existingEntity == null)
            {
                existingEntity = entity;
                list = new List<IBindingModel>();
                ExistingRequests[existingEntity] = list;
            }
            else
            {
                list = ExistingRequests[existingEntity];
                existingEntity.MergeWith(entity);
            }

            list.Add(model);
        }

        public IEnumerator<UpsertRequest> GetEnumerator() => ExistingRequests.Keys.Select(e => new UpsertRequest { Target = e }).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void UpdateIds(UpsertRequest upsertRequest, Guid id)
        {
            if (ExistingRequests.ContainsKey(upsertRequest.Target))
            {
                foreach (var model in ExistingRequests[upsertRequest.Target])
                {
                    model.Id = id;
                }
            }
        }
    }
}
