// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

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

        ///// <summary>
        /////     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        /////     the same compatibility standards as public APIs. It may be changed or removed without notice in
        /////     any release. You should only use it directly in your code with extreme caution and knowing that
        /////     doing so can result in application failures when updating to a new Entity Framework Core release.
        ///// </summary>
        //public static string DisplayName(this ITypeSymbol type, bool fullName = true)
        //{
        //    var stringBuilder = new StringBuilder();
        //    ProcessType(stringBuilder, type, fullName);
        //    return stringBuilder.ToString();
        //}

        //private static void ProcessType(StringBuilder builder, ITypeSymbol type, bool fullName)
        //{
        //    if (type is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType)
        //    {
        //        var genericArguments = namedTypeSymbol.TypeArguments;
        //        ProcessGenericType(builder, namedTypeSymbol, genericArguments, genericArguments.Length, fullName);
        //    }
        //    else if (type.IsArray)
        //    {
        //        ProcessArrayType(builder, type, fullName);
        //    }
        //    else if (_builtInTypeNames.TryGetValue(type, out var builtInName))
        //    {
        //        builder.Append(builtInName);
        //    }
        //    else if (!type.IsGenericParameter)
        //    {
        //        builder.Append(fullName ? type.FullName : type.Name);
        //    }
        //    else if (type.IsGenericParameter)
        //    {
        //        builder.Append(type.Name);
        //    }
        //}

        //private static void ProcessArrayType(StringBuilder builder, Type type, bool fullName)
        //{
        //    var innerType = type;
        //    while (innerType.IsArray)
        //    {
        //        innerType = innerType.GetElementType();
        //    }

        //    ProcessType(builder, innerType, fullName);

        //    while (type.IsArray)
        //    {
        //        builder.Append('[');
        //        builder.Append(',', type.GetArrayRank() - 1);
        //        builder.Append(']');
        //        type = type.GetElementType();
        //    }
        //}

        //private static void ProcessGenericType(StringBuilder builder, INamedTypeSymbol type, ITypeSymbol[] genericArguments, int length, bool fullName)
        //{
        //    var offset = type.IsNested ? type.DeclaringType.GetGenericArguments().Length : 0;

        //    if (fullName)
        //    {
        //        if (type.IsNested)
        //        {
        //            ProcessGenericType(builder, type.DeclaringType, genericArguments, offset, fullName);
        //            builder.Append('+');
        //        }
        //        else
        //        {
        //            builder.Append(type.Namespace);
        //            builder.Append('.');
        //        }
        //    }

        //    var genericPartIndex = type.Name.IndexOf('`');
        //    if (genericPartIndex <= 0)
        //    {
        //        builder.Append(type.Name);
        //        return;
        //    }

        //    builder.Append(type.Name, 0, genericPartIndex);
        //    builder.Append('<');

        //    for (var i = offset; i < length; i++)
        //    {
        //        ProcessType(builder, genericArguments[i], fullName);
        //        if (i + 1 == length)
        //        {
        //            continue;
        //        }

        //        builder.Append(',');
        //        if (!genericArguments[i + 1].IsGenericParameter)
        //        {
        //            builder.Append(' ');
        //        }
        //    }

        //    builder.Append('>');
        //}

        ///// <summary>
        /////     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        /////     the same compatibility standards as public APIs. It may be changed or removed without notice in
        /////     any release. You should only use it directly in your code with extreme caution and knowing that
        /////     doing so can result in application failures when updating to a new Entity Framework Core release.
        ///// </summary>
        //public static IFieldSymbol GetFieldInfo(this ITypeSymbol type, string fieldName)
        //    => type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(f => f.Name == fieldName && !f.IsStatic);

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
    }
}
