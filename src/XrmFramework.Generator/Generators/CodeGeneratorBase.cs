using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;

namespace XrmFramework.DeployUtils.Generators
{
    public abstract class CodeGeneratorBase
    {
        /// <summary>
        ///     Gets the namespaces required for a list of <see cref="MigrationOperation" /> objects.
        /// </summary>
        /// <param name="operations"> The operations. </param>
        /// <returns> The namespaces. </returns>
        protected IEnumerable<string> GetNamespaces([NotNull] IEnumerable<MethodInfo> methods)
        {
            var list = new List<string>();
            foreach (var m in methods)
            {
                var tempList = GetAttributeNamespaces(m.CustomAttributes).ToList();

                    tempList.AddRange(m.GetGenericArguments().SelectMany(t => t.GetNamespaces()));
                    tempList.AddRange(m.ReturnType.GetNamespaces());
                    tempList.AddRange(m.GetParameters()
                        .SelectMany(p => 
                            p.ParameterType.GetNamespaces()
                            .Concat(GetAttributeNamespaces(p.CustomAttributes))));
                    tempList.AddRange(m.GetGenericArguments()
                        .SelectMany(g => g.GetGenericParameterConstraints()
                            .SelectMany(c => c.GetNamespaces())));
                list.AddRange(tempList);
            }

            return list;
        }

        protected IEnumerable<string> GetAttributeNamespaces(IEnumerable<CustomAttributeData> attributes)
            => attributes.Where(a => !a.AttributeType.IsNotPublic && a.AttributeType.FullName != "System.Runtime.InteropServices.OptionalAttribute")
                .SelectMany(a => a.AttributeType.GetNamespaces()
                .Concat(a.ConstructorArguments.SelectMany(ca => ca.ArgumentType.GetNamespaces())));
    }
}
