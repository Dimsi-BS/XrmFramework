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
    public class InternalDependencyProviderGenerator
    {
        private static readonly ICSharpHelper Code = new CSharpHelper();

        public static void Generate(string loggedServiceFolder, IEnumerable<Type> types, Type iServiceType, Type defaultServiceType, Type iLoggedServiceType)
        {
            var namespaceSet = new HashSet<string>();

            var sb = new IndentedStringBuilder();

            List<(Type serviceType, Type implementationType)> listServices = new();

            var allTypes = iServiceType.Assembly.GetModules().SelectMany(m => m.GetTypes()).ToList();

            foreach (var t in types)
            {
                namespaceSet.Add(t.Namespace);

                foreach (var type in allTypes)
                {
                    if (t.IsAssignableFrom(type) && !type.IsAbstract && type.IsClass &&
                        !iLoggedServiceType.IsAssignableFrom(type)
                        && (t == iServiceType && type == defaultServiceType || t != iServiceType))
                    {
                        namespaceSet.Add(type.Namespace);
                        listServices.Add((t, type));
                    }
                }
            }

            foreach (var ns in namespaceSet.OrderBy(n => n))
            {
                sb
                    .Append("using ")
                    .Append(ns)
                    .AppendLine(";");

                sb
                    .Append("using ")
                    .Append(Code.Namespace(ns, "LoggedServices"))
                    .AppendLine(";");
            }

            sb
                .AppendLine("namespace XrmFramework")
                .AppendLine("{");

            using (sb.Indent())
            {
                sb
                    .AppendLine("partial class InternalDependencyProvider")
                    .AppendLine("{");

                using (sb.Indent())
                {
                    sb
                        .AppendLine("static partial void RegisterServices(IObjectContainer container)")
                        .AppendLine("{");

                    using (sb.Indent())
                    {
                        foreach (var service in listServices)
                        {
                            sb
                                .Append("container.RegisterTypeAs<")
                                .Append(Code.Reference(service.implementationType))
                                .Append(", ")
                                .Append(Code.Reference(service.implementationType))
                                .AppendLine(">();")
                                
                                .AppendLine()
                                
                                .Append("container.RegisterFactoryAs<")
                                .Append(Code.Reference(service.serviceType))
                                .AppendLine(">(objectContainer =>");

                            using (sb.Indent())
                            {
                                sb
                                    .AppendLine("var context = objectContainer.Resolve<IServiceContext>();")

                                    .Append("var service = objectContainer.Resolve<")
                                    .Append(Code.Reference(service.implementationType))
                                    .AppendLine(">();")

                                    .Append("return new ")
                                    .Append(GetLogServiceName(service.serviceType.Name))
                                    .AppendLine("(context, service);");
                            }

                            sb.AppendLine("});");
                        }
                    }

                    sb.AppendLine("}");
                }

                sb.AppendLine("}");
            }

            sb.AppendLine("}");


            var fileName = Path.Combine(loggedServiceFolder, "InternalDependencyProvider.cs");

            File.WriteAllText(fileName, sb.ToString());
        }

        private static string GetLogServiceName(string serviceName)
        {
            return $"Logged{serviceName.Substring(1)}";
        }
    }
}
