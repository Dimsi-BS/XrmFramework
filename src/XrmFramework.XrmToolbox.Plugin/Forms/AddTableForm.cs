using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmFramework.Core;
using XrmFramework.XrmToolbox.DataHandlers;

namespace XrmFramework.XrmToolbox
{
    public partial class AddTableForm : Form
    {
        public List<string> PublisherPrefixes { get; set; } = new();
        public TableCollection BaseTables = new TableCollection();
        public XrmFrameworkPluginControl PluginControl;

        public AddTableForm()
        {
            InitializeComponent();
            
        }

        public void RetrieveEntities()
        {
            PluginControl.RetrieveAllEntities(tableBindingSource1,BaseTables);
            //BaseTables.Add(new Table { Name = "fezg", LogicalName = "fzefze" });

            //tableBindingSource1.DataSource = BaseTables;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddSelectedTablesToPluginControl();
            this.Close();
        }

        private void SearchBar_TextChanged(object sender, EventArgs e)
        {
            
            var search = SearchBar.Text.Split(' ');

            if (search.Length == 1)
            {
                var lowerSearch = SearchBar.Text.ToLower();

                tableBindingSource1.DataSource = BaseTables.Where(t => t.Name.ToLower().Contains(lowerSearch) || t.LogicalName.Contains(lowerSearch));

            }
            else
            {

                var tablesToShow = new TableCollection();
                foreach (var searchWord in search)
                {
                    if (searchWord == " " || searchWord == "")
                    {
                        continue;
                    }
                    var lowerSearch = searchWord.ToLower();
                    var correspondingTables = BaseTables.Where(t => t.Name.ToLower().Contains(lowerSearch) || t.LogicalName.Contains(lowerSearch));
                    foreach (var table in correspondingTables)
                    {
                        tablesToShow.Add(table);
                    }

                }
                tableBindingSource1.DataSource = tablesToShow;
            }

            if(SearchBar.Text == "")
            {
                tableBindingSource1.DataSource = BaseTables;

            }
        }

        public void AddSelectedTablesToPluginControl()
        {
            PluginControl.AddTablesToProject(TableHandler.BasicTables.Where(t => t.Selected).ToList());
            PluginControl.RefreshTreeDisplay();

        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Search_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
