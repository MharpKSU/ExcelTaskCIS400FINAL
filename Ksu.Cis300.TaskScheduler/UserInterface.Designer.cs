namespace Ksu.Cis300.TaskScheduler
{
    partial class UserInterface
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            uxListView = new ListView();
            uxFile = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            uxOpen = new ToolStripMenuItem();
            Save = new ToolStripMenuItem();
            tableLayoutPanel1 = new TableLayoutPanel();
            uxFile.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // uxListView
            // 
            uxListView.Dock = DockStyle.Fill;
            uxListView.Location = new Point(3, 52);
            uxListView.Name = "uxListView";
            uxListView.Size = new Size(891, 395);
            uxListView.TabIndex = 0;
            uxListView.UseCompatibleStateImageBehavior = false;
            uxListView.SelectedIndexChanged += UXListView;
            // 
            // uxFile
            // 
            uxFile.Dock = DockStyle.None;
            uxFile.ImageScalingSize = new Size(40, 40);
            uxFile.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            uxFile.Location = new Point(0, 0);
            uxFile.Name = "uxFile";
            uxFile.Size = new Size(95, 49);
            uxFile.TabIndex = 1;
            uxFile.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { uxOpen, Save });
            fileToolStripMenuItem.ImageAlign = ContentAlignment.TopLeft;
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(87, 45);
            fileToolStripMenuItem.Text = "File";
            // 
            // uxOpen
            // 
            uxOpen.Name = "uxOpen";
            uxOpen.Size = new Size(306, 54);
            uxOpen.Text = "Open...";
            uxOpen.Click += UXOpen;
            // 
            // Save
            // 
            Save.Enabled = false;
            Save.Name = "Save";
            Save.Size = new Size(306, 54);
            Save.Text = "Save As...";
            Save.Click += UXSave;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(uxListView, 0, 1);
            tableLayoutPanel1.Controls.Add(uxFile, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(897, 450);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // UserInterface
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(897, 450);
            Controls.Add(tableLayoutPanel1);
            MainMenuStrip = uxFile;
            Name = "UserInterface";
            Text = "Form1";
            Load += UserInterface_Load;
            uxFile.ResumeLayout(false);
            uxFile.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ListView uxListView;
        private MenuStrip uxFile;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem uxOpen;
        private ToolStripMenuItem Save;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
