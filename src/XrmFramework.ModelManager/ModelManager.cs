using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Xrm.Sdk.Metadata;
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
        public static TableCollection Tables = new TableCollection();
        public static List<OptionSetEnum> GlobalEnums = new List<OptionSetEnum>();
        public static List<Model> Models = new List<Model>();
        public static string CoreProjectName;

        public static void TestModelCreation(string coreProjectName)
        {
            CoreProjectName = coreProjectName;
            LoadTablesAndEnums();
            LoadModels();
            Console.WriteLine("This is the CLI version of the model manager.");

            CLI_EntryPoint();
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
            if (models.Count == 0)
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
            foreach (var model in models)
            {
                var hasCorrespondingTable = false;
                foreach (var table in tables)
                {
                    if (table.LogicalName == model.TableLogicalName) ;
                    {
                        hasCorrespondingTable |= true;
                    }
                }
                if (!hasCorrespondingTable)
                {
                    Console.WriteLine($"No corresponding table was found for {model.Name}, press any key to exit.");
                    Console.ReadKey();
                    return;
                }
            }

            foreach (var model in models)
            {
                // Create start of class
                var sb = new IndentedStringBuilder();
                var correspondingTable = tables.FirstOrDefault(t => t.LogicalName == model.TableLogicalName);
                if(correspondingTable == null)
                {
                    throw new Exception("The table corresponding to this model was not found, its logical name is : " + model.TableLogicalName);
                }
                sb.AppendLine("");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.CodeDom.Compiler;");
                sb.AppendLine("using System.ComponentModel.DataAnnotations;");
                sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using XrmFramework;");
                sb.AppendLine("using Newtonsoft.Json;");
                sb.AppendLine("using XrmFramework.BindingModel;");
                sb.AppendLine($"using {CoreProjectName};");
                sb.AppendLine();
                if(model.ModelNamespace != null && model.ModelNamespace != "")
                {
                    sb.AppendLine($"namespace {CoreProjectName}.{model.ModelNamespace}");

                }
                else
                {
                    sb.AppendLine($"namespace {CoreProjectName}");

                }
                sb.AppendLine("{");

                using (sb.Indent())
                {
                    // Class declaration
                    sb.AppendLine("[GeneratedCode(\"XrmFramework\", \"2.0\")]");
                    sb.AppendLine("[ExcludeFromCodeCoverage]");
                    sb.AppendLine($"[CrmEntity({correspondingTable.Name}Definition.EntityName)]");
                    sb.AppendLine("[JsonObject(MemberSerialization.OptIn)]");



                    
                    sb.AppendLine($"public partial class {model.Name}Model : BindingModelBase");


                    sb.AppendLine();
                    sb.AppendLine("{");
                    // Properties
                    using (sb.Indent())
                    {
                        sb.AppendLine();
                        sb.AppendLine($"[CrmMapping({correspondingTable.Name}Definition.Columns.Id)]");
                        sb.AppendLine("public Guid Id { get; set; }");
                        sb.AppendLine();
                        foreach (var prop in model.Properties)
                        {
                            var correspondingColumn = correspondingTable.Columns.FirstOrDefault(c => c.LogicalName == prop.LogicalName);
                            {
                                if (correspondingColumn != null)
                                {   //This property is a column
                                    sb.Append($"[CrmMapping({correspondingTable.Name}Definition.Columns.{correspondingColumn.Name}");//)]");
                                    if(prop.IsValidForUpdate)
                                    {
                                        sb.Append(")]");

                                    }
                                    else
                                    {
                                        sb.Append(",IsValidForUpdate = false)]");
                                    }
                                    if (correspondingColumn.Type == AttributeTypeCode.Lookup)
                                    {
                                        //Get the corresponding relationship info in the table
                                        var correspondingRelation = correspondingTable.ManyToOneRelationships.FirstOrDefault(r => r.LookupFieldName == prop.LogicalName);
                                        if(correspondingRelation == null)
                                        {
                                            throw new Exception("No corresponding relationship found in table for " + prop.Name);
                                        }
                                        else
                                        {
                                            sb.Append($"[CrmLookup(");
                                            var referencedTable = tables.FirstOrDefault(t => t.LogicalName == correspondingRelation.EntityName);
                                            if (referencedTable != null)
                                            {
                                                sb.Append($"{referencedTable.Name}Definition.EntityName,");
                                                var referencedColumn = referencedTable.Columns.FirstOrDefault(c => c.LogicalName == correspondingRelation.LookupFieldName);
                                                if (referencedColumn != null)
                                                {
                                                    sb.Append($"{referencedTable.Name}Definition.Columns.{referencedColumn.Name})]");
                                                }
                                                else
                                                {
                                                    sb.Append($"\"{correspondingRelation.LookupFieldName}\")]");

                                                }
                                            }
                                            else
                                            {
                                                sb.AppendLine($"\"{correspondingRelation.EntityName}\",\"{correspondingRelation.LookupFieldName}\")]");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //This property is a OneToMany relation
                                    var correspondingRelation = correspondingTable.OneToManyRelationships.FirstOrDefault(r => r.Name == prop.LogicalName);
                                    if (correspondingRelation == null)
                                    {
                                        throw new Exception("Error, no corresponding OneToMany relation found for this property : " + prop.Name);
                                    }
                                    sb.AppendLine($"[ChildRelationship({correspondingTable.Name}Definition.OneToManyRelationships.{correspondingRelation.NavigationPropertyName})]");
                                }

                                if (prop.JsonPropertyName != null)
                                {
                                    sb.AppendLine($"[JsonProperty(\"{prop.JsonPropertyName}\")]");
                                }

                                // Add other possible attributes


                                if (!prop.IsValidForUpdate)
                                {
                                    // Write regular declaration
                                    if (correspondingColumn != null)
                                    {
                                        sb.AppendLine(String.Format("public {0} {1} {{get; set;}}", prop.TypeFullName, prop.Name));
                                    }
                                    else
                                    {
                                        sb.AppendLine($"public List<{prop.TypeFullName}> {prop.Name} {{get;set;}} = new List<{prop.TypeFullName}>();");
                                    }
                                }
                                else
                                {

                                    // Write property declaration with call to OnPropertyChanged()
                                    if (correspondingColumn != null)
                                    {
                                        string tmp = @$"
        public {prop.TypeFullName} {prop.Name}
        {{
            get {{return _{prop.Name};}}
            set 
            {{
                if(value == _{prop.Name})
                    return;
                _{prop.Name} = value;
                OnPropertyChanged();
            }}
        }}
                                                      ";
                                        //Console.WriteLine(tmp);
                                        sb.AppendLine(tmp);
                                    }
                                    else
                                    {
                                        string tmp2 = @$"
        public List<{prop.TypeFullName}> {prop.Name}
        {{
            get {{return _{prop.Name};}}
            set 
            {{
                if(value == _{prop.Name})
                    return;
                _{prop.Name} = value;
                OnPropertyChanged();
            }}
        }}= new List<{prop.TypeFullName}>();
                                                      ";
                                        sb.AppendLine(tmp2);
                                       // "{" +
                                       // "   get { return _{1};}\n" +
                                       // "   set\n" +
                                       // "       {\n" +
                                       // "           if(value == _{1})\n" +
                                       // "           {\n" +
                                       // "               return;\n" +
                                       // "           }\n" +
                                       // "           _{1} = value;\n" +
                                       // "           OnPropertyChanged();\n" +
                                       // "       }\n" +
                                       // "} = new List<{0}>();\n", prop.TypeFullName, prop.Name));
                                    }

                                }

                                sb.AppendLine();
                            }
                        }



                        
                        sb.AppendLine("#region Fields");

                        foreach (var prop in model.Properties.Where(p => p.IsValidForUpdate))
                        {
                            //Add the corresponding field
                            var correspondingColumn = correspondingTable.Columns.FirstOrDefault(c => c.LogicalName == prop.LogicalName);
                            if (correspondingColumn != null)
                            {
                                sb.AppendLine(String.Format("private {0} _{1};", prop.TypeFullName, prop.Name));

                            }
                            else
                            {
                                sb.AppendLine(String.Format("private List<{0}> _{1};", prop.TypeFullName, prop.Name));
                            }
                        }
                        sb.AppendLine("#endregion");
                        
                       




                    }
                    sb.AppendLine("}");

                }

                sb.AppendLine("}");

                var classFileInfo = new FileInfo($"../../../../../../{CoreProjectName}/Models/{model.Name}.cs");



                File.WriteAllText(classFileInfo.FullName, sb.ToString());

            }

        }
        private static void GetFilesFromFolder(String folderPath, string extension, List<string> fileNames)
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
            model.TableLogicalName = table.LogicalName;
            // Temporaire
            model.Name = table.Name; // A modifier
            //model.Properties = new List<ModelProperty>();
            //foreach (var col in table.Columns)
            //{
            //    var prop = new ModelProperty();
            //    prop.LogicalName = col.LogicalName;
            //    prop.Name = col.Name;
            //    prop.useOnPropertyChanged = false;
            //    model.Properties.Add(prop);
            //}
            //foreach(var relation in table.OneToManyRelationships)
            //{
            //    var prop = new ModelProperty();
            //    prop.LogicalName = relation.Name;
            //    prop.Name = relation.Name;
            //    prop.useOnPropertyChanged = true;
            //
            //    model.Properties.Add(prop);
            //}

            return model;
        }


        public static void SetModelName(Model model, string name)
        {
            model.Name = name;
        }

        public static void AddModelProperty(Model model, Table table, string propertyLogicalName)
        {
            var col = table.Columns.FirstOrDefault(c => c.LogicalName == propertyLogicalName);
            if (col != null)
            {
                var prop = new ModelProperty();

                prop.LogicalName = col.Name;
                prop.Name = col.Name;
                var sameMapping = model.Properties.FirstOrDefault(p => p.LogicalName == prop.LogicalName);
                foreach(var modelProp in model.Properties)
                {
                    if(modelProp.LogicalName == prop.LogicalName)
                    {

                    }
                }
                prop.IsValidForUpdate = true;

                model.Properties.Add(prop);
            }
            else
            {
                var rel = table.OneToManyRelationships.FirstOrDefault(r => r.LookupFieldName == propertyLogicalName);

                var prop = new ModelProperty();

                prop.LogicalName = rel.Name;
                prop.Name = rel.Name;
                prop.IsValidForUpdate = true;

                model.Properties.Add(prop);
            }
        }

        public static void SetPropertyJsonPropertyName(Model model, string propertyLogicalName, string JsonPropertyName)
        {
            var prop = model.Properties.FirstOrDefault(p => p.LogicalName == propertyLogicalName);
            prop.Name = JsonPropertyName;
        }

        public static void ToggleOnPropertyChanged(Model model, string propertyLogicalName)
        {
            var prop = model.Properties.FirstOrDefault(p => p.LogicalName == propertyLogicalName);
            prop.IsValidForUpdate = !prop.IsValidForUpdate;
        }

        public static void SelectPropertyType(Model model, ModelProperty property, AttributeTypeCode type)
        {


        }

        public static List<String> GetPossiblePropertyTypes(Model model, ModelProperty property)
        {
            OptionSetEnum FirstEnum;
            OptionSetEnum SecondEnum;
            var possibleTypes = new List<String>();


            var table = Tables.FirstOrDefault(t => t.LogicalName == model.TableLogicalName);
            if (table == null)
            {
                return possibleTypes;
            }
            var column = table.Columns.FirstOrDefault(c => c.LogicalName == property.LogicalName);
            Relation relation;
            if (column == null)
            {
                relation = table.OneToManyRelationships.FirstOrDefault(r => r.Name == property.LogicalName);
                if (relation == null)
                {
                    return possibleTypes;
                }
                else
                {
                    // Find a corresponding model and return

                    foreach (var possibleModel in Models)
                    {
                        if (possibleModel.TableLogicalName == relation.EntityName)
                        {
                            possibleTypes.Add($"{CoreProjectName}.{possibleModel.ModelNamespace}.{possibleModel.Name}");

                        }
                    }
                    
                    possibleTypes.Add("System.Guid");
                    possibleTypes.Add("Microsoft.Xrm.Sdk.EntityReference");

                    return possibleTypes;

                }
            }

            switch (column.Type)
            {

                case AttributeTypeCode.Boolean:
                    possibleTypes.Add("System.Boolean");
                    possibleTypes.Add("System.Int32");
                    possibleTypes.Add("System.String");

                    break;
                case AttributeTypeCode.Customer:
                    throw new Exception("The type Customer is not currently handled by the model manager");
                    //possibleTypes.Add($"{CoreProjectName}.{model.}.{}");
                    possibleTypes.Add("System.Guid");
                    possibleTypes.Add("Microsoft.Xrm.Sdk.EntityReference");

                    break;
                case AttributeTypeCode.Integer:
                    possibleTypes.Add("System.Int32");
                    break;
                case AttributeTypeCode.DateTime:
                    possibleTypes.Add("System.DateTime");
                    possibleTypes.Add("System.String");
                    break;
                case AttributeTypeCode.Decimal:
                    possibleTypes.Add("System.Decimal");
                    break;
                case AttributeTypeCode.Double:
                    possibleTypes.Add("System.Double");
                    break;
                case AttributeTypeCode.Lookup:
                    //Get the corresponding relation, find the corresponding model if it exists
                    
                    
                    var re = table.ManyToOneRelationships.FirstOrDefault(r => r.LookupFieldName == column.LogicalName);
                    if (re == null)
                    {
                        throw new Exception();
                    }
                    foreach(var possibleModel in Models)
                    {
                        if(possibleModel.TableLogicalName == re.EntityName)
                        {
                            possibleTypes.Add($"{CoreProjectName}.{possibleModel.ModelNamespace}.{possibleModel.Name}");

                        }
                    }
                    
                    possibleTypes.Add("System.Guid");
                    possibleTypes.Add("Microsoft.Xrm.Sdk.EntityReference");
                    break;
                case AttributeTypeCode.Memo:
                    possibleTypes.Add("System.String");
                    break;
                case AttributeTypeCode.Money:
                    possibleTypes.Add("System.Decimal");
                    break;
                case AttributeTypeCode.Owner:
                    possibleTypes.Add("System.Guid");
                    possibleTypes.Add("Microsoft.Xrm.Sdk.EntityReference");
                    //possibleTypes.Add("ModelType à déterminer");
                    break;
                case AttributeTypeCode.PartyList: // Est ce que c'est les one to many relationships ? parce que si oui ça pose problème avec la manière dont j'ai écrit mon code de génération
                    //possibleTypes.Add("System.Collections.Generic.List<Model à déterminer>");
                    //possibleTypes.Add("System.Collections.Generic.List<System.Guid>");
                    //possibleTypes.Add("System.Collections.Generic.List<Microsoft.Xrm.Sdk.EntityReference>");
                    throw new Exception("The type PartyList is not currently handled in the Model Manager");

                    break;
                case AttributeTypeCode.Picklist:
                    //possibleTypes.Add("Truc.Enum à déterminer");
                    // Get the corresponding enum
                    FirstEnum = table.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                    if (FirstEnum == null)
                    {
                        SecondEnum = GlobalEnums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                        if (SecondEnum != null)
                        {
                            possibleTypes.Add($"{CoreProjectName}.{SecondEnum.Name}");
                        }
                        else
                        {
                            throw new Exception("No corresponding enumeration found for property : " + column.Name);
                        }
                    }
                    else
                    {
                        possibleTypes.Add($"{CoreProjectName}.{FirstEnum.Name}");
                    }

                    possibleTypes.Add("System.Int32");
                    break;
                case AttributeTypeCode.State:

                    FirstEnum = table.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                    if (FirstEnum == null)
                    {
                        SecondEnum = GlobalEnums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                        if (SecondEnum != null)
                        {
                            possibleTypes.Add($"{CoreProjectName}.{SecondEnum.Name}");
                        }
                        else
                        {
                            throw new Exception("No corresponding enumeration found for property : " + column.Name);
                        }
                    }
                    else
                    {
                        possibleTypes.Add($"{CoreProjectName}.{FirstEnum.Name}");
                    }

                    possibleTypes.Add("System.Int32");
                    break;
                case AttributeTypeCode.Status:
                    FirstEnum = table.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                    if (FirstEnum == null)
                    {
                        SecondEnum = GlobalEnums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                        if (SecondEnum != null)
                        {
                            possibleTypes.Add($"{CoreProjectName}.{SecondEnum.Name}");
                        }
                        else
                        {
                            throw new Exception("No corresponding enumeration found for property : " + column.Name);
                        }
                    }
                    else
                    {
                        possibleTypes.Add($"{CoreProjectName}.{FirstEnum.Name}");
                    }
                    possibleTypes.Add("System.Int32");
                    break;
                case AttributeTypeCode.String:
                    possibleTypes.Add("System.String");
                    break;
                case AttributeTypeCode.Uniqueidentifier:
                    possibleTypes.Add("System.Guid");
                    break;
                case AttributeTypeCode.CalendarRules:
                    throw new Exception("The type CalendarRules is not currently handled in the Model Manager");
                    break;
                case AttributeTypeCode.Virtual:
                    throw new Exception("The type Virtual is not currently handled in the Model Manager");
                    break;
                case AttributeTypeCode.BigInt:
                    possibleTypes.Add("System.Int64");
                    break;
                case AttributeTypeCode.ManagedProperty:
                    throw new Exception("The type ManagedProperty is not currently handled in the Model Manager");
                    break;
                case AttributeTypeCode.EntityName:
                    possibleTypes.Add("System.String");
                    break;
            }

            return possibleTypes;
        }

        public static void LoadTablesAndEnums()
        {
            List<string> tableFileNames = new List<string>();
            Table currentTable;
            FileInfo fileInfo;
            String text;

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
                    Tables.Add(currentTable);
                }
                else
                {
                    fileInfo = new FileInfo(fileName);
                    text = File.ReadAllText(fileInfo.FullName);
                    GlobalEnums = JsonConvert.DeserializeObject<List<OptionSetEnum>>(text, new JsonSerializerSettings
                    {
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });
                }

            }
        }

        public static void LoadModels()
        {
            List<string> modelFileNames = new List<string>();

            FileInfo fileInfo;
            String text;

            GetFilesFromFolder($"../../../../../../{CoreProjectName}", "model", modelFileNames);
            foreach (string fileName in modelFileNames)
            {
                if (!fileName.Contains("OptionSet.table"))
                {
                    fileInfo = new FileInfo(fileName);
                    text = File.ReadAllText(fileInfo.FullName);

                    Models.Add(JsonConvert.DeserializeObject<Model>(text));
                }


            }
        }
        public static void CLI_EntryPoint()
        {
            string userInput;
            
            do
            {
                Console.WriteLine("Here are the available actions");

                Console.WriteLine("a : Create a Model");
                Console.WriteLine("b : Edit a Model");
                Console.WriteLine("c : Generate bindingModel classes");
                Console.WriteLine("d : Exit the program");

                Console.WriteLine("Enter the letter corresponding to your action :");
                userInput = Console.ReadLine();

                if (userInput == "a")
                {
                    CLI_CreateModel();
                }
                else if (userInput == "b")
                {
                    CLI_EditModel();
                }
                else if(userInput == "c")
                {
                    CLI_GenerateClasses();
                }

            }
            while (userInput != "d");
            
            



        }

        public static void CLI_CreateModel()
        {
            string userInput;
            int intInput;
            if (Tables.Count == 0)
            {
                Console.WriteLine("You tried to create a model but there are no table currently available.");
                return;
            }

            for (int i = 0; i < Tables.Count; i++)
            {
                Console.WriteLine($"{i} : {Tables.ElementAt(i).Name}");
            }

            do
            {
                Console.WriteLine("Which table do you want to create a model for ? Enter the corresponding number.");

                userInput = Console.ReadLine();
                intInput = int.Parse(userInput);
            }
            while (userInput == null || ((intInput < 0) || intInput >= Tables.Count));

            Console.WriteLine($"You chose table {int.Parse(userInput)} : {Tables.ElementAt(int.Parse(userInput)).Name}");

            do
            {
                Console.WriteLine("Enter the name for you model (cannot be the same name as an existing model) ?");
                userInput = Console.ReadLine();
            } while (userInput == "" || Models.Any(m => m.Name == userInput));




            var correspondingTable = Tables.ElementAt(intInput);
            var model = CreateModelFromTable(correspondingTable);
            model.Name = userInput;

            //Console.WriteLine("Enter b to use BindingModelBase as the parent class, anything else to use IBindingModel.");
            //if (Console.ReadLine() == "b")
            //{
            //    model.IsBindingModelBase = true;
            //}
            //else
            //{
            //    model.IsBindingModelBase = false;
            //}

            Models.Add(model);

            Console.WriteLine("SavingModelToFile");

            var serializedModel = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });


            var modelFileInfo = new FileInfo($"../../../../../../{CoreProjectName}/Models/{model.Name}.model");

            var modelFolder = new DirectoryInfo($"../../../../../../{CoreProjectName}/Models");
            if (modelFolder.Exists == false)
            {
                modelFolder.Create();
            }

            File.WriteAllText(modelFileInfo.FullName, serializedModel);

            Console.WriteLine($"Model was saved at {modelFileInfo.FullName}");

            return;


        }

        public static void CLI_EditModel()
        {
            string userInput;
            int intInput;
            if(Models.Count == 0)
            {
                Console.WriteLine("Tried to edit a model but none were found for this project");
                return;
            }

            for(int i = 0; i < Models.Count; i++)
            {
                Console.WriteLine($"_{i}_ : {Models[i].Name}");
            }

            do
            {
                Console.WriteLine("Which model do you want to edit ? Enter the corresponding number.");

                userInput = Console.ReadLine();
                intInput = int.Parse(userInput);
            }
            while (userInput == null || ((intInput < 0) || intInput >= Models.Count));
            Model model = Models.ElementAt(intInput);
            do
            {
                Console.WriteLine("What do you want to do ?");
                Console.WriteLine("a : edit a property.");
                Console.WriteLine("b : add a property.");
                Console.WriteLine("c : modify bindingModel class namespace.");
                Console.WriteLine("d : Modify bindingModel class name.");
                Console.WriteLine("e : Toggle isBindingModelBase.");
                Console.WriteLine("f : to exit.");
                userInput= Console.ReadLine();

                if(userInput == "a")
                {
                    // Edit property
                    CLI_EditProperty(model);
                }
                else if(userInput == "b")
                {
                    // Add property
                    CLI_AddProperty(model);
                }
                else if(userInput=="c")
                {
                    do
                    {
                        Console.WriteLine("Enter the new name of the class namespace :");
                        userInput = Console.ReadLine();
                    }
                    while (userInput == "");

                    model.ModelNamespace = userInput;
                }
                else if(userInput=="d")
                {
                    
                    do
                    {
                        Console.WriteLine("Enter the new name for the model (cannot be the same name as another model):");
                        userInput = Console.ReadLine();
                    }
                    while (userInput == "" || Models.Any(m => m.Name == userInput));

                    model.Name = userInput;
                }
                else if(userInput == "e")
                {
                    //Console.WriteLine("Enter b to use BindingModelBase as a parent, anything else to not : ");
                    //if(Console.ReadLine() == "b")
                    //{
                    //    model.IsBindingModelBase = true;
                    //}
                    //else
                    //{
                    //    model.IsBindingModelBase= false;
                    //}
                }
               

            } while (userInput != "f");

            Console.WriteLine("SavingModelToFile");

            var serializedModel = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

            var modelFileInfo = new FileInfo($"../../../../../../{CoreProjectName}/Models/{model.Name}.model");

            var modelFolder = new DirectoryInfo($"../../../../../../{CoreProjectName}/Models");
            if (modelFolder.Exists == false)
            {
                modelFolder.Create();
            }

            File.WriteAllText(modelFileInfo.FullName, serializedModel);

            Console.WriteLine($"Model was saved at {modelFileInfo.FullName}");
        }

        public static void CLI_GenerateClasses()
        {
            string userInput;
            if(Models.Count == 0)
            {
                Console.WriteLine("Tried to generate BindingModel classes but no model was found.");
                return;
            }
            do
            {
                Console.WriteLine("Enter the letter corresponding to what you what you want to do :");
                Console.WriteLine("a : Generate all classes");
                Console.WriteLine("b : do nothing for now");
                userInput = Console.ReadLine();



            }
            while(userInput != "a" && userInput != "b");

            if(userInput == "a")
            {
                TestBindingModelGeneration(CoreProjectName);
            }
            else if(userInput!= "b")
            {

            }
        }

        public static void CLI_AddProperty(Model model)
        {
            // Choose relation or column property
            string userInput;
            int intInput;
            var table = Tables.FirstOrDefault(t => t.LogicalName == model.TableLogicalName);
            ModelProperty newProperty = new ModelProperty();
            if(table == null)
            {
                Console.WriteLine("Tried to retrieve the table corresponding to this model but it was not found.");
                    return;
            }
            Console.WriteLine("Do you want to create a property from :");
            Console.WriteLine("a : a column");
            Console.WriteLine("b : a OneToMany relation");
            userInput = Console.ReadLine();

            if(userInput == "a")
            {
                // Create from column
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Console.WriteLine($"_{i}_ : {table.Columns.ElementAt(i).Name}");
                }
                Console.WriteLine("Choose the column you want to add :");
                do
                {
                    Console.WriteLine("Enter the number corresponding to the column you want to add.");
                    userInput = Console.ReadLine();
                    intInput = int.Parse(userInput);
                }
                while (userInput == null || ((intInput < 0) || intInput >= table.Columns.Count));

               

                var correspondingColumn = table.Columns.ElementAt(intInput);
                // Create new property
                newProperty = new ModelProperty();
                newProperty.LogicalName = correspondingColumn.LogicalName;
                ModelProperty sameProperty;
                do
                {
                    Console.WriteLine("Enter the name for this property");
                    userInput = Console.ReadLine();
                    sameProperty = model.Properties.FirstOrDefault(p => p.Name == userInput);

                }
                while (userInput == "" || sameProperty != null);

                newProperty.Name = userInput;
                

                Console.WriteLine("Enter p to set IsValidForUpdate to true, enter anything else to not");
                if (Console.ReadLine() == "p")
                {
                    newProperty.IsValidForUpdate = true;
                    //Check if any other common crm mapping is true
                    var similarMapping = model.Properties.FirstOrDefault(p => p.LogicalName == newProperty.LogicalName && p.IsValidForUpdate);
                    if(similarMapping != null)
                    {
                        //Choose the right one
                        do
                        {
                            Console.WriteLine($"Another property named {similarMapping.Name} connected to the same crm property already has IsValidForUpdate set to true, do you want to switch it to false in order to set it to true for your property {newProperty.Name} ? (y/n)");
                            userInput = Console.ReadLine();
                        }
                        while(userInput != "y" && userInput != "n");
                        if(userInput == "y")
                        {
                            similarMapping.IsValidForUpdate = false;
                            newProperty.IsValidForUpdate = true;
                        }
                        else
                        {
                            newProperty.IsValidForUpdate = false;
                        }


                    }
                }
                else
                {
                    newProperty.IsValidForUpdate = false;
                }

               

                Console.WriteLine("If you want this property to be serialized, enter a name, otherwise press enter");
                userInput = Console.ReadLine();
                var samejsonProperty = model.Properties.FirstOrDefault(p=>p.JsonPropertyName == userInput);
                if (userInput != "" && samejsonProperty != null)
                {
                    newProperty.JsonPropertyName = userInput;
                }
                var possibleTypes = GetPossiblePropertyTypes(model, newProperty);

                Console.WriteLine("Here are the possible types for this property :");
                for (int i = 0; i < possibleTypes.Count; i++)
                {
                    Console.WriteLine($"_{i}_ : {possibleTypes.ElementAt(i)}");
                }
                do
                {
                    Console.WriteLine("Type the number corresponding to the type you want to choose :");
                    userInput = Console.ReadLine();
                    if(userInput != "")
                    {
                        intInput = int.Parse(userInput);

                    }
                    else
                    {
                        intInput = -1;
                    }
                }
                while (userInput == "" || ((intInput < 0) || intInput >= possibleTypes.Count));
                
                newProperty.TypeFullName = possibleTypes.ElementAt(intInput);
            }
            else if(userInput == "b")
            {
                // Create from relation

                // Show all available columns for the corresponding table
                for (int i = 0; i < table.OneToManyRelationships.Count; i++)
                {
                    Console.WriteLine($"_{i}_ : {table.OneToManyRelationships.ElementAt(i).Name}");
                }
                Console.WriteLine("Choose the relation you want to add :");
                do
                {
                    Console.WriteLine("Enter the number corresponding to the relation you want to add.");
                    userInput = Console.ReadLine();
                    if(userInput != null)
                    {
                        intInput = int.Parse(userInput);
                    }
                    else
                    {
                        intInput= -1;
                    }
                    
                }
                while (userInput == null || ((intInput < 0) || intInput >= table.OneToManyRelationships.Count));

                var correspondingRelation = table.OneToManyRelationships.ElementAt(intInput);
                // Create new property
                newProperty = new ModelProperty();
                newProperty.LogicalName = correspondingRelation.NavigationPropertyName;

                Console.WriteLine($" Lookup field name is : {correspondingRelation.LookupFieldName}");
                Console.WriteLine($" Navigation property name is : {correspondingRelation.NavigationPropertyName}");

                ModelProperty sameProperty;
                do
                {
                    Console.WriteLine("Enter the name for this property");
                    userInput = Console.ReadLine();
                    sameProperty = model.Properties.FirstOrDefault(p => p.Name == userInput);
                }
                while (userInput == "" || sameProperty != null);
                newProperty.Name = userInput;
                Console.WriteLine("Enter p to use onPropertyChanged (only works if model.isBindingModelBase is true), enter anything to not");
                if (Console.ReadLine() == "p")
                {
                    newProperty.IsValidForUpdate = true;
                }
                else
                {
                    newProperty.IsValidForUpdate = false;
                }

                Console.WriteLine("If you want this property to be serialized, enter a name, otherwise press enter");
                userInput = Console.ReadLine();
                if (userInput != "")
                {
                    newProperty.JsonPropertyName = userInput;
                }
                var possibleTypes = GetPossiblePropertyTypes(model, newProperty);

                Console.WriteLine("Here are the possible types for this property :");
                for (int i = 0; i < possibleTypes.Count; i++)
                {
                    Console.WriteLine($"_{i}_ : {possibleTypes.ElementAt(i)}");
                }
                do
                {
                    Console.WriteLine("Type the corresponding number the type you want to choose :");
                    userInput = Console.ReadLine();
                    intInput = int.Parse(userInput);
                }
                while (userInput == null || ((intInput < 0) || intInput >= possibleTypes.Count));
                

                newProperty.TypeFullName = possibleTypes.ElementAt(intInput);
            }


            model.Properties.Add(newProperty);
        }

        public static void CLI_EditProperty(Model model)
        {
            string userInput;
            int intInput;
            if(model.Properties.Count == 0)
            {
                Console.WriteLine("No properties were found for this model");
                return;
            }

            for(int i = 0; i < model.Properties.Count; i++)
            {
                Console.WriteLine($"_{i}_ : {model.Properties.ElementAt(i).Name}");
            }

            do
            {
                Console.WriteLine("Enter the number corresponding to the property you want to edit");
                userInput = Console.ReadLine();
                intInput = int.Parse(userInput);
            }
            while(userInput == null || ((intInput < 0) || intInput >= model.Properties.Count));

            var property = model.Properties.ElementAt(intInput);

            Console.WriteLine("Here are the available actions :");
            do
            {
                Console.WriteLine("a : modify Name");
                Console.WriteLine("b : toggle use onPropertyChanged");
                Console.WriteLine("c : modify JsonPropertyName");
                Console.WriteLine("d : modify type name");
                Console.WriteLine("e : exit");
                userInput = Console.ReadLine();

                if(userInput == "a")
                {
                    // Modify property name
                    ModelProperty sameProperty;
                    do
                    {
                        Console.WriteLine("Enter the new name for your property");
                        userInput = Console.ReadLine();
                        sameProperty = model.Properties.FirstOrDefault(p=>p.Name == userInput);
                    }while(sameProperty != null || userInput == "");

                    property.Name = userInput;
                }
                else if(userInput == "b")
                {
                    // Toggle useOnPropertyChanged
                    Console.WriteLine("Enter u to use onPropertyChanged, anything else to not");
                    if(userInput == "u")
                    {
                        property.IsValidForUpdate = true;
                    }
                    else
                    {
                        property.IsValidForUpdate = false;
                    }
                }
                else if(userInput=="c")
                {
                    // Modify JsonPropertyName
                    ModelProperty sameProperty;
                    do
                    {
                        Console.WriteLine("Enter the new name for your property");
                        userInput = Console.ReadLine();
                        sameProperty = model.Properties.FirstOrDefault(p => p.JsonPropertyName == userInput);
                    } while (sameProperty != null);
                    if(userInput != "")
                    {
                        property.JsonPropertyName = userInput;

                    }
                    else
                    {
                        property.JsonPropertyName = null;
                    }


                }
                else if(userInput == "d")
                {
                    // Modify type name
                    var possibleTypeNames = GetPossiblePropertyTypes(model, property);
                    for(int i = 0;i<possibleTypeNames.Count;i++)
                    {
                        Console.WriteLine($"_{i}_ : {possibleTypeNames[i]}");
                    }

                    do
                    {
                        Console.WriteLine("Enter the number corresponding to the type you want to use in your model :");
                        userInput = Console.ReadLine();
                        intInput = int.Parse(userInput);
                    }while(intInput<0 || intInput>= possibleTypeNames.Count);

                    property.TypeFullName = possibleTypeNames[intInput];
                }
                


            }
            while (userInput != "e");




        }

    }
}
