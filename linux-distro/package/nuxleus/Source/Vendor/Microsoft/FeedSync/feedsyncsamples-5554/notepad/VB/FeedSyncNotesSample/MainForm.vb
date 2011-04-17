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
Imports System.Configuration
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms

Namespace FeedSyncNotesSample
	Partial Public Class MainForm
		Inherits Form
		Private Const SHARED_NOTES_LOCAL_FILE_NAME As String = "notes.xml"
		Private Const SHARED_NOTES_FEED_TYPE As Microsoft.Samples.FeedSync.Feed.FeedTypes = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom

		Private m_FeedURL As String = Nothing
		Private m_Since As String = Nothing
		Private m_Username As String = Nothing
		Private m_Published As Boolean = False
		Private m_CurrentItemIndex As Integer = 0
		Private m_Feed As Microsoft.Samples.FeedSync.Feed = Nothing

		Private Class FeedItemComparer
			Implements IComparer(Of Microsoft.Samples.FeedSync.FeedItemNode)
            Public Function Compare(ByVal x As Microsoft.Samples.FeedSync.FeedItemNode, ByVal y As Microsoft.Samples.FeedSync.FeedItemNode) As Integer Implements System.Collections.Generic.IComparer(Of Microsoft.Samples.FeedSync.FeedItemNode).Compare
                ' compare first time stamps for the items:
                Dim xCreated As Nullable(Of DateTime) = x.SyncNode.HistoryNodes(x.SyncNode.HistoryNodes.Length - 1).WhenDateTime
                Dim yCreated As Nullable(Of DateTime) = y.SyncNode.HistoryNodes(y.SyncNode.HistoryNodes.Length - 1).WhenDateTime

                Dim res As Integer = xCreated.Value.CompareTo(yCreated.Value)

                If res <> 0 Then
                    Return res
                End If

                ' both where created at exactly the same time! Use the item id to break the tie
                ' ideally an application would use a custom data model to specify ordering
                Dim xID As String = x.SyncNode.ID
                Dim yID As String = y.SyncNode.ID

                Return xID.CompareTo(yID)
            End Function
		End Class

		' A class to maintain a reasonable ordering of feed items. This is as simplistic implementation for sample
		' purposes. A more robust implementation should maintain a data model within each feed item to specify
		' logical ordering within an application.
		Private Class SortedFeedItemList
			Inherits SortedList(Of Microsoft.Samples.FeedSync.FeedItemNode, Microsoft.Samples.FeedSync.FeedItemNode)
			Public Sub New()
				MyBase.New(New FeedItemComparer())
			End Sub

			Public Sub AddFeedItemNode(ByVal feedItemNode As Microsoft.Samples.FeedSync.FeedItemNode)
				Add(feedItemNode, feedItemNode)
			End Sub
		End Class

		Private m_ItemNodes As SortedFeedItemList = New SortedFeedItemList()

		Public Sub New()
			InitializeComponent()

			m_FeedURL = My.Settings.Default.FeedURL

			m_Username = Environment.UserName & "@" & Environment.MachineName
		End Sub

		Private Sub CreateOrOpenFeed()
			Dim incomingFeed As Microsoft.Samples.FeedSync.Feed = Nothing

			' if there is a url to use, attempt to read the feed from that url
			If (Not String.IsNullOrEmpty(m_FeedURL)) Then
				Try
					' ideally this would happen asynchronously while the app displays
					' a progress bar, or loading message. Doing this here will slow down the
					' app start up.
					incomingFeed = ReadFeedFromURL(m_FeedURL)
				Catch ex As Exception
					System.Diagnostics.Debug.WriteLine("Failed to read feed from " & m_FeedURL & " : " & ex.Message)
				End Try
			End If

			' see if there is a local file to use
			If System.IO.File.Exists(SHARED_NOTES_LOCAL_FILE_NAME) Then
				Dim xmlDoc As System.Xml.XmlDocument = New System.Xml.XmlDocument()
				xmlDoc.Load(SHARED_NOTES_LOCAL_FILE_NAME)

				m_Feed = Microsoft.Samples.FeedSync.Feed.Create(xmlDoc)

				LoadFeedContents()

				' there is a local file and a feed from the web so they have to be synchronized
				' now to merge any changes that occured since the app last ran
				If incomingFeed IsNot Nothing Then
					SyncToIncomingFeed(incomingFeed)
				End If
			Else
				' there was no local file. If there is a feed from the web use that
				' otherwise create a new feed in memory

				If incomingFeed IsNot Nothing Then
					m_Feed = incomingFeed
					LoadFeedContents()
				Else
					m_Feed = Microsoft.Samples.FeedSync.Feed.Create ("Shared Notes", "Shared Notes sample feed", SHARED_NOTES_LOCAL_FILE_NAME, SHARED_NOTES_FEED_TYPE)
				End If
			End If

			' display the first note
			DisplayCurrentNote()
		End Sub

		Private Sub SaveLocalFeed()
			' serialize changes to disk:
			m_Feed.XmlDocument.Save(SHARED_NOTES_LOCAL_FILE_NAME)
		End Sub

		Private Sub LoadFeedContents()
			m_ItemNodes.Clear()

			' load up the item nodes into the list, skipping items that are deleted:
			For Each itemNode As Microsoft.Samples.FeedSync.FeedItemNode In m_Feed.FeedItemNodes
				If (Not itemNode.SyncNode.Deleted) Then
					m_ItemNodes.AddFeedItemNode(itemNode)
				End If
			Next itemNode

			Me.Text = m_Feed.Title
		End Sub

		Private Function IsLocalFeedEmpty() As Boolean
			' Test to see if the user hasn't actually created any notes yet
			Return (m_Feed.FeedItemNodes.Length = 1 AndAlso CurrentItemNode.Description.Length = 0)
		End Function

		Private ReadOnly Property CurrentItemNode() As Microsoft.Samples.FeedSync.FeedItemNode
			Get
				Return m_ItemNodes.Values(m_CurrentItemIndex)
			End Get
		End Property

		Private Sub DisplayConflictStatus()
			' Display conflict button if there are any conflicts on this node
			Dim haveConflicts As Boolean = (CurrentItemNode.SyncNode.ConflictFeedItemNodes.Length > 0)
			toolStripButtonConflict.Visible = haveConflicts
			toolStripStatusLabelConflict.Visible = haveConflicts
		End Sub

		Private Sub DisplayCurrentNote()
			' if there are no notes available to display, create one now:
			If m_ItemNodes.Count = 0 Then
				CreateNote()
			Else
				' Display current item content. Assume these are plain text
				textBox1.Text = CurrentItemNode.Description

				' Move caret to the end of the control
				' for some reason this control wants to select all the text when changed programatically
				textBox1.Select(textBox1.Text.Length, 0)

				' Enable or Disable appropriate navigation commands
				toolStripButtonPreviousNote.Enabled = (m_CurrentItemIndex <> 0)
				toolStripButtonNextNote.Enabled = (m_CurrentItemIndex <> m_ItemNodes.Count - 1)
				toolStripButtonDeleteNote.Enabled = m_ItemNodes.Count > 1

				DisplayConflictStatus()

				' Update status bar with current item index (1 based index) and the total number of items
				toolStripStatusLabel1.Text = String.Format("Note {0}/{1}", m_CurrentItemIndex + 1, m_ItemNodes.Count)
			End If
		End Sub

		Private Sub DisplayNoteByID(ByVal syncID As String)
			' default to first item in case the item is gone
			m_CurrentItemIndex = 0

			Dim feedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = Nothing

			' get the feed item with this id:
			If m_Feed.FindFeedItemNode(syncID, feedItemNode) Then
				' get index in local list of nodes
				m_CurrentItemIndex = m_ItemNodes.IndexOfValue(feedItemNode)

				If m_CurrentItemIndex = -1 Then
					' not found. default to first item
					m_CurrentItemIndex = 0
				End If
			End If

			DisplayCurrentNote()
		End Sub

		Private Sub DisplayNextNote()
			' This should be the case because otherwise the toolbar button is disabled:
			System.Diagnostics.Debug.Assert(m_CurrentItemIndex < m_ItemNodes.Count - 1)

			UpdateNote()

			m_CurrentItemIndex += 1

			DisplayCurrentNote()
		End Sub

		Private Sub DisplayPreviousNote()
			System.Diagnostics.Debug.Assert(m_CurrentItemIndex > 0)

			UpdateNote()

			m_CurrentItemIndex -= 1

			DisplayCurrentNote()
		End Sub

		Private Function ReadFeedFromURL(ByVal url As String) As Microsoft.Samples.FeedSync.Feed
			' Get the feed from URL. Passing in a string for the "i_Since" parameter will generate a 304
			' response from the GET request if the feed hasn't changed since the last time it was read.
			' If this is the case, then the incoming feed will be empty and does not need to be merged
			Dim strFeed As String = Microsoft.Samples.FeedSyncService.FeedManager.ReadFeedContents(url, m_Since)

			If strFeed Is Nothing Then
				Return Nothing
			End If

			' Create a new XML document instance for the incoming feed
			Dim xmlDoc As System.Xml.XmlDocument = New System.Xml.XmlDocument()
			xmlDoc.LoadXml(strFeed)

			' Create a Feed object to read and manipulate feed items
			Return Microsoft.Samples.FeedSync.Feed.Create(xmlDoc)
		End Function

		Private Sub SyncToIncomingFeed(ByVal incomingFeed As Microsoft.Samples.FeedSync.Feed)
			Dim refreshView As Boolean = False

			If incomingFeed IsNot Nothing Then
				' small usability hack: if there is only one item and it is empty, this
				' will replace the current feed with the incoming feed since the user
				' hasn't done any real work yet.
				If IsLocalFeedEmpty() Then
					m_Feed = incomingFeed
					' no need to publish anything. This effectively replaces the empty local feed with the incoming feed
					m_Published = True
				Else
					m_Feed = Microsoft.Samples.FeedSync.Feed.MergeFeeds(m_Feed, incomingFeed)
				End If
				refreshView = True
			End If

			' See if the feed needs to be published (have local changes)
			If (Not m_Published) Then
				' publish the modified feed back up to the service
				Microsoft.Samples.FeedSyncService.FeedManager.UpdateFeedContents (m_Feed.XmlDocument.OuterXml, m_FeedURL)

				m_Published = True
			End If

			If refreshView Then
				' get the node id of the current node to perserve current view
				Dim currentNodeID As String = CurrentItemNode.SyncNode.ID

				' load new feed
				LoadFeedContents()

				' display the note that was previously being displayed
				DisplayNoteByID(currentNodeID)
			End If
		End Sub

		Private Sub Synchronize()
			Try
				' save any new note text in the current note
				UpdateNote()

				If m_FeedURL IsNot Nothing Then
					' read feed from the service
					Dim incomingFeed As Microsoft.Samples.FeedSync.Feed = ReadFeedFromURL(m_FeedURL)
					SyncToIncomingFeed(incomingFeed)
				End If

				' write changes to disk
				SaveLocalFeed()
			Catch ex As Exception
				MessageBox.Show(Me, ex.Message, "Synchronization Error")
			End Try
		End Sub

		Private Sub CreateNote()
			Dim itemIndex As Integer = m_Feed.FeedItemNodes.Length

			Dim title As String = String.Format("Item #{0}", itemIndex)
			Dim ID As String = String.Format("Item_{0}.{1}", itemIndex, System.Guid.NewGuid().ToString())

			'  Create new FeedItemNode
			Dim feedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (m_Feed, title, "", ID, m_Username)

			'  Add FeedItemNode to feed
			m_Feed.AddFeedItem(feedItemNode)

			' And to our own list of nodes
			Dim itemNode As Microsoft.Samples.FeedSync.FeedItemNode = m_Feed.GetFeedItemNode(ID)
			m_ItemNodes.AddFeedItemNode(itemNode)

			' Remember that the feed has to be published
			m_Published = False

			' Display this new item, which is now the last one
			m_CurrentItemIndex = m_ItemNodes.Count - 1

			DisplayCurrentNote()
		End Sub

		Private Sub DeleteNote()
			' Delete the current item from the feed
			m_Feed.DeleteFeedItem(CurrentItemNode.SyncNode.ID, DateTime.Now, m_Username)

			' and from local list
			m_ItemNodes.RemoveAt(m_CurrentItemIndex)

			' remember that this change needs to be published
			m_Published = False

			' If the current note was the last one, rewind to the previous
			If m_CurrentItemIndex = m_ItemNodes.Count - 1 AndAlso m_CurrentItemIndex <> 0 Then
				m_CurrentItemIndex -= 1
			End If

			DisplayCurrentNote()
		End Sub

		Private Sub UpdateNote(ByVal force As Boolean)
			' see if the note actually changed contents
			If force OrElse textBox1.Text <> CurrentItemNode.Description Then
				' update the feed with new data
				m_Feed.UpdateFeedItem (CurrentItemNode.Title, textBox1.Text, CurrentItemNode.SyncNode.ID, System.DateTime.Now, m_Username, True)

				DisplayConflictStatus()

				' remember that the feed needs to be published now
				m_Published = False
			End If
		End Sub

		Private Sub UpdateNote()
			UpdateNote(False)
		End Sub

		#Region "Event handlers"

		Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			CreateOrOpenFeed()
		End Sub

		Private Sub MainForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
			' serialize changes to disk:
			SaveLocalFeed()

			' save settings:
			If (Not String.IsNullOrEmpty(m_FeedURL)) Then
				My.Settings.Default.FeedURL = m_FeedURL
			End If

			My.Settings.Default.Save()
		End Sub

		Private Sub toolStripButtonDeleteNote_Click(ByVal sender As Object, ByVal e As EventArgs) Handles toolStripButtonDeleteNote.Click
			DeleteNote()
		End Sub

		Private Sub toolStripButtonPreviousNote_Click(ByVal sender As Object, ByVal e As EventArgs) Handles toolStripButtonPreviousNote.Click
			DisplayPreviousNote()
		End Sub

		Private Sub toolStripButtonNextNote_Click(ByVal sender As Object, ByVal e As EventArgs) Handles toolStripButtonNextNote.Click
			DisplayNextNote()
		End Sub

		Private Sub toolStripButtonCreateNote_Click(ByVal sender As Object, ByVal e As EventArgs) Handles toolStripButtonCreateNote.Click
			UpdateNote()
			CreateNote()
		End Sub

		Private Sub textBox1_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles textBox1.TextChanged
			' reset the timer to wait before updating, since the user
			' is now typing in the current note
			timer1.Stop()

			If (Not String.IsNullOrEmpty(m_FeedURL)) Then
				timer1.Start()
			End If
		End Sub

		Private Sub textBox1_Validating(ByVal sender As Object, ByVal e As CancelEventArgs) Handles textBox1.Validating
			' this event will be fired when the focus changes
			' if the text has been altered, update the feed
			UpdateNote()
		End Sub

		Private Sub timer1_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles timer1.Tick
			Synchronize()
		End Sub

		Private Sub refreshToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles refreshToolStripMenuItem.Click
			' Perform a manual refresh, and restart the timer
			timer1.Stop()

			Synchronize()

			timer1.Start()
		End Sub

		Private Sub toolStripButtonConflict_Click(ByVal sender As Object, ByVal e As EventArgs) Handles toolStripButtonConflict.Click
			Dim resolveConflictsForm As ResolveConflictsForm = New ResolveConflictsForm()
			resolveConflictsForm.ConflictItem = CurrentItemNode

			' Turn off syncing while displaying the dialog:
			timer1.Stop()

			If resolveConflictsForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
				Dim resolvedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = resolveConflictsForm.ResolvedItem

				' Update view of current item. This will be updated the next time synchronization happens
				textBox1.Text = resolvedItemNode.Description

				' force update so that the conflict is resolved even if the user picks
				' the current item
				UpdateNote(True)
			End If

			timer1.Start()
		End Sub

		Private Sub syncToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles syncToolStripMenuItem.Click
			Dim enterFeedForm As EnterFeedForm = New EnterFeedForm()
			enterFeedForm.FeedURL = m_FeedURL
			enterFeedForm.FeedType = SHARED_NOTES_FEED_TYPE

			If enterFeedForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
				' Store the feed url that was entered
				m_FeedURL = enterFeedForm.FeedURL
				m_Since = enterFeedForm.Since

				' update any current item before synchronizing
				UpdateNote()

				' Synchronize with the feed that was entered
				Dim xmlDoc As System.Xml.XmlDocument = New System.Xml.XmlDocument()
				xmlDoc.LoadXml(enterFeedForm.FeedContents)
				SyncToIncomingFeed(Microsoft.Samples.FeedSync.Feed.Create(xmlDoc))

				' now that there is a feed to synchronize with, start a timer to do this periodically
				timer1.Start()
			End If
		End Sub

		Private Sub exitToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles exitToolStripMenuItem.Click
			Me.Close()
		End Sub

		#End Region
	End Class
End Namespace