namespace FeedSyncNotesSample
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.syncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelConflict = new System.Windows.Forms.ToolStripStatusLabel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.toolStripCommands = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonPreviousNote = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonNextNote = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCreateNote = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDeleteNote = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonConflict = new System.Windows.Forms.ToolStripButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStripCommands.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(206, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.refreshToolStripMenuItem.Text = "&Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.syncToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // syncToolStripMenuItem
            // 
            this.syncToolStripMenuItem.Name = "syncToolStripMenuItem";
            this.syncToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.syncToolStripMenuItem.Text = "&Sync...";
            this.syncToolStripMenuItem.Click += new System.EventHandler(this.syncToolStripMenuItem_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.BackColor = System.Drawing.Color.LemonChiffon;
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.textBox1);
            this.toolStripContainer1.ContentPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(206, 145);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(206, 216);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStripCommands);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabelConflict});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(206, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(53, 17);
            this.toolStripStatusLabel1.Text = "Note 0/0";
            // 
            // toolStripStatusLabelConflict
            // 
            this.toolStripStatusLabelConflict.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStatusLabelConflict.Image")));
            this.toolStripStatusLabelConflict.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusLabelConflict.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.toolStripStatusLabelConflict.Margin = new System.Windows.Forms.Padding(12, 3, 0, 2);
            this.toolStripStatusLabelConflict.Name = "toolStripStatusLabelConflict";
            this.toolStripStatusLabelConflict.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabelConflict.Text = "Conflicts detected";
            this.toolStripStatusLabelConflict.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.LemonChiffon;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(206, 145);
            this.textBox1.TabIndex = 3;
            this.textBox1.Validating += new System.ComponentModel.CancelEventHandler(this.textBox1_Validating);
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // toolStripCommands
            // 
            this.toolStripCommands.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.toolStripCommands.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripCommands.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripCommands.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonPreviousNote,
            this.toolStripButtonNextNote,
            this.toolStripButtonCreateNote,
            this.toolStripButtonDeleteNote,
            this.toolStripButtonConflict});
            this.toolStripCommands.Location = new System.Drawing.Point(0, 24);
            this.toolStripCommands.Name = "toolStripCommands";
            this.toolStripCommands.Size = new System.Drawing.Size(206, 25);
            this.toolStripCommands.Stretch = true;
            this.toolStripCommands.TabIndex = 1;
            // 
            // toolStripButtonPreviousNote
            // 
            this.toolStripButtonPreviousNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPreviousNote.Enabled = false;
            this.toolStripButtonPreviousNote.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPreviousNote.Image")));
            this.toolStripButtonPreviousNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPreviousNote.Name = "toolStripButtonPreviousNote";
            this.toolStripButtonPreviousNote.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPreviousNote.Text = "toolStripButton2";
            this.toolStripButtonPreviousNote.ToolTipText = "View the previous note";
            this.toolStripButtonPreviousNote.Click += new System.EventHandler(this.toolStripButtonPreviousNote_Click);
            // 
            // toolStripButtonNextNote
            // 
            this.toolStripButtonNextNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNextNote.Enabled = false;
            this.toolStripButtonNextNote.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonNextNote.Image")));
            this.toolStripButtonNextNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNextNote.Name = "toolStripButtonNextNote";
            this.toolStripButtonNextNote.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonNextNote.Text = "toolStripButton3";
            this.toolStripButtonNextNote.ToolTipText = "View the next note";
            this.toolStripButtonNextNote.Click += new System.EventHandler(this.toolStripButtonNextNote_Click);
            // 
            // toolStripButtonCreateNote
            // 
            this.toolStripButtonCreateNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCreateNote.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCreateNote.Image")));
            this.toolStripButtonCreateNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCreateNote.Name = "toolStripButtonCreateNote";
            this.toolStripButtonCreateNote.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCreateNote.Text = "toolStripButton4";
            this.toolStripButtonCreateNote.ToolTipText = "Create a new note";
            this.toolStripButtonCreateNote.Click += new System.EventHandler(this.toolStripButtonCreateNote_Click);
            // 
            // toolStripButtonDeleteNote
            // 
            this.toolStripButtonDeleteNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDeleteNote.Enabled = false;
            this.toolStripButtonDeleteNote.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDeleteNote.Image")));
            this.toolStripButtonDeleteNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeleteNote.Name = "toolStripButtonDeleteNote";
            this.toolStripButtonDeleteNote.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDeleteNote.Text = "toolStripButton1";
            this.toolStripButtonDeleteNote.ToolTipText = "Delete the current note";
            this.toolStripButtonDeleteNote.Click += new System.EventHandler(this.toolStripButtonDeleteNote_Click);
            // 
            // toolStripButtonConflict
            // 
            this.toolStripButtonConflict.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonConflict.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonConflict.Image")));
            this.toolStripButtonConflict.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConflict.Name = "toolStripButtonConflict";
            this.toolStripButtonConflict.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonConflict.Text = "toolStripButtonConflict";
            this.toolStripButtonConflict.ToolTipText = "There is a conflict. Click to resolve.";
            this.toolStripButtonConflict.Visible = false;
            this.toolStripButtonConflict.Click += new System.EventHandler(this.toolStripButtonConflict_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 30000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(206, 216);
            this.Controls.Add(this.toolStripContainer1);
            this.Font = new System.Drawing.Font("Segoe Print", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "FeedSync Notes";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripCommands.ResumeLayout(false);
            this.toolStripCommands.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem syncToolStripMenuItem;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStrip toolStripCommands;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeleteNote;
        private System.Windows.Forms.ToolStripButton toolStripButtonPreviousNote;
        private System.Windows.Forms.ToolStripButton toolStripButtonNextNote;
        private System.Windows.Forms.ToolStripButton toolStripButtonCreateNote;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonConflict;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelConflict;



    }
}

