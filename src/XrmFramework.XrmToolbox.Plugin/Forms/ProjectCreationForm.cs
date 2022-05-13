using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace XrmFramework.XrmToolbox
{
    public partial class ProjectCreationForm : Form
    {
        public string ProjectPath = "";
        public XrmFrameworkPluginControl PluginControl;

        public ProjectCreationForm()
        {
            InitializeComponent();
            PromptProjectFolderSelection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Enter a valid project name");
                return;
            }
            if(string.IsNullOrEmpty(ProjectPath))
            {
                MessageBox.Show("Select a valid folder to create your project");
                PromptProjectFolderSelection();

            }

            var projectName = textBox1.Text;
            var cmdText = $"/C powershell -command \" dotnet new -i XrmFramework.Templates\"";
            var cmdText2 = $"/C powershell -command \"Set-Location {ProjectPath}; dotnet new xrmSolution -n {projectName};\"";
            //Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.Arguments = $"new -i XrmFramework.Templates";
            startInfo.Arguments = cmdText;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = ProjectPath;
            //startInfo.FileName = "dotnet";
            var process = Process.Start(startInfo);
            process.WaitForExit();

            //startInfo.Arguments = $"new -n {projectName}";
            startInfo.FileName = "cmd.exe";
            //startInfo.FileName = "dotnet";
            startInfo.Arguments = cmdText2;

            startInfo.Verb = "runas";
            var process2 = Process.Start(startInfo);
            process2.WaitForExit();

            // plugin Control add the path to this project and set it as current
            PluginControl.SetCurrentProject(ProjectPath + "\\" + projectName);
            this.Close();
            //process2.WaitForInputIdle();
            //Run creation commands in powershell opened from the project folder
            //System.Diagnostics.Process.Start("CMD.exe", cmdText);




        }

        private void PromptProjectFolderSelection()
        {
            folderBrowserDialog1.ShowDialog();
            if (Directory.Exists(folderBrowserDialog1.SelectedPath))
            {
                ProjectPath = folderBrowserDialog1.SelectedPath;
            }
            else
            {
                MessageBox.Show("Selected path is invalid");
                PromptProjectFolderSelection();
            }

            textBox4.Text = ProjectPath;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            PromptProjectFolderSelection();
        }
    }
}
