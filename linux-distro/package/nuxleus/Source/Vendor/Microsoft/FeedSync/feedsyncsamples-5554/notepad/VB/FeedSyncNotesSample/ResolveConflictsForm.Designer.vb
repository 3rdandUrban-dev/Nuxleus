Imports Microsoft.VisualBasic
Imports System
Namespace FeedSyncNotesSample
	Partial Public Class ResolveConflictsForm
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
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ResolveConflictsForm))
			Me.toolStrip1 = New System.Windows.Forms.ToolStrip()
			Me.toolStripButtonPrevConflict = New System.Windows.Forms.ToolStripButton()
			Me.toolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
			Me.toolStripButtonNextConflict = New System.Windows.Forms.ToolStripButton()
			Me.buttonOk = New System.Windows.Forms.Button()
			Me.label2 = New System.Windows.Forms.Label()
			Me.label1 = New System.Windows.Forms.Label()
			Me.toolStrip1.SuspendLayout()
			Me.SuspendLayout()
			' 
			' toolStrip1
			' 
			Me.toolStrip1.Anchor = (CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles))
			Me.toolStrip1.BackColor = System.Drawing.Color.Transparent
			Me.toolStrip1.Dock = System.Windows.Forms.DockStyle.None
			Me.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
			Me.toolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() { Me.toolStripButtonPrevConflict, Me.toolStripLabel1, Me.toolStripButtonNextConflict})
			Me.toolStrip1.Location = New System.Drawing.Point(82, 144)
			Me.toolStrip1.Name = "toolStrip1"
			Me.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
			Me.toolStrip1.Size = New System.Drawing.Size(73, 25)
			Me.toolStrip1.TabIndex = 0
			Me.toolStrip1.Text = "toolStrip1"
			' 
			' toolStripButtonPrevConflict
			' 
			Me.toolStripButtonPrevConflict.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.toolStripButtonPrevConflict.Image = (CType(resources.GetObject("toolStripButtonPrevConflict.Image"), System.Drawing.Image))
			Me.toolStripButtonPrevConflict.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.toolStripButtonPrevConflict.Name = "toolStripButtonPrevConflict"
			Me.toolStripButtonPrevConflict.Size = New System.Drawing.Size(23, 22)
			Me.toolStripButtonPrevConflict.Text = "toolStripButtonPrev"
			Me.toolStripButtonPrevConflict.ToolTipText = "View Previous Note"
'			Me.toolStripButtonPrevConflict.Click += New System.EventHandler(Me.toolStripButtonPrevConflict_Click);
			' 
			' toolStripLabel1
			' 
			Me.toolStripLabel1.Name = "toolStripLabel1"
			Me.toolStripLabel1.Size = New System.Drawing.Size(24, 22)
			Me.toolStripLabel1.Text = "0/0"
			' 
			' toolStripButtonNextConflict
			' 
			Me.toolStripButtonNextConflict.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.toolStripButtonNextConflict.Image = (CType(resources.GetObject("toolStripButtonNextConflict.Image"), System.Drawing.Image))
			Me.toolStripButtonNextConflict.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.toolStripButtonNextConflict.Name = "toolStripButtonNextConflict"
			Me.toolStripButtonNextConflict.Size = New System.Drawing.Size(23, 22)
			Me.toolStripButtonNextConflict.Text = "toolStripButtonNext"
			Me.toolStripButtonNextConflict.ToolTipText = "View Next Note"
'			Me.toolStripButtonNextConflict.Click += New System.EventHandler(Me.toolStripButtonNextConflict_Click);
			' 
			' buttonOk
			' 
			Me.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK
			Me.buttonOk.Location = New System.Drawing.Point(79, 172)
			Me.buttonOk.Name = "buttonOk"
			Me.buttonOk.Size = New System.Drawing.Size(75, 23)
			Me.buttonOk.TabIndex = 2
			Me.buttonOk.Text = "Ok"
			Me.buttonOk.UseVisualStyleBackColor = True
			' 
			' label2
			' 
			Me.label2.Location = New System.Drawing.Point(-1, 3)
			Me.label2.Name = "label2"
			Me.label2.Size = New System.Drawing.Size(235, 18)
			Me.label2.TabIndex = 3
			Me.label2.Text = "Select the note to use as the final version:"
			' 
			' label1
			' 
			Me.label1.BackColor = System.Drawing.Color.LemonChiffon
			Me.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
			Me.label1.Font = New System.Drawing.Font("Segoe Print", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte(0)))
			Me.label1.Location = New System.Drawing.Point(29, 21)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(174, 123)
			Me.label1.TabIndex = 4
			Me.label1.Text = "label2"
			' 
			' ResolveConflictsForm
			' 
			Me.AcceptButton = Me.buttonOk
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.ClientSize = New System.Drawing.Size(232, 199)
			Me.Controls.Add(Me.label1)
			Me.Controls.Add(Me.label2)
			Me.Controls.Add(Me.buttonOk)
			Me.Controls.Add(Me.toolStrip1)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.MaximizeBox = False
			Me.MinimizeBox = False
			Me.Name = "ResolveConflictsForm"
			Me.Text = "Resolve Conflicts"
'			Me.Load += New System.EventHandler(Me.ResolveConflictsForm_Load);
			Me.toolStrip1.ResumeLayout(False)
			Me.toolStrip1.PerformLayout()
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub

		#End Region

		Private toolStrip1 As System.Windows.Forms.ToolStrip
		Private WithEvents toolStripButtonPrevConflict As System.Windows.Forms.ToolStripButton
		Private toolStripLabel1 As System.Windows.Forms.ToolStripLabel
		Private WithEvents toolStripButtonNextConflict As System.Windows.Forms.ToolStripButton
		Private buttonOk As System.Windows.Forms.Button
		Private label2 As System.Windows.Forms.Label
		Private label1 As System.Windows.Forms.Label
	End Class
End Namespace