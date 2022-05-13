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
using XrmFramework.XrmToolbox.DataHandlers;
using XrmToolBox.Extensibility;

namespace XrmFramework.XrmToolbox
{
    public partial class XrmFrameworkPluginControl : PluginControlBase
    {
        //private string CurrentTable = null;
        //private string TableHandler.PathToRegisterTables = "";
        public Settings Settings;
        //private DataTable TableContent = new DataTable();
        //private TableCollection ProjectTables = new TableCollection();
        //private Dictionary<string, TableData> TableHandler.TableAndPath = new Dictionary<string, TableData>();
        //private Table globalEnumsTable = null;
        private List<string> PublisherPrefixes { get; } = new();
        //private TableCollection BasicTables = new TableCollection();
        public Project CurrentProject;
        //private List<string> PreexistingTablePaths = new List<String>();
        //private string CurrentEnum = null;



        public XrmFrameworkPluginControl()
        {
            InitializeComponent();
            this.TableNameText.Click += TableNameText_Click;
            this.EnumNameText.Click += EnumNameText_Click;


        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("This is a notification that can lead to XrmToolBox repository", new Uri("https://github.com/MscrmTools/XrmToolBox"));

            // Loads or creates the settings for the plugin
            if(Settings == null)

            {
                if (!SettingsManager.Instance.TryLoad(GetType(), out Settings))
                {
                    Settings = new Settings();

                    LogWarning("Settings not found => a new settings file has been created!");
                }
                else
                {
                    LogInfo("Settings found and loaded");
                }
            }
            Settings.LastUsedOrganizationWebappUrl = ConnectionDetail.WebApplicationUrl;
            LogInfo("Connection has changed to: {0}", ConnectionDetail.WebApplicationUrl);
            // Add organizationName
            Settings.CurrentOrganizationName = ConnectionDetail.OrganizationFriendlyName;
            SettingsManager.Instance.Save(GetType(), Settings);
            //var currentPair = mySettings.RootFolders.FirstOrDefault(p => p.OrganizationName == mySettings.CurrentOrganizationName);
            CurrentProject = null;
            if(Settings.RootFolders.Count != 0)
            {
                CurrentProject = Settings.RootFolders.ElementAt(Settings.RootFolders.Count - 1);
                
            }
            //CurrentProject = mySettings.RootFolders.FirstOrDefault(p => p.OrganizationName == mySettings.CurrentOrganizationName);
            
            if (CurrentProject != null)
            {
                if(Directory.Exists(CurrentProject.FolderPath))
                {
                    TableHandler.LoadTablesFromProject(CurrentProject.FolderPath);
                    RefreshTreeDisplay();
                    RefreshGlobalEnum();
                    CurrentProjectTextBox.Text = CurrentProject.FolderPath;

                }

                textBox1.Text = CurrentProject.FolderPath;
                TableHandler.PathToRegisterTables = CurrentProject.FolderPath + "\\Definitions\\";
                //MessageBox.Show($"TableHandler.PathToRegisterTables is {TableHandler.PathToRegisterTables}");

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

       

        

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), Settings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (Settings != null && detail != null)
            {
                Settings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
                // Add organizationName
                Settings.CurrentOrganizationName = detail.OrganizationFriendlyName;
                SettingsManager.Instance.Save(GetType(), Settings);
            }
            
        }

        private void toolStripMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        

        private void button1_Click(object sender, EventArgs e)
        {
           
            ProjectChoiceDialog.ShowDialog();
            //mySettings.RootFolders[]
            SetCurrentProject(ProjectChoiceDialog.SelectedPath);
            

            

        }

        public void SetCurrentProject(string path)
        {
            if (!Directory.Exists(path) || !ContainsSlnFile(path))
            {
                MessageBox.Show("Selected path does not correspond to a visual studio project, please try again");
                return;
            }
            TableHandler.TableAndPath.Clear();
            CurrentProjectTextBox.Text = path;
            var pairToRemove = Settings.RootFolders.FirstOrDefault(p => p.FolderPath == path);
            if (pairToRemove != null)
            {
                Settings.RootFolders.Remove(pairToRemove);

            }
            Settings.RootFolders.Add(new Project(Settings.CurrentOrganizationName, path));
            CurrentProject = Settings.RootFolders.FirstOrDefault(p => p.FolderPath == path);
            //var finalPair = mySettings.RootFolders.FirstOrDefault(p => p.OrganizationName == mySettings.CurrentOrganizationName);
            MessageBox.Show($"Project folder has been set to {CurrentProject.FolderPath}");

            MessageBox.Show($"Looking for folders in {CurrentProject.FolderPath}");
            SettingsManager.Instance.Save(GetType(), Settings);

            TableHandler.LoadTablesFromProject(CurrentProject.FolderPath);
            RefreshTreeDisplay();
            textBox1.Text = CurrentProject.FolderPath;
            TableHandler.PathToRegisterTables = CurrentProject.FolderPath + "\\Definitions\\";
            MessageBox.Show($"TableHandler.PathToRegisterTables is {TableHandler.PathToRegisterTables}");
 

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        

        private bool ContainsSlnFile(string projectPath)
        {
            return Directory.GetFiles(projectPath, "*.sln").Length > 0;
        }
        public void RefreshTreeDisplay()
        {
            TableHandler.CheckDefaultSelectColumns();
            TableTreeView.Nodes.Clear();
            AddTableTreeNodes();
            if(TableTreeView.Nodes.Count!=0)
            {
                TableTreeView.Nodes[0].ExpandAll();

            }
        }

        private void AddTableTreeNodes()
        {

            
            foreach (var key in TableHandler.TableAndPath.Keys)
            {
                
                var path = TableHandler.TableAndPath[key].path.Split('\\');
                AddPathTreeNode(null, path, 0,TableHandler.TableAndPath[key].table.LogicalName);
                
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
            
            if(!TableHandler.TableAndPath.ContainsKey(name))
            {
                return;
            }
            var selectedTable = TableHandler.TableAndPath[name].table;
            if(selectedTable != null)
            {
                //MessageBox.Show($"You selected a table file of name {selectedTable.Name}");
                tableBindingSource.DataSource = selectedTable;
                columnBindingSource.DataSource = selectedTable.Columns;
                TableHandler.CurrentTable = selectedTable.LogicalName;
                TableHandler.CurrentEnum = null;
                //if(optionSetEnumValueBindingSource.Count > 0)
                //{
                //    optionSetEnumValueBindingSource.RemoveCurrent();
                //
                //}
                //if(optionSetEnumBindingSource.Count >0)
                //{
                //    optionSetEnumBindingSource.RemoveCurrent();
                //
                //}

            }
            
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            //Clicked an enum thing
            if(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString() == "Picklist")
            {
                //var correspondingEnum
                var selectedTable = TableHandler.TableAndPath[TableHandler.CurrentTable].table;
                var column = selectedTable.Columns.FirstOrDefault(c=>c.LogicalName == dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                var correspondingEnum = selectedTable.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                if(correspondingEnum == null)
                {
                    correspondingEnum = TableHandler.globalEnumsTable.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                }
                //dataGridView3.DataSource = correspondingEnum.Values;
                optionSetEnumValueBindingSource.DataSource = correspondingEnum.Values;
                optionSetEnumBindingSource.DataSource = correspondingEnum;
                TableHandler.CurrentEnum = correspondingEnum.LogicalName;
                //EnumNameText.Text = correspondingEnum.Name;

            }

           
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }


        private void EnumNameText_Click(object sender,EventArgs e)
        {
            if (string.IsNullOrEmpty(EnumNameText.Text) || TableHandler.CurrentTable == null)
            {
                return;

            }
            //Start modifying
            var text = EnumNameText.Text;
            var currentTable = TableHandler.TableAndPath[TableHandler.CurrentTable].table;
            //Get the current table
            var currentEnum = TableHandler.TableAndPath[TableHandler.CurrentTable].table.Enums.FirstOrDefault(en=>en.LogicalName == TableHandler.CurrentEnum);
            if(currentEnum == null)
            {
                currentEnum = TableHandler.globalEnumsTable.Enums.FirstOrDefault(en => en.LogicalName == TableHandler.CurrentEnum);
                if(currentEnum == null)
                {
                    return;
                }
            }
            TableHandler.ModifyEnumName(EnumNameText.Text, currentEnum, text); ;

            EnumNameText.Text = currentEnum.Name;
        }

        

        

        private void RetrieveEntitiesButton_Click(object sender, EventArgs e)
        {
            if (PublisherPrefixes.Count == 0)
            {
                LoadPrefixes();
            }
            //RetrieveAllEntities();
        }

        public void RetrieveAllEntities(BindingSource bindingSource, TableCollection basicTables)
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

                PostWorkCallBack = (args) =>
                {
                    // Create the corresponding base tables while also processing the names
                    var entitiesData = (EntityMetadata[])args.Result;
                    TableHandler.ProcessBasicTableRequest(entitiesData);
                    bindingSource.DataSource = TableHandler.BasicTables;
                    //tableBindingSource1.DataSource = BasicTables;


                }

            });
            
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
            foreach(var table in TableHandler.BasicTables.Where(t=>t.Selected))
            {
                //MessageBox.Show($"Adding {table.Name}");
                //var txt = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings
                //{
                //    DefaultValueHandling = DefaultValueHandling.Ignore
                //});
                //var fileInfo = new FileInfo(TableHandler.PathToRegisterTables + $"{table.Name}.table");
                //File.WriteAllText(fileInfo.FullName, txt);
                
                //var projectData = mySettings.RootFolders.FirstOrDefault(p => p.OrganizationName == mySettings.CurrentOrganizationName);
                //if(projectData != null)
                //{
                //    LoadTablesFromProject(projectData.FolderPath);
                //    
                //}
                if(TableHandler.TableAndPath.ContainsKey(table.LogicalName))
                {
                    //MessageBox.Show("This is a message for when a table is already in a project, it should be replaced with a dialog box asking wether you want to overwrite it");
                    return;
                }
                TableHandler.TableAndPath[table.LogicalName] = new TableData(table, RemoveCurrentProjectPathFromTablePath(TableHandler.PathToRegisterTables+$"{table.Name}.table"));


                RefreshTreeDisplay();
                RefreshGlobalEnum();



                //Serialize
                //Saveit
                //Refresh tree 

            }
        }

       

        private void SearchColumnTextBox_TextChanged(object sender, EventArgs e)
        {
            if(TableHandler.CurrentTable != null)
            {
                if(TableHandler.TableAndPath.ContainsKey(TableHandler.CurrentTable))
                {
                    var search = SearchColumnTextBox.Text.Split(' ');
                    
                    if(search.Length == 1)
                    {
                        var lowerSearch = SearchColumnTextBox.Text.ToLower();

                        var selectedTable = TableHandler.TableAndPath[TableHandler.CurrentTable].table;
                        columnBindingSource.DataSource = selectedTable.Columns.Where(t => t.Name.ToLower().Contains(lowerSearch) || t.Type.ToString().ToLower().Contains(lowerSearch) || t.LogicalName.Contains(lowerSearch));

                    }
                    else
                    {
                        var selectedTable = TableHandler.TableAndPath[TableHandler.CurrentTable].table;

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
                    throw new Exception($"Current table {TableHandler.CurrentTable} is not contained in the project");
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


        public void AddTablesToProject(List<Table> tables)
        {
            foreach(Table table in tables)
            {
                if(TableHandler.TableAndPath.ContainsKey(table.LogicalName))
                {
                    continue;
                }
                while(TableHandler.IsTableNameUsed(table,table.Name))
                {
                    TableHandler.ModifyTableName(table.Name,table, table.Name);
                }
                
                //Get table attributes
                //RetrieveAttributesForTable(table);
                RefreshTableAttributes(table);
                TableHandler.TableAndPath[table.LogicalName] = new TableData(table, RemoveCurrentProjectPathFromTablePath(TableHandler.PathToRegisterTables + $"{table.Name}.table"));
            }
        }

        private void SaveTablesButton_Click(object sender, EventArgs e)
        {
            TableHandler.SaveTables();
        }

        

        private void RefreshGlobalEnum()
        {
            optionSetEnumBindingSource.DataSource = TableHandler.globalEnumsTable.Enums;
            
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
            if(e.ColumnIndex == 0)
            {
                //MessageBox.Show(e.ColumnIndex.ToString());
                //MessageBox.Show(dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString());
                var name = dataGridView3.Rows[e.RowIndex].Cells[0].Value;
                var currentEnum = TableHandler.TableAndPath[TableHandler.CurrentTable].table.Enums.FirstOrDefault(en => en.LogicalName == TableHandler.CurrentEnum);
                if(currentEnum == null)
                {
                    currentEnum = TableHandler.globalEnumsTable.Enums.FirstOrDefault(en => en.LogicalName == TableHandler.CurrentEnum);
                    if(currentEnum == null)
                    {
                        return;
                    }
                }
                //MessageBox.Show(value.ToString());
                var enumValue = currentEnum.Values.ElementAt(e.RowIndex);
                //MessageBox.Show(enumValue.Name);
                TableHandler.ModifyEnumeValueName(enumValue,currentEnum,enumValue.Name);
                dataGridView3.Rows[e.RowIndex].Cells[0].Value = enumValue.Name;
                //enumValue.Name = 
                //MessageBox.Show(currentEnum.LogicalName);
                
               
            }
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
            if(String.IsNullOrEmpty(TableHandler.CurrentTable))
            {
                return;
            }
            var selectedTable = TableHandler.TableAndPath[TableHandler.CurrentTable].table;
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

                    TableHandler.ProcessEntityResponse(table, entity);
                    // La seule partie a retirer pour utiliser une seule fonction à priori
                    columnBindingSource.DataSource = table.Columns.ToList();

                }
            });


        }




        
        private void TableNameText_TextChanged(object sender, EventArgs e)
        {
        }

        private void TableNameText_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(TableNameText.Text))
            {
                return;

            }
            //Start modifying
            var text = TableNameText.Text;
            //Get the current table
            var currentTable = TableHandler.TableAndPath[TableLogicalNameText.Text].table;
            TableHandler.ModifyTableName(TableNameText.Text,currentTable, text);
            TableNameText.Text = currentTable.Name;


        }

        

        
        private void NewProjectButton_Click(object sender, EventArgs e)
        {
            var newProjectForm = new ProjectCreationForm();
            newProjectForm.PluginControl = this;

            newProjectForm.ShowDialog();
            
        }

        public void ReloadProject()
        {

        }

        private void CurrentProjectTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void EnumNameText_TextChanged(object sender, EventArgs e)
        {

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

//private void LoadTablesFromProject(string projectPath)
//{
//    foreach (var fileName in Directory.GetFiles($"{projectPath}", "*.table", SearchOption.AllDirectories))
//    {
//        PreexistingTablePaths.Add(fileName);
//        if(fileName.Contains("OptionSet"))
//        {
//            //MessageBox.Show(fileName);
//            var fileInfo = new FileInfo(fileName);
//            var text = File.ReadAllText(fileInfo.FullName);
//            var table = JsonConvert.DeserializeObject<Table>(text);
//            globalEnumsTable = table;
//        }
//        else
//        {
//            //MessageBox.Show(fileName);
//            var fileInfo = new FileInfo(fileName);
//            var text = File.ReadAllText(fileInfo.FullName);
//            var table = JsonConvert.DeserializeObject<Table>(text);
//
//            var splitFilename = fileInfo.Name.Split('\\');
//            var rootPath = Settings.RootFolders.FirstOrDefault(r => r.OrganizationName == Settings.CurrentOrganizationName).FolderPath;
//            var splitRootPath = rootPath.Split('\\').ToList();
//            splitRootPath.Remove(splitRootPath.ElementAt(splitRootPath.Count - 1));
//            rootPath = String.Join("\\", splitRootPath);
//
//            TableHandler.TableAndPath[table.LogicalName] = new TableData(table, RemoveCurrentProjectPathFromTablePath(fileName));
//        }
//        
//    }
//
//    if(globalEnumsTable == null)
//    {
//        globalEnumsTable = new Table()
//        {
//            LogicalName = "globalEnums",
//            Name = "OptionSet"
//        };
//    }
//
//    
//}

/*private void GetAccounts()
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
        }*/