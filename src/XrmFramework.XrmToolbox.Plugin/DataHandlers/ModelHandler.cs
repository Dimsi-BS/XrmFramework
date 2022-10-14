using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XrmFramework.Core;
using XrmFramework.XrmToolbox.Forms;


namespace XrmFramework.XrmToolbox.DataHandlers

{
    public static class ModelHandler
    {
        public static Dictionary<string, ModelData> ModelAndPath = new Dictionary<string, ModelData>();
        public static List<string> PreexistingModelPaths = new List<string>();
        public static XrmFrameworkPluginControl PluginControl;
        public static string PathToRegisterModel;
        private static SortedSet<string> existingNamespaces = new SortedSet<string>();
        public static void LoadModelsFromProject(string projectPath)
        {
            foreach (var fileName in Directory.GetFiles($"{projectPath}", "*.model", SearchOption.AllDirectories))
            {
                PreexistingModelPaths.Add(fileName);

                //MessageBox.Show(fileName);
                var fileInfo = new FileInfo(fileName);
                var text = File.ReadAllText(fileInfo.FullName);
                var model = JsonConvert.DeserializeObject<XrmFramework.Core.Model>(text);

                var splitFilename = fileName.Split('\\').ToList();
                splitFilename.RemoveAt(splitFilename.Count - 1);
                var path = RemoveCurrentProjectPathFromModelPath(string.Join("\\", splitFilename), projectPath);
                //var rootPath = projectPath;
                //var splitRootPath = rootPath.Split('\\').ToList();
                //splitRootPath.Remove(splitRootPath.ElementAt(splitRootPath.Count - 1));
                //rootPath = String.Join("\\", splitRootPath);
                ModelAndPath[model.Name] = new ModelData(model, path);
                if (!string.IsNullOrEmpty(model.ModelNamespace))
                {
                    existingNamespaces.Add(model.ModelNamespace);
                }



            }


        }

        public static void AddModel()
        {
            var createModelForm = new CreateModelForm();
            createModelForm.PluginControl = PluginControl;
            TableCollection tables = new TableCollection();
            foreach (var key in TableHandler.TableAndPath.Keys)
            {
                tables.Add(TableHandler.TableAndPath[key].table);
            }
            createModelForm.SetTableBindingSource(tables);
            createModelForm.SetExistingNamespaces(existingNamespaces.ToList());
            createModelForm.FormClosing += (s, e) =>
            {
                if (!createModelForm.CreateModel)
                {
                    return;
                }
                // Create the model 
                var model = new XrmFramework.Core.Model()
                {
                    TableLogicalName = createModelForm.tableLogicalName,
                    ModelNamespace = createModelForm.modelNamespace,
                    Name = createModelForm.modelName,
                };
                var table = TableHandler.TableAndPath[model.TableLogicalName].table;
                var idCol = table.Columns.FirstOrDefault(c => c.PrimaryType == PrimaryType.Id);
                if (idCol == null)
                {
                    throw new Exception("Error, table id corresponding to model was not found");
                }
                var idProperty = new ModelProperty();
                idProperty.Name = "Id";
                idProperty.LogicalName = idCol.LogicalName;
                idProperty.TypeFullName = "Guid";

                var nameCol = table.Columns.FirstOrDefault(c => c.PrimaryType == PrimaryType.Name);
                //idProperty.JsonPropertyName = 
                var nameProperty = new ModelProperty();

                if (nameCol == null)
                {
                    throw new Exception("Error, table name corresponding to model was not found");
                }
                nameProperty.Name = "Name";
                nameProperty.LogicalName = nameCol.LogicalName;
                nameProperty.TypeFullName = "string";
                model.Properties.Add(nameProperty);
                model.Properties.Add(idProperty);

                // Create the path to register it
                ModelAndPath[model.Name] = new ModelData(model, ModelHandler.RemoveCurrentProjectPathFromModelPath(ModelHandler.PathToRegisterModel, PluginControl.CurrentProject.FolderPath));
                existingNamespaces.Add(model.ModelNamespace);
                PluginControl.RefreshModelsDisplay();

            };
            createModelForm.ShowDialog();
        }
        public static string RemoveCurrentProjectPathFromModelPath(string path, string projectPath)
        {
            var splitPath = path.Split('\\');
            var rootPath = projectPath;
            var splitRootPath = rootPath.Split('\\').ToList();
            splitRootPath.Remove(splitRootPath.ElementAt(splitRootPath.Count - 1));
            rootPath = String.Join("\\", splitRootPath);
            return path.Replace(rootPath.Trim('\\'), "");
        }

        public static void SaveModels()
        {
            if (string.IsNullOrEmpty(PathToRegisterModel))
            {

                return;
            }
            if (!Directory.Exists(ModelHandler.PathToRegisterModel))
            {
                Directory.CreateDirectory(ModelHandler.PathToRegisterModel);
            }

            foreach (var key in ModelHandler.ModelAndPath.Keys)
            {
                var path = ModelHandler.ModelAndPath[key].path;
                var model = ModelHandler.ModelAndPath[key].model;
                var splitPath = PluginControl.CurrentProject.FolderPath.Split('\\').ToList();
                //var rootPath = mySettings.RootFolders.FirstOrDefault(r => r.OrganizationName == mySettings.CurrentOrganizationName).FolderPath;
                //var splitRootPath = rootPath.Split('\\').ToList();
                splitPath.Remove(splitPath.ElementAt(splitPath.Count - 1));

                var splitTablePath = path.Split('\\').ToList();
                //splitTablePath = 

                //var project = mySettings.RootFolders.FirstOrDefault(r => r.FolderPath == CurrentProject);
                if (PluginControl.CurrentProject == null)
                {
                    MessageBox.Show("Could not save model");
                    return;
                }

                var projectPath = String.Join("\\", splitPath);
                var txt = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
                //MessageBox.Show(projectPath);
                //MessageBox.Show(registrationPath);


                var finalPath = projectPath + path + "\\" + $"{model.Name}.model";
                //CheckForDuplicateTableFile(finalPath, table);
                var fileInfo = new FileInfo(finalPath);
                File.WriteAllText(fileInfo.FullName, txt);
            }
        }

        public static List<String> GetPossiblePropertyTypes(XrmFramework.Core.Model model, string columnLogicalName)
        {
            OptionSetEnum FirstEnum;
            OptionSetEnum SecondEnum;
            var possibleTypes = new List<String>();

            // Get the corresponding tabke
            var table = TableHandler.TableAndPath[model.TableLogicalName].table;
            if (table == null)
            {
                return possibleTypes;
            }
            var column = table.Columns.FirstOrDefault(c => c.LogicalName == columnLogicalName);
            Relation relation;
            if (column == null)
            {
                relation = table.OneToManyRelationships.FirstOrDefault(r => r.Name == columnLogicalName);
                if (relation == null)
                {
                    return possibleTypes;
                }

                var possibleModelAdded = false;
                // Find a corresponding model and return
                foreach (var key in ModelAndPath.Keys)
                {
                    var possibleModel = ModelAndPath[key].model;
                    if (possibleModel.TableLogicalName == relation.EntityName)
                    {
                        //possibleTypes.Add($"{CoreProjectName}.{possibleModel.ModelNamespace}.{possibleModel.Name}");
                        //Todo : Find a way to use the full typename of the model
                        possibleTypes.Add(possibleModel.ModelNamespace + "." + possibleModel.Name);
                        possibleModelAdded = true;


                    }
                }
                if (!possibleModelAdded)
                {
                    // Find corresponding table, and get its name
                    // Find corresponding table, and get its name
                    if (!TableHandler.TableAndPath.ContainsKey(relation.EntityName))
                    {
                        MessageBox.Show($"can't find table {relation.EntityName}");
                    }
                    else
                    {
                        var correspondingTable = TableHandler.TableAndPath[relation.EntityName].table;
                        possibleTypes.Add(correspondingTable.Name + "Model");
                    }
                }
                //foreach (var possibleModel in Models)
                //{
                //    if (possibleModel.TableLogicalName == relation.EntityName)
                //    {
                //          
                //    }
                //}

                possibleTypes.Add("System.Guid");
                possibleTypes.Add("Microsoft.Xrm.Sdk.EntityReference");

                return possibleTypes;
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
                    var possibleModelAdded = false;
                    foreach (var key in ModelAndPath.Keys)
                    {
                        var possibleModel = ModelAndPath[key].model;
                        //TODO : find a way to use the typefullname
                        if (possibleModel.TableLogicalName == re.EntityName)
                        {
                            possibleTypes.Add(possibleModel.ModelNamespace + "." + possibleModel.Name);
                            possibleModelAdded = true;
                        }


                    }
                    if (!possibleModelAdded)
                    {
                        // Find corresponding table, and get its name
                        if (!TableHandler.TableAndPath.ContainsKey(re.EntityName))
                        {
                            MessageBox.Show($"can't find table {re.EntityName}");
                        }
                        else
                        {
                            var correspondingTable = TableHandler.TableAndPath[re.EntityName].table;
                            possibleTypes.Add(correspondingTable.Name + "Model");
                        }

                    }
                    //foreach (var possibleModel in Models)
                    //{
                    //    if (possibleModel.TableLogicalName == re.EntityName)
                    //    {
                    //        possibleTypes.Add($"{CoreProjectName}.{possibleModel.ModelNamespace}.{possibleModel.Name}");
                    //
                    //    }
                    //}

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
                        SecondEnum = TableHandler.globalEnumsTable.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                        //SecondEnum = GlobalEnums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                        if (SecondEnum != null)
                        {
                            // TODO : Find a way to get the namespace
                            //possibleTypes.Add($"{CoreProjectName}.{SecondEnum.Name}");
                            possibleTypes.Add($"{SecondEnum.Name}");
                        }
                        else
                        {
                            throw new Exception("No corresponding enumeration found for property : " + column.Name);
                        }
                    }
                    else
                    {
                        // TODO : Find a way to get the namespace
                        //possibleTypes.Add($"{CoreProjectName}.{FirstEnum.Name}");
                        possibleTypes.Add($"{FirstEnum.Name}");

                    }

                    possibleTypes.Add("System.Int32");
                    break;
                case AttributeTypeCode.State:

                    FirstEnum = table.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                    if (FirstEnum == null)
                    {
                        SecondEnum = TableHandler.globalEnumsTable.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                        //SecondEnum = GlobalEnums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                        if (SecondEnum != null)
                        {
                            // TODO : Find a way to get the namespace

                            possibleTypes.Add($"{SecondEnum.Name}");
                        }
                        else
                        {
                            throw new Exception("No corresponding enumeration found for property : " + column.Name);
                        }
                    }
                    else
                    {
                        // TODO : Find a way to get the namespace

                        possibleTypes.Add($"{FirstEnum.Name}");
                    }

                    possibleTypes.Add("System.Int32");
                    break;
                case AttributeTypeCode.Status:
                    FirstEnum = table.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                    if (FirstEnum == null)
                    {
                        SecondEnum = TableHandler.globalEnumsTable.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                        //SecondEnum = GlobalEnums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                        if (SecondEnum != null)
                        {
                            // TODO : Find a way to get the namespace
                            possibleTypes.Add($"{SecondEnum.Name}");
                        }
                        else
                        {
                            throw new Exception("No corresponding enumeration found for property : " + column.Name);
                        }
                    }
                    else
                    {
                        // TODO : Find a way to get the namespace
                        possibleTypes.Add($"{FirstEnum.Name}");
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

        public static bool IsPropertyNameUsed(string name, XrmFramework.Core.Model model)
        {
            foreach (var prop in model.Properties)
            {
                if (prop.Name == name)
                {
                    return true;
                }
            }

            return false;
        }
        public static bool IsModelNameUsed(string name)
        {
            if (ModelAndPath.ContainsKey(name))
            {
                return true;
            }
            return false;
        }

        public static bool IsJsonPropertyNameUsed(string name, Core.Model model)
        {
            foreach (var prop in model.Properties)
            {
                if (prop.JsonPropertyName == name)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class ModelData
    {
        public XrmFramework.Core.Model model;
        public String path;
        public ModelData(XrmFramework.Core.Model model, string path)
        {
            this.model = model;
            this.path = path;
        }
    }
}
