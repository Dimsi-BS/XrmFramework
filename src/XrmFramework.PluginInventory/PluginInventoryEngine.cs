// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XrmFramework.PluginInventory
{
    /// <summary>
    /// Inventory engine: loads an XrmFramework plugin assembly, EXECUTES the registration
    /// code (constructors / AddSteps) and reflects over the types to produce the JSON
    /// manifest (plugins/steps/workflows/custom APIs).
    ///
    /// The emitted schema is exactly the one read by
    /// <c>XrmFramework.DeployUtils.Factories.PluginInventoryReader</c>.
    ///
    /// This code is shared (linked source):
    /// <list type="bullet">
    ///   <item>executed out-of-process by the net10 inventory host (CLI -> deploy plugins);</item>
    ///   <item>executed in-process by the net462/net48 DeployUtils (legacy deployment program).</item>
    /// </list>
    /// It is therefore TFM-agnostic: pure reflection, read by property name, never casting to a
    /// type of the inspected assembly. Everything a particular runtime needs to make the load
    /// succeed is passed in by its host - see the <c>hostSuppliedAssemblies</c> argument of
    /// <see cref="BuildManifestJson(string, IReadOnlyDictionary{string, string})" />.
    /// </summary>
    internal static class PluginInventoryEngine
    {
        private const string PluginTypeName = "XrmFramework.Plugin";
        private const string CustomApiTypeName = "XrmFramework.CustomApi";
        private const string WorkflowTypeName = "XrmFramework.Workflow.CustomWorkflowActivity";
        private const string CustomApiAttributeName = "XrmFramework.CustomApiAttribute";
        private const string CustomApiInputAttributeName = "XrmFramework.CustomApiInputAttribute";
        private const string CustomApiOutputAttributeName = "XrmFramework.CustomApiOutputAttribute";

        private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Loads the assembly located at <paramref name="dllPath" /> and returns the JSON manifest.
        /// </summary>
        /// <param name="dllPath">The plugin assembly to describe.</param>
        /// <param name="hostSuppliedAssemblies">
        ///     Assemblies the HOST provides itself, by simple name, mapped to the file to load. Consulted
        ///     before the probe directory. The .NET-modern host uses it for <c>System.Activities</c>,
        ///     which does not exist on .NET 5+ and whose stand-in ships next to the host; .NET Framework
        ///     callers pass nothing and get the GAC's copy as before.
        /// </param>
        public static string BuildManifestJson(
            string dllPath,
            IReadOnlyDictionary<string, string> hostSuppliedAssemblies = null)
        {
            var fullPath = Path.GetFullPath(dllPath);

            // The plugin's dependencies (Microsoft.Xrm.Sdk, etc.) live next to the DLL.
            var probeDir = Path.GetDirectoryName(fullPath);
            ResolveEventHandler resolver = (_, e) => Resolve(probeDir, hostSuppliedAssemblies, e.Name);
            AppDomain.CurrentDomain.AssemblyResolve += resolver;
            try
            {
                var assembly = Assembly.LoadFrom(fullPath);
                return BuildManifest(assembly);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= resolver;
            }
        }

        private static string BuildManifest(Assembly assembly)
        {
            var plugins = new List<PluginInfo>();
            var workflows = new List<WorkflowInfo>();
            var customApis = new List<CustomApiInfo>();

            // Classification by inheritance on the FULL NAME of the base type: robust whether
            // the XrmFramework base types are compiled into the assembly (source package) or
            // provided by a referenced XrmFramework.Plugin.dll (ProjectReference).
            foreach (var type in GetTypes(assembly))
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                // Order matters: a CustomApi derives from Plugin -> test it first.
                if (InheritsFrom(type, WorkflowTypeName))
                {
                    workflows.Add(ExtractWorkflow(type));
                    continue;
                }

                if (InheritsFrom(type, CustomApiTypeName))
                {
                    customApis.Add(ExtractCustomApi(type));
                    continue;
                }

                if (InheritsFrom(type, PluginTypeName))
                    plugins.Add(ExtractPlugin(type));
            }

            return ManifestJson.Build(plugins, workflows, customApis, UsesPluginPackage(assembly));
        }

        /// <summary>
        ///     Whether the assembly was built as a Dataverse plugin package, read from the
        ///     <c>XrmFrameworkPluginPackage</c> assembly metadata stamped by the packaging targets.
        /// </summary>
        /// <remarks>
        ///     Reported here because this is the one place that already has the assembly loaded: the
        ///     deployment side deliberately reads a plugin DLL's identity without loading it, so that
        ///     a net8 host can describe a net462 assembly.
        /// </remarks>
        private static bool UsesPluginPackage(Assembly assembly)
        {
            try
            {
                foreach (AssemblyMetadataAttribute metadata in
                    assembly.GetCustomAttributes(typeof(AssemblyMetadataAttribute), false))
                {
                    if (string.Equals(metadata.Key, "XrmFrameworkPluginPackage", StringComparison.OrdinalIgnoreCase))
                        return string.Equals(metadata.Value, "true", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception)
            {
                // An unreadable attribute must not cost the whole inventory: the deployment then
                // falls back on the deployment= attribute in xrmFramework.config.
            }

            return false;
        }

        private static bool InheritsFrom(Type type, string baseFullName)
        {
            for (var current = type.BaseType; current != null; current = current.BaseType)
                if (current.FullName == baseFullName)
                    return true;
            return false;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Plugins: instantiate (the ctor triggers AddSteps) and read the real Steps.
        // All the image/filtering logic from Step.cs is thus applied natively.
        // ──────────────────────────────────────────────────────────────────────

        private static PluginInfo ExtractPlugin(Type type)
        {
            object instance;
            try
            {
                instance = Instantiate(type);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to instantiate plugin '{type.FullName}': {Flatten(ex)}", ex);
            }

            var steps = new List<StepInfo>();
            foreach (var step in AsEnumerable(GetProp(instance, "Steps")))
                steps.Add(ExtractStep(step));

            return new PluginInfo(type.FullName, steps);
        }

        private static StepInfo ExtractStep(object step)
        {
            var method = GetProp(step, "Method") as MethodInfo;
            return new StepInfo
            {
                Message = GetProp(step, "Message")?.ToString() ?? "",
                Stage = GetProp(step, "Stage")?.ToString() ?? "",
                Mode = GetProp(step, "Mode")?.ToString() ?? "",
                EntityName = GetProp(step, "EntityName") as string ?? "",
                MethodName = method?.Name ?? "",
                MethodNames = AsStringList(GetProp(step, "MethodNames")),
                FilteringAttributes = AsStringList(GetProp(step, "FilteringAttributes")),
                PreImageAll = GetProp(step, "PreImageAllAttributes") as bool? ?? false,
                PreImageAttributes = AsStringList(GetProp(step, "PreImageAttributes")),
                PostImageAll = GetProp(step, "PostImageAllAttributes") as bool? ?? false,
                PostImageAttributes = AsStringList(GetProp(step, "PostImageAttributes")),
                Order = GetProp(step, "Order") as int? ?? 1,
                ImpersonationUsername = GetProp(step, "ImpersonationUsername") as string ?? "",
                UnsecureConfig = GetProp(step, "UnsecureConfig") as string,
            };
        }

        // ──────────────────────────────────────────────────────────────────────
        // Workflows: instantiate and read DisplayName (fall back to the type's short name).
        // ──────────────────────────────────────────────────────────────────────

        private static WorkflowInfo ExtractWorkflow(Type type)
        {
            // A failure here used to be swallowed and the type's short name reported instead. That
            // silently shipped a WRONG workflow display name - Dataverse registers the plugintype
            // under it - with nothing in the output to say so. The inventory describes what will be
            // deployed, so it has to fail rather than guess.
            object instance;
            try
            {
                instance = Instantiate(type);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to instantiate workflow activity '{type.FullName}': {Flatten(ex)}", ex);
            }

            var displayName = GetProp(instance, "DisplayName") as string;

            // Still a legitimate fallback: an activity that never sets a display name is named after
            // its type, which is what .NET Framework's System.Activities does on its own.
            if (string.IsNullOrEmpty(displayName))
                displayName = type.Name;

            return new WorkflowInfo(type.FullName, displayName);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Custom APIs: purely declarative (attributes + property types),
        // no need to instantiate. The schema carries typeFullName + isEnum.
        // ──────────────────────────────────────────────────────────────────────

        private static CustomApiInfo ExtractCustomApi(Type type)
        {
            var attr = type.GetCustomAttributes(false)
                .FirstOrDefault(a => a.GetType().FullName == CustomApiAttributeName);

            var name = GetProp(attr, "Name") as string;
            var info = new CustomApiInfo
            {
                FullName = type.FullName,
                Name = string.IsNullOrEmpty(name) ? type.Name : name,
                DisplayName = GetProp(attr, "DisplayName") as string,
                Description = GetProp(attr, "Description") as string,
                BoundEntityLogicalName = GetProp(attr, "BoundEntityLogicalName") as string,
                ExecutePrivilegeName = GetProp(attr, "ExecutePrivilegeName") as string,
                IsFunction = GetProp(attr, "IsFunction") as bool? ?? false,
                IsPrivate = GetProp(attr, "IsPrivate") as bool? ?? false,
                WorkflowSdkStepEnabled = GetProp(attr, "WorkflowSdkStepEnabled") as bool? ?? false,
                BindingType = GetProp(attr, "BindingType")?.ToString(),
                AllowedCustomProcessing = GetProp(attr, "AllowedCustomProcessing")?.ToString(),
            };

            foreach (var prop in EnumerateProperties(type))
            {
                var inAttr = prop.GetCustomAttributes()
                    .FirstOrDefault(a => a.GetType().FullName == CustomApiInputAttributeName);
                var outAttr = prop.GetCustomAttributes()
                    .FirstOrDefault(a => a.GetType().FullName == CustomApiOutputAttributeName);
                var argAttr = inAttr ?? outAttr;
                if (argAttr == null)
                    continue;

                // Generic type T of CustomApiIn/OutArgument<T>.
                var typeArg = prop.PropertyType.IsGenericType
                    ? prop.PropertyType.GetGenericArguments().FirstOrDefault()
                    : null;

                info.Arguments.Add(new ArgInfo
                {
                    IsIn = inAttr != null,
                    Name = GetProp(argAttr, "Name") as string ?? prop.Name,
                    TypeFullName = typeArg?.FullName ?? "",
                    IsEnum = typeArg?.IsEnum ?? false,
                    DisplayName = GetProp(argAttr, "DisplayName") as string,
                    Description = GetProp(argAttr, "Description") as string,
                    LogicalEntityName = GetProp(argAttr, "LogicalEntityName") as string,
                    IsOptional = GetProp(argAttr, "IsOptional") as bool? ?? false,
                });
            }

            return info;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────────────────────────────

        private static object Instantiate(Type type)
        {
            // Plugin / CustomApi: ctor (unsecuredConfig, securedConfig) invoked with (null, null).
            var ctor = type.GetConstructor(Instance, null, new[] { typeof(string), typeof(string) }, null);
            if (ctor != null)
                return ctor.Invoke(new object[] { null, null });

            // Otherwise parameterless ctor (workflows), public or not.
            return Activator.CreateInstance(type, nonPublic: true);
        }

        private static IEnumerable<PropertyInfo> EnumerateProperties(Type type)
        {
            for (var current = type; current != null && current != typeof(object); current = current.BaseType)
                foreach (var prop in current.GetProperties(Instance | BindingFlags.DeclaredOnly))
                    yield return prop;
        }

        private static object GetProp(object instance, string name)
        {
            if (instance == null) return null;
            var prop = instance.GetType().GetProperty(name, Instance);
            return prop?.GetValue(instance);
        }

        private static IEnumerable AsEnumerable(object value)
            => value as IEnumerable ?? Array.Empty<object>();

        private static List<string> AsStringList(object value)
        {
            var result = new List<string>();
            foreach (var item in AsEnumerable(value))
                if (item != null)
                    result.Add(item.ToString());
            return result;
        }

        /// <summary>
        ///     Resolves a dependency of the plugin assembly: what the host supplies itself first, then
        ///     the directory the plugin assembly sits in.
        /// </summary>
        /// <remarks>
        ///     The host's own list wins deliberately, rather than letting the probe directory be tried
        ///     first and falling back when it throws. A plugin's output directory ALWAYS contains a
        ///     <c>System.Activities.dll</c> - the plugin props copy it local - and it is always a
        ///     reference assembly, which .NET modern refuses to load. That is a known, predictable
        ///     shape, not an anomaly, so it should not be driven by exception handling. It also means a
        ///     directory that happens to hold a loadable-but-wrong <c>System.Activities</c> (someone
        ///     dropped CoreWF in it) cannot silently win over the stand-in the host shipped.
        /// </remarks>
        private static Assembly Resolve(
            string probeDir, IReadOnlyDictionary<string, string> hostSuppliedAssemblies, string assemblyFullName)
        {
            var simpleName = new AssemblyName(assemblyFullName).Name;

            string hostSupplied;
            if (hostSuppliedAssemblies != null
                    && hostSuppliedAssemblies.TryGetValue(simpleName, out hostSupplied)
                    && File.Exists(hostSupplied))
                return Assembly.LoadFrom(hostSupplied);

            foreach (var ext in new[] { ".dll", ".exe" })
            {
                var candidate = Path.Combine(probeDir, simpleName + ext);
                if (!File.Exists(candidate))
                    continue;

                try
                {
                    return Assembly.LoadFrom(candidate);
                }
                catch (BadImageFormatException)
                {
                    // A reference assembly copied next to the plugin (the plugin props do this for
                    // System.Activities). It carries metadata but no IL, so a modern runtime cannot
                    // load it. Keep looking rather than failing the whole inventory - and let the
                    // caller see an unresolved dependency if nothing else answers.
                }
            }

            return null;
        }

        /// <summary>
        ///     All the types of the assembly, or a failure naming what could not be loaded.
        /// </summary>
        /// <remarks>
        ///     A type that fails to load used to produce a warning on stderr and be skipped. That is
        ///     the most dangerous outcome this tool can have: a missing dependency takes out exactly
        ///     the types that depend on it, so a whole set of plugins, custom APIs or workflow
        ///     activities would DISAPPEAR from the manifest - and the deployment, reading a manifest
        ///     that simply does not mention them, would treat them as removed and happily unregister
        ///     their steps. An incomplete inventory is indistinguishable from a deliberate deletion,
        ///     so it must not be reported as a success.
        /// </remarks>
        private static IEnumerable<Type> GetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                var reasons = (ex.LoaderExceptions ?? Array.Empty<Exception>())
                    .Where(e => e != null)
                    .Select(Flatten)
                    .Distinct()
                    .ToList();

                throw new InvalidOperationException(
                    $"Some types of '{assembly.Location}' could not be loaded, so the inventory would " +
                    "be incomplete. A dependency is most likely missing next to the assembly." +
                    Environment.NewLine + string.Join(Environment.NewLine, reasons), ex);
            }
        }

        internal static string Flatten(Exception ex)
        {
            var sb = new StringBuilder();
            for (var e = ex; e != null; e = e.InnerException)
                sb.AppendLine($"{e.GetType().Name}: {e.Message}");
            return sb.ToString().TrimEnd();
        }
    }
}
