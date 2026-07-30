// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Inventaire des tables décrites par les fichiers <c>.table</c> livrés avec le package
    /// XrmFramework (<c>src/XrmFramework/Definitions</c>).
    ///
    /// Ces fichiers sont ajoutés en <c>AdditionalFiles</c> au projet consommateur : le
    /// générateur Roslyn y produit <c>SystemUserDefinition</c>, <c>RoleDefinition</c>, ... qui se
    /// retrouvent donc dans l'assembly analysé par <c>tables sync</c>. Sans ce filtre, la commande
    /// déposerait dans le répertoire <c>Definitions</c> du projet des doublons
    /// (<c>SystemUser.table</c>, <c>Role.table</c>, ...) des fichiers déjà fournis par le framework.
    ///
    /// Le filtre ne porte que sur la <b>création</b> : un projet peut légitimement suivre sa propre
    /// copie d'une table du framework pour y déclarer des colonnes supplémentaires — celles du
    /// framework y étant marquées <c>"Locked": true</c>. Dès que le fichier existe,
    /// <see cref="TableFileSyncer"/> le met à jour comme n'importe quel autre.
    ///
    /// La liste est figée ici plutôt que déduite par réflexion de l'assembly courant :
    /// XrmFramework.DeployUtils compile aussi ses propres <c>.table</c> (Publisher,
    /// SolutionComponent, WebResource, Pluginpackage) qui, eux, ne sont pas livrés aux projets
    /// consommateurs et doivent donc rester synchronisables.
    /// <c>FrameworkTableCatalogTests</c> vérifie que cet inventaire correspond exactement aux
    /// fichiers présents dans le dépôt.
    /// </summary>
    public static class FrameworkTableCatalog
    {
        /// <summary>
        /// Couple (nom de table, nom logique) d'un <c>.table</c> livré par le framework.
        /// Le nom de table est celui du JSON (<c>Name</c>), pas celui du fichier : c'est lui qui
        /// donne son nom à la classe générée, donc au <see cref="DefinitionInfo.TableName"/> vu
        /// par <c>tables sync</c> (<c>Systemuser.table</c> déclare ainsi <c>SystemUser</c>).
        /// </summary>
        private sealed class FrameworkTable
        {
            public FrameworkTable(string name, string logicalName)
            {
                Name = name;
                LogicalName = logicalName;
            }

            public string Name { get; }

            public string LogicalName { get; }
        }

        private static readonly FrameworkTable[] ShippedTables =
        {
            new FrameworkTable("BusinessUnit",                        "businessunit"),
            new FrameworkTable("CustomApi",                           "customapi"),
            new FrameworkTable("CustomApiRequestParameter",           "customapirequestparameter"),
            new FrameworkTable("CustomApiResponseProperty",           "customapiresponseproperty"),
            new FrameworkTable("DebugSession",                        "dimsi_debugsession"),
            new FrameworkTable("EnvironmentVariable",                 "environmentvariabledefinition"),
            new FrameworkTable("EnvironmentVariableValue",            "environmentvariablevalue"),
            new FrameworkTable("OptionSet",                           "globalEnums"),
            new FrameworkTable("PluginAssembly",                      "pluginassembly"),
            new FrameworkTable("PluginType",                          "plugintype"),
            new FrameworkTable("Role",                                "role"),
            new FrameworkTable("SdkMessage",                          "sdkmessage"),
            new FrameworkTable("SdkMessageFilter",                    "sdkmessagefilter"),
            new FrameworkTable("SdkMessageProcessingStep",            "sdkmessageprocessingstep"),
            new FrameworkTable("SdkMessageProcessingStepImage",       "sdkmessageprocessingstepimage"),
            new FrameworkTable("SdkMessageProcessingStepSecureConfig", "sdkmessageprocessingstepsecureconfig"),
            new FrameworkTable("Solution",                            "solution"),
            new FrameworkTable("SystemUser",                          "systemuser"),
            new FrameworkTable("SystemUserRoles",                     "systemuserroles"),
            new FrameworkTable("Team",                                "team"),
            new FrameworkTable("Workflow",                            "workflow")
        };

        private static readonly HashSet<string> Names =
            new HashSet<string>(ShippedTables.Select(t => t.Name), StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<string> LogicalNamesSet =
            new HashSet<string>(ShippedTables.Select(t => t.LogicalName), StringComparer.OrdinalIgnoreCase);

        /// <summary>Noms de table (<c>Name</c>) des <c>.table</c> livrés par le framework.</summary>
        public static IReadOnlyCollection<string> TableNames => Names;

        /// <summary>Noms logiques (<c>LogName</c>) des <c>.table</c> livrés par le framework.</summary>
        public static IReadOnlyCollection<string> LogicalNames => LogicalNamesSet;

        /// <summary>
        /// Vrai si la Definition décrit une table livrée par le framework.
        /// </summary>
        public static bool IsFrameworkTable(DefinitionInfo definition)
            => definition != null
            && IsFrameworkTable(definition.TableName, definition.EntityName);

        /// <summary>
        /// Vrai si l'un des identifiants fournis désigne une table livrée par le framework.
        /// Les deux sont testés indépendamment : un <c>.table</c> renommé côté projet reste
        /// reconnaissable par son nom logique, et un fichier dont le contenu est illisible reste
        /// reconnaissable par son nom. <c>null</c> est accepté pour l'identifiant inconnu.
        /// </summary>
        public static bool IsFrameworkTable(string tableName, string logicalName)
            => (!string.IsNullOrEmpty(tableName) && Names.Contains(tableName))
            || (!string.IsNullOrEmpty(logicalName) && LogicalNamesSet.Contains(logicalName));
    }
}
