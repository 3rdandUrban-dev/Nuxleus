'****************************************************************************************
'   
'   Copyright (c) Microsoft Corporation. All rights reserved.
'
'   Use of this code sample is subject to the terms of the Microsoft
'   Permissive License, a copy of which should always be distributed with
'   this file.  You can also access a copy of this license agreement at:
'   http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
'
' ***************************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms

Namespace FeedSyncNotesSample
	Partial Public Class EnterFeedForm
		Inherits Form
		Private m_FeedURL As String = ""
		Private m_FeedContents As String = Nothing
		Private m_Since As String = Nothing
		Private m_FeedType As Microsoft.Samples.FeedSync.Feed.FeedTypes = Microsoft.Samples.FeedSync.Feed.FeedTypes.RSS

		Public Sub New()
			InitializeComponent()
		End Sub

		Public Property FeedURL() As String
			Get
				Return m_FeedURL
			End Get

			Set(ByVal value As String)
				m_FeedURL = value
			End Set
		End Property

		Public ReadOnly Property FeedContents() As String
			Get
				Return m_FeedContents
			End Get
		End Property

		Public Property FeedType() As Microsoft.Samples.FeedSync.Feed.FeedTypes
			Get
				Return m_FeedType
			End Get

			Set(ByVal value As Microsoft.Samples.FeedSync.Feed.FeedTypes)
				m_FeedType = value
			End Set
		End Property

		Public ReadOnly Property Since() As String
			Get
				Return m_Since
			End Get
		End Property

		Private Sub EnterFeedForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			textBox1.Text = m_FeedURL
		End Sub

		Private Sub EnterFeedForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
			If Me.DialogResult = System.Windows.Forms.DialogResult.OK Then
				Dim cursor As Cursor = Me.Cursor
				Try
					Me.Cursor = Cursors.WaitCursor

                    If m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.RSS Then
                        m_FeedURL = Microsoft.Samples.FeedSyncService.FeedManager.GetRSSFeedURL(textBox1.Text)
                    Else
                        m_FeedURL = Microsoft.Samples.FeedSyncService.FeedManager.GetAtomFeedURL(textBox1.Text)
                    End If

					' attempt to load the feed in order to validate this form
					m_FeedContents = Microsoft.Samples.FeedSyncService.FeedManager.ReadFeedContents(m_FeedURL, m_Since)

					e.Cancel = False
				Catch ex As Exception
					Dim err As String = "Unable to read feed:" & Constants.vbCrLf & Constants.vbCrLf & ex.Message
					MessageBox.Show(Me, err, "Error", MessageBoxButtons.OK)
					e.Cancel = True
				Finally
					Me.Cursor = cursor
				End Try
			End If
		End Sub
	End Class
End Namespace
