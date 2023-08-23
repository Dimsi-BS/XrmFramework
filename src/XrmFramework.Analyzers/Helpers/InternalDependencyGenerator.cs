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
			var namespaceSet = new HashSet<string> {"BoDi"};

			var internalDependencySb = new IndentedStringBuilder();
			var serviceCollectionSb = new IndentedStringBuilder();

			var listServices = ListAllServices(services, implementations, namespaceSet);

			internalDependencySb.AppendLine("#if !DISABLE_SERVICES && (PLUGIN || CORE_PROJECT)");
			serviceCollectionSb.AppendLine("#if !DISABLE_SERVICES && CORE_PROJECT");

			foreach (var ns in namespaceSet.OrderBy(n => n))
			{
				internalDependencySb
				   .Append("using ")
				   .Append(ns)
				   .AppendLine(";");
			}
			
			internalDependencySb
			   .AppendLine()
			   .AppendLine("namespace XrmFramework")
			   .AppendLine("{");

			serviceCollectionSb
			   .AppendLine()
			   .AppendLine("namespace Microsoft.Extensions.DependencyInjection")
			   .AppendLine("{");

			using (internalDependencySb.Indent())
			using (serviceCollectionSb.Indent())
			{
				internalDependencySb
				   .AppendLine("partial class InternalDependencyProvider")
				   .AppendLine("{");

				serviceCollectionSb
				   .AppendLine("partial class XrmFrameworkServiceCollectionExtension")
				   .AppendLine("{");

				using (internalDependencySb.Indent())
				using (serviceCollectionSb.Indent())
				{
					internalDependencySb
					   .AppendLine("static partial void RegisterServices(IObjectContainer container)")
					   .AppendLine("{");
					serviceCollectionSb
					   .AppendLine("static partial void RegisterServices(IServiceCollection serviceCollection)")
					   .AppendLine("{");

					using (internalDependencySb.Indent())
					using (serviceCollectionSb.Indent())
					{
						foreach (var service in listServices)
						{
							internalDependencySb
							   .Append("RegisterService<")
							   .Append(service.serviceType.Name)
							   .Append(", ")
							   .Append(service.implementationType.Name)
							   .Append(", ")
							   .Append($"Logged{service.serviceType.Name}")
							   .AppendLine(">(container);")
							   .AppendLine();
							
							serviceCollectionSb
							   .Append("RegisterService<")
							   .Append(service.serviceType.Name)
							   .AppendLine(">(serviceCollection);")
							   .AppendLine();
						}
					}
					
					internalDependencySb.AppendLine("}");
					serviceCollectionSb.AppendLine("}");
				}
				
				internalDependencySb.AppendLine("}");
				serviceCollectionSb.AppendLine("}");
			}

			internalDependencySb.AppendLine("}");
			serviceCollectionSb.AppendLine("}");

			internalDependencySb.AppendLine("#endif");
			serviceCollectionSb.AppendLine("#endif");

			return $"{internalDependencySb}\n{serviceCollectionSb}";
		}

		private static List<(ITypeSymbol serviceType, ITypeSymbol implementationType)> ListAllServices(ImmutableArray<INamedTypeSymbol?> services, ImmutableArray<INamedTypeSymbol?> implementations, HashSet<string> namespaceSet)
		{
			List<(ITypeSymbol serviceType, ITypeSymbol implementationType)> listServices = new();

			foreach (var t in services.Distinct(SymbolEqualityComparer.Default).Cast<INamedTypeSymbol?>())
			{
				if (t == null) continue;

				foreach (var type in implementations)
				{
					if (type == null || !t.IsAssignableFrom(type) || ((!t.IsIService() || !type.IsDefaultService()) && t.IsIService()))
					{
						continue;
					}

					namespaceSet.Add(t.ContainingNamespace.GetFullMetadataName());
					namespaceSet.Add(type.ContainingNamespace.GetFullMetadataName());
					listServices.Add((t, type));
				}
			}

			return listServices;
		}
	}
}
