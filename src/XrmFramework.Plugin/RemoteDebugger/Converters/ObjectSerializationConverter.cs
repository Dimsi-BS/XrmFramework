using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XrmFramework.RemoteDebugger.Converters
{
    /// <summary>
    ///     Carries a value over the wire together with the type needed to read it back.
    /// </summary>
    /// <remarks>
    ///     Both ends of the remote debugger compile this file, and since the runner can be a .NET one
    ///     while the plugin is always .NET Framework, the two ends do not agree on what a type is
    ///     called. <c>typeof(DateTime).AssemblyQualifiedName</c> names <c>System.Private.CoreLib</c> on
    ///     .NET and <c>mscorlib</c> on .NET Framework; .NET resolves either, .NET Framework resolves
    ///     only its own. The Dataverse SDK types are not affected - <c>Microsoft.Xrm.Sdk</c> has one
    ///     identity everywhere - so the problem is confined to the core library, and so is the fix.
    /// </remarks>
    public class ObjectSerializationConverter : JsonConverter<ObjectSerialization>
    {
        public override void WriteJson(JsonWriter writer, ObjectSerialization value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(ObjectSerialization.PortableTypeName(value.Value?.GetType()));
            writer.WritePropertyName("Value");
            serializer.Serialize(writer, value.Value);
            writer.WriteEndObject();
        }

        public override ObjectSerialization ReadJson(JsonReader reader, Type objectType, ObjectSerialization existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            Type type = null;

            reader.Read();
            var propertyName = (string)reader.Value;
            reader.Read();

            var typeName = (string)reader.Value;

            if (!string.IsNullOrEmpty(typeName))
            {
                type = ResolveType(typeName);
            }

            reader.Read();
            reader.Read();
            object value = default;

            if (type != null)
            {
                value = serializer.Deserialize(reader, type);
            }
            else
            {
                // The value token has to be consumed whatever happens. Skipping it - which is what
                // an unresolved type used to do - leaves the reader one token behind, and since
                // everything around here is read positionally, the rest of the message is then
                // parsed against the wrong tokens and fails far away with a NullReferenceException
                // that says nothing about a type name.
                JToken.Load(reader);
            }

            reader.Read();

            return new ObjectSerialization(value);
        }

        /// <summary>
        ///     Resolves a type name written by either end.
        /// </summary>
        /// <remarks>
        ///     The fallback is for names written before <see cref="ObjectSerialization.PortableTypeName" /> existed: a
        ///     plugin deployed from an older build still sends fully qualified core library types,
        ///     and a runner must keep reading them. Dropping the assembly and retrying costs nothing
        ///     and turns an unreadable message into a readable one.
        /// </remarks>
        private static Type ResolveType(string typeName)
        {
            var type = Type.GetType(typeName, throwOnError: false);
            if (type != null)
                return type;

            var comma = typeName.IndexOf(',');
            if (comma <= 0)
                return null;

            return Type.GetType(typeName.Substring(0, comma).Trim(), throwOnError: false);
        }
    }
}
