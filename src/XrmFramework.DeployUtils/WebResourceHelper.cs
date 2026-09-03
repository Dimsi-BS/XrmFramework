// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;
using Deploy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Runtime.CompilerServices;
using XrmFramework.DeployUtils.CommandOptions;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Model;

[assembly: InternalsVisibleTo("XrmFramework.DeployUtils.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001000d196816c56c09f53b4235803db7e452f0c1911a84b0f48ed49fc5b6cd544869a7e74fb971f388bd335b537b22e43a63101907a395e40bc0e434dc9a98c8f2d4e61e84f274cbf9bcb2b8415f582b26d5f2bd3d152d1736440ecd978b8216bb9a6ee429c9f84e87b00ca4e8fb747292d433a4017c8fa51456e80c6f12c95f59b4")]

namespace XrmFramework.DeployUtils;

public static class WebResourceHelper
{
    /// <summary>
    ///     Folder a bundler emits into. Preferred over the project folder when it exists, so that
    ///     sources are not published alongside their bundles.
    /// </summary>
    private const string DistDirectoryName = "dist";

    /// <summary>
    ///     Folders never scanned for web resources, whatever their depth. They hold dependencies or
    ///     build output: <c>node_modules</c> alone contributes thousands of <c>.js</c>, <c>.css</c>
    ///     and <c>.png</c> files that have nothing to do with the environment.
    /// </summary>
    private static readonly string[] ExcludedDirectoryNames =
    {
        "node_modules", "bin", "obj", ".vs", ".git"
    };

    /// <summary>
    ///     Walks <paramref name="root" /> depth-first, skipping <see cref="ExcludedDirectoryNames" />,
    ///     and yields the files whose extension Dataverse accepts as a web resource.
    /// </summary>
    internal static IEnumerable<FileInfo> EnumerateWebResourceFiles(DirectoryInfo root)
    {
        foreach (var file in root.GetFiles())
        {
            if (IsWebResource(file.Extension))
            {
                yield return file;
            }
        }

        foreach (var directory in root.GetDirectories())
        {
            if (ExcludedDirectoryNames.Contains(directory.Name, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            foreach (var file in EnumerateWebResourceFiles(directory))
            {
                yield return file;
            }
        }
    }

    public static int SyncWebResources(string projectName, params string[] args)
    {
        var options = new WebresourceCommandOptions();

        Parser.Default.ParseArguments<WebresourceCommandOptions>(args)
            .WithParsed(o =>
            {
                options = o;
                if (o.DisablePrompt)
                {
                    Console.WriteLine($@"Disabled connection prompt. Current Arguments: -n {o.DisablePrompt}");
                }

                if (!string.IsNullOrEmpty(o.Path))
                {
                    Console.WriteLine($@"Forced path");
                    Console.WriteLine($@"Path : -p {o.Path}");
                }
            });

        return SyncWebResources(projectName, options.Path, options.DisablePrompt);
    }

    /// <summary>
    ///     Publishes the web resources found under <paramref name="webresourcesPath" /> (or, when
    ///     omitted, auto-discovered from a folder named <paramref name="projectName" /> walked up from
    ///     the current directory) to the CRM solution declared for <paramref name="projectName" /> in
    ///     <c>xrmFramework.config</c>.
    /// </summary>
    /// <param name="projectName">Name of the project as declared in <c>xrmFramework.config</c> (e.g. <c>"Webresources"</c>).</param>
    /// <param name="webresourcesPath">Webresources project folder. Auto-discovered when <see langword="null" /> or empty.</param>
    /// <param name="noPrompt">Silent mode: skips the interactive connection confirmation (CI/CD).</param>
    /// <returns>
    ///     The process exit code: <c>0</c> on success, <c>1</c> when <paramref name="projectName" /> is
    ///     not declared in <c>xrmFramework.config</c>, <c>3</c> on any other failure.
    /// </returns>
    public static int SyncWebResources(string projectName, string webresourcesPath, bool noPrompt)
    {
        try
        {
            return SyncWebResourcesCore(projectName, webresourcesPath, noPrompt);
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($@"Error : {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return 3;
        }
    }

    private static int SyncWebResourcesCore(string projectName, string webresourcesPath, bool noPrompt)
    {
        var nbWebresources = 0;

        var xrmFrameworkConfigSection = ConfigHelper.GetSection();

        var project = xrmFrameworkConfigSection.Projects.OfType<ProjectElement>().SingleOrDefault(p => p.Name == projectName);

        if (project == null)
        {
            Console.WriteLine(@"Error : Project {0} is not declared in xrmFramework.config.", projectName);
            return 1;
        }

        var solutionName = project.TargetSolution;

        var connectionString = ConfigurationManager.ConnectionStrings[xrmFrameworkConfigSection.SelectedConnection].ConnectionString;

        if (!noPrompt)
        {
            Console.WriteLine($@"You are about to deploy on {connectionString} organization. If ok press any key.");
            Console.ReadKey();
        }
        else
        {
            var parsedConnectionString = ConnectionStringParser.Parse(connectionString);
                
            Console.WriteLine($@"Connecting to the environment {parsedConnectionString.Url}");
        }

        Console.WriteLine(@"Connecting to CRM...");

#if NET462_OR_GREATER

        Microsoft.Xrm.Tooling.Connector.CrmServiceClient.MaxConnectionTimeout = TimeSpan.FromMinutes(10);

        var service = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(connectionString);

#else
        Microsoft.PowerPlatform.Dataverse.Client.ServiceClient.MaxConnectionTimeout = TimeSpan.FromMinutes(10);
        var service = new Microsoft.PowerPlatform.Dataverse.Client.ServiceClient(connectionString);
#endif
        if (!service.IsReady)
        {
            throw new Exception(
#if NET462_OR_GREATER
                $"Unable to connect to CRM : {service.LastCrmError}");
#else
            $"Unable to connect to CRM : {service.LastError}");
#endif
        }

        try
        {
            service.Execute(new WhoAmIRequest());
        }
        catch (FaultException<OrganizationServiceFault>)
        {
            throw new Exception(
#if NET462_OR_GREATER
                $"Unable to connect to CRM : {service.LastCrmError}"
#else
            $"Unable to connect to CRM : {service.LastError}"
#endif
                );
        }

        var query = new QueryExpression(Solution.EntityLogicalName);
        query.ColumnSet.AddColumn("uniquename");
        query.ColumnSet.AddColumn("publisherid");
        query.ColumnSet.AddColumn("ismanaged");
        query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solutionName);
        var result = service.RetrieveMultiple(query);

        var solution = result.Entities.FirstOrDefault();
        if (solution == null)
        {
            Console.WriteLine(@"Error : Solution not found : {0}", solutionName);
            return 3;
        }

        if (solution.GetAttributeValue<bool>("ismanaged"))
        {
            Console.WriteLine(@"Error : Solution {0} is managed, no deployment possible.", solutionName);
            return 3;
        }

        var publisherId = solution.GetAttributeValue<EntityReference>("publisherid").Id;

        query = new QueryExpression(Publisher.EntityLogicalName);
        query.ColumnSet.AddColumn("customizationprefix");
        query.Criteria.AddCondition("publisherid", ConditionOperator.Equal, publisherId);
        result = service.RetrieveMultiple(query);

        var publisher = result.Entities.FirstOrDefault();
        if (publisher == null)
        {
            Console.WriteLine(@"Error : Publisher not found : {0}", solutionName);
            return 3;
        }
        var prefix = publisher.GetAttributeValue<string>("customizationprefix");
        Console.WriteLine(@" ==> Prefix : {0}", prefix);

        if (string.IsNullOrWhiteSpace(webresourcesPath))
        {
            var currentDirectory = new DirectoryInfo(".");

            while (currentDirectory != null && currentDirectory.GetDirectories().All(d => d.Name != projectName))
            {
                Console.WriteLine($@"currentPath = {currentDirectory.FullName}");
                currentDirectory = currentDirectory.Parent;
            }

            if (currentDirectory == null)
            {
                throw new DirectoryNotFoundException($"The {projectName} folder cannot be found");
            }

            webresourcesPath = currentDirectory.GetDirectories(projectName).Single().FullName;

            // A project with a TypeScript / bundler toolchain emits everything it wants deployed
            // into dist/. Publishing the project folder instead would send the sources next to the
            // bundles, and prefix every unique name with "dist/".
            var distDirectory = Path.Combine(webresourcesPath, DistDirectoryName);

            if (Directory.Exists(distDirectory))
            {
                Console.WriteLine(@" ==> Using the {0} folder", DistDirectoryName);
                webresourcesPath = distDirectory;
            }
        }

        DirectoryInfo root = new DirectoryInfo(webresourcesPath);
        var resourcesToPublish = new StringBuilder();

        var files = EnumerateWebResourceFiles(root)
            .Select(fi => new WebResource(fi, root, prefix))
            .ToList();

        foreach (var fi in files)
        {
            var publish = false;

            string webResourceUniqueName = fi.FullName;
            Guid webResourceId;

            var webResource = GetWebResource(webResourceUniqueName, service);
            if (webResource == null)
            {
                webResourceId = CreateWebResource(webResourceUniqueName, fi, solutionName, service);
                publish = true;
            }
            else
            {
                // Web resource exists, check if update is required

                webResourceId = webResource.Id;

                if (webResource.Equals(fi))
                {
                    // Content is identical, no need to update
                }
                else
                {
                    var updatedWr = new Entity("webresource", webResource.Id);
                    updatedWr["content"] = fi.Base64Content;
                    updatedWr["dependencyxml"] = fi.GetDependenciesXml();

                    service.Update(updatedWr);
                    publish = true;
                }
            }
            Console.ForegroundColor = publish ? ConsoleColor.DarkGreen : ConsoleColor.White;
            Console.WriteLine($@"{fi.FullName} => {webResourceUniqueName}");
            Console.ForegroundColor = ConsoleColor.White;

            if (publish)
            {
                resourcesToPublish.AppendFormat("<webresource>{0}</webresource>", webResourceId);
                nbWebresources++;
            }
        }

        if (resourcesToPublish.Length > 0)
        {
            Console.WriteLine();
            Console.WriteLine($@"Publishing {nbWebresources} Resources...");

            var request = new PublishXmlRequest
            {
                ParameterXml = $"<importexportxml><webresources>{resourcesToPublish}</webresources></importexportxml>"
            };

            service.Execute(request);

        }

        return 0;
    }


    /// <summary>
    /// Gets the web resource.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="service">The service.</param>
    /// <returns></returns>
    private static WebResource GetWebResource(string name, IOrganizationService service)
    {
        var query = new QueryExpression("webresource");
        query.ColumnSet.AddColumns("content", "dependencyxml", "name");
        query.Criteria.AddCondition("name", ConditionOperator.Equal, name);
        var result = service.RetrieveMultiple(query);

        var webResource = result.Entities.Select(e => new WebResource(e)).FirstOrDefault();
        return webResource;
    }

    /// <summary>
    /// Creates the web resource.
    /// </summary>
    /// <param name="webResourceName">Name of the web resource.</param>
    /// <param name="fi">The fi.</param>
    /// <param name="solutionUniqueName">Name of the solution unique.</param>
    /// <param name="service">The service.</param>
    /// <exception cref="System.Exception">Unsupported extension:  + fi.Extension.Remove(0, 1).ToLower()</exception>
    private static Guid CreateWebResource(string webResourceName, WebResource fi, string solutionUniqueName, IOrganizationService service)
    {
        var wr = new Entity("webresource");
        wr["name"] = webResourceName;
        wr["displayname"] = webResourceName;
        wr["content"] = fi.Base64Content;
        wr["dependencyxml"] = fi.GetDependenciesXml();

        if (string.IsNullOrEmpty(fi.Extension))
        {
                throw new Exception($@"No extension found for the file '{fi.FullName}'!");
        }

        string extension = fi.Extension.Remove(0, 1).ToLower();
        switch (extension)
        {
            case "htm":
            case "html":
                wr["webresourcetype"] = new OptionSetValue(1);
                break;
            case "css":
                wr["webresourcetype"] = new OptionSetValue(2);
                break;
            case "js":
                wr["webresourcetype"] = new OptionSetValue(3);
                break;
            case "xml":
                wr["webresourcetype"] = new OptionSetValue(4);
                break;
            case "png":
                wr["webresourcetype"] = new OptionSetValue(5);
                break;
            case "jpg":
            case "jpeg":
                wr["webresourcetype"] = new OptionSetValue(6);
                break;
            case "gif":
                wr["webresourcetype"] = new OptionSetValue(7);
                break;
            case "xap":
                wr["webresourcetype"] = new OptionSetValue(8);
                break;
            case "xsl":
                wr["webresourcetype"] = new OptionSetValue(9);
                break;
            case "ico":
                wr["webresourcetype"] = new OptionSetValue(10);
                break;
            case "svg":
                wr["webresourcetype"] = new OptionSetValue(11);
                break;
            case "resx":
                wr["webresourcetype"] = new OptionSetValue(12);
                break;
            default:
                throw new Exception("Unsupported extension: " + fi.Extension.Remove(0, 1).ToLower());
        }

        var id = service.Create(wr);

        // Add current web resource to defined solution
        var request = new AddSolutionComponentRequest { AddRequiredComponents = false, ComponentType = 61, ComponentId = id, SolutionUniqueName = solutionUniqueName };
        service.Execute(request);

        return id;
    }

    /// <summary>
    /// Determines whether [is web resource] [the specified extension].
    /// </summary>
    /// <param name="extension">The extension.</param>
    /// <returns></returns>
    private static bool IsWebResource(string extension)
    {
        switch (extension.ToLower())
        {
            case ".htm":
            case ".html":
            case ".css":
            case ".js":
            case ".xml":
            case ".png":
            case ".jpg":
            case ".jpeg":
            case ".gif":
            case ".xap":
            case ".xsl":
            case ".ico":
            case ".svg":
            case ".resx":
                return true;
            default:
                return false;
        }
    }
}
