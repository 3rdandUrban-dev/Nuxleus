/*****************************************************************************************
   
   Copyright (c) Microsoft Corporation. All rights reserved.

   Use of this code sample is subject to the terms of the Microsoft
   Permissive License, a copy of which should always be distributed with
   this file.  You can also access a copy of this license agreement at:
   http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx

 ****************************************************************************************/

using System;

namespace Microsoft.Samples.FeedSyncService
{
    public class Constants
    {
        public const string HTTP_UNAUTHORIZED_FRAGMENT = "not authorized";
        public const string HTTP_UNAUTHENTICATED_FRAGMENT = "not authenticated";

        public const string DATE_STRING_FORMAT = "yyyy-MM-ddThh:mm:ssZ";

        public const string RSS_QUERY_FRAGMENT = "&alt=rss";
        public const string ATOM_QUERY_FRAGMENT = "&alt=atom";

        public const string FEEDSYNC_XML_NAMESPACE_PREFIX = "sx";
        public const string FEEDSYNC_XML_NAMESPACE_URI = "http://feedsync.org/2007/feedsync";

        public const string ATOM_DOCUMENT_ELEMENT_NAME = "feed";
        public const string ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME = "feed";

        public const string RSS_DOCUMENT_ELEMENT_NAME = "rss";
        public const string RSS_FEED_ITEM_CONTAINER_ELEMENT_NAME = "channel";

        public const string SYNC_ELEMENT_NAME = "sync";

    }
}
