// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;
using Deploy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using XrmFramework.DeployUtils.CommandOptions;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils;

public static class WebResourceHelper
{
    public static void SyncWebResources(string projectName, params string[] args)
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

        SyncWebResources(projectName, options.Path, options.DisablePrompt);
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
    public static void SyncWebResources(string projectName, string webresourcesPath, bool noPrompt)
        => SyncWebResources(projectName, webresourcesPath, new DeployOptions { NoPrompt = noPrompt });

    /// <param name="deployOptions">
    ///     Everything the command line settled: the silent mode, and how to authenticate.
    /// </param>
    /// <inheritdoc cref="SyncWebResources(string,string,bool)" />
    /// <remarks>
    ///     The Dataverse client comes from the same container the plugin deployment uses, so this
    ///     reaches the environment exactly the same way - workload identity federation included.
    ///     It used to read <c>ConfigurationManager.ConnectionStrings</c> directly, which ignores the
    ///     project configuration <see cref="ConfigHelper.UseProjectConfig" /> loads: on the CLI,
    ///     where there is no application App.config to read, that could only ever fail.
    /// </remarks>
    public static void SyncWebResources(string projectName, string webresourcesPath, DeployOptions deployOptions)
    {
        var nbWebresources = 0;

        var xrmFrameworkConfigSection = ConfigHelper.GetSection();

        var solutionName = xrmFrameworkConfigSection.Projects.OfType<ProjectElement>().Single(p => p.Name == projectName).TargetSolution;

        var serviceProvider = DeployServiceCollectionFactory
            .CreateServiceCollection(projectName, deployOptions)
            .BuildServiceProvider();

        var deploySettings = serviceProvider.GetRequiredService<DeploySettings>();

        // The URL, never the connection string: that one carries a client secret, and this line
        // used to print it.
        if (!deployOptions.NoPrompt)
        {
            Console.WriteLine($@"You are about to deploy on {deploySettings.Url}. If ok press any key.");
            Console.ReadKey();
        }
        else
        {
            Console.WriteLine($@"Connecting to the environment {deploySettings.Url}");
        }

        Console.WriteLine(@"Connecting to CRM...");

#if NET462_OR_GREATER
        Microsoft.Xrm.Tooling.Connector.CrmServiceClient.MaxConnectionTimeout = TimeSpan.FromMinutes(10);
#else
        Microsoft.PowerPlatform.Dataverse.Client.ServiceClient.MaxConnectionTimeout = TimeSpan.FromMinutes(10);
#endif

        var service = serviceProvider.GetRequiredService<IOrganizationService>();

        // WhoAmI is the readiness check: the client's own IsReady says nothing about whether the
        // identity can actually be used, and a federated sign-in only reaches Entra here.
        try
        {
            service.Execute(new WhoAmIRequest());
        }
        catch (FaultException<OrganizationServiceFault> ex)
        {
            throw new Exception($"Unable to connect to CRM : {ex.Message}", ex);
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
            return;
        }

        if (solution.GetAttributeValue<bool>("ismanaged"))
        {
            Console.WriteLine(@"Error : Solution {0} is managed, no deployment possible.", solutionName);
            return;
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
            return;
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
        }

        DirectoryInfo root = new DirectoryInfo(webresourcesPath);
        var resourcesToPublish = new StringBuilder();

        var files = Directory
            .GetFiles(webresourcesPath, "*.*", SearchOption.AllDirectories)
            .Select(file => new FileInfo(file))
            .Where(fi => IsWebResource(fi.Extension))
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
