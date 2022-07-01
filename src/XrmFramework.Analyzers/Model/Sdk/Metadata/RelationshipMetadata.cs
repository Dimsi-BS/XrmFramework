using Microsoft.CodeAnalysis;
using System.Runtime.Serialization;
using XrmFramework.Analyzers.Utils;

namespace Model.Sdk.Metadata
{
    [DataContract]
    public class RelationshipMetadata
    {
        [DataMember(Name = "n")]
        public string SchemaName { get; set; }

        [DataMember(Name = "r")]
        public string RoleString { get; set; }

        [DataMember(Name = "e")]
        public string TargetEntityName { get; set; }

        [DataMember(Name = "a")]
        public string TargetAttributeName { get; set; }

        [DataMember(Name = "np")]
        public string NavigationPropertyName { get; set; }

        [DataMember(Name = "l")]
        public string LookupFieldName { get; set; }

        public EntityDefinition TargetEntityDefinition => SymbolEqualityComparer.Default.Equals(TargetNamedTypeSymbol, default(INamedTypeSymbol)) ? null : EntityDefinition.GetEntityDefinition(TargetNamedTypeSymbol);

        public EntityRole Role
        {
            get => (EntityRole)Enum.Parse(typeof(EntityRole), RoleString);
            set => RoleString = value.ToString();
        }

        public INamedTypeSymbol TargetNamedTypeSymbol { get; set; }
    }
}
