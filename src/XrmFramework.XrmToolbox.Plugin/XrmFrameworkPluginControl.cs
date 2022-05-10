using Deploy;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XrmFramework.Core;
using XrmToolBox.Extensibility;

namespace XrmFramework.XrmToolbox
{
    public partial class XrmFrameworkPluginControl : PluginControlBase
    {
        private string currentTable = null;
        private string pathToRegisterTables = "";
        private Settings mySettings;
        private DataTable TableContent = new DataTable();
        private TableCollection ProjectTables = new TableCollection();
        private Dictionary<string, TableData> TableAndPath = new Dictionary<string, TableData>();
        private Table globalEnumsTable = null;
        private List<string> PublisherPrefixes { get; } = new();
        private TableCollection BasicTables = new TableCollection();
        private Project CurrentProject;
        private List<string> PreexistingTablePaths = new List<String>();



        public XrmFrameworkPluginControl()
        {
            InitializeComponent();
            
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("This is a notification that can lead to XrmToolBox repository", new Uri("https://github.com/MscrmTools/XrmToolBox"));

            // Loads or creates the settings for the plugin
            if(mySettings == null)

            {
                if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
                {
                    mySettings = new Settings();

                    LogWarning("Settings not found => a new settings file has been created!");
                }
                else
                {
                    LogInfo("Settings found and loaded");
                }
            }
            mySettings.LastUsedOrganizationWebappUrl = ConnectionDetail.WebApplicationUrl;
            LogInfo("Connection has changed to: {0}", ConnectionDetail.WebApplicationUrl);
            // Add organizationName
            mySettings.CurrentOrganizationName = ConnectionDetail.OrganizationFriendlyName;
            SettingsManager.Instance.Save(GetType(), mySettings);
            //var currentPair = mySettings.RootFolders.FirstOrDefault(p => p.OrganizationName == mySettings.CurrentOrganizationName);
            CurrentProject = mySettings.RootFolders.FirstOrDefault(p => p.OrganizationName == mySettings.CurrentOrganizationName);
            pathToRegisterTables = CurrentProject.FolderPath + "\\Definitions\\";
            if (CurrentProject != null)
            {
                LoadTablesFromProject(CurrentProject.FolderPath);
                RefreshTreeDisplay();
                RefreshGlobalEnum();
            }

            if (PublisherPrefixes.Count == 0)
            {
                LoadPrefixes();
            }
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            
            CloseTool();
        }

        private void tsbSample_Click(object sender, EventArgs e)
        {
            // The ExecuteMethod method handles connecting to an
            // organization if XrmToolBox is not yet connected
            ExecuteMethod(GetAccounts);
        }

        private void GetAccounts()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting accounts",
                Work = (worker, args) =>
                {
                    args.Result = Service.RetrieveMultiple(new QueryExpression("account")
                    {
                        TopCount = 50
                    });
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as EntityCollection;
                    if (result != null)
                    {
                        MessageBox.Show($"Found {result.Entities.Count} accounts");
                    }
                }
            });
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
                // Add organizationName
                mySettings.CurrentOrganizationName = detail.OrganizationFriendlyName;
                SettingsManager.Instance.Save(GetType(), mySettings);
            }
            
        }

        private void toolStripMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        

        private void button1_Click(object sender, EventArgs e)
        {
           
            ProjectChoiceDialog.ShowDialog();
            //mySettings.RootFolders[]
            
            var pairToRemove = mySettings.RootFolders.FirstOrDefault(p => p.OrganizationName == mySettings.CurrentOrganizationName);
            if(pairToRemove != null)
            {
                mySettings.RootFolders.Remove(pairToRemove);
            
            }
            mySettings.RootFolders.Add(new Project(mySettings.CurrentOrganizationName, ProjectChoiceDialog.SelectedPath));
            CurrentProject = mySettings.RootFolders.FirstOrDefault(p => p.OrganizationName == mySettings.CurrentOrganizationName);
            //var finalPair = mySettings.RootFolders.FirstOrDefault(p => p.OrganizationName == mySettings.CurrentOrganizationName);
            MessageBox.Show($"Project folder has been set to {CurrentProject.FolderPath}");

            MessageBox.Show($"Looking for folders in {CurrentProject.FolderPath}");
            SettingsManager.Instance.Save(GetType(), mySettings);

            LoadTablesFromProject(CurrentProject.FolderPath);
            RefreshTreeDisplay();

            

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void LoadTablesFromProject(string projectPath)
        {
            foreach (var fileName in Directory.GetFiles($"{projectPath}", "*.table", SearchOption.AllDirectories))
            {
                PreexistingTablePaths.Add(fileName);
                if(fileName.Contains("OptionSet"))
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
                    ProjectTables.Add(table);

                    var splitFilename = fileInfo.Name.Split('\\');
                    var rootPath = mySettings.RootFolders.FirstOrDefault(r => r.OrganizationName == mySettings.CurrentOrganizationName).FolderPath;
                    var splitRootPath = rootPath.Split('\\').ToList();
                    splitRootPath.Remove(splitRootPath.ElementAt(splitRootPath.Count - 1));
                    rootPath = String.Join("\\", splitRootPath);

                    TableAndPath[table.LogicalName] = new TableData(table, RemoveCurrentProjectPathFromTablePath(fileName));
                }
                
            }

            if(globalEnumsTable == null)
            {
                globalEnumsTable = new Table()
                {
                    LogicalName = "globalEnums",
                    Name = "OptionSet"
                };
            }

            RefreshTreeDisplay();
            
        }

        public void RefreshTreeDisplay()
        {
            TableTreeView.Nodes.Clear();
            AddTableTreeNodes();
            if(TableTreeView.Nodes.Count!=0)
            {
                TableTreeView.Nodes[0].ExpandAll();

            }
        }

        private void AddTableTreeNodes()
        {

            
            foreach (var key in TableAndPath.Keys)
            {
                
                var path = TableAndPath[key].path.Split('\\');
                AddPathTreeNode(null, path, 0,TableAndPath[key].table.LogicalName);
                
            }
        }

        private void AddPathTreeNode(TreeNode? currentNode,string[] path,int index,string tableLogicalName)
        {
            if(index>=path.Length)
            {
                return;
            }
            else if(path[index] == "")
            {
                AddPathTreeNode(currentNode, path, index + 1,tableLogicalName);
                return;
            }
            
            var newNode = new TreeNode(path[index]);
            newNode.Name = path[index];
            
            if (currentNode == null)
            {
                if(TableTreeView.Nodes.Count >0)
                {
                    AddPathTreeNode(TableTreeView.Nodes[0], path, index + 1,tableLogicalName);
                }
                else
                {
                    if (path[index].Contains(".table"))
                    {
                        newNode.Name = tableLogicalName; 
                    }
                    TableTreeView.Nodes.Add(newNode);
                    AddPathTreeNode(newNode, path, index + 1,tableLogicalName);

                }


                return;

            }
            var existingNode = currentNode.Nodes.Find(newNode.Name,false);
            if(existingNode.Length ==0)
            {
                if (path[index].Contains(".table"))
                {
                    newNode.Name = tableLogicalName;
                }
                currentNode.Nodes.Add(newNode);
                AddPathTreeNode(newNode, path, index + 1,tableLogicalName);

            }
            else
            {
                AddPathTreeNode(existingNode[0], path, index + 1,tableLogicalName);
            }

        }

        private void TableTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //MessageBox.Show(e.Node.Name);
            var name = e.Node.Name;
            
            if(!TableAndPath.ContainsKey(name))
            {
                return;
            }
            var selectedTable = TableAndPath[name].table;
            if(selectedTable != null)
            {
                //MessageBox.Show($"You selected a table file of name {selectedTable.Name}");
                tableBindingSource.DataSource = selectedTable;
                columnBindingSource.DataSource = selectedTable.Columns;
                currentTable = selectedTable.LogicalName;
                
            }
            
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());
            if(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString() == "Picklist")
            {
                //var correspondingEnum
                var selectedTable = TableAndPath[currentTable].table;
                var column = selectedTable.Columns.FirstOrDefault(c=>c.LogicalName == dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                var correspondingEnum = selectedTable.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                if(correspondingEnum == null)
                {
                    correspondingEnum = globalEnumsTable.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                }
                dataGridView3.DataSource = correspondingEnum.Values;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void RetrieveEntitiesButton_Click(object sender, EventArgs e)
        {
            if (PublisherPrefixes.Count == 0)
            {
                LoadPrefixes();
            }
            //RetrieveAllEntities();
        }

        public TableCollection RetrieveAllEntities(BindingSource bindingSource,TableCollection basicTables)
        {
            //var basicTables = new TableCollection();
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving base entities data",

                Work = (worker, args) =>
                {
                    var req = new RetrieveAllEntitiesRequest
                    {
                        EntityFilters = EntityFilters.Entity 
                    };

                    var response = (RetrieveAllEntitiesResponse)Service.Execute(req);

                    args.Result = response.EntityMetadata;
                },

                PostWorkCallBack = (args)=>
                {
                    // Create the corresponding base tables while also processing the names
                    var entitiesData = (EntityMetadata[])args.Result;
                    foreach(var entity in entitiesData)
                    {
                        basicTables.Add(new Table()
                        {
                            LogicalName = entity.LogicalName,
                            Name = RemovePrefix(entity.SchemaName).FormatText(),

                        });
                    }
                    bindingSource.DataSource = basicTables;
                    //tableBindingSource1.DataSource = BasicTables;


                }
                
            });
            return basicTables;
        }

        public string RemovePrefix(string name)
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

        public void LoadPrefixes()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving Prefixes",

                Work = (worker, args) =>
                {

                    var query = new QueryExpression(Solution.EntityLogicalName);
                    query.ColumnSet.AllColumns = true;
                    var linkPublisher = query.AddLink(Deploy.Publisher.EntityLogicalName, "publisherid", "publisherid");
                    linkPublisher.EntityAlias = "publisher";
                    linkPublisher.Columns.AddColumn("customizationprefix");

                    var solutions = Service.RetrieveMultiple(query).Entities.Select(s => s.ToEntity<Solution>());
                    args.Result = solutions.ToArray();

                },

                PostWorkCallBack = (args) =>
                {
                    // Create the corresponding base tables while also processing the names
                    Solution[] solutions = (Solution[])args.Result;
                    
                    PublisherPrefixes.AddRange(solutions.Select(s => s.GetAttributeValue<AliasedValue>("publisher.customizationprefix").Value as string).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());

                }

            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddBasicTablesToProject();
        }

        private void AddBasicTablesToProject()
        {
            foreach(var table in BasicTables.Where(t=>t.Selected))
            {
                //MessageBox.Show($"Adding {table.Name}");
                //var txt = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings
                //{
                //    DefaultValueHandling = DefaultValueHandling.Ignore
                //});
                //var fileInfo = new FileInfo(pathToRegisterTables + $"{table.Name}.table");
                //File.WriteAllText(fileInfo.FullName, txt);
                
                //var projectData = mySettings.RootFolders.FirstOrDefault(p => p.OrganizationName == mySettings.CurrentOrganizationName);
                //if(projectData != null)
                //{
                //    LoadTablesFromProject(projectData.FolderPath);
                //    
                //}
                if(TableAndPath.ContainsKey(table.LogicalName))
                {
                    //MessageBox.Show("This is a message for when a table is already in a project, it should be replaced with a dialog box asking wether you want to overwrite it");
                    return;
                }
                TableAndPath[table.LogicalName] = new TableData(table, RemoveCurrentProjectPathFromTablePath(pathToRegisterTables+$"{table.Name}.table"));


                RefreshTreeDisplay();
                RefreshGlobalEnum();



                //Serialize
                //Saveit
                //Refresh tree 

            }
        }

       

        private void SearchColumnTextBox_TextChanged(object sender, EventArgs e)
        {
            if(currentTable != null)
            {
                if(TableAndPath.ContainsKey(currentTable))
                {
                    var search = SearchColumnTextBox.Text.Split(' ');
                    
                    if(search.Length == 1)
                    {
                        var lowerSearch = SearchColumnTextBox.Text.ToLower();

                        var selectedTable = TableAndPath[currentTable].table;
                        columnBindingSource.DataSource = selectedTable.Columns.Where(t => t.Name.ToLower().Contains(lowerSearch) || t.Type.ToString().ToLower().Contains(lowerSearch) || t.LogicalName.Contains(lowerSearch));

                    }
                    else
                    {
                        var selectedTable = TableAndPath[currentTable].table;

                        var columnsToShow = new ColumnCollection();
                        foreach(var searchWord in search)
                        {
                            if(searchWord == " " || searchWord =="")
                            {
                                continue;
                            }
                            var lowerSearch = searchWord.ToLower();
                            var correspondingColumns = selectedTable.Columns.Where(t => t.Name.ToLower().Contains(lowerSearch) || t.Type.ToString().ToLower().Contains(lowerSearch) || t.LogicalName.Contains(lowerSearch));
                            foreach(var column in correspondingColumns)
                            {
                                columnsToShow.Add(column);
                            }

                        }
                        columnBindingSource.DataSource = columnsToShow;
                    }

                }
                else
                {
                    throw new Exception($"Current table {currentTable} is not contained in the project");
                }




            }
            else
            {
                MessageBox.Show($"currentTable is null");
            }
        }

        private void RetrieveAttributesButton_Click(object sender, EventArgs e)
        {
            //RetrieveAttributesForTable(currentTable);
            var form2 = new AddTableForm();
            form2.PublisherPrefixes = this.PublisherPrefixes;
            form2.PluginControl = this;
            form2.RetrieveEntities();            
            form2.ShowDialog();
        }

        private void RetrieveAttributesForTable(Table table)
        {
            
            //if()

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving Entity Metadata",

                Work = (worker, args) =>
                {

                    //query.ColumnSet.AllColumns = true;
                    //var linkPublisher = query.AddLink(Deploy.Publisher.EntityLogicalName, "publisherid", "publisherid");
                    //linkPublisher.EntityAlias = "publisher";
                    //linkPublisher.Columns.AddColumn("customizationprefix");
                    //
                    //var solutions = Service.RetrieveMultiple(query).Entities.Select(s => s.ToEntity<Solution>());
                    //args.Result = solutions.ToArray();

                    RetrieveEntityRequest req = new RetrieveEntityRequest()
                    {
                        EntityFilters = EntityFilters.All,
                        LogicalName = table.LogicalName,
                        //RetrieveAsIfPublished = true
                    };

                    var result = (RetrieveEntityResponse)Service.Execute(req);
                    args.Result = result;

                },

                PostWorkCallBack = (args) =>
                {
                    var entity = ((RetrieveEntityResponse)args.Result).EntityMetadata;

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



                            table.Keys.Add(newKey);
                        }
                    }

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
                                table.Enums.Add(newEnum);
                            }
                            else if (globalEnumsTable.Enums.All(e => e.LogicalName != newEnum.LogicalName))
                            {
                                globalEnumsTable.Enums.Add(newEnum);
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
                            EnumName = attributeEnumName
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


                    //columnBindingSource.DataSource = selectedTable.Columns;
                }
            });

            RefreshGlobalEnum();

        }

        public void AddTablesToProject(List<Table> tables)
        {
            foreach(Table table in tables)
            {
                if(TableAndPath.ContainsKey(table.LogicalName))
                {
                    continue;
                }
                //Get table attributes
                RetrieveAttributesForTable(table);

                TableAndPath[table.LogicalName] = new TableData(table, RemoveCurrentProjectPathFromTablePath(pathToRegisterTables + $"{table.Name}.table"));
            }
        }

        private void SaveTablesButton_Click(object sender, EventArgs e)
        {
            if(!Directory.Exists(pathToRegisterTables))
            {
                Directory.CreateDirectory(pathToRegisterTables);
            }
            foreach(var key in TableAndPath.Keys)
            {
                var path = TableAndPath[key].path;
                var table = TableAndPath[key].table;
                var splitPath = path.Split('\\').ToList();
                //var rootPath = mySettings.RootFolders.FirstOrDefault(r => r.OrganizationName == mySettings.CurrentOrganizationName).FolderPath;
                //var splitRootPath = rootPath.Split('\\').ToList();
                splitPath.Remove(splitPath.ElementAt(splitPath.Count - 1));
                splitPath.Remove(splitPath.ElementAt(0));
                splitPath.Remove(splitPath.ElementAt(0));



                var project = mySettings.RootFolders.FirstOrDefault(r => r.OrganizationName == mySettings.CurrentOrganizationName);
                if(project == null)
                {
                    MessageBox.Show("Could not save tables");
                    return;
                }
                var projectPath = project.FolderPath;
                var registrationPath = String.Join("\\", splitPath);

                var txt = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
                var finalPath = projectPath + '\\' + registrationPath + '\\' + $"{table.Name}.table";
                CheckForDuplicateTableFile(finalPath,table);
                var fileInfo = new FileInfo(projectPath +'\\'+ registrationPath +'\\'+ $"{table.Name}.table");
                File.WriteAllText(fileInfo.FullName, txt);

                
                
            }

            // Register global enums

            var enumsPath = pathToRegisterTables + $"{globalEnumsTable.Name}.table";

            var splitEnumPath = enumsPath.Split('\\').ToList();
            //var rootPath = mySettings.RootFolders.FirstOrDefault(r => r.OrganizationName == mySettings.CurrentOrganizationName).FolderPath;
            //var splitRootPath = rootPath.Split('\\').ToList();
            splitEnumPath.Remove(splitEnumPath.ElementAt(splitEnumPath.Count - 1));
            splitEnumPath.Remove(splitEnumPath.ElementAt(0));
            splitEnumPath.Remove(splitEnumPath.ElementAt(0));



            var projectE = mySettings.RootFolders.FirstOrDefault(r => r.OrganizationName == mySettings.CurrentOrganizationName);
            if (projectE == null)
            {
                MessageBox.Show("Could not save enums");
                return;
            }
            var projectPathE = projectE.FolderPath;
            var registrationPathE = String.Join("\\", splitEnumPath);

            var txtE = JsonConvert.SerializeObject(globalEnumsTable, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            var fileInfoE = new FileInfo(enumsPath);
            File.WriteAllText(fileInfoE.FullName, txtE);
        }

        private void CheckForDuplicateTableFile(string finalPath,Table table)
        {
            // Check for exisiting file
            if(File.Exists(finalPath))
            {
                return;
            }

            // Iterate through each preexisting tableFile 
            foreach(var path in PreexistingTablePaths)
            {
                var fileInfo = new FileInfo(path);
                var text = File.ReadAllText(fileInfo.FullName);
                var deserializedTable = JsonConvert.DeserializeObject<Table>(text);
                if(deserializedTable.LogicalName == table.LogicalName)
                {
                    //Delete first file
                    File.Delete(path);
                   
                }
            }
        }

        private void RefreshGlobalEnum()
        {
            optionSetEnumBindingSource.DataSource = globalEnumsTable.Enums;
        }
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void TableLogicalNameText_TextChanged(object sender, EventArgs e)
        {

        }

        public string RemoveCurrentProjectPathFromTablePath(string path)
        {
            var splitPath = path.Split('\\');
            var rootPath = CurrentProject.FolderPath;
            var splitRootPath = rootPath.Split('\\').ToList();
            splitRootPath.Remove(splitRootPath.ElementAt(splitRootPath.Count - 1));
            rootPath = String.Join("\\", splitRootPath);
            return path.Replace(rootPath.Trim('\\'), "");

        }

        private void dataGridView3_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void splitContainer3_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer5_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer6_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void RefreshAttributesButton_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(currentTable))
            {
                return;
            }
            var selectedTable = TableAndPath[currentTable].table;
            if(selectedTable == null)
            {
                return;
            }

            // Call the function to refresh attributes
            RefreshTableAttributes(selectedTable);
            


        }

        private void RefreshTableAttributes(Table table)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving Entity Metadata",

                Work = (worker, args) =>
                {

                    //query.ColumnSet.AllColumns = true;
                    //var linkPublisher = query.AddLink(Deploy.Publisher.EntityLogicalName, "publisherid", "publisherid");
                    //linkPublisher.EntityAlias = "publisher";
                    //linkPublisher.Columns.AddColumn("customizationprefix");
                    //
                    //var solutions = Service.RetrieveMultiple(query).Entities.Select(s => s.ToEntity<Solution>());
                    //args.Result = solutions.ToArray();

                    RetrieveEntityRequest req = new RetrieveEntityRequest()
                    {
                        EntityFilters = EntityFilters.All,
                        LogicalName = table.LogicalName,
                        //RetrieveAsIfPublished = true
                    };

                    var result = (RetrieveEntityResponse)Service.Execute(req);
                    args.Result = result;

                },

                PostWorkCallBack = (args) =>
                {
                    var entity = ((RetrieveEntityResponse)args.Result).EntityMetadata;

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



                            //table.Keys.Add(newKey);
                        }
                    }

                    if (entity.OneToManyRelationships.Any())
                    {

                        foreach (var relationship in entity.OneToManyRelationships)
                        {


                            //table.OneToManyRelationships.Add(new Relation
                            //{
                            //    Name = relationship.SchemaName,
                            //    Role = EntityRole.Referenced,
                            //    EntityName = relationship.ReferencingEntity,
                            //    NavigationPropertyName = relationship.ReferencedEntityNavigationPropertyName,
                            //    LookupFieldName = relationship.ReferencingAttribute
                            //});
                        }
                    }

                    if (entity.ManyToManyRelationships.Any())
                    {

                        foreach (var relationship in entity.ManyToManyRelationships)
                        {

                            //table.ManyToManyRelationships.Add(new Relation
                            //{
                            //    Name = relationship.SchemaName,
                            //    Role = EntityRole.Referencing,
                            //    EntityName = relationship.Entity1LogicalName == table.LogicalName ? relationship.Entity2LogicalName : relationship.Entity1LogicalName,
                            //    NavigationPropertyName = relationship.IntersectEntityName,
                            //    LookupFieldName = relationship.Entity1LogicalName == table.LogicalName ? relationship.Entity2IntersectAttribute : relationship.Entity1IntersectAttribute
                            //});
                        }
                    }

                    if (entity.ManyToOneRelationships.Any())
                    {
                        //foreach (var relationship in entity.ManyToOneRelationships)
                        //{
                        //    table.ManyToOneRelationships.Add(new Relation
                        //    {
                        //        Name = relationship.SchemaName,
                        //        Role = EntityRole.Referencing,
                        //        NavigationPropertyName = relationship.ReferencingEntityNavigationPropertyName,
                        //        EntityName = relationship.ReferencedEntity,
                        //        LookupFieldName = relationship.ReferencingAttribute
                        //    });
                        //}
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
                                //table.Enums.Add(newEnum);
                            }
                            else if (globalEnumsTable.Enums.All(e => e.LogicalName != newEnum.LogicalName))
                            {
                                //globalEnumsTable.Enums.Add(newEnum);
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

                    columnBindingSource.DataSource = table.Columns.ToList();
                    MessageBox.Show(table.Name);

                }
            });

            //RefreshGlobalEnum();

        }

        public void TryEnumName(string enumName)
        {
            Table currentTable;
            //First check tables
            foreach(var key in TableAndPath.Keys)
            {
                currentTable = TableAndPath[key].table;
                if(currentTable.Enums.Any(e=>e.Name == enumName))
                {
                    MessageBox.Show($"The name {enumName} is already being used in table {currentTable.LogicalName} for enum {currentTable.Enums.FirstOrDefault(e => e.Name == enumName).LogicalName}");
                    // Open new name prompt (can be used for table and enums) at the point of creation of a table
                    //var otherNameForm = new
                    // Try the newly chosen name again
                    return;
                }

            }
            var globalEnum = globalEnumsTable.Enums.FirstOrDefault(e => e.Name == enumName);
            if (globalEnum!=null)
            {
                MessageBox.Show($"The name {enumName} is already being used in global enumerations for enum {globalEnum.LogicalName}");
                // Open new name prompt (can be used for table and enums) at the point of creation of a table

                // Try the newly chosen name again
                return;
            }
        }

        public void TryTableName(string tableName)
        {
            Table currentTable;
            //First check tables
            foreach (var key in TableAndPath.Keys)
            {
                currentTable = TableAndPath[key].table;
                if (currentTable.Name == tableName)
                {
                    MessageBox.Show($"The name {tableName} is already being used in table {currentTable.LogicalName}");
                    // Open new name prompt (can be used for table and enums) at the point of creation of a table

                    // Try the newly chosen name again
                    return;
                }

            }
            
        }
    }

    public class TableData
    {
        public Table table;
        public String path;
        public TableData(Table table,string path)
        {
            this.table = table;
            this.path = path;   
        }
    }

    public static class DateTimeBehaviorExtensions
    {
        public static XrmFramework.DateTimeBehavior ToDateTimeBehavior(this Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior behav)
        {
            if (behav == Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior.DateOnly)
            {
                return XrmFramework.DateTimeBehavior.DateOnly;
            }

            if (behav == Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior.TimeZoneIndependent)
            {
                return XrmFramework.DateTimeBehavior.TimeZoneIndependent;
            }

            return XrmFramework.DateTimeBehavior.UserLocal;
        }
    }
}