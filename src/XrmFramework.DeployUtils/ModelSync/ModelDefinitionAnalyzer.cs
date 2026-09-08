// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmFramework.Core;
using CoreModel = XrmFramework.Core.Model;

namespace XrmFramework.DeployUtils.ModelSync
{
    /// <summary>
    /// Loads an assembly via reflection and extracts a <see cref="CoreModel"/> for every hand-written
    /// binding model class it declares — the input <c>migrate sync-models</c> writes out as
    /// <c>.model</c> files, for <c>ModelSourceFileGenerator</c> to reproduce at compile time.
    /// </summary>
    /// <remarks>
    /// Attribute types are identified by simple name off <see cref="CustomAttributeData"/>, never
    /// resolved and instantiated, the same technique <see cref="TableSync.DefinitionAnalyzer"/> uses
    /// for <c>[EntityDefinition]</c> classes and for the same reason: the assembly being analyzed
    /// references its own copy of XrmFramework, whose version need not match the one this tool
    /// itself was built against, and resolving <c>[CrmMapping]</c> etc. as live types would collide
    /// with it. Reading <see cref="CustomAttributeData"/> instead works from raw metadata, so it
    /// never needs XrmFramework.BindingModel's actual attribute types to load.
    /// </remarks>
    /// <summary>
    /// One hand-written <c>IBindingModel</c> class <see cref="ModelDefinitionAnalyzer.ExtractModels(Assembly, out IReadOnlyList{ModelExtractionSkip})"/>
    /// declined to turn into a <c>.model</c> file, and why.
    /// </summary>
    public readonly struct ModelExtractionSkip
    {
        public string ClassName { get; }

        public string Reason { get; }

        public ModelExtractionSkip(string className, string reason)
        {
            ClassName = className;
            Reason = reason;
        }
    }

    public static class ModelDefinitionAnalyzer
    {
        /// <summary>
        /// Attribute types <see cref="ExtractProperty"/> already reads into a dedicated
        /// <see cref="ModelProperty"/> field — never re-emitted a second time into
        /// <see cref="ModelProperty.Attrs"/>.
        /// </summary>
        private static readonly HashSet<string> RecognizedAttributeNames = new(StringComparer.Ordinal)
        {
            "CrmMappingAttribute",
            "CrmLookupAttribute",
            "ExtendBindingModelAttribute",
            "ChildRelationshipAttribute",
            "JsonPropertyAttribute",
            "JsonIgnoreAttribute",
        };

        /// <summary>
        /// Namespaces <c>ModelSourceFileGenerator</c> already imports into every generated file — an
        /// attribute living in one of these needs no entry of its own in <see cref="CoreModel.Usings"/>.
        /// </summary>
        private static readonly HashSet<string> DefaultGeneratedUsings = new(StringComparer.Ordinal)
        {
            "System",
            "System.CodeDom.Compiler",
            "System.ComponentModel.DataAnnotations",
            "System.Diagnostics.CodeAnalysis",
            "System.Collections.Generic",
            "System.Linq",
            "Microsoft.Xrm.Sdk",
            "XrmFramework",
            "Newtonsoft.Json",
            "XrmFramework.BindingModel",
        };

        private static readonly Dictionary<Type, string> PrimitiveAliases = new()
        {
            [typeof(bool)] = "bool",
            [typeof(byte)] = "byte",
            [typeof(sbyte)] = "sbyte",
            [typeof(char)] = "char",
            [typeof(decimal)] = "decimal",
            [typeof(double)] = "double",
            [typeof(float)] = "float",
            [typeof(int)] = "int",
            [typeof(uint)] = "uint",
            [typeof(long)] = "long",
            [typeof(ulong)] = "ulong",
            [typeof(short)] = "short",
            [typeof(ushort)] = "ushort",
            [typeof(object)] = "object",
            [typeof(string)] = "string",
            [typeof(Guid)] = "Guid",
            [typeof(DateTime)] = "DateTime",
        };

        /// <summary>
        /// Loads the specified DLL and returns the <see cref="CoreModel"/>s found.
        /// </summary>
        /// <param name="dllPath">Full path to the .dll to analyze.</param>
        public static IReadOnlyList<CoreModel> ExtractModels(string dllPath) => ExtractModels(dllPath, out _);

        /// <summary>
        /// Loads the specified DLL and returns the <see cref="CoreModel"/>s found, plus the classes
        /// that otherwise qualified but were left alone — see
        /// <see cref="ExtractModels(Assembly, out IReadOnlyList{ModelExtractionSkip})"/>.
        /// </summary>
        public static IReadOnlyList<CoreModel> ExtractModels(string dllPath, out IReadOnlyList<ModelExtractionSkip> skipped)
        {
            if (!File.Exists(dllPath))
                throw new FileNotFoundException($"DLL not found: {dllPath}", dllPath);

            // LoadFrom loads the assembly into the current context. Attribute types are identified
            // by name (not by type reference) to avoid version conflicts — see the class remarks.
            var assembly = Assembly.LoadFrom(dllPath);
            return ExtractModels(assembly, out skipped);
        }

        /// <summary>
        /// Extracts the <see cref="CoreModel"/>s from an already loaded assembly.
        /// </summary>
        public static IReadOnlyList<CoreModel> ExtractModels(Assembly assembly) => ExtractModels(assembly, out _);

        /// <summary>
        /// Extracts the <see cref="CoreModel"/>s from an already loaded assembly, plus the classes
        /// that carry <c>[CrmEntity]</c> and implement <c>IBindingModel</c> — everything else this
        /// method looks for — but were left alone anyway: a class that already extends something
        /// other than <c>BindingModelBase</c> by hand cannot be reproduced by
        /// <c>ModelSourceFileGenerator</c>, which always emits <c>partial class X : BindingModelBase</c>.
        /// Converting it to a <c>.model</c> file would break the build (CS0263) rather than migrate
        /// it — such a class is left hand-written, which <c>MappingSourceGenerator</c> already
        /// supports directly (it reads the same <c>[CrmMapping]</c> attributes off the Roslyn symbol,
        /// without needing a <c>.model</c> file or touching the class's own base type).
        /// </summary>
        public static IReadOnlyList<CoreModel> ExtractModels(Assembly assembly, out IReadOnlyList<ModelExtractionSkip> skipped)
        {
            var result = new List<CoreModel>();
            var skippedList = new List<ModelExtractionSkip>();

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
                if (!type.IsClass || type.IsAbstract)
                    continue;

                if (!ImplementsInterfaceNamed(type, "IBindingModel"))
                    continue;

                // Already produced by ModelSourceFileGenerator (or TableSourceFileGenerator, for
                // an unrelated class that happens to also implement IBindingModel by hand): nothing
                // hand-written to recover, and re-extracting it would just be a lossy round-trip of
                // a file already sitting next to it.
                if (IsGeneratedByXrmFramework(type))
                    continue;

                var crmEntityAttr = type.GetCustomAttributesData()
                    .FirstOrDefault(a => a.AttributeType.Name == "CrmEntityAttribute");

                if (crmEntityAttr == null)
                    continue;

                var entityName = ReadCrmEntityName(crmEntityAttr);
                if (string.IsNullOrEmpty(entityName))
                    continue;

                if (HasIncompatibleBaseClass(type, out var baseClassName))
                {
                    skippedList.Add(new ModelExtractionSkip(
                        type.Name,
                        $"extends {baseClassName} by hand, not BindingModelBase — already handled as a " +
                        "hand-written class by MappingSourceGenerator"));
                    continue;
                }

                if (IsFrameworkOwnedNamespace(type.Namespace))
                {
                    skippedList.Add(new ModelExtractionSkip(
                        type.Name,
                        $"lives in the {type.Namespace} namespace — shipped by the framework itself " +
                        "(as a contentFiles source, compiled directly into this assembly), not a class " +
                        "this project's own source tree declares"));
                    continue;
                }

                var model = new CoreModel
                {
                    TableLogicalName = entityName,
                    Name = type.Name,
                    ModelNamespace = type.Namespace ?? string.Empty
                };

                var usings = new HashSet<string>(StringComparer.Ordinal);

                foreach (var property in CollectProperties(type))
                {
                    var extracted = ExtractProperty(property, usings);
                    if (extracted != null)
                        model.Properties.Add(extracted);
                }

                if (usings.Count > 0)
                {
                    model.Usings = usings.OrderBy(ns => ns, StringComparer.Ordinal).ToArray();
                }

                result.Add(model);
            }

            skipped = skippedList;
            return result;
        }

        // ──────────────────────────────────────────────────────────────────────────
        //  Type discovery
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Walks <paramref name="root"/> and its base types, yielding each declared property once —
        /// the same inheritance walk <c>MappingSourceGenerator.CollectMappings</c> does over Roslyn
        /// symbols, so a property overridden or hidden further down is not read twice.
        /// </summary>
        private static IEnumerable<PropertyInfo> CollectProperties(Type root)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);

            for (var type = root; type != null && type != typeof(object); type = type.BaseType)
            {
                foreach (var property in type.GetProperties(
                             BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (seen.Add(property.Name))
                        yield return property;
                }
            }
        }

        /// <summary>
        /// Checks for the presence of an interface by its simple name (without type resolution),
        /// which avoids conflicts if several versions of XrmFramework coexist.
        /// </summary>
        private static bool ImplementsInterfaceNamed(Type type, string simpleName)
        {
            try
            {
                return type.GetInterfaces().Any(i => i.Name == simpleName);
            }
            catch
            {
                // An interface that fails to load (a missing dependency) is not the one we want.
                return false;
            }
        }

        /// <summary>
        /// True when <paramref name="type"/> already extends a hand-written class of its own —
        /// something other than <c>BindingModelBase</c> (or plain <see cref="object"/>, meaning no
        /// base was declared at all). <c>ModelSourceFileGenerator</c> always emits
        /// <c>partial class X : BindingModelBase</c>; a differing base class on the hand-written half
        /// is a compile error (CS0263), not a merge — interfaces are the only thing partial
        /// declarations are free to repeat independently.
        /// </summary>
        private static bool HasIncompatibleBaseClass(Type type, out string baseClassName)
        {
            var baseType = type.BaseType;
            baseClassName = baseType?.Name;

            return baseType != null && baseType != typeof(object) && baseClassName != "BindingModelBase";
        }

        /// <summary>
        /// True for the one namespace the framework itself ships a pre-built, hand-written binding
        /// model under (<c>XrmFramework.Model</c> — <c>EnvironmentVariable</c>, at the time of
        /// writing). <c>XrmFramework</c>'s own <c>.nuspec</c> ships its <c>**\*.cs</c> as
        /// <c>contentFiles</c>, compiled directly into every consuming project's own assembly — so
        /// such a class shows up in <c>Assembly.GetTypes()</c> exactly like a project-authored one,
        /// with no assembly-identity difference to tell them apart. Writing a <c>.model</c> file for
        /// it would, if the project ever merges it in the same way (its own <c>Definitions</c>
        /// directory, say), regenerate a second, incomplete copy of a class the framework already
        /// provides in full — shadowing the real one rather than migrating it.
        /// </summary>
        private static bool IsFrameworkOwnedNamespace(string ns)
            => ns == "XrmFramework.Model" || (ns != null && ns.StartsWith("XrmFramework.Model.", StringComparison.Ordinal));

        /// <summary>
        /// Returns true if [GeneratedCode("XrmFramework", ...)] is present on the type — the same
        /// check <see cref="TableSync.DefinitionAnalyzer"/> uses for *Definition classes.
        /// </summary>
        private static bool IsGeneratedByXrmFramework(Type type)
            => type.GetCustomAttributesData()
                   .Any(a => a.AttributeType.Name == "GeneratedCodeAttribute"
                          && a.ConstructorArguments.Count >= 1
                          && string.Equals(
                                 a.ConstructorArguments[0].Value?.ToString(),
                                 "XrmFramework",
                                 StringComparison.OrdinalIgnoreCase));

        // ──────────────────────────────────────────────────────────────────────────
        //  [CrmEntity]
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Reads the logical name off <c>[CrmEntity("account")]</c> or
        /// <c>[CrmEntity(typeof(AccountDefinition))]</c> — whichever form the class used.
        /// </summary>
        private static string ReadCrmEntityName(CustomAttributeData crmEntityAttr)
        {
            if (crmEntityAttr.ConstructorArguments.Count == 0)
                return null;

            var argument = crmEntityAttr.ConstructorArguments[0];

            return argument.Value is Type definitionType
                ? ReadEntityNameConstant(definitionType)
                : argument.Value as string;
        }

        /// <summary>
        /// Reads the <c>EntityName</c> constant off a generated <c>*Definition</c> class, or returns
        /// <see langword="null"/> when the type is not one — the same technique
        /// <c>CrmEntityAttribute</c>'s own <c>typeof(...)</c> constructor uses at runtime, done here
        /// instead of by constructing the real attribute.
        /// </summary>
        private static string ReadEntityNameConstant(Type definitionType)
        {
            var field = definitionType?.GetField(
                "EntityName", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if (field == null)
                return null;

            try
            {
                return field.IsLiteral ? field.GetRawConstantValue() as string : field.GetValue(null) as string;
            }
            catch
            {
                return null;
            }
        }

        // ──────────────────────────────────────────────────────────────────────────
        //  Properties
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the <see cref="ModelProperty"/> for one property, from whichever of
        /// <c>[ExtendBindingModel]</c>, <c>[ChildRelationship]</c> or <c>[CrmMapping]</c> it carries
        /// — or <see langword="null"/> when it carries none of them, meaning it plays no part in the
        /// mapping (a plain <c>Id</c>, a computed property, ...).
        /// </summary>
        private static ModelProperty ExtractProperty(PropertyInfo property, HashSet<string> usings)
        {
            var attributes = property.GetCustomAttributesData();

            if (attributes.Any(a => a.AttributeType.Name == "ExtendBindingModelAttribute"))
            {
                return new ModelProperty
                {
                    Name = property.Name,
                    TypeFullName = FormatTypeName(property.PropertyType),
                    ExtendBindingModel = true,
                    JsonPropertyName = ReadJsonPropertyName(attributes),
                    JsonIgnore = attributes.Any(a => a.AttributeType.Name == "JsonIgnoreAttribute"),
                    Attrs = ExtractCustomAttributes(attributes, usings)
                };
            }

            var relationshipAttr = attributes.FirstOrDefault(a => a.AttributeType.Name == "ChildRelationshipAttribute");
            if (relationshipAttr != null)
            {
                return ExtractRelationshipProperty(property, relationshipAttr, attributes, usings);
            }

            var mappingAttr = attributes.FirstOrDefault(a => a.AttributeType.Name == "CrmMappingAttribute");
            if (mappingAttr != null)
            {
                return ExtractMappedProperty(property, attributes, mappingAttr, usings);
            }

            return null;
        }

        private static ModelProperty ExtractRelationshipProperty(
            PropertyInfo property, CustomAttributeData relationshipAttr, IList<CustomAttributeData> attributes, HashSet<string> usings)
        {
            var relationshipName = relationshipAttr.ConstructorArguments.Count > 0
                ? relationshipAttr.ConstructorArguments[0].Value as string
                : null;

            if (string.IsNullOrEmpty(relationshipName) || !IsRelationshipCollection(property.PropertyType, out var elementType))
            {
                // Nothing sensible to write: a relationship names no schema, or the property isn't a
                // collection a one-to-many relationship can populate.
                return null;
            }

            return new ModelProperty
            {
                Name = property.Name,
                TypeFullName = $"List<{FormatTypeName(elementType)}>",
                LogicalName = relationshipName,
                IsValidForUpdate = ReadNamedBool(relationshipAttr, "IsValidForUpdate", true),
                Attrs = ExtractCustomAttributes(attributes, usings)
            };
        }

        private static ModelProperty ExtractMappedProperty(
            PropertyInfo property, IList<CustomAttributeData> attributes, CustomAttributeData mappingAttr, HashSet<string> usings)
        {
            var columnLogicalName = mappingAttr.ConstructorArguments.Count > 0
                ? mappingAttr.ConstructorArguments[0].Value as string
                : null;

            if (string.IsNullOrEmpty(columnLogicalName))
                return null;

            var modelProperty = new ModelProperty
            {
                Name = property.Name,
                TypeFullName = FormatTypeName(property.PropertyType),
                LogicalName = columnLogicalName,
                FollowLink = ReadNamedBool(mappingAttr, "FollowLink", false),
                IsValidForUpdate = ReadNamedBool(mappingAttr, "IsValidForUpdate", true),
                JsonPropertyName = ReadJsonPropertyName(attributes),
                JsonIgnore = attributes.Any(a => a.AttributeType.Name == "JsonIgnoreAttribute"),
                Attrs = ExtractCustomAttributes(attributes, usings)
            };

            var lookupAttr = attributes.FirstOrDefault(a => a.AttributeType.Name == "CrmLookupAttribute");
            if (lookupAttr != null)
            {
                ApplyCrmLookup(modelProperty, lookupAttr);
            }
            else if (ImplementsInterfaceNamed(property.PropertyType, "IBindingModel"))
            {
                // No [CrmLookup] and the property's own type is a binding model: the query builder
                // and the mapper resolve the target off that model's own [CrmEntity] — see
                // BindingModelQueryBuilder.AppendProperty — the same thing LookupTargetModel says.
                modelProperty.LookupTargetModel = true;
            }

            return modelProperty;
        }

        /// <summary>
        /// Reads <c>[CrmLookup(target, column, allowNotExisting)]</c> into the property's
        /// disambiguation/projection fields — <c>target</c> in either the string or the
        /// <c>typeof(...)</c> form, same as <c>[CrmEntity]</c>.
        /// </summary>
        private static void ApplyCrmLookup(ModelProperty modelProperty, CustomAttributeData lookupAttr)
        {
            if (lookupAttr.ConstructorArguments.Count < 2)
                return;

            var targetArgument = lookupAttr.ConstructorArguments[0];
            var targetEntityName = targetArgument.Value is Type definitionType
                ? ReadEntityNameConstant(definitionType)
                : targetArgument.Value as string;

            if (!string.IsNullOrEmpty(targetEntityName))
                modelProperty.LookupTargetTableLogicalName = targetEntityName;

            if (lookupAttr.ConstructorArguments[1].Value is string projectedColumn && !string.IsNullOrEmpty(projectedColumn))
                modelProperty.LookupTargetColumnLogicalName = projectedColumn;

            if (lookupAttr.ConstructorArguments.Count > 2 && lookupAttr.ConstructorArguments[2].Value is bool allowNotExisting)
                modelProperty.AllowNotExisting = allowNotExisting;
        }

        // ──────────────────────────────────────────────────────────────────────────
        //  Attrs / Usings — attributes ExtractProperty has no dedicated field for
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Renders every attribute on <paramref name="attributes"/> that <see cref="ExtractProperty"/>
        /// does not already read into a dedicated <see cref="ModelProperty"/> field — a
        /// <c>[StringLength]</c>, a <c>[DataMember]</c>, anything project-specific — into
        /// <see cref="ModelProperty.Attrs"/>, so a round trip through <c>migrate sync-models</c>
        /// carries them into the <c>.model</c> file rather than silently dropping them. Each one's
        /// namespace is added to <paramref name="usings"/> unless <c>ModelSourceFileGenerator</c>
        /// already imports it into every generated file.
        /// </summary>
        private static string[] ExtractCustomAttributes(IList<CustomAttributeData> attributes, HashSet<string> usings)
        {
            List<string> result = null;

            foreach (var attribute in attributes)
            {
                if (RecognizedAttributeNames.Contains(attribute.AttributeType.Name))
                    continue;

                (result ??= new List<string>()).Add(FormatAttributeUsage(attribute));

                var ns = attribute.AttributeType.Namespace;
                if (!string.IsNullOrEmpty(ns) && !DefaultGeneratedUsings.Contains(ns))
                    usings.Add(ns);
            }

            return result?.ToArray();
        }

        /// <summary>
        /// Renders a <see cref="CustomAttributeData"/> the way it would be written back onto the
        /// generated property — e.g. <c>StringLength(100)</c> or <c>DataMember(Name = "Foo")</c>.
        /// Never resolves or instantiates the attribute type, for the same reason the rest of this
        /// class reads by name off raw metadata (see the class remarks).
        /// </summary>
        private static string FormatAttributeUsage(CustomAttributeData attribute)
        {
            var name = attribute.AttributeType.Name;
            if (name.EndsWith("Attribute", StringComparison.Ordinal))
                name = name.Substring(0, name.Length - "Attribute".Length);

            var arguments = new List<string>();

            foreach (var argument in attribute.ConstructorArguments)
                arguments.Add(FormatAttributeArgument(argument));

            foreach (var argument in attribute.NamedArguments)
                arguments.Add($"{argument.MemberName} = {FormatAttributeArgument(argument.TypedValue)}");

            return arguments.Count > 0 ? $"{name}({string.Join(", ", arguments)})" : name;
        }

        /// <summary>
        /// Formats one constructor or named argument as a C# literal — the same handful of shapes an
        /// attribute's compile-time-constant arguments can ever take: a primitive, a string, a
        /// <c>Type</c>, an enum member, or a one-dimensional array of one of those.
        /// </summary>
        private static string FormatAttributeArgument(CustomAttributeTypedArgument argument)
        {
            if (argument.Value is IList<CustomAttributeTypedArgument> arrayValue)
                return "new[] { " + string.Join(", ", arrayValue.Select(FormatAttributeArgument)) + " }";

            if (argument.Value == null)
                return "null";

            if (argument.ArgumentType.IsEnum)
                return $"{argument.ArgumentType.Name}.{argument.Value}";

            return argument.Value switch
            {
                string s => "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"",
                bool b => b ? "true" : "false",
                char c => "'" + c + "'",
                Type t => $"typeof({t.Name})",
                double d => d.ToString(CultureInfo.InvariantCulture),
                float f => f.ToString(CultureInfo.InvariantCulture) + "f",
                _ => Convert.ToString(argument.Value, CultureInfo.InvariantCulture)
            };
        }

        private static bool ReadNamedBool(CustomAttributeData attribute, string argumentName, bool defaultValue)
        {
            foreach (var argument in attribute.NamedArguments)
            {
                if (argument.MemberName == argumentName && argument.TypedValue.Value is bool value)
                    return value;
            }

            return defaultValue;
        }

        /// <summary>Reads the rename off Newtonsoft's <c>[JsonProperty("name")]</c>, positional or named.</summary>
        private static string ReadJsonPropertyName(IList<CustomAttributeData> attributes)
        {
            var jsonPropertyAttr = attributes.FirstOrDefault(a => a.AttributeType.Name == "JsonPropertyAttribute");
            if (jsonPropertyAttr == null)
                return null;

            if (jsonPropertyAttr.ConstructorArguments.Count > 0
                && jsonPropertyAttr.ConstructorArguments[0].Value is string positionalName
                && !string.IsNullOrEmpty(positionalName))
                return positionalName;

            foreach (var argument in jsonPropertyAttr.NamedArguments)
            {
                if (argument.MemberName == "PropertyName" && argument.TypedValue.Value is string namedName
                    && !string.IsNullOrEmpty(namedName))
                    return namedName;
            }

            return null;
        }

        private static bool IsGenericList(Type type, out Type elementType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }

            elementType = null;
            return false;
        }

        /// <summary>
        /// True for any single-type-argument generic assignable to <c>ICollection&lt;T&gt;</c> —
        /// <c>List&lt;T&gt;</c>, but also <c>ICollection&lt;T&gt;</c>, <c>IList&lt;T&gt;</c>,
        /// <c>IReadOnlyCollection&lt;T&gt;</c>, a custom collection type, and so on. The same check
        /// <c>ModelDefinition.IsCollectionProperty</c> uses at runtime to populate a
        /// <c>[ChildRelationship]</c> property off <c>Entity.RelatedEntities</c> — a hand-written
        /// class exposing its relationship through an interface rather than the concrete <c>List</c>
        /// (read-only from the outside, initialized once) works today and must round-trip the same
        /// way, even though the extracted <c>.model</c>'s <c>Cols[].Type</c> always normalizes to
        /// <c>List&lt;T&gt;</c> regardless — the one shape <c>ModelSourceFileGenerator</c> ever emits.
        /// </summary>
        private static bool IsRelationshipCollection(Type type, out Type elementType)
        {
            elementType = null;

            if (!type.IsGenericType)
                return false;

            var typeArguments = type.GetGenericArguments();
            if (typeArguments.Length != 1)
                return false;

            if (!typeof(ICollection<>).MakeGenericType(typeArguments).IsAssignableFrom(type))
                return false;

            elementType = typeArguments[0];
            return true;
        }

        /// <summary>
        /// Renders a <see cref="Type"/> the way a <c>.model</c> file spells it: a primitive by its
        /// C# alias, <c>Nullable&lt;T&gt;</c> as <c>T?</c>, <c>List&lt;T&gt;</c> recursively, a
        /// binding model by its fully qualified name — so it can never collide with a same-named
        /// class in another namespace — and anything else (an option-set enum) by its bare name,
        /// the convention every <c>.model</c> example already follows.
        /// </summary>
        private static string FormatTypeName(Type type)
        {
            var underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null)
                return FormatTypeName(underlying) + "?";

            if (IsGenericList(type, out var elementType))
                return $"List<{FormatTypeName(elementType)}>";

            if (PrimitiveAliases.TryGetValue(type, out var alias))
                return alias;

            if (ImplementsInterfaceNamed(type, "IBindingModel"))
                return type.FullName ?? type.Name;

            return type.Name;
        }
    }
}
