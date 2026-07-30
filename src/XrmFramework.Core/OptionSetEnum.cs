using System.Collections.Generic;
using Newtonsoft.Json;

namespace XrmFramework.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    public class OptionSetEnum
    {
        [JsonProperty("LogName")]
        public string LogicalName { get; set; }

        public string Name { get; set; }

        public ICollection<OptionSetEnumValue> Values { get; } = new List<OptionSetEnumValue>();

        public bool IsGlobal { get; set; }

        public bool HasNullValue { get; set; }

        /// <summary>
        /// Marqueur local des .table internes au framework : l'option set est figé et ne doit pas
        /// être régénéré. Absent des métadonnées CRM, il n'est jamais produit par une commande de
        /// génération — il est uniquement relu et réécrit tel quel afin de ne pas être perdu.
        /// </summary>
        [JsonProperty("Locked")]
        public bool IsLocked { get; set; }
    }
}