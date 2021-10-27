// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using XrmFramework.Generator.Properties;

namespace XrmFramework.DeployUtils.Generators
{
    public class DependencyInjectionGenerator
    {
        private static readonly ICSharpHelper Code = new CSharpHelper();

        public static void Generate(string loggedServiceFolder, IEnumerable<Type> types, Type iServiceType, Type defaultServiceType)
        {
            var namespaceSet = new HashSet<string>{ "XrmFramework.DependencyInjection" };

            var sb = new IndentedStringBuilder();

            List<(Type serviceType, Type implementationType)> listServices = new();

            var allTypes = iServiceType.Assembly.GetTypes().ToList();

            foreach (var t in types)
            {
                foreach (var type in allTypes)
                {
                    if (t.IsAssignableFrom(type) && !type.IsAbstract && type.IsClass && 
                        (t == iServiceType && type == defaultServiceType || t != iServiceType))
                    {
                        namespaceSet.Add(t.Namespace);
                        namespaceSet.Add(type.Namespace);
                        listServices.Add((t, type));
                    }
                }
            }

            sb.AppendLine("#if !DISABLE_DI");

            foreach (var ns in namespaceSet.OrderBy(n => n))
            {
                sb
                    .Append("using ")
                    .Append(ns)
                    .AppendLine(";");
            }

            sb
                .AppendLine()
                .AppendLine("namespace Microsoft.Extensions.DependencyInjection")
                .AppendLine("{");

            using (sb.Indent())
            {
                sb
                    .AppendLine("partial class XrmFrameworkServiceCollectionExtension")
                    .AppendLine("{");

                using (sb.Indent())
                {
                    sb
                        .AppendLine("static partial void RegisterServices(IServiceCollection serviceCollection)")
                        .AppendLine("{");

                    using (sb.Indent())
                    {
                        foreach (var service in listServices)
                        {
                            sb
                                .Append("RegisterService<")
                                .Append(Code.Reference(service.serviceType))
                                .Append(", ")
                                .Append(Code.Reference(service.implementationType))
                                .AppendLine(">(serviceCollection);")
                                .AppendLine();
                        }
                    }

                    sb.AppendLine("}");
                }

                sb.AppendLine("}");
            }

            sb
                .AppendLine("}")
                .AppendLine()
                .AppendLine("#endif");


            var fileName = Path.Combine(loggedServiceFolder, "XrmFrameworkServiceCollectionExtension.cs");

            File.WriteAllText(fileName, sb.ToString());
        }

        private static string GetLogServiceName(string serviceName)
        {
            return $"Logged{serviceName.Substring(1)}";
        }
    }
}
