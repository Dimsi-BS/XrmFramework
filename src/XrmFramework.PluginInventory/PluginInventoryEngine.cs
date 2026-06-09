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
    /// Moteur d'inventaire : charge une assembly plugin XrmFramework, EXÉCUTE le code
    /// d'enregistrement (constructeurs / AddSteps) et reflète les types pour produire le
    /// manifeste JSON (plugins/steps/workflows/custom APIs).
    ///
    /// Le schéma émis est exactement celui lu par
    /// <c>XrmFramework.DeployUtils.Factories.PluginInventoryReader</c>.
    ///
    /// Ce code est partagé (source liée) :
    /// <list type="bullet">
    ///   <item>exécuté hors-process par l'outil net462 (CLI net8 → deploy plugins) ;</item>
    ///   <item>exécuté in-process par DeployUtils net462 (programme de déploiement legacy).</item>
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
        /// Charge l'assembly située à <paramref name="dllPath" /> et renvoie le manifeste JSON.
        /// </summary>
        public static string BuildManifestJson(string dllPath)
        {
            var fullPath = Path.GetFullPath(dllPath);

            // Les dépendances du plugin (Microsoft.Xrm.Sdk, etc.) résident à côté de la DLL.
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

            // Classification par héritage sur le NOM COMPLET du type de base : robuste que les
            // types de base XrmFramework soient compilés dans l'assembly (package source) ou
            // fournis par un XrmFramework.Plugin.dll référencé (ProjectReference).
            foreach (var type in GetLoadableTypes(assembly))
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                // Ordre important : un CustomApi dérive de Plugin → le tester avant.
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
        // Plugins : on instancie (le ctor déclenche AddSteps) et on lit les vrais Step.
        // Toute la logique d'images / filtering de Step.cs est donc appliquée nativement.
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
                    $"Échec d'instanciation du plugin '{type.FullName}' : {Flatten(ex)}", ex);
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
        // Workflows : on instancie et on lit DisplayName (repli sur le nom court du type).
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
                // DisplayName non résoluble → repli sur le nom du type.
            }

            if (string.IsNullOrEmpty(displayName))
                displayName = type.Name;

            return new WorkflowInfo(type.FullName, displayName);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Custom APIs : purement déclaratif (attributs + types des propriétés),
        // pas besoin d'instancier. Le schéma porte typeFullName + isEnum.
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

                // Type générique T de CustomApiIn/OutArgument<T>.
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
            // Plugin / CustomApi : ctor (unsecuredConfig, securedConfig) appelé avec (null, null).
            var ctor = type.GetConstructor(Instance, null, new[] { typeof(string), typeof(string) }, null);
            if (ctor != null)
                return ctor.Invoke(new object[] { null, null });

            // Sinon ctor sans paramètre (workflows), public ou non.
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
                // Surfacer les causes de chargement (deps manquantes à côté de la DLL) sans bloquer
                // l'inventaire des types qui ont pu être chargés.
                foreach (var le in ex.LoaderExceptions ?? Array.Empty<Exception>())
                    Console.Error.WriteLine("Avertissement chargement de type : " + Flatten(le));
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
