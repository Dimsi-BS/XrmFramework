using Plugins;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using XrmProject.Utils;

namespace GenerateMocks
{
    class Program
    {
        static void Main(string[] args)
        {
            var types = typeof(IService).Assembly.GetTypes().Where(t => typeof(IService).IsAssignableFrom(t) && t.IsInterface);
            
            var serviceUtilsProjFileName = "../../../../Utils/LoggedServices/LoggedServices.projitems";

            MockGenerator.GenerateMocks(serviceUtilsProjFileName, types, typeof(NullableAttribute));
        }
    }
}