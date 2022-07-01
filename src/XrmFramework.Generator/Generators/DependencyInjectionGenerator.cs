// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XrmFramework.Generator.Generators
{
    public class DependencyInjectionGenerator
    {
        private static readonly ICSharpHelper Code = new CSharpHelper();

        public static void Generate(string loggedServiceFolder, ICollection<Type> types)
        {
            var namespaceSet = new HashSet<string>();

            var sb = new IndentedStringBuilder();

            foreach (var t in types)
            {
                namespaceSet.Add(t.Namespace);
            }

            sb.AppendLine("#if PLUGIN || CORE_PROJECT");

            foreach (var ns in namespaceSet.OrderBy(n => n))
            {
                sb
                    .Append("using ")
                    .Append(ns)
                    .AppendLine(";");
            }

            sb
                .AppendLine()
                .AppendLine("// ReSharper disable once CheckNamespace")
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
                        foreach (var serviceType in types)
                        {
                            sb
                                .Append("RegisterService<")
                                .Append(Code.Reference(serviceType))
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
    }
}
