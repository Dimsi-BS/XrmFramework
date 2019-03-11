using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Model;
using Model.Sdk;

namespace DefinitionManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());


            //var gen = new DefinitionGenerator();
            //File.WriteAllText("../../../Model/ModelTemp.cs", gen.TransformText());
        }
    }
}
