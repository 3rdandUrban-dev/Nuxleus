Imports Microsoft.VisualBasic
Imports System
Namespace FeedSyncNotesSample
	Partial Public Class EnterFeedForm
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
			Me.label1 = New System.Windows.Forms.Label()
			Me.textBox1 = New System.Windows.Forms.TextBox()
			Me.buttonOK = New System.Windows.Forms.Button()
			Me.buttonCancel = New System.Windows.Forms.Button()
			Me.SuspendLayout()
			' 
			' label1
			' 
			Me.label1.AutoSize = True
			Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (CByte(0)))
			Me.label1.Location = New System.Drawing.Point(13, 13)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(326, 13)
			Me.label1.TabIndex = 0
			Me.label1.Text = "Enter the URL for a valid FeedSync feed (RSS or Atom):"
			' 
			' textBox1
			' 
			Me.textBox1.Location = New System.Drawing.Point(16, 34)
			Me.textBox1.Name = "textBox1"
			Me.textBox1.Size = New System.Drawing.Size(323, 20)
			Me.textBox1.TabIndex = 1
			' 
			' buttonOK
			' 
			Me.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK
			Me.buttonOK.Location = New System.Drawing.Point(96, 71)
			Me.buttonOK.Name = "buttonOK"
			Me.buttonOK.Size = New System.Drawing.Size(75, 23)
			Me.buttonOK.TabIndex = 2
			Me.buttonOK.Text = "OK"
			Me.buttonOK.UseVisualStyleBackColor = True
			' 
			' buttonCancel
			' 
			Me.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.buttonCancel.Location = New System.Drawing.Point(177, 71)
			Me.buttonCancel.Name = "buttonCancel"
			Me.buttonCancel.Size = New System.Drawing.Size(75, 23)
			Me.buttonCancel.TabIndex = 3
			Me.buttonCancel.Text = "Cancel"
			Me.buttonCancel.UseVisualStyleBackColor = True
			' 
			' EnterFeedForm
			' 
			Me.AcceptButton = Me.buttonOK
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.CancelButton = Me.buttonCancel
			Me.ClientSize = New System.Drawing.Size(349, 117)
			Me.Controls.Add(Me.buttonCancel)
			Me.Controls.Add(Me.buttonOK)
			Me.Controls.Add(Me.textBox1)
			Me.Controls.Add(Me.label1)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.MaximizeBox = False
			Me.MinimizeBox = False
			Me.Name = "EnterFeedForm"
			Me.Text = "Enter Feed URL"
'			Me.FormClosing += New System.Windows.Forms.FormClosingEventHandler(Me.EnterFeedForm_FormClosing);
'			Me.Load += New System.EventHandler(Me.EnterFeedForm_Load);
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub

		#End Region

		Private label1 As System.Windows.Forms.Label
		Private textBox1 As System.Windows.Forms.TextBox
		Private buttonOK As System.Windows.Forms.Button
		Private buttonCancel As System.Windows.Forms.Button
	End Class
End Namespace