// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Deploy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XrmFramework.Core;
using XrmFramework.DefinitionManager;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Configuration;
using AttributeMetadata = Microsoft.Xrm.Sdk.Metadata.AttributeMetadata;
using AttributeTypeCode = Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode;
using DateTimeAttributeMetadata = Microsoft.Xrm.Sdk.Metadata.DateTimeAttributeMetadata;
using DateTimeBehavior = Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior;
using DecimalAttributeMetadata = Microsoft.Xrm.Sdk.Metadata.DecimalAttributeMetadata;
using DoubleAttributeMetadata = Microsoft.Xrm.Sdk.Metadata.DoubleAttributeMetadata;
using EntityRole = XrmFramework.EntityRole;
using IntegerAttributeMetadata = Microsoft.Xrm.Sdk.Metadata.IntegerAttributeMetadata;
using LocalizedLabel = XrmFramework.Core.LocalizedLabel;
using MultiSelectPicklistAttributeMetadata = Microsoft.Xrm.Sdk.Metadata.MultiSelectPicklistAttributeMetadata;
using RelationshipAttributeDefinition = DefinitionManager.Definitions.RelationshipAttributeDefinition;
using StringAttributeMetadata = Microsoft.Xrm.Sdk.Metadata.StringAttributeMetadata;
using Table = XrmFramework.Core.Table;

namespace DefinitionManager
{
    class DataAccessManager : AsyncManager
    {
        private Solution _solution;
        private IOrganizationService _service;
        private List<string> PublisherPrefixes { get; } = new();

        private DataAccessManager()
        {
        }

        private static DataAccessManager _instance = new();

        public static DataAccessManager Instance { get { return _instance; } }

        private string Prefix { get; set; }

        public void Connect(Action<object> callback)
        {
            Run(DoConnect, callback, null);
        }
        public void RetrieveEntities(Action<object> callback, params string[] entityNames)
        {
            Run(DoRetrieveEntities, callback, entityNames);
        }

        internal void RetrieveAttributes(EntityDefinition item, Action<object> callback)
        {
            Run(DoRetrieveAttributes, callback, item);
        }

        object DoConnect(object arg)
        {
            SendStepChange("Connecting...");

            _service = new CrmServiceClient(ConfigHelper.GetSelectedConnectionString());

            _service.Execute(new WhoAmIRequest());
            SendStepChange("Connected!");

            var solutionName = ConfigHelper.GetEntitiesSolutionUniqueName();

            var query = new QueryExpression(SolutionDefinition.EntityName);
            query.ColumnSet.AllColumns = true;
            var linkPublisher = query.AddLink(PublisherDefinition.EntityName, PublisherDefinition.Columns.Id, PublisherDefinition.Columns.Id);
            linkPublisher.EntityAlias = PublisherDefinition.EntityName;
            linkPublisher.Columns.AddColumn(PublisherDefinition.Columns.CustomizationPrefix);

            var solutions = _service.RetrieveMultiple(query).Entities.Select(s => s.ToEntity<Solution>());

            PublisherPrefixes.AddRange(solutions.Select(s => s.GetAttributeValue<AliasedValue>($"{PublisherDefinition.EntityName}.{PublisherDefinition.Columns.CustomizationPrefix}").Value as string).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());

            _solution = solutions.FirstOrDefault(s => string.Compare(s.GetAttributeValue<string>(SolutionDefinition.Columns.UniqueName), solutionName, true) == 0);
            return _service;
        }

        object DoRetrieveEntities(object arg)
        {
            var entities = new List<EntityDefinition>();
            var newEntities = new List<Table>();
            var enums = new List<OptionSetEnum>();

            SendStepChange("Retrieving entities...");

            var req = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships
            };

            var sw = Stopwatch.StartNew();
            var response = (RetrieveAllEntitiesResponse)_service.Execute(req);
            sw.Stop();

            var entitiesMetadata = response.EntityMetadata;

            SendStepChange($"Metadata retrieved in {sw.Elapsed}");

            var queryPublishers = new QueryExpression(PublisherDefinition.EntityName);
            queryPublishers.ColumnSet.AddColumn(PublisherDefinition.Columns.CustomizationPrefix);

            var publisherPrefixes = _service.RetrieveMultiple(queryPublishers).Entities.Select(e => e.GetAttributeValue<string>(PublisherDefinition.Columns.CustomizationPrefix)).ToList();

            foreach (var entity in entitiesMetadata)
            {
                var entityDefinition = new EntityDefinition
                {
                    LogicalName = entity.LogicalName,
                    Name = RemovePrefix(entity.SchemaName).FormatText() + "Definition",
                    IsActivity = entity.IsActivity.Value,
                    LogicalCollectionName = entity.LogicalCollectionName,
                    IsLoaded = true
                };

                var newEntity = new Table
                {
                    LogicalName = entity.LogicalName,
                    CollectionName = entity.LogicalCollectionName,
                    Name = RemovePrefix(entity.SchemaName).FormatText()
                };

                ClassDefinition keyDefinition = null;
                if (entity.Keys != null && entity.Keys.Any())
                {
                    keyDefinition = new ClassDefinition
                    {
                        LogicalName = "AlternateKeyNames",
                        Name = "AlternateKeyNames"
                    };
                    entityDefinition.AdditionalClassesCollection.Add(keyDefinition);
                    foreach (var key in entity.Keys)
                    {
                        keyDefinition.Attributes.Add(new AttributeDefinition { LogicalName = key.LogicalName.Trim('"'), Name = key.DisplayName.UserLocalizedLabel.Label.FormatText(), Value = key.LogicalName.Trim('"'), Type = "String" });

                        var newKey = new Key
                        {
                            LogicalName = key.LogicalName,
                            Name = key.DisplayName.UserLocalizedLabel.Label.FormatText()

                        };
                        newKey.FieldNames.AddRange(key.KeyAttributes);



                        newEntity.Keys.Add(newKey);
                    }
                }

                if (entity.OneToManyRelationships.Any())
                {
                    var classDefinition = new ClassDefinition
                    {
                        LogicalName = "OneToManyRelationships",
                        Name = "OneToManyRelationships"
                    };
                    entityDefinition.AdditionalClassesCollection.Add(classDefinition);
                    foreach (var relationship in entity.OneToManyRelationships)
                    {
                        var attribute = new RelationshipAttributeDefinition
                        {
                            LogicalName = relationship.SchemaName,
                            Name = relationship.SchemaName,
                            Value = relationship.SchemaName,
                            Type = "String",
                            Role = "Referenced",
                            NavigationPropertyName = relationship.ReferencedEntityNavigationPropertyName,
                            TargetEntityName = relationship.ReferencingEntity,
                            LookupFieldName = relationship.ReferencingAttribute
                        };

                        classDefinition.Attributes.Add(attribute);

                        newEntity.OneToManyRelationships.Add(new Relation
                        {
                            Name = relationship.SchemaName,
                            Role = EntityRole.Referenced,
                            EntityName = relationship.ReferencingEntity,
                            NavigationPropertyName = relationship.ReferencedEntityNavigationPropertyName,
                            LookupFieldName = relationship.ReferencingAttribute
                        });
                    }
                }

                if (entity.ManyToManyRelationships.Any())
                {
                    var classDefinition = new ClassDefinition
                    {
                        LogicalName = "ManyToManyRelationships",
                        Name = "ManyToManyRelationships"
                    };
                    entityDefinition.AdditionalClassesCollection.Add(classDefinition);
                    foreach (var relationship in entity.ManyToManyRelationships)
                    {
                        classDefinition.Attributes.Add(new RelationshipAttributeDefinition
                        {
                            LogicalName = relationship.SchemaName,
                            Name = relationship.SchemaName,
                            Value = relationship.SchemaName,
                            Type = "String",
                            Role = "Referencing",
                            NavigationPropertyName = relationship.IntersectEntityName,
                            TargetEntityName = relationship.Entity1LogicalName == entityDefinition.LogicalName ? relationship.Entity2LogicalName : relationship.Entity1LogicalName,
                            LookupFieldName = relationship.Entity1LogicalName == entityDefinition.LogicalName ? relationship.Entity2IntersectAttribute : relationship.Entity1IntersectAttribute
                        });

                        newEntity.ManyToManyRelationships.Add(new Relation
                        {
                            Name = relationship.SchemaName,
                            Role = EntityRole.Referencing,
                            EntityName = relationship.Entity1LogicalName == entityDefinition.LogicalName ? relationship.Entity2LogicalName : relationship.Entity1LogicalName,
                            NavigationPropertyName = relationship.IntersectEntityName,
                            LookupFieldName = relationship.Entity1LogicalName == entityDefinition.LogicalName ? relationship.Entity2IntersectAttribute : relationship.Entity1IntersectAttribute
                        });
                    }
                }

                
                var lookupFields = new Dictionary<string, List<OneToManyRelationshipMetadata>>();
                if (entity.ManyToOneRelationships.Any())
                {
                    ClassDefinition classDefinition = new ClassDefinition
                    {
                        LogicalName = "ManyToOneRelationships",
                        Name = "ManyToOneRelationships"
                    };
                    entityDefinition.AdditionalClassesCollection.Add(classDefinition);
                    foreach (var relationship in entity.ManyToOneRelationships)
                    {
                        var attribute = new RelationshipAttributeDefinition
                        {
                            LogicalName = relationship.SchemaName,
                            Name = relationship.SchemaName,
                            Value = relationship.SchemaName,
                            Type = "String",
                            Role = "Referencing",
                            NavigationPropertyName = relationship.ReferencingEntityNavigationPropertyName,
                            TargetEntityName = relationship.ReferencedEntity,
                            LookupFieldName = relationship.ReferencingAttribute
                        };

                        classDefinition.Attributes.Add(attribute);

                        List<OneToManyRelationshipMetadata> list;

                        if (lookupFields.ContainsKey(relationship.ReferencingAttribute))
                        {
                            list = lookupFields[relationship.ReferencingAttribute];
                        }
                        else
                        {
                            list = new List<OneToManyRelationshipMetadata>();
                            lookupFields.Add(relationship.ReferencingAttribute, list);
                        }

                        list.Add(relationship);

                        newEntity.ManyToOneRelationships.Add(new Relation
                        {
                            Name = relationship.SchemaName,
                            Role = EntityRole.Referencing,
                            NavigationPropertyName = relationship.ReferencingEntityNavigationPropertyName,
                            EntityName = relationship.ReferencedEntity,
                            LookupFieldName = relationship.ReferencingAttribute
                        });
                    }
                }

                // bar 2
                entities.Add(entityDefinition);
                newEntities.Add(newEntity);

                foreach (AttributeMetadata attributeMetadata in entity.Attributes.OrderBy(a => a.LogicalName))
                {
                    if (!attributeMetadata.IsValidForCreate.Value && !attributeMetadata.IsValidForRead.Value && !attributeMetadata.IsValidForUpdate.Value)
                    {
                        continue;
                    }
                    if (attributeMetadata.AttributeType.Value == AttributeTypeCode.EntityName || !string.IsNullOrEmpty(attributeMetadata.AttributeOf))
                    {
                        continue;
                    }

                    EnumDefinition enumDefinition = null;
                    string attributeEnumName = null;

                    if (attributeMetadata.AttributeType.Value == AttributeTypeCode.Picklist || attributeMetadata.AttributeType.Value == AttributeTypeCode.State || attributeMetadata.AttributeType.Value == AttributeTypeCode.Status || attributeMetadata.AttributeType.Value == AttributeTypeCode.Virtual && attributeMetadata is MultiSelectPicklistAttributeMetadata)
                    {
                        var meta = ((EnumAttributeMetadata)attributeMetadata).OptionSet;

                        var enumLogicalName = meta.IsGlobal.Value ? meta.Name : entity.LogicalName + "|" + attributeMetadata.LogicalName;

                        attributeEnumName = enumLogicalName;

                        var tempEnumDefinition = new EnumDefinition
                        {
                            LogicalName = enumLogicalName,
                            IsGlobal = meta.IsGlobal.Value,
                            HasNullValue = attributeMetadata.AttributeType.Value == AttributeTypeCode.Picklist && meta.Options.All(option => option.Value.GetValueOrDefault() != 0)
                        };

                        var newEnum = new OptionSetEnum
                        {
                            LogicalName = enumLogicalName,
                            IsGlobal = meta.IsGlobal.Value,
                            HasNullValue = attributeMetadata.AttributeType.Value == AttributeTypeCode.Picklist &&
                                           meta.Options.All(option => option.Value.GetValueOrDefault() != 0),
                        };

                        if (attributeMetadata.AttributeType.Value == AttributeTypeCode.State)
                        {
                            tempEnumDefinition.Name = entityDefinition.Name.Replace("Definition", "") + "State";

                            newEnum.Name = entityDefinition.Name.Replace("Definition", "") + "State";
                        }
                        else if (attributeMetadata.AttributeType.Value == AttributeTypeCode.Status)
                        {
                            tempEnumDefinition.Name = entityDefinition.Name.Replace("Definition", "") + "Status";
                            newEnum.Name = entityDefinition.Name.Replace("Definition", "") + "Status";
                        }
                        else
                        {
                            tempEnumDefinition.Name = meta.DisplayName.UserLocalizedLabel?.Label.FormatText();
                            newEnum.Name = meta.DisplayName.UserLocalizedLabel?.Label.FormatText();
                        }

                        if (string.IsNullOrEmpty(newEnum.Name))
                        {
                            continue;
                        }

                        foreach (var option in meta.Options)
                        {
                            if (option.Label.UserLocalizedLabel == null)
                            {
                                continue;
                            }

                            var optionValue = new OptionSetEnumValue
                            {
                                Name = option.Label.UserLocalizedLabel.Label.FormatText(),
                                Value = option.Value.Value,
                                ExternalValue = option.ExternalValue
                            };

                            foreach (var displayNameLocalizedLabel in option.Label.LocalizedLabels)
                            {
                                optionValue.Labels.Add(new LocalizedLabel
                                {
                                    Label = displayNameLocalizedLabel.Label,
                                    LangId = displayNameLocalizedLabel.LanguageCode
                                });
                            }

                            newEnum.Values.Add(optionValue);

                            tempEnumDefinition.Values.Add(new EnumValueDefinition
                            {
                                Name = option.Label.UserLocalizedLabel.Label.FormatText(),
                                LogicalName = option.Value.Value.ToString(),
                                DisplayName = option.Label.UserLocalizedLabel.Label,
                                Value = option.Value.Value.ToString(),
                                ExternalValue = option.ExternalValue
                            });
                        }

                        if (newEnum.IsGlobal == false)
                        {
                            newEntity.Enums.Add(newEnum);
                        }
                        else if (enums.All(e => e.LogicalName != newEnum.LogicalName))
                        {
                            enums.Add(newEnum);
                        }

                        if (!EnumDefinitionCollection.Instance.Contains(enumLogicalName))
                        {
                            enumDefinition = tempEnumDefinition;
                            EnumDefinitionCollection.Instance.Add(enumDefinition);
                        }
                        else
                        {
                            enumDefinition = EnumDefinitionCollection.Instance[enumLogicalName];
                            enumDefinition.Merge(tempEnumDefinition);
                        }
                    }

                    var name = RemovePrefix(attributeMetadata.SchemaName);

                    if (attributeMetadata.LogicalName == entity.PrimaryIdAttribute)
                    {
                        name = "Id";
                    }

                    int? maxLength = null;
                    double? minRangeDouble = null, maxRangeDouble = null;

                    switch (attributeMetadata.AttributeType.Value)
                    {
                        case AttributeTypeCode.String:
                            maxLength = ((StringAttributeMetadata)attributeMetadata).MaxLength;
                            break;
                        case AttributeTypeCode.Memo:
                            maxLength = ((MemoAttributeMetadata)attributeMetadata).MaxLength;
                            break;
                        case AttributeTypeCode.Money:
                            var m = (MoneyAttributeMetadata)attributeMetadata;
                            minRangeDouble = m.MinValue;
                            maxRangeDouble = m.MaxValue;
                            break;
                        case AttributeTypeCode.Integer:
                            var mi = (IntegerAttributeMetadata)attributeMetadata;
                            minRangeDouble = mi.MinValue;
                            maxRangeDouble = mi.MaxValue;
                            break;
                        case AttributeTypeCode.Double:
                            var md = (DoubleAttributeMetadata)attributeMetadata;
                            minRangeDouble = md.MinValue;
                            maxRangeDouble = md.MaxValue;
                            break;
                        case AttributeTypeCode.Decimal:
                            var mde = (DecimalAttributeMetadata)attributeMetadata;
                            minRangeDouble = (double?)mde.MinValue;
                            maxRangeDouble = (double?)mde.MaxValue;
                            break;
                    }

                    var attributeDefinition = new AttributeDefinition
                    {
                        LogicalName = attributeMetadata.LogicalName,
                        Name = name,
                        DisplayName = attributeMetadata.DisplayName.UserLocalizedLabel == null ? attributeMetadata.SchemaName : attributeMetadata.DisplayName.UserLocalizedLabel.Label,
                        IsValidForAdvancedFind = attributeMetadata.IsValidForAdvancedFind.Value,
                        IsValidForCreate = attributeMetadata.IsValidForCreate.Value,
                        IsValidForRead = attributeMetadata.IsValidForRead.Value,
                        IsValidForUpdate = attributeMetadata.IsValidForUpdate.Value,
                        Type = attributeMetadata.AttributeType.Value == AttributeTypeCode.Virtual && attributeMetadata is MultiSelectPicklistAttributeMetadata
                            ? "MultiSelectPicklist" : attributeMetadata.AttributeType.Value.ToString(),
                        Enum = enumDefinition,
                        ParentEntity = entityDefinition,
                        IsPrimaryIdAttribute = attributeMetadata.LogicalName == entity.PrimaryIdAttribute,
                        IsPrimaryNameAttribute = attributeMetadata.LogicalName == entity.PrimaryNameAttribute,
                        IsPrimaryImageAttribute = attributeMetadata.LogicalName == entity.PrimaryImageAttribute,
                        Relationships = lookupFields.ContainsKey(attributeMetadata.LogicalName) ? lookupFields[attributeMetadata.LogicalName] : null,
                        StringMaxLength = maxLength,
                        MinRange = minRangeDouble,
                        MaxRange = maxRangeDouble
                    };

                    var attribute = new Column
                    {
                        LogicalName = attributeMetadata.LogicalName,
                        Name = name,
                        Type = (XrmFramework.AttributeTypeCode)(int)(attributeMetadata.AttributeType.Value == AttributeTypeCode.Virtual && attributeMetadata is MultiSelectPicklistAttributeMetadata
                            ? AttributeTypeCode.Picklist : attributeMetadata.AttributeType.Value),
                        IsMultiSelect = attributeMetadata.AttributeType.Value == AttributeTypeCode.Virtual && attributeMetadata is MultiSelectPicklistAttributeMetadata,
                        PrimaryType = attributeMetadata.LogicalName == entity.PrimaryIdAttribute ?
                            PrimaryType.Id :
                            attributeMetadata.LogicalName == entity.PrimaryNameAttribute ?
                                PrimaryType.Name :
                                attributeMetadata.LogicalName == entity.PrimaryImageAttribute ? PrimaryType.Image : PrimaryType.None,
                        StringLength = maxLength,
                        MinRange = minRangeDouble,
                        MaxRange = maxRangeDouble,
                        EnumName = attributeEnumName
                    };

                    foreach (var displayNameLocalizedLabel in attributeMetadata.DisplayName.LocalizedLabels)
                    {
                        attribute.Labels.Add(new LocalizedLabel
                        {
                            Label = displayNameLocalizedLabel.Label,
                            LangId = displayNameLocalizedLabel.LanguageCode
                        });
                    }

                    if (attributeMetadata.IsValidForAdvancedFind.Value)
                    {
                        attribute.Capabilities |= AttributeCapabilities.AdvancedFind;
                    }

                    if (attributeMetadata.IsValidForCreate.Value)
                    {
                        attribute.Capabilities |= AttributeCapabilities.Create;
                    }

                    if (attributeMetadata.IsValidForRead.Value)
                    {
                        attribute.Capabilities |= AttributeCapabilities.Read;
                    }

                    if (attributeMetadata.IsValidForUpdate.Value)
                    {
                        attribute.Capabilities |= AttributeCapabilities.Update;
                    }


                    if (attributeMetadata.AttributeType == AttributeTypeCode.DateTime)
                    {
                        var meta = (DateTimeAttributeMetadata)attributeMetadata;

                        attributeDefinition.DateTimeBehavior = meta.DateTimeBehavior;

                        attribute.DateTimeBehavior = meta.DateTimeBehavior.ToDateTimeBehavior();
                    }

                    if (attributeMetadata.LogicalName == "ownerid")
                    {
                        attributeDefinition.Relationships.Clear();
                        if (lookupFields.ContainsKey("owninguser"))
                        {
                            attributeDefinition.Relationships.Add(new OneToManyRelationshipMetadata
                            {
                                ReferencingAttribute = "ownerid",
                                ReferencedAttribute = "systemuserid",
                                ReferencedEntity = "systemuser",
                                ReferencingEntity = entityDefinition.LogicalName,
                                SchemaName = lookupFields["owninguser"].First().SchemaName
                            });
                        }
                        if (lookupFields.ContainsKey("owningteam"))
                        {
                            attributeDefinition.Relationships.Add(new OneToManyRelationshipMetadata
                            {
                                ReferencingAttribute = "ownerid",
                                ReferencedAttribute = "teamid",
                                ReferencedEntity = "team",
                                ReferencingEntity = entityDefinition.LogicalName,
                                SchemaName = lookupFields["owningteam"].First().SchemaName
                            });
                        }
                    }

                    if (keyDefinition != null)
                    {
                        foreach (var key in entity.Keys.Where(k => k.KeyAttributes.Contains(attributeDefinition.LogicalName)))
                        {
                            attributeDefinition.KeyNames.Add(keyDefinition.Attributes[key.LogicalName.Trim('"')].Name);
                        }
                    }

                    if (attributeDefinition.IsPrimaryIdAttribute || attributeDefinition.IsPrimaryNameAttribute || attributeDefinition.IsPrimaryImageAttribute || attributeDefinition.KeyNames.Any())
                    {
                        attributeDefinition.IsSelected = true;
                    }

                    entityDefinition.AttributesCollection.Add(attributeDefinition);
                    newEntity.Columns.Add(attribute);
                }
            }
            SendStepChange(string.Empty);
            return new Tuple<List<EntityDefinition>, List<Table>, List<OptionSetEnum>>(entities, newEntities, enums);
        }

        object DoRetrieveAttributes(object item)
        {
            var list = new List<AttributeDefinition>();
            return list;
            var i = item as EntityDefinition;

            SendStepChange($"Retrieving '{i.LogicalName}' attributes...");

            var request = new RetrieveEntityRequest
            {
                LogicalName = i.LogicalName,
                EntityFilters = EntityFilters.Attributes,
                RetrieveAsIfPublished = true
            };

            var response = (RetrieveEntityResponse)_service.Execute(request);

            

            foreach (AttributeMetadata attributeMetadata in response.EntityMetadata.Attributes.OrderBy(a => a.LogicalName))
            {
                if (!attributeMetadata.IsValidForCreate.Value && !attributeMetadata.IsValidForRead.Value && !attributeMetadata.IsValidForUpdate.Value)
                {
                    continue;
                }
                if (attributeMetadata.IsLogical.Value || attributeMetadata.AttributeType.Value == AttributeTypeCode.EntityName || !string.IsNullOrEmpty(attributeMetadata.AttributeOf))
                {
                    continue;
                }

                EnumDefinition enumDefinition = null;

                if (attributeMetadata.AttributeType.Value == AttributeTypeCode.Picklist || attributeMetadata.AttributeType.Value == AttributeTypeCode.State || attributeMetadata.AttributeType.Value == AttributeTypeCode.Status || attributeMetadata.AttributeType == AttributeTypeCode.Virtual && attributeMetadata is MultiSelectPicklistAttributeMetadata)
                {
                    var meta = ((EnumAttributeMetadata)attributeMetadata).OptionSet;


                    var tempEnumDefinition = new EnumDefinition
                    {
                        LogicalName = meta.Name,
                        IsGlobal = meta.IsGlobal.Value
                    };

                    if (attributeMetadata.AttributeType.Value == AttributeTypeCode.State)
                    {
                        tempEnumDefinition.Name = i.Name.Replace("Definition", "") + "State";
                    }
                    else if (attributeMetadata.AttributeType.Value == AttributeTypeCode.Status)
                    {
                        tempEnumDefinition.Name = i.Name.Replace("Definition", "") + "Status";
                    }
                    else
                    {
                        tempEnumDefinition.Name = meta.DisplayName.UserLocalizedLabel.Label.FormatText();
                    }

                    foreach (var option in meta.Options)
                    {
                        tempEnumDefinition.Values.Add(new EnumValueDefinition
                        {
                            Name = option.Label.UserLocalizedLabel.Label.FormatText(),
                            LogicalName = option.Value.Value.ToString(),
                            DisplayName = option.Label.UserLocalizedLabel.Label,
                            Value = option.Value.Value.ToString(),
                            ExternalValue = option.ExternalValue
                        });
                    }

                    if (!EnumDefinitionCollection.Instance.Definitions.Any(d => d.LogicalName == meta.Name))
                    {
                        enumDefinition = tempEnumDefinition;
                        EnumDefinitionCollection.Instance.Add(enumDefinition);
                    }
                    else
                    {
                        enumDefinition = EnumDefinitionCollection.Instance.Definitions.First(d => d.LogicalName == meta.Name);
                        enumDefinition.Merge(tempEnumDefinition);
                    }
                }

                int? maxLength = null;
                double? minRangeDouble = null, maxRangeDouble = null;

                switch (attributeMetadata.AttributeType.Value)
                {
                    case AttributeTypeCode.String:
                        maxLength = ((StringAttributeMetadata)attributeMetadata).MaxLength;
                        break;
                    case AttributeTypeCode.Memo:
                        maxLength = ((MemoAttributeMetadata)attributeMetadata).MaxLength;
                        break;
                    case AttributeTypeCode.Money:
                        var m = (MoneyAttributeMetadata)attributeMetadata;
                        minRangeDouble = m.MinValue;
                        maxRangeDouble = m.MaxValue;
                        break;
                    case AttributeTypeCode.Integer:
                        var mi = (IntegerAttributeMetadata)attributeMetadata;
                        minRangeDouble = mi.MinValue;
                        maxRangeDouble = mi.MaxValue;
                        break;
                }

                list.Add(new AttributeDefinition
                {
                    LogicalName = attributeMetadata.LogicalName,
                    Name = RemovePrefix(attributeMetadata.SchemaName),
                    DisplayName = attributeMetadata.DisplayName.UserLocalizedLabel == null ? attributeMetadata.SchemaName : attributeMetadata.DisplayName.UserLocalizedLabel.Label,
                    IsValidForAdvancedFind = attributeMetadata.IsValidForAdvancedFind.Value,
                    IsValidForCreate = attributeMetadata.IsValidForCreate.Value,
                    IsValidForRead = attributeMetadata.IsValidForRead.Value,
                    IsValidForUpdate = attributeMetadata.IsValidForUpdate.Value,
                    Type = attributeMetadata.AttributeType.Value == AttributeTypeCode.Virtual && attributeMetadata is MultiSelectPicklistAttributeMetadata
                        ? "MultiSelectPicklist" : attributeMetadata.AttributeType.Value.ToString(),
                    StringMaxLength = maxLength,
                    MinRange = minRangeDouble,
                    MaxRange = maxRangeDouble,
                    Enum = enumDefinition
                });
            }

            SendStepChange(string.Empty);
            return list;
        }



        public string RemovePrefix(string name)
        {
            foreach (var prefix in PublisherPrefixes)
            {
                if (!string.IsNullOrEmpty(prefix) && name.StartsWith(prefix))
                {
                    name = name.Substring(prefix.Length + 1);
                }
            }
            name = name.Substring(0, 1).ToUpperInvariant() + name.Substring(1);
            return name;
        }
    }

    public static class DateTimeBehaviorExtensions
    {
        public static XrmFramework.DateTimeBehavior ToDateTimeBehavior(this DateTimeBehavior behav)
        {
            if (behav == DateTimeBehavior.DateOnly)
            {
                return XrmFramework.DateTimeBehavior.DateOnly;
            }

            if (behav == DateTimeBehavior.TimeZoneIndependent)
            {
                return XrmFramework.DateTimeBehavior.TimeZoneIndependent;
            }

            return XrmFramework.DateTimeBehavior.UserLocal;
        }
    }
}
