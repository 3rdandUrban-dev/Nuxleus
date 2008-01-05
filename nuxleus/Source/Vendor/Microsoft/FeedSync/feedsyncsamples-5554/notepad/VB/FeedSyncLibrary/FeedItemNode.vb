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
	Public Class FeedItemNode
		Inherits Microsoft.Samples.FeedSync.Node
		Private m_Title As String
		Private m_Description As String
		Private m_ID As String

		Private m_Feed As Microsoft.Samples.FeedSync.Feed
		Private m_SyncNode As Microsoft.Samples.FeedSync.SyncNode

		Public Enum CompareResult
			ItemNode1Newer = 1
			ItemNode2Newer = 2
			ItemNodesEqual = 3
			ItemNodesEqual_DifferentConflictData = 4
		End Enum

		#Region "CreateFromXmlElement methods"

		Public Shared Function CreateFromXmlElement(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_ItemXmlElement As System.Xml.XmlElement) As Microsoft.Samples.FeedSync.FeedItemNode
			If i_ItemXmlElement.OwnerDocument IsNot i_Feed.XmlDocument Then
				i_ItemXmlElement = CType(i_Feed.XmlDocument.ImportNode(i_ItemXmlElement, True), System.Xml.XmlElement)
			End If

			Dim FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = New Microsoft.Samples.FeedSync.FeedItemNode (i_Feed, i_ItemXmlElement)

			Return FeedItemNode
		End Function

		#End Region

		#Region "CreateNewFromXmlElement methods"

		Public Shared Function CreateNewFromXmlElement(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_ItemXmlElement As System.Xml.XmlElement, ByVal i_SyncNodeID As String) As Microsoft.Samples.FeedSync.FeedItemNode
			'  First create an element that works in this document
			Dim ImportedItemXmlElement As System.Xml.XmlElement = CType(i_Feed.XmlDocument.ImportNode(i_ItemXmlElement, True), System.Xml.XmlElement)

			Dim FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = New Microsoft.Samples.FeedSync.FeedItemNode (i_Feed, ImportedItemXmlElement, i_SyncNodeID)

			Return FeedItemNode
		End Function

		Public Shared Function CreateNewFromXmlElement(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_ItemXmlElement As System.Xml.XmlElement, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_WhenDateTime As Nullable(Of System.DateTime), ByVal i_By As String, ByVal i_Deleted As Boolean, ByVal i_NoConflicts As Boolean, ByVal i_Updates As Integer) As Microsoft.Samples.FeedSync.FeedItemNode
			'  First create an element that works in this document
			Dim ImportedItemXmlElement As System.Xml.XmlElement = CType(i_Feed.XmlDocument.ImportNode(i_ItemXmlElement, True), System.Xml.XmlElement)

			Dim FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = New Microsoft.Samples.FeedSync.FeedItemNode (i_Feed, ImportedItemXmlElement, i_SyncNodeID, i_Sequence, i_WhenDateTime, i_By, i_Deleted, i_NoConflicts, i_Updates)

			Return FeedItemNode
		End Function

		#End Region

		#Region "CreateNew methods"

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Title As String, ByVal i_Description As String, ByVal i_SyncNodeID As String, ByVal i_By As String) As Microsoft.Samples.FeedSync.FeedItemNode
			Return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (i_Feed, i_Title, i_Description, Nothing, i_SyncNodeID, 1, System.DateTime.Now, i_By, False, False, 0)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Title As String, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_When As String, ByVal i_By As String) As Microsoft.Samples.FeedSync.FeedItemNode
			Return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (i_Feed, i_Title, Nothing, Nothing, i_SyncNodeID, i_Sequence, System.Convert.ToDateTime(i_When), i_By, False, False, 0)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Title As String, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_When As String, ByVal i_By As String, ByVal i_Deleted As Boolean, ByVal i_NoConflicts As Boolean) As Microsoft.Samples.FeedSync.FeedItemNode
			Return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (i_Feed, i_Title, Nothing, Nothing, i_SyncNodeID, i_Sequence, System.Convert.ToDateTime(i_When), i_By, i_Deleted, i_NoConflicts, 0)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Title As String, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_WhenDateTime As Nullable(Of System.DateTime), ByVal i_By As String) As Microsoft.Samples.FeedSync.FeedItemNode
			Return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (i_Feed, i_Title, Nothing, Nothing, i_SyncNodeID, i_Sequence, i_WhenDateTime, i_By, False, False, 0)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Title As String, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_WhenDateTime As Nullable(Of System.DateTime), ByVal i_By As String, ByVal i_Deleted As Boolean, ByVal i_NoConflicts As Boolean) As Microsoft.Samples.FeedSync.FeedItemNode
			Return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (i_Feed, i_Title, Nothing, Nothing, i_SyncNodeID, i_Sequence, i_WhenDateTime, i_By, i_Deleted, i_NoConflicts, 0)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Title As String, ByVal i_ID As String, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_When As String, ByVal i_By As String) As Microsoft.Samples.FeedSync.FeedItemNode
			Return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (i_Feed, i_Title, Nothing, i_ID, i_SyncNodeID, i_Sequence, System.Convert.ToDateTime(i_When), i_By, False, False, 0)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Title As String, ByVal i_ID As String, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_When As String, ByVal i_By As String, ByVal i_Deleted As Boolean, ByVal i_NoConflicts As Boolean) As Microsoft.Samples.FeedSync.FeedItemNode
			Return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (i_Feed, i_Title, Nothing, i_ID, i_SyncNodeID, i_Sequence, System.Convert.ToDateTime(i_When), i_By, i_Deleted, i_NoConflicts, 0)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Title As String, ByVal i_ID As String, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_WhenDateTime As Nullable(Of System.DateTime), ByVal i_By As String) As Microsoft.Samples.FeedSync.FeedItemNode
			Return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (i_Feed, i_Title, Nothing, i_ID, i_SyncNodeID, i_Sequence, i_WhenDateTime, i_By, False, False, 0)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Title As String, ByVal i_ID As String, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_WhenDateTime As Nullable(Of System.DateTime), ByVal i_By As String, ByVal i_Deleted As Boolean, ByVal i_NoConflicts As Boolean) As Microsoft.Samples.FeedSync.FeedItemNode
			Return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (i_Feed, i_Title, Nothing, i_ID, i_SyncNodeID, i_Sequence, i_WhenDateTime, i_By, i_Deleted, i_NoConflicts, 0)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Title As String, ByVal i_Description As String, ByVal i_ID As String, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_When As String, ByVal i_By As String) As Microsoft.Samples.FeedSync.FeedItemNode
			Return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (i_Feed, i_Title, i_Description, i_ID, i_SyncNodeID, i_Sequence, System.Convert.ToDateTime(i_When), i_By, False, False, 0)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Title As String, ByVal i_Description As String, ByVal i_ID As String, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_When As String, ByVal i_By As String, ByVal i_Deleted As Boolean, ByVal i_NoConflicts As Boolean) As Microsoft.Samples.FeedSync.FeedItemNode
			Return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (i_Feed, i_Title, i_Description, i_ID, i_SyncNodeID, i_Sequence, System.Convert.ToDateTime(i_When), i_By, i_Deleted, i_NoConflicts, 0)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Title As String, ByVal i_Description As String, ByVal i_ID As String, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_WhenDateTime As Nullable(Of System.DateTime), ByVal i_By As String, ByVal i_Deleted As Boolean, ByVal i_NoConflicts As Boolean, ByVal i_Updates As Integer) As Microsoft.Samples.FeedSync.FeedItemNode
			Dim FeedItemXmlElement As System.Xml.XmlElement

			If i_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				FeedItemXmlElement = i_Feed.XmlDocument.CreateElement (i_Feed.FeedItemElementName, Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
			Else
				FeedItemXmlElement = i_Feed.XmlDocument.CreateElement(i_Feed.FeedItemElementName)
			End If

'			#Region "Create feed item title element"

			Dim TitleXmlElement As System.Xml.XmlElement = Nothing

			If i_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				Dim ElementName As String

				If i_Feed.XmlNamespaceManager.DefaultNamespace = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI Then
					ElementName = Microsoft.Samples.FeedSync.Constants.ATOM_TITLE_ELEMENT_NAME
				Else
					ElementName = System.String.Format ("{0}:{1}", i_Feed.AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_TITLE_ELEMENT_NAME)
				End If

				TitleXmlElement = i_Feed.XmlDocument.CreateElement (ElementName, Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
			Else
				TitleXmlElement = i_Feed.XmlDocument.CreateElement(Microsoft.Samples.FeedSync.Constants.RSS_TITLE_ELEMENT_NAME)
			End If

			FeedItemXmlElement.AppendChild(TitleXmlElement)
			TitleXmlElement.InnerText = i_Title

'			#End Region

'			#Region "Create feed item description element"

			Dim DescriptionXmlElement As System.Xml.XmlElement = Nothing

			If i_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				Dim ElementName As String

				If i_Feed.XmlNamespaceManager.DefaultNamespace = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI Then
					ElementName = Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_DESCRIPTION_ELEMENT_NAME
				Else
					ElementName = System.String.Format ("{0}:{1}", i_Feed.AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_DESCRIPTION_ELEMENT_NAME)
				End If

				DescriptionXmlElement = i_Feed.XmlDocument.CreateElement (ElementName, Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
			Else
				DescriptionXmlElement = i_Feed.XmlDocument.CreateElement(Microsoft.Samples.FeedSync.Constants.RSS_DESCRIPTION_ELEMENT_NAME)
			End If

			FeedItemXmlElement.AppendChild(DescriptionXmlElement)
			DescriptionXmlElement.InnerText = i_Description

'			#End Region

'			#Region "Create feed item id element"

			Dim IDXmlElement As System.Xml.XmlElement = Nothing

			If (Not System.String.IsNullOrEmpty(i_ID)) OrElse (i_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom) Then
				If i_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
					If System.String.IsNullOrEmpty(i_ID) Then
						i_ID = "urn:uuid:" & System.Guid.NewGuid().ToString()
					End If

					Dim ElementName As String

					If i_Feed.XmlNamespaceManager.DefaultNamespace = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI Then
						ElementName = Microsoft.Samples.FeedSync.Constants.ATOM_ID_ELEMENT_NAME
					Else
						ElementName = System.String.Format ("{0}:{1}", i_Feed.AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_ID_ELEMENT_NAME)
					End If

					IDXmlElement = i_Feed.XmlDocument.CreateElement (ElementName, Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
				Else
					IDXmlElement = i_Feed.XmlDocument.CreateElement(Microsoft.Samples.FeedSync.Constants.RSS_ID_ELEMENT_NAME)
				End If
			End If

			If IDXmlElement IsNot Nothing Then
				FeedItemXmlElement.AppendChild(IDXmlElement)
				IDXmlElement.InnerText = i_ID
			End If

'			#End Region

			Dim FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = Microsoft.Samples.FeedSync.FeedItemNode.CreateNewFromXmlElement (i_Feed, FeedItemXmlElement, i_SyncNodeID, i_Sequence, i_WhenDateTime, i_By, i_Deleted, i_NoConflicts, i_Updates)

			Return FeedItemNode
		End Function

		#End Region

		#Region "MergeFeedItems methods"

		Public Shared Function MergeFeedItems(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_LocalFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode, ByVal i_IncomingFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode) As Microsoft.Samples.FeedSync.FeedItemNode
			Dim ImportedLocalFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = i_Feed.ImportFeedItemNode(i_LocalFeedItemNode)
			Dim ImportedIncomingFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = i_Feed.ImportFeedItemNode(i_IncomingFeedItemNode)

			Dim LocalFeedItemNodeList As System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.FeedItemNode) = New System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.FeedItemNode)()
			LocalFeedItemNodeList.Add(ImportedLocalFeedItemNode)

			Dim IncomingFeedItemNodeList As System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.FeedItemNode) = New System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.FeedItemNode)()
			IncomingFeedItemNodeList.Add(ImportedIncomingFeedItemNode)

			System.Diagnostics.Debug.Assert(ImportedLocalFeedItemNode.SyncNode.NoConflicts = ImportedIncomingFeedItemNode.SyncNode.NoConflicts)

			'  Perform conflict feed item processing (if necessary)
			If (Not ImportedLocalFeedItemNode.SyncNode.NoConflicts) Then
				LocalFeedItemNodeList.AddRange(ImportedLocalFeedItemNode.SyncNode.ConflictFeedItemNodes)
				IncomingFeedItemNodeList.AddRange(ImportedIncomingFeedItemNode.SyncNode.ConflictFeedItemNodes)
			End If

			ImportedLocalFeedItemNode.SyncNode.RemoveAllConflictItemNodes()
			ImportedIncomingFeedItemNode.SyncNode.RemoveAllConflictItemNodes()

			Dim WinnerFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = Nothing
			Dim MergedFeedItemNodeList As System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.FeedItemNode) = New System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.FeedItemNode)()

'			#Region "Process local feeditem node list"

			'  Iterate local items first, looking for subsumption
			For Each LocalFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode In LocalFeedItemNodeList.ToArray()
				Dim Subsumed As Boolean = False

				For Each IncomingFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode In IncomingFeedItemNodeList.ToArray()
					If LocalFeedItemNode.IsSubsumedByFeedItemNode(IncomingFeedItemNode) Then
						Subsumed = True
						Exit For
					End If
				Next IncomingFeedItemNode

				If Subsumed Then
					LocalFeedItemNodeList.Remove(LocalFeedItemNode)
					Continue For
				End If

				MergedFeedItemNodeList.Add(LocalFeedItemNode)

				If WinnerFeedItemNode Is Nothing Then
					WinnerFeedItemNode = LocalFeedItemNode
				Else
					Dim CompareResult As Microsoft.Samples.FeedSync.FeedItemNode.CompareResult = Microsoft.Samples.FeedSync.FeedItemNode.CompareFeedItems (WinnerFeedItemNode, LocalFeedItemNode)

					If Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer <> CompareResult Then
						WinnerFeedItemNode = LocalFeedItemNode
					End If
				End If
			Next LocalFeedItemNode

'			#End Region

'			#Region "Process incoming feeditem node list"

			'  Iterate incoming items next, looking for subsumption
			For Each IncomingFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode In IncomingFeedItemNodeList.ToArray()
				Dim Subsumed As Boolean = False

				For Each LocalFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode In LocalFeedItemNodeList.ToArray()
					If IncomingFeedItemNode.IsSubsumedByFeedItemNode(LocalFeedItemNode) Then
						Subsumed = True
						Exit For
					End If
				Next LocalFeedItemNode

				If Subsumed Then
					IncomingFeedItemNodeList.Remove(IncomingFeedItemNode)
					Continue For
				End If

				MergedFeedItemNodeList.Add(IncomingFeedItemNode)

				If WinnerFeedItemNode Is Nothing Then
					WinnerFeedItemNode = IncomingFeedItemNode
				Else
					Dim CompareResult As Microsoft.Samples.FeedSync.FeedItemNode.CompareResult = Microsoft.Samples.FeedSync.FeedItemNode.CompareFeedItems (WinnerFeedItemNode, IncomingFeedItemNode)

					If Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer <> CompareResult Then
						WinnerFeedItemNode = IncomingFeedItemNode
					End If
				End If
			Next IncomingFeedItemNode

'			#End Region

			For Each ConflictFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode In MergedFeedItemNodeList
				If WinnerFeedItemNode IsNot ConflictFeedItemNode Then
					WinnerFeedItemNode.SyncNode.AddConflictItemNode(ConflictFeedItemNode)
				End If
			Next ConflictFeedItemNode

			Return WinnerFeedItemNode
		End Function

		#End Region

		Public Shared Function CompareFeedItems(ByVal i_FeedItemNode1 As Microsoft.Samples.FeedSync.FeedItemNode, ByVal i_FeedItemNode2 As Microsoft.Samples.FeedSync.FeedItemNode) As Microsoft.Samples.FeedSync.FeedItemNode.CompareResult
			Dim SyncNode1 As Microsoft.Samples.FeedSync.SyncNode = i_FeedItemNode1.SyncNode
			Dim SyncNode2 As Microsoft.Samples.FeedSync.SyncNode = i_FeedItemNode2.SyncNode

			'  If SyncNode1 has higher version, it is newer
			If SyncNode1.Updates> SyncNode2.Updates Then
				Return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer
			End If

			'  If SyncNode2 has higher version, it is newer
			If SyncNode2.Updates> SyncNode1.Updates Then
				Return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode2Newer
			End If

			Dim FeedSyncHistoryNode1 As Microsoft.Samples.FeedSync.HistoryNode = SyncNode1.TopMostHistoryNode
			Dim FeedSyncHistoryNode2 As Microsoft.Samples.FeedSync.HistoryNode = SyncNode2.TopMostHistoryNode

			'  Compare by "when" attribute
            If (FeedSyncHistoryNode1.WhenDateTime.HasValue) AndAlso (Not FeedSyncHistoryNode2.WhenDateTime.HasValue) Then
                Return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer
            End If

            If (FeedSyncHistoryNode1.WhenDateTime.HasValue) AndAlso (FeedSyncHistoryNode2.WhenDateTime.HasValue) Then
                If FeedSyncHistoryNode1.WhenDateTime.Value > FeedSyncHistoryNode2.WhenDateTime.Value Then
                    Return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer
                End If

                If FeedSyncHistoryNode1.WhenDateTime.Equals(FeedSyncHistoryNode2.WhenDateTime) Then
                    If (Not System.String.IsNullOrEmpty(FeedSyncHistoryNode1.By)) AndAlso System.String.IsNullOrEmpty(FeedSyncHistoryNode2.By) Then
                        Return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer
                    End If

                    If (Not System.String.IsNullOrEmpty(FeedSyncHistoryNode1.By)) AndAlso (Not System.String.IsNullOrEmpty(FeedSyncHistoryNode2.By)) Then
                        If FeedSyncHistoryNode1.By.CompareTo(FeedSyncHistoryNode2.By) > 0 Then
                            Return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer
                        End If
                    End If
                End If
            End If

			Dim ConflictFeedItemNodes1() As Microsoft.Samples.FeedSync.FeedItemNode = i_FeedItemNode1.SyncNode.ConflictFeedItemNodes
			Dim ConflictFeedItemNodes2() As Microsoft.Samples.FeedSync.FeedItemNode = i_FeedItemNode2.SyncNode.ConflictFeedItemNodes

			'  Compare conflict items
			If ConflictFeedItemNodes1.Length <> ConflictFeedItemNodes2.Length Then
				Return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNodesEqual_DifferentConflictData
			End If

			If ConflictFeedItemNodes1.Length > 0 Then
				For Each ConflictFeedItemNode1 As Microsoft.Samples.FeedSync.FeedItemNode In ConflictFeedItemNodes1
					Dim MatchingConflictItem As Boolean = False

					For Each ConflictFeedItemNode2 As Microsoft.Samples.FeedSync.FeedItemNode In ConflictFeedItemNodes2
						Dim CompareResult As Microsoft.Samples.FeedSync.FeedItemNode.CompareResult = Microsoft.Samples.FeedSync.FeedItemNode.CompareFeedItems (ConflictFeedItemNode1, ConflictFeedItemNode2)

						If Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNodesEqual = CompareResult Then
							MatchingConflictItem = True
							Exit For
						End If
					Next ConflictFeedItemNode2

					If (Not MatchingConflictItem) Then
						Return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNodesEqual_DifferentConflictData
					End If
				Next ConflictFeedItemNode1
			End If

			Return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNodesEqual
		End Function

		#Region "constructors"

		Private Sub New(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_FeedItemXmlElement As System.Xml.XmlElement)
			m_Feed = i_Feed

			Me.GetFeedItemDataFromXmlElement(i_FeedItemXmlElement)

			m_XmlElement = i_FeedItemXmlElement

'			#Region "Get sx:sync element"

			Dim ElementName As String = System.String.Format ("{0}:{1}", i_Feed.FeedSyncNamespacePrefix, Microsoft.Samples.FeedSync.Constants.SYNC_ELEMENT_NAME)

			Dim SyncNodeXmlElement As System.Xml.XmlElement = CType(m_XmlElement.SelectSingleNode (ElementName, i_Feed.XmlNamespaceManager), System.Xml.XmlElement)

			If SyncNodeXmlElement Is Nothing Then
				Throw New System.ArgumentException("Item is missing 'sx:sync' element!")
			End If

			m_SyncNode = Microsoft.Samples.FeedSync.SyncNode.CreateFromXmlElement(Me, SyncNodeXmlElement)

'			#End Region
		End Sub

		Private Sub New(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_FeedItemXmlElement As System.Xml.XmlElement, ByVal i_SyncNodeID As String)
			m_Feed = i_Feed

			Me.GetFeedItemDataFromXmlElement(i_FeedItemXmlElement)

			m_XmlElement = i_FeedItemXmlElement

			m_SyncNode = Microsoft.Samples.FeedSync.SyncNode.CreateNew (m_Feed, Me, i_SyncNodeID)

			If m_XmlElement.ChildNodes.Count > 0 Then
				m_XmlElement.InsertBefore(m_SyncNode.XmlElement, m_XmlElement.ChildNodes(0))
			Else
				m_XmlElement.AppendChild(m_SyncNode.XmlElement)
			End If
		End Sub

		Private Sub New(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_FeedItemXmlElement As System.Xml.XmlElement, ByVal i_SyncNodeID As String, ByVal i_Sequence As Integer, ByVal i_WhenDateTime As Nullable(Of System.DateTime), ByVal i_By As String, ByVal i_Deleted As Boolean, ByVal i_NoConflicts As Boolean, ByVal i_Updates As Integer)
			m_Feed = i_Feed

			Me.GetFeedItemDataFromXmlElement(i_FeedItemXmlElement)

			m_XmlElement = i_FeedItemXmlElement

			m_SyncNode = Microsoft.Samples.FeedSync.SyncNode.CreateNew (m_Feed, Me, i_SyncNodeID, i_Sequence, i_WhenDateTime, i_By, i_Deleted, i_NoConflicts, i_Updates)

			If m_XmlElement.ChildNodes.Count > 0 Then
				m_XmlElement.InsertBefore(m_SyncNode.XmlElement, m_XmlElement.ChildNodes(0))
			Else
				m_XmlElement.AppendChild(m_SyncNode.XmlElement)
			End If
			If m_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				Me.AddUpdatedElement()
			End If
		End Sub

		#End Region

		Private Sub GetFeedItemDataFromXmlElement(ByVal i_XmlElement As System.Xml.XmlElement)
			Dim ElementName As String

'			#Region "Validate feed item element"

			If m_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				ElementName = Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME
			Else
				ElementName = Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_ELEMENT_NAME
			End If

			If i_XmlElement.LocalName <> ElementName Then
				Throw New System.ArgumentException("Invalid feed item xml element!")
			End If

'			#End Region

'			#Region "Get feed item title element"

			If m_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				ElementName = System.String.Format ("{0}:{1}", m_Feed.AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_TITLE_ELEMENT_NAME)
			Else
				ElementName = Microsoft.Samples.FeedSync.Constants.RSS_TITLE_ELEMENT_NAME
			End If

			Dim TitleXmlNode As System.Xml.XmlNode = i_XmlElement.SelectSingleNode (ElementName, m_Feed.XmlNamespaceManager)

			If TitleXmlNode IsNot Nothing Then
				m_Title = TitleXmlNode.InnerText
			End If

'			#End Region

'			#Region "Get feed item description element"

			If m_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				ElementName = System.String.Format ("{0}:{1}", m_Feed.AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_DESCRIPTION_ELEMENT_NAME)
			Else
				ElementName = Microsoft.Samples.FeedSync.Constants.RSS_DESCRIPTION_ELEMENT_NAME
			End If

			Dim DescriptionXmlNode As System.Xml.XmlNode = i_XmlElement.SelectSingleNode (ElementName, m_Feed.XmlNamespaceManager)

			If DescriptionXmlNode IsNot Nothing Then
				m_Description = DescriptionXmlNode.InnerText
			End If

'			#End Region

'			#Region "Get feed item id element"

			If m_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				ElementName = System.String.Format ("{0}:{1}", m_Feed.AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_ID_ELEMENT_NAME)
			Else
				ElementName = Microsoft.Samples.FeedSync.Constants.RSS_ID_ELEMENT_NAME
			End If

			Dim IDXmlNode As System.Xml.XmlNode = i_XmlElement.SelectSingleNode (ElementName, m_Feed.XmlNamespaceManager)

			If IDXmlNode IsNot Nothing Then
				m_ID = IDXmlNode.InnerText
			End If

'			#End Region
		End Sub

		Public Function Delete(ByVal i_WhenDateTime As System.DateTime, ByVal i_By As String) As Boolean
			Dim HistoryNode As Microsoft.Samples.FeedSync.HistoryNode = Microsoft.Samples.FeedSync.HistoryNode.CreateNew (m_SyncNode, i_WhenDateTime, i_By)

			m_SyncNode.AddHistoryNode(HistoryNode)
			m_SyncNode.Deleted = True

			If m_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				Me.AddUpdatedElement()
			End If

			'  Indicate successful deletion
			Return True
		End Function

		Public Function Update(ByVal i_Title As String, ByVal i_Description As String, ByVal i_WhenDateTime As System.DateTime, ByVal i_By As String, ByVal i_ResolveConflicts As Boolean) As Boolean
			Dim HistoryNode As Microsoft.Samples.FeedSync.HistoryNode = Microsoft.Samples.FeedSync.HistoryNode.CreateNew (m_SyncNode, i_WhenDateTime, i_By)

			m_SyncNode.AddHistoryNode(HistoryNode)

			Me.Title = i_Title
			Me.Description = i_Description

			If m_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				Me.AddUpdatedElement()
			End If

			Dim IgnoreConflictProcessing As Boolean = m_SyncNode.NoConflicts OrElse (Not i_ResolveConflicts) OrElse (m_SyncNode.ConflictFeedItemNodes.Length = 0)

			If IgnoreConflictProcessing Then
				Return True
			End If

			'  *********************************************************************************
			'  BIG HONKING NOTE:  This code resolves all conflicts and does not accomodate for
			'                     selective conflict resolution
			'  *********************************************************************************

			For Each ConflictFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode In m_SyncNode.ConflictFeedItemNodes
				For Each ConflictHistoryNode As Microsoft.Samples.FeedSync.HistoryNode In ConflictFeedItemNode.SyncNode.HistoryNodes
					Dim Subsumed As Boolean = False

					For Each MainHistoryNode As Microsoft.Samples.FeedSync.HistoryNode In m_SyncNode.HistoryNodes
						If ConflictHistoryNode.IsSubsumedByHistoryNode(MainHistoryNode) Then
							Subsumed = True
							Exit For
						End If
					Next MainHistoryNode

					If (Not Subsumed) Then
						m_SyncNode.AddConflictHistoryNode(ConflictHistoryNode)
					End If
				Next ConflictHistoryNode
			Next ConflictFeedItemNode

			m_SyncNode.RemoveAllConflictItemNodes()

			'  Indicate successful update
			Return True
		End Function

		Public Property Title() As String
			Get
				Return m_Title
			End Get
			Set(ByVal value As String)
				If value Is Nothing Then
					Throw New System.ArgumentException("Must assign non-null value for title")
				End If

				Dim ElementName As String

				If m_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
					ElementName = System.String.Format ("{0}:{1}", m_Feed.AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_TITLE_ELEMENT_NAME)
				Else
					ElementName = Microsoft.Samples.FeedSync.Constants.RSS_TITLE_ELEMENT_NAME
				End If

				'  Get reference to title xml node
				Dim TitleXmlNode As System.Xml.XmlNode = m_XmlElement.SelectSingleNode (ElementName, m_Feed.XmlNamespaceManager)

				If TitleXmlNode Is Nothing Then
					If m_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
						TitleXmlNode = m_XmlElement.OwnerDocument.CreateElement (ElementName, Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
					Else
						TitleXmlNode = m_XmlElement.OwnerDocument.CreateElement(ElementName)
					End If

					m_XmlElement.AppendChild(TitleXmlNode)
				End If

				TitleXmlNode.InnerText = value
				m_Title = value
			End Set
		End Property

		Public Property Description() As String
			Get
				Return m_Description
			End Get
			Set(ByVal value As String)
				If value Is Nothing Then
					Throw New System.ArgumentException("Must assign non-null value for description")
				End If

				Dim ElementName As String

				If m_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
					ElementName = System.String.Format ("{0}:{1}", m_Feed.AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_DESCRIPTION_ELEMENT_NAME)
				Else
					ElementName = Microsoft.Samples.FeedSync.Constants.RSS_DESCRIPTION_ELEMENT_NAME
				End If

				'  Get reference to description xml node
				Dim DescriptionXmlNode As System.Xml.XmlNode = m_XmlElement.SelectSingleNode (ElementName, m_Feed.XmlNamespaceManager)

				If DescriptionXmlNode Is Nothing Then
					If m_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
						DescriptionXmlNode = m_XmlElement.OwnerDocument.CreateElement (ElementName, Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
					Else
						DescriptionXmlNode = m_XmlElement.OwnerDocument.CreateElement(ElementName)
					End If

					m_XmlElement.AppendChild(DescriptionXmlNode)
				End If


				DescriptionXmlNode.InnerText = value
				m_Description = value
			End Set
		End Property

		Public Property ID() As String
			Get
				Return m_ID
			End Get
			Set(ByVal value As String)
				Dim ElementName As String

				If m_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
					ElementName = System.String.Format ("{0}:{1}", m_Feed.AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_ID_ELEMENT_NAME)
				Else
					ElementName = Microsoft.Samples.FeedSync.Constants.RSS_ID_ELEMENT_NAME
				End If

				'  Get reference to id xml node
				Dim IDXmlNode As System.Xml.XmlNode = m_XmlElement.SelectSingleNode (ElementName, m_Feed.XmlNamespaceManager)

				If IDXmlNode Is Nothing Then
					'  Don't need to do anything if setting null
					If System.String.IsNullOrEmpty(value) Then
						Return
					End If

					If m_Feed.FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
						IDXmlNode = m_Feed.XmlDocument.CreateElement (ElementName, Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
					Else
						IDXmlNode = m_Feed.XmlDocument.CreateElement(ElementName)
					End If

					m_XmlElement.AppendChild(IDXmlNode)
				Else
					If System.String.IsNullOrEmpty(value) Then
						IDXmlNode.ParentNode.RemoveChild(IDXmlNode)
						m_ID = Nothing
						Return
					End If
				End If

				IDXmlNode.InnerText = value
				m_ID = value
			End Set
		End Property

		Public ReadOnly Property SyncNode() As Microsoft.Samples.FeedSync.SyncNode
			Get
				Return m_SyncNode
			End Get
		End Property

		Public ReadOnly Property Feed() As Microsoft.Samples.FeedSync.Feed
			Get
				Return m_Feed
			End Get
		End Property

		Public Function IsSubsumedByFeedItemNode(ByVal i_FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode) As Boolean
			For Each HistoryNode As Microsoft.Samples.FeedSync.HistoryNode In i_FeedItemNode.SyncNode.HistoryNodes
				If m_SyncNode.TopMostHistoryNode.IsSubsumedByHistoryNode(HistoryNode) Then
					Return True
				End If
			Next HistoryNode

			Return False
		End Function

		Public Function Clone() As Microsoft.Samples.FeedSync.FeedItemNode
			Return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (m_Feed, m_Title, m_Description, m_ID, m_SyncNode.ID, m_SyncNode.TopMostHistoryNode.Sequence, m_SyncNode.TopMostHistoryNode.WhenDateTime, m_SyncNode.TopMostHistoryNode.By, m_SyncNode.Deleted, m_SyncNode.NoConflicts, m_SyncNode.Updates)
		End Function

		Public Function Copy(ByVal i_By As String) As Microsoft.Samples.FeedSync.FeedItemNode

			Return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew (m_Feed, m_Title, m_Description, m_ID, System.Guid.NewGuid().ToString(), 1, System.DateTime.Now, i_By, m_SyncNode.Deleted, m_SyncNode.NoConflicts, 1)
		End Function

		Private Sub AddUpdatedElement()
			Dim ElementName As String = System.String.Format ("{0}:{1}", m_Feed.AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_UPDATED_ELEMENT_NAME)

			Dim UpdatedXmlNode As System.Xml.XmlNode = m_XmlElement.SelectSingleNode (ElementName, m_Feed.XmlNamespaceManager)

			'  Remove existing <updated> element if necessary
			If UpdatedXmlNode IsNot Nothing Then
				UpdatedXmlNode.ParentNode.RemoveChild(UpdatedXmlNode)
			End If

			'  Strip namespace prefix if using default namespace
			If m_Feed.XmlNamespaceManager.DefaultNamespace = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI Then
				ElementName = Microsoft.Samples.FeedSync.Constants.ATOM_UPDATED_ELEMENT_NAME
			End If

			UpdatedXmlNode = m_Feed.XmlDocument.CreateElement (ElementName, Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)

            If m_SyncNode.TopMostHistoryNode.WhenDateTime.HasValue Then
                UpdatedXmlNode.InnerText = (CDate(m_SyncNode.TopMostHistoryNode.WhenDateTime)).ToString(Microsoft.Samples.FeedSync.Constants.DATE_STRING_FORMAT)
            Else
                UpdatedXmlNode.InnerText = System.DateTime.Now.ToString(Microsoft.Samples.FeedSync.Constants.DATE_STRING_FORMAT)
            End If

			m_XmlElement.AppendChild(UpdatedXmlNode)
		End Sub
	End Class
End Namespace
