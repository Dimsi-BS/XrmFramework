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
    /// Charge un assembly par réflexion et extrait les informations de toutes les classes
    /// décorées par [EntityDefinition].
    /// </summary>
    public static class DefinitionAnalyzer
    {
        /// <summary>
        /// Charge le DLL indiqué et retourne les <see cref="DefinitionInfo"/> trouvées.
        /// </summary>
        /// <param name="dllPath">Chemin complet vers le .dll à analyser.</param>
        public static IReadOnlyList<DefinitionInfo> ExtractDefinitions(string dllPath)
        {
            if (!File.Exists(dllPath))
                throw new FileNotFoundException($"DLL introuvable : {dllPath}", dllPath);

            // LoadFrom charge l'assembly dans le contexte courant.
            // Les types des attributs sont identifiés par nom (pas par référence de type)
            // pour éviter les conflits de version.
            var assembly = Assembly.LoadFrom(dllPath);
            return ExtractDefinitions(assembly);
        }

        /// <summary>
        /// Extrait les <see cref="DefinitionInfo"/> d'un assembly déjà chargé.
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
                // Certains types peuvent échouer si leurs dépendances sont absentes.
                // On travaille avec les types qui ont pu être chargés.
                types = Array.FindAll(ex.Types, t => t != null);
            }

            foreach (var type in types)
            {
                if (!HasAttribute(type, "EntityDefinitionAttribute"))
                    continue;

                // EntityName est la propriété clé — sans elle, pas de .table correspondant.
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

                // Nom de la table = nom de la classe sans le suffixe "Definition"
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
                    catch { /* optionnel */ }
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
        /// Vérifie la présence d'un attribut par son nom de classe (sans résolution de type),
        /// ce qui évite les conflits si plusieurs versions de XrmFramework coexistent.
        /// </summary>
        private static bool HasAttribute(Type type, string attributeSimpleName)
            => type.GetCustomAttributesData()
                   .Any(a => a.AttributeType.Name == attributeSimpleName);

        /// <summary>
        /// Retourne true si [GeneratedCode("XrmFramework", "2.0")] est présent sur le type.
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
        /// Extrait les paires (LogicalName, CSharpName) depuis la nested class "Columns".
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
                // On ne veut que les constantes de type string.
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

                if (!string.IsNullOrEmpty(logicalName))
                    columns.Add(new DefinitionColumnInfo(logicalName, field.Name));
            }

            return columns;
        }
    }
}
