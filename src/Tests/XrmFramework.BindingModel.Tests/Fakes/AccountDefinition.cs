// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace XrmFramework.BindingModel.Tests.Fakes
{
    /// <summary>
    /// Minimal in-test entity definition for "account", used as a lookup target by <see cref="ContactDefinition"/>.
    /// </summary>
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    public static class AccountDefinition
    {
        public const string EntityName = "account";
        public const string EntityCollectionName = "accounts";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>Primary identifier of the account record.</summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "accountid";

            /// <summary>Name of the account.</summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            public const string Name = "name";
        }
    }
}
