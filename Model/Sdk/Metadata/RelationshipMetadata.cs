using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

        [DataMember(Name = "np")]
        public string NavigationPropertyName { get; set; }
        
        [DataMember(Name = "l")]
        public string LookupFieldName { get; set; }

        public EntityRole Role
        {
            get => (EntityRole) Enum.Parse(typeof(EntityRole), RoleString);
            set => RoleString = value.ToString();
        }
    }
}
