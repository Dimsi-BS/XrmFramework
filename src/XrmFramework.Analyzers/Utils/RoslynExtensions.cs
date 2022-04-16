using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace XrmFramework.Analyzers.Utils
{
    public static class RoslynExtensions
    {
        public static AttributeSyntax Update(this AttributeSyntax originalAttribute,
          NameSyntax name = null,
          AttributeArgumentListSyntax argumentList = null)
        {
            return originalAttribute.Update(
              name ?? originalAttribute.Name,
              argumentList ?? originalAttribute.ArgumentList);
        }

        public static AttributeSyntax GetAttribute<TAttribute>(this MethodDeclarationSyntax method)
        {
            return GetAttribute<TAttribute>(method.AttributeLists);
        }

        public static AttributeSyntax GetAttribute<TAttribute>(this ParameterSyntax parameter)
        {
            return GetAttribute<TAttribute>(parameter.AttributeLists);
        }

        public static AttributeSyntax GetAttribute<TAttribute>(this TypeDeclarationSyntax type)
        {
            return GetAttribute<TAttribute>(type.AttributeLists);
        }

        public static AttributeSyntax GetAttribute<TAttribute>(this PropertyDeclarationSyntax property)
        {
            return GetAttribute<TAttribute>(property.AttributeLists);
        }

        private static AttributeSyntax GetAttribute<TAttribute>(SyntaxList<AttributeListSyntax> attributeList)
        {
            var attributeType = typeof(TAttribute);
            return
              attributeList.SelectMany(a => a.Attributes).FirstOrDefault(
                a =>
                ((IdentifierNameSyntax)a.Name).Identifier.Text == attributeType.Name ||
                attributeType.Name.EndsWith("Attribute") &&
                ((IdentifierNameSyntax)a.Name).Identifier.Text ==
                attributeType.Name.Substring(0, attributeType.Name.Length - "Attribute".Length));
        }
        public static CompilationUnitSyntax CheckAndAddUsing(this CompilationUnitSyntax syntax, string usingName)
        {
            var newSyntax = syntax;

            if (newSyntax.Usings.All(u => u.Name.ToString() != usingName))
            {
                newSyntax = syntax.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(usingName)));
            }

            return newSyntax;
        }

        public static MethodDeclarationSyntax AddParameter(this MethodDeclarationSyntax method, string parameterName, string typeName, string defaultValue = null)
        {
            var parameterSyntax = SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName)).WithType(SyntaxFactory.ParseTypeName(typeName));

            if (!string.IsNullOrEmpty(defaultValue))
            {
                parameterSyntax = parameterSyntax.WithDefault(SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(defaultValue)));
            }

            return method.AddParameterListParameters(parameterSyntax);
        }

        public static ClassDeclarationSyntax AddAttribute(this ClassDeclarationSyntax classSyntax, string attributeClassName, string arguments = null)
        {
            var attributeSyntax = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attributeClassName));

            if (!string.IsNullOrEmpty(arguments))
            {
                attributeSyntax = attributeSyntax.WithArgumentList(SyntaxFactory.ParseAttributeArgumentList(arguments));
            }

            var newClassSyntax = classSyntax.AddAttributeLists(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SeparatedList(new[]
                    {
                        attributeSyntax
                    })));

            return newClassSyntax;
        }

        public static StatementSyntax WithTrailingTrivia(this StatementSyntax statementSyntax, string trailingTrivia)
        {
            return statementSyntax.WithTrailingTrivia(SyntaxFactory.ParseTrailingTrivia(trailingTrivia));
        }


        internal static bool IsActiveBindingModel(this INamedTypeSymbol classSymbol)
        {
            if (classSymbol.Name == "Object" && classSymbol.ContainingNamespace.Name == "System")
            {
                return false;
            }

            if (IsActiveBindingModel(classSymbol.BaseType))
            {
                return true;
            }

            if (!TryGetCrmEntityAttribute(classSymbol, out _))
            {
                return false;
            }

            return HasCrmMappingAttribute(classSymbol);
        }

        public static bool TryGetCrmEntityAttribute(this ITypeSymbol classSymbol, out AttributeData attributeData, bool deep = true)
        {
            if (classSymbol?.BaseType == null || classSymbol.IsAnonymousType || !classSymbol.IsReferenceType || classSymbol.Name == "Object" && classSymbol.ContainingNamespace.Name == "System")
            {
                attributeData = null;
                return false;
            }

            attributeData = classSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass.Name == "CrmEntityAttribute");

            if (attributeData != null)
            {
                return true;
            }

            if (deep)
            {
                foreach (var @interface in classSymbol.AllInterfaces)
                {
                    if (TryGetCrmEntityAttribute(@interface, out attributeData))
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (var @interface in classSymbol.Interfaces)
                {
                    if (TryGetCrmEntityAttribute(@interface, out attributeData, false))
                    {
                        return true;
                    }
                }
            }

            return deep && TryGetCrmEntityAttribute(classSymbol.BaseType, out attributeData);
        }

        internal static bool HasCrmMappingAttribute(this INamedTypeSymbol classSymbol)
        {
            var hasAttribute = classSymbol.GetMembers().OfType<IPropertySymbol>().Any(p => p.GetAttributes().Any(a => a.AttributeClass.Name == "CrmMappingAttribute"));

            hasAttribute &= classSymbol.AllInterfaces.Any(HasCrmMappingAttribute);

            return hasAttribute;
        }

        public static bool IsTypeOrNullableOf(this ITypeSymbol typeSymbol, TypeKind typeKind)
        {
            var isType = typeSymbol.TypeKind == typeKind;

            if (!isType)
            {
                isType = (typeSymbol is INamedTypeSymbol namedTypeSymbol) && typeSymbol.Name == "Nullable" && typeSymbol.ContainingNamespace.Name == "System"
                         && namedTypeSymbol.TypeArguments.Single().IsTypeOrNullableOf(typeKind);
            }

            return isType;
        }

        public static bool IsTypeOrNullableOf(this ITypeSymbol typeSymbol, SpecialType specialType)
        {
            var isType = typeSymbol.SpecialType == specialType;

            if (!isType)
            {
                isType = (typeSymbol is INamedTypeSymbol namedTypeSymbol) && typeSymbol.Name == "Nullable" && typeSymbol.ContainingNamespace.Name == "System"
                         && namedTypeSymbol.TypeArguments.Single().IsTypeOrNullableOf(specialType);
            }

            return isType;
        }

        public static bool IsTypeOrNullableOf(this ITypeSymbol typeSymbol, string typeFullName)
        {
            var isType = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == typeFullName;

            if (!isType)
            {
                isType = (typeSymbol is INamedTypeSymbol namedTypeSymbol) && typeSymbol.Name == "Nullable" && typeSymbol.ContainingNamespace.Name == "System"
                         && namedTypeSymbol.TypeArguments.Single().IsTypeOrNullableOf(typeFullName);
            }

            return isType;
        }

        public static bool IsPrimitiveValueType(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol != null)
            {
                if (typeSymbol.IsValueType && typeSymbol.TypeKind == TypeKind.Enum)
                {
                    return true;
                }

                switch (typeSymbol.SpecialType)
                {
                    case SpecialType.System_Boolean:
                    case SpecialType.System_Char:
                    case SpecialType.System_SByte:
                    case SpecialType.System_Byte:
                    case SpecialType.System_Int16:
                    case SpecialType.System_UInt16:
                    case SpecialType.System_Int32:
                    case SpecialType.System_UInt32:
                    case SpecialType.System_Int64:
                    case SpecialType.System_UInt64:
                    case SpecialType.System_Decimal:
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                    case SpecialType.System_DateTime:
                        return true;
                }
            }

            return false;
        }
    }
}
