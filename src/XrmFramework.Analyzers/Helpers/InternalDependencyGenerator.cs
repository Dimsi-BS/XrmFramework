#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Immutable;
using XrmFramework.Analyzers.Extensions;

namespace XrmFramework.Analyzers.Generators
{
    public class InternalDependencyGenerator
    {
        public string GetInternalDependencyFileContent(ImmutableArray<INamedTypeSymbol?> services, ImmutableArray<INamedTypeSymbol?> implementations)
        {
            var namespaceSet = new HashSet<string> { "BoDi" };

            var sb = new IndentedStringBuilder();

            List<(ITypeSymbol serviceType, ITypeSymbol implementationType)> listServices = new();

            foreach (var t in services.Distinct(SymbolEqualityComparer.Default).Cast<INamedTypeSymbol?>())
            {
                if (t == null) continue;

                foreach (var type in implementations)
                {
                    if (type == null) continue;

                    if (t.IsAssignableFrom(type) && (t.IsIService() && type.IsDefaultService() || !t.IsIService()))
                    {
                        namespaceSet.Add(t.ContainingNamespace.GetFullMetadataName());
                        namespaceSet.Add(type.ContainingNamespace.GetFullMetadataName());
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
            }

            sb
                .AppendLine()
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
                                .Append("RegisterService<")
                                .Append(service.serviceType.Name)
                                .Append(", ")
                                .Append(service.implementationType.Name)
                                .Append(", ")
                                .Append($"Logged{service.serviceType.Name}")
                                .AppendLine(">(container);")
                                .AppendLine();
                        }
                    }

                    sb.AppendLine("}");
                }

                sb.AppendLine("}");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
