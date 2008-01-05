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
	Public Class SharingNode
		Inherits Microsoft.Samples.FeedSync.Node
		Private m_Since As String
		Private m_Until As String

		Private m_Expires As Nullable(Of System.DateTime) = Nothing

		Private m_Feed As Microsoft.Samples.FeedSync.Feed

		Private m_RelatedNodeList As System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.RelatedNode) = New System.Collections.Generic.List(Of Microsoft.Samples.FeedSync.RelatedNode)()

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed) As Microsoft.Samples.FeedSync.SharingNode
			Return Microsoft.Samples.FeedSync.SharingNode.CreateNew (i_Feed, Nothing, Nothing, Nothing)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Since As String, ByVal i_Until As String) As Microsoft.Samples.FeedSync.SharingNode
			Return Microsoft.Samples.FeedSync.SharingNode.CreateNew (i_Feed, i_Since, i_Until, Nothing)
		End Function

		Public Shared Function CreateNew(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_Since As String, ByVal i_Until As String, ByVal i_Expires As Nullable(Of System.DateTime)) As Microsoft.Samples.FeedSync.SharingNode
			Dim ElementName As String = System.String.Format ("{0}:{1}", i_Feed.FeedSyncNamespacePrefix, Microsoft.Samples.FeedSync.Constants.SHARING_ELEMENT_NAME)

			Dim SharingXmlElement As System.Xml.XmlElement = i_Feed.XmlDocument.CreateElement (ElementName, Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI)

			If (Not System.String.IsNullOrEmpty(i_Since)) Then
				SharingXmlElement.SetAttribute(Microsoft.Samples.FeedSync.Constants.SINCE_ATTRIBUTE, i_Since)
			End If

			If (Not System.String.IsNullOrEmpty(i_Until)) Then
				SharingXmlElement.SetAttribute(Microsoft.Samples.FeedSync.Constants.UNTIL_ATTRIBUTE, i_Until)
			End If

			If i_Expires.HasValue Then
				SharingXmlElement.SetAttribute(Microsoft.Samples.FeedSync.Constants.EXPIRES_ATTRIBUTE, (CDate(i_Expires)).ToString(Microsoft.Samples.FeedSync.Constants.DATE_STRING_FORMAT))
			End If

			Return New Microsoft.Samples.FeedSync.SharingNode(i_Feed, SharingXmlElement)
		End Function

		Public Sub New(ByVal i_Feed As Microsoft.Samples.FeedSync.Feed, ByVal i_SharingXmlElement As System.Xml.XmlElement)
			m_Feed = i_Feed

			m_XmlElement = i_SharingXmlElement

			If m_XmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.SINCE_ATTRIBUTE) Then
				m_Since = m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.SINCE_ATTRIBUTE)
			End If

			If m_XmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.UNTIL_ATTRIBUTE) Then
				m_Until = m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.SINCE_ATTRIBUTE)
			End If

			If m_XmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.EXPIRES_ATTRIBUTE) Then
				Dim Expires As String = m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.EXPIRES_ATTRIBUTE)
				Dim ExpiresDateTime As System.DateTime

				If System.DateTime.TryParse(Expires, ExpiresDateTime) Then
					m_Expires = ExpiresDateTime
				End If
			End If

			Dim XPathQuery As String = System.String.Format ("{0}:{1}", m_Feed.FeedSyncNamespacePrefix, Microsoft.Samples.FeedSync.Constants.RELATED_ELEMENT_NAME)

			Dim RelatedNodeList As System.Xml.XmlNodeList = i_SharingXmlElement.SelectNodes (XPathQuery, m_Feed.XmlNamespaceManager)

			For Each RelatedNodeXmlElement As System.Xml.XmlElement In RelatedNodeList
				Dim RelatedNode As Microsoft.Samples.FeedSync.RelatedNode = Microsoft.Samples.FeedSync.RelatedNode.CreateFromXmlElement (Me, RelatedNodeXmlElement)

				m_RelatedNodeList.Add(RelatedNode)
			Next RelatedNodeXmlElement
		End Sub

		Public Property Since() As String
			Get
				Return m_Since
			End Get
			Set(ByVal value As String)
				If System.String.IsNullOrEmpty(value) Then
					Throw New System.ArgumentException("Invalid value!")
				End If

				m_Since = value
				m_XmlElement.SetAttribute(Microsoft.Samples.FeedSync.Constants.SINCE_ATTRIBUTE, value)
			End Set
		End Property

		Public Property [Until]() As String
			Get
				Return m_Until
			End Get
			Set(ByVal value As String)
				If System.String.IsNullOrEmpty(value) Then
					Throw New System.ArgumentException("Invalid value!")
				End If

				m_Until = value
				m_XmlElement.SetAttribute(Microsoft.Samples.FeedSync.Constants.UNTIL_ATTRIBUTE, value)
			End Set
		End Property

		Public ReadOnly Property Expires() As Nullable(Of System.DateTime)
			Get
				Return m_Expires
			End Get
		End Property

		Public ReadOnly Property Feed() As Microsoft.Samples.FeedSync.Feed
			Get
				Return m_Feed
			End Get
		End Property

		Public Sub AddRelatedNode(ByVal i_RelatedNode As Microsoft.Samples.FeedSync.RelatedNode)
			Dim ImportedRelatedXmlElement As System.Xml.XmlElement = CType(m_Feed.XmlDocument.ImportNode (i_RelatedNode.XmlElement, True), System.Xml.XmlElement)

			Dim ImportedRelatedNode As Microsoft.Samples.FeedSync.RelatedNode = Microsoft.Samples.FeedSync.RelatedNode.CreateFromXmlElement (Me, ImportedRelatedXmlElement)

			m_XmlElement.AppendChild(ImportedRelatedNode.XmlElement)
			m_RelatedNodeList.Add(ImportedRelatedNode)
		End Sub
	End Class
End Namespace
