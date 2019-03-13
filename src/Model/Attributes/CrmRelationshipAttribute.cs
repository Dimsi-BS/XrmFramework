// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using Model.Sdk;

namespace Model
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public abstract class CrmRelationshipAttribute : Attribute
    {
        protected CrmRelationshipAttribute(string relationshipName, EntityRole role)
        {
            RelationshipName = relationshipName;
            Role = role;
        }

        private string RelationshipName { get; set; }

        private EntityRole Role { get; set; }

        public bool IsValidForUpdate { get; set; } = true;

        public Relationship GetRelationship()
        {
            var relationship = new Relationship(RelationshipName);
            relationship.PrimaryEntityRole = Role;

            return relationship;
        }
    }

    //public class LookupRelationshipAttribute : CrmRelationshipAttribute
    //{
    //    public LookupRelationshipAttribute(string relationshipName) : base(relationshipName, EntityRole.Referencing) { }
    //}

    public class ChildRelationshipAttribute : CrmRelationshipAttribute
    {
        public ChildRelationshipAttribute(string relationshipName) : base(relationshipName, EntityRole.Referenced) { }
    }

    public class ManyToManyRelationshipAttribute : CrmRelationshipAttribute
    {
        public ManyToManyRelationshipAttribute(string relationshipName) : base(relationshipName, EntityRole.Referenced) { }

        public UpdateStrategy UpdateStrategy { get; set; }
    }

    public enum UpdateStrategy
    {
        None,
        Add,
        Replace
    }
}
