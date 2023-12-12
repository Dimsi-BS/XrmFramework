// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using XrmFramework.DeployUtils.Properties;

namespace XrmFramework.DeployUtils.Generators
{
    public class MockGenerator
    {
        private readonly ICSharpHelper Code = new CSharpHelper();

        public static void GenerateMocks(string serviceUtilsProjFileName, IEnumerable<Type> types, Type nullableAttributeType)
        {
            string logServicePath = Path.GetDirectoryName(serviceUtilsProjFileName);

            var filesLoggedServices = new List<TfsHelper.FileInfo>();
            
            foreach (var type in types)
            {
                foreach (var f in GenerateLogServiceFile(logServicePath, type, nullableAttributeType))
                {
                    filesLoggedServices.Add(new TfsHelper.FileInfo(f));
                }
            }
       }

        private static IEnumerable<string> GenerateLogServiceFile(string basePath, Type type, Type nullableAttributeType)
        {
            string loggedServiceName = GetLogServiceName(type.Name);

            var isIService = type.Name == "IService";
            var baseClassName = GetBaseClassName(type.Name);
            string parentLoggedServiceName = isIService ? "ILoggedService" : GetLogServiceName(type.BaseType?.Name ?? "IService");
            var fileName = Path.Combine(basePath, loggedServiceName + ".cs");

            var generator = new LoggedServiceCodeGenerator(nullableAttributeType);

            var fileContent = generator.Generate(type);

            File.WriteAllText(fileName, fileContent);

            var list = new List<string> {
               $"$(MSBuildThisFileDirectory){Path.GetFileName(fileName)}"
            };

            return list;
        }

        private static string GetMethodLog(MethodInfo method, bool start)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("\t\t\tLog(\"{0}\", \"{1}", method.Name, start ? "Start" : "End : duration = {0}");

            var parameters = method.GetParameters();
            int i = start ? 0 : 1;
            var sb2 = new StringBuilder();
            if (!start)
            {
                sb2.AppendFormat("sw.Elapsed");
            }

            if (parameters.Any(p => (start && !p.IsOut) || (!start && p.IsOut)))
            {
                sb.Append(": ");
                foreach (var param in parameters)
                {
                    if ((start && !param.IsOut) || (!start && param.IsOut))
                    {
                        if (i > 0)
                        {
                            sb.Append(", ");
                            sb2.Append(", ");
                        }
                        sb.AppendFormat("{0} = {{{1}}}", param.Name, i++);
                        sb2.Append(param.Name);
                    }
                }
            }

            if (!start && method.ReturnType != typeof(void))
            {
                if (!sb.ToString().Contains(": "))
                {
                    sb.Append(": ");
                }
                if (i > 0)
                {
                    sb.Append(", ");
                    sb2.Append(", ");
                }
                sb.AppendFormat("returnValue = {{{0}}}", i);
                sb2.AppendFormat("returnValue");
            }
            sb.Append("\"");
            if (sb2.Length > 0)
            {
                sb.AppendFormat(", {0}", sb2.ToString());
            }
            sb.Append(");");
            return sb.ToString();
        }

        private static string GetParametersCheck(MethodInfo method, Type nullableAttributeType)
        {
            var sb = new StringBuilder();
            sb.AppendLine("\t\t\t#region Parameters check");
            foreach (var param in method.GetParameters())
            {
                var nullable = param.GetCustomAttribute(nullableAttributeType);
                if (((!param.ParameterType.IsPrimitive && (nullable == null)) && (!param.ParameterType.IsEnum && !param.HasDefaultValue)) && !param.IsOut)
                {
                    if (param.ParameterType == typeof(string))
                    {
                        sb.AppendFormat("\t\t\tif (string.IsNullOrEmpty({0}))\r\n", param.Name);
                    }
                    else if (param.ParameterType == typeof(Guid))
                    {
                        sb.AppendFormat("\t\t\tif ({0} == Guid.Empty)\r\n", param.Name);
                    }
                    else
                    {
                        sb.AppendFormat("\t\t\tif ({0} == null)\r\n", param.Name);
                    }
                    sb.AppendLine("\t\t\t{");
                    sb.AppendFormat("\t\t\t\tthrow new ArgumentNullException(nameof({0}));\r\n", param.Name);
                    sb.AppendLine("\t\t\t}");
                }
            }
            sb.AppendLine("\t\t\t#endregion");
            return sb.ToString();
        }

        private static string GetBaseClassName(string serviceName)
        {
            return serviceName == "IService" ? "DefaultService" : serviceName.Substring(1);
        }

        private static string GetLogServiceName(string serviceName)
        {
            return $"Logged{serviceName.Substring(1)}";
        }

        public static string GenerateTestFile(string basePath, Type type, bool isIService)
        {
            var stubName = GetStubName(type.Name);

            var fileName = Path.Combine(basePath, "Mocks/", stubName + "Tests.cs");

            var sb = new StringBuilder();

            var constructorString = string.Format(Resources.TestConstructorContent, stubName, isIService ? "context" : string.Empty);

            var content = new StringBuilder();

            var methods = new Dictionary<string, int>();

            foreach (var method in type.GetMethods())
            {
                var suffix = string.Empty;
                if (methods.ContainsKey(method.Name))
                {
                    suffix = (++methods[method.Name]).ToString();
                }
                else
                {
                    methods.Add(method.Name, 1);
                }

                content.AppendFormat(Resources.TestOkMethod
                    , stubName
                    , method.Name
                    , suffix
                    , GetOutParameterInitializer(method, true)
                    , method.ContainsGenericParameters ? GetGenericParameters(method, true) : string.Empty
                    , GetParameterDefaultValues(method)
                    );

                content.AppendFormat(Resources.TestKoMethod
                    , stubName
                    , method.Name
                    , suffix
                    , GetOutParameterInitializer(method, true)
                    , method.ContainsGenericParameters ? GetGenericParameters(method, true) : string.Empty
                    , GetParameterDefaultValues(method)
                    , GetDefaultReturnStatement(method)
                    , GetResponderPrototype(method)
                    , GetParameterNames(method, true, true)
                    );
            }

            sb.AppendFormat(Resources.TestFileShell, stubName, constructorString, content);

            File.WriteAllText(fileName, sb.ToString());
            return Path.GetFileName(fileName);
        }

        private static string GenerateMockFile(string basePath, Type type, bool isIService)
        {
            var stubName = GetStubName(type.Name);

            var fileName = Path.Combine(basePath, "Mocks/Services/", stubName + ".cs");

            var sb = new StringBuilder();

            var content = new StringBuilder();

            if (isIService)
            {
                content.AppendFormat("        public {0}(IServiceContext context){2} {1}\r\n", stubName, "{ }", stubName != "MockService" ? " : base(context)" : string.Empty);
                content.AppendLine("");
            }

            var methods = new Dictionary<string, int>();

            var i = 0;

            foreach (var method in type.GetMethods())
            {
                var responderPrototype = GetResponderPrototype(method);
                var responderListName = GetResponderListName(method.Name) + i++;

                var value = string.Empty;

                if (method.ReturnType != typeof(void))
                {
                    value += "var value = ";
                    if (method.ReturnType.IsGenericParameter)
                    {
                        value += $"({GetTypePrettyName(method.ReturnType)})";
                    }
                }

                var returnValue = string.Empty;
                if (method.ReturnType != typeof(void))
                {
                    returnValue = "return value;";
                }

                content.Append("\r\n		");

                content.AppendFormat(Resources.MockMethod
                    , method.Name
                    , responderPrototype
                    , responderListName
                    , GetMethodPrototype(method, true)
                    , GetOutParameterInitializer(method)
                    , value
                    , GetParameterNames(method)
                    , returnValue
                    );
            }

            sb.AppendFormat(Resources.MockFileShell, stubName, !isIService || stubName == "MockService" ? string.Empty : "MockService, ", type.Name, content);

            File.WriteAllText(fileName, sb.ToString());
            return Path.GetFileName(fileName);
        }

        public static void GenerateFiles(string basePath, Type type, bool isIService)
        {
            var csProjFileName = basePath + "Plugins.Checkin.Tests.csproj";

            var fileInfo = new FileInfo(basePath + "Mocks/Services/" + GetStubName(type.Name) + ".cs");
            var fileInfoTest = new FileInfo(basePath + "Mocks/" + GetStubName(type.Name) + "Tests.cs");

            FileStream fileStream;
            if (fileInfo.Exists)
            {
                fileStream = fileInfo.Open(FileMode.Truncate);
            }
            else
            {
                fileStream = fileInfo.Open(FileMode.Create);
            }

            using (var sr = new StreamWriter(fileStream))
            {
                FileStream fileStreamTest;
                if (fileInfoTest.Exists)
                {
                    fileStreamTest = fileInfoTest.Open(FileMode.Truncate);
                }
                else
                {
                    fileStreamTest = fileInfoTest.Open(FileMode.Create);
                }

                using (var srTest = new StreamWriter(fileStreamTest))
                {
                    var testFileContent = new StringBuilder();

                    InitializeMock(sr, type.Name, isIService);
                    InitializeMockTests(srTest, type.Name, isIService);

                    var i = 0;
                    MockType(type, sr, ref i);

                    GenerateTests(type, srTest);

                    CloseClass(sr);
                    CloseClass(srTest);
                }
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(csProjFileName);
            XmlNode root = doc.DocumentElement;

            XmlNode compileNode = root.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.LocalName == "ItemGroup" && n.ChildNodes.Cast<XmlNode>().Any(cn => cn.LocalName == "Compile"));

            var updateProject = false;

            if (compileNode != null)
            {
                var mockInfo = "Mocks\\Services\\" + fileInfo.Name;
                var mockTestsInfo = "Mocks\\" + fileInfoTest.Name;

                if (!compileNode.ChildNodes.Cast<XmlNode>().Any(n => n.Attributes["Include"] != null && n.Attributes["Include"].Value == mockInfo))
                {
                    var tempNode = doc.CreateElement("Compile", "http://schemas.microsoft.com/developer/msbuild/2003");
                    tempNode.SetAttribute("Include", mockInfo);
                    tempNode.RemoveAttribute("xmlns");
                    compileNode.AppendChild(tempNode);
                    updateProject = true;
                }

                if (!compileNode.ChildNodes.Cast<XmlNode>().Any(n => n.Attributes["Include"] != null && n.Attributes["Include"].Value == mockTestsInfo))
                {
                    var tempNode = doc.CreateElement("Compile", "http://schemas.microsoft.com/developer/msbuild/2003");
                    tempNode.SetAttribute("Include", mockTestsInfo);
                    tempNode.RemoveAttribute("xmlns");
                    compileNode.AppendChild(tempNode);
                    updateProject = true;
                }
            }

            if (updateProject)
            {
                doc.Save(csProjFileName);
            }
        }

        private static void GenerateTests(Type type, StreamWriter sb)
        {
            var methods = new Dictionary<string, int>();

            foreach (var method in type.GetMethods())
            {

                var suffix = string.Empty;
                if (methods.ContainsKey(method.Name))
                {
                    suffix = (++methods[method.Name]).ToString();
                }
                else
                {
                    methods.Add(method.Name, 1);
                }

                sb.WriteLine("        #region {0}", method.Name);
                sb.WriteLine("        [TestMethod]");
                sb.WriteLine("        public void {0}{1}Ko()", method.Name, suffix);
                sb.WriteLine("        {");
                sb.WriteLine("            var isException = false;");
                sb.WriteLine("{0}", GetOutParameterInitializer(method, true));
                sb.WriteLine("            try");
                sb.WriteLine("            {");
                sb.WriteLine("                {0}.{1}{3}({2});", GetStubName(type.Name), method.Name, GetParameterDefaultValues(method), method.ContainsGenericParameters ? GetGenericParameters(method, true) : string.Empty);
                sb.WriteLine("            }");
                sb.WriteLine("            catch (NotImplementedException e)");
                sb.WriteLine("            {");
                sb.WriteLine("                Assert.IsTrue(e.Message.Contains(\"{0}\"));", method.Name);
                sb.WriteLine("                isException = true;");
                sb.WriteLine("            }");
                sb.WriteLine("            finally");
                sb.WriteLine("            {");
                sb.WriteLine("                Assert.IsTrue(isException);");
                sb.WriteLine("            }");
                sb.WriteLine("        }");
                sb.WriteLine(string.Empty);
                sb.WriteLine("        [TestMethod]");
                sb.WriteLine("        public void {0}{1}Ok()", method.Name, suffix);
                sb.WriteLine("        {");
                sb.WriteLine("{0}", GetOutParameterInitializer(method, true));
                sb.Write("            {0}.Add{1}Responder(new {2}(({3}) =>", GetStubName(type.Name), method.Name, GetResponderPrototype(method), GetParameterNames(method, true));
                sb.Write(" {");
                sb.Write("{0}", GetDefaultReturnStatement(method));
                sb.Write(" }));\r\n");
                sb.WriteLine(string.Empty);
                sb.WriteLine("            {0}.{1}{3}({2});", GetStubName(type.Name), method.Name, GetParameterDefaultValues(method), method.ContainsGenericParameters ? GetGenericParameters(method, true) : string.Empty);
                sb.WriteLine("        }");
                sb.WriteLine("        #endregion");
                sb.WriteLine(string.Empty);
            }
        }

        private static string GetDefaultReturnStatement(MethodInfo method)
        {
            var sb = new StringBuilder();
            if (method.ReturnType != typeof(void))
            {
                sb.AppendFormat("return default({0});", GetTypePrettyName(method.ReturnType, true));
            }
            return sb.ToString();
        }

        private static string GetParameterDefaultValues(MethodInfo method)
        {
            var sb = new StringBuilder();
            var first = true;
            foreach (var param in method.GetParameters())
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }
                if (param.IsOut)
                {
                    sb.Append("out ");
                }
                else if (param.ParameterType.IsByRef)
                {
                    sb.Append("ref ");
                }
                sb.Append(param.Name);
            }
            return sb.ToString();
        }

        private static void CloseClass(StreamWriter sr)
        {
            sr.WriteLine("    }");
            sr.WriteLine("}");
        }

        private static void InitializeMock(StreamWriter sb, string name, bool isIService)
        {
            sb.WriteLine(Resources.MockFileShell, GetStubName(name), !isIService || GetStubName(name) == "MockService" ? string.Empty : "MockService, ", name);

            if (isIService)
            {
                sb.WriteLine("        public {0}(IServiceContext context){2} {1}", GetStubName(name), "{ }", name != "IService" ? " : base(context)" : string.Empty);
                sb.WriteLine("");
            }
        }

        private static void InitializeMockTests(StreamWriter sb, string name, bool isIService)
        {
            sb.WriteLine(Resources.TestFileShell, GetStubName(name));
            if (isIService)
            {
                sb.WriteLine("            {0} = new {0}(context);", GetStubName(name));
            }
            else
            {
                sb.WriteLine("            {0} = new {0}();", GetStubName(name));
            }
        }

        private static void MockType(Type type, StreamWriter sr, ref int i)
        {

            foreach (var method in type.GetMethods())
            {
                var responderPrototype = GetResponderPrototype(method);
                var responderListName = GetResponderListName(method.Name) + i++;
                sr.WriteLine("        #region {0}", method.Name);
                sr.WriteLine("        private List<{0}> {1} = new List<{0}>();", responderPrototype, responderListName);
                sr.WriteLine(string.Empty);
                sr.WriteLine("        public void Add{0}Responder({1} responder)", method.Name, responderPrototype);
                sr.WriteLine("        {");
                sr.WriteLine("            {0}.Add(responder);", responderListName);
                sr.WriteLine("        }");
                sr.WriteLine(string.Empty);
                sr.WriteLine("        public {0}", GetMethodPrototype(method));
                sr.WriteLine("        {");
                sr.Write(GetOutParameterInitializer(method));
                sr.WriteLine("            if ({0}.Count > 0)", responderListName);
                sr.WriteLine("            {");
                sr.WriteLine("                var responder = {0}.First();", responderListName);

                sr.Write("                ");
                if (method.ReturnType != typeof(void))
                {
                    sr.Write("var value = ");
                    if (method.ReturnType.IsGenericParameter)
                    {
                        sr.Write("({0})", GetTypePrettyName(method.ReturnType));
                    }
                }
                sr.Write("responder({0});\r\n", GetParameterNames(method));
                sr.WriteLine("                {0}.RemoveAt(0);", responderListName);
                if (method.ReturnType != typeof(void))
                {
                    sr.WriteLine("                return value;");
                }

                sr.WriteLine("            }");
                sr.WriteLine("            else");
                sr.WriteLine("            {");
                sr.WriteLine("                throw new NotImplementedException(\"Unexpected call to the method {0}\");", method.Name);
                sr.WriteLine("            }");
                sr.WriteLine("        }");
                sr.WriteLine("        #endregion");
                sr.WriteLine(string.Empty);
            }
        }

        private static string GetOutParameterInitializer(MethodInfo method, bool displayAll = false)
        {
            var sb = new StringBuilder();

            foreach (var param in method.GetParameters())
            {
                if (!displayAll && !param.IsOut && !param.ParameterType.IsByRef)
                {
                    continue;
                }

                sb.AppendFormat("            {0}{1} = default({2});\r\n", displayAll ? "var " : string.Empty, param.Name, GetTypePrettyName(param.ParameterType));
            }
            return sb.ToString();
        }

        private static string GetMethodPrototype(MethodInfo method, bool displayTypes = true)
        {
            var sb = new StringBuilder();

            if (displayTypes)
            {
                if (method.ReturnType == typeof(void))
                {
                    sb.Append("void ");
                }
                else
                {
                    sb.AppendFormat("{0} ", GetTypePrettyName(method.ReturnType));
                }
            }
            sb.AppendFormat("{0}", method.Name);
            if (method.ContainsGenericParameters)
            {
                sb.Append(GetGenericParameters(method, false));
            }
            sb.Append("(");

            var first = true;
            foreach (var param in method.GetParameters())
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }
                if (param.IsOut)
                {
                    sb.Append("out ");
                }
                else if (param.ParameterType.IsByRef)
                {
                    sb.Append("ref ");
                }

                sb.AppendFormat("{0} {1}", displayTypes ? (GetTypePrettyName(param.ParameterType) + " ") : string.Empty, param.Name);

                if (param.HasDefaultValue && displayTypes)
                {
                    if (param.DefaultValue is bool)
                    {
                        var b = (bool)param.DefaultValue;
                        sb.AppendFormat(" = {0}", b.ToString().ToLowerInvariant());
                    }
                    else if (param.DefaultValue == null)
                    {
                        sb.Append(" = null");
                    }
                    else if (param.DefaultValue.GetType().IsEnum)
                    {
                        sb.AppendFormat(" = {0}.{1}", param.DefaultValue.GetType().FullName, param.DefaultValue);
                    }
                    else
                    {
                        sb.AppendFormat(" = {0}", param.DefaultValue);
                    }
                }

            }

            sb.Append(")");

            if (method.ContainsGenericParameters && displayTypes)
            {
                sb.Append(GetGenericParametersConstraints(method));
            }

            return sb.ToString();
        }

        private static string GetGenericParametersConstraints(MethodInfo method)
        {
            var sb = new StringBuilder();

            foreach (var tp in method.GetGenericArguments())
            {
                if (tp.IsGenericParameter)
                {
                    var classConstraint = tp.BaseType == typeof(object) ? null : tp.BaseType;

                    var sbParam = new StringBuilder();
                    sbParam.Append($" where {tp.Name}");
                    var hasConstraints = false;

                    if (classConstraint != null)
                    {
                        hasConstraints = true;
                        sbParam.Append($" : {GetTypePrettyName(classConstraint)}");
                    }

                    foreach (var iConstraint in tp.GetGenericParameterConstraints())
                    {
                        if (iConstraint.IsInterface)
                        {
                            if (hasConstraints)
                            {
                                sbParam.Append(", ");
                            }
                            else
                            {
                                sbParam.Append(" : ");
                                hasConstraints = true;
                            }

                            sbParam.Append(GetTypePrettyName(iConstraint));
                        }
                    }

                    GenericParameterAttributes sConstraints =
                        tp.GenericParameterAttributes &
                        GenericParameterAttributes.SpecialConstraintMask;

                    if (sConstraints == GenericParameterAttributes.None)
                    {
                        // No special constraints.
                    }
                    else
                    {
                        if (GenericParameterAttributes.None != (sConstraints &
                                                                GenericParameterAttributes.DefaultConstructorConstraint))
                        {
                            if (hasConstraints)
                            {
                                sbParam.Append(", ");
                            }
                            else
                            {
                                sbParam.Append(" : ");
                                hasConstraints = true;
                            }
                            sbParam.Append("new()");
                        }
                        if (GenericParameterAttributes.None != (sConstraints &
                                                                GenericParameterAttributes.ReferenceTypeConstraint))
                        {
                            Console.WriteLine("Must be a reference type.");
                        }
                        if (GenericParameterAttributes.None != (sConstraints &
                                                                GenericParameterAttributes.NotNullableValueTypeConstraint))
                        {
                            Console.WriteLine("Must be a non-nullable value type.");
                        }
                    }

                    if (hasConstraints)
                    {
                        sb.Append(sbParam);
                    }
                }
            }

            return sb.ToString();
        }



        private static string GetTypePrettyName(Type t, bool isResponder = false)
        {
            var sb = new StringBuilder();
            if (t.IsGenericType)
            {
                if (t.Name.Contains("IEnumerable"))
                {
                    sb.Append("IEnumerable");
                }
                else if (t.Name.Contains("ICollection"))
                {
                    sb.Append("ICollection");
                }
                else if (t.Name.Contains("Nullable"))
                {
                    sb.Append("Nullable");
                }
                else if (t.Name.Contains("IList"))
                {
                    sb.Append("IList");
                }
                else if (t.Name.Contains("IDictionary"))
                {
                    sb.Append("IDictionary");
                }
                else if (t.Name.Contains("Action"))
                {
                    sb.Append("Action");
                }
                if (t.GenericTypeArguments != null && t.GenericTypeArguments.Length > 0)
                {
                    sb.Append("<");

                    var first = true;
                    foreach (var type in t.GenericTypeArguments)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            sb.Append(", ");
                        }
                        sb.Append(GetTypePrettyName(type));
                    }

                    sb.Append(">");
                }
            }
            else if (t.IsNested && !t.ContainsGenericParameters)
            {
                sb.AppendFormat("{0}.{1}", GetTypePrettyName(t.ReflectedType), t.Name);
            }
            else if (t.IsByRef)
            {
                sb.Append(t.Name.Remove(t.Name.Length - 1));
            }
            else if (t.ContainsGenericParameters && isResponder)
            {
                sb.Append("object");
            }
            else
            {
                sb.Append(t.Name);
            }
            return sb.ToString();
        }

        private static string GetParameterNames(MethodInfo method, bool suffixParams = false, bool isTest = false)
        {
            var sb = new StringBuilder();

            var first = true;
            foreach (var paramType in method.GetGenericArguments())
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }
                if (isTest)
                {
                    sb.Append(paramType.Name.ToLowerInvariant());
                }
                else
                {
                    sb.AppendFormat("typeof({0})", GetTypePrettyName(paramType));
                }
            }

            foreach (var param in method.GetParameters())
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }
                sb.AppendFormat("{0}{1}", param.Name, suffixParams ? "Param" : string.Empty);
            }

            return sb.ToString();
        }

        private static string GetResponderListName(string methodName)
        {
            return $"_{methodName.Substring(0, 1).ToLowerInvariant()}{methodName.Substring(1)}Responders";
        }

        private static string GetGenericParameters(MethodInfo method, bool isTest = false)
        {
            var sb = new StringBuilder();

            sb.Append("<");

            var first = true;
            foreach (var paramType in method.GetGenericArguments())
            {
                if (paramType.IsGenericParameter)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                    if (isTest)
                    {
                        sb.Append("object");
                    }
                    else
                    {
                        sb.Append(GetTypePrettyName(paramType, false));
                    }
                }
            }

            sb.Append(">");

            return sb.ToString();
        }

        private static string GetResponderPrototype(MethodInfo method)
        {
            var callType = "Func";
            if (method.ReturnType == typeof(void))
            {
                callType = "Action";
            }
            var sb = new StringBuilder();


            var genericArguments = method.GetGenericArguments();
            var parameters = method.GetParameters();

            var hasAnyType = genericArguments.Any() || parameters.Any() || method.ReturnType != typeof(void);

            sb.AppendFormat("{0}{1}", callType, hasAnyType ? "<" : string.Empty);

            var first = true;

            if (genericArguments.Any())
            {
                for (var i = 0; i < genericArguments.Length; i++)
                {
                    if (first) { first = false; } else { sb.Append(", "); }
                }
                sb.Append("Type");
            }

            foreach (var param in parameters)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }
                sb.Append(GetTypePrettyName(param.ParameterType, true));
            }
            if (method.ReturnType != typeof(void))
            {
                if (!first) { sb.Append(", "); }
                sb.Append(GetTypePrettyName(method.ReturnType, true));
            }

            if (hasAnyType)
            {
                sb.Append(">");
            }

            return sb.ToString();
        }

        private static string GetStubName(string serviceName)
        {
            return $"Mock{serviceName.Substring(1)}";
        }
    }
}
