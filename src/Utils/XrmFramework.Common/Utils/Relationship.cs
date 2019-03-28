// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Model.Sdk
{
    //
    // Summary:
    //     Represents a relationship between two entities.
    public sealed class Relationship
    {
        //
        // Summary:
        //     Initializes a new instance of the Microsoft.Xrm.Sdk.Relationship class.
        public Relationship()
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the Microsoft.Xrm.Sdk.Relationship class setting
        //     the schema name property.
        //
        // Parameters:
        //   schemaName:
        //     Type: https://msdn.microsoft.com/library/system.string.aspx. The name of the
        //     relationship.
        public Relationship(string schemaName)
        {
            SchemaName = schemaName;
        }

        //
        // Summary:
        //     Gets or sets the name of the relationship.
        //
        // Returns:
        //     Type: https://msdn.microsoft.com/library/system.string.aspx The name of the relationship
        //     as defined defined in the Microsoft.Xrm.Sdk.Relationship.SchemaName property.
        public string SchemaName { get; set; }

        public EntityRole PrimaryEntityRole { get; set; }

        public string NavigationPropertyName { get; set; }

        public string LookupFieldName { get; set; }

        public string TargetEntityName { get; set; }
        //
        // Summary:
        //     Gets or sets the entity role: referenced or referencing.
        //
        // Returns:
        //     Type: System.Nullable`1<Microsoft.Xrm.Sdk.EntityRole> The entity role.
        //public EntityRole? PrimaryEntityRole { get; set; }

        //
        // Summary:
        //     Determines whether two instances are equal.
        //
        // Parameters:
        //   obj:
        //     Type: https://msdn.microsoft.com/library/system.object.aspx. The Relationship
        //     to compare with the current Relationship.
        //
        // Returns:
        //     Type: https://msdn.microsoft.com/library/system.boolean.aspxtrue if the specified
        //     Relationship is equal to the Relationship Object; otherwise, false.
        public override bool Equals(object obj)
        {
            return SchemaName == ((Relationship) obj)?.SchemaName;
        }

        //
        // Summary:
        //     Serves as a hash function for this type.
        //
        // Returns:
        //     Type: https://msdn.microsoft.com/library/system.int32.aspx The hash code for
        //     the current Relationship.
        public override int GetHashCode()
        {
            return SchemaName?.GetHashCode() ?? 0;
        }

        //
        // Summary:
        //     Returns a String that represents the current Relationship.
        //
        // Returns:
        //     Type: https://msdn.microsoft.com/library/system.string.aspx The name of the current
        //     Relationship.
        public override string ToString() => $"Relationship:{SchemaName}";
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RelationshipAttribute : Attribute
    {
        public EntityRole Role { get; }
        public string NavigationPropertyName { get; }
        public string TargetEntityName { get; }
        public string LookupFieldName { get; }

        public RelationshipAttribute(string targetEntityName, EntityRole role, string navigationPropertyName, string lookupFieldName)
        {
            TargetEntityName = targetEntityName;
            Role = role;
            NavigationPropertyName = navigationPropertyName;
            LookupFieldName = lookupFieldName;
        }
    }
}
