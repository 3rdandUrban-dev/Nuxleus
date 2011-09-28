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
Imports System.Text

Namespace Microsoft.Samples.FeedSync
	Public Class RelatedNode
		Inherits Microsoft.Samples.FeedSync.Node
		Public Enum RelatedNodeTypes
			Complete = 0
			Aggregated = 1
		End Enum

		Private m_Link As String
		Private m_Title As String

		Private m_SharingNode As Microsoft.Samples.FeedSync.SharingNode

		Private m_RelatedNodeType As Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes

		Public Shared Function CreateNew(ByVal i_SharingNode As Microsoft.Samples.FeedSync.SharingNode, ByVal i_RelatedNodeType As Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes, ByVal i_Link As String, ByVal i_Title As String) As Microsoft.Samples.FeedSync.RelatedNode
			Dim Feed As Microsoft.Samples.FeedSync.Feed = i_SharingNode.Feed

			Dim ElementName As String = System.String.Format ("{0}:{1}", Feed.FeedSyncNamespacePrefix, Microsoft.Samples.FeedSync.Constants.RELATED_ELEMENT_NAME)

			Dim RelatedNodeXmlElement As System.Xml.XmlElement = Feed.XmlDocument.CreateElement (ElementName, Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI)

			Dim RelatedNodeType As String = System.String.Empty
			Select Case i_RelatedNodeType
				Case Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes.Aggregated
						RelatedNodeType = Microsoft.Samples.FeedSync.Constants.RELATED_TYPE_AGGREGATED
						Exit Select

				Case Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes.Complete
						RelatedNodeType = Microsoft.Samples.FeedSync.Constants.RELATED_TYPE_COMPLETE
						Exit Select

				Case Else
						Throw New System.ArgumentException("Unknown related type!")
			End Select

			RelatedNodeXmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.TYPE_ATTRIBUTE, RelatedNodeType)

			RelatedNodeXmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.LINK_ATTRIBUTE, i_Link)

			If (Not System.String.IsNullOrEmpty(i_Title)) Then
				RelatedNodeXmlElement.SetAttribute (Microsoft.Samples.FeedSync.Constants.TITLE_ATTRIBUTE, i_Title)
			End If

			Dim RelatedNode As Microsoft.Samples.FeedSync.RelatedNode = New Microsoft.Samples.FeedSync.RelatedNode (Nothing, RelatedNodeXmlElement)

			Return RelatedNode
		End Function


		Public Shared Function CreateFromXmlElement(ByVal i_SharingNode As Microsoft.Samples.FeedSync.SharingNode, ByVal i_RelatedNodeXmlElement As System.Xml.XmlElement) As Microsoft.Samples.FeedSync.RelatedNode
			If i_RelatedNodeXmlElement.OwnerDocument IsNot i_SharingNode.Feed.XmlDocument Then
				i_RelatedNodeXmlElement = CType(i_SharingNode.Feed.XmlDocument.ImportNode(i_RelatedNodeXmlElement, True), System.Xml.XmlElement)
			End If

			Dim RelatedNode As Microsoft.Samples.FeedSync.RelatedNode = New Microsoft.Samples.FeedSync.RelatedNode (i_SharingNode, i_RelatedNodeXmlElement)

			Return RelatedNode
		End Function

		Private Sub New(ByVal i_SharingNode As Microsoft.Samples.FeedSync.SharingNode, ByVal i_RelatedNodeXmlElement As System.Xml.XmlElement)
			Dim InvalidXmlElement As Boolean = (i_RelatedNodeXmlElement.LocalName <> Microsoft.Samples.FeedSync.Constants.RELATED_ELEMENT_NAME) OrElse (i_RelatedNodeXmlElement.NamespaceURI <> Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI)

			If InvalidXmlElement Then
				Throw New System.Exception("Invalid xml element!")
			End If

			m_SharingNode = i_SharingNode
			m_XmlElement = i_RelatedNodeXmlElement

			Dim RelatedNodeType As String = i_RelatedNodeXmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.TYPE_ATTRIBUTE)
			Select Case RelatedNodeType
				Case Microsoft.Samples.FeedSync.Constants.RELATED_TYPE_AGGREGATED
					m_RelatedNodeType = Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes.Aggregated
					Exit Select

				Case Microsoft.Samples.FeedSync.Constants.RELATED_TYPE_COMPLETE
					m_RelatedNodeType = Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes.Complete
					Exit Select

				Case Else
					Throw New System.ArgumentException("Unknown related node type: " & RelatedNodeType)
			End Select

			m_Link = i_RelatedNodeXmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.LINK_ATTRIBUTE)

			If i_RelatedNodeXmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.TITLE_ATTRIBUTE) Then
				m_Title = i_RelatedNodeXmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.TITLE_ATTRIBUTE)
			End If
		End Sub

		Public ReadOnly Property SharingNode() As Microsoft.Samples.FeedSync.SharingNode
			Get
				Return m_SharingNode
			End Get
		End Property

		Public ReadOnly Property RelatedNodeType() As Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes
			Get
				Return m_RelatedNodeType
			End Get
		End Property

		Public ReadOnly Property Link() As String
			Get
				Return m_Link
			End Get
		End Property

		Public ReadOnly Property Title() As String
			Get
				Return m_Title
			End Get
		End Property
	End Class
End Namespace
