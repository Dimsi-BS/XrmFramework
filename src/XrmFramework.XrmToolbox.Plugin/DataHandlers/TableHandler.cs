using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.Core;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace XrmFramework.XrmToolbox.DataHandlers
{
    public static class TableHandler
    {
        public static string CurrentTable = null;
        public static string PathToRegisterTables = "";
        //private DataTable TableContent = new DataTable();
        //private TableCollection ProjectTables = new TableCollection();
        public static Dictionary<string, TableData> TableAndPath = new Dictionary<string, TableData>();
        public static Table globalEnumsTable = null;
        public static List<string> PublisherPrefixes { get; } = new();
        public static TableCollection BasicTables = new TableCollection();
        public static List<string> PreexistingTablePaths = new List<String>();
        public static string CurrentEnum = null;
        public static XrmFrameworkPluginControl PluginControl;


        public static void LoadTablesFromProject(string projectPath)
        {
            foreach (var fileName in Directory.GetFiles($"{projectPath}", "*.table", SearchOption.AllDirectories))
            {
                PreexistingTablePaths.Add(fileName);
                if (fileName.Contains("OptionSet"))
                {
                    //MessageBox.Show(fileName);
                    var fileInfo = new FileInfo(fileName);
                    var text = File.ReadAllText(fileInfo.FullName);
                    var table = JsonConvert.DeserializeObject<Table>(text);
                    globalEnumsTable = table;
                }
                else
                {
                    //MessageBox.Show(fileName);
                    var fileInfo = new FileInfo(fileName);
                    var text = File.ReadAllText(fileInfo.FullName);
                    var table = JsonConvert.DeserializeObject<Table>(text);
                    var splitFilename = fileName.Split('\\').ToList();
                    splitFilename.RemoveAt(splitFilename.Count - 1);
                    var rootPath = projectPath;
                    var splitRootPath = rootPath.Split('\\').ToList();
                    splitRootPath.Remove(splitRootPath.ElementAt(splitRootPath.Count - 1));
                    rootPath = String.Join("\\", splitRootPath);
                    //var path = RemoveCurrentProjectPathFromTablePath(string.Join("\\", splitFilename), projectPath);
                    var path = RemoveCurrentProjectPathFromTablePath(string.Join("\\",splitFilename), projectPath);

                    //MessageBox.Show(fileName);
                    //foreach(var txt in splitFilename)
                    //{
                    //    MessageBox.Show(txt);
                    //}
                    //MessageBox.Show(string.Join("\\", splitFilename));
                    //MessageBox.Show(projectPath);
                    //MessageBox.Show(path);
                    TableAndPath[table.LogicalName] = new TableData(table, path);
                }



            }


        }

        public static string RemoveCurrentProjectPathFromTablePath(string path, string projectPath)
        {

            var splitPath = projectPath.Split('\\').ToList();
            splitPath.RemoveAt(splitPath.Count - 1);
            var rootPath = string.Join("\\", splitPath);
            //var splitRootPath = rootPath.Split('\\').ToList();
            //splitRootPath.Remove(splitRootPath.ElementAt(splitRootPath.Count - 1));
            //rootPath = String.Join("\\", splitRootPath);
            return path.Replace(rootPath.Trim('\\'), "");
        }
        public static void ModifyEnumName(string previousText, OptionSetEnum en, string text)
        {
            var newForm = new TryOtherNameForm(text);
            newForm.FormClosing += (o, e) =>
            {
                //If the user has chosen to modify (he clicked the modify button) send ou object ?
                if (!newForm.ModifyName || newForm.Name == previousText)
                {
                    //MessageBox.Show("did not modify name");
                    return;
                }
                // Check if a table is already using this name 

                if (IsEnumNameUsed(en, newForm.Name))
                {
                    // If already used, don't modify table name and notify user
                    MessageBox.Show("This name is already in use");
                    return;
                }
                else
                {
                    // If not, then modify the name 
                    //MessageBox.Show("You modified this Name");
                    var finalName = RemovePrefix(newForm.Name).StrongFormat();
                    en.Name = finalName;

                }
            };

            newForm.ShowDialog();
        }

        private static bool IsEnumNameUsed(OptionSetEnum en, string name)
        {
            // Is enum name used in a global enum
            if (TableHandler.globalEnumsTable.Enums.Any(e => e.Name == name && e.LogicalName != en.LogicalName))
            {
                return true;
            }
            // Is enum name used in a table enum
            else
            {
                foreach (var key in TableHandler.TableAndPath.Keys)
                {
                    var currentTable = TableHandler.TableAndPath[key].table;
                    if (currentTable.Enums.Any(e => e.Name == name && e.LogicalName != en.LogicalName))
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        public static string RemovePrefix(string name)
        {
            foreach (var prefix in PublisherPrefixes)
            {
                if (!string.IsNullOrEmpty(prefix) && name.StartsWith(prefix))
                {
                    name = name.Substring(prefix.Length + 1);
                }
            }
            name = name.Substring(0, 1).ToUpperInvariant() + name.Substring(1);
            return name;
        }

        public static void ProcessBasicTableRequest(EntityMetadata[] response)
        {
            BasicTables.Clear();
            foreach (var entity in response)
            {

                if (entity.DisplayName.UserLocalizedLabel != null)
                {
                    BasicTables.Add(new Table()
                    {
                        LogicalName = entity.LogicalName,
                        //Name = RemovePrefix(entity.SchemaName).FormatText(),

                        Name = entity.DisplayName.UserLocalizedLabel.Label.StrongFormat(),

                    });
                }
                else
                {
                    BasicTables.Add(new Table()
                    {
                        LogicalName = entity.LogicalName,
                        Name = RemovePrefix(entity.SchemaName).FormatText(),

                        //Name = entity.DisplayName.UserLocalizedLabel.Label.StrongFormat(),

                    });
                }

            }

        }

        public static void ModifyEnumeValueName(OptionSetEnumValue enumValue, OptionSetEnum en, string text)
        {
            var newForm = new TryOtherNameForm(text);
            newForm.FormClosing += (o, e) =>
            {
                //If the user has chosen to modify (he clicked the modify button) send ou object ?
                if (!newForm.ModifyName || newForm.Name == enumValue.Name)
                {
                    //MessageBox.Show("did not modify name");
                    return;
                }
                // Check if a table is already using this name 

                if (en.Values.Any(v => v.Name == newForm.Name && v.Value != enumValue.Value))
                {
                    // If already used, don't modify table name and notify user
                    MessageBox.Show("This name is already in use");
                    return;
                }
                else
                {
                    // If not, then modify the name 
                    //MessageBox.Show("You modified this Name");
                    var finalName = RemovePrefix(newForm.Name).StrongFormat();
                    enumValue.Name = finalName;
                }
            };

            newForm.ShowDialog();
        }
        public static void ModifyTableName(string previousText, Table table, string text)
        {
            var newForm = new TryOtherNameForm(text);
            newForm.FormClosing += (o, e) =>
            {
                //If the user has chosen to modify (he clicked the modify button) send ou object ?
                if (!newForm.ModifyName || newForm.Name == previousText)
                {
                    //MessageBox.Show("did not modify name");
                    return;
                }
                // Check if a table is already using this name 

                if (IsTableNameUsed(table, newForm.Name))
                {
                    // If already used, don't modify table name and notify user
                    MessageBox.Show("This name is already in use");
                    return;
                }
                else
                {
                    // If not, then modify the name 
                    //MessageBox.Show("You modified this Name");
                    var finalName = RemovePrefix(newForm.Name).StrongFormat();
                    table.Name = finalName;
                }
            };

            newForm.ShowDialog();


        }
        public static void ProcessEntityResponse(Table table, EntityMetadata entity)
        {

            if (entity.Keys != null && entity.Keys.Any())
            {

                foreach (var key in entity.Keys)
                {

                    var newKey = new Key
                    {
                        LogicalName = key.LogicalName,
                        Name = key.DisplayName.UserLocalizedLabel.Label.FormatText()

                    };
                    newKey.FieldNames.AddRange(key.KeyAttributes);


                    if (!table.Keys.Any(k => k.LogicalName == newKey.LogicalName))
                    {
                        table.Keys.Add(newKey);

                    }
                }
            }
            table.OneToManyRelationships.Clear();
            if (entity.OneToManyRelationships.Any())
            {

                foreach (var relationship in entity.OneToManyRelationships)
                {


                    table.OneToManyRelationships.Add(new Relation
                    {
                        Name = relationship.SchemaName,
                        Role = EntityRole.Referenced,
                        EntityName = relationship.ReferencingEntity,
                        NavigationPropertyName = relationship.ReferencedEntityNavigationPropertyName,
                        LookupFieldName = relationship.ReferencingAttribute
                    });
                }
            }
            table.ManyToManyRelationships.Clear();
            if (entity.ManyToManyRelationships.Any())
            {

                foreach (var relationship in entity.ManyToManyRelationships)
                {

                    table.ManyToManyRelationships.Add(new Relation
                    {
                        Name = relationship.SchemaName,
                        Role = EntityRole.Referencing,
                        EntityName = relationship.Entity1LogicalName == table.LogicalName ? relationship.Entity2LogicalName : relationship.Entity1LogicalName,
                        NavigationPropertyName = relationship.IntersectEntityName,
                        LookupFieldName = relationship.Entity1LogicalName == table.LogicalName ? relationship.Entity2IntersectAttribute : relationship.Entity1IntersectAttribute
                    });
                }
            }
            table.ManyToOneRelationships.Clear();
            if (entity.ManyToOneRelationships.Any())
            {
                foreach (var relationship in entity.ManyToOneRelationships)
                {
                    table.ManyToOneRelationships.Add(new Relation
                    {
                        Name = relationship.SchemaName,
                        Role = EntityRole.Referencing,
                        NavigationPropertyName = relationship.ReferencingEntityNavigationPropertyName,
                        EntityName = relationship.ReferencedEntity,
                        LookupFieldName = relationship.ReferencingAttribute
                    });
                }
            }

            foreach (var attributeMetadata in entity.Attributes.OrderBy(a => a.LogicalName))
            {
                if (!attributeMetadata.IsValidForCreate.Value && !attributeMetadata.IsValidForRead.Value && !attributeMetadata.IsValidForUpdate.Value)
                {
                    continue;
                }
                if (attributeMetadata.AttributeType.Value == Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.EntityName || !string.IsNullOrEmpty(attributeMetadata.AttributeOf))
                {
                    continue;
                }

                string attributeEnumName = null;

                if (attributeMetadata.AttributeType.Value == Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Picklist || attributeMetadata.AttributeType.Value == Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.State || attributeMetadata.AttributeType.Value == Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Status || attributeMetadata.AttributeType.Value == Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Virtual && attributeMetadata is MultiSelectPicklistAttributeMetadata)
                {
                    var meta = ((EnumAttributeMetadata)attributeMetadata).OptionSet;

                    var enumLogicalName = meta.IsGlobal.Value ? meta.Name : entity.LogicalName + "|" + attributeMetadata.LogicalName;

                    attributeEnumName = enumLogicalName;


                    var newEnum = new OptionSetEnum
                    {
                        LogicalName = enumLogicalName,
                        IsGlobal = meta.IsGlobal.Value,
                        HasNullValue = attributeMetadata.AttributeType.Value == Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Picklist &&
                                       meta.Options.All(option => option.Value.GetValueOrDefault() != 0),
                    };

                    if (attributeMetadata.AttributeType.Value == Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.State)
                    {

                        newEnum.Name = table.Name.Replace("Definition", "") + "State";
                    }
                    else if (attributeMetadata.AttributeType.Value == Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Status)
                    {
                        newEnum.Name = table.Name.Replace("Definition", "") + "Status";
                    }
                    else
                    {
                        newEnum.Name = meta.DisplayName.UserLocalizedLabel?.Label.FormatText();
                    }

                    if (string.IsNullOrEmpty(newEnum.Name))
                    {
                        continue;
                    }

                    foreach (var option in meta.Options)
                    {
                        if (option.Label.UserLocalizedLabel == null)
                        {
                            continue;
                        }

                        var optionValue = new OptionSetEnumValue
                        {
                            Name = option.Label.UserLocalizedLabel.Label.FormatText(),
                            Value = option.Value.Value,
                            ExternalValue = option.ExternalValue
                        };

                        foreach (var displayNameLocalizedLabel in option.Label.LocalizedLabels)
                        {
                            optionValue.Labels.Add(new XrmFramework.Core.LocalizedLabel
                            {
                                Label = displayNameLocalizedLabel.Label,
                                LangId = displayNameLocalizedLabel.LanguageCode
                            });
                        }

                        newEnum.Values.Add(optionValue);

                    }

                    if (!newEnum.IsGlobal)
                    {
                        var sameEnum = table.Enums.FirstOrDefault(e => e.LogicalName == newEnum.LogicalName);
                        if (sameEnum != null)
                        {
                            newEnum.Name = sameEnum.Name;
                            table.Enums.Remove(sameEnum);
                            table.Enums.Add(newEnum);
                        }
                        else
                        {
                            while (IsEnumNameUsed(newEnum, newEnum.Name))
                            {
                                ModifyEnumName(newEnum.Name, newEnum, newEnum.Name);
                            }
                            table.Enums.Add(newEnum);

                        }
                        //table.Enums.Add(newEnum);
                    }
                    else if (TableHandler.globalEnumsTable.Enums.All(e => e.LogicalName != newEnum.LogicalName))
                    {
                        var sameEnum = TableHandler.globalEnumsTable.Enums.FirstOrDefault(e => e.LogicalName == newEnum.LogicalName);
                        if (sameEnum != null)
                        {
                            newEnum.Name = sameEnum.Name;
                            TableHandler.globalEnumsTable.Enums.Remove(sameEnum);
                            TableHandler.globalEnumsTable.Enums.Add(newEnum);
                        }
                        else
                        {
                            while (IsEnumNameUsed(newEnum, newEnum.Name))
                            {
                                ModifyEnumName(newEnum.Name, newEnum, newEnum.Name);
                            }
                            TableHandler.globalEnumsTable.Enums.Add(newEnum);

                        }
                    }


                }

                var name = RemovePrefix(attributeMetadata.SchemaName);

                if (attributeMetadata.LogicalName == entity.PrimaryIdAttribute)
                {
                    name = "Id";
                }

                int? maxLength = null;
                double? minRangeDouble = null, maxRangeDouble = null;

                switch (attributeMetadata.AttributeType.Value)
                {
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.String:
                        maxLength = ((StringAttributeMetadata)attributeMetadata).MaxLength;
                        break;
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Memo:
                        maxLength = ((MemoAttributeMetadata)attributeMetadata).MaxLength;
                        break;
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Money:
                        var m = (MoneyAttributeMetadata)attributeMetadata;
                        minRangeDouble = m.MinValue;
                        maxRangeDouble = m.MaxValue;
                        break;
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Integer:
                        var mi = (IntegerAttributeMetadata)attributeMetadata;
                        minRangeDouble = mi.MinValue;
                        maxRangeDouble = mi.MaxValue;
                        break;
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Double:
                        var md = (DoubleAttributeMetadata)attributeMetadata;
                        minRangeDouble = md.MinValue;
                        maxRangeDouble = md.MaxValue;
                        break;
                    case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Decimal:
                        var mde = (DecimalAttributeMetadata)attributeMetadata;
                        minRangeDouble = (double?)mde.MinValue;
                        maxRangeDouble = (double?)mde.MaxValue;
                        break;
                }


                var attribute = new Column
                {
                    LogicalName = attributeMetadata.LogicalName,
                    Name = RemovePrefix(name).FormatText(),
                    Type = (XrmFramework.AttributeTypeCode)(int)(attributeMetadata.AttributeType.Value == Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Virtual && attributeMetadata is MultiSelectPicklistAttributeMetadata
                        ? Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Picklist : attributeMetadata.AttributeType.Value),
                    IsMultiSelect = attributeMetadata.AttributeType.Value == Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Virtual && attributeMetadata is MultiSelectPicklistAttributeMetadata,
                    PrimaryType = attributeMetadata.LogicalName == entity.PrimaryIdAttribute ?
                        PrimaryType.Id :
                        attributeMetadata.LogicalName == entity.PrimaryNameAttribute ?
                            PrimaryType.Name :
                            attributeMetadata.LogicalName == entity.PrimaryImageAttribute ? PrimaryType.Image : PrimaryType.None,
                    StringLength = maxLength,
                    MinRange = minRangeDouble,
                    MaxRange = maxRangeDouble,
                    EnumName = attributeEnumName,
                    Selected = false
                };

                foreach (var displayNameLocalizedLabel in attributeMetadata.DisplayName.LocalizedLabels)
                {
                    attribute.Labels.Add(new XrmFramework.Core.LocalizedLabel
                    {
                        Label = displayNameLocalizedLabel.Label,
                        LangId = displayNameLocalizedLabel.LanguageCode
                    });
                }

                if (attributeMetadata.IsValidForAdvancedFind.Value)
                {
                    attribute.Capabilities |= AttributeCapabilities.AdvancedFind;
                }

                if (attributeMetadata.IsValidForCreate.Value)
                {
                    attribute.Capabilities |= AttributeCapabilities.Create;
                }

                if (attributeMetadata.IsValidForRead.Value)
                {
                    attribute.Capabilities |= AttributeCapabilities.Read;
                }

                if (attributeMetadata.IsValidForUpdate.Value)
                {
                    attribute.Capabilities |= AttributeCapabilities.Update;
                }


                if (attributeMetadata.AttributeType == Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.DateTime)
                {
                    var meta = (DateTimeAttributeMetadata)attributeMetadata;

                    attribute.DateTimeBehavior = meta.DateTimeBehavior.ToDateTimeBehavior();
                }

                if (attributeMetadata.LogicalName == "ownerid")
                {

                }





                table.Columns.Add(attribute);
            }

        }
        public static bool IsTableNameUsed(Table table, string name)
        {
            if (TableHandler.TableAndPath.Any(t => t.Value.table.Name == name && t.Value.table.LogicalName != table.LogicalName))
            {
                return true;
            }
            return false;
        }

        public static void CheckDefaultSelectColumns()
        {
            Table currentTable;
            foreach (var key in TableHandler.TableAndPath.Keys)
            {

                currentTable = TableHandler.TableAndPath[key].table;
                //Check for key columns

                foreach (var currentKey in currentTable.Keys)
                {
                    foreach (var fieldName in currentKey.FieldNames)
                    {
                        var correspondingColumn = currentTable.Columns.FirstOrDefault(c => c.LogicalName == fieldName);
                        if (correspondingColumn != null)
                        {
                            correspondingColumn.Selected = true;
                        }
                    }
                }

                var idColumn = currentTable.Columns.FirstOrDefault(c => c.PrimaryType == PrimaryType.Id);
                var nameColumn = currentTable.Columns.FirstOrDefault(c => c.PrimaryType == PrimaryType.Name);
                if (idColumn != null)
                {
                    idColumn.Selected = true;
                }
                if (nameColumn != null)
                {
                    nameColumn.Selected = true;
                }
            }


            



        }

        public static void CheckForDuplicateTableFile(string finalPath, Table table)
        {
            // Check for exisiting file
            if (File.Exists(finalPath))
            {
                return;
            }

            // Iterate through each preexisting tableFile 
            foreach (var path in TableHandler.PreexistingTablePaths)
            {
                if(!File.Exists(path))
                {
                    return;
                }
                var fileInfo = new FileInfo(path);
                var text = File.ReadAllText(fileInfo.FullName);
                var deserializedTable = JsonConvert.DeserializeObject<Table>(text);
                if (deserializedTable.LogicalName == table.LogicalName)
                {
                    //Delete first file
                    File.Delete(path);

                }
            }
        }

        public static void SaveTables()
        {
            if (!Directory.Exists(TableHandler.PathToRegisterTables))
            {
                Directory.CreateDirectory(TableHandler.PathToRegisterTables);
            }

            TableHandler.CheckDefaultSelectColumns();

            foreach (var key in TableHandler.TableAndPath.Keys)
            {
                var path = TableHandler.TableAndPath[key].path;
                var table = TableHandler.TableAndPath[key].table;
                var splitPath = PluginControl.CurrentProject.FolderPath.Split('\\').ToList();
                //var rootPath = mySettings.RootFolders.FirstOrDefault(r => r.OrganizationName == mySettings.CurrentOrganizationName).FolderPath;
                //var splitRootPath = rootPath.Split('\\').ToList();
                splitPath.Remove(splitPath.ElementAt(splitPath.Count - 1));

                var splitTablePath = path.Split('\\').ToList();
                //splitTablePath = 

                //var project = mySettings.RootFolders.FirstOrDefault(r => r.FolderPath == CurrentProject);
                if (PluginControl.CurrentProject == null)
                {
                    MessageBox.Show("Could not save tables");
                    return;
                }
                
                var projectPath = String.Join("\\", splitPath);
                var txt = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
                //MessageBox.Show(projectPath);
                //MessageBox.Show(registrationPath);


                var finalPath = projectPath +path +"\\"+ $"{table.Name}.table";
                CheckForDuplicateTableFile(finalPath, table);
                var fileInfo = new FileInfo(finalPath);
                File.WriteAllText(fileInfo.FullName, txt);
            }

            // Register global enums

            var enumsPath = TableHandler.PathToRegisterTables + $"{TableHandler.globalEnumsTable.Name}.table";

            var splitEnumPath = enumsPath.Split('\\').ToList();
            //var rootPath = mySettings.RootFolders.FirstOrDefault(r => r.OrganizationName == mySettings.CurrentOrganizationName).FolderPath;
            //var splitRootPath = rootPath.Split('\\').ToList();
            splitEnumPath.Remove(splitEnumPath.ElementAt(splitEnumPath.Count - 1));
            splitEnumPath.Remove(splitEnumPath.ElementAt(0));
            splitEnumPath.Remove(splitEnumPath.ElementAt(0));



            var projectE = PluginControl.Settings.RootFolders.FirstOrDefault(r => r.OrganizationName == PluginControl.Settings.CurrentOrganizationName);
            if (projectE == null)
            {
                MessageBox.Show("Could not save enums");
                return;
            }
            var projectPathE = projectE.FolderPath;
            var registrationPathE = String.Join("\\", splitEnumPath);

            var txtE = JsonConvert.SerializeObject(TableHandler.globalEnumsTable, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            var fileInfoE = new FileInfo(enumsPath);
            File.WriteAllText(fileInfoE.FullName, txtE);
        }



        
        


    }

    public class TableData
    {
        public Table table;
        public String path;
        public TableData(Table table, string path)
        {
            this.table = table;
            this.path = path;
        }
    }
}
