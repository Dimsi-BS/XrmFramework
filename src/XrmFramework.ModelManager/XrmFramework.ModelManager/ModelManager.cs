using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XrmFramework.Core;

namespace XrmFramework.ModelManager
{
    public static class ModelManager
    {

        public static void TestModelCreation(string CoreProjectName)
        {

            TableCollection tables = new TableCollection();
            List<OptionSetEnum> globalEnums = new List<OptionSetEnum>();
            List<string> tableFileNames = new List<string>();
            FileInfo fileInfo;
            String text;
            Table currentTable;
            string userInput;
            
            GetFilesFromFolder($"../../../../../../{CoreProjectName}", "table", tableFileNames);
            foreach (string fileName in tableFileNames)
            {
                if (!fileName.Contains("OptionSet.table"))
                {
                    fileInfo = new FileInfo(fileName);
                    text = File.ReadAllText(fileInfo.FullName);
                    JObject jTable = JObject.Parse(text);


                    currentTable = jTable.ToObject<Table>();


                    if (jTable["Cols"].Any())
                    {
                        Column currentColumn;
                        foreach (var jColumn in jTable["Cols"])
                        {
                            currentColumn = jColumn.ToObject<Column>();
                            currentTable.Columns.Add(currentColumn);
                        }
                    }
                    tables.Add(currentTable);
                }
                else
                {
                    fileInfo = new FileInfo(fileName);
                    text = File.ReadAllText(fileInfo.FullName);
                    globalEnums = JsonConvert.DeserializeObject<List<OptionSetEnum>>(text, new JsonSerializerSettings
                    {
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });
                }

            }

            Console.WriteLine($"There are {tables.Count} tables in this project :");
            for(int i = 0;i<tables.Count;i++)
            {
                Console.WriteLine($"{i} : {tables.ElementAt(i).Name}");
            }

            Console.WriteLine("Do you want to create a Model for one of these tables ? (y/n)");
            userInput = Console.ReadLine();
            while(userInput != "y" && userInput != "n")
            {
                userInput = Console.ReadLine();

            }

            if(userInput == "n")
            {
                return;
            }


            Console.WriteLine("Which table do you want to create a model for ? (Enter the corresponding number)");
            userInput = Console.ReadLine();
            
            while(userInput == null || ((int.Parse(userInput) < 0)  || int.Parse(userInput) >= tables.Count))
            {
                Console.WriteLine("Which table do you want to create a model for ? (Enter the corresponding number)");

                userInput = Console.ReadLine();
            }

            Console.WriteLine($"You chose table {int.Parse(userInput)} : {tables.ElementAt(int.Parse(userInput)).Name}");

            var model = CreateModelFromTable(tables.ElementAt(int.Parse(userInput)));

            var serializedModel = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });


            Console.WriteLine("Enter v to view the serialized model, s to save it, or x to quit.");
            userInput = Console.ReadLine();
            while(userInput != "s" && userInput != "x")
            {
                if(userInput == "v")
                {
                    Console.Write(serializedModel);
                    Console.WriteLine();
                }
                Console.WriteLine("Enter v to view the serialized model, s to save it, or x to quit.");
                userInput = Console.ReadLine();
            }

            if(userInput == "x")
            {
                return;
            }

            var modelFileInfo = new FileInfo($"../../../../../../{CoreProjectName}/Models/{model.Name}.model");

            var definitionFolder = new DirectoryInfo($"../../../../../../{CoreProjectName}/Models");
            if (definitionFolder.Exists == false)
            {
                definitionFolder.Create();
            }

            File.WriteAllText(modelFileInfo.FullName, serializedModel);

            Console.WriteLine($"Model was registered at {modelFileInfo.FullName}");

        }

        public static void TestBindingModelGeneration(string CoreProjectName)
        {
            TableCollection tables = new TableCollection();
            List<OptionSetEnum> globalEnums = new List<OptionSetEnum>();
            List<Model> models = new List<Model>();
            List<string> tableFileNames = new List<string>();
            List<string> modelFileNames = new List<string>();
            FileInfo fileInfo;
            String text;
            Table currentTable;
            string userInput;


            // Get models
            GetFilesFromFolder($"../../../../../../{CoreProjectName}", "model", modelFileNames);
            foreach (string fileName in modelFileNames)
            {
                if (!fileName.Contains("OptionSet.table"))
                {
                    fileInfo = new FileInfo(fileName);
                    text = File.ReadAllText(fileInfo.FullName);
                    
                    models.Add(JsonConvert.DeserializeObject<Model>(text));
                }
               

            }
            // If no models are found for the project, exit the function
            if(models.Count == 0)
            {
                Console.WriteLine("No .model file were found for this project, press any key to leave");
                Console.ReadKey();
                return;
            }
            else
            {
                foreach (Model model in models)
                {
                    Console.WriteLine(model.Name);
                }
            }
            // Get tables
            GetFilesFromFolder($"../../../../../../{CoreProjectName}", "table", tableFileNames);
            foreach (string fileName in tableFileNames)
            {
                if (!fileName.Contains("OptionSet.table"))
                {
                    fileInfo = new FileInfo(fileName);
                    text = File.ReadAllText(fileInfo.FullName);
                    JObject jTable = JObject.Parse(text);


                    currentTable = jTable.ToObject<Table>();


                    if (jTable["Cols"].Any())
                    {
                        Column currentColumn;
                        foreach (var jColumn in jTable["Cols"])
                        {
                            currentColumn = jColumn.ToObject<Column>();
                            currentTable.Columns.Add(currentColumn);
                        }
                    }
                    tables.Add(currentTable);
                }
                else
                {
                    fileInfo = new FileInfo(fileName);
                    text = File.ReadAllText(fileInfo.FullName);
                    globalEnums = JsonConvert.DeserializeObject<List<OptionSetEnum>>(text, new JsonSerializerSettings
                    {
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });
                }

            }
            // If a model does not have a corresponding class exit
            foreach(var model in models)
            {
                var hasCorrespondingTable = false;
                foreach (var table in tables)
                {
                    if (table.LogicalName == model.tableName) ;
                    {
                        hasCorrespondingTable |= true;
                    }
                }
                if(!hasCorrespondingTable)
                {
                    Console.WriteLine($"No corresponding table was found for {model.Name}, press any key to exit.");
                    Console.ReadKey();
                    return;
                }
            }

            foreach(var model in models)
            {
                // Create start of class
                var sb = new IndentedStringBuilder();
                var correspondingTable = tables.FirstOrDefault(t => t.Name == model.tableName);
                sb.AppendLine("");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.CodeDom.Compiler;");
                sb.AppendLine("using System.ComponentModel.DataAnnotations;");
                sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
                sb.AppendLine("using XrmFramework;");
                sb.AppendLine("using Newtonsoft.Json;");
                sb.AppendLine($"using {CoreProjectName};");
                sb.AppendLine();
                sb.AppendLine($"namespace {CoreProjectName}.Model");
                sb.AppendLine("{");

                using (sb.Indent())
                {
                    // Class declaration
                    sb.AppendLine("[GeneratedCode(\"XrmFramework\", \"2.0\")]");
                    sb.AppendLine("[ExcludeFromCodeCoverage]");
                    sb.AppendLine($"[CrmEntity({correspondingTable.Name}Definition.EntityName)]");


                    if (model.IsBindingModelBase)
                    {
                        sb.AppendLine($"public partial class {model.Name}Model : BindingModelBase");

                    }
                    else
                    {
                        sb.AppendLine($"public partial class {model.Name}Model : IBindingModel");
                    }


                    sb.AppendLine("{");
                    // Properties
                    using (sb.Indent())
                    {
                        sb.AppendLine("[CrmMapping({correspondingTable.Name}Definition.Columns.Id)");
                        sb.AppendLine("public Guid Id { get; set; }");
                    }
                    sb.AppendLine("}");

                }

                sb.AppendLine("}");
            }
            
        }
        private static void GetFilesFromFolder(string folderPath, string extension, List<string> fileNames)
        {
            foreach (string fileName in Directory.GetFiles(folderPath, $"*.{extension}"))
            {
                fileNames.Add(fileName);
            }

            foreach (string folderName in Directory.GetDirectories(folderPath))
            {
                GetFilesFromFolder(folderName, extension, fileNames);
            }


            return;
        }

        private static Model CreateModelFromTable(Table table)
        {
            Model model = new Model();
            model.tableName = table.Name;
            // Temporaire
            model.Name = table.Name;
            model.Properties = new List<ModelProperty>();
            foreach (var col in table.Columns)
            {
                var prop = new ModelProperty();
                prop.LogicalName = col.LogicalName;
                prop.Name = col.Name;
                prop.useOnPropertyChanged = false;
                model.Properties.Add(prop);
            }
            foreach(var relation in table.OneToManyRelationships)
            {
                var prop = new ModelProperty();
                prop.LogicalName = relation.Name;
                prop.Name = relation.Name;
                prop.useOnPropertyChanged = true;

                model.Properties.Add(prop);
            }

            return model;
        }

        public static void GenerateCodeFromModel(Model model)
        {

        }
    }
}
