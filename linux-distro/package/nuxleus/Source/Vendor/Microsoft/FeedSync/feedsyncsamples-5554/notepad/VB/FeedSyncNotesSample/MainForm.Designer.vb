Imports Microsoft.VisualBasic
Imports System
Namespace FeedSyncNotesSample
	Partial Public Class MainForm
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Windows Form Designer generated code"

		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container()
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
			Me.menuStrip1 = New System.Windows.Forms.MenuStrip()
			Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.refreshToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.optionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.syncToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
			Me.toolStripContainer1 = New System.Windows.Forms.ToolStripContainer()
			Me.statusStrip1 = New System.Windows.Forms.StatusStrip()
			Me.toolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
			Me.toolStripStatusLabelConflict = New System.Windows.Forms.ToolStripStatusLabel()
			Me.textBox1 = New System.Windows.Forms.TextBox()
			Me.toolStripCommands = New System.Windows.Forms.ToolStrip()
			Me.toolStripButtonPreviousNote = New System.Windows.Forms.ToolStripButton()
			Me.toolStripButtonNextNote = New System.Windows.Forms.ToolStripButton()
			Me.toolStripButtonCreateNote = New System.Windows.Forms.ToolStripButton()
			Me.toolStripButtonDeleteNote = New System.Windows.Forms.ToolStripButton()
			Me.toolStripButtonConflict = New System.Windows.Forms.ToolStripButton()
			Me.timer1 = New System.Windows.Forms.Timer(Me.components)
			Me.menuStrip1.SuspendLayout()
			Me.toolStripContainer1.BottomToolStripPanel.SuspendLayout()
			Me.toolStripContainer1.ContentPanel.SuspendLayout()
			Me.toolStripContainer1.TopToolStripPanel.SuspendLayout()
			Me.toolStripContainer1.SuspendLayout()
			Me.statusStrip1.SuspendLayout()
			Me.toolStripCommands.SuspendLayout()
			Me.SuspendLayout()
			' 
			' menuStrip1
			' 
			Me.menuStrip1.Dock = System.Windows.Forms.DockStyle.None
			Me.menuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() { Me.fileToolStripMenuItem, Me.optionsToolStripMenuItem})
			Me.menuStrip1.Location = New System.Drawing.Point(0, 0)
			Me.menuStrip1.Name = "menuStrip1"
			Me.menuStrip1.Size = New System.Drawing.Size(206, 24)
			Me.menuStrip1.TabIndex = 1
			Me.menuStrip1.Text = "menuStrip1"
			' 
			' fileToolStripMenuItem
			' 
			Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() { Me.refreshToolStripMenuItem, Me.exitToolStripMenuItem})
			Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
			Me.fileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
			Me.fileToolStripMenuItem.Text = "&File"
			' 
			' refreshToolStripMenuItem
			' 
			Me.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem"
			Me.refreshToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5
			Me.refreshToolStripMenuItem.Size = New System.Drawing.Size(132, 22)
			Me.refreshToolStripMenuItem.Text = "&Refresh"
'			Me.refreshToolStripMenuItem.Click += New System.EventHandler(Me.refreshToolStripMenuItem_Click);
			' 
			' exitToolStripMenuItem
			' 
			Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
			Me.exitToolStripMenuItem.Size = New System.Drawing.Size(132, 22)
			Me.exitToolStripMenuItem.Text = "E&xit"
'			Me.exitToolStripMenuItem.Click += New System.EventHandler(Me.exitToolStripMenuItem_Click);
			' 
			' optionsToolStripMenuItem
			' 
			Me.optionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() { Me.syncToolStripMenuItem})
			Me.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem"
			Me.optionsToolStripMenuItem.Size = New System.Drawing.Size(61, 20)
			Me.optionsToolStripMenuItem.Text = "&Options"
			' 
			' syncToolStripMenuItem
			' 
			Me.syncToolStripMenuItem.Name = "syncToolStripMenuItem"
			Me.syncToolStripMenuItem.Size = New System.Drawing.Size(108, 22)
			Me.syncToolStripMenuItem.Text = "&Sync..."
'			Me.syncToolStripMenuItem.Click += New System.EventHandler(Me.syncToolStripMenuItem_Click);
			' 
			' toolStripContainer1
			' 
			' 
			' toolStripContainer1.BottomToolStripPanel
			' 
			Me.toolStripContainer1.BottomToolStripPanel.BackColor = System.Drawing.Color.LemonChiffon
			Me.toolStripContainer1.BottomToolStripPanel.Controls.Add(Me.statusStrip1)
			' 
			' toolStripContainer1.ContentPanel
			' 
			Me.toolStripContainer1.ContentPanel.AutoScroll = True
			Me.toolStripContainer1.ContentPanel.Controls.Add(Me.textBox1)
			Me.toolStripContainer1.ContentPanel.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
			Me.toolStripContainer1.ContentPanel.Size = New System.Drawing.Size(206, 145)
			Me.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill
			Me.toolStripContainer1.LeftToolStripPanelVisible = False
			Me.toolStripContainer1.Location = New System.Drawing.Point(0, 0)
			Me.toolStripContainer1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
			Me.toolStripContainer1.Name = "toolStripContainer1"
			Me.toolStripContainer1.RightToolStripPanelVisible = False
			Me.toolStripContainer1.Size = New System.Drawing.Size(206, 216)
			Me.toolStripContainer1.TabIndex = 2
			Me.toolStripContainer1.Text = "toolStripContainer1"
			' 
			' toolStripContainer1.TopToolStripPanel
			' 
			Me.toolStripContainer1.TopToolStripPanel.Controls.Add(Me.menuStrip1)
			Me.toolStripContainer1.TopToolStripPanel.Controls.Add(Me.toolStripCommands)
			' 
			' statusStrip1
			' 
			Me.statusStrip1.BackColor = System.Drawing.SystemColors.Control
			Me.statusStrip1.Dock = System.Windows.Forms.DockStyle.None
			Me.statusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() { Me.toolStripStatusLabel1, Me.toolStripStatusLabelConflict})
			Me.statusStrip1.Location = New System.Drawing.Point(0, 0)
			Me.statusStrip1.Name = "statusStrip1"
			Me.statusStrip1.Size = New System.Drawing.Size(206, 22)
			Me.statusStrip1.SizingGrip = False
			Me.statusStrip1.TabIndex = 4
			Me.statusStrip1.Text = "statusStrip1"
			' 
			' toolStripStatusLabel1
			' 
			Me.toolStripStatusLabel1.Name = "toolStripStatusLabel1"
			Me.toolStripStatusLabel1.Size = New System.Drawing.Size(53, 17)
			Me.toolStripStatusLabel1.Text = "Note 0/0"
			' 
			' toolStripStatusLabelConflict
			' 
			Me.toolStripStatusLabelConflict.Image = (CType(resources.GetObject("toolStripStatusLabelConflict.Image"), System.Drawing.Image))
			Me.toolStripStatusLabelConflict.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
			Me.toolStripStatusLabelConflict.ImageTransparentColor = System.Drawing.Color.Fuchsia
			Me.toolStripStatusLabelConflict.Margin = New System.Windows.Forms.Padding(12, 3, 0, 2)
			Me.toolStripStatusLabelConflict.Name = "toolStripStatusLabelConflict"
			Me.toolStripStatusLabelConflict.Size = New System.Drawing.Size(118, 17)
			Me.toolStripStatusLabelConflict.Text = "Conflicts detected"
			Me.toolStripStatusLabelConflict.Visible = False
			' 
			' textBox1
			' 
			Me.textBox1.BackColor = System.Drawing.Color.LemonChiffon
			Me.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
			Me.textBox1.Dock = System.Windows.Forms.DockStyle.Fill
			Me.textBox1.Location = New System.Drawing.Point(0, 0)
			Me.textBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
			Me.textBox1.Multiline = True
			Me.textBox1.Name = "textBox1"
			Me.textBox1.Size = New System.Drawing.Size(206, 145)
			Me.textBox1.TabIndex = 3
'			Me.textBox1.Validating += New System.ComponentModel.CancelEventHandler(Me.textBox1_Validating);
'			Me.textBox1.TextChanged += New System.EventHandler(Me.textBox1_TextChanged);
			' 
			' toolStripCommands
			' 
			Me.toolStripCommands.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.toolStripCommands.BackColor = System.Drawing.SystemColors.Control
			Me.toolStripCommands.Dock = System.Windows.Forms.DockStyle.None
			Me.toolStripCommands.Items.AddRange(New System.Windows.Forms.ToolStripItem() { Me.toolStripButtonPreviousNote, Me.toolStripButtonNextNote, Me.toolStripButtonCreateNote, Me.toolStripButtonDeleteNote, Me.toolStripButtonConflict})
			Me.toolStripCommands.Location = New System.Drawing.Point(0, 24)
			Me.toolStripCommands.Name = "toolStripCommands"
			Me.toolStripCommands.Size = New System.Drawing.Size(206, 25)
			Me.toolStripCommands.Stretch = True
			Me.toolStripCommands.TabIndex = 1
			' 
			' toolStripButtonPreviousNote
			' 
			Me.toolStripButtonPreviousNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.toolStripButtonPreviousNote.Enabled = False
			Me.toolStripButtonPreviousNote.Image = (CType(resources.GetObject("toolStripButtonPreviousNote.Image"), System.Drawing.Image))
			Me.toolStripButtonPreviousNote.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.toolStripButtonPreviousNote.Name = "toolStripButtonPreviousNote"
			Me.toolStripButtonPreviousNote.Size = New System.Drawing.Size(23, 22)
			Me.toolStripButtonPreviousNote.Text = "toolStripButton2"
			Me.toolStripButtonPreviousNote.ToolTipText = "View the previous note"
'			Me.toolStripButtonPreviousNote.Click += New System.EventHandler(Me.toolStripButtonPreviousNote_Click);
			' 
			' toolStripButtonNextNote
			' 
			Me.toolStripButtonNextNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.toolStripButtonNextNote.Enabled = False
			Me.toolStripButtonNextNote.Image = (CType(resources.GetObject("toolStripButtonNextNote.Image"), System.Drawing.Image))
			Me.toolStripButtonNextNote.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.toolStripButtonNextNote.Name = "toolStripButtonNextNote"
			Me.toolStripButtonNextNote.Size = New System.Drawing.Size(23, 22)
			Me.toolStripButtonNextNote.Text = "toolStripButton3"
			Me.toolStripButtonNextNote.ToolTipText = "View the next note"
'			Me.toolStripButtonNextNote.Click += New System.EventHandler(Me.toolStripButtonNextNote_Click);
			' 
			' toolStripButtonCreateNote
			' 
			Me.toolStripButtonCreateNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.toolStripButtonCreateNote.Image = (CType(resources.GetObject("toolStripButtonCreateNote.Image"), System.Drawing.Image))
			Me.toolStripButtonCreateNote.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.toolStripButtonCreateNote.Name = "toolStripButtonCreateNote"
			Me.toolStripButtonCreateNote.Size = New System.Drawing.Size(23, 22)
			Me.toolStripButtonCreateNote.Text = "toolStripButton4"
			Me.toolStripButtonCreateNote.ToolTipText = "Create a new note"
'			Me.toolStripButtonCreateNote.Click += New System.EventHandler(Me.toolStripButtonCreateNote_Click);
			' 
			' toolStripButtonDeleteNote
			' 
			Me.toolStripButtonDeleteNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.toolStripButtonDeleteNote.Enabled = False
			Me.toolStripButtonDeleteNote.Image = (CType(resources.GetObject("toolStripButtonDeleteNote.Image"), System.Drawing.Image))
			Me.toolStripButtonDeleteNote.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.toolStripButtonDeleteNote.Name = "toolStripButtonDeleteNote"
			Me.toolStripButtonDeleteNote.Size = New System.Drawing.Size(23, 22)
			Me.toolStripButtonDeleteNote.Text = "toolStripButton1"
			Me.toolStripButtonDeleteNote.ToolTipText = "Delete the current note"
'			Me.toolStripButtonDeleteNote.Click += New System.EventHandler(Me.toolStripButtonDeleteNote_Click);
			' 
			' toolStripButtonConflict
			' 
			Me.toolStripButtonConflict.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.toolStripButtonConflict.Image = (CType(resources.GetObject("toolStripButtonConflict.Image"), System.Drawing.Image))
			Me.toolStripButtonConflict.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.toolStripButtonConflict.Name = "toolStripButtonConflict"
			Me.toolStripButtonConflict.Size = New System.Drawing.Size(23, 22)
			Me.toolStripButtonConflict.Text = "toolStripButtonConflict"
			Me.toolStripButtonConflict.ToolTipText = "There is a conflict. Click to resolve."
			Me.toolStripButtonConflict.Visible = False
'			Me.toolStripButtonConflict.Click += New System.EventHandler(Me.toolStripButtonConflict_Click);
			' 
			' timer1
			' 
			Me.timer1.Interval = 30000
'			Me.timer1.Tick += New System.EventHandler(Me.timer1_Tick);
			' 
			' MainForm
			' 
			Me.AutoScaleDimensions = New System.Drawing.SizeF(8F, 21F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.ClientSize = New System.Drawing.Size(206, 216)
			Me.Controls.Add(Me.toolStripContainer1)
			Me.Font = New System.Drawing.Font("Segoe Print", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte(0)))
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
			Me.MainMenuStrip = Me.menuStrip1
			Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
			Me.Name = "MainForm"
			Me.Text = "FeedSync Notes"
'			Me.FormClosing += New System.Windows.Forms.FormClosingEventHandler(Me.MainForm_FormClosing);
'			Me.Load += New System.EventHandler(Me.MainForm_Load);
			Me.menuStrip1.ResumeLayout(False)
			Me.menuStrip1.PerformLayout()
			Me.toolStripContainer1.BottomToolStripPanel.ResumeLayout(False)
			Me.toolStripContainer1.BottomToolStripPanel.PerformLayout()
			Me.toolStripContainer1.ContentPanel.ResumeLayout(False)
			Me.toolStripContainer1.ContentPanel.PerformLayout()
			Me.toolStripContainer1.TopToolStripPanel.ResumeLayout(False)
			Me.toolStripContainer1.TopToolStripPanel.PerformLayout()
			Me.toolStripContainer1.ResumeLayout(False)
			Me.toolStripContainer1.PerformLayout()
			Me.statusStrip1.ResumeLayout(False)
			Me.statusStrip1.PerformLayout()
			Me.toolStripCommands.ResumeLayout(False)
			Me.toolStripCommands.PerformLayout()
			Me.ResumeLayout(False)

		End Sub

		#End Region

		Private menuStrip1 As System.Windows.Forms.MenuStrip
		Private fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
		Private WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
		Private optionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
		Private WithEvents syncToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
		Private toolStripContainer1 As System.Windows.Forms.ToolStripContainer
		Private WithEvents textBox1 As System.Windows.Forms.TextBox
		Private WithEvents timer1 As System.Windows.Forms.Timer
		Private toolStripCommands As System.Windows.Forms.ToolStrip
		Private WithEvents toolStripButtonDeleteNote As System.Windows.Forms.ToolStripButton
		Private WithEvents toolStripButtonPreviousNote As System.Windows.Forms.ToolStripButton
		Private WithEvents toolStripButtonNextNote As System.Windows.Forms.ToolStripButton
		Private WithEvents toolStripButtonCreateNote As System.Windows.Forms.ToolStripButton
		Private WithEvents refreshToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
		Private WithEvents toolStripButtonConflict As System.Windows.Forms.ToolStripButton
		Private statusStrip1 As System.Windows.Forms.StatusStrip
		Private toolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
		Private toolStripStatusLabelConflict As System.Windows.Forms.ToolStripStatusLabel



	End Class
End Namespace

