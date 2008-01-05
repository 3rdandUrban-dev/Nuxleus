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

Namespace Microsoft.Samples.FeedSyncService
	Public Class Constants
		Public Const HTTP_UNAUTHORIZED_FRAGMENT As String = "not authorized"
		Public Const HTTP_UNAUTHENTICATED_FRAGMENT As String = "not authenticated"

		Public Const DATE_STRING_FORMAT As String = "yyyy-MM-ddThh:mm:ssZ"

		Public Const RSS_QUERY_FRAGMENT As String = "&alt=rss"
		Public Const ATOM_QUERY_FRAGMENT As String = "&alt=atom"

		Public Const FEEDSYNC_XML_NAMESPACE_PREFIX As String = "sx"
		Public Const FEEDSYNC_XML_NAMESPACE_URI As String = "http://feedsync.org/2007/feedsync"

		Public Const ATOM_DOCUMENT_ELEMENT_NAME As String = "feed"
		Public Const ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME As String = "feed"

		Public Const RSS_DOCUMENT_ELEMENT_NAME As String = "rss"
		Public Const RSS_FEED_ITEM_CONTAINER_ELEMENT_NAME As String = "channel"

		Public Const SYNC_ELEMENT_NAME As String = "sync"

	End Class
End Namespace
