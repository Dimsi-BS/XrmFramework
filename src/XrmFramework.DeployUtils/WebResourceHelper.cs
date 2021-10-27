// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Description;
using Deploy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.DeployUtils
{
    public static class WebResourceHelper
    {
        public static void SyncWebResources(string webresourcesPath, string projectName)
        {
            var nbWebresources = 0;

            var xrmFrameworkConfigSection = ConfigHelper.GetSection();

            var solutionName = xrmFrameworkConfigSection.Projects.OfType<ProjectElement>().Single(p => p.Name == projectName).TargetSolution;

            string prefix = string.Empty;

            var connectionString = ConfigurationManager.ConnectionStrings[xrmFrameworkConfigSection.SelectedConnection].ConnectionString;

            Console.WriteLine($"You are about to deploy on {connectionString} organization. If ok press any key.");
            Console.ReadKey();
            Console.WriteLine("Connecting to CRM...");

            CrmServiceClient.MaxConnectionTimeout = TimeSpan.FromMinutes(10);

            var service = new CrmServiceClient(connectionString);

            service.OrganizationServiceProxy?.EnableProxyTypes();

            var query = new QueryExpression(Solution.EntityLogicalName);
            query.ColumnSet.AddColumn("uniquename");
            query.ColumnSet.AddColumn("publisherid");
            query.ColumnSet.AddColumn("ismanaged");
            query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solutionName);
            var result = service.RetrieveMultiple(query);

            var solution = result.Entities.FirstOrDefault();
            if (solution == null)
            {
                Console.WriteLine("Error : Solution not found : {0}", solutionName);
                return;
            }
            var s = new Solution();
            if (solution.GetAttributeValue<bool>("ismanaged"))
            {
                Console.WriteLine("Error : Solution {0} is managed, no deployment possible.", solutionName);
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
                Console.WriteLine("Error : Publisher not found : {0}", solutionName);
            }
            prefix = publisher.GetAttributeValue<string>("customizationprefix");
            Console.WriteLine(" ==> Prefix : {0}", prefix);

            DirectoryInfo root = new DirectoryInfo(webresourcesPath);
            var resourcesToPublish = string.Empty;

            var files = Directory.GetFiles(webresourcesPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (!IsWebResource(fi.Extension))
                {
                    continue;
                }

                if (fi.Directory.Name == root.Name)
                {
                    continue;
                }

                var publish = false;

                string webResourceUniqueName = file;
                webResourceUniqueName = webResourceUniqueName.Replace(webresourcesPath, string.Empty);
                webResourceUniqueName = webResourceUniqueName.Replace(@"\", "/");
                webResourceUniqueName = string.Concat(prefix, "_", "/", webResourceUniqueName);


                var webResource = GetWebResource(webResourceUniqueName, service);
                if (webResource == null)
                {
                    webResource = new Entity("webresource");
                    webResource.Id = CreateWebResource(webResourceUniqueName, fi, solutionName, service);
                    publish = true;
                }
                else
                {

                    // Web resource exists, check if update is required
                    string b64File = Convert.ToBase64String(File.ReadAllBytes(fi.FullName));

                    if (webResource.Contains("content") && string.Compare(b64File, webResource["content"].ToString(), StringComparison.Ordinal) == 0)
                    {
                        // Content is identical, no need to update
                    }
                    else
                    {
                        webResource["content"] = b64File;

                        service.Update(webResource);
                        publish = true;
                    }
                }
                Console.ForegroundColor = publish ? ConsoleColor.DarkGreen : ConsoleColor.White;
                Console.WriteLine($"{file} => {webResourceUniqueName}");
                Console.ForegroundColor = ConsoleColor.White;

                if (publish)
                {
                    resourcesToPublish += string.Format("<webresource>{0}</webresource>", webResource.Id);
                    nbWebresources++;
                }
            }

            if (!string.IsNullOrEmpty(resourcesToPublish))
            {
                Console.WriteLine();
                Console.WriteLine($"Publishing {nbWebresources} Resources...");

                var request = new PublishXmlRequest
                {
                    ParameterXml = string.Format("<importexportxml><webresources>{0}</webresources></importexportxml>", resourcesToPublish)
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
        private static Entity GetWebResource(string name, IOrganizationService service)
        {
            var query = new QueryExpression("webresource");
            query.ColumnSet.AddColumn("content");
            query.Criteria.AddCondition("name", ConditionOperator.Equal, name);
            var result = service.RetrieveMultiple(query);

            var webResource = result.Entities.FirstOrDefault();
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
        private static Guid CreateWebResource(string webResourceName, FileInfo fi, string solutionUniqueName, IOrganizationService service)
        {
            var wr = new Entity("webresource");
            wr["name"] = webResourceName;
            wr["displayname"] = webResourceName;
            wr["content"] = Convert.ToBase64String(File.ReadAllBytes(fi.FullName));

            if (string.IsNullOrEmpty(fi.Extension))
            {
                throw new Exception(string.Format("No extension found for the file '{0}'!", fi.FullName));
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
}
