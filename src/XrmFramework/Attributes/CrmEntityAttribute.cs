// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class CrmEntityAttribute : Attribute
    {
        private const string EntityNameField = "EntityName";

        public CrmEntityAttribute(string entityName)
        {
            EntityName = entityName;
        }

        /// <summary>
        ///     Names the table through its generated definition class —
        ///     <c>[CrmEntity(typeof(AccountDefinition))]</c> — rather than through the
        ///     <c>AccountDefinition.EntityName</c> constant.
        /// </summary>
        /// <remarks>
        ///     Preferred over the string form. It is refactor-safe, and it lets the source
        ///     generators name the table without resolving a constant: in the project that owns
        ///     the <c>.table</c> files, the definition class is emitted in the same pass and is
        ///     therefore not resolvable while the mapping is being generated.
        ///
        ///     At runtime the logical name is read back off the definition's <c>EntityName</c>
        ///     constant, so <see cref="EntityName" /> carries the same value as with the string
        ///     form and every consumer of it is unaffected.
        /// </remarks>
        /// <param name="definitionType">
        ///     A generated <c>…Definition</c> class exposing a <c>public const string EntityName</c>.
        /// </param>
        public CrmEntityAttribute(Type definitionType)
        {
            if (definitionType == null)
            {
                throw new ArgumentNullException(nameof(definitionType));
            }

            var field = definitionType.GetField(EntityNameField, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if (field == null || field.FieldType != typeof(string))
            {
                throw new ArgumentException(
                    $"{definitionType.Name} is not an entity definition: it exposes no public const string {EntityNameField}.",
                    nameof(definitionType));
            }

            EntityName = (string)field.GetRawConstantValue();
            DefinitionType = definitionType;
        }

        public string EntityName { get; private set; }

        /// <summary>
        ///     The definition class the table was named through, when the <see cref="Type" />
        ///     form was used; <see langword="null" /> with the string form.
        /// </summary>
        public Type DefinitionType { get; private set; }

        public bool ValidForCreate { get; set; } = true;

        public bool AllowDeactivation { get; set; } = true;
    }
}