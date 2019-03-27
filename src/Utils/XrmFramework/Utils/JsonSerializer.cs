// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class JsonSerializer
    {
        //"yyyy-MM-ddTHH:mm:ss"
        public static M Deserialize<M>(string serialized, string dateTimeFormat = null, bool useSimpleDictionaryFormat = false) where M : new()
        {
            if (string.IsNullOrEmpty(serialized))
            {
                return default(M);
            }

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(serialized)))
            {
                DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings
                {
                    UseSimpleDictionaryFormat = useSimpleDictionaryFormat
                };

                if (!string.IsNullOrEmpty(dateTimeFormat))
                {
                    settings.DateTimeFormat = new System.Runtime.Serialization.DateTimeFormat(dateTimeFormat);
                }

                var ser = new DataContractJsonSerializer(typeof(M), settings);
                return (M) ser.ReadObject(ms);
            }
        }

        /// <summary>
        /// Serialize object M to string
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="deserialized"></param>
        /// <returns></returns>
        public static string Serialize<M>(M deserialized, string dateTimeFormat = null)
        {
            if (deserialized == null)
            {
                return null;
            }

            using (var stream1 = new MemoryStream())
            {
                DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();

                if (!string.IsNullOrEmpty(dateTimeFormat))
                {
                    settings.DateTimeFormat = new System.Runtime.Serialization.DateTimeFormat(dateTimeFormat);
                }

                var ser2 = new DataContractJsonSerializer(typeof(M), settings);

                ser2.WriteObject(stream1, deserialized);

                return Encoding.UTF8.GetString(stream1.ToArray());
            }
        }
    }
}
