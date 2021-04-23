// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace XrmFramework.Sdk.Metadata
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
