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
    ///   <item>executed out-of-process by the net462 tool (net8 CLI → deploy plugins);</item>
    ///   <item>executed in-process by the net462 DeployUtils (legacy deployment program).</item>
    /// </list>
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
        public static string BuildManifestJson(string dllPath)
        {
            var fullPath = Path.GetFullPath(dllPath);

            // The plugin's dependencies (Microsoft.Xrm.Sdk, etc.) live next to the DLL.
            var probeDir = Path.GetDirectoryName(fullPath);
            ResolveEventHandler resolver = (_, e) => ResolveFromProbeDir(probeDir, e.Name);
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
            foreach (var type in GetLoadableTypes(assembly))
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                // Order matters: a CustomApi derives from Plugin → test it first.
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

            return ManifestJson.Build(plugins, workflows, customApis);
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
            string displayName = null;
            try
            {
                var instance = Instantiate(type);
                displayName = GetProp(instance, "DisplayName") as string;
            }
            catch
            {
                // DisplayName not resolvable → fall back to the type name.
            }

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

        private static Assembly ResolveFromProbeDir(string probeDir, string assemblyFullName)
        {
            var simpleName = new AssemblyName(assemblyFullName).Name;
            foreach (var ext in new[] { ".dll", ".exe" })
            {
                var candidate = Path.Combine(probeDir, simpleName + ext);
                if (File.Exists(candidate))
                    return Assembly.LoadFrom(candidate);
            }
            return null;
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Surface load failures (missing deps next to the DLL) without blocking the
                // inventory of the types that could be loaded.
                foreach (var le in ex.LoaderExceptions ?? Array.Empty<Exception>())
                    Console.Error.WriteLine("Type load warning: " + Flatten(le));
                return ex.Types.Where(t => t != null);
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
