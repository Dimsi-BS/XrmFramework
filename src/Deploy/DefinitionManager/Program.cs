// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xrm.Sdk.Query;
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
            using (var orgService = ConnectionHelper.GetCrmServiceClient("Xrm"))
            {
                var query = new QueryExpression(SystemUserDefinition.EntityName);
                query.ColumnSet.AllColumns = true;

                var users = orgService.RetrieveAll(query).ToDictionary(e => e.GetAttributeValue<string>(SystemUserDefinition.Columns.FullName));
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());


            //var gen = new DefinitionGenerator();
            //File.WriteAllText("../../../Model/ModelTemp.cs", gen.TransformText());
        }
    }
}
