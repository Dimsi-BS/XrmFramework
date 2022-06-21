using DefinitionManager;

namespace XrmFramework.DefinitionManager
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.generateDefinitionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getEntitiesFromCRMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.entityListView = new XrmFramework.DefinitionManager.EntityListViewControl();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.attributeListView = new XrmFramework.DefinitionManager.AttributeListViewControl();
            this.enumListView = new XrmFramework.DefinitionManager.EnumListViewControl();
            this.picklistNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.picklistDisplayNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.picklistValueHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.attributePropertyNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.attributeDisplayNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.attributeLogicalNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.attributeTypeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 656);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1612, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 16);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateDefinitionsToolStripMenuItem,
            this.getEntitiesFromCRMToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(1612, 30);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // generateDefinitionsToolStripMenuItem
            // 
            this.generateDefinitionsToolStripMenuItem.Enabled = false;
            this.generateDefinitionsToolStripMenuItem.Name = "generateDefinitionsToolStripMenuItem";
            this.generateDefinitionsToolStripMenuItem.Size = new System.Drawing.Size(159, 28);
            this.generateDefinitionsToolStripMenuItem.Text = "Generate Definitions";
            this.generateDefinitionsToolStripMenuItem.Click += new System.EventHandler(this.generateDefinitionsToolStripMenuItem_Click);
            // 
            // getEntitiesFromCRMToolStripMenuItem
            // 
            this.getEntitiesFromCRMToolStripMenuItem.Name = "getEntitiesFromCRMToolStripMenuItem";
            this.getEntitiesFromCRMToolStripMenuItem.Size = new System.Drawing.Size(171, 28);
            this.getEntitiesFromCRMToolStripMenuItem.Text = "Get Entities From CRM";
            this.getEntitiesFromCRMToolStripMenuItem.Click += new System.EventHandler(this.getEntitiesFromCRMToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 30);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.entityListView);
            this.splitContainer1.Panel1MinSize = 300;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2MinSize = 600;
            this.splitContainer1.Size = new System.Drawing.Size(1612, 626);
            this.splitContainer1.SplitterDistance = 406;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 3;
            // 
            // entityListView
            // 
            this.entityListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityListView.Enabled = false;
            this.entityListView.Location = new System.Drawing.Point(0, 0);
            this.entityListView.Margin = new System.Windows.Forms.Padding(0);
            this.entityListView.Name = "entityListView";
            this.entityListView.Size = new System.Drawing.Size(406, 626);
            this.entityListView.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.attributeListView);
            this.splitContainer2.Panel1MinSize = 100;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.enumListView);
            this.splitContainer2.Panel2Collapsed = true;
            this.splitContainer2.Panel2MinSize = 100;
            this.splitContainer2.Size = new System.Drawing.Size(1201, 626);
            this.splitContainer2.SplitterDistance = 100;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 4;
            // 
            // attributeListView
            // 
            this.attributeListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attributeListView.Enabled = false;
            this.attributeListView.Location = new System.Drawing.Point(0, 0);
            this.attributeListView.Margin = new System.Windows.Forms.Padding(0);
            this.attributeListView.Name = "attributeListView";
            this.attributeListView.Size = new System.Drawing.Size(1201, 626);
            this.attributeListView.TabIndex = 2;
            // 
            // enumListView
            // 
            this.enumListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.enumListView.Location = new System.Drawing.Point(0, 0);
            this.enumListView.Margin = new System.Windows.Forms.Padding(0);
            this.enumListView.Name = "enumListView";
            this.enumListView.Size = new System.Drawing.Size(150, 46);
            this.enumListView.TabIndex = 4;
            // 
            // picklistNameHeader
            // 
            this.picklistNameHeader.Text = "Name";
            this.picklistNameHeader.Width = 234;
            // 
            // picklistDisplayNameHeader
            // 
            this.picklistDisplayNameHeader.Text = "Display Name";
            this.picklistDisplayNameHeader.Width = 205;
            // 
            // picklistValueHeader
            // 
            this.picklistValueHeader.Text = "Value";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(205, 28);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(204, 24);
            this.toolStripMenuItem1.Text = "Copy LogicalName";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1612, 678);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Definition Manager";
            this.Load += new System.EventHandler(this.DefinitionManager_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private EntityListViewControl entityListView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem generateDefinitionsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private AttributeListViewControl attributeListView;
        private System.Windows.Forms.ColumnHeader attributePropertyNameHeader;
        private System.Windows.Forms.ColumnHeader attributeDisplayNameHeader;
        private System.Windows.Forms.ColumnHeader attributeLogicalNameHeader;
        private System.Windows.Forms.ColumnHeader attributeTypeHeader;
        private EnumListViewControl enumListView;
        private System.Windows.Forms.ColumnHeader picklistNameHeader;
        private System.Windows.Forms.ColumnHeader picklistDisplayNameHeader;
        private System.Windows.Forms.ColumnHeader picklistValueHeader;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem getEntitiesFromCRMToolStripMenuItem;
    }
}

