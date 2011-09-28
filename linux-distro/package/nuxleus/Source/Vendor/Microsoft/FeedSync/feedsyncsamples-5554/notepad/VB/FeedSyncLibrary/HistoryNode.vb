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
	Public Class HistoryNode
		Inherits Microsoft.Samples.FeedSync.Node
		Private m_WhenDateTime As Nullable(Of System.DateTime)

		Private m_By As String

		Private m_Sequence As Integer

		Private m_SyncNode As Microsoft.Samples.FeedSync.SyncNode

		Public Shared Function CreateNew(ByVal i_SyncNode As Microsoft.Samples.FeedSync.SyncNode, ByVal i_WhenDateTime As Nullable(Of System.DateTime), ByVal i_By As String) As Microsoft.Samples.FeedSync.HistoryNode
			Dim Feed As Microsoft.Samples.FeedSync.Feed = i_SyncNode.FeedItemNode.Feed

			Dim ElementName As String = System.String.Format ("{0}:{1}", Feed.FeedSyncNamespacePrefix, Microsoft.Samples.FeedSync.Constants.HISTORY_ELEMENT_NAME)

			Dim HistoryNodeXmlElement As System.Xml.XmlElement = Feed.XmlDocument.CreateElement (ElementName, Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI)

			If i_WhenDateTime.HasValue Then
				Dim WhenDateTime As System.DateTime = (CDate(i_WhenDateTime)).ToUniversalTime()
				Dim [When] As String = WhenDateTime.ToString(Microsoft.Samples.FeedSync.Constants.DATE_STRING_FORMAT)

				HistoryNodeXmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.WHEN_ATTRIBUTE, [When])
			End If

			Dim Sequence As Integer = i_SyncNode.Updates + 1

			If (Not System.String.IsNullOrEmpty(i_By)) Then
				HistoryNodeXmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.BY_ATTRIBUTE, i_By)

				For Each ExistingHistoryNode As Microsoft.Samples.FeedSync.HistoryNode In i_SyncNode.HistoryNodes
					If (ExistingHistoryNode.By = i_By) AndAlso (ExistingHistoryNode.Sequence >= Sequence) Then
						Sequence = ExistingHistoryNode.Sequence + 1
					End If
				Next ExistingHistoryNode
			End If

			HistoryNodeXmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.SEQUENCE_ATTRIBUTE, (i_SyncNode.Updates + 1).ToString())

			Dim HistoryNode As Microsoft.Samples.FeedSync.HistoryNode = New Microsoft.Samples.FeedSync.HistoryNode (Nothing, HistoryNodeXmlElement)

			Return HistoryNode
		End Function

		Public Shared Function CreateFromXmlElement(ByVal i_SyncNode As Microsoft.Samples.FeedSync.SyncNode, ByVal i_HistoryNodeXmlElement As System.Xml.XmlElement) As Microsoft.Samples.FeedSync.HistoryNode
			If i_HistoryNodeXmlElement.OwnerDocument IsNot i_SyncNode.FeedItemNode.Feed.XmlDocument Then
				i_HistoryNodeXmlElement = CType(i_SyncNode.FeedItemNode.Feed.XmlDocument.ImportNode(i_HistoryNodeXmlElement, True), System.Xml.XmlElement)
			End If

			Dim HistoryNode As Microsoft.Samples.FeedSync.HistoryNode = New Microsoft.Samples.FeedSync.HistoryNode (i_SyncNode, i_HistoryNodeXmlElement)

			Return HistoryNode
		End Function

		Private Sub New(ByVal i_SyncNode As Microsoft.Samples.FeedSync.SyncNode, ByVal i_HistoryNodeXmlElement As System.Xml.XmlElement)
			Dim InvalidXmlElement As Boolean = (i_HistoryNodeXmlElement.LocalName <> Microsoft.Samples.FeedSync.Constants.HISTORY_ELEMENT_NAME) OrElse (i_HistoryNodeXmlElement.NamespaceURI <> Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI)

			If InvalidXmlElement Then
				Throw New System.Exception("Invalid xml element!")
			End If

			m_SyncNode = i_SyncNode
			m_XmlElement = i_HistoryNodeXmlElement

			m_Sequence = System.Convert.ToInt32(i_HistoryNodeXmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.SEQUENCE_ATTRIBUTE))

			If i_HistoryNodeXmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.WHEN_ATTRIBUTE) Then
				Dim [When] As String = i_HistoryNodeXmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.WHEN_ATTRIBUTE)
				m_WhenDateTime = System.Convert.ToDateTime([When])
			End If

			If i_HistoryNodeXmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.BY_ATTRIBUTE) Then
				m_By = i_HistoryNodeXmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.BY_ATTRIBUTE)
			End If

			If (Not m_WhenDateTime.HasValue) AndAlso (System.String.IsNullOrEmpty(m_By)) Then
				Throw New System.ArgumentException("Must have 'when' or 'by' attribute!")
			End If

			If m_Sequence > (2 Xor 32 - 1) Then
				Throw New System.ArgumentException("Invalid value for 'sequence' attribute!")
			End If
		End Sub

		Public ReadOnly Property WhenDateTime() As Nullable(Of System.DateTime)
			Get
				Return m_WhenDateTime
			End Get
		End Property

		Public ReadOnly Property By() As String
			Get
				Return m_By
			End Get
		End Property

		Public ReadOnly Property Sequence() As Integer
			Get
				Return m_Sequence
			End Get
		End Property

		Public ReadOnly Property SyncNode() As Microsoft.Samples.FeedSync.SyncNode
			Get
				Return m_SyncNode
			End Get
		End Property

		Public Function IsSubsumedByHistoryNode(ByVal i_HistoryNode As Microsoft.Samples.FeedSync.HistoryNode) As Boolean
			Dim Subsumed As Boolean = False

			If (Not System.String.IsNullOrEmpty(m_By)) Then
				Subsumed = (m_By = i_HistoryNode.By) AndAlso (i_HistoryNode.Sequence >= m_Sequence)
			Else
				Subsumed = (m_WhenDateTime.Equals(i_HistoryNode.WhenDateTime)) AndAlso (m_Sequence = i_HistoryNode.Sequence)
			End If

			Return Subsumed
		End Function
	End Class
End Namespace
