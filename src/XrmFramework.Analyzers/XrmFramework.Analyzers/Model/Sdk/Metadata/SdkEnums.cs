using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Sdk
{
    public enum AttributeTypeCode
    {
        //
        // Summary:
        //     A Boolean attribute. Value = 0.
        Boolean = 0,

        //
        // Summary:
        //     An attribute that represents a customer. Value = 1.
        Customer = 1,

        //
        // Summary:
        //     A date/time attribute. Value = 2.
        DateTime = 2,

        //
        // Summary:
        //     A decimal attribute. Value = 3.
        Decimal = 3,

        //
        // Summary:
        //     A double attribute. Value = 4.
        Double = 4,

        //
        // Summary:
        //     An integer attribute. Value = 5.
        Integer = 5,

        //
        // Summary:
        //     A lookup attribute. Value = 6.
        Lookup = 6,

        //
        // Summary:
        //     A memo attribute. Value = 7.
        Memo = 7,

        //
        // Summary:
        //     A money attribute. Value = 8.
        Money = 8,

        //
        // Summary:
        //     An owner attribute. Value = 9.
        Owner = 9,

        //
        // Summary:
        //     A partylist attribute. Value = 10.
        PartyList = 10,

        //
        // Summary:
        //     A picklist attribute. Value = 11.
        Picklist = 11,

        //
        // Summary:
        //     A state attribute. Value = 12.
        State = 12,

        //
        // Summary:
        //     A status attribute. Value = 13.
        Status = 13,

        //
        // Summary:
        //     A string attribute. Value = 14.
        String = 14,

        //
        // Summary:
        //     An attribute that is an ID. Value = 15.
        Uniqueidentifier = 15,

        //
        // Summary:
        //     An attribute that contains calendar rules. Value = 0x10.
        CalendarRules = 16,

        //
        // Summary:
        //     An attribute that is created by the system at run time. Value = 0x11.
        Virtual = 17,

        //
        // Summary:
        //     A big integer attribute. Value = 0x12.
        BigInt = 18,

        //
        // Summary:
        //     A managed property attribute. Value = 0x13.
        ManagedProperty = 19,

        //
        // Summary:
        //     An entity name attribute. Value = 20.
        EntityName = 20
    }


    //
    // Summary:
    //     Contains values to indicate the role the entity plays in a relationship.
    public enum EntityRole
    {
        //
        // Summary:
        //     Specifies that the entity is the referencing entity. Value = 0.
        Referencing = 0,
        //
        // Summary:
        //     Specifies that the entity is the referenced entity. Value = 1.
        Referenced = 1
    }



    public enum DateTimeBehavior
    {
        UserLocal = 0,
        DateOnly = 1,
        TimeZoneIndependent = 2
    }
}
