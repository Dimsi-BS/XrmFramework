
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.DeployUtils.Generators
{
    public abstract class CodeGeneratorBase
    {
        /// <summary>
        ///     Gets the namespaces required for a list of <see cref="MigrationOperation" /> objects.
        /// </summary>
        /// <param name="operations"> The operations. </param>
        /// <returns> The namespaces. </returns>
        protected IEnumerable<string> GetNamespaces(IEnumerable<IMethodSymbol> methods)
        {
            var list = new List<string>();
            foreach (var m in methods)
            {
                var tempList = GetAttributeNamespaces(m.GetAttributes()).ToList();

                tempList.AddRange(m.TypeArguments.SelectMany(t => t.GetNamespaces()));
                tempList.AddRange(m.ReturnType.GetNamespaces());
                tempList.AddRange(m.Parameters
                    .SelectMany(p =>
                        p.Type.GetNamespaces()
                        .Concat(GetAttributeNamespaces(p.GetAttributes()))));
                tempList.AddRange(m.TypeParameters
                    .SelectMany(g => g.ConstraintTypes
                        .SelectMany(c => c.GetNamespaces())));
                list.AddRange(tempList);
            }



            return list;
        }

        protected IEnumerable<string> GetAttributeNamespaces(IEnumerable<AttributeData> attributes)
            => attributes
            .Where(a => a.AttributeClass != null)
            .Where(a => a.AttributeClass.DeclaredAccessibility != Accessibility.Public && a.AttributeClass.ToDisplayString() != "System.Runtime.InteropServices.OptionalAttribute")
                .SelectMany(a => a.AttributeClass.GetNamespaces()
                .Concat(a.ConstructorArguments.SelectMany(ca => ca.Type.GetNamespaces())));
    }
}
