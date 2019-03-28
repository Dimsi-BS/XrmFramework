// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins
{
    public class LogHelper
    {
        public TraceLogger LogMethod { get; set; }

        public LogHelper(TraceLogger logMethod)
        {
            LogMethod = logMethod;
        }

        public void Log(string methodName, string message, params object[] formatArgs)
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

                        switch (argType.Name)
                        {
                            case "EntityReference":
                                var eRValue = (EntityReference)formattedArg;
                                formattedArg = string.Format("{0}|{1}", eRValue.LogicalName, eRValue.Id);
                                break;
                            case "Entity":
                                formattedArg = DumpEntity((Entity)formattedArg);
                                break;
                            case "OptionSetValue":
                                formattedArg = ((OptionSetValue)formattedArg).Value;
                                break;
                            case "Money":
                                formattedArg = ((Money)formattedArg).Value;
                                break;
                            case "String[]":
                                formattedArg = ((string[])formattedArg).Aggregate(string.Empty, (last, column) =>
                                                                        {
                                                                            var value = last;
                                                                            if (last != string.Empty)
                                                                            {
                                                                                value = value + ", ";
                                                                            }
                                                                            value = value + column;
                                                                            return value;
                                                                        });
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
                            formattedArg = string.Format("Count({0})", argType.GetMethod("Count").Invoke(formattedArg, new object[] { }));
                        }

                    }
                    args[i] = formattedArg;
                }
            }

            LogMethod("{0} : {1}", methodName, string.Format(CultureInfo.CurrentCulture, message, args));
        }

        public void DumpObject(string parameterName, object parameter)
        {

            var sb = new StringBuilder();
            if (parameter != null)
            {
                var parameterType = parameter.GetType();

                sb.AppendFormat("\r\n{0} type {1} : ", parameterName, parameterType.Name);

                switch (parameterType.Name)
                {
                    case "Entity":
                        var valueEntity = parameter as Entity;
                        DumpEntity(sb, valueEntity);
                        break;
                    case "EntityReference":
                        var valueRef = parameter as EntityReference;
                        sb.AppendFormat("{0}|{1}", valueRef.LogicalName, valueRef.Id);
                        break;
                    case "OptionSetValue":
                        var valueCode = parameter as OptionSetValue;
                        sb.AppendFormat("Value={0}", valueCode.Value);
                        break;
                    default:
                        sb.AppendFormat("{0}", parameter);
                        break;
                }

                LogMethod(sb.ToString());
            }
            else
            {
                LogMethod("{0} is null", parameterName);
            }
        }

        public virtual string DumpEntity(Entity entity)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("LogicalName={0}, Id={1}", entity.LogicalName, entity.Id);

            foreach (var attribute in entity.Attributes)
            {
                DumpAttribute(sb, attribute.Key, attribute.Value);
            }
            return sb.ToString();
        }
        public void DumpEntity(StringBuilder sb, Entity entity, string prefix = "")
        {
            sb.AppendFormat("{2}LogicalName={0}, Id={1}", entity.LogicalName, entity.Id, prefix);

            foreach (var attribute in entity.Attributes)
            {
                DumpAttribute(sb, attribute.Key, attribute.Value, prefix);
            }
        }
        private void DumpAttribute(StringBuilder sb, string attributeName, object attributeValue, string prefix = "")
        {
            sb.AppendFormat("\r\n{1}\t\"{0}\" : ", attributeName, prefix);

            if (attributeValue == null)
            {
                sb.Append("null");
            }
            else
            {
                switch (attributeValue.GetType().Name)
                {
                    case "EntityReference":
                        var valueRef = attributeValue as EntityReference;
                        sb.AppendFormat("LogicalName={0}, Id={1}", valueRef.LogicalName, valueRef.Id);
                        break;
                    case "OptionSetValue":
                        var valueCode = attributeValue as OptionSetValue;
                        sb.AppendFormat("Value={0}", valueCode.Value);
                        break;
                    case "EntityCollection":
                        var valueCollection = attributeValue as EntityCollection;
                        sb.Append("[");
                        foreach (var entity in valueCollection.Entities)
                        {
                            prefix = prefix + "\t";
                            sb.Append("\r\n{");
                            DumpEntity(sb, entity, prefix);
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
    }
}
