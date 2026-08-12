//HintName: Account.table.cs
using System;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;

namespace XrmFramework
{
    [GeneratedCode("XrmFramework", "2.0")]
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    public static partial class AccountDefinition
    {
        public const string EntityName = "account";
        public const string EntityCollectionName = "accounts";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : Picklist (Categorie)
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Picklist)]
            [OptionSet(typeof(Categorie))]
            public const string AccountCategoryCode = "accountcategorycode";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "accountid";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            [StringLength(160)]
            public const string Name = "name";

        }
    }

    [OptionSetDefinition(AccountDefinition.EntityName, AccountDefinition.Columns.AccountCategoryCode)]
    public enum Categorie
    {
        Null = 0,
        [Description("ClientFavori")]
        ClientFavori = 1,
        [Description("Standard")]
        Standard = 2,
    }
}
