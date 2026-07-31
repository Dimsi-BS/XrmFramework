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
        ///     Configuration loaded explicitly from a project's <c>Config/</c> folder
        ///     (via <see cref="UseProjectConfig" />). If <see langword="null" />, falls back to
        ///     the application's <see cref="ConfigurationManager" /> (classic net462 Deploy projects).
        /// </summary>
        private static System.Configuration.Configuration _projectConfig;

        /// <summary>
        ///     Points configuration reading to <c>Config/xrmFramework.config</c> and
        ///     <c>Config/connectionStrings.config</c> located under <paramref name="projectRoot" />.
        /// </summary>
        /// <remarks>
        ///     Allows a standalone tool (net10.0 CLI) to read the consumer project's config
        ///     without depending on the application's App.config file. The two fragments are assembled
        ///     into a temporary App.config loaded via <see cref="ConfigurationManager.OpenMappedExeConfiguration" />
        ///     — no global state, no reflection.
        /// </remarks>
        /// <param name="projectRoot">Root of the project containing the <c>Config/</c> folder.</param>
        /// <exception cref="FileNotFoundException">If either of the two config files is missing.</exception>
        public static void UseProjectConfig(string projectRoot)
        {
            var configDir = Path.Combine(projectRoot, "Config");
            var xrmConfigPath = Path.Combine(configDir, "xrmFramework.config");
            var connectionsPath = Path.Combine(configDir, "connectionStrings.config");

            if (!File.Exists(xrmConfigPath))
                throw new FileNotFoundException(
                    $"Configuration file not found: {xrmConfigPath}", xrmConfigPath);

            if (!File.Exists(connectionsPath))
                throw new FileNotFoundException(
                    $"Configuration file not found: {connectionsPath}", connectionsPath);

            // The two files are fragments (<xrmFramework .../> and <connectionStrings .../>):
            // they are inlined into a full App.config declaring the xrmFramework section.
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

            // Best-effort cleanup at process exit (the file is read lazily).
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
