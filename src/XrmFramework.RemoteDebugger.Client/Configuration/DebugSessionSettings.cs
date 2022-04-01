using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmFramework.RemoteDebugger.Client.Configuration
{
    public class DebugSessionSettings
    {
        public string userUniqueUri { get; private set; }

        public void ParseAndSetUri(string raw)
        {
            var columns = raw.Split(';');
            string uri = "", entityPath = "";
            foreach(var column in columns)
            {
                var key = column.Split('=')[0].Trim();
                if (string.IsNullOrEmpty(key)) continue;
                var value = column.Split('=')[1].Trim();

                if(key.Equals("Endpoint")) uri = value;
                else if(key.Equals("EntityPath")) entityPath = value;
            }
            userUniqueUri = $"{uri}{entityPath}";
        }

        public void SetUri(string parsed) => userUniqueUri = parsed;
    }
}