using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Model.Sdk;
using System.Collections.Immutable;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace XrmFramework.Analyzers.Utils
{
    public class ModelAttributeDefinition : Definition<IPropertySymbol>
    {
        public ModelDefinition ContainingModel { get; }

        public ModelDefinition AssociatedDefinition => ModelDefinition.GetModelDefinition(Symbol.Type);

        public AttributeDefinition AttributeDefinition { get; }

        public bool IsModelExtension { get; }

        public bool IsValidForUpdate { get; } = true;

        public bool FollowLink { get; }

        public INamedTypeSymbol TypeConverter { get; }

        public AttributeDefinition ExplicitLinkedAttributeDefinition { get; }

        private ModelAttributeDefinition(ModelDefinition containingModel, IPropertySymbol property, ImmutableArray<AttributeData> attributes) : base(property)
        {
            ContainingModel = containingModel;

            var crmMappingAttribute = attributes.FirstOrDefault(a => a.AttributeClass?.Name == "CrmMappingAttribute");
            if (crmMappingAttribute != null)
            {
                var constructorArgument = crmMappingAttribute.ConstructorArguments.First();
                var targetAttributeName = constructorArgument.Value as string ?? throw new ArgumentNullException(nameof(attributes));

                AttributeDefinition = ContainingModel.EntityDefinition[targetAttributeName];

                if (crmMappingAttribute.NamedArguments.Any(kvp => kvp.Key == "IsValidForUpdate"))
                {
                    IsValidForUpdate = (bool)crmMappingAttribute.NamedArguments.First(kvp => kvp.Key == "IsValidForUpdate").Value.Value;
                }
                if (crmMappingAttribute.NamedArguments.Any(kvp => kvp.Key == "FollowLink"))
                {
                    FollowLink = (bool)crmMappingAttribute.NamedArguments.First(kvp => kvp.Key == "FollowLink").Value.Value;
                }

                if (attributes.Any(a => a.AttributeClass.Name == "CrmLookupAttribute"))
                {
                    var lookupAttribute = attributes.FirstOrDefault(a => a.AttributeClass?.Name == "CrmLookupAttribute");
                    var targetDefinition = EntityDefinition.GetEntityDefinition((INamedTypeSymbol)lookupAttribute.ConstructorArguments.First().Value);
                    ExplicitLinkedAttributeDefinition = targetDefinition[(string)lookupAttribute.ConstructorArguments.Skip(1).First().Value];
                }

                if (attributes.Any(a => a.AttributeClass?.Name == "TypeConverterAttribute" && a.AttributeClass.ContainingNamespace.ToDisplayString() == "System.ComponentModel"))
                {
                    var typeConverterAttribute = attributes.FirstOrDefault(a => a.AttributeClass.Name == "TypeConverterAttribute" && a.AttributeClass.ContainingNamespace.ToDisplayString() == "System.ComponentModel");
                    if (!(typeConverterAttribute.AttributeClass is IErrorTypeSymbol))
                    {
                        TypeConverter = (INamedTypeSymbol)typeConverterAttribute.ConstructorArguments.First().Value;
                    }
                }
            }

            IsModelExtension = attributes.Any(a => a.AttributeClass.Name == "ExtendBindingModelAttribute");
        }

        internal static bool TryGetDefinition(ModelDefinition modelDefinition, IPropertySymbol property, out ModelAttributeDefinition modelAttributeDefinition)
        {
            var attributes = property.GetAttributes();

            if (attributes.All(a => a.AttributeClass.Name != "CrmMappingAttribute" && a.AttributeClass.Name != "ExtendBindingModelAttribute"))
            {
                modelAttributeDefinition = null;
                return false;
            }

            modelAttributeDefinition = new ModelAttributeDefinition(modelDefinition, property, attributes);
            return true;
        }

        public ICollection<StatementSyntax> GetFillEntitySyntax()
        {
            var list = new List<StatementSyntax>();
            if (IsModelExtension)
            {
                list.Add(SyntaxFactory.ParseStatement($"record.MergeWith({Name}?.ToEntity(service));\r\n"));
            }
            else
            {
                if (GetIsDirectCast(AttributeDefinition))
                {
                    list.Add(SyntaxFactory.ParseStatement($"record[{AttributeDefinition.FullName}] = {Name};\r\n"));
                }
                else if (!string.IsNullOrEmpty(FillEntityConversionText))
                {
                    list.Add(SyntaxFactory.ParseStatement(FillEntityConversionText));
                }
                else if (TypeConverter != null)
                {
                    list.Add(SyntaxFactory.ParseStatement($"var converter{Name} = new {TypeConverter.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}();\r\n"));
                    list.Add(SyntaxFactory.ParseStatement($"record[{AttributeDefinition.FullName}] = converter{Name}.ConvertFrom({Name});\r\n"));
                }

                if (ContainingModel.IsBindingModelBase)
                {
                    var syntax = ((IfStatementSyntax)SyntaxFactory.ParseStatement($"if (InitializedProperties.Contains(nameof({Name})))"))
                        .WithStatement(SyntaxFactory.Block().AddStatements(list.ToArray()).WithTrailingTrivia("\r\n\r\n"));
                    list.Clear();

                    list.Add(syntax);
                }
            }

            return list;
        }

        public ICollection<StatementSyntax> GetToBindingModelSyntax(SubEntitySyntaxGenerator generator)
        {
            var list = new List<StatementSyntax>();

            var recordName = "record";
            var attributeDefinition = AttributeDefinition;
            if ((AssociatedDefinition != null && !IsModelExtension) || ExplicitLinkedAttributeDefinition != null)
            {
                if (generator.GetSubSyntax(AttributeDefinition, out var syntax, out recordName))
                {
                    list.Add(syntax);
                }
            }

            if (ExplicitLinkedAttributeDefinition != null)
            {
                attributeDefinition = ExplicitLinkedAttributeDefinition;
            }

            if (AssociatedDefinition != null)
            {
                list.Add(SyntaxFactory.ParseStatement($"model.{Name} = {Symbol.Type.ToDisplayString()}.ToBindingModel({recordName});\r\n"));
            }
            else if (GetIsDirectCast(attributeDefinition))
            {
                list.Add(SyntaxFactory.ParseStatement($"model.{Name} = {recordName}.GetAttributeValue<{Symbol.Type.ToDisplayString()}>({attributeDefinition.FullName});\r\n"));
            }
            else if (!string.IsNullOrEmpty(GetToBindingModelConversionText(attributeDefinition)))
            {
                list.Add(SyntaxFactory.ParseStatement(GetToBindingModelConversionText(attributeDefinition)));
            }

            if (TypeConverter != null)
            {
                var syntax = ((IfStatementSyntax)SyntaxFactory.ParseStatement($"if ({recordName}.Contains({attributeDefinition.FullName})\r\n"))
                    .WithStatement(SyntaxFactory.Block(
                        SyntaxFactory.ParseStatement($"var converter{Name} = new {TypeConverter.ToDisplayString()}();\r\n"),
                        SyntaxFactory.ParseStatement($"model.{Name} = ({Symbol.Type.ToDisplayString()}) converter{Name}.ConvertFrom({recordName}[{attributeDefinition.FullName}]);\r\n")
                    ));

                list.Add(syntax);
            }

            return list;
        }

        public bool GetIsDirectCast(AttributeDefinition attributeDefinition)
        {
            switch (attributeDefinition.Type)
            {
                case AttributeTypeCode.Boolean:
                    return Symbol.Type.IsTypeOrNullableOf(SpecialType.System_Boolean);
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.Lookup:
                    return Symbol.Type.Name == "EntityReference" && Symbol.Type.ContainingNamespace.Name == "Microsoft.Xrm.Sdk";
                case AttributeTypeCode.DateTime:
                    return Symbol.Type.IsTypeOrNullableOf(SpecialType.System_DateTime);
                case AttributeTypeCode.Decimal:
                    return Symbol.Type.IsTypeOrNullableOf(SpecialType.System_Decimal);
                case AttributeTypeCode.Double:
                    return Symbol.Type.IsTypeOrNullableOf(SpecialType.System_Double);
                case AttributeTypeCode.Integer:
                    return Symbol.Type.IsTypeOrNullableOf(SpecialType.System_Int32);
                case AttributeTypeCode.Money:
                    return Symbol.Type.Name == "Money" && Symbol.Type.ContainingNamespace.Name == "Microsoft.Xrm.Sdk";
                case AttributeTypeCode.Picklist:
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                    return Symbol.Type.Name == "OptionSetValue" && Symbol.Type.ContainingNamespace.Name == "Microsoft.Xrm.Sdk";
                case AttributeTypeCode.Memo:
                case AttributeTypeCode.EntityName:
                case AttributeTypeCode.String:
                    return Symbol.Type.SpecialType == SpecialType.System_String;
                case AttributeTypeCode.Uniqueidentifier:
                    return Symbol.Type.TypeKind == TypeKind.Struct && Symbol.Type.Name == "Guid";
                default:
                    return false;
            }
        }

        public string FillEntityConversionText
        {
            get
            {
                switch (AttributeDefinition.Type)
                {
                    case AttributeTypeCode.Customer:
                    case AttributeTypeCode.Owner:
                    case AttributeTypeCode.Lookup:
                        if (Symbol.Type.IsTypeOrNullableOf("System.Guid"))
                        {
                            var relationship = AttributeDefinition.Relationships.Single();
                            return $"record[{AttributeDefinition.FullName}] = new EntityReference({relationship.TargetEntityDefinition.Name}.EntityName, {Name});\r\n";
                        }

                        break;
                    case AttributeTypeCode.Money:
                        if (Symbol.Type.IsTypeOrNullableOf(SpecialType.System_Decimal))
                        {
                            if (Symbol.Type.SpecialType == SpecialType.System_Decimal)
                            {
                                return $"new Money({Name});\r\n";
                            }

                            return $"record[{AttributeDefinition.FullName}] = {Name}.HasValue ? new Money({Name}.Value) : null;\r\n";
                        }

                        break;
                    case AttributeTypeCode.Picklist:
                    case AttributeTypeCode.State:
                    case AttributeTypeCode.Status:
                        if (Symbol.Type.IsTypeOrNullableOf(SpecialType.System_Int32))
                        {
                            if (Symbol.Type.SpecialType == SpecialType.System_Int32)
                            {
                                return $"record[{AttributeDefinition.FullName}] = new OptionSetValue({Name});\r\n";
                            }

                            return $"record[{AttributeDefinition.FullName}] = {Name}.HasValue ? new OptionSetValue({Name}.Value) : null;\r\n";
                        }

                        if (Symbol.Type.IsTypeOrNullableOf(TypeKind.Enum))
                        {
                            if (Symbol.Type.TypeKind == TypeKind.Enum)
                            {
                                return $"record.SetOptionSetValue({AttributeDefinition.FullName}, {Name});\r\n";
                            }

                            return $"if ({Name}.HasValue) \r\n{{\r\nrecord.SetOptionSetValue({AttributeDefinition.FullName}, {Name}.Value);\r\n}} else {{\r\nrecord[{AttributeDefinition.FullName}] = null; \r\n}}\r\n";
                        }

                        break;
                }

                return null;
            }
        }

        public string GetToBindingModelConversionText(AttributeDefinition attributeDefinition)
        {
            switch (attributeDefinition.Type)
            {
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.Lookup:
                    if (Symbol.Type.IsTypeOrNullableOf("System.Guid"))
                    {
                        var result = $"model.{Name} = record.GetAttributeValue<EntityReference>({attributeDefinition.FullName})?.Id";

                        if (Symbol.Type.Name == "Guid")
                        {
                            result += " ?? Guid.Empty";
                        }

                        result += ";\r\n";

                        return result;
                    }

                    break;
                case AttributeTypeCode.Money:
                    if (Symbol.Type.IsTypeOrNullableOf(SpecialType.System_Decimal))
                    {
                        var result = $"model.{Name} = record.GetAttributeValue<Money>({attributeDefinition.FullName})?.Value";

                        if (Symbol.Type.SpecialType == SpecialType.System_Decimal)
                        {
                            result += " ?? default(decimal)";
                        }

                        result += ";\r\n";

                        return result;
                    }

                    break;
                case AttributeTypeCode.Picklist:
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                    if (Symbol.Type.IsTypeOrNullableOf(SpecialType.System_Int32))
                    {
                        var result = $"model.{Name} = record.GetAttributeValue<OptionSetValue>({attributeDefinition.FullName})?.Value";

                        if (Symbol.Type.SpecialType == SpecialType.System_Int32)
                        {
                            result += " ?? default(int)";
                        }

                        result += ";\r\n";

                        return result;
                    }

                    if (Symbol.Type.IsTypeOrNullableOf(TypeKind.Enum))
                    {
                        var content = $"model.{Name} = record.GetOptionSetValue<{Symbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}>({attributeDefinition.FullName});\r\n";

                        if (Symbol.Type.TypeKind != TypeKind.Enum)
                        {
                            content = $"if (record.Contains({attributeDefinition.FullName})) \r\n{{\r\n {content} }}\n\n";
                        }

                        return content;
                    }

                    break;
            }

            return null;
        }

        public override string ToString() => $"{Name} ({AttributeDefinition?.Name ?? "Extension"})";
    }

    public abstract class Definition<T> where T : ISymbol
    {
        protected Definition(T symbol)
        {
            Symbol = symbol;
        }

        internal T Symbol { get; }

        public string Name => Symbol.Name;
    }
}
