﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmFramework.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Model
    {
        [JsonProperty("tName")]
        public string tableLogicalName;

        [JsonProperty("Name")]
        public string Name;

        public string ModelNamespace;

        //Contains the logical name of the columns we want to include in the binding model
        [JsonProperty("Cols")]
        public List<ModelProperty> Properties;




        public bool IsBindingModelBase;
    }
}
