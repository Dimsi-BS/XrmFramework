using NUnit.Framework;
using System.Threading.Tasks;
using XrmFramework.Analyzers.Generators;

namespace XrmFramework.Analyzers.Tests;

[TestFixture]
public class MappingSourceGeneratorTests
{
    // Inline source that defines all types needed by the generator:
    //  - the attributes the generator looks for ([CrmEntity], [CrmMapping], [AttributeMetadata], [CrmLookup])
    //  - a definition class with a Columns nested class
    //  - a concrete partial binding model (ContactModel)
    //  - a BindingModelBase subclass (ContactModelWithBase) to verify InitializedProperties guards
    private const string Source = @"
using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

// ── Minimal attribute stubs ────────────────────────────────────────────────────

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class CrmEntityAttribute : Attribute
    {
        public CrmEntityAttribute(string entityName) => EntityName = entityName;
        public string EntityName { get; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class AttributeMetadataAttribute : Attribute
    {
        public AttributeMetadataAttribute(AttributeTypeCode type) => Type = type;
        public AttributeTypeCode Type { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class CrmLookupAttribute : Attribute
    {
        public CrmLookupAttribute(string targetEntityName, string attributeName) { }
        public string RelationshipName { get; set; }
    }

    public enum AttributeTypeCode
    {
        Boolean = 0, Customer = 1, DateTime = 2, Decimal = 3, Double = 4,
        Integer = 5, Lookup = 6, Money = 8, Memo = 9, Owner = 10,
        Picklist = 11, State = 12, Status = 13, String = 14,
        Uniqueidentifier = 15, MultiSelectPicklist = 19,
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class PrimaryAttributeAttribute : Attribute
    {
        public PrimaryAttributeAttribute(PrimaryAttributeType type) { }
    }

    public enum PrimaryAttributeType { Id, Name, Image }

    [AttributeUsage(AttributeTargets.Class)]
    public class EntityDefinitionAttribute : Attribute { }
}

namespace XrmFramework.BindingModel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CrmMappingAttribute : Attribute
    {
        public CrmMappingAttribute(string attributeName) => AttributeName = attributeName;
        public string AttributeName { get; }
        public bool IsValidForUpdate { get; set; } = true;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ExtendBindingModelAttribute : Attribute { }

    public interface IBindingModel
    {
        Guid Id { get; set; }
    }

    public abstract class BindingModelBase : IBindingModel
    {
        public Guid Id { get; set; }
        protected System.Collections.Generic.HashSet<string> InitializedProperties { get; } = new();
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
            => InitializedProperties.Add(name);
    }
}

// ── AccountDefinition (lookup target) ─────────────────────────────────────────

[XrmFramework.EntityDefinition]
public static class AccountDefinition
{
    public const string EntityName = ""account"";
    public const string EntityCollectionName = ""accounts"";

    public static class Columns
    {
        [XrmFramework.AttributeMetadata(XrmFramework.AttributeTypeCode.Uniqueidentifier)]
        [XrmFramework.PrimaryAttribute(XrmFramework.PrimaryAttributeType.Id)]
        public const string Id = ""accountid"";
    }
}

// ── ContactDefinition ─────────────────────────────────────────────────────────

[XrmFramework.EntityDefinition]
public static class ContactDefinition
{
    public const string EntityName = ""contact"";
    public const string EntityCollectionName = ""contacts"";

    public static class Columns
    {
        [XrmFramework.AttributeMetadata(XrmFramework.AttributeTypeCode.Uniqueidentifier)]
        [XrmFramework.PrimaryAttribute(XrmFramework.PrimaryAttributeType.Id)]
        public const string Id = ""contactid"";

        [XrmFramework.AttributeMetadata(XrmFramework.AttributeTypeCode.String)]
        [XrmFramework.PrimaryAttribute(XrmFramework.PrimaryAttributeType.Name)]
        public const string FullName = ""fullname"";

        [XrmFramework.AttributeMetadata(XrmFramework.AttributeTypeCode.String)]
        public const string Email = ""emailaddress1"";

        [XrmFramework.AttributeMetadata(XrmFramework.AttributeTypeCode.Boolean)]
        public const string IsActive = ""xrm_isactive"";

        [XrmFramework.AttributeMetadata(XrmFramework.AttributeTypeCode.DateTime)]
        public const string BirthDate = ""birthdate"";

        [XrmFramework.AttributeMetadata(XrmFramework.AttributeTypeCode.Money)]
        public const string Revenue = ""revenue"";

        [XrmFramework.AttributeMetadata(XrmFramework.AttributeTypeCode.Picklist)]
        public const string StatusCode = ""statuscode"";

        [XrmFramework.AttributeMetadata(XrmFramework.AttributeTypeCode.MultiSelectPicklist)]
        public const string Interests = ""xrm_interests"";

        [XrmFramework.AttributeMetadata(XrmFramework.AttributeTypeCode.Lookup)]
        [XrmFramework.CrmLookup(AccountDefinition.EntityName, AccountDefinition.Columns.Id)]
        public const string AccountId = ""parentcustomerid"";
    }
}

// ── Enums ─────────────────────────────────────────────────────────────────────

public enum ContactStatus  { Null = 0, Active = 1, Inactive = 2 }
public enum ContactInterest { Null = 0, Sports = 1, Music = 2 }

// ── ContactModel  (IBindingModel, no BindingModelBase) ────────────────────────

[XrmFramework.CrmEntity(ContactDefinition.EntityName)]
public partial class ContactModel : XrmFramework.BindingModel.IBindingModel
{
    public Guid Id { get; set; }

    [XrmFramework.BindingModel.CrmMapping(ContactDefinition.Columns.FullName)]
    public string? FullName { get; set; }

    [XrmFramework.BindingModel.CrmMapping(ContactDefinition.Columns.Email)]
    public string? Email { get; set; }

    [XrmFramework.BindingModel.CrmMapping(ContactDefinition.Columns.IsActive)]
    public bool? IsActive { get; set; }

    [XrmFramework.BindingModel.CrmMapping(ContactDefinition.Columns.BirthDate)]
    public DateTime? BirthDate { get; set; }

    [XrmFramework.BindingModel.CrmMapping(ContactDefinition.Columns.Revenue)]
    public decimal? Revenue { get; set; }

    [XrmFramework.BindingModel.CrmMapping(ContactDefinition.Columns.StatusCode)]
    public ContactStatus StatusCode { get; set; }

    [XrmFramework.BindingModel.CrmMapping(ContactDefinition.Columns.Interests)]
    public List<ContactInterest> Interests { get; } = new();

    [XrmFramework.BindingModel.CrmMapping(ContactDefinition.Columns.AccountId)]
    public Guid AccountId { get; set; }
}

// ── ContactModelWithBase  (extends BindingModelBase -> InitializedProperties) ──

[XrmFramework.CrmEntity(ContactDefinition.EntityName)]
public partial class ContactModelWithBase : XrmFramework.BindingModel.BindingModelBase
{
    [XrmFramework.BindingModel.CrmMapping(ContactDefinition.Columns.FullName)]
    public string? FullName { get; set { field = value; OnPropertyChanged(); } }

    [XrmFramework.BindingModel.CrmMapping(ContactDefinition.Columns.Revenue)]
    public decimal? Revenue { get; set { field = value; OnPropertyChanged(); } }

    [XrmFramework.BindingModel.CrmMapping(ContactDefinition.Columns.StatusCode)]
    public ContactStatus StatusCode { get; set { field = value; OnPropertyChanged(); } }
}
";

    [Test]
    public async Task MappingGenerator_ContactModel_GeneratesCorrectMappings()
    {
        await TestHelper.Verify<MappingSourceGenerator>(Source);
    }
}
