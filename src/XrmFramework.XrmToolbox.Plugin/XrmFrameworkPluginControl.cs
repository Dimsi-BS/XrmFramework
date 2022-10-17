using Deploy;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XrmFramework.Core;
using XrmFramework.XrmToolbox.DataHandlers;
using XrmFramework.XrmToolbox.Forms;
using XrmToolBox.Extensibility;

namespace XrmFramework.XrmToolbox
{
    public partial class XrmFrameworkPluginControl : PluginControlBase
    {

        public Settings Settings;

        //private List<string> PublisherPrefixes { get; } = new();
        public Project CurrentProject;
        string RedCircleIcon = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAA7EAAAOxAGVKw4bAAAABmJLR0QA/wD/AP+gvaeTAAAAJXRFWHRkYXRlOmNyZWF0ZQAyMDIxLTA0LTI3VDE4OjMxOjMxKzAwOjAwQs1UEQAAACV0RVh0ZGF0ZTptb2RpZnkAMjAyMS0wNC0yN1QxODozMTozMSswMDowMDOQ7K0AAAGnSURBVDhPnZO/S0JRFMe/9z6fLy1JCavBCIpoCFxaAzcF/4tC2hqShpZAKJAgiIZaXPoHWoOC2oKWlghqaXCwMCVI/NnzeTvn+gitIe0zXHjn3O95997zPUIR6MF5K6N6dIjW7RWclzyElDBm5mGtxBFIb0H4R9ydXfoKvK+ton52Cs+cH/BaEIZHx5XThmo24Tw3ENjYxng2q+PMd4HichSd+iNkKEQKneKlBwpSyCmWYC7GET6/0FHJC/9Zi4MsZuFPMUMxyhlTYdhPl6js7nWj7WJJvS6EYS6FXfEACAX7voRIVcFIe81Mp3wH4bHc7GAIX5vONAFj0/zMKOcDQhhuahDopLRd1SSkbpX72kMhPbrNVEK/4/+QguqQSbjPQ0Ma1kp2GMgkus8Do+j+DVixBESn1lCFSR/M6DBt7MB+KCNSUXQL8jbbkx3G/f0T2tPOlxE8yLmfrpVLyYR2mDHNJ9EpXnpgK3fF/mQKodyPAgzbs7K/A2OWBslPxqJWaXiY6M5OwdZ/HltPdePEr3FmqscnaN1c6z5zq/i1rVgcozQz/QBfkF6s4ueHnhgAAAAASUVORK5CYII=";
        string PurpleCircleIcon = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABHNCSVQICAgIfAhkiAAAAAFzUkdCAK7OHOkAAAAEZ0FNQQAAsY8L/GEFAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAAa1JREFUOE+dU88rBGEYfmZmJTM7y+JEUluyCrdN5KDc3CQcXMQ/oLiQXCQXyj9ALk4b5eJAykEhR1rZsiXLCbO7Y7aY+WZ837uNX0uWp57p/Z7vfZ+++b73lTwOfIPUyTXMW4tivV5DpKOR4q8oMtiZO0Bq6w4hLQRFkUljzEXOyiHSX4e++R7SfLwZWGYea72bqAyEEVAVsQPfWRIfT4KTZ8g6Bsb2B6DpamHPN1iJraNarQHKSP8ZNvCYf8DE6Sgt6Yzbs7sIKjqY4oC5v5DniNzt2b13g0Q8BakCcD1WEkVuIn5VMEgeXUGrUME87s43S6NDNcmjFORMOgdPdkn8C0VNJp2FLO7a8dx/UdTKlfU6bPsZLr+gv1DUiFo52tUEwzT4DRf+rSTyXOPJgKilV2gbjsJ6eoHteiVR5LYPRekV3hppqmURVWoYUoD0H8EPgEzewNLFNK3fWzlnYSa2Ao03SbnG25H69wN41rNlw2ImFk4nEAxpJBcN0+pkHMcb59A1nQ8TnwkOxhgfJhOdI60YXx4kzUeRgY+zw0vc3xgU1zaE0dbdTPFnAK+Za64FTBl3nwAAAABJRU5ErkJggg==";
        string GreenCircleIcon = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAkDAfcUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAAZiS0dEAP8A/wD/oL2nkwAAAFF0RVh0Y29tbWVudABGaWxlIHNvdXJjZTogaHR0cHM6Ly9jb21tb25zLndpa2ltZWRpYS5vcmcvd2lraS9GaWxlOlBhbl9HcmVlbl9DaXJjbGUucG5nYd7qngAAACV0RVh0ZGF0ZTpjcmVhdGUAMjAxNS0wNy0xN1QxNzoyMzo0NSswMDowMITE1MMAAAAldEVYdGRhdGU6bW9kaWZ5ADIwMTUtMDctMTdUMTc6MjM6NDUrMDA6MDD1mWx/AAAAR3RFWHRzb2Z0d2FyZQBJbWFnZU1hZ2ljayA2LjcuNy0xMCAyMDE0LTAzLTA2IFExNiBodHRwOi8vd3d3LmltYWdlbWFnaWNrLm9yZ2+foqIAAAAYdEVYdFRodW1iOjpEb2N1bWVudDo6UGFnZXMAMaf/uy8AAAAYdEVYdFRodW1iOjpJbWFnZTo6aGVpZ2h0ADY3N+SmvQEAAAAXdEVYdFRodW1iOjpJbWFnZTo6V2lkdGgANjc3OArujAAAABl0RVh0VGh1bWI6Ok1pbWV0eXBlAGltYWdlL3BuZz+yVk4AAAAXdEVYdFRodW1iOjpNVGltZQAxNDM3MTUzODI1OpdaBgAAABN0RVh0VGh1bWI6OlNpemUAOS4wNktCQmLlkQoAAAAzdEVYdFRodW1iOjpVUkkAZmlsZTovLy90bXAvbG9jYWxjb3B5X2I0MzQ5ZDA5YTUxYS0xLnBuZ5Ikju4AAAFuSURBVDhPpZPPSwJREMe/b10vUQf9F3Sjqxe9iIcu5Q/6IyLopMGyB0EEwYNHw2vQX6FFXYTSgx70FlR6CDwbRIYH223mPXUTktb6LMPumx/vzc7MEw6Bb9iOjfJdGY2nBgbjARx6wsEwUkYKxUQRuqbPPRUrG5g3JqpXVWCLFuynSTXtSjIj+QCyh1nUkjWpZpYbxC5i6D52gW3WsuYH2HMCRMIR9E57UiXPsG4tFbxDi3XBDNvogP6gj9x1Tqk4A3FGlsDcwQucySv9VXUGn76vl5ovTcCvbJ7gg0iEX8A3jU9Lo7eRW7ANmHxOoHGr/hLMMcPxkF5e/3sNmhE0VJ83hWJ4wLS0kVZDsikUk9nN/K+N9rmtypc/ygPv/OUR8jXTJoQQ7ijHL+NoP7R/H2UKju5F0TnpSNWyga3jFvJJyoRSw5SE68LFXVwk1pHNOrCWwczKbVxQua+g/lyX15kJBULgYhcSBbl2Ab4AXi2Hijz2MQwAAAAASUVORK5CYII=";
        string BlueCircleIcon = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAA7EAAAOxAGVKw4bAAAABmJLR0QA/wD/AP+gvaeTAAAAB3RJTUUH4gYXAikl2RAVbQAAACV0RVh0ZGF0ZTpjcmVhdGUAMjAxOC0wNi0yM1QwMjo0MTozNyswMDowMNd9wnMAAAAldEVYdGRhdGU6bW9kaWZ5ADIwMTgtMDYtMjNUMDI6NDE6MzcrMDA6MDCmIHrPAAABdUlEQVQ4T6WTvUoDQRSFz0x+NkGDJlYSQS2EFGJlZ6NvYCOksRDFUh8lhZXiGwg+gilsLYydlRbRQsWQGFk2JjPeM5usCf6QxQ92WWbumb1z77nKChii1bGo1AJUH7q4b1lwcz6nsFFM4nDFQyGjwsA+IwccXPo4qnWQzyp4CSDZj+1KRNADGr7F/nIax+vZcEOIDlg7b+P6pYeCp6DU6F8GMLQRAKW8xtXWpFvTfPHPFM9k9K9iwj1e4bZhsFf1w7VmYOzUSQtzcs+/xMMwk/qbxdNuDpoF453HFROXiWgqUi/NarNgcaHmQrT6Tlo1qHYcEpJFvW3CIv4HTZOwz3HpSSGLExqaDqNJ4kINterVN7ZwGreNkDYaPO5IG/NiDNqTDhuXpszLdimFWblCZOXVs7Zz2LQX9vknGNrsAAuS7U0559aiLtDb5aWUc9j7h0XXyCSKgA+/uca9zcVkJCbfxvnZN85hNAn7TFjtcJzTLu0vgE9UHa2mf3ARigAAAABJRU5ErkJggg==";
        string BlankIcon = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAAmJLR0QAAKqNIzIAAAAHdElNRQfkBgkQMjvzFFc7AAAAJXRFWHRkYXRlOmNyZWF0ZQAyMDIwLTA2LTA5VDE2OjUwOjU5KzAwOjAw3f66YQAAACV0RVh0ZGF0ZTptb2RpZnkAMjAyMC0wNi0wOVQxNjo1MDo1OSswMDowMKyjAt0AAAAgSURBVDhPY/wPBAwUACYoTTYYNWDUABAYNWDgDWBgAABrygQclUTopgAAAABJRU5ErkJggg==";
        public XrmFrameworkPluginControl()
        {
            InitializeComponent();
            ChoicePickList.Leave += ChoicePickList_Leave;
            ChoicePickList.LostFocus += ChoicePickList_LostFocus;
            this.TableNameText.Click += TableNameText_Click;
            this.EnumNameText.Click += EnumNameText_Click;
            //string ims = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAMAAABEpIrGAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAF6UExURfv7+/r7+73o+GjO9FrJ82DM86Lg9vn6+8Dp+Aqy8ACt7QOv74/a9m7Q9TzB8/D3+mPN9AOu7XfT9Y3b93nU9lvK9DO98RGy7gGt7TO/8+33+giw7uD1/f////P7/tHx/H7W9iK48N/1/en4/nHR9Quw7vv+/6jj+azk+Q2x7gKu7XfU9ZHc+K/l+f3+//z+/3zV9gCs7Q6x7t71/e76/im68CC37ye57xe07zG98eL2/Y3a9wev7tr0/KTi+VXI8/j9/9jz/BWz7g+x7snu+/X8/jW98YbY9lTI82vO9GbO9WrO9GbN9YPX9lLH8wyx7sLs+03G8vb8/tXx/BSz7iy78IfZ9wKt7V7L9Nnz/Oz5/ia58Ira94va947b963l+dvz/XfU9v7///r+/6Hh+eb3/WvP9Qqw7vT8/tDw/B6273TT9Yna93rT9V3L9BKy7jfA8+73+qvi9wSv7wGu7nbT9Jfd90LD8zjC9DzC9PL4+/H4+u/3+vD4+vS4s1gAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAFLSURBVDhPfZNnU8JAFEU3oqJro2iIJaioQbCBvaLYO/YK9t5797+7TbMZ8zhfsnfumexm8hYRFBDaIpThyATIynaSPicXg+Tlk5cUiGBLYRFCLvJ0e7y/FJeoPo23hNIyLpRX6Cb+yqpqXktCQJSCmtq69IJuBH1WQd6CUx+ShXCDOKK3sSnQzISWiCyYRHGrq40Z7R0k/gmdXYRuusJY6+llRp/bFPoHDEKMCxgPDlEhPiwdkn1FkNcYj4zSODYOCtoEjfokKOApJkzDwgwTZuEt5pgwDwqJBRr9KigsLtG4vAII2ipL+tr6PyFK+9DGJuu3tkmwCvFkKrWzu8d/lrFPbasgcxBLpBUOj9g4gMLxySnrTUGeqLPzyAWvTSF8mRRcXd+ot9axd4i1LXf3CD08imDD0zO5nMrL6xvAu+eD3m7l8wvg24nQD18JcKWCg6C0AAAAAElFTkSuQmCC";
            MemoryStream msR = new MemoryStream(Convert.FromBase64String(RedCircleIcon));
            MemoryStream msP = new MemoryStream(Convert.FromBase64String(PurpleCircleIcon));
            MemoryStream msG = new MemoryStream(Convert.FromBase64String(GreenCircleIcon));
            MemoryStream msB = new MemoryStream(Convert.FromBase64String(BlueCircleIcon));
            MemoryStream msBlank = new MemoryStream(Convert.FromBase64String(BlankIcon));
            imageList1.Images.Add(Image.FromStream(msBlank));
            imageList1.Images.Add(Image.FromStream(msR));
            imageList1.Images.Add(Image.FromStream(msP));
            imageList1.Images.Add(Image.FromStream(msG));
            imageList1.Images.Add(Image.FromStream(msB));
            ModelsTreeView.ImageList = imageList1;
            TableTreeView.ImageList = imageList1;
            ModelDetailTree.ImageList = imageList1;



        }

        private void ChoicePickList_LostFocus(object sender, EventArgs e)
        {
            ChoicePickList.Enabled = false;
            ChoicePickList.Visible = false;
            //MessageBox.Show("Lost da focus ?????");
        }

        private void ChoicePickList_Leave(object sender, EventArgs e)
        {
            //ChoicePickList.Visible = false;
            //ChoicePickList.Enabled = false;
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("This is a notification that can lead to XrmToolBox repository", new Uri("https://github.com/MscrmTools/XrmToolBox"));

            // Loads or creates the settings for the plugin
            if (Settings == null)

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
            if (Settings.RootFolders.Count != 0)
            {
                CurrentProject = Settings.RootFolders.ElementAt(Settings.RootFolders.Count - 1);

            }
            //CurrentProject = mySettings.RootFolders.FirstOrDefault(p => p.OrganizationName == mySettings.CurrentOrganizationName);

            if (CurrentProject != null)
            {
                if (Directory.Exists(CurrentProject.FolderPath))
                {
                    TableHandler.LoadTablesFromProject(CurrentProject.FolderPath);
                    RefreshTreeDisplay();
                    RefreshGlobalEnum();

                    ModelHandler.LoadModelsFromProject(CurrentProject.FolderPath);
                    RefreshModelsDisplay();
                    CurrentProjectTextBox.Text = CurrentProject.FolderPath;

                }

                textBox1.Text = CurrentProject.FolderPath;
                TableHandler.PathToRegisterTables = CurrentProject.FolderPath + "\\Definitions\\";
                ModelHandler.PathToRegisterModel = CurrentProject.FolderPath + "\\Models\\";

            }

            if (TableHandler.PublisherPrefixes.Count == 0)
            {
                LoadPrefixes();
            }

            TableHandler.PluginControl = this;
            ModelHandler.PluginControl = this;
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
            LoadPrefixes();
            //TableHandler.loas
            if (!Directory.Exists(path) || !ContainsSlnFile(path))
            {
                MessageBox.Show("Selected path does not correspond to a visual studio project, please try again");
                return;
            }
            TableHandler.TableAndPath.Clear();
            ModelHandler.ModelAndPath.Clear();
            CurrentProjectTextBox.Text = path;
            var pairToRemove = Settings.RootFolders.FirstOrDefault(p => p.FolderPath == path);
            if (pairToRemove != null)
            {
                Settings.RootFolders.Remove(pairToRemove);

            }
            Settings.RootFolders.Add(new Project(Settings.CurrentOrganizationName, path));
            CurrentProject = Settings.RootFolders.FirstOrDefault(p => p.FolderPath == path);
            //var finalPair = mySettings.RootFolders.FirstOrDefault(p => p.OrganizationName == mySettings.CurrentOrganizationName);
            //MessageBox.Show($"Project folder has been set to {CurrentProject.FolderPath}");

            //MessageBox.Show($"Looking for folders in {CurrentProject.FolderPath}");
            SettingsManager.Instance.Save(GetType(), Settings);
            ModelHandler.LoadModelsFromProject(CurrentProject.FolderPath);
            TableHandler.LoadTablesFromProject(CurrentProject.FolderPath);
            RefreshTreeDisplay();
            RefreshModelsDisplay();
            //RefreshModelDetails();
            textBox1.Text = CurrentProject.FolderPath;
            TableHandler.PathToRegisterTables = CurrentProject.FolderPath + "\\Definitions\\";
            ModelHandler.PathToRegisterModel = CurrentProject.FolderPath + "\\Models\\";
            //MessageBox.Show($"TableHandler.PathToRegisterTables is {TableHandler.PathToRegisterTables}");


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
            TableTreeView.ImageList = imageList1;
            AddTableTreeNodes();
            if (TableTreeView.Nodes.Count != 0)
            {
                TableTreeView.Nodes[0].ExpandAll();

            }
        }



        public void AddTreeNode(TreeView tree, TreeNode? currentNode, string[] path, int index, string name, string identifier)
        {
            if (index < path.Length)
            {

                if (path[index] == "")
                {
                    AddTreeNode(tree, currentNode, path, index + 1, name, identifier);
                    return;
                }
                var newNode = new TreeNode(path[index]);
                newNode.Name = path[index];
                if (currentNode != null)
                {
                    // We are inside the tree
                    var sameNodes = currentNode.Nodes.Find(newNode.Name, false);
                    if (sameNodes.Count() > 0)
                    {
                        AddTreeNode(tree, sameNodes[0], path, index + 1, name, identifier);
                        return;

                    }

                    currentNode.Nodes.Add(newNode);
                    var nextCurrentNode = currentNode.Nodes.Find(newNode.Name, false)[0];
                    AddTreeNode(tree, nextCurrentNode, path, index + 1, name, identifier);
                    return;
                }
                else
                {
                    // We have to create the root of the tree
                    var sameNodes = tree.Nodes.Find(newNode.Name, false);
                    if (sameNodes.Count() > 0)
                    {
                        AddTreeNode(tree, sameNodes[0], path, index + 1, name, identifier);

                    }
                    else
                    {
                        tree.Nodes.Add(newNode);
                        var nextCurrentNode = tree.Nodes.Find(newNode.Name, false)[0];
                        AddTreeNode(tree, nextCurrentNode, path, index + 1, name, identifier);

                    }
                    return;
                }

            }

            if (index == path.Length)
            {
                // we are at the end of the branch and have to add the actual element
                if (currentNode == null)
                {
                    throw new Exception("Error, tried to add an element while no project root was added");

                }
                var newNode = new TreeNode(name);
                newNode.Name = identifier;
                newNode.ImageIndex = 2;
                newNode.SelectedImageIndex = 4;
                currentNode.Nodes.Add(newNode);
                return;

            }
            return;
        }
        private void AddTableTreeNodes()
        {


            foreach (var key in TableHandler.TableAndPath.Keys)
            {
                var table = TableHandler.TableAndPath[key].table;
                var path = TableHandler.TableAndPath[key].path.Split('\\');
                //AddPathTreeNode(null, path, 0,TableHandler.TableAndPath[key].table.LogicalName);
                AddTreeNode(TableTreeView, null, path, 0, table.Name, table.LogicalName);

            }
        }

        private void AddModelTreeNodes()
        {
            ModelsTreeView.Nodes.Clear();
            foreach (var key in ModelHandler.ModelAndPath.Keys)
            {

                var model = ModelHandler.ModelAndPath[key].model;
                var path = ModelHandler.ModelAndPath[key].path.Split('\\');
                //MessageBox.Show(ModelHandler.ModelAndPath[key].path);
                AddTreeNode(ModelsTreeView, null, path, 0, model.Name, model.Name);

            }
            ModelsTreeView.ExpandAll();
        }



        private void AddPathTreeNode(TreeNode? currentNode, string[] path, int index, string tableLogicalName)
        {

            if (index >= path.Length)
            {
                if (index > path.Length)
                {
                    return;

                }
            }
            else if (path[index] == "")
            {
                AddPathTreeNode(currentNode, path, index + 1, tableLogicalName);
                return;
            }

            TreeNode newNode = null;
            if (index < path.Length)
            {
                newNode = new TreeNode(path[index]);
                newNode.Name = path[index];
            }
            else
            {
                var table = TableHandler.TableAndPath[tableLogicalName].table;
                newNode = new TreeNode(table.Name + ".table");
                //newNode.Name = path[index];
            }


            if (currentNode == null)
            {
                if (TableTreeView.Nodes.Count > 0)
                {
                    AddPathTreeNode(TableTreeView.Nodes[0], path, index + 1, tableLogicalName);
                }
                else
                {
                    if (path[index].Contains(".table"))
                    {
                        newNode.Name = tableLogicalName;
                    }
                    TableTreeView.Nodes.Add(newNode);
                    AddPathTreeNode(newNode, path, index + 1, tableLogicalName);

                }


                return;

            }
            var existingNode = currentNode.Nodes.Find(newNode.Name, false);
            if (existingNode.Length == 0)
            {
                if (newNode.Text.Contains(".table"))
                {
                    newNode.Name = tableLogicalName;
                }
                currentNode.Nodes.Add(newNode);
                AddPathTreeNode(newNode, path, index + 1, tableLogicalName);

            }
            else
            {
                AddPathTreeNode(existingNode[0], path, index + 1, tableLogicalName);
            }

        }

        private void TableTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //MessageBox.Show(e.Node.Name);
            var name = e.Node.Name;

            if (!TableHandler.TableAndPath.ContainsKey(name))
            {
                return;
            }
            var selectedTable = TableHandler.TableAndPath[name].table;
            if (selectedTable != null)
            {
                //MessageBox.Show($"You selected a table file of name {selectedTable.Name}");
                tableBindingSource.DataSource = selectedTable;
                columnBindingSource.DataSource = selectedTable.Columns;
                TableHandler.CurrentTable = selectedTable.LogicalName;
                TableHandler.CurrentEnum = null;


            }

        }


        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            //Clicked an enum thing
            if (dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString() == "Picklist")
            {
                //var correspondingEnum
                if (TableHandler.CurrentTable == null)
                {
                    return;
                }
                var selectedTable = TableHandler.TableAndPath[TableHandler.CurrentTable].table;
                var column = selectedTable.Columns.FirstOrDefault(c => c.LogicalName == dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                var correspondingEnum = selectedTable.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                if (correspondingEnum == null)
                {
                    correspondingEnum = TableHandler.globalEnumsTable.Enums.FirstOrDefault(e => e.LogicalName == column.EnumName);
                }
                if (correspondingEnum == null)
                {
                    return;
                }
                //dataGridView3.DataSource = correspondingEnum.Values;
                optionSetEnumValueBindingSource.DataSource = correspondingEnum.Values;
                optionSetEnumBindingSource.DataSource = correspondingEnum;
                TableHandler.CurrentEnum = correspondingEnum.LogicalName;
                //EnumNameText.Text = correspondingEnum.Name;

            }
            else if (e.ColumnIndex == 1)
            {
                if (TableHandler.CurrentTable == null)
                {
                    return;
                }
                var currentTable = TableHandler.TableAndPath[TableHandler.CurrentTable].table;
                var currentColumn = currentTable.Columns.FirstOrDefault(c => c.LogicalName == dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                if (currentColumn == null)
                {
                    throw new Exception("Could not find corresponding column");
                }
                var name = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                //MessageBox.Show(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                var newForm = new TryOtherNameForm(name);
                newForm.FormClosing += (s, e) =>
                 {
                     if (!newForm.ModifyName || newForm.Name == name)
                     {
                         return;
                     }
                     // Check if a table is already using this name 

                     if (currentTable.Columns.Any(c => c.Name == newForm.Name))
                     {
                         // If already used, don't modify table name and notify user
                         MessageBox.Show("This name is already in use");
                         return;
                     }
                     // If not, then modify the name 
                     //MessageBox.Show("You modified this Name");


                     var finalName = newForm.Name.StrongFormat();
                     currentColumn.Name = finalName;
                     // Now refresh the whole property
                     //RefreshTabme();
                 };
                newForm.ShowDialog();
            }


        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }


        private void EnumNameText_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EnumNameText.Text) || TableHandler.CurrentTable == null)
            {
                return;

            }
            //Start modifying
            var text = EnumNameText.Text;
            var currentTable = TableHandler.TableAndPath[TableHandler.CurrentTable].table;
            //Get the current table
            var currentEnum = TableHandler.TableAndPath[TableHandler.CurrentTable].table.Enums.FirstOrDefault(en => en.LogicalName == TableHandler.CurrentEnum);
            if (currentEnum == null)
            {
                currentEnum = TableHandler.globalEnumsTable.Enums.FirstOrDefault(en => en.LogicalName == TableHandler.CurrentEnum);
                if (currentEnum == null)
                {
                    return;
                }
            }
            TableHandler.ModifyEnumName(EnumNameText.Text, currentEnum, text); ;

            EnumNameText.Text = currentEnum.Name;
        }





        private void RetrieveEntitiesButton_Click(object sender, EventArgs e)
        {
            if (TableHandler.PublisherPrefixes.Count == 0)
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

                    TableHandler.PublisherPrefixes.AddRange(solutions.Select(s => s.GetAttributeValue<AliasedValue>("publisher.customizationprefix").Value as string).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());

                }

            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddBasicTablesToProject();
        }

        private void AddBasicTablesToProject()
        {
            foreach (var table in TableHandler.BasicTables.Where(t => t.Selected))
            {

                if (TableHandler.TableAndPath.ContainsKey(table.LogicalName))
                {
                    //MessageBox.Show("This is a message for when a table is already in a project, it should be replaced with a dialog box asking wether you want to overwrite it");
                    return;
                }
                TableHandler.TableAndPath[table.LogicalName] = new TableData(table, RemoveCurrentProjectPathFromTablePath(TableHandler.PathToRegisterTables + $"{table.Name}.table"));


                RefreshTreeDisplay();
                RefreshGlobalEnum();



                //Serialize
                //Saveit
                //Refresh tree 

            }
        }

        public void RefreshModelsDisplay()
        {

            AddModelTreeNodes();
        }

        private void SearchColumnTextBox_TextChanged(object sender, EventArgs e)
        {
            if (TableHandler.CurrentTable != null)
            {
                if (TableHandler.TableAndPath.ContainsKey(TableHandler.CurrentTable))
                {
                    var search = SearchColumnTextBox.Text.Split(' ');

                    if (search.Length == 1)
                    {
                        var lowerSearch = SearchColumnTextBox.Text.ToLower();

                        var selectedTable = TableHandler.TableAndPath[TableHandler.CurrentTable].table;
                        columnBindingSource.DataSource = selectedTable.Columns.Where(t => t.Name.ToLower().Contains(lowerSearch) || t.Type.ToString().ToLower().Contains(lowerSearch) || t.LogicalName.Contains(lowerSearch));

                    }
                    else
                    {
                        var selectedTable = TableHandler.TableAndPath[TableHandler.CurrentTable].table;

                        var columnsToShow = new ColumnCollection();
                        foreach (var searchWord in search)
                        {
                            if (searchWord == " " || searchWord == "")
                            {
                                continue;
                            }
                            var lowerSearch = searchWord.ToLower();
                            var correspondingColumns = selectedTable.Columns.Where(t => t.Name.ToLower().Contains(lowerSearch) || t.Type.ToString().ToLower().Contains(lowerSearch) || t.LogicalName.Contains(lowerSearch));
                            foreach (var column in correspondingColumns)
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
            if (CurrentProject == null)
            {
                MessageBox.Show("Select or create a project first.");
                return;
            }
            //RetrieveAttributesForTable(currentTable);
            var form2 = new AddTableForm();
            form2.PublisherPrefixes = TableHandler.PublisherPrefixes;
            form2.PluginControl = this;
            form2.RetrieveEntities();
            form2.ShowDialog();
        }


        public void AddTablesToProject(List<Table> tables)
        {
            foreach (Table table in tables)
            {
                if (TableHandler.TableAndPath.ContainsKey(table.LogicalName))
                {

                    continue;
                }
                while (TableHandler.IsTableNameUsed(table, table.Name))
                {
                    TableHandler.ModifyTableName(table.Name, table, table.Name);
                }
                //MessageBox.Show($"adding table {table.LogicalName} at {TableHandler.PathToRegisterTables}");
                RefreshTableAttributes(table);
                TableHandler.TableAndPath[table.LogicalName] = new TableData(table, RemoveCurrentProjectPathFromTablePath(TableHandler.PathToRegisterTables));
            }
        }

        private void SaveTablesButton_Click(object sender, EventArgs e)
        {
            TableHandler.SaveTables();
        }



        private void RefreshGlobalEnum()
        {
            //optionSetEnumBindingSource.DataSource = TableHandler.globalEnumsTable.Enums;

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
            if (e.ColumnIndex == 0)
            {
                //MessageBox.Show(e.ColumnIndex.ToString());
                //MessageBox.Show(dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString());
                var name = dataGridView3.Rows[e.RowIndex].Cells[0].Value;
                var currentEnum = TableHandler.TableAndPath[TableHandler.CurrentTable].table.Enums.FirstOrDefault(en => en.LogicalName == TableHandler.CurrentEnum);
                if (currentEnum == null)
                {
                    currentEnum = TableHandler.globalEnumsTable.Enums.FirstOrDefault(en => en.LogicalName == TableHandler.CurrentEnum);
                    if (currentEnum == null)
                    {
                        return;
                    }
                }
                //MessageBox.Show(value.ToString());
                var enumValue = currentEnum.Values.ElementAt(e.RowIndex);
                //MessageBox.Show(enumValue.Name);
                TableHandler.ModifyEnumeValueName(enumValue, currentEnum, enumValue.Name);
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
            if (String.IsNullOrEmpty(TableHandler.CurrentTable))
            {
                return;
            }
            var selectedTable = TableHandler.TableAndPath[TableHandler.CurrentTable].table;
            if (selectedTable == null)
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
            if (string.IsNullOrEmpty(TableNameText.Text))
            {
                return;

            }
            //Start modifying
            var text = TableNameText.Text;
            //Get the current table
            var currentTable = TableHandler.TableAndPath[TableLogicalNameText.Text].table;
            TableHandler.ModifyTableName(TableNameText.Text, currentTable, text);
            TableNameText.Text = currentTable.Name;


        }


        private void RefreshModelDetails(XrmFramework.Core.Model model, TreeView tree)
        {
            tree.Nodes.Clear();
            var newNode = new TreeNode(model.Name);
            var nameNode = new TreeNode("name : " + model.Name);
            nameNode.Name = model.Name + ";" + "name";
            newNode.Nodes.Add(nameNode);
            newNode.Nodes.Add(new TreeNode("namespace : " + model.ModelNamespace));
            newNode.Nodes.Add(new TreeNode("table logical name : " + model.TableLogicalName));
            newNode.Nodes.Add(new TreeNode("json serialization strategy : " + model.JsonMemberSerializationStrategy.ToString()));
            var propertiesNode = new TreeNode("Properties");
            propertiesNode.Name = model.Name + ";" + "properties";
            foreach (var prop in model.Properties)
            {
                var propertyNode = new TreeNode(prop.Name);
                propertyNode.Name = prop.Name;
                AddPropertyNode(propertyNode, model, prop);
                propertiesNode.Nodes.Add(propertyNode);
            }
            newNode.Nodes.Add(propertiesNode);
            //ModelDetailTree.Nodes.Add(new Nodes
            tree.Nodes.Add(newNode);

            tree.ExpandAll();
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

        private void AddModelButton_Click(object sender, EventArgs e)
        {
            if (CurrentProject == null)
            {
                MessageBox.Show("Select or create a project first.");
                return;

            }
            ModelHandler.AddModel();
        }

        private void SaveModelsButton_Click(object sender, EventArgs e)
        {
            ModelHandler.SaveModels();
        }

        private void ModelsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ModelDetailTree.Nodes.Clear();
            var name = e.Node.Name;

            if (!ModelHandler.ModelAndPath.ContainsKey(name))
            {
                return;
            }
            // MessageBox.Show(name);
            var model = ModelHandler.ModelAndPath[name].model;
            if (model == null)
            {
                return;
            }

            RefreshModelDetails(model, ModelDetailTree);


        }
        private void AddPropertyNode(TreeNode node, XrmFramework.Core.Model model, ModelProperty property)
        {
            // Add Name
            var nameNode = new TreeNode("Name : " + property.Name);
            nameNode.Name = model.Name + ";" + property.Name + ";" + "name";
            nameNode.ImageIndex = 4;

            node.Nodes.Add(nameNode);
            // Add type
            var typeNode = new TreeNode("Type : " + property.TypeFullName);
            typeNode.Name = model.Name + ";" + property.Name + ";" + "type";
            typeNode.ImageIndex = 4;
            node.Nodes.Add(typeNode);
            // Add JsonName
            var jsonNameNode = new TreeNode("Json Name : " + property.JsonPropertyName);
            jsonNameNode.Name = model.Name + ";" + property.Name + ";" + "JsonPropertyName";
            jsonNameNode.ImageIndex = 4;
            node.Nodes.Add(jsonNameNode);
            var isValidForUpdateNode = new TreeNode("Is Valid For Update : " + property.IsValidForUpdate);
            isValidForUpdateNode.Name = model.Name + ";" + property.Name + ";" + "isValidForUpdate";
            if (property.IsValidForUpdate)
            {
                isValidForUpdateNode.ImageIndex = 3;
                //isValidForUpdateNode.SelectedImageIndex = 1;
            }
            else
            {
                isValidForUpdateNode.ImageIndex = 1;
                //isValidForUpdateNode.SelectedImageIndex = 3;
            }
            node.Nodes.Add(isValidForUpdateNode);

        }

        private void ModelDetailTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var nodeName = e.Node.Name.Split(';');
            var node = e.Node;

            if (nodeName.Length == 2)
            {
                // Modifying one of the main model characteristic
                var model = ModelHandler.ModelAndPath[nodeName[0]].model;

                if (nodeName[1] == "name")
                {

                    // Modifying the name

                    var modelName = nodeName[1];
                    var newForm = new TryOtherNameForm(model.Name);
                    newForm.FormClosing += (o, e) =>
                    {
                        //If the user has chosen to modify (he clicked the modify button) send ou object ?
                        if (!newForm.ModifyName || newForm.Name == model.Name)
                        {
                            return;
                        }
                        // Check if a table is already using this name 

                        if (ModelHandler.IsModelNameUsed(newForm.Name))
                        {
                            // If already used, don't modify table name and notify user
                            MessageBox.Show("This name is already in use");
                            return;
                        }

                        // If not, then modify the name 
                        //MessageBox.Show("You modified this Name");
                        var modelData = ModelHandler.ModelAndPath[model.Name];
                        ModelHandler.ModelAndPath.Remove(model.Name);

                        var finalName = newForm.Name.StrongFormat();
                        modelData.model.Name = finalName;
                        ModelHandler.ModelAndPath[finalName] = modelData;
                        // Now refresh the whole property
                        RefreshModelsDisplay();
                        RefreshModelDetails(ModelHandler.ModelAndPath[finalName].model, ModelDetailTree);
                    };

                    newForm.ShowDialog();

                }
                if (nodeName[1] == "properties")
                {
                    var newForm = new AddModelPropertyForm();
                    newForm.model = model;
                    newForm.LoadPossibleColumns();
                    newForm.FormClosing += (o, e) =>
                      {
                          if (newForm.CreateProperty)
                          {
                              Table correspondingTable = null;
                              correspondingTable = TableHandler.TableAndPath.FirstOrDefault(tp => tp.Value.table.Name == newForm.typeFullName.Replace("Model", "")).Value.table;
                              if (correspondingTable == null)
                              {
                                  throw new Exception("This table should not be null");
                              }
                              var newProp = new ModelProperty();
                              newProp.IsValidForUpdate = newForm.isValidForUpdate;
                              newProp.Name = newForm.propertyName;
                              newProp.JsonPropertyName = newForm.JsonPropertyName;
                              if (newForm.typeFullName.Contains("Model"))
                              {
                                  // Check if this model exists
                                  if (!ModelHandler.ModelAndPath.ContainsKey(newForm.typeFullName))
                                  {
                                      // If not create it

                                      var newModel = new XrmFramework.Core.Model()
                                      {
                                          TableLogicalName = correspondingTable.LogicalName,
                                          ModelNamespace = "BindingModel",
                                          Name = newForm.typeFullName,
                                      };

                                      var idCol = correspondingTable.Columns.FirstOrDefault(c => c.PrimaryType == PrimaryType.Id);
                                      if (idCol == null)
                                      {
                                          throw new Exception("Error, table id corresponding to model was not found");
                                      }
                                      var idProperty = new ModelProperty();
                                      idProperty.Name = "Id";
                                      idProperty.LogicalName = idCol.LogicalName;
                                      idProperty.TypeFullName = "Guid";

                                      var nameCol = correspondingTable.Columns.FirstOrDefault(c => c.PrimaryType == PrimaryType.Name);
                                      //idProperty.JsonPropertyName = 
                                      var nameProperty = new ModelProperty();

                                      if (nameCol == null)
                                      {
                                          throw new Exception("Error, table name corresponding to model was not found");
                                      }
                                      nameProperty.Name = "Name";
                                      nameProperty.LogicalName = nameCol.LogicalName;
                                      nameProperty.TypeFullName = "string";
                                      newModel.Properties.Add(nameProperty);
                                      newModel.Properties.Add(idProperty);


                                      ModelHandler.ModelAndPath[newModel.Name] = new ModelData(newModel, ModelHandler.RemoveCurrentProjectPathFromModelPath(ModelHandler.PathToRegisterModel, CurrentProject.FolderPath));

                                      newProp.TypeFullName = newModel.ModelNamespace + "." + newModel.Name;

                                      RefreshModelsDisplay();

                                  }
                                  else
                                  {
                                      newProp.TypeFullName = newForm.typeFullName;

                                  }




                              }
                              newProp.LogicalName = newForm.logicalName;
                              {
                                  //MessageBox.Show("the newly added property is valid for update");
                                  var sameProp = model.Properties.FirstOrDefault(p => p.LogicalName == newProp.LogicalName && p.IsValidForUpdate);
                                  if (sameProp != null)
                                  {
                                      sameProp.IsValidForUpdate = false;
                                      MessageBox.Show($"The value of IsValidForUpdate has been set to false for {sameProp.Name} because it shares the same column mapping as {newProp.Name}");
                                  }
                              }


                              model.Properties.Add(newProp);
                              RefreshModelDetails(model, ModelDetailTree);

                          }
                      };
                    newForm.ShowDialog();
                    //newForm.Set
                }

            }
            if (nodeName.Length == 3)
            {
                var model = ModelHandler.ModelAndPath[nodeName[0]].model;
                var property = model.Properties.FirstOrDefault(p => p.Name == nodeName[1]);

                // we are modifying a property
                if (nodeName[2] == "name")
                {
                    var propName = nodeName[1];
                    var newForm = new TryOtherNameForm(property.Name);
                    newForm.FormClosing += (o, e) =>
                    {
                        //If the user has chosen to modify (he clicked the modify button) send ou object ?
                        if (!newForm.ModifyName || newForm.Name == property.Name)
                        {
                            return;
                        }
                        // Check if a table is already using this name 

                        if (ModelHandler.IsPropertyNameUsed(newForm.Name, model))
                        {
                            // If already used, don't modify table name and notify user
                            MessageBox.Show("This name is already in use");
                            return;
                        }

                        // If not, then modify the name 
                        //MessageBox.Show("You modified this Name");
                        var finalName = newForm.Name.StrongFormat();
                        property.Name = finalName;
                        // Now refresh the whole property
                        RefreshModelDetails(model, ModelDetailTree);
                    };

                    newForm.ShowDialog();

                }

                if (nodeName[2] == "JsonPropertyName")
                {
                    var propName = nodeName[1];
                    var newForm = new TryOtherNameForm(property.JsonPropertyName);
                    newForm.FormClosing += (o, e) =>
                    {
                        //If the user has chosen to modify (he clicked the modify button) send ou object ?
                        if (!newForm.ModifyName || newForm.Name == property.JsonPropertyName)
                        {


                            return;
                        }
                        // Check if a table is already using this name 

                        if (ModelHandler.IsJsonPropertyNameUsed(newForm.Name, model))
                        {
                            // If already used, don't modify table name and notify user
                            MessageBox.Show("This json property name is already in use");
                            return;
                        }

                        // If not, then modify the name 
                        //MessageBox.Show("You modified this Name");
                        var finalName = newForm.Name.StrongFormat();
                        property.JsonPropertyName = finalName;
                        // Now refresh the whole property
                        RefreshModelDetails(model, ModelDetailTree);
                    };

                    newForm.ShowDialog();
                }

                if (nodeName[2] == "type")
                {
                    //  MessageBox.Show("wuuuu");
                    //  ChoicePickList.Location = e.Node.Bounds.Location;
                    //  ChoicePickList.Enabled = true;
                    //  ChoicePickList.DropDownStyle = ComboBoxStyle.DropDownList;
                    //  
                    //
                    //  ChoicePickList.Visible = true;
                    // // ChoicePickList.BringToFront();
                    // if (!ChoicePickList.Focus())
                    // {
                    //     MessageBox.Show("wut ?");
                    // }
                    //  ChoicePickList.BringToFront();



                    //ChoicePickList.
                    var newForm = new PickListChoiceForm(true);
                    newForm.SetPossibleChoices(ModelHandler.GetPossiblePropertyTypes(model, property.LogicalName));

                    newForm.FormClosing += (o, e) =>
                    {
                        MessageBox.Show(newForm.Value);
                        if (!newForm.ChangeValue || newForm.Value == property.TypeFullName || newForm.Value == "")
                        {
                            return;
                        }
                        property.TypeFullName = newForm.Value;
                        RefreshModelDetails(model, ModelDetailTree);
                        //MessageBox.Show("refreshed model");
                    };
                    newForm.ShowDialog();
                }

                if (nodeName[2] == "isValidForUpdate")
                {
                    if (!property.IsValidForUpdate)
                    {
                        node.ImageIndex = 3;
                        //node.S
                        // Find any property with the same mapping and set their value for IsValidForUpdate to false
                        var sameProp = model.Properties.FirstOrDefault(p => p.LogicalName == property.LogicalName && p.IsValidForUpdate);
                        if (sameProp != null)
                        {
                            sameProp.IsValidForUpdate = false;
                            MessageBox.Show($"The value of IsValidForUpdate for {sameProp.Name} was set to false as it has the same mapping as {property.Name}.");

                        }
                    }
                    else
                    {
                        node.ImageIndex = 1;
                    }
                    property.IsValidForUpdate = !property.IsValidForUpdate;
                    node.Text = "Is Valid For Update : " + property.IsValidForUpdate;



                    RefreshModelDetails(model, ModelDetailTree);

                }

            }

        }

        private void ReloadProjectButton_Click(object sender, EventArgs e)
        {
            if (CurrentProject == null)
            {
                MessageBox.Show("Create or select a project first");
                return;
            }
            TableHandler.TableAndPath.Clear();
            ModelHandler.ModelAndPath.Clear();

            ModelHandler.LoadModelsFromProject(CurrentProject.FolderPath);
            TableHandler.LoadTablesFromProject(CurrentProject.FolderPath);
            RefreshTreeDisplay();
            RefreshModelsDisplay();
            //RefreshModelDetails();
            //textBox1.Text = CurrentProject.FolderPath;
            TableHandler.PathToRegisterTables = CurrentProject.FolderPath + "\\Definitions\\";
            ModelHandler.PathToRegisterModel = CurrentProject.FolderPath + "\\Models\\";
            //MessageBox.Show($"TableHandler.PathToRegisterTables is {TableHandler.PathToRegisterTables}");
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

