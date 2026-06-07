// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Configuration;
using System.IO;
using System.Xml.Linq;

namespace XrmFramework.DeployUtils.Configuration
{
    public static class ConfigHelper
    {
        /// <summary>
        ///     Configuration chargée explicitement depuis le dossier <c>Config/</c> d'un projet
        ///     (via <see cref="UseProjectConfig" />). Si <see langword="null" />, on retombe sur
        ///     le <see cref="ConfigurationManager" /> applicatif (projets Deploy net462 classiques).
        /// </summary>
        private static System.Configuration.Configuration _projectConfig;

        /// <summary>
        ///     Pointe la lecture de configuration vers <c>Config/xrmFramework.config</c> et
        ///     <c>Config/connectionStrings.config</c> situés sous <paramref name="projectRoot" />.
        /// </summary>
        /// <remarks>
        ///     Permet à un outil autonome (CLI net8.0) de lire la config du projet consommateur
        ///     sans dépendre du fichier App.config applicatif. Les deux fragments sont assemblés
        ///     dans un App.config temporaire chargé via <see cref="ConfigurationManager.OpenMappedExeConfiguration" />
        ///     — aucun état global ni réflexion.
        /// </remarks>
        /// <param name="projectRoot">Racine du projet contenant le dossier <c>Config/</c>.</param>
        /// <exception cref="FileNotFoundException">Si un des deux fichiers de config est absent.</exception>
        public static void UseProjectConfig(string projectRoot)
        {
            var configDir = Path.Combine(projectRoot, "Config");
            var xrmConfigPath = Path.Combine(configDir, "xrmFramework.config");
            var connectionsPath = Path.Combine(configDir, "connectionStrings.config");

            if (!File.Exists(xrmConfigPath))
                throw new FileNotFoundException(
                    $"Fichier de configuration introuvable : {xrmConfigPath}", xrmConfigPath);

            if (!File.Exists(connectionsPath))
                throw new FileNotFoundException(
                    $"Fichier de configuration introuvable : {connectionsPath}", connectionsPath);

            // Les deux fichiers sont des fragments (<xrmFramework .../> et <connectionStrings .../>) :
            // on les inline dans un App.config complet déclarant la section xrmFramework.
            var xrmRoot = XDocument.Load(xrmConfigPath).Root;
            var connectionsRoot = XDocument.Load(connectionsPath).Root;

            var appConfig = new XDocument(
                new XElement("configuration",
                    new XElement("configSections",
                        new XElement("section",
                            new XAttribute("name", "xrmFramework"),
                            new XAttribute("type",
                                "XrmFramework.DeployUtils.Configuration.XrmFrameworkSection, XrmFramework.DeployUtils"))),
                    new XElement(connectionsRoot),
                    new XElement(xrmRoot)));

            var tempConfigPath = Path.Combine(
                Path.GetTempPath(), $"xrmframework-cli-{Guid.NewGuid():N}.config");
            appConfig.Save(tempConfigPath);

            // Nettoyage best-effort en fin de process (le fichier est lu paresseusement).
            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                try { File.Delete(tempConfigPath); } catch { /* ignore */ }
            };

            var map = new ExeConfigurationFileMap { ExeConfigFilename = tempConfigPath };
            _projectConfig = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }

        public static XrmFrameworkSection GetSection()
        {
            return (_projectConfig != null
                ? _projectConfig.GetSection("xrmFramework")
                : ConfigurationManager.GetSection("xrmFramework")) as XrmFrameworkSection;
        }

        public static string GetEntitiesSolutionUniqueName()
        {
            return GetSection().EntitySolution.Name;
        }

        public static string GetSelectedConnectionString()
        {
            var selectedConnection = GetSection().SelectedConnection;

            return _projectConfig != null
                ? _projectConfig.ConnectionStrings.ConnectionStrings[selectedConnection].ConnectionString
                : ConfigurationManager.ConnectionStrings[selectedConnection].ConnectionString;
        }
    }
}
