
using System;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LoggerClassAttribute : Attribute
    {
        public Type LoggerClassType { get; }

        public LoggerClassAttribute(Type loggerClassType)
        {
            LoggerClassType = loggerClassType;
        }
    }
}
