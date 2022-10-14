// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Text;

namespace XrmFramework.DeployUtils
{
    public static class ObjectHelper<T>
    {
        public static void ApplyCode(Type[] constructorTypes, object[] constuctorParams, Func<T, Type, StringBuilder, bool> callback, bool failOnError = true)
        {
            var types = typeof(T).Assembly.GetTypes();

            var filteredTypes = types.Where(t => typeof(T).IsAssignableFrom(t) && t.IsPublic && !t.IsAbstract);

            var sb = new StringBuilder();
            var hasError = false;

            foreach (Type type in filteredTypes)
            {
                var constructor = type.GetConstructor(constructorTypes);
                if (constructor == null)
                {
                    throw new Exception($"The type {type.Name} should have a public parameterless constructor");
                }
                try
                {
                    var obj = (T)constructor.Invoke(constuctorParams);

                    var interfaceType = obj.GetType().GetInterfaces().FirstOrDefault(i => i.GetInterfaces().Any());

                    if (callback != null)
                    {
                        hasError |= callback(obj, interfaceType, sb);
                    }
                }
                catch (Exception e)
                {
                    hasError = true;
                    if (e.InnerException != null)
                    {
                        sb.AppendFormat("Error : {0}\r\n", e.InnerException.Message);
                    }
                    else
                    {
                        sb.AppendFormat("Error : {0}\r\n", e.Message);
                    }
                }
            }

            if (hasError)
            {
                throw new Exception(sb.ToString());
            }
        }
    }
}
