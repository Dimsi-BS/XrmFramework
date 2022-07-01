#nullable enable
using Microsoft.CodeAnalysis;
using System.Text;

namespace XrmFramework.Analyzers.Extensions
{
    public static class SymbolExtensions
    {
        public static string GetFullMetadataName(this ISymbol? s)
        {
            var symbolName = s?.ToString();

            if (s == null || IsRootNamespace(s))
            {
                return string.Empty;
            }

            var sb = new StringBuilder(s.MetadataName);
            var last = s;

            if (s.ContainingSymbol != null)
            {
                s = s.ContainingSymbol;

                while (!IsRootNamespace(s))
                {
                    if (s is ITypeSymbol && last is ITypeSymbol)
                    {
                        sb.Insert(0, '+');
                    }
                    else
                    {
                        sb.Insert(0, '.');
                    }

                    sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));

                    s = s.ContainingSymbol;
                }
            }
            else
            {
                sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            }

            return sb.ToString();
        }

        private static bool IsRootNamespace(ISymbol symbol)
            => symbol is INamespaceSymbol { IsGlobalNamespace: true };

        public static bool IsAssignableFrom(this INamedTypeSymbol source, INamedTypeSymbol target)
        {
            if (SymbolEqualityComparer.Default.Equals(source, target))
            {
                return true;
            }

            if (target.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(source, i)))
            {
                return true;
            }

            if (target.BaseType != null && source.IsAssignableFrom(target.BaseType))
            {
                return true;
            }

            return false;
        }

        public static bool IsAssignableToIService(this INamedTypeSymbol? symbol, bool checkAbstraction)
            => IsIService(symbol) || symbol is not null && (!checkAbstraction || !symbol.IsAbstract) && symbol.AllInterfaces.Any(IsIService);

        public static bool IsIService(this INamedTypeSymbol? symbol)
            => symbol is
            {
                Name: "IService",
                ContainingNamespace:
                {
                    Name: "XrmFramework",
                    ContainingNamespace.IsGlobalNamespace: true
                }
            };

        public static bool IsDefaultService(this INamedTypeSymbol? symbol)
            => symbol is
            {
                Name: "DefaultService",
                ContainingNamespace:
                {
                    Name: "XrmFramework",
                    ContainingNamespace.IsGlobalNamespace: true
                }
            };
    }
}
