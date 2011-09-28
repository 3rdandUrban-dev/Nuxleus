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
	Public Class Constants
		Public Const FEEDSYNC_XML_NAMESPACE_PREFIX As String = "sx"
		Public Const FEEDSYNC_XML_NAMESPACE_URI As String = "http://feedsync.org/2007/feedsync"
		Public Const ATOM_XML_NAMESPACE_PREFIX As String = "atom"
		Public Const ATOM_XML_NAMESPACE_URI As String = "http://www.w3.org/2005/Atom"

		Friend Const RSS_FEED_TEMPLATE As String = "<?xml version='1.0' encoding='utf-8'?><rss version='2.0' xmlns:sx='" & FEEDSYNC_XML_NAMESPACE_URI & "'><channel><title>{0}</title><description>{1}</description><link>{2}</link></channel></rss>"
		Friend Const ATOM_FEED_TEMPLATE As String = "<?xml version='1.0' encoding='utf-8'?><" & ATOM_XML_NAMESPACE_PREFIX & ":feed xmlns:" & ATOM_XML_NAMESPACE_PREFIX & "='" & ATOM_XML_NAMESPACE_URI & "' xmlns:sx='" & FEEDSYNC_XML_NAMESPACE_URI & "'><atom:title>{0}</atom:title><atom:subtitle>{1}</atom:subtitle><atom:link rel='self' href='{2}' /><atom:updated>{3}</atom:updated><atom:author><atom:name>{4}</atom:name></atom:author><atom:id>{5}</atom:id></" & ATOM_XML_NAMESPACE_PREFIX & ":feed>"

		Public Const SHARING_ELEMENT_NAME As String = "sharing"
		Public Const RELATED_ELEMENT_NAME As String = "related"
		Public Const SYNC_ELEMENT_NAME As String = "sync"
		Public Const HISTORY_ELEMENT_NAME As String = "history"
		Public Const CONFLICTS_ELEMENT_NAME As String = "conflicts"

		Public Const RSS_FEED_ITEM_CONTAINER_ELEMENT_NAME As String = "channel"
		Public Const RSS_FEED_ITEM_ELEMENT_NAME As String = "item"
		Public Const RSS_TITLE_ELEMENT_NAME As String = "title"
		Public Const RSS_DESCRIPTION_ELEMENT_NAME As String = "description"
		Public Const RSS_LINK_ELEMENT_NAME As String = "link"
		Public Const RSS_ID_ELEMENT_NAME As String = "id"

		Public Const ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME As String = "feed"
		Public Const ATOM_FEED_ITEM_ELEMENT_NAME As String = "entry"
		Public Const ATOM_TITLE_ELEMENT_NAME As String = "title"
		Public Const ATOM_FEED_DESCRIPTION_ELEMENT_NAME As String = "subtitle"
		Public Const ATOM_FEED_ITEM_DESCRIPTION_ELEMENT_NAME As String = "content"
		Public Const ATOM_LINK_ELEMENT_NAME As String = "link"
		Public Const ATOM_ID_ELEMENT_NAME As String = "id"
		Public Const ATOM_UPDATED_ELEMENT_NAME As String = "updated"

		Public Const DATE_STRING_FORMAT As String = "yyyy-MM-ddThh:mm:ssZ"

		Public Const VERSION_ATTRIBUTE As String = "version"
		Public Const SINCE_ATTRIBUTE As String = "since"
		Public Const UNTIL_ATTRIBUTE As String = "until"
		Public Const EXPIRES_ATTRIBUTE As String = "expires"
		Public Const TYPE_ATTRIBUTE As String = "type"
		Public Const TITLE_ATTRIBUTE As String = "title"
		Public Const LINK_ATTRIBUTE As String = "link"
		Public Const ID_ATTRIBUTE As String = "id"
		Public Const UPDATES_ATTRIBUTE As String = "updates"
		Public Const NO_CONFLICTS_ATTRIBUTE As String = "noconflicts"
		Public Const DELETED_ATTRIBUTE As String = "deleted"
		Public Const SEQUENCE_ATTRIBUTE As String = "sequence"
		Public Const WHEN_ATTRIBUTE As String = "when"
		Public Const BY_ATTRIBUTE As String = "by"

		Public Const RELATED_TYPE_COMPLETE As String = "complete"
		Public Const RELATED_TYPE_AGGREGATED As String = "aggregated"
	End Class
End Namespace
