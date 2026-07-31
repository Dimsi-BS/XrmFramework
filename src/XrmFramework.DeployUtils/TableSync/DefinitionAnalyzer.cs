using System.Linq;

// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Loads an assembly via reflection and extracts the information from all classes
    /// decorated with [EntityDefinition].
    /// </summary>
    public static class DefinitionAnalyzer
    {
        /// <summary>
        /// Loads the specified DLL and returns the <see cref="DefinitionInfo"/> found.
        /// </summary>
        /// <param name="dllPath">Full path to the .dll to analyze.</param>
        public static IReadOnlyList<DefinitionInfo> ExtractDefinitions(string dllPath)
        {
            if (!File.Exists(dllPath))
                throw new FileNotFoundException($"DLL not found: {dllPath}", dllPath);

            // LoadFrom loads the assembly into the current context.
            // Attribute types are identified by name (not by type reference)
            // to avoid version conflicts.
            var assembly = Assembly.LoadFrom(dllPath);
            return ExtractDefinitions(assembly);
        }

        /// <summary>
        /// Extracts the <see cref="DefinitionInfo"/> from an already loaded assembly.
        /// </summary>
        public static IReadOnlyList<DefinitionInfo> ExtractDefinitions(Assembly assembly)
        {
            var result = new List<DefinitionInfo>();

            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Some types may fail to load if their dependencies are missing.
                // We work with the types that could be loaded.
                types = Array.FindAll(ex.Types, t => t != null);
            }

            foreach (var type in types)
            {
                if (!HasAttribute(type, "EntityDefinitionAttribute"))
                    continue;

                // EntityName is the key property — without it, there's no matching .table.
                var entityNameField = type.GetField("EntityName",
                    BindingFlags.Public | BindingFlags.Static);

                if (entityNameField == null)
                    continue;

                string entityName;
                try
                {
                    entityName = entityNameField.IsLiteral
                        ? entityNameField.GetRawConstantValue() as string
                        : entityNameField.GetValue(null) as string;
                }
                catch
                {
                    continue;
                }

                if (string.IsNullOrEmpty(entityName))
                    continue;

                // Table name = class name without the "Definition" suffix
                var typeName = type.Name;
                var tableName = typeName.EndsWith("Definition")
                    ? typeName.Substring(0, typeName.Length - "Definition".Length)
                    : typeName;

                string collectionName = null;
                var collectionField = type.GetField("EntityCollectionName",
                    BindingFlags.Public | BindingFlags.Static);
                if (collectionField != null)
                {
                    try
                    {
                        collectionName = collectionField.IsLiteral
                            ? collectionField.GetRawConstantValue() as string
                            : collectionField.GetValue(null) as string;
                    }
                    catch { /* optional */ }
                }

                result.Add(new DefinitionInfo
                {
                    TableName = tableName,
                    EntityName = entityName,
                    EntityCollectionName = collectionName,
                    Columns = ExtractColumns(type),
                    IsFullyGenerated = IsGeneratedByXrmFramework(type)
                });
            }

            return result;
        }

        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Checks for the presence of an attribute by its class name (without type resolution),
        /// which avoids conflicts if several versions of XrmFramework coexist.
        /// </summary>
        private static bool HasAttribute(Type type, string attributeSimpleName)
            => type.GetCustomAttributesData()
                   .Any(a => a.AttributeType.Name == attributeSimpleName);

        /// <summary>
        /// Returns true if [GeneratedCode("XrmFramework", "2.0")] is present on the type.
        /// </summary>
        private static bool IsGeneratedByXrmFramework(Type type)
            => type.GetCustomAttributesData()
                   .Any(a => a.AttributeType.Name == "GeneratedCodeAttribute"
                          && a.ConstructorArguments.Count >= 1
                          && string.Equals(
                                 a.ConstructorArguments[0].Value?.ToString(),
                                 "XrmFramework",
                                 StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Extracts the (LogicalName, CSharpName) pairs from the nested "Columns" class.
        /// </summary>
        private static IReadOnlyList<DefinitionColumnInfo> ExtractColumns(Type definitionType)
        {
            var columnsType = definitionType.GetNestedType("Columns",
                BindingFlags.Public | BindingFlags.NonPublic);

            if (columnsType == null)
                return new List<DefinitionColumnInfo>();

            var columns = new List<DefinitionColumnInfo>();

            foreach (var field in columnsType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                // We only want string-type constants.
                if (!field.IsLiteral || field.FieldType.FullName != "System.String")
                    continue;

                string logicalName;
                try
                {
                    logicalName = field.GetRawConstantValue() as string;
                }
                catch
                {
                    continue;
                }

                if (string.IsNullOrEmpty(logicalName))
                    continue;

                var optionSetType = ExtractOptionSetType(field);

                columns.Add(new DefinitionColumnInfo(
                    logicalName, field.Name, optionSetType?.Name, ExtractOptionSetValues(optionSetType)));
            }

            return columns;
        }

        /// <summary>
        /// Reads the enum type from <c>[OptionSet(typeof(SomeEnum))]</c> carried by a column constant.
        /// </summary>
        /// <remarks>
        /// Like the rest of this analyzer, the attribute is matched by simple name so that several
        /// versions of XrmFramework can coexist in the load context. The argument is a
        /// <see cref="Type" />, whose resolution may fail if the enum lives in an assembly that could
        /// not be loaded — in that case the column simply carries no option set.
        /// </remarks>
        private static Type ExtractOptionSetType(FieldInfo field)
        {
            try
            {
                foreach (var attribute in field.GetCustomAttributesData())
                {
                    if (attribute.AttributeType.Name != "OptionSetAttribute"
                        || attribute.ConstructorArguments.Count < 1)
                        continue;

                    return attribute.ConstructorArguments[0].Value as Type;
                }
            }
            catch
            {
                // Unresolvable attribute or type: not worth failing the whole migration for.
            }

            return null;
        }

        /// <summary>
        /// Reads the members of an option set enum, in declaration order, as (value, C# name) pairs.
        /// </summary>
        /// <remarks>
        /// Declaration order matters: when the generator allows an empty value it emits a synthetic
        /// <c>Null = 0</c> ahead of the real members, and the caller relies on the order to tell that
        /// one apart from a genuine option numbered 0.
        /// </remarks>
        private static IReadOnlyList<DefinitionOptionSetValue> ExtractOptionSetValues(Type enumType)
        {
            var values = new List<DefinitionOptionSetValue>();

            if (enumType == null || !enumType.IsEnum)
                return values;

            foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (!field.IsLiteral)
                    continue;

                try
                {
                    var raw = field.GetRawConstantValue();
                    if (raw == null)
                        continue;

                    // CRM option values are int; an enum backed by a wider type that overflows is
                    // not one of ours, and skipping the member beats failing the migration.
                    values.Add(new DefinitionOptionSetValue(Convert.ToInt32(raw), field.Name));
                }
                catch (Exception)
                {
                }
            }

            return values;
        }
    }
}
