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

namespace XrmFramework.XrmToolbox.Forms
{
    public partial class CreateModelForm : Form
    {
        public XrmFrameworkPluginControl PluginControl;
        public bool CreateModel = false;
        public string modelName;
        public string modelNamespace;
        public string tableLogicalName;

        public CreateModelForm()
        {
            InitializeComponent();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        public void SetTableBindingSource(TableCollection tables)
        {
            tableBindingSource.DataSource = tables;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.SelectedRows.Count > 0)
            {
                TableNameTextBox.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                TableLogicalName.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            tableLogicalName = TableLogicalName.Text;
            modelName = ModelNameText.Text;
            modelNamespace = NamespaceComboBox.Text;
            if(string.IsNullOrEmpty(tableLogicalName))
            {
                MessageBox.Show("Select a table to create a model from.");
                return; 
            }
            if (string.IsNullOrEmpty(modelNamespace))
            {
                MessageBox.Show("Select a namespace in which to create your model.");
                return;
            }
            if (string.IsNullOrEmpty(modelName))
            {
                MessageBox.Show("Give a name to your model");
                return;
            }
            CreateModel = true;
            Close();
        }

        public void SetExistingNamespaces(List<string> namespaces)
        {
            NamespaceComboBox.Items.AddRange(namespaces.ToArray());
        }
    }
}
