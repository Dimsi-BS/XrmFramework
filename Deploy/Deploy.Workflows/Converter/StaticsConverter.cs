using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deploy
{
    public static class StaticsConverter
    {
        public static string Convert(global::Plugins.Messages message)
        {
            return message.ToString();
        }
        public static Modes Convert(global::Plugins.Modes mode)
        {
            return (Modes)Enum.ToObject(typeof(Modes), (int)mode);
        }
        public static Stages Convert(global::Plugins.Stages stage)
        {
            return (Stages)Enum.ToObject(typeof(Stages), (int)stage);
        }
        public static global::Plugins.Messages Convert(Messages message)
        {
            return (global::Plugins.Messages)Enum.Parse(typeof(global::Plugins.Messages), message.ToString());
        }
        public static global::Plugins.Modes Convert(Modes mode)
        {
            return (global::Plugins.Modes)Enum.ToObject(typeof(global::Plugins.Modes), (int)mode);
        }
        public static global::Plugins.Stages Convert(Stages stage)
        {
            return (global::Plugins.Stages)Enum.ToObject(typeof(global::Plugins.Stages), (int)stage);
        }
    }
}
