using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XrmFramework.XrmToolbox.Forms
{
    public partial class PickListChoiceForm : Form
    {
        public string Value = "";
        public bool ChangeValue = false;
        
        public PickListChoiceForm(bool IsDropDownList)
        {
            
            InitializeComponent();
           //ChoiceComboBox.SelectedValueChanged += ChoiceComboBox_SelectedIndexChanged;
           //ChoiceComboBox.DropDownClosed += ChoiceComboBox_SelectedIndexChanged;

            if (IsDropDownList)
            {
                ChoiceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            else
            {
                ChoiceComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            }
        }

        private void ChoiceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("Changed value");
            Value = ChoiceComboBox.Text;
            
        }

        public void SetPossibleChoices(List<string> choices)
        {
            ChoiceComboBox.Items.AddRange(choices.ToArray());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangeValue = true;
            Close();
        }
    }
}
