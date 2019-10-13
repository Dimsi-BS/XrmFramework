// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
#if INTERNAL_NEWTONSOFT
using Newtonsoft.Json.Xrm;
#else 
using Newtonsoft.Json;
#endif

namespace Model.Utils
{
    public static class JsonSerializer
    {
        public static bool TryDeserialize<M>(string serialized, out M result, out string errorMessage, string dateTimeFormat = null, bool useSimpleDictionaryFormat = false) where M : new()
        {
            errorMessage = null;
            result = default;

            try
            {
                result = Deserialize<M>(serialized, dateTimeFormat, useSimpleDictionaryFormat);
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        public static M Deserialize<M>(string serialized, string dateTimeFormat = null, bool useSimpleDictionaryFormat = false) where M : new()
        {
            if (string.IsNullOrEmpty(serialized))
            {
                return default(M);
            }


#if DO_NOT_HAVE_NEWTONSOFT

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
#else
            var setting = new JsonSerializerSettings
            {
                DateFormatString = dateTimeFormat
            };

            try
            {
                return JsonConvert.DeserializeObject<M>(serialized, setting);
            }
            catch (JsonSerializationException e)
            {
                throw new InvalidPluginExecutionException($"Erreur de désérialisation : {serialized}\r\n{e.Message}");
            }
#endif

        }

        public static bool TrySerialize<M>(M deserialized, out string serialized, out string errorMessage, string dateTimeFormat = null) where M : new()
        {
            errorMessage = null;
            serialized = default;

            try
            {
                serialized = Serialize<M>(deserialized, dateTimeFormat);
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
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



#if DO_NOT_HAVE_NEWTONSOFT
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
#else
            var setting = new JsonSerializerSettings
            {
                DateFormatString = dateTimeFormat
            };

            try
            {
                return JsonConvert.SerializeObject(deserialized, setting);
            }
            catch (JsonSerializationException e)
            {
                throw new InvalidPluginExecutionException($"Erreur de sérialisation : {deserialized}\r\n{e.Message}");
            }
#endif
        }
    }
}
