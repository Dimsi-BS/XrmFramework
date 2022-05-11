// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework
{
    public enum AttributeTypeCode
    {
        /// <summary>
        /// A Boolean attribute. Value = 0.
        /// </summary>
        Boolean = 0,

        /// <summary>
        /// An attribute that represents a customer. Value = 1.
        /// </summary>
        Customer = 1,

        /// <summary>
        /// A date/time attribute. Value = 2.
        /// </summary>
        DateTime = 2,

        /// <summary>
        /// A decimal attribute. Value = 3.
        /// </summary>
        Decimal = 3,

        /// <summary>
        /// A double attribute. Value = 4.
        /// </summary>
        Double = 4,

        /// <summary>
        /// An integer attribute. Value = 5.
        /// </summary>
        Integer = 5,

        /// <summary>
        /// A lookup attribute. Value = 6.
        /// </summary>
        Lookup = 6,

        /// <summary>
        /// A memo attribute. Value = 7.
        /// </summary>
        Memo = 7,

        /// <summary>
        /// A money attribute. Value = 8.
        /// </summary>
        Money = 8,

        /// <summary>
        /// An owner attribute. Value = 9.
        /// </summary>
        Owner = 9,

        /// <summary>
        /// A partylist attribute. Value = 10.
        /// </summary>
        PartyList = 10,

        /// <summary>
        /// A picklist attribute. Value = 11.
        /// </summary>
        Picklist = 11,

        /// <summary>
        /// A state attribute. Value = 12.
        /// </summary>
        State = 12,

        /// <summary>
        /// A status attribute. Value = 13.
        /// </summary>
        Status = 13,

        /// <summary>
        /// A string attribute. Value = 14.
        /// </summary>
        String = 14,

        /// <summary>
        /// An attribute that is an ID. Value = 15.
        /// </summary>
        Uniqueidentifier = 15,

        /// <summary>
        /// An attribute that contains calendar rules. Value = 0x10.
        /// </summary>
        CalendarRules = 16,

        /// <summary>
        /// An attribute that is created by the system at run time. Value = 0x11.
        /// </summary>
        Virtual = 17,

        /// <summary>
        /// A big integer attribute. Value = 0x12.
        /// </summary>
        BigInt = 18,

        /// <summary>
        /// A managed property attribute. Value = 0x13.
        /// </summary>
        ManagedProperty = 19,

        /// <summary>
        /// An entity name attribute. Value = 20.
        /// </summary>
        EntityName = 20,

        /// <summary>
        /// A mulit select Picklist
        /// </summary>
        MultiSelectPicklist = 21
    }


    //
    // Summary:
    //     Contains values to indicate the role the entity plays in a relationship.
    public enum EntityRole
    {
        /// <summary>
        /// Specifies that the entity is the referencing entity. Value = 0.
        /// </summary>
        Referencing = 0,

        /// <summary>
        /// Specifies that the entity is the referenced entity. Value = 1.
        /// </summary>
        Referenced = 1
    }

    public enum PluginImageType
    {
        PreImage,
        PostImage
    }


    public enum DateTimeBehavior
    {
        UserLocal = 0,
        DateOnly = 1,
        TimeZoneIndependent = 2
    }
}
