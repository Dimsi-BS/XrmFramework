// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Linq;
using FakeXrmEasy.Extensions;
using Microsoft.Xrm.Sdk.Metadata;
using Model;
using AttributeMetadata = Microsoft.Xrm.Sdk.Metadata.AttributeMetadata;
using AttributeTypeCode = Model.Sdk.AttributeTypeCode;
using DateTimeBehavior = Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior;
using EntityMetadata = Microsoft.Xrm.Sdk.Metadata.EntityMetadata;

namespace Tests.Common.Metadata
{
    public static class DefinitionExtensions
    {
        public static EntityMetadata ToEntityMetadata(this EntityDefinition entityDefinition)
        {

            var metadata = new EntityMetadata
            {
                LogicalName = entityDefinition.EntityName,
                LogicalCollectionName = entityDefinition.EntityCollectionName,
                IsActivity = entityDefinition.PrimaryIdAttributeName == "activityid",
            };

            foreach (var attributeName in entityDefinition.AttributeNames)
            {
                AttributeMetadata attribute = null;

                switch (entityDefinition.GetAttributeType(attributeName))
                {
                    case AttributeTypeCode.Boolean:
                        attribute = new BooleanAttributeMetadata();
                        break;
                    case AttributeTypeCode.Customer:
                    case AttributeTypeCode.Owner:
                    case AttributeTypeCode.Lookup:
                        var lookups = entityDefinition.GetCrmLookupAttributes(attributeName);
                        attribute = new LookupAttributeMetadata
                        {
                            Targets = lookups.Select(l => l.TargetEntityName).Distinct().ToArray()
                        };
                        break;
                    case AttributeTypeCode.DateTime:
                        DateTimeBehavior behavior;
                        switch (entityDefinition.GetDateTimeBehavior(attributeName))
                        {
                            case Model.DateTimeBehavior.UserLocal:
                                behavior = DateTimeBehavior.UserLocal;
                                break;
                            case Model.DateTimeBehavior.DateOnly:
                                behavior = DateTimeBehavior.DateOnly;
                                break;
                            case Model.DateTimeBehavior.TimeZoneIndependent:
                                behavior = DateTimeBehavior.TimeZoneIndependent;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        attribute = new DateTimeAttributeMetadata
                        {
                            DateTimeBehavior = behavior
                        };
                        break;
                    case AttributeTypeCode.Decimal:
                        attribute = new DecimalAttributeMetadata
                        {

                        };
                        break;
                    case AttributeTypeCode.Double:
                        break;
                    case AttributeTypeCode.Integer:
                        break;
                    case AttributeTypeCode.Memo:
                        break;
                    case AttributeTypeCode.Money:
                        break;
                    case AttributeTypeCode.PartyList:
                        break;
                    case AttributeTypeCode.Picklist:
                        break;
                    case AttributeTypeCode.State:
                        break;
                    case AttributeTypeCode.Status:
                        break;
                    case AttributeTypeCode.String:
                        break;
                    case AttributeTypeCode.Uniqueidentifier:
                        break;
                    case AttributeTypeCode.CalendarRules:
                        break;
                    case AttributeTypeCode.Virtual:
                        break;
                    case AttributeTypeCode.BigInt:
                        break;
                    case AttributeTypeCode.ManagedProperty:
                        break;
                    case AttributeTypeCode.EntityName:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (attribute != null)
                {
                    metadata.SetAttribute(attribute);
                }
            }

            return metadata;
        }
    }
}
