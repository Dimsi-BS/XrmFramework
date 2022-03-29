using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace XrmFramework.Analyzers.Utils
{
    public class EntityDefinition : Definition<INamedTypeSymbol>
    {
        private readonly List<AttributeDefinition> _attributes = new List<AttributeDefinition>();

        public bool Contains(string attributeName) => _attributes.Any(a => a.LogicalName == attributeName);

        public AttributeDefinition this[string attributeName] => _attributes.Single(a => a.LogicalName == attributeName);

        internal EntityDefinition(INamedTypeSymbol symbol) : base(symbol)
        {
            LogicalName = (string) ((IFieldSymbol) symbol.GetMembers("EntityName").Single()).ConstantValue;
            CollectionLogicalName = (string)((IFieldSymbol)symbol.GetMembers("EntityCollectionName").Single()).ConstantValue;

            var columnsClass = symbol.GetTypeMembers("Columns").Single();

            foreach (var memberName in columnsClass.MemberNames)
            {
                var field = (IFieldSymbol)columnsClass.GetMembers(memberName).Single();

                _attributes.Add(new AttributeDefinition(this, field));
            }
        }

        public string LogicalName { get; }

        public string CollectionLogicalName { get; }

        internal static EntityDefinition GetEntityDefinition(INamedTypeSymbol symbol)
        {
            if (!Cache.ContainsKey(symbol))
            {
                lock (SyncRoot)
                {
                    if (!Cache.ContainsKey(symbol))
                    {
                        Cache[symbol] = new EntityDefinition(symbol);
                    }
                }
            }

            return Cache[symbol];
        }

        private static readonly object SyncRoot = new object();

        private static readonly Dictionary<INamedTypeSymbol, EntityDefinition> Cache = new(SymbolEqualityComparer.Default);

        internal static void ClearCache()
        {
            Cache.Clear();
        }
    }
}
