// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class CrmLookupAttribute : Attribute
    {
        public CrmLookupAttribute(string targetEntityName, string attributeName, bool allowNotExisting = false)
        {
            TargetEntityName = targetEntityName;
            AttributeName = attributeName;
            AllowNotExisting = allowNotExisting;
        }
        /// <summary>
        ///     Names the targeted table through its generated definition class —
        ///     <c>[CrmLookup(typeof(SystemUserDefinition), …)]</c> — rather than through the
        ///     <c>SystemUserDefinition.EntityName</c> constant.
        /// </summary>
        /// <remarks>
        ///     The logical name is read back off the definition's <c>EntityName</c> constant, so
        ///     <see cref="TargetEntityName" /> carries the same value as with the string form.
        ///     Without that read the overload left it <see langword="null" />, and every consumer
        ///     of the attribute — the query builder that adds the link, the mapper that reads the
        ///     aliased value — silently had no target.
        /// </remarks>
        public CrmLookupAttribute(Type definitionType, string attributeName, bool allowNotExisting = false)
        {
            if (definitionType == null)
            {
                throw new ArgumentNullException(nameof(definitionType));
            }

            var field = definitionType.GetField("EntityName", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if (field == null || field.FieldType != typeof(string))
            {
                throw new ArgumentException(
                    $"{definitionType.Name} is not an entity definition: it exposes no public const string EntityName.",
                    nameof(definitionType));
            }

            DefinitionType = definitionType;
            TargetEntityName = (string)field.GetRawConstantValue();
            AttributeName = attributeName;
            AllowNotExisting = allowNotExisting;
        }

        public string RelationshipName { get; set; }

        private Type DefinitionType { get; set; }

        public string TargetEntityName { get; private set; }

        public string AttributeName { get; private set; }

        public bool AllowNotExisting { get; private set; }
    }
}