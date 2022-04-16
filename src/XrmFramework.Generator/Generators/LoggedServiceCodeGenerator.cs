using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Generators;

namespace XrmFramework.Generator.Generators
{
    public class LoggedServiceCodeGenerator : CodeGeneratorBase
    {
        private readonly ICSharpHelper _code;
        private readonly Type _nullableType;

        public LoggedServiceCodeGenerator(Type nullableType)
        {
            _code = new CSharpHelper();
            _nullableType = nullableType;
        }

        public string Generate(Type serviceType)
        {
            var namespaces = new List<string> { "System.Diagnostics", "System", "System.Runtime.CompilerServices" };

            namespaces.AddRange(serviceType.GetNamespaces());

            if (serviceType.BaseType != default)
            {
                namespaces.Add(serviceType.BaseType.Namespace);
            }

            namespaces.AddRange(serviceType.GetInterfaces().Select(i => i.Namespace));

            namespaces.AddRange(GetNamespaces(serviceType.GetMethods()));

            namespaces.AddRange(serviceType.GetProperties()
                .SelectMany(p => p.PropertyType.GetNamespaces()
                    .Concat(GetAttributeNamespaces(p.CustomAttributes))));

            var isIService = serviceType.FullName == "XrmFramework.IService";

            var builder = new IndentedStringBuilder();

            foreach (var ns in namespaces.Where(n => !string.IsNullOrEmpty(n)).OrderBy(n => n).Distinct())
            {
                builder
                    .Append("using ")
                    .Append(ns)
                    .AppendLine(";");
            }

            builder
                .AppendLine()
                .Append("namespace ")
                .AppendLine(serviceType.Namespace ?? throw new NotSupportedException($"Service type {serviceType} has no namespace"))
                .AppendLine("{");

            using (builder.Indent())
            {
                var className = $"Logged{serviceType.Name.Substring(1)}";

                builder
                    .AppendLine("[DebuggerStepThrough, CompilerGenerated]")

                    .Append("public class ")
                    .Append(_code.Identifier(className))
                    .Append(" : ")

                    .Append(!isIService ? "LoggedService, " : "LoggedServiceBase, ")

                    .AppendLine(_code.Reference(serviceType))
                    .AppendLine("{");

                using (builder.Indent())
                {
                    if (!isIService)
                    {
                        builder
                            .Append("protected new ")
                            .Append(_code.Reference(serviceType))
                            .Append(" Service => (")
                            .Append(_code.Reference(serviceType))
                            .AppendLine(") base.Service;");
                    }

                    builder
                        .AppendLine()
                        .AppendLine("#region .ctor")
                        .Append("public ").Append(_code.Identifier(className))
                        .Append("(IServiceContext context, ")
                        .Append(_code.Reference(serviceType))
                        .AppendLine(" service) : base(context, service)");

                    builder
                        .AppendLine("{")
                        .AppendLine("}")
                        .AppendLine("#endregion");

                    foreach (var method in serviceType.GetMethods())
                    {
                        GenerateMethod(method, builder);
                    }
                }

                builder.AppendLine("}");
            }

            builder.AppendLine("}");

            return builder.ToString();
        }

        private void GenerateMethod(MethodInfo m, IndentedStringBuilder builder)
        {
            var isAsyncMethod = typeof(Task).IsAssignableFrom(m.ReturnType);

            builder
                .AppendLine()
                .Append("public ");

            if (m.ReturnType == typeof(void))
            {
                builder.Append("void");
            }
            else
            {
                if (isAsyncMethod)
                {
                    builder.Append("async ");
                }

                builder.Append(_code.Reference(m.ReturnType));
            }

            builder
                .Append(" ");

            GenerateMethodSignature(m, builder, true);

            builder
                .AppendLine()
                .AppendLine("{");

            using (builder.Indent())
            {
                GetParametersCheck(m, builder);

                builder
                    .AppendLine()
                    .AppendLine("var sw = new Stopwatch();")
                    .AppendLine("sw.Start();")
                    .AppendLine();

                GetMethodLog(m, true, builder);

                builder
                    .AppendLine()
                    ;

                if (m.ReturnType != typeof(void) && m.ReturnType != typeof(Task))
                {
                    builder
                        .Append("var returnValue = ");
                }

                if (isAsyncMethod)
                {
                    builder.Append("await ");
                }

                builder
                    .Append("Service.");

                GenerateMethodSignature(m, builder, false);

                builder
                    .AppendLine(";")
                    .AppendLine();

                GetMethodLog(m, false, builder);

                if (m.ReturnType != typeof(void) && m.ReturnType != typeof(Task))
                {
                    builder
                        .AppendLine()
                        .AppendLine("return returnValue;");
                }
            }

            builder.AppendLine("}");
        }

        private void GenerateMethodSignature(MethodInfo m, IndentedStringBuilder builder, bool displayTypes)
        {
            builder.Append(m.Name);

            if (m.ContainsGenericParameters)
            {
                var first = true;

                builder.Append("<");

                foreach (var argType in m.GetGenericArguments().Where(a => a.IsGenericParameter))
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        builder.Append(", ");
                    }

                    builder.Append(argType.Name);
                }

                builder.Append(">");
            }

            builder.Append("(");

            var firstParameter = true;
            foreach (var p in m.GetParameters())
            {
                if (!firstParameter)
                {
                    builder.Append(", ");
                }
                else
                {
                    firstParameter = false;
                }

                if (p.ParameterType.IsByRef)
                {
                    if (p.IsOut)
                    {
                        builder.Append("out ");
                    }
                    else
                    {
                        builder.Append("ref ");
                    }
                }

                if (displayTypes)
                {
                    builder
                        .Append(_code.Reference(p.ParameterType, true))
                        .Append(" ");
                }

                builder
                    .Append(_code.Identifier(p.Name));

                if (p.HasDefaultValue && displayTypes)
                {
                    if (p.DefaultValue == null)
                    {
                        builder.Append(" = null");
                    }
                    else
                    {
                        builder
                            .Append(" = ")
                            .Append(_code.Literal((dynamic)p.DefaultValue));
                    }
                }
            }

            builder.Append(")");

            if (m.ContainsGenericParameters && displayTypes)
            {
                GenerateGenericParametersConstraints(m, builder);
            }
        }

        private void GetMethodLog(MethodInfo method, bool start, IndentedStringBuilder builder)
        {
            builder
                .Append("Log(")
                .Append(_code.Literal((method.DeclaringType != null ? $"{method.DeclaringType.Name}." : "") + method.Name))
                .Append(", \"")
                .Append(start ? "Start" : "End : duration = {0}");

            var parameters = method.GetParameters();
            var i = start ? 0 : 1;

            var sb2 = new StringBuilder();
            if (!start)
            {
                sb2.AppendFormat("sw.Elapsed");
            }

            if (parameters.Any(p => (start && !p.IsOut) || (!start && p.IsOut)))
            {
                builder.Append(": ");
                foreach (var param in parameters)
                {
                    if ((start && !param.IsOut) || (!start && param.IsOut))
                    {
                        if (i > 0)
                        {
                            builder.Append(", ");
                            sb2.Append(", ");
                        }

                        builder.Append(param.Name).Append(" = {").Append(i++).Append("}");
                        sb2.Append(param.Name);
                    }
                }
            }

            if (!start && method.ReturnType != typeof(void) && method.ReturnType != typeof(Task))
            {
                if (!builder.ToString().Contains(": "))
                {
                    builder.Append(": ");
                }
                if (i > 0)
                {
                    builder.Append(", ");
                    sb2.Append(", ");
                }
                builder.Append("returnValue = {").Append(i).Append("}");
                sb2.AppendFormat("returnValue");
            }
            builder.Append("\"");
            if (sb2.Length > 0)
            {
                builder.Append(", ").Append(sb2.ToString());
            }
            builder.AppendLine(");");
        }

        private void GetParametersCheck(MethodInfo method, IndentedStringBuilder builder)
        {
            builder.AppendLine("#region Parameters check");
            foreach (var param in method.GetParameters())
            {
                var nullable = param.GetCustomAttribute(_nullableType);

                if (param.IsOut) continue;
                if (nullable != null) continue;
                if (param.ParameterType.IsPrimitive) continue;
                if (param.ParameterType.IsEnum) continue;
                if (param.HasDefaultValue) continue;
                if (param.ParameterType == typeof(decimal)) continue;

                if (param.ParameterType == typeof(string))
                {
                    builder
                        .Append("if (string.IsNullOrEmpty(")
                        .Append(_code.Identifier(param.Name))
                        .AppendLine("))");
                }
                else if (param.ParameterType == typeof(Guid))
                {
                    builder
                        .Append("if (")
                        .Append(param.Name)
                        .AppendLine(" == Guid.Empty)");
                }
                else
                {
                    builder.Append("if (")
                        .Append(param.Name)
                        .Append(" == null)");
                }

                builder
                    .AppendLine()
                    .AppendLine("{");

                using (builder.Indent())
                {
                    builder
                        .Append("throw new ArgumentNullException(nameof(")
                        .Append(param.Name).AppendLine("));");
                }

                builder.AppendLine("}");
            }
            builder.AppendLine("#endregion");
        }

        private void GenerateGenericParametersConstraints(MethodInfo method, IndentedStringBuilder builder)
        {
            using (builder.Indent())
            {
                foreach (var tp in method.GetGenericArguments())
                {
                    var gpa = tp.GenericParameterAttributes;

                    var typeConstraints = tp.GetGenericParameterConstraints();

                    if (gpa == GenericParameterAttributes.None && !typeConstraints.Any())
                    {
                        continue;
                    }

                    builder
                        .AppendLine()
                        .Append($"where {tp.Name} : ");

                    var constraints = gpa & GenericParameterAttributes.SpecialConstraintMask;

                    var isFirst = true;
                    if (constraints != GenericParameterAttributes.None)
                    {
                        if ((constraints & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
                        {
                            builder.Append("class");
                            isFirst = false;
                        }

                        if ((constraints & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
                        {
                            builder.Append("struct");
                            isFirst = false;
                        }
                    }

                    foreach (var tC in typeConstraints)
                    {
                        if (isFirst)
                        {
                            isFirst = false;
                        }
                        else
                        {
                            builder.Append(", ");
                        }

                        builder.Append(_code.Reference(tC));
                    }

                    if ((constraints & GenericParameterAttributes.DefaultConstructorConstraint) != 0)
                    {
                        if (!isFirst) builder.Append(", ");
                        builder.Append("new()");
                    }
                }
            }
        }
    }
}
