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

Namespace Microsoft.Samples.FeedSync
	Public Class SyncNode
		Inherits Microsoft.Samples.FeedSync.Node
		Private m_ID As String

		Private m_Updates As Integer

		Private m_NoConflicts As Boolean = False
		Private m_Deleted As Boolean = False

		Private m_ConflictNodeList As System.Collections.Generic.SortedList(Of String, Microsoft.Samples.FeedSync.FeedItemNode) = New System.Collections.Generic.SortedList(Of String, Microsoft.Samples.FeedSync.FeedItemNode)()

		Private m_ConflictsNodeXmlElement As System.Xml.XmlElement = Nothing

		Private m_HistoryNodeList As System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.HistoryNode) = New System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.HistoryNode)()

		Private m_FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode) As Microsoft.Samples.FeedSync.SyncNode
			Return Microsoft.Samples.FeedSync.SyncNode.CreateNew (i_Feed, i_FeedItemNode, System.Guid.NewGuid().ToString(), 0, System.DateTime.Now, Nothing, False, False, 0)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode, ByVal i_SyncNodeID As String) As Microsoft.Samples.FeedSync.SyncNode
			Return Microsoft.Samples.FeedSync.SyncNode.CreateNew (i_Feed, i_FeedItemNode, i_SyncNodeID, 0, System.DateTime.Now, Nothing, False, False, 0)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_WhenDateTime As Nullable(Of System.DateTime), ByVal i_By As String, ByVal i_Deleted As Boolean, ByVal i_NoConflicts As Boolean, ByVal i_Updates As Integer) As Microsoft.Samples.FeedSync.SyncNode
			Dim ElementName As String = System.String.Format ("{0}:{1}", i_Feed.FeedSyncNamespacePrefix, Microsoft.Samples.FeedSync.Constants.SYNC_ELEMENT_NAME)

			Dim SyncNodeXmlElement As System.Xml.XmlElement = i_Feed.XmlDocument.CreateElement (ElementName, Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI)

			SyncNodeXmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.ID_ATTRIBUTE, i_SyncNodeID)

			SyncNodeXmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.UPDATES_ATTRIBUTE, i_Updates.ToString())

			If i_Deleted Then
				SyncNodeXmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.DELETED_ATTRIBUTE,"true")
			Else
				SyncNodeXmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.DELETED_ATTRIBUTE,"false")
			End If

			If i_NoConflicts Then
				SyncNodeXmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.NO_CONFLICTS_ATTRIBUTE,"true")
			Else
				SyncNodeXmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.NO_CONFLICTS_ATTRIBUTE,"false")
			End If

			Dim SyncNode As Microsoft.Samples.FeedSync.SyncNode = New Microsoft.Samples.FeedSync.SyncNode (i_FeedItemNode, SyncNodeXmlElement)

			Dim FeedSyncHistoryNode As Microsoft.Samples.FeedSync.HistoryNode = Microsoft.Samples.FeedSync.HistoryNode.CreateNew (SyncNode, i_WhenDateTime, i_By)

			SyncNode.AddHistoryNode(FeedSyncHistoryNode)

			Return SyncNode
		End Function

		Public Shared Function CreateFromXmlElement(ByVal i_FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode, ByVal i_SyncNodeXmlElement As System.Xml.XmlElement) As Microsoft.Samples.FeedSync.SyncNode
			If i_SyncNodeXmlElement.OwnerDocument IsNot i_FeedItemNode.Feed.XmlDocument Then
				i_SyncNodeXmlElement = CType(i_FeedItemNode.Feed.XmlDocument.ImportNode(i_SyncNodeXmlElement, True), System.Xml.XmlElement)
			End If

			Dim SyncNode As Microsoft.Samples.FeedSync.SyncNode = New Microsoft.Samples.FeedSync.SyncNode(i_FeedItemNode, i_SyncNodeXmlElement)
			Return SyncNode
		End Function

		Private Sub New(ByVal i_FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode, ByVal i_SyncNodeXmlElement As System.Xml.XmlElement)
			m_FeedItemNode = i_FeedItemNode
			m_XmlElement = i_SyncNodeXmlElement

			m_ID = m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.ID_ATTRIBUTE)
			m_Updates = System.Convert.ToInt32(m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.UPDATES_ATTRIBUTE))

			If m_XmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.DELETED_ATTRIBUTE) Then
				m_Deleted = (m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.DELETED_ATTRIBUTE) = "true")
			End If

			If m_XmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.NO_CONFLICTS_ATTRIBUTE) Then
				m_NoConflicts = (m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.NO_CONFLICTS_ATTRIBUTE) = "true")
			End If

			Dim XPathQuery As String = System.String.Format ("{0}:{1}", m_FeedItemNode.Feed.FeedSyncNamespacePrefix, Microsoft.Samples.FeedSync.Constants.HISTORY_ELEMENT_NAME)

			Dim HistoryNodeList As System.Xml.XmlNodeList = i_SyncNodeXmlElement.SelectNodes (XPathQuery, i_FeedItemNode.Feed.XmlNamespaceManager)

			For Each HistoryNodeXmlElement As System.Xml.XmlElement In HistoryNodeList
				Dim HistoryNode As Microsoft.Samples.FeedSync.HistoryNode = Microsoft.Samples.FeedSync.HistoryNode.CreateFromXmlElement (Me, HistoryNodeXmlElement)

				m_HistoryNodeList.Add(HistoryNode)
			Next HistoryNodeXmlElement

			If (Not m_NoConflicts) Then
				XPathQuery = System.String.Format ("{0}:{1}", m_FeedItemNode.Feed.FeedSyncNamespacePrefix, Microsoft.Samples.FeedSync.Constants.CONFLICTS_ELEMENT_NAME)

				m_ConflictsNodeXmlElement = CType(i_SyncNodeXmlElement.SelectSingleNode (XPathQuery, m_FeedItemNode.Feed.XmlNamespaceManager), System.Xml.XmlElement)

				If m_ConflictsNodeXmlElement IsNot Nothing Then
					Dim FeedItemXmlNodeList As System.Xml.XmlNodeList = m_ConflictsNodeXmlElement.SelectNodes (m_FeedItemNode.Feed.FeedItemXPathQuery, m_FeedItemNode.Feed.XmlNamespaceManager)

					For Each ConflictFeedItemNodeXmlElement As System.Xml.XmlElement In FeedItemXmlNodeList
						Dim ConflictFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = Me.CreateConflictItemNodeFromXmlElement(ConflictFeedItemNodeXmlElement)

						Dim Key As String = System.String.Format ("{0}{1}{2}", ConflictFeedItemNode.SyncNode.Updates, ConflictFeedItemNode.SyncNode.TopMostHistoryNode.Sequence, ConflictFeedItemNode.SyncNode.TopMostHistoryNode.By)

                        If ConflictFeedItemNode.SyncNode.TopMostHistoryNode.WhenDateTime.HasValue Then
                            Key &= (CDate(ConflictFeedItemNode.SyncNode.TopMostHistoryNode.WhenDateTime))
                        End If

						If (Not m_ConflictNodeList.ContainsKey(Key)) Then
							m_ConflictNodeList.Add(Key, ConflictFeedItemNode)
						End If
					Next ConflictFeedItemNodeXmlElement
				End If
			End If
		End Sub

		Public Sub RemoveAllConflictItemNodes()
			If m_ConflictNodeList.Count = 0 Then
				Return
			End If

			'  Delete "sx:conflicts" element
			m_ConflictsNodeXmlElement.ParentNode.RemoveChild(m_ConflictsNodeXmlElement)
			m_ConflictsNodeXmlElement = Nothing

			'  Empty node list
			m_ConflictNodeList.Clear()
		End Sub

		Public Function CreateConflictItemNodeFromXmlElement(ByVal i_ConflictFeedItemNodeXmlElement As System.Xml.XmlElement) As Microsoft.Samples.FeedSync.FeedItemNode
			Dim ConflictFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = Microsoft.Samples.FeedSync.FeedItemNode.CreateFromXmlElement (m_FeedItemNode.Feed, i_ConflictFeedItemNodeXmlElement)

			Return ConflictFeedItemNode
		End Function

		Public Sub AddConflictItemNode(ByVal i_ConflictFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode)
			'  Create clone of item in case it's from different document
			Dim ImportedConflictFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = m_FeedItemNode.Feed.ImportFeedItemNode(i_ConflictFeedItemNode)

			Dim Key As String = System.String.Format ("{0}{1}{2}", ImportedConflictFeedItemNode.SyncNode.Updates, ImportedConflictFeedItemNode.SyncNode.TopMostHistoryNode.Sequence, ImportedConflictFeedItemNode.SyncNode.TopMostHistoryNode.By)

            If ImportedConflictFeedItemNode.SyncNode.TopMostHistoryNode.WhenDateTime.HasValue Then
                Key &= (CDate(ImportedConflictFeedItemNode.SyncNode.TopMostHistoryNode.WhenDateTime))
            End If

			'  Check if if item already exists in either list
			If m_ConflictNodeList.ContainsKey(Key) Then
				If m_ConflictNodeList.ContainsKey(Key) Then
					Throw New System.ArgumentException("SyncNode::AddConflictItemNode (" & Key & ") - item already exists as conflict")
				End If
			End If

			'  Create "sx:conflicts" element if necessary
			If m_ConflictsNodeXmlElement Is Nothing Then
				Dim ElementName As String = System.String.Format ("{0}:{1}", m_FeedItemNode.Feed.FeedSyncNamespacePrefix, Microsoft.Samples.FeedSync.Constants.CONFLICTS_ELEMENT_NAME)

				m_ConflictsNodeXmlElement = m_FeedItemNode.Feed.XmlDocument.CreateElement (ElementName, Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI)

				m_XmlElement.AppendChild(m_ConflictsNodeXmlElement)
			End If

			'  Append conflict node's element to "sx:conflicts" element
			m_ConflictsNodeXmlElement.AppendChild(ImportedConflictFeedItemNode.XmlElement)

			'  Add node to list
			m_ConflictNodeList.Add(Key, ImportedConflictFeedItemNode)
		End Sub

		Public Function DoesConflictItemNodeExist(ByVal i_FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode) As Boolean
			Dim Key As String = System.String.Format ("{0}{1}{2}", i_FeedItemNode.SyncNode.Updates, i_FeedItemNode.SyncNode.TopMostHistoryNode.Sequence, i_FeedItemNode.SyncNode.TopMostHistoryNode.By)

            If i_FeedItemNode.SyncNode.TopMostHistoryNode.WhenDateTime.HasValue Then
                Key &= (CDate(i_FeedItemNode.SyncNode.TopMostHistoryNode.WhenDateTime))
            End If

			 Return (m_ConflictNodeList.ContainsKey(Key))
		End Function

		Public ReadOnly Property ID() As String
			Get
				Return m_ID
			End Get
		End Property

		Public Property Updates() As Integer
			Get
				Return m_Updates
			End Get
			Set(ByVal value As Integer)
				If value < m_Updates Then
					Throw New System.ArgumentException("Invalid value!")
				End If

				m_XmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.UPDATES_ATTRIBUTE, value.ToString())

				m_Updates = value
			End Set
		End Property

		Public ReadOnly Property NoConflicts() As Boolean
			Get
				Return m_NoConflicts
			End Get
		End Property

		Public Property Deleted() As Boolean
			Get
				Return m_Deleted
			End Get
			Set(ByVal value As Boolean)
				If value = m_Deleted Then
					Return
				End If

				If value Then
					m_XmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.DELETED_ATTRIBUTE,"true")
				Else
					m_XmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.DELETED_ATTRIBUTE,"false")
				End If

				m_Deleted = value
			End Set
		End Property

		Public ReadOnly Property TopMostHistoryNode() As Microsoft.Samples.FeedSync.HistoryNode
			Get
				If m_HistoryNodeList.Count = 0 Then
					Return Nothing
				End If

				Return m_HistoryNodeList(0)
			End Get
		End Property

		Public ReadOnly Property HistoryNodes() As Microsoft.Samples.FeedSync.HistoryNode()
			Get
				Return m_HistoryNodeList.ToArray()
			End Get
		End Property

		Public ReadOnly Property ConflictFeedItemNodes() As Microsoft.Samples.FeedSync.FeedItemNode()
			Get
				Dim ConflictFeedItemNodeList As System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.FeedItemNode) = New System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.FeedItemNode)(m_ConflictNodeList.Values)
				Return ConflictFeedItemNodeList.ToArray()
			End Get
		End Property

		Public ReadOnly Property FeedItemNode() As Microsoft.Samples.FeedSync.FeedItemNode
			Get
				Return m_FeedItemNode
			End Get
		End Property

		Friend Sub AddHistoryNode(ByVal i_HistoryNode As Microsoft.Samples.FeedSync.HistoryNode)
			Dim ImportedHistoryXmlElement As System.Xml.XmlElement = CType(m_FeedItemNode.Feed.XmlDocument.ImportNode (i_HistoryNode.XmlElement, True), System.Xml.XmlElement)

			Dim ImportedHistoryNode As Microsoft.Samples.FeedSync.HistoryNode = Microsoft.Samples.FeedSync.HistoryNode.CreateFromXmlElement (Me, ImportedHistoryXmlElement)

			If m_XmlElement.ChildNodes.Count > 0 Then
				m_XmlElement.InsertBefore (ImportedHistoryNode.XmlElement, m_XmlElement.ChildNodes(0))
			Else
				m_XmlElement.AppendChild(ImportedHistoryNode.XmlElement)
			End If

			'  Make sure new history node is first node in list
			m_HistoryNodeList.Insert (0, ImportedHistoryNode)

			'  Remember not to use m_Updates here because
			'  property accessor modifies xml element
			Me.Updates = i_HistoryNode.Sequence
		End Sub

		Friend Sub AddConflictHistoryNode(ByVal i_ConflictHistoryNode As Microsoft.Samples.FeedSync.HistoryNode)
			Dim ImportedConflictHistoryXmlElement As System.Xml.XmlElement = CType(m_FeedItemNode.Feed.XmlDocument.ImportNode (i_ConflictHistoryNode.XmlElement, True), System.Xml.XmlElement)

			Dim ImportedConflictHistoryNode As Microsoft.Samples.FeedSync.HistoryNode = Microsoft.Samples.FeedSync.HistoryNode.CreateFromXmlElement (Me, ImportedConflictHistoryXmlElement)

			'  Insert after topmost history element
			m_XmlElement.InsertBefore (ImportedConflictHistoryNode.XmlElement, Me.TopMostHistoryNode.XmlElement.NextSibling)

			m_HistoryNodeList.Add(ImportedConflictHistoryNode)
		End Sub
	End Class
End Namespace
