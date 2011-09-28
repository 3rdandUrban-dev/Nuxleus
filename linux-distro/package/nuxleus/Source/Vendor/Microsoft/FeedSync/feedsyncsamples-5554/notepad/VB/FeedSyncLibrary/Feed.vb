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
	Public Class Feed
		Private m_XmlDocument As System.Xml.XmlDocument

		Private m_Title As String
		Private m_Description As String
		Private m_Link As String
		Private m_FeedSyncNamespacePrefix As String
		Private m_AtomNamespacePrefix As String
		Private m_FeedItemElementName As String
		Private m_FeedItemXPathQuery As String

		Public Enum FeedTypes
			Atom = 1
			RSS = 2
		End Enum

		Private m_SharingNode As Microsoft.Samples.FeedSync.SharingNode

		Private m_FeedType As Microsoft.Samples.FeedSync.Feed.FeedTypes

		Private m_FeedItemNodeSortedList As System.Collections.Generic.SortedList(Of String, Microsoft.Samples.FeedSync.FeedItemNode) = New System.Collections.Generic.SortedList(Of String, FeedItemNode)()

		Private m_ItemContainerXmlElement As System.Xml.XmlElement

		Private m_XmlNamespaceManager As System.Xml.XmlNamespaceManager

		#Region "Create methods"

		Public Shared Function Create(ByVal i_Title As String, ByVal i_Description As String, ByVal i_Link As String, ByVal i_FeedType As Microsoft.Samples.FeedSync.Feed.FeedTypes) As Microsoft.Samples.FeedSync.Feed
            Dim Feed As Microsoft.Samples.FeedSync.Feed = New Microsoft.Samples.FeedSync.Feed(i_Title, _
                                                                                              i_Description, _
                                                                                              i_Link, _
                                                                                              i_FeedType)

			Return Feed
		End Function

		Public Shared Function Create(ByVal i_FeedSyncXmlDocument As System.Xml.XmlDocument) As Microsoft.Samples.FeedSync.Feed
			Dim Feed As Microsoft.Samples.FeedSync.Feed = New Microsoft.Samples.FeedSync.Feed(i_FeedSyncXmlDocument)
			Return Feed
		End Function

		#End Region

		#Region "ConvertFromFeed methods"

		Public Shared Function ConvertFromFeed(ByVal i_FeedXmlDocument As System.Xml.XmlDocument, ByVal i_SequenceIDBase As Nullable(Of Integer)) As Microsoft.Samples.FeedSync.Feed
			Return Microsoft.Samples.FeedSync.Feed.ConvertFromFeed (i_FeedXmlDocument, i_SequenceIDBase, Nothing, Nothing)
		End Function

		Public Shared Function ConvertFromFeed(ByVal i_FeedXmlDocument As System.Xml.XmlDocument, ByVal i_SequenceIDBase As Nullable(Of Integer), ByVal i_Since As String, ByVal i_Until As String) As Microsoft.Samples.FeedSync.Feed
			Dim Feed As Microsoft.Samples.FeedSync.Feed = New Microsoft.Samples.FeedSync.Feed (i_FeedXmlDocument, i_SequenceIDBase, i_Since, i_Until)

			Return Feed
		End Function

		#End Region

		Public Shared Function MergeFeeds(ByVal i_LocalFeed As Microsoft.Samples.FeedSync.Feed, ByVal i_IncomingFeed As Microsoft.Samples.FeedSync.Feed) As Microsoft.Samples.FeedSync.Feed
			'  Create new instance of xml document for output feed
			Dim OutputXmlDocument As System.Xml.XmlDocument = New System.Xml.XmlDocument()

			'  Create output rss document based on source feed
			OutputXmlDocument.LoadXml(i_LocalFeed.XmlDocument.OuterXml)

			'  Create an instance of Feed
			Dim OutputFeed As Microsoft.Samples.FeedSync.Feed = Microsoft.Samples.FeedSync.Feed.Create(OutputXmlDocument)

			'  Remove all items from output feed
			OutputFeed.PruneAllFeedItemNodes()

			For Each SourceFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode In i_LocalFeed.FeedItemNodes
				Dim FeedSyncID As String = SourceFeedItemNode.SyncNode.ID
				Dim OutputFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = SourceFeedItemNode

				'  Check if item exists in merge feed
				If i_IncomingFeed.DoesFeedItemNodeExist(FeedSyncID) Then
					Dim MergeFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = i_IncomingFeed.GetFeedItemNode(FeedSyncID)
					OutputFeedItemNode = Microsoft.Samples.FeedSync.FeedItemNode.MergeFeedItems (OutputFeed, SourceFeedItemNode, MergeFeedItemNode)
				End If

				OutputFeed.AddFeedItem(OutputFeedItemNode)
			Next SourceFeedItemNode

			For Each MergeFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode In i_IncomingFeed.FeedItemNodes
				Dim FeedSyncID As String = MergeFeedItemNode.SyncNode.ID

				'  Check if item exists in source feed - if so just
				'  continue loop because we would have processed it above
				If i_LocalFeed.DoesFeedItemNodeExist(FeedSyncID) Then
					Continue For
				End If

				OutputFeed.AddFeedItem(MergeFeedItemNode)
			Next MergeFeedItemNode

			Return OutputFeed
		End Function

		#Region "constructors"

		Private Sub New(ByVal i_Title As String, ByVal i_Description As String, ByVal i_Link As String, ByVal i_FeedType As Microsoft.Samples.FeedSync.Feed.FeedTypes)
			Dim XmlDocumentContents As String

			If i_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				XmlDocumentContents = System.String.Format (Microsoft.Samples.FeedSync.Constants.ATOM_FEED_TEMPLATE, Microsoft.Samples.FeedSync.MiscHelpers.XMLEncode(i_Title), Microsoft.Samples.FeedSync.MiscHelpers.XMLEncode(i_Description), Microsoft.Samples.FeedSync.MiscHelpers.XMLEncode(i_Link), System.DateTime.Now.ToUniversalTime().ToString(Microsoft.Samples.FeedSync.Constants.DATE_STRING_FORMAT), System.Reflection.Assembly.GetExecutingAssembly().Location, System.Guid.NewGuid().ToString())
			Else
				XmlDocumentContents = System.String.Format (Microsoft.Samples.FeedSync.Constants.RSS_FEED_TEMPLATE, Microsoft.Samples.FeedSync.MiscHelpers.XMLEncode(i_Title), Microsoft.Samples.FeedSync.MiscHelpers.XMLEncode(i_Description), Microsoft.Samples.FeedSync.MiscHelpers.XMLEncode(i_Link))
			End If

			m_XmlDocument = New System.Xml.XmlDocument()
			m_XmlDocument.LoadXml(XmlDocumentContents)

			m_XmlNamespaceManager = New System.Xml.XmlNamespaceManager(m_XmlDocument.NameTable)

			Me.InitializeXmlNamespaceManager(False)

			m_FeedType = i_FeedType
			If m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				m_FeedItemXPathQuery = System.String.Format ("{0}:{1}", m_AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME)

				If m_XmlNamespaceManager.DefaultNamespace = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI Then
					m_FeedItemElementName = Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME
				Else
					m_FeedItemElementName = m_FeedItemXPathQuery
				End If
			Else
				m_FeedItemXPathQuery = Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_ELEMENT_NAME
				m_FeedItemElementName = m_FeedItemXPathQuery
			End If

			Me.Initialize()
		End Sub

		Private Sub New(ByVal i_FeedSyncXmlDocument As System.Xml.XmlDocument)
			m_XmlDocument = i_FeedSyncXmlDocument
			m_XmlNamespaceManager = New System.Xml.XmlNamespaceManager(m_XmlDocument.NameTable)

			If m_XmlDocument.DocumentElement.LocalName = Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME Then
				m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom
			Else
				m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.RSS
			End If

			Me.InitializeXmlNamespaceManager(False)

			If m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				m_FeedItemXPathQuery = System.String.Format ("{0}:{1}", m_AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME)

				If m_XmlNamespaceManager.DefaultNamespace = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI Then
					m_FeedItemElementName = Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME
				Else
					m_FeedItemElementName = m_FeedItemXPathQuery
				End If
			Else
				m_FeedItemXPathQuery = Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_ELEMENT_NAME
				m_FeedItemElementName = m_FeedItemXPathQuery
			End If

			'  Get reference to 'sx:sharing' element
			Dim XPathQuery As String = System.String.Format ("{0}:{1}", m_FeedSyncNamespacePrefix, Microsoft.Samples.FeedSync.Constants.SHARING_ELEMENT_NAME)

			Dim SharingXmlElement As System.Xml.XmlElement = CType(Me.ItemContainerXmlElement.SelectSingleNode (XPathQuery, m_XmlNamespaceManager), System.Xml.XmlElement)

			If SharingXmlElement IsNot Nothing Then
				m_SharingNode = New Microsoft.Samples.FeedSync.SharingNode(Me, SharingXmlElement)
			End If

			'  Iterate items
			Dim FeedItemXmlNodeList As System.Xml.XmlNodeList = Me.ItemContainerXmlElement.SelectNodes (m_FeedItemXPathQuery, m_XmlNamespaceManager)

			For Each FeedItemXmlElement As System.Xml.XmlElement In FeedItemXmlNodeList
				Dim FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = Microsoft.Samples.FeedSync.FeedItemNode.CreateFromXmlElement (Me, FeedItemXmlElement)

				m_FeedItemNodeSortedList(FeedItemNode.SyncNode.ID) = FeedItemNode
			Next FeedItemXmlElement

			Me.Initialize()
		End Sub

		Private Sub New(ByVal i_FeedXmlDocument As System.Xml.XmlDocument, ByVal i_SequenceIDBase As Nullable(Of Integer), ByVal i_Since As String, ByVal i_Until As String)
			m_XmlDocument = i_FeedXmlDocument
			m_XmlNamespaceManager = New System.Xml.XmlNamespaceManager(m_XmlDocument.NameTable)

			If m_XmlDocument.DocumentElement.LocalName = Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME Then
				m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom
			Else
				m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.RSS
			End If

			Me.InitializeXmlNamespaceManager(True)

			If m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				m_FeedItemXPathQuery = System.String.Format ("{0}:{1}", m_AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME)

				If m_XmlNamespaceManager.DefaultNamespace = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI Then
					m_FeedItemElementName = Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME
				Else
					m_FeedItemElementName = m_FeedItemXPathQuery
				End If
			Else
				m_FeedItemXPathQuery = Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_ELEMENT_NAME
				m_FeedItemElementName = m_FeedItemXPathQuery
			End If

			'  Get reference to 'sx:sharing' element
			Dim XPathQuery As String = System.String.Format ("{0}:{1}", m_FeedSyncNamespacePrefix, Microsoft.Samples.FeedSync.Constants.SHARING_ELEMENT_NAME)

			Dim SharingXmlElement As System.Xml.XmlElement = CType(Me.ItemContainerXmlElement.SelectSingleNode (XPathQuery, m_XmlNamespaceManager), System.Xml.XmlElement)

			XPathQuery = System.String.Format ("descendant::*[{0}:{1}]", m_FeedSyncNamespacePrefix, Microsoft.Samples.FeedSync.Constants.SYNC_ELEMENT_NAME)

			Dim SyncXmlNodeList As System.Xml.XmlNodeList = Me.ItemContainerXmlElement.SelectNodes (XPathQuery, m_XmlNamespaceManager)

			If (SharingXmlElement IsNot Nothing) OrElse (SyncXmlNodeList.Count > 0) Then
				Throw New System.Exception("Document is already a valid FeedSync document!")
			End If

			Dim FeedItemXmlNodeList As System.Xml.XmlNodeList = Me.ItemContainerXmlElement.SelectNodes (m_FeedItemXPathQuery, m_XmlNamespaceManager)

			'  BIG HONKING NOTE:  Iterate nodes using index instead of enumerator
			For Index As Integer = 0 To FeedItemXmlNodeList.Count - 1
				Dim FeedItemXmlElement As System.Xml.XmlElement = CType(FeedItemXmlNodeList(Index), System.Xml.XmlElement)

				'  Remove item from document
				FeedItemXmlElement.ParentNode.RemoveChild(FeedItemXmlElement)

				Dim SyncNodeID As String = Nothing
                If i_SequenceIDBase.HasValue Then
                    Dim val As Integer = i_SequenceIDBase.Value
                    i_SequenceIDBase = val + 1
                    SyncNodeID = i_SequenceIDBase.ToString()
                End If

				'  Create new FeedItemNode and add it to feed
				Dim FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = Microsoft.Samples.FeedSync.FeedItemNode.CreateNewFromXmlElement (Me, FeedItemXmlElement, SyncNodeID)

				Me.AddFeedItem(FeedItemNode)
			Next Index

			Me.Initialize()
		End Sub

		#End Region

		Private Sub Initialize()
			Dim XPathQuery As String

'			#Region "Feed Title related"

			If m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				XPathQuery = System.String.Format ("/{0}:{1}/{2}:{3}", m_AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME, m_AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_TITLE_ELEMENT_NAME)
			Else
				XPathQuery = System.String.Format ("{0}/{1}", Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_CONTAINER_ELEMENT_NAME, Microsoft.Samples.FeedSync.Constants.RSS_TITLE_ELEMENT_NAME)
			End If

			'  Get title
			Dim TitleXmlNode As System.Xml.XmlNode = m_XmlDocument.DocumentElement.SelectSingleNode(XPathQuery, m_XmlNamespaceManager)
			If TitleXmlNode IsNot Nothing Then
				m_Title = TitleXmlNode.InnerText
			End If

'			#End Region

'			#Region "Feed Description related"

			If m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				XPathQuery = System.String.Format ("{0}/{1}", Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME, Microsoft.Samples.FeedSync.Constants.ATOM_FEED_DESCRIPTION_ELEMENT_NAME)
			Else
				XPathQuery = System.String.Format ("{0}/{1}", Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_CONTAINER_ELEMENT_NAME, Microsoft.Samples.FeedSync.Constants.RSS_DESCRIPTION_ELEMENT_NAME)
			End If

			'  Get description
			Dim DescriptionXmlNode As System.Xml.XmlNode = m_XmlDocument.DocumentElement.SelectSingleNode(XPathQuery)
			If DescriptionXmlNode IsNot Nothing Then
				m_Description = DescriptionXmlNode.InnerText
			End If

'			#End Region

'			#Region "Feed Link related"

			If m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
				XPathQuery = System.String.Format ("{0}/{1}", Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME, Microsoft.Samples.FeedSync.Constants.ATOM_LINK_ELEMENT_NAME)
			Else
				XPathQuery = System.String.Format ("{0}/{1}", Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_CONTAINER_ELEMENT_NAME, Microsoft.Samples.FeedSync.Constants.RSS_LINK_ELEMENT_NAME)
			End If

			'  Get link
			Dim LinkXmlNode As System.Xml.XmlNode = m_XmlDocument.DocumentElement.SelectSingleNode(XPathQuery)
			If LinkXmlNode IsNot Nothing Then
				m_Link = LinkXmlNode.InnerText
			End If

'			#End Region
		End Sub

		Private Sub InitializeXmlNamespaceManager(ByVal i_AddFeedSyncNamespace As Boolean)
			System.Diagnostics.Debug.Assert(m_XmlDocument IsNot Nothing)
			System.Diagnostics.Debug.Assert(m_XmlNamespaceManager IsNot Nothing)

			If i_AddFeedSyncNamespace Then
				m_XmlDocument.DocumentElement.SetAttribute ("xmlns:" & Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX, Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI)
			End If

'			#Region "Populate namespaces into XmlNamespaceManager instance"

			Dim XmlReader As System.Xml.XmlReader = New System.Xml.XmlNodeReader(m_XmlDocument)

			Do While XmlReader.Read()
				XmlReader.MoveToFirstAttribute()
				Do
					If XmlReader.Name.StartsWith("xmlns:") OrElse (XmlReader.Name = "xmlns") Then
						Dim NamespacePrefix As String = System.String.Empty
						Dim NamespaceValue As String = System.String.Empty

						If XmlReader.Name.StartsWith("xmlns:") Then
							NamespacePrefix = XmlReader.Name.Substring(XmlReader.Name.IndexOf(":"c) + 1)
						End If

						NamespaceValue = XmlReader.Value

						If (Not System.String.IsNullOrEmpty(NamespaceValue)) Then
							m_XmlNamespaceManager.AddNamespace(NamespacePrefix, NamespaceValue)
						End If
					End If
				Loop While XmlReader.MoveToNextAttribute()
			Loop

			XmlReader.Close()

'			#End Region

'			#Region "Get/set namespace prefix for subsequent xpath queries"

			For Each KeyValuePair As System.Collections.Generic.KeyValuePair(Of String, String) In m_XmlNamespaceManager.GetNamespacesInScope(System.Xml.XmlNamespaceScope.All)
				If KeyValuePair.Value = Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI Then
					If KeyValuePair.Key = Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX Then
						m_FeedSyncNamespacePrefix = Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX
						Continue For
					End If

					'  Check for default namespace
					If System.String.IsNullOrEmpty(KeyValuePair.Key) Then
						'  See if sse prefix used
						If (Not m_XmlNamespaceManager.HasNamespace(Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX)) Then
							m_FeedSyncNamespacePrefix = Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX
						Else
							'  See if sse prefix is mapped to atom namespace uri
							Dim NamespaceURI As String = m_XmlNamespaceManager.LookupNamespace(Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX)
							If NamespaceURI = Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI Then
								m_FeedSyncNamespacePrefix = Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX
							Else
								'  Create a new prefix
								m_FeedSyncNamespacePrefix = Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX & System.Guid.NewGuid().ToString()
							End If
						End If

						'  Add namespace with mapped prefix
						m_XmlNamespaceManager.AddNamespace (m_FeedSyncNamespacePrefix, Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI)
					End If

					If System.String.IsNullOrEmpty(m_FeedSyncNamespacePrefix) Then
						Throw New System.Exception("Invalid FeedSync XML document!")
					End If
				ElseIf KeyValuePair.Value = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI Then
					If KeyValuePair.Key = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX Then
						m_AtomNamespacePrefix = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX
						Continue For
					End If

					'  Check for default namespace
					If System.String.IsNullOrEmpty(KeyValuePair.Key) Then
						'  See if atom prefix used
						If (Not m_XmlNamespaceManager.HasNamespace(Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX)) Then
							m_AtomNamespacePrefix = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX
						Else
							'  See if atom prefix is mapped to atom namespace uri
							Dim NamespaceURI As String = m_XmlNamespaceManager.LookupNamespace(Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX)
							If NamespaceURI = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI Then
								m_AtomNamespacePrefix = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX
							Else
								'  Create a new prefix
								m_AtomNamespacePrefix = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX & System.Guid.NewGuid().ToString()
							End If
						End If

						'  Add namespace with mapped prefix
						m_XmlNamespaceManager.AddNamespace (m_AtomNamespacePrefix, Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
					End If

					If System.String.IsNullOrEmpty(m_AtomNamespacePrefix) Then
						Throw New System.Exception("Invalid Atom XML document!")
					End If
				End If
			Next KeyValuePair

'			#End Region
		End Sub

		Public ReadOnly Property Title() As String
			Get
				Return m_Title
			End Get
		End Property

		Public ReadOnly Property Description() As String
			Get
				Return m_Description
			End Get
		End Property

		Public ReadOnly Property Link() As String
			Get
				Return m_Link
			End Get
		End Property

		Public ReadOnly Property FeedSyncNamespacePrefix() As String
			Get
				Return m_FeedSyncNamespacePrefix
			End Get
		End Property

		Public ReadOnly Property AtomNamespacePrefix() As String
			Get
				Return m_AtomNamespacePrefix
			End Get
		End Property

		Public ReadOnly Property XmlDocument() As System.Xml.XmlDocument
			Get
				Return m_XmlDocument
			End Get
		End Property

		Public ReadOnly Property ItemContainerXmlElement() As System.Xml.XmlElement
			Get
				If m_ItemContainerXmlElement Is Nothing Then
					If m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom Then
						m_ItemContainerXmlElement = CType(m_XmlDocument.DocumentElement, System.Xml.XmlElement)
					Else
						m_ItemContainerXmlElement = CType(m_XmlDocument.DocumentElement.SelectSingleNode(Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_CONTAINER_ELEMENT_NAME), System.Xml.XmlElement)
					End If

					System.Diagnostics.Debug.Assert(m_ItemContainerXmlElement IsNot Nothing)
				End If

				Return m_ItemContainerXmlElement
			End Get
		End Property

		Public ReadOnly Property FeedSyncSharingNode() As Microsoft.Samples.FeedSync.SharingNode
			Get
				Return m_SharingNode
			End Get
		End Property

		Public ReadOnly Property XmlNamespaceManager() As System.Xml.XmlNamespaceManager
			Get
				Return m_XmlNamespaceManager
			End Get
		End Property

		Public ReadOnly Property FeedType() As Microsoft.Samples.FeedSync.Feed.FeedTypes
			Get
				Return m_FeedType
			End Get
		End Property

		Public Sub AddFeedItem(ByVal i_FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode)
			If i_FeedItemNode.XmlElement.OwnerDocument IsNot m_XmlDocument Then
				i_FeedItemNode = Me.ImportFeedItemNode(i_FeedItemNode)
			End If

			'  Append feed item element to 'channel' element
			Me.ItemContainerXmlElement.AppendChild(i_FeedItemNode.XmlElement)

			'  Cache item in hashtable
			m_FeedItemNodeSortedList(i_FeedItemNode.SyncNode.ID) = i_FeedItemNode
		End Sub

		#Region "UpdateFeedItem methods"

		Public Sub UpdateFeedItem(ByVal i_FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode)
			'  Get item in hashtable
			Dim FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = m_FeedItemNodeSortedList(i_FeedItemNode.SyncNode.ID)

			'  Get feed item element to 'channel' element
			Me.ItemContainerXmlElement.ReplaceChild(i_FeedItemNode.XmlElement, FeedItemNode.XmlElement)

			'  Cache item in hashtable
			m_FeedItemNodeSortedList(i_FeedItemNode.SyncNode.ID) = i_FeedItemNode
		End Sub

		Public Function UpdateFeedItem(ByVal i_Title As String, ByVal i_Description As String, ByVal i_SyncNodeID As String, ByVal i_When As String, ByVal i_By As String) As Boolean
			Return Me.UpdateFeedItem (i_Title, i_Description, i_SyncNodeID, System.Convert.ToDateTime(i_When), i_By, False)
		End Function

		Public Function UpdateFeedItem(ByVal i_Title As String, ByVal i_Description As String, ByVal i_SyncNodeID As String, ByVal i_WhenDateTime As System.DateTime, ByVal i_By As String) As Boolean
			Return Me.UpdateFeedItem (i_Title, i_Description, i_SyncNodeID, i_WhenDateTime, i_By, False)
		End Function

		Public Function UpdateFeedItem(ByVal i_Title As String, ByVal i_Description As String, ByVal i_SyncNodeID As String, ByVal i_When As String, ByVal i_By As String, ByVal i_ResolveConflicts As Boolean) As Boolean
			Return Me.UpdateFeedItem (i_Title, i_Description, i_SyncNodeID, System.Convert.ToDateTime(i_When), i_By, i_ResolveConflicts)
		End Function

		Public Function UpdateFeedItem(ByVal i_Title As String, ByVal i_Description As String, ByVal i_SyncNodeID As String, ByVal i_WhenDateTime As System.DateTime, ByVal i_By As String, ByVal i_ResolveConflicts As Boolean) As Boolean
			'  Make sure item already exists
			If (Not m_FeedItemNodeSortedList.ContainsKey(i_SyncNodeID)) Then
				Throw New System.Exception("FeedItem doesn't exist - ID = " & i_SyncNodeID)
			End If

			Dim FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = CType(m_FeedItemNodeSortedList(i_SyncNodeID), Microsoft.Samples.FeedSync.FeedItemNode)

			Dim UpdateSucceeded As Boolean = FeedItemNode.Update (i_Title, i_Description, i_WhenDateTime, i_By, i_ResolveConflicts)

			Return UpdateSucceeded
		End Function

		#End Region

		#Region "DeleteFeedItem methods"

		Public Function DeleteFeedItem(ByVal i_SyncNodeID As String) As Boolean
			Return Me.DeleteFeedItem (i_SyncNodeID, System.DateTime.Now, "")
		End Function

		Public Function DeleteFeedItem(ByVal i_SyncNodeID As String, ByVal i_When As String, ByVal i_By As String) As Boolean
			Return Me.DeleteFeedItem (i_SyncNodeID, System.Convert.ToDateTime(i_When), i_By)
		End Function

		Public Function DeleteFeedItem(ByVal i_SyncNodeID As String, ByVal i_WhenDateTime As System.DateTime, ByVal i_By As String) As Boolean
			'  Make sure item already exists
			If (Not m_FeedItemNodeSortedList.ContainsKey(i_SyncNodeID)) Then
				Throw New System.Exception("FeedItem doesn't exist - ID = " & i_SyncNodeID)
			End If

			Dim FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = CType(m_FeedItemNodeSortedList(i_SyncNodeID), Microsoft.Samples.FeedSync.FeedItemNode)

			Dim DeleteSucceeded As Boolean = FeedItemNode.Delete (i_WhenDateTime, i_By)

			Return DeleteSucceeded
		End Function

		#End Region

		#Region "PruneFeedItemNode methods"

		Public Function PruneFeedItemNode(ByVal i_SyncNodeID As String) As Boolean
			If (Not m_FeedItemNodeSortedList.ContainsKey(i_SyncNodeID)) Then
				Return False
			End If

			Dim FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = CType(m_FeedItemNodeSortedList(i_SyncNodeID), Microsoft.Samples.FeedSync.FeedItemNode)
			Return Me.PruneFeedItemNode(FeedItemNode)
		End Function

		Public Function PruneFeedItemNode(ByVal i_FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode) As Boolean
			m_FeedItemNodeSortedList.Remove(i_FeedItemNode.SyncNode.ID)

			Dim FeedItemXmlElement As System.Xml.XmlElement = i_FeedItemNode.XmlElement
			FeedItemXmlElement.ParentNode.RemoveChild(FeedItemXmlElement)

			Return True
		End Function

		#End Region

		#Region "PruneAllFeedItemNodes"

		Public Sub PruneAllFeedItemNodes()
			Me.PruneAllFeedItemNodes(False)
		End Sub

		Public Sub PruneAllFeedItemNodes(ByVal i_ExcludeDeletedItems As Boolean)
			Dim FeedItemNodeList As System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.FeedItemNode) = New System.Collections.Generic.List(Of FeedItemNode)()
			For Each FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode In m_FeedItemNodeSortedList.Values
				If i_ExcludeDeletedItems AndAlso FeedItemNode.SyncNode.Deleted Then
					Continue For
				End If

				FeedItemNodeList.Add(FeedItemNode)
			Next FeedItemNode

			For Each FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode In FeedItemNodeList
				Me.PruneFeedItemNode(FeedItemNode)
			Next FeedItemNode
		End Sub

		#End Region

		Public Function FindFeedItemNode(ByVal i_SyncNodeID As String, <System.Runtime.InteropServices.Out()> ByRef o_FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode) As Boolean
			o_FeedItemNode = Nothing

			If m_FeedItemNodeSortedList.ContainsKey(i_SyncNodeID) Then
				o_FeedItemNode = CType(m_FeedItemNodeSortedList(i_SyncNodeID), Microsoft.Samples.FeedSync.FeedItemNode)
				Return True
			End If

			Return False
		End Function

		Public Function DoesFeedItemNodeExist(ByVal i_SyncNodeID As String) As Boolean
			Return m_FeedItemNodeSortedList.ContainsKey(i_SyncNodeID)
		End Function

		Public Function GetFeedItemNode(ByVal i_SyncNodeID As String) As Microsoft.Samples.FeedSync.FeedItemNode
			'  Make sure item already exists
			If (Not m_FeedItemNodeSortedList.ContainsKey(i_SyncNodeID)) Then
				Throw New System.Exception("FeedItem doesn't exist - ID = " & i_SyncNodeID)
			End If

			Return CType(m_FeedItemNodeSortedList(i_SyncNodeID), Microsoft.Samples.FeedSync.FeedItemNode)
		End Function

		Public ReadOnly Property FeedItemNodes() As Microsoft.Samples.FeedSync.FeedItemNode()
			Get
				Dim FeedItemNodeList As System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.FeedItemNode) = New System.Collections.Generic.List(Of FeedItemNode)(m_FeedItemNodeSortedList.Values)
				Return FeedItemNodeList.ToArray()
			End Get
		End Property

		Public Function ImportFeedItemNode(ByVal i_FeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode) As Microsoft.Samples.FeedSync.FeedItemNode
			'  Import the node into the current document
			Dim ImportedFeedItemNodeXmlElement As System.Xml.XmlElement = CType(m_XmlDocument.ImportNode(i_FeedItemNode.XmlElement, True), System.Xml.XmlElement)

			Dim ImportedFeedItemNode As Microsoft.Samples.FeedSync.FeedItemNode = Microsoft.Samples.FeedSync.FeedItemNode.CreateFromXmlElement (Me, ImportedFeedItemNodeXmlElement)

			Return ImportedFeedItemNode
		End Function

		Friend ReadOnly Property FeedItemElementName() As String
			Get
				Return m_FeedItemElementName
			End Get
		End Property

		Friend ReadOnly Property FeedItemXPathQuery() As String
			Get
				Return m_FeedItemXPathQuery
			End Get
		End Property
	End Class
End Namespace
