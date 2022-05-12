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
        private static readonly Dictionary<Type, CustomApiArgumentType> _mapper = new();

        static CustomApiArgumentTypeMapper()
        {
            _mapper.Add(typeof(bool), CustomApiArgumentType.Boolean);
            _mapper.Add(typeof(DateTime), CustomApiArgumentType.DateTime);
            _mapper.Add(typeof(decimal), CustomApiArgumentType.Decimal);
            _mapper.Add(typeof(Entity), CustomApiArgumentType.Entity);
            _mapper.Add(typeof(EntityCollection), CustomApiArgumentType.EntityCollection);
            _mapper.Add(typeof(EntityReference), CustomApiArgumentType.EntityReference);
            _mapper.Add(typeof(float), CustomApiArgumentType.Float);
            _mapper.Add(typeof(int), CustomApiArgumentType.Integer);
            _mapper.Add(typeof(Money), CustomApiArgumentType.Money);
            _mapper.Add(typeof(OptionSetValue), CustomApiArgumentType.Picklist);
            _mapper.Add(typeof(string), CustomApiArgumentType.String);
            _mapper.Add(typeof(string[]), CustomApiArgumentType.StringArray);
            _mapper.Add(typeof(Guid), CustomApiArgumentType.Guid);
        }
        /// <summary>
        /// Maps the given <see cref="Type"/> to the corresponding <see cref="CustomApiArgumentType"/>
        /// and puts it in <paramref name="argumentType"/>
        /// If not found, <paramref name="argumentType"/> is set to <see cref="CustomApiArgumentType.String"/> and returns false
        /// </summary>
        /// <param name="type"></param>
        /// <param name="argumentType"></param>
        /// <returns><c>True</c> if the given <paramref name="type"/> matches a <see cref="CustomApiArgumentType"/>, false otherwise</returns>
        public static bool TryMap(Type type, out CustomApiArgumentType argumentType)
        {
            if (type.IsEnum)
            {
                argumentType = CustomApiArgumentType.Picklist;
                return true;
            }
            if (_mapper.TryGetValue(type, out argumentType)) return true;

            argumentType = CustomApiArgumentType.String;
            return false;
        }
    }
}
