// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Deploy;
using Microsoft.Xrm.Sdk.Client;
using XrmFramework;
using AttributeMetadata = Microsoft.Xrm.Sdk.Metadata.AttributeMetadata;
using AttributeTypeCode = Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode;
using DateTimeAttributeMetadata = Microsoft.Xrm.Sdk.Metadata.DateTimeAttributeMetadata;
using DecimalAttributeMetadata = Microsoft.Xrm.Sdk.Metadata.DecimalAttributeMetadata;
using DoubleAttributeMetadata = Microsoft.Xrm.Sdk.Metadata.DoubleAttributeMetadata;
using IntegerAttributeMetadata = Microsoft.Xrm.Sdk.Metadata.IntegerAttributeMetadata;
using MultiSelectPicklistAttributeMetadata = Microsoft.Xrm.Sdk.Metadata.MultiSelectPicklistAttributeMetadata;
using RelationshipAttributeDefinition = DefinitionManager.Definitions.RelationshipAttributeDefinition;
using StringAttributeMetadata = Microsoft.Xrm.Sdk.Metadata.StringAttributeMetadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using XrmFramework.DeployUtils.Configuration;

namespace DefinitionManager
{
    class DataAccessManager : AsyncManager
    {
        private Solution _solution;
        private IOrganizationService _service;

        private DataAccessManager()
        {
        }

        private static DataAccessManager _instance = new DataAccessManager();

        public static DataAccessManager Instance { get { return _instance; } }

        private string Prefix { get; set; }

        public void Connect(Action<object> callback)
        {
            Run(DoConnect, callback, null);
        }
        public void RetrieveEntities(Action<object> callback)
        {
            Run(DoRetrieveEntities, callback, null);
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

            var query = new QueryExpression(Solution.EntityLogicalName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solutionName);
            var linkPublisher = query.AddLink(Deploy.Publisher.EntityLogicalName, "publisherid", "publisherid");
            linkPublisher.EntityAlias = "publisher";
            linkPublisher.Columns.AddColumn("customizationprefix");

            _solution = _service.RetrieveMultiple(query).Entities.Select(s => s.ToEntity<Solution>()).FirstOrDefault();
            return _service;
        }

        object DoRetrieveEntities(object arg)
        {
            SendStepChange("Retrieving entities...");

            var entities = new List<EntityDefinition>();

            var queryComponents = new QueryExpression(SolutionComponent.EntityLogicalName);
            queryComponents.ColumnSet.AllColumns = true;
            queryComponents.Criteria.AddCondition("componenttype", ConditionOperator.Equal, (int)componenttype.Entity);

            var linkSolution = queryComponents.AddLink(Solution.EntityLogicalName, "solutionid", "solutionid");
            linkSolution.EntityAlias = "solution";
            linkSolution.Columns.AddColumn("uniquename");

            var linkPublisher = linkSolution.AddLink(Deploy.Publisher.EntityLogicalName, "publisherid", "publisherid");
            linkPublisher.EntityAlias = "publisher";
            linkPublisher.Columns.AddColumn("customizationprefix");
            queryComponents.Criteria.AddCondition("solutionid", ConditionOperator.Equal, _solution.Id);

            var components = _service.RetrieveMultiple(queryComponents).Entities;

            var max = components.Count;
            var current = 1;

            foreach (var component in components)
            {
                var entity = ((RetrieveEntityResponse)_service.Execute(new RetrieveEntityRequest { MetadataId = component.GetAttributeValue<Guid>("objectid"), EntityFilters = EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships })).EntityMetadata;

                var solutionName = component.GetAttributeValue<Microsoft.Xrm.Sdk.AliasedValue>("solution.uniquename").Value as string;
                Prefix = component.GetAttributeValue<Microsoft.Xrm.Sdk.AliasedValue>("publisher.customizationprefix").Value as string;

                var entityDefinition = new EntityDefinition
                {
                    LogicalName = entity.LogicalName,
                    Name = RemovePrefix(entity.SchemaName).FormatText() + "Definition",
                    IsActivity = entity.IsActivity.Value,
                    LogicalCollectionName = entity.LogicalCollectionName,
                    IsLoaded = true
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
                        keyDefinition.Attributes.Add(new AttributeDefinition { LogicalName = key.LogicalName, Name = key.DisplayName.UserLocalizedLabel.Label.FormatText(), Value = key.LogicalName, Type = "String" });
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
                    }
                }

                SendStepChange(string.Format("({1}/{2}) Retrieved '{0}' entity", entity.LogicalName, current.ToString("00"), max.ToString("00")));
                current++;

                entities.Add(entityDefinition);

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

                    if (attributeMetadata.AttributeType.Value == AttributeTypeCode.Picklist || attributeMetadata.AttributeType.Value == AttributeTypeCode.State || attributeMetadata.AttributeType.Value == AttributeTypeCode.Status || attributeMetadata.AttributeType.Value == AttributeTypeCode.Virtual && attributeMetadata is MultiSelectPicklistAttributeMetadata)
                    {
                        var meta = ((EnumAttributeMetadata)attributeMetadata).OptionSet;

                        var enumLogicalName = meta.IsGlobal.Value ? meta.Name : entity.LogicalName + "_" + attributeMetadata.LogicalName;

                        var tempEnumDefinition = new EnumDefinition
                        {
                            LogicalName = enumLogicalName,
                            IsGlobal = meta.IsGlobal.Value,
                            HasNullValue = attributeMetadata.AttributeType.Value == AttributeTypeCode.Picklist && meta.Options.All(option => option.Value.GetValueOrDefault() != 0)
                        };

                        if (attributeMetadata.AttributeType.Value == AttributeTypeCode.State)
                        {
                            tempEnumDefinition.Name = entityDefinition.Name.Replace("Definition", "") + "State";
                        }
                        else if (attributeMetadata.AttributeType.Value == AttributeTypeCode.Status)
                        {
                            tempEnumDefinition.Name = entityDefinition.Name.Replace("Definition", "") + "Status";
                        }
                        else
                        {
                            tempEnumDefinition.Name = meta.DisplayName.UserLocalizedLabel?.Label.FormatText();
                        }

                        if (string.IsNullOrEmpty(tempEnumDefinition.Name))
                        {
                            continue;
                        }

                        foreach (var option in meta.Options)
                        {
                            if (option.Label.UserLocalizedLabel == null)
                            {
                                continue;
                            }
                            tempEnumDefinition.Values.Add(new EnumValueDefinition
                            {
                                Name = option.Label.UserLocalizedLabel.Label.FormatText(),
                                LogicalName = option.Value.Value.ToString(),
                                DisplayName = option.Label.UserLocalizedLabel.Label,
                                Value = option.Value.Value.ToString(),
                                ExternalValue = option.ExternalValue
                            });
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

                    if (attributeMetadata.AttributeType == AttributeTypeCode.DateTime)
                    {
                        var meta = (DateTimeAttributeMetadata)attributeMetadata;

                        attributeDefinition.DateTimeBehavior = meta.DateTimeBehavior;
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
                            attributeDefinition.KeyNames.Add(keyDefinition.Attributes[key.LogicalName].Name);
                        }
                    }

                    if (attributeDefinition.IsPrimaryIdAttribute || attributeDefinition.IsPrimaryNameAttribute || attributeDefinition.IsPrimaryImageAttribute || attributeDefinition.KeyNames.Any())
                    {
                        attributeDefinition.IsSelected = true;
                    }

                    entityDefinition.AttributesCollection.Add(attributeDefinition);
                }

            }
            SendStepChange(string.Empty);
            return entities;
        }

        object DoRetrieveAttributes(object item)
        {
            var i = item as EntityDefinition;

            SendStepChange(string.Format("Retrieving '{0}' attributes...", i.LogicalName));

            var request = new RetrieveEntityRequest
            {
                LogicalName = i.LogicalName,
                EntityFilters = EntityFilters.Attributes,
                RetrieveAsIfPublished = true
            };

            var response = (RetrieveEntityResponse)_service.Execute(request);

            var list = new List<AttributeDefinition>();

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

        private string RemovePrefix(string name)
        {
            if (!string.IsNullOrEmpty(Prefix) && name.StartsWith(Prefix))
            {
                name = name.Substring(Prefix.Length + 1);
            }
            name = name.Substring(0, 1).ToUpperInvariant() + name.Substring(1);
            return name;
        }
    }
}
