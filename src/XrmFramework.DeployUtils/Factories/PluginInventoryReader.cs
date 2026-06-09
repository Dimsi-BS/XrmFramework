// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Factories
{
    /// <summary>
    ///     Désérialise le JSON d'inventaire produit par <c>XrmFramework.PluginInventory</c> (qui exécute
    ///     le code d'enregistrement des plugins) et le mappe vers le modèle de déploiement
    ///     (<see cref="Plugin" />, <see cref="Step" />, <see cref="CustomApi" />).
    /// </summary>
    public static class PluginInventoryReader
    {
        /// <summary>
        ///     Désérialise le manifeste JSON et mappe les plugins (et leurs steps) vers le modèle.
        /// </summary>
        public static IReadOnlyList<Plugin> ReadPlugins(string manifestJson)
        {
            var manifest = JsonConvert.DeserializeObject<ManifestDto>(manifestJson) ?? new ManifestDto();

            var result = new List<Plugin>();
            foreach (var pluginDto in manifest.Plugins)
                result.Add(MapPlugin(pluginDto));

            return result;
        }

        /// <summary>
        ///     Désérialise le manifeste et mappe les workflows vers le modèle (forme « plugin workflow »).
        /// </summary>
        public static IReadOnlyList<Plugin> ReadWorkflows(string manifestJson)
        {
            var manifest = JsonConvert.DeserializeObject<ManifestDto>(manifestJson) ?? new ManifestDto();

            var result = new List<Plugin>();
            foreach (var wf in manifest.Workflows)
                result.Add(new Plugin(wf.FullName, wf.DisplayName));

            return result;
        }

        /// <summary>
        ///     Désérialise le manifeste et mappe les custom APIs (et leurs arguments) vers le modèle.
        /// </summary>
        /// <param name="manifestJson">Le manifeste JSON.</param>
        /// <param name="customizationPrefix">
        ///     Préfixe de personnalisation du publisher (résolu depuis l'environnement connecté) ;
        ///     entre dans le <see cref="CustomApi.UniqueName" /> (<c>prefix_name</c>).
        /// </param>
        public static IReadOnlyList<CustomApi> ReadCustomApis(string manifestJson, string customizationPrefix)
        {
            var manifest = JsonConvert.DeserializeObject<ManifestDto>(manifestJson) ?? new ManifestDto();

            var result = new List<CustomApi>();
            foreach (var dto in manifest.CustomApis)
                result.Add(MapCustomApi(dto, customizationPrefix));

            return result;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Mapping DTO → modèle (réplique FromXrmFrameworkPlugin / FromXrmFrameworkStep)
        // ──────────────────────────────────────────────────────────────────────

        private static Plugin MapPlugin(PluginDto dto)
        {
            var plugin = new Plugin(dto.FullName);
            var simpleName = SimpleName(dto.FullName);

            foreach (var stepDto in dto.Steps)
                plugin.Steps.Add(MapStep(dto.FullName, simpleName, stepDto));

            return plugin;
        }

        private static Step MapStep(string pluginFullName, string pluginSimpleName, StepDto dto)
        {
            var step = new Step(
                pluginSimpleName,
                Messages.GetMessage(dto.Message),
                (Stages)Enum.Parse(typeof(Stages), dto.Stage),
                (Modes)Enum.Parse(typeof(Modes), dto.Mode),
                dto.EntityName)
            {
                PluginTypeFullName = pluginFullName
            };

            if (dto.FilteringAttributes != null)
                step.FilteringAttributes.UnionWith(dto.FilteringAttributes);

            step.ImpersonationUsername = dto.ImpersonationUsername ?? "";
            step.Order = dto.Order;

            if (dto.PreImage != null)
            {
                step.PreImage.AllAttributes = dto.PreImage.AllAttributes;
                if (dto.PreImage.Attributes != null)
                    step.PreImage.Attributes.UnionWith(dto.PreImage.Attributes);
            }

            if (dto.PostImage != null)
            {
                step.PostImage.AllAttributes = dto.PostImage.AllAttributes;
                if (dto.PostImage.Attributes != null)
                    step.PostImage.Attributes.UnionWith(dto.PostImage.Attributes);
            }

            if (!string.IsNullOrWhiteSpace(dto.UnsecureConfig))
                step.StepConfiguration = JsonConvert.DeserializeObject<StepConfiguration>(dto.UnsecureConfig);

            if (dto.MethodNames != null)
                step.MethodNames.UnionWith(dto.MethodNames);

            return step;
        }

        private static string SimpleName(string fullName)
        {
            var lastDot = fullName.LastIndexOf('.');
            return lastDot < 0 ? fullName : fullName.Substring(lastDot + 1);
        }

        // ── Custom API (réplique FromXrmFrameworkCustomApi / FromXrmFrameworkArgument) ──

        private static CustomApi MapCustomApi(CustomApiDto dto, string prefix)
        {
            var name = string.IsNullOrWhiteSpace(dto.Name) ? SimpleName(dto.FullName) : dto.Name;

            var customApi = new CustomApi
            {
                FullName = dto.FullName,
                Name = name,
                Prefix = prefix,
                DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? name : dto.DisplayName,
                Description = string.IsNullOrWhiteSpace(dto.Description) ? name : dto.Description,
                BindingType = new OptionSetValue(BindingTypeValue(dto.BindingType)),
                AllowedCustomProcessingStepType = new OptionSetValue(AllowedProcessingValue(dto.AllowedCustomProcessing)),
                BoundEntityLogicalName = dto.BoundEntityLogicalName,
                ExecutePrivilegeName = dto.ExecutePrivilegeName,
                IsFunction = dto.IsFunction,
                IsPrivate = dto.IsPrivate,
                WorkflowSdkStepEnabled = dto.WorkflowSdkStepEnabled
            };

            foreach (var arg in dto.Arguments)
                customApi.AddChild(MapArgument(name, arg));

            return customApi;
        }

        private static ICrmComponent MapArgument(string customApiName, ArgDto arg)
        {
            var unique = $"{customApiName}.{arg.Name}";
            var type = new OptionSetValue(ResolveArgumentType(arg.TypeFullName, arg.IsEnum));

            if (arg.IsInArgument)
                return new CustomApiRequestParameter
                {
                    Name = arg.Name,
                    UniqueName = unique,
                    DisplayName = string.IsNullOrWhiteSpace(arg.DisplayName) ? unique : arg.DisplayName,
                    Description = string.IsNullOrWhiteSpace(arg.Description) ? unique : arg.Description,
                    Type = type,
                    IsOptional = arg.IsOptional
                };

            return new CustomApiResponseProperty
            {
                Name = arg.Name,
                UniqueName = unique,
                DisplayName = string.IsNullOrWhiteSpace(arg.DisplayName) ? unique : arg.DisplayName,
                Description = string.IsNullOrWhiteSpace(arg.Description) ? unique : arg.Description,
                Type = type
            };
        }

        // Les enums custom API (CustomApiArgumentType / CustomApiBindingType /
        // AllowedCustomProcessingStep) ne sont pas compilés dans DeployUtils — comme
        // FromXrmFrameworkCustomApi (qui passe par dynamic), on travaille avec leurs valeurs int.

        /// <summary>
        ///     Mappe un nom de type .NET (et son caractère enum) vers la valeur int d'un
        ///     <c>CustomApiArgumentType</c>. Réplique <c>CustomApiArgumentTypeMapper</c> par nom.
        /// </summary>
        private static int ResolveArgumentType(string typeFullName, bool isEnum)
        {
            if (isEnum) return 9; // Picklist

            switch (typeFullName)
            {
                case "System.Boolean": return 0;                         // Boolean
                case "System.DateTime": return 1;                        // DateTime
                case "System.Decimal": return 2;                         // Decimal
                case "Microsoft.Xrm.Sdk.Entity": return 3;               // Entity
                case "Microsoft.Xrm.Sdk.EntityCollection": return 4;     // EntityCollection
                case "Microsoft.Xrm.Sdk.EntityReference": return 5;      // EntityReference
                case "System.Single": return 6;                          // Float
                case "System.Int32": return 7;                           // Integer
                case "Microsoft.Xrm.Sdk.Money": return 8;                // Money
                case "Microsoft.Xrm.Sdk.OptionSetValue": return 9;       // Picklist
                case "System.String": return 10;                         // String
                case "System.String[]": return 11;                       // StringArray
                case "System.Guid": return 12;                           // Guid
                default: return 10;                                      // non mappé → String (sérialisé)
            }
        }

        private static int BindingTypeValue(string name)
        {
            switch (name)
            {
                case "Entity": return 1;
                case "EntityCollection": return 2;
                default: return 0; // Global
            }
        }

        private static int AllowedProcessingValue(string name)
        {
            switch (name)
            {
                case "AsyncOnly": return 1;
                case "SyncAndAsync": return 2;
                default: return 0; // None
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // DTO (schéma du manifeste)
        // ──────────────────────────────────────────────────────────────────────

        private sealed class ManifestDto
        {
            [JsonProperty("plugins")] public List<PluginDto> Plugins { get; set; } = new List<PluginDto>();
            [JsonProperty("workflows")] public List<WorkflowDto> Workflows { get; set; } = new List<WorkflowDto>();
            [JsonProperty("customApis")] public List<CustomApiDto> CustomApis { get; set; } = new List<CustomApiDto>();
        }

        private sealed class WorkflowDto
        {
            [JsonProperty("fullName")] public string FullName { get; set; }
            [JsonProperty("displayName")] public string DisplayName { get; set; }
        }

        private sealed class CustomApiDto
        {
            [JsonProperty("fullName")] public string FullName { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("displayName")] public string DisplayName { get; set; }
            [JsonProperty("description")] public string Description { get; set; }
            [JsonProperty("bindingType")] public string BindingType { get; set; }
            [JsonProperty("boundEntityLogicalName")] public string BoundEntityLogicalName { get; set; }
            [JsonProperty("isFunction")] public bool IsFunction { get; set; }
            [JsonProperty("isPrivate")] public bool IsPrivate { get; set; }
            [JsonProperty("allowedCustomProcessing")] public string AllowedCustomProcessing { get; set; }
            [JsonProperty("executePrivilegeName")] public string ExecutePrivilegeName { get; set; }
            [JsonProperty("workflowSdkStepEnabled")] public bool WorkflowSdkStepEnabled { get; set; }
            [JsonProperty("arguments")] public List<ArgDto> Arguments { get; set; } = new List<ArgDto>();
        }

        private sealed class ArgDto
        {
            [JsonProperty("isInArgument")] public bool IsInArgument { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
            [JsonProperty("typeFullName")] public string TypeFullName { get; set; }
            [JsonProperty("isEnum")] public bool IsEnum { get; set; }
            [JsonProperty("displayName")] public string DisplayName { get; set; }
            [JsonProperty("description")] public string Description { get; set; }
            [JsonProperty("logicalEntityName")] public string LogicalEntityName { get; set; }
            [JsonProperty("isOptional")] public bool IsOptional { get; set; }
        }

        private sealed class PluginDto
        {
            [JsonProperty("fullName")] public string FullName { get; set; }
            [JsonProperty("steps")] public List<StepDto> Steps { get; set; } = new List<StepDto>();
        }

        private sealed class StepDto
        {
            [JsonProperty("message")] public string Message { get; set; }
            [JsonProperty("stage")] public string Stage { get; set; }
            [JsonProperty("mode")] public string Mode { get; set; }
            [JsonProperty("entityName")] public string EntityName { get; set; }
            [JsonProperty("methodName")] public string MethodName { get; set; }
            [JsonProperty("methodNames")] public List<string> MethodNames { get; set; }
            [JsonProperty("filteringAttributes")] public List<string> FilteringAttributes { get; set; }
            [JsonProperty("order")] public int Order { get; set; }
            [JsonProperty("impersonationUsername")] public string ImpersonationUsername { get; set; }
            [JsonProperty("unsecureConfig")] public string UnsecureConfig { get; set; }
            [JsonProperty("preImage")] public ImageDto PreImage { get; set; }
            [JsonProperty("postImage")] public ImageDto PostImage { get; set; }
        }

        private sealed class ImageDto
        {
            [JsonProperty("allAttributes")] public bool AllAttributes { get; set; }
            [JsonProperty("attributes")] public List<string> Attributes { get; set; }
        }
    }
}
