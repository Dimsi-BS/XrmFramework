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
    public partial class AddModelPropertyForm : Form
    {
        public XrmFramework.Core.Model model;
        //public ModelProperty property;
        public string typeFullName;
        public string JsonPropertyName;
        public string propertyName;
        public string logicalName;
        public bool isValidForUpdate = false;
        public bool CreateProperty = false;

        public AddModelPropertyForm()
        {
            InitializeComponent();
        }

        private void AddModelPropertyForm_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show(dataGridView1.SelectedRows.Count.ToString());
            if(dataGridView1.SelectedRows.Count > 0)
            {
                ColumnLogicalName.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            }
        }
        public void SetPossibleTypes(List<string> possibleTypes)
        {
            TypeComboBox.Items.Clear();
            TypeComboBox.Items.AddRange(possibleTypes.ToArray());
            TypeComboBox.ResetText();
        }

        public void LoadPossibleColumns()
        {
            var table = TableHandler.TableAndPath[model.TableLogicalName].table;
            var possibleColumn = new List<Column>();
            foreach(var column in table.Columns)
            {
                possibleColumn.Add(column);
               //if(!model.Properties.Any(p=>p.LogicalName == column.LogicalName))
               //{
               //    possibleColumn.Add(column);
               //}
            }
            dataGridView1.DataSource = possibleColumn;
        }

        private void ColumnLogicalName_TextChanged(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(ColumnLogicalName.Text))
            {
                
                var typesList = ModelHandler.GetPossiblePropertyTypes(model, ColumnLogicalName.Text);
                if(typesList.Count > 0)
                {
                    SetPossibleTypes(typesList);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(PropertyNameTextBox.Text) || string.IsNullOrEmpty(JsonNameTextBox.Text) ||string.IsNullOrEmpty(TypeComboBox.Text))
            {
                MessageBox.Show("Fill all fields to create a property.");
                return;
            }
            //Check the name is not already in use for another property
            if(model.Properties.Any(p=>p.Name == PropertyNameTextBox.Text))
            {
                MessageBox.Show($"The name {PropertyNameTextBox.Text} is already in use.");
                return;
            }
            if (model.Properties.Any(p => p.JsonPropertyName == JsonNameTextBox.Text))
            {
                MessageBox.Show($"The json name {JsonNameTextBox.Text} is already in use.");
                return;

            }
            typeFullName = TypeComboBox.Text;
            propertyName = PropertyNameTextBox.Text;
            JsonPropertyName = JsonNameTextBox.Text;
            logicalName = ColumnLogicalName.Text;
            CreateProperty = true;
            Close();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                MessageBox.Show("For each crm mapping, there can only be one property that is valid for update, any other property linked to the same table column will be set to IsValidForUpdate = false");

            }
            isValidForUpdate = checkBox1.Checked;
        }
    }
}
