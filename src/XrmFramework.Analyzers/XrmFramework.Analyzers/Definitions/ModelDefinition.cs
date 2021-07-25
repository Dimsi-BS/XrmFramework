using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XrmFramework.Analyzers.Utils
{
    public class ModelDefinition : Definition<INamedTypeSymbol>
    {
        public ModelDefinition ParentModel { get; }

        public EntityDefinition EntityDefinition { get; }

        public bool IsBindingModelBase => Symbol.BaseType.Name == "BindingModelBase" || (ParentModel?.IsBindingModelBase ?? false);

        private readonly List<ModelAttributeDefinition> _attributes = new List<ModelAttributeDefinition>();

        public IImmutableList<ModelAttributeDefinition> AllAttributes => ImmutableList.CreateRange(_attributes.Union(ParentModel?.AllAttributes ?? Enumerable.Empty<ModelAttributeDefinition>()));

        public IImmutableList<ModelAttributeDefinition> Attributes => ImmutableList.CreateRange(_attributes);

        private ModelDefinition(INamedTypeSymbol symbol, AttributeData crmEntityAttribute) : base(symbol)
        {
            ParentModel = GetModelDefinition(symbol.BaseType);

            var entityDefinitionSymbol = (INamedTypeSymbol) crmEntityAttribute.ConstructorArguments.Single().Value;

            EntityDefinition = EntityDefinition.GetEntityDefinition(entityDefinitionSymbol);

            foreach (var memberName in symbol.MemberNames)
            {
                var members = symbol.GetMembers(memberName);

                if (!members.OfType<IPropertySymbol>().Any())
                {
                    continue;
                }

                var property = members.OfType<IPropertySymbol>().First();

                if (ModelAttributeDefinition.TryGetDefinition(this, property, out var modelAttributeDefinition))
                {
                    _attributes.Add(modelAttributeDefinition);
                }
            }

            foreach (var interfaceSymbol in symbol.Interfaces)
            {
                foreach (var memberName in interfaceSymbol.MemberNames)
                {
                    var members = interfaceSymbol.GetMembers(memberName);

                    if (!members.OfType<IPropertySymbol>().Any())
                    {
                        continue;
                    }

                    var property = members.OfType<IPropertySymbol>().First();

                    if (ModelAttributeDefinition.TryGetDefinition(this, property, out var modelAttributeDefinition))
                    {
                        _attributes.Add(modelAttributeDefinition);
                    }
                }
            }
        }

        public static ModelDefinition GetModelDefinition(ITypeSymbol symbol)
        {
            if (symbol == null)
            {
                return null;
            }

            if (symbol.Name == "Object" && symbol.ContainingNamespace.Name == "System")
            {
                return null;
            }

            if (symbol.AllInterfaces.All(i => i.Name != "IBindingModel"))
            {
                return null;
            }

            if (!symbol.TryGetCrmEntityAttribute(out var typedConstant))
            {
                return null;
            }

            if (!(symbol is INamedTypeSymbol namedTypeSymbol))
            {
                return null;
            }


            if (!Cache.ContainsKey(namedTypeSymbol))
            {
                lock (SyncRoot)
                {
                    if (!Cache.ContainsKey(namedTypeSymbol))
                    {
                        Cache[namedTypeSymbol] = new ModelDefinition(namedTypeSymbol, typedConstant);
                    }
                }
            }

            return Cache[namedTypeSymbol];
        }

        private static readonly object SyncRoot = new object();

        private static readonly Dictionary<INamedTypeSymbol, ModelDefinition> Cache = new Dictionary<INamedTypeSymbol, ModelDefinition>();

        public bool IsAbstract => Symbol.IsAbstract;

        internal static void ClearCache()
        {
            Cache.Clear();
        }

        public override string ToString() => $"{Name} ({EntityDefinition.Name})";
    }
}
