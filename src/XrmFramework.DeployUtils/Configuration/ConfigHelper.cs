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

        /// <remarks>
        ///     Deliberately not routed through <see cref="GetConnectionString" />: that one answers
        ///     null for a name it does not find, and the deployment path has always failed loudly
        ///     here instead. Changing that is not this method's business.
        /// </remarks>
        public static string GetSelectedConnectionString()
        {
            var selectedConnection = GetSection().SelectedConnection;

            return _projectConfig != null
                ? _projectConfig.ConnectionStrings.ConnectionStrings[selectedConnection].ConnectionString
                : ConfigurationManager.ConnectionStrings[selectedConnection].ConnectionString;
        }

        /// <summary>
        ///     A connection string by name, or <see langword="null" /> if it is not declared.
        /// </summary>
        /// <remarks>
        ///     Named rather than selected, for the connection strings that are not the Dataverse one:
        ///     the remote debugger's <c>DebugConnectionString</c> (Azure Relay) is read this way, so
        ///     that it resolves identically whether the host is a classic .NET Framework runner
        ///     reading its App.config or a .NET one pointed at a project's <c>Config/</c> folder by
        ///     <see cref="UseProjectConfig" />.
        ///     <para>
        ///         Returns null instead of throwing: a missing connection string is the caller's to
        ///         explain, and it is the one that knows what the name is for.
        ///     </para>
        /// </remarks>
        public static string GetConnectionString(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var settings = _projectConfig != null
                ? _projectConfig.ConnectionStrings.ConnectionStrings[name]
                : ConfigurationManager.ConnectionStrings[name];

            return settings?.ConnectionString;
        }

        /// <summary>
        ///     An <c>appSettings</c> value by key, or <see langword="null" /> if it is not declared.
        /// </summary>
        /// <remarks>
        ///     Same duality as <see cref="GetConnectionString" />, for the same reason: it lets a
        ///     runner read its settings with one line of code whether it is a .NET Framework host with
        ///     an App.config or a .NET one pointed at a project's <c>Config/</c> folder.
        /// </remarks>
        public static string GetAppSetting(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            return _projectConfig != null
                ? _projectConfig.AppSettings.Settings[key]?.Value
                : ConfigurationManager.AppSettings[key];
        }
    }
}
