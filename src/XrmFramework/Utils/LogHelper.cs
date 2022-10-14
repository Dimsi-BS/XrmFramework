// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using XrmFramework.BindingModel;

namespace XrmFramework
{
    public class DefaultLogHelper : ILogger
    {
        protected IOrganizationService Service { get; }
        public ILoggerContext Context { get; }

        private LogMethod LogMethod { get; }

        public DefaultLogHelper(IOrganizationService service, ILoggerContext context, LogMethod logMethod)
        {
            Service = service;
            Context = context;
            LogMethod = logMethod;
        }

        public void Log(string message, params object[] args)
        {
            LogInternal(message, FormatArgs(args));
        }

        public void LogCollection(IEnumerable<KeyValuePair<string, object>> collection, bool verifyIncluded = false, params string[] excludedIncludedKeys)
        {
            if (collection == null)
            {
                return;
            }

            var sb = new StringBuilder();

            var keyValuePairs = collection.ToList();
            foreach (var kvp in keyValuePairs)
            {
                if (!verifyIncluded && excludedIncludedKeys != null && excludedIncludedKeys.Contains(kvp.Key))
                {
                    continue;
                }

                if (verifyIncluded && excludedIncludedKeys != null && !excludedIncludedKeys.Contains(kvp.Key))
                {
                    continue;
                }

                Log("{0} : {1}", kvp.Key, LogObject(kvp.Value));

            }

            if (verifyIncluded && excludedIncludedKeys != null)
            {
                foreach (var key in excludedIncludedKeys.Where(key => keyValuePairs.All(c => c.Key != key)))
                {
                    Log("{0} not present", key);
                }
            }
        }

        private string LogObject(object parameter, string prefix = "")
        {
            var sb = new StringBuilder();
            if (parameter != null)
            {
                var parameterType = parameter.GetType();

                sb.AppendFormat(" type {0} : ", parameterType.Name);

                switch (parameter)
                {
                    case Entity valueEntity:
                        LogEntity(sb, valueEntity, $"\t{prefix}");
                        break;
                    case Microsoft.Xrm.Sdk.EntityReference valueRef:
                        sb.Append(valueRef.LogicalName);
                        if (valueRef.Id != Guid.Empty)
                        {
                            sb.Append("|").Append(valueRef.Id);
                        }
                        else
                        {
                            foreach (var keyAttribute in valueRef.KeyAttributes)
                            {
                                sb.AppendFormat("|{0}|{1}", keyAttribute.Key, keyAttribute.Value);
                            }
                        }

                        break;
                    case OptionSetValue valueCode:
                        sb.AppendFormat("Value={0}", valueCode.Value);
                        break;
                    case OptionSetValueCollection optionsetCollection:
                        sb.Append("[");
                        foreach (var option in optionsetCollection)
                        {
                            sb.Append("\r\n{");
                            sb.AppendFormat("Value={0}", option.Value);
                            sb.Append("}");
                        }
                        sb.Append("]");
                        break;
                    case Money mValue:
                        sb.AppendFormat("Value={0}", mValue.Value);
                        break;
                    case string[] stValue:
                        sb.Append(string.Join(", ", stValue));
                        break;
                    default:
                        sb.AppendFormat("{0}", parameter);
                        break;
                }

            }
            else
            {
                sb.Append("null");
            }

            return sb.ToString();
        }

        public virtual string LogEntity(Entity entity)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("LogicalName={0}, Id={1}", entity.LogicalName, entity.Id);

            foreach (var attribute in entity.Attributes)
            {
                LogAttribute(sb, attribute.Key, attribute.Value);
            }
            return sb.ToString();
        }

        private void LogEntity(StringBuilder sb, Entity entity, string prefix = "")
        {
            sb.AppendFormat("{2}LogicalName={0}, Id={1}", entity.LogicalName, entity.Id, prefix);

            foreach (var attribute in entity.Attributes)
            {
                LogAttribute(sb, attribute.Key, attribute.Value, prefix);
            }
        }

        private void LogAttribute(StringBuilder sb, string attributeName, object attributeValue, string prefix = "")
        {
            sb.AppendFormat("\r\n{1}\t\"{0}\" : ", attributeName, prefix);

            if (attributeValue == null)
            {
                sb.Append("null");
            }
            else
            {
                switch (attributeValue)
                {
                    case Microsoft.Xrm.Sdk.EntityReference valueRef:
                        sb.AppendFormat("LogicalName={0}, Id={1}", valueRef.LogicalName, valueRef.Id);
                        break;
                    case OptionSetValue valueCode:
                        sb.AppendFormat("Value={0}", valueCode.Value);
                        break;
                    case OptionSetValueCollection optionsetCollection:
                        sb.Append("[");
                        foreach (var option in optionsetCollection)
                        {
                            sb.Append("\r\n{");
                            sb.AppendFormat("Value={0}", option.Value);
                            sb.Append("}");
                        }
                        sb.Append("]");
                        break;
                    case EntityCollection valueCollection:
                        sb.Append("[");
                        foreach (var entity in valueCollection.Entities)
                        {
                            prefix = prefix + "\t";
                            sb.Append("\r\n{");
                            LogEntity(sb, entity, prefix);
                            sb.Append("}");
                        }
                        sb.Append("]");
                        break;
                    default:
                        sb.AppendFormat("{0}", attributeValue);
                        break;
                }
            }
        }

        protected virtual void LogInternal(string message, params object[] args)
            => LogMethod(message, args);

        private object[] FormatArgs(params object[] formatArgs)
        {
            object[] args = null;

            if (formatArgs != null)
            {
                args = new object[formatArgs.Length];

                for (var i = 0; i < formatArgs.Length; i++)
                {
                    object formattedArg = "null";
                    if (formatArgs[i] != null)
                    {
                        formattedArg = formatArgs[i];
                        var argType = formattedArg.GetType();

                        switch (formattedArg)
                        {
                            case Microsoft.Xrm.Sdk.EntityReference eRValue:
                                formattedArg = $"{eRValue.LogicalName}|{eRValue.Id}";
                                break;
                            case Entity eValue:
                                formattedArg = LogEntity(eValue);
                                break;
                            case OptionSetValue oValue:
                                formattedArg = oValue.Value;
                                break;
                            case Money mValue:
                                formattedArg = mValue.Value;
                                break;
                            case string[] stValue:
                                formattedArg = string.Join(", ", stValue);
                                break;
                        }

                        if (typeof(IBindingModel).IsAssignableFrom(argType))
                        {
                            var sb = new StringBuilder();
                            sb.AppendFormat("\t\t{0}\r\n", argType.FullName);
                            foreach (var property in argType.GetProperties(System.Reflection.BindingFlags.Public))
                            {
                                sb.AppendFormat("\t\t{0} : {1}\r\n", property.Name, property.GetValue(formattedArg));
                            }
                            formattedArg = sb.ToString();
                        }

                        if (argType.Name != "String" && typeof(IEnumerable).IsAssignableFrom(argType) && argType.GetMethod("Count") != null)
                        {
                            formattedArg =
                                $"Count({argType.GetMethod("Count").Invoke(formattedArg, new object[] { })})";
                        }

                    }
                    args[i] = formattedArg;
                }
            }

            return args;
        }

        public void LogWithMethodName(string methodName, string message, params object[] args) => LogInternal($"{methodName} : {message}", FormatArgs(args));

        public virtual void LogError(Exception e, string message = null, params object[] args) => LogInternal("ERROR : {0}\r\n{1}", message == null ? string.Empty : string.Format(message, FormatArgs(args)), e);

        public virtual void DumpLog()
        {
        }
    }

    public interface ILogger
    {
        void LogWithMethodName(string methodName, string message, params object[] formatArgs);

        void Log(string message, params object[] args);

        void LogError(Exception e, string message = null, params object[] args);

        void LogCollection(IEnumerable<KeyValuePair<string, object>> collection, bool verifyIncluded = false, params string[] excludedIncludedKeys);

        void DumpLog();
    }
}
