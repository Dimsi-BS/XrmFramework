// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Model.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Model.Utils;

namespace Model
{
    public class EntityDefinition
    {
        public Type DefinitionType { get; private set; }

        public IReadOnlyCollection<AttributeDefinition> Attributes => new ReadOnlyCollection<AttributeDefinition>(_attributes);

        private readonly List<AttributeDefinition> _attributes = new List<AttributeDefinition>();
        
        private readonly IDictionary<string, IList<CrmLookupAttribute>> _crmLookupAttributes = new Dictionary<string, IList<CrmLookupAttribute>>();

        private readonly IDictionary<string, AttributeTypeCode> _attributeTypes = new Dictionary<string, AttributeTypeCode>();

        private readonly IDictionary<string, DateTimeBehavior> _dateTimeBehaviours = new Dictionary<string, DateTimeBehavior>();

        private readonly IDictionary<string, IList<string>> _keyInformations = new Dictionary<string, IList<string>>();

        private readonly IDictionary<string, Relationship> _manyToOneRelationships = new Dictionary<string, Relationship>();

        private readonly IDictionary<string, Relationship> _oneToManyRelationships = new Dictionary<string, Relationship>();

        private readonly IDictionary<string, Relationship> _manyToManyRelationships = new Dictionary<string, Relationship>();

        private readonly IList<string> _attributeNames = new List<string>();

        internal EntityDefinition(Type type)
        {
            DefinitionType = type;

            EntityName = DefinitionType.GetField("EntityName").GetValue(null) as string;

            EntityCollectionName = DefinitionType.GetField("EntityCollectionName").GetValue(null) as string;

            foreach (var field in DefinitionType.GetNestedType("ManyToOneRelationships")?.GetFields() ?? Enumerable.Empty<FieldInfo>())
            {
                var relationshipName = field.GetValue(null) as string;
                if (string.IsNullOrEmpty(relationshipName)) continue;

                var at = field.GetCustomAttribute<RelationshipAttribute>();

                _manyToOneRelationships.Add(relationshipName, new Relationship { PrimaryEntityRole = at.Role, NavigationPropertyName = at.NavigationPropertyName, SchemaName = relationshipName, LookupFieldName = at.LookupFieldName, TargetEntityName = at.TargetEntityName});
            }

            foreach (var field in DefinitionType.GetNestedType("OneToManyRelationships")?.GetFields() ?? Enumerable.Empty<FieldInfo>())
            {
                var relationshipName = field.GetValue(null) as string;
                if (string.IsNullOrEmpty(relationshipName)) continue;

                var at = field.GetCustomAttribute<RelationshipAttribute>();

                _oneToManyRelationships.Add(relationshipName, new Relationship { PrimaryEntityRole = at.Role, NavigationPropertyName = at.NavigationPropertyName, SchemaName = relationshipName, LookupFieldName = at.LookupFieldName, TargetEntityName = at.TargetEntityName });
            }

            foreach (var field in DefinitionType.GetNestedType("ManyToManyRelationships")?.GetFields() ?? Enumerable.Empty<FieldInfo>())
            {
                var relationshipName = field.GetValue(null) as string;
                if (string.IsNullOrEmpty(relationshipName)) continue;

                var at = field.GetCustomAttribute<RelationshipAttribute>();

                _manyToManyRelationships.Add(relationshipName, new Relationship { NavigationPropertyName = at.NavigationPropertyName, SchemaName = relationshipName, LookupFieldName = at.LookupFieldName, TargetEntityName = at.TargetEntityName });
            }

            foreach (var field in DefinitionType.GetNestedType("Columns")?.GetFields() ?? Enumerable.Empty<FieldInfo>())
            {
                var fieldName = field.GetValue(null) as string;
                if (string.IsNullOrEmpty(fieldName)) continue;

                _attributeNames.Add(fieldName);

                _attributeTypes.Add(fieldName, field.GetCustomAttribute<AttributeMetadataAttribute>()?.Type ?? AttributeTypeCode.String);

                if (_attributeTypes[fieldName] == AttributeTypeCode.DateTime)
                {
                    _dateTimeBehaviours.Add(fieldName, field.GetCustomAttribute<DateTimeBehaviorAttribute>()?.Behavior ?? DateTimeBehavior.UserLocal);
                }

                var keyAttributes = field.GetCustomAttributes<KeyAttribute>().Select(k => k.KeyName).ToList();
                if (keyAttributes.Any())
                {
                    _keyInformations.Add(fieldName, keyAttributes);
                }

                var lookupAttributes = field.GetCustomAttributes<CrmLookupAttribute>().ToList();

                if (lookupAttributes.Any())
                {
                    _crmLookupAttributes.Add(fieldName, lookupAttributes);
                }

                var primaryAttribute = field.GetCustomAttribute<PrimaryAttributeAttribute>();
                if (primaryAttribute != null)
                {
                    switch (primaryAttribute.Type)
                    {
                        case PrimaryAttributeType.Id:
                            PrimaryIdAttributeName = fieldName;
                            break;
                        case PrimaryAttributeType.Name:
                            PrimaryNameAttributeName = fieldName;
                            break;
                        case PrimaryAttributeType.Image:
                            PrimaryImageAttributeName = fieldName;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public IReadOnlyCollection<string> AttributeNames => new ReadOnlyCollection<string>(_attributeNames);

        public string EntityName { get; }
        public string EntityCollectionName { get; }

        public IEnumerable<CrmLookupAttribute> GetCrmLookupAttributes(string attributeName)
        {
            if (!_crmLookupAttributes.ContainsKey(attributeName))
            {
                throw new Exception($"The attribut {attributeName} is not a lookup attribute.");
            }

            return _crmLookupAttributes[attributeName];
        }

        public string PrimaryIdAttributeName { get; }

        public string PrimaryNameAttributeName { get; }

        public string PrimaryImageAttributeName { get; }

        public bool IsFullyValid => !string.IsNullOrEmpty(PrimaryIdAttributeName) && !string.IsNullOrEmpty(PrimaryNameAttributeName) && _attributeTypes.Any();
        
        public AttributeTypeCode GetAttributeType(string attributeName)
        {
            if (!_attributeTypes.ContainsKey(attributeName))
            {
                throw new Exception($"The attribut {attributeName} is not present in the definition.");
            }

            return _attributeTypes[attributeName];
        }

        public DateTimeBehavior GetDateTimeBehavior(string attributeName)
        {
            if (!_dateTimeBehaviours.ContainsKey(attributeName))
            {
                throw new Exception($"The attribut {attributeName} is not a datetime attribute.");
            }

            return _dateTimeBehaviours[attributeName];
        }

        public bool IsPrimaryAttribute(string attributeName, PrimaryAttributeType type)
        {
            switch (type)
            {
                case PrimaryAttributeType.Id:
                    return PrimaryIdAttributeName == attributeName;
                case PrimaryAttributeType.Name:
                    return PrimaryNameAttributeName == attributeName;
                case PrimaryAttributeType.Image:
                    return PrimaryImageAttributeName == attributeName;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public IEnumerable<Model.Sdk.Relationship> GetRelationshipsFromAttributeName(string attributeName)
        {
            return _manyToOneRelationships.Values.Where(r => r.LookupFieldName == attributeName);
        }

        //public Relationship GetRelationshipFromAttributeName(string attributeName)
        //{
        //    return GetRelationshipsFromAttributeName(attributeName).FirstOrDefault();
        //}

        public EntityDefinition GetSubDefinition(string attributeName, string entityName = null)
        {
            var lookupAttributes = GetCrmLookupAttributes(attributeName);

            var lookupAttribute = lookupAttributes.FirstOrDefault(l => string.IsNullOrEmpty(entityName) || l.TargetEntityName == entityName);
            if (lookupAttribute == null)
            {
                throw new KeyNotFoundException($"Attribute {attributeName} is not a lookup to the {entityName} entity.");
            }

            return DefinitionCache.GetEntityDefinition(lookupAttribute.TargetEntityName);
        }

        public KeyInfos GetKeyInfos()
        {
            var keyInfos = new KeyInfos();
            foreach (var fieldName in _keyInformations.Keys)
            {
                foreach (var keyName in _keyInformations[fieldName])
                {
                    keyInfos.AddKeyColumn(keyName, fieldName);
                }
            }
            return keyInfos;
        }

        public bool IsLookupAttribute(string attributeName)
        {
            var attributeType = GetAttributeType(attributeName);
            return attributeType == AttributeTypeCode.Lookup || attributeType == AttributeTypeCode.Owner || attributeType == AttributeTypeCode.Customer;
        }

        public bool IsKey(string attributeName)
        {
            return _keyInformations.ContainsKey(attributeName);
        }

        public string GetPrimaryAttributeName(PrimaryAttributeType primaryAttributeType)
        {
            switch (primaryAttributeType)
            {
                case PrimaryAttributeType.Id:
                    return PrimaryIdAttributeName;
                case PrimaryAttributeType.Name:
                    return PrimaryNameAttributeName;
                case PrimaryAttributeType.Image:
                    return PrimaryImageAttributeName;
                default:
                    throw new ArgumentOutOfRangeException(nameof(primaryAttributeType), primaryAttributeType, null);
            }
        }

        public Relationship GetRelationshipByAttributeNameAndTargetEntityName(string attributeName, string targetEntityName)
        {
            return _manyToOneRelationships.Values.FirstOrDefault(r => r.LookupFieldName == attributeName && r.TargetEntityName == targetEntityName);
        }

        public string GetWebApiAttributeName(string attributeName)
        {
            var attributeType = GetAttributeType(attributeName);
            switch (attributeType)
            {
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.Customer:
                    attributeName = $"_{attributeName}_value";
                    break;
            }

            return attributeName;
        }
    }

    public class KeyInfos
    {
        private Dictionary<string, List<Column>> _keys = new Dictionary<string, List<Column>>();

        public void AddKeyColumn(string keyName, string columnName)
        {
            if (!_keys.ContainsKey(keyName))
            {
                _keys[keyName] = new List<Column>();
            }

            _keys[keyName].Add(new Column(columnName));
        }

        public void Clear()
        {
            foreach (var keyColumns in _keys.Values)
            {
                foreach (var column in keyColumns)
                {
                    column.Checked = false;
                }
            }
        }

        public void CheckColumn(string columnName)
        {
            foreach (var keyColumns in _keys.Values)
            {
                foreach (var column in keyColumns.Where(k => k.Name == columnName))
                {
                    column.Checked = true;
                }
            }
        }

        public IList<string> GetKeyColumns(string keyName)
        {
            if (!_keys.ContainsKey(keyName))
            {
                throw new Exception(string.Format("No Key exists with the name {0}", keyName));
            }

            return _keys[keyName].Select(k => k.Name).ToList();
        }

        public IList<string> CheckedKeys
        {
            get
            {
                return _keys.Where(k => k.Value.All(c => c.Checked)).Select(k => k.Key).ToList();
            }
        }

        private class Column
        {
            public Column(string name)
            {
                Name = name;
            }
            public string Name { get; set; }
            public bool Checked { get; set; }
        }
    }

    public enum DateTimeBehavior
    {
        UserLocal = 0,
        DateOnly = 1,
        TimeZoneIndependent = 2
    }
}
