using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XrmFramework.XrmToolbox
{
    public partial class TryOtherNameForm : Form
    {
        public XrmFrameworkPluginControl PluginControl;
        public string Name;
        public bool ModifyName = false;
        public TryOtherNameForm(string currentName)
        {
            InitializeComponent();
            textBox2.Text = currentName;
            Name = currentName;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(textBox2.Text))
            {
                Name = textBox2.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ModifyName = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
