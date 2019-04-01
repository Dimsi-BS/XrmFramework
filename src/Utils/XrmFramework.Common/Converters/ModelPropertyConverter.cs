using System;
using System.ComponentModel;

namespace Model
{
    public abstract class ModelPropertyConverter
    {
        public virtual bool CanConvertFrom(Type sourceType)
        {
            return false;
        }

        public virtual bool CanConvertTo(Type destinationType)
        {
            return false;
        }

        public virtual object ConvertFrom(object value)
        {
            return null;
        }

        public virtual object ConvertTo(object initialValue, Type destinationType)
        {
            return null;
        }
    }

}
