using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace XrmFramework
{
    /// <summary>
    /// Maps a <see cref="Type"/> from a <c>PropertyType.GenericTypeArguments</c> to a corresponding <see cref="CustomApiArgumentType"/>
    /// </summary>
    public static class CustomApiArgumentTypeMapper
    {
        private static readonly Dictionary<Type, CustomApiArgumentType> Mapper = new();

        static CustomApiArgumentTypeMapper()
        {
            Mapper.Add(typeof(bool), CustomApiArgumentType.Boolean);
            Mapper.Add(typeof(DateTime), CustomApiArgumentType.DateTime);
            Mapper.Add(typeof(decimal), CustomApiArgumentType.Decimal);
            Mapper.Add(typeof(Entity), CustomApiArgumentType.Entity);
            Mapper.Add(typeof(EntityCollection), CustomApiArgumentType.EntityCollection);
            Mapper.Add(typeof(EntityReference), CustomApiArgumentType.EntityReference);
            Mapper.Add(typeof(float), CustomApiArgumentType.Float);
            Mapper.Add(typeof(int), CustomApiArgumentType.Integer);
            Mapper.Add(typeof(Money), CustomApiArgumentType.Money);
            Mapper.Add(typeof(OptionSetValue), CustomApiArgumentType.Picklist);
            Mapper.Add(typeof(string), CustomApiArgumentType.String);
            Mapper.Add(typeof(string[]), CustomApiArgumentType.StringArray);
            Mapper.Add(typeof(Guid), CustomApiArgumentType.Guid);
        }

        /// <summary>
        /// Maps the given <see cref="Type"/> to the corresponding <see cref="CustomApiArgumentType"/>
        /// and puts it in <paramref name="argumentType"/>
        /// If not found, <paramref name="argumentType"/> is set to <see cref="CustomApiArgumentType.String"/> and returns false
        /// </summary>
        /// <param name="type"></param>
        /// <param name="argumentType"></param>
        /// <returns><c>True</c> if the given <paramref name="type"/> matches a <see cref="CustomApiArgumentType"/>, <c>False</c> otherwise</returns>
        public static bool TryMap(Type type, out CustomApiArgumentType argumentType)
        {
            if (type.IsEnum)
            {
                argumentType = CustomApiArgumentType.Picklist;
                return true;
            }
            if (Mapper.TryGetValue(type, out argumentType)) return true;

            argumentType = CustomApiArgumentType.String;
            return false;
        }
    }
}
