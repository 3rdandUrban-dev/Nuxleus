/*****************************************************************************************
   
   Copyright (c) Microsoft Corporation. All rights reserved.

   Use of this code sample is subject to the terms of the Microsoft
   Permissive License, a copy of which should always be distributed with
   this file.  You can also access a copy of this license agreement at:
   http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx

 ****************************************************************************************/

using System;

namespace Microsoft.Samples.FeedSync
{
    public class Constants
    {
        public const string FEEDSYNC_XML_NAMESPACE_PREFIX = "sx";
        public const string FEEDSYNC_XML_NAMESPACE_URI = "http://feedsync.org/2007/feedsync";
        public const string ATOM_XML_NAMESPACE_PREFIX = "atom";
        public const string ATOM_XML_NAMESPACE_URI = "http://www.w3.org/2005/Atom";

        internal const string RSS_FEED_TEMPLATE = "<?xml version='1.0' encoding='utf-8'?><rss version='2.0' xmlns:sx='" + FEEDSYNC_XML_NAMESPACE_URI + "'><channel><title>{0}</title><description>{1}</description><link>{2}</link></channel></rss>";
        internal const string ATOM_FEED_TEMPLATE = "<?xml version='1.0' encoding='utf-8'?><" + ATOM_XML_NAMESPACE_PREFIX + ":feed xmlns:" + ATOM_XML_NAMESPACE_PREFIX + "='" + ATOM_XML_NAMESPACE_URI + "' xmlns:sx='" + FEEDSYNC_XML_NAMESPACE_URI + "'><atom:title>{0}</atom:title><atom:subtitle>{1}</atom:subtitle><atom:link rel='self' href='{2}' /><atom:updated>{3}</atom:updated><atom:author><atom:name>{4}</atom:name></atom:author><atom:id>{5}</atom:id></" + ATOM_XML_NAMESPACE_PREFIX + ":feed>";

        public const string SHARING_ELEMENT_NAME = "sharing";
        public const string RELATED_ELEMENT_NAME = "related";
        public const string SYNC_ELEMENT_NAME = "sync";
        public const string HISTORY_ELEMENT_NAME = "history";
        public const string CONFLICTS_ELEMENT_NAME = "conflicts";

        public const string RSS_FEED_ITEM_CONTAINER_ELEMENT_NAME = "channel"; 
        public const string RSS_FEED_ITEM_ELEMENT_NAME = "item";
        public const string RSS_TITLE_ELEMENT_NAME = "title";
        public const string RSS_DESCRIPTION_ELEMENT_NAME = "description";
        public const string RSS_LINK_ELEMENT_NAME = "link";
        public const string RSS_ID_ELEMENT_NAME = "id";

        public const string ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME = "feed";
        public const string ATOM_FEED_ITEM_ELEMENT_NAME = "entry";
        public const string ATOM_TITLE_ELEMENT_NAME = "title";
        public const string ATOM_FEED_DESCRIPTION_ELEMENT_NAME = "subtitle";
        public const string ATOM_FEED_ITEM_DESCRIPTION_ELEMENT_NAME = "content";
        public const string ATOM_LINK_ELEMENT_NAME = "link";
        public const string ATOM_ID_ELEMENT_NAME = "id";
        public const string ATOM_UPDATED_ELEMENT_NAME = "updated";
        
        public const string DATE_STRING_FORMAT = "yyyy-MM-ddThh:mm:ssZ";

        public const string VERSION_ATTRIBUTE = "version";
        public const string SINCE_ATTRIBUTE = "since";
        public const string UNTIL_ATTRIBUTE = "until";
        public const string EXPIRES_ATTRIBUTE = "expires";
        public const string TYPE_ATTRIBUTE = "type";
        public const string TITLE_ATTRIBUTE = "title";
        public const string LINK_ATTRIBUTE = "link";
        public const string ID_ATTRIBUTE = "id";
        public const string UPDATES_ATTRIBUTE = "updates";
        public const string NO_CONFLICTS_ATTRIBUTE = "noconflicts";
        public const string DELETED_ATTRIBUTE = "deleted";
        public const string SEQUENCE_ATTRIBUTE = "sequence";
        public const string WHEN_ATTRIBUTE = "when";
        public const string BY_ATTRIBUTE = "by";

        public const string RELATED_TYPE_COMPLETE = "complete";
        public const string RELATED_TYPE_AGGREGATED = "aggregated";
    }
}
