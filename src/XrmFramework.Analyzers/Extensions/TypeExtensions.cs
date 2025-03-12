// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using XrmFramework.Analyzers.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly Dictionary<SpecialType, string> _primitiveTypes = new Dictionary<SpecialType, string>
        {
           { SpecialType.System_Boolean, "bool"   },
           { SpecialType.System_Byte, "byte"   },
           { SpecialType.System_Char, "char"   },
           { SpecialType.System_Decimal, "decimal"} ,
           { SpecialType.System_Double, "double" },
           { SpecialType.System_Single, "float"  },
           { SpecialType.System_Int32, "int"    },
           { SpecialType.System_Int64, "long"   } ,
           { SpecialType.System_SByte, "sbyte"  },
           { SpecialType.System_Int16, "short"  } ,
           { SpecialType.System_String, "string" }  ,
           { SpecialType.System_Object, "object" }  ,
           { SpecialType.System_UInt32, "uint"   }  ,
           { SpecialType.System_UInt64, "ulong"  }  ,
           { SpecialType.System_UInt16, "ushort" }  ,
            { SpecialType.System_Void,  "void"   }
        };

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static IEnumerable<string> GetNamespaces(this ITypeSymbol type)
        {
            if (type.IsPrimitive())
            {
                yield break;
            }

            if (type.ContainingNamespace?.IsGlobalNamespace ?? true)
            {
                yield break;
            }

            yield return type.ContainingNamespace.ToDisplayString();

            if (type is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType)
            {
                foreach (var typeArgument in namedTypeSymbol.TypeArguments)
                {
                    foreach (var ns in typeArgument.GetNamespaces())
                    {
                        yield return ns;
                    }
                }
            }
        }

        public static bool IsPrimitive(this ITypeSymbol symbol)
            => _primitiveTypes.ContainsKey(symbol.SpecialType);

        public static bool ImplementsIService(this ITypeSymbol symbol) 
            => symbol.AllInterfaces.Any(i => i.GetFullMetadataName() == "XrmFramework.IService");
    }
}
