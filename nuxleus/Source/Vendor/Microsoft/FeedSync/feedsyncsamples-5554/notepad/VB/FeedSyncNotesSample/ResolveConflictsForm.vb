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
	Partial Public Class ResolveConflictsForm
		Inherits Form
		Private m_ConflictItemNode As Microsoft.Samples.FeedSync.FeedItemNode = Nothing
		Private m_CurrentConflictIndex As Integer = 0

		Public Sub New()
			InitializeComponent()
		End Sub

		' Set the conflict item node that this form will display
		' This property must be set before running the form
		Public WriteOnly Property ConflictItem() As Microsoft.Samples.FeedSync.FeedItemNode
			Set(ByVal value As Microsoft.Samples.FeedSync.FeedItemNode)
				m_ConflictItemNode = value
			End Set
		End Property

		Public ReadOnly Property ResolvedItem() As Microsoft.Samples.FeedSync.FeedItemNode
			Get
				' Return either the original node, or any of the conflicts.
				' The item and index 0 will represent the original, and above that as an 
				' index into the array of conflict nodes
				If m_CurrentConflictIndex = 0 Then
					Return m_ConflictItemNode
				Else
					Return m_ConflictItemNode.SyncNode.ConflictFeedItemNodes(m_CurrentConflictIndex - 1)
				End If
			End Get
		End Property

		Private Sub DisplayCurrentConflictItem()
			Dim node As Microsoft.Samples.FeedSync.FeedItemNode = ResolvedItem

			label1.Text = node.Description

			Dim total As Integer = m_ConflictItemNode.SyncNode.ConflictFeedItemNodes.Length

			' enable/disable buttons appropriately
			toolStripButtonPrevConflict.Enabled = m_CurrentConflictIndex > 0
			toolStripButtonNextConflict.Enabled = m_CurrentConflictIndex < total

			' Update current and total number of conflicts
			toolStripLabel1.Text = String.Format("{0}/{1}", m_CurrentConflictIndex + 1, total+1)
		End Sub

		Private Sub ResolveConflictsForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			DisplayCurrentConflictItem()
		End Sub

		Private Sub toolStripButtonPrevConflict_Click(ByVal sender As Object, ByVal e As EventArgs) Handles toolStripButtonPrevConflict.Click
			If m_CurrentConflictIndex > 0 Then
				m_CurrentConflictIndex -= 1
				DisplayCurrentConflictItem()
			End If
		End Sub

		Private Sub toolStripButtonNextConflict_Click(ByVal sender As Object, ByVal e As EventArgs) Handles toolStripButtonNextConflict.Click
			If m_CurrentConflictIndex < m_ConflictItemNode.SyncNode.ConflictFeedItemNodes.Length Then
				m_CurrentConflictIndex += 1
				DisplayCurrentConflictItem()
			End If
		End Sub
	End Class
End Namespace