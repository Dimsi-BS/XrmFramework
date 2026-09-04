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
            DefinitionType = definitionType;
            TargetEntityName = ReadEntityName(definitionType);
            AttributeName = attributeName;
            AllowNotExisting = allowNotExisting;
        }

        /// <summary>
        ///     Reads the <c>EntityName</c> constant off a generated definition class, or returns
        ///     <see langword="null" /> when the type is not one.
        /// </summary>
        /// <remarks>
        ///     Deliberately silent rather than throwing. Attributes are constructed by reflection
        ///     while the mappers read them, so an exception here would surface as a
        ///     <see cref="System.Reflection.TargetInvocationException" /> from the middle of a
        ///     mapping rather than at the point of the mistake.
        /// </remarks>
        private static string ReadEntityName(Type definitionType)
        {
            var field = definitionType?.GetField(
                "EntityName",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return field != null && field.FieldType == typeof(string)
                ? (string)field.GetRawConstantValue()
                : null;
        }

        public string RelationshipName { get; set; }

        private Type DefinitionType { get; set; }

        public string TargetEntityName { get; private set; }

        public string AttributeName { get; private set; }

        public bool AllowNotExisting { get; private set; }
    }
}