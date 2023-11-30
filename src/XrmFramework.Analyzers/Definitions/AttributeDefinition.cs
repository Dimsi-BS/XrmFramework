using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Model;
using Model.Sdk;
using Model.Sdk.Metadata;

namespace XrmFramework.Analyzers.Utils
{
    [CLSCompliant(false)]
    public class AttributeDefinition : Definition<IFieldSymbol>
    {
        public EntityDefinition ContainingDefinition { get; }

        public string LogicalName { get; }

        public string FullName => $"{ContainingDefinition.Name}.Columns.{Name}";

        public AttributeTypeCode Type { get; }

        private readonly PrimaryAttributeType? _primaryType;

        public bool IsPrimaryId => _primaryType == PrimaryAttributeType.Id;

        public bool IsPrimaryName => _primaryType == PrimaryAttributeType.Name;

        public bool IsPrimaryImage => _primaryType == PrimaryAttributeType.Image;

        public List<RelationshipMetadata> Relationships { get; } = new List<RelationshipMetadata>();

        internal AttributeDefinition(EntityDefinition containingDefinition, IFieldSymbol attributeSymbol) : base(attributeSymbol)
        {
            ContainingDefinition = containingDefinition;
            LogicalName = (string)attributeSymbol.ConstantValue;

            var attributes = attributeSymbol.GetAttributes();

            var metadata = attributes.Single(a => a.AttributeClass?.Name == "AttributeMetadataAttribute").ConstructorArguments.Single();
            Type = (AttributeTypeCode) Enum.ToObject(typeof(AttributeTypeCode), metadata.Value);

            if (attributes.Any(a => a.AttributeClass?.Name == "PrimaryAttributeAttribute"))
            {
                var attribute = attributes.FirstOrDefault(a => a.AttributeClass?.Name == "PrimaryAttributeAttribute");

                if (attribute != null)
                {
                    metadata = attribute.ConstructorArguments.Single();
                    _primaryType = metadata.IsNull ? null : (PrimaryAttributeType?)Enum.ToObject(typeof(PrimaryAttributeType), metadata.Value);
                }
            }

            foreach (var relationMetadata in attributes.Where(a => a.AttributeClass.Name == "CrmLookupAttribute"))
            {
                if (relationMetadata.ConstructorArguments.First().Kind == TypedConstantKind.Primitive)
                {

                }
                else
                {
                    var targetSymbol = (INamedTypeSymbol) relationMetadata.ConstructorArguments.First().Value;

                    var relationship = new RelationshipMetadata
                    {
                        TargetNamedTypeSymbol = targetSymbol,
                        TargetAttributeName = (string) relationMetadata.ConstructorArguments[1].Value,
                        SchemaName = (string) relationMetadata.NamedArguments[0].Value.Value
                    };
                    Relationships.Add(relationship);
                }
            }


        }
    }
}
