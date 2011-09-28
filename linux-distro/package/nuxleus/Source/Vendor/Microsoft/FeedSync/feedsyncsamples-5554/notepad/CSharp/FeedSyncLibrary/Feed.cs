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
    public class Feed
    {
        private System.Xml.XmlDocument m_XmlDocument;

        private string m_Title;
        private string m_Description;
        private string m_Link;
        private string m_FeedSyncNamespacePrefix;
        private string m_AtomNamespacePrefix;
        private string m_FeedItemElementName;
        private string m_FeedItemXPathQuery;

        public enum FeedTypes
        {
            Atom = 1,
            RSS = 2
        };

        private Microsoft.Samples.FeedSync.SharingNode m_SharingNode;

        private Microsoft.Samples.FeedSync.Feed.FeedTypes m_FeedType;

        private System.Collections.Generic.SortedList<string, Microsoft.Samples.FeedSync.FeedItemNode> m_FeedItemNodeSortedList = new System.Collections.Generic.SortedList<string, FeedItemNode>();

        private System.Xml.XmlElement m_ItemContainerXmlElement;

        private System.Xml.XmlNamespaceManager m_XmlNamespaceManager;

        #region Create methods

        public static Microsoft.Samples.FeedSync.Feed Create(string i_Title, string i_Description, string i_Link, Microsoft.Samples.FeedSync.Feed.FeedTypes i_FeedType)
        {
            Microsoft.Samples.FeedSync.Feed Feed = new Microsoft.Samples.FeedSync.Feed
                (
                i_Title,
                i_Description,
                i_Link,
                i_FeedType
                );

            return Feed;
        }

        public static Microsoft.Samples.FeedSync.Feed Create(System.Xml.XmlDocument i_FeedSyncXmlDocument)
        {
            Microsoft.Samples.FeedSync.Feed Feed = new Microsoft.Samples.FeedSync.Feed(i_FeedSyncXmlDocument);
            return Feed;
        }

        #endregion

        #region ConvertFromFeed methods

        public static Microsoft.Samples.FeedSync.Feed ConvertFromFeed(System.Xml.XmlDocument i_FeedXmlDocument, int? i_SequenceIDBase)
        {
            return Microsoft.Samples.FeedSync.Feed.ConvertFromFeed
                (
                i_FeedXmlDocument,
                i_SequenceIDBase,
                null,
                null
                );
        }

        public static Microsoft.Samples.FeedSync.Feed ConvertFromFeed(System.Xml.XmlDocument i_FeedXmlDocument, int? i_SequenceIDBase, string i_Since, string i_Until)
        {
            Microsoft.Samples.FeedSync.Feed Feed = new Microsoft.Samples.FeedSync.Feed
                (
                i_FeedXmlDocument,
                i_SequenceIDBase,
                i_Since,
                i_Until
                );

            return Feed;
        }

        #endregion

        public static Microsoft.Samples.FeedSync.Feed MergeFeeds(Microsoft.Samples.FeedSync.Feed i_LocalFeed, Microsoft.Samples.FeedSync.Feed i_IncomingFeed)
        {
            //  Create new instance of xml document for output feed
            System.Xml.XmlDocument OutputXmlDocument = new System.Xml.XmlDocument();

            //  Create output rss document based on source feed
            OutputXmlDocument.LoadXml(i_LocalFeed.XmlDocument.OuterXml);

            //  Create an instance of Feed
            Microsoft.Samples.FeedSync.Feed OutputFeed = Microsoft.Samples.FeedSync.Feed.Create(OutputXmlDocument);

            //  Remove all items from output feed
            OutputFeed.PruneAllFeedItemNodes();

            foreach (Microsoft.Samples.FeedSync.FeedItemNode SourceFeedItemNode in i_LocalFeed.FeedItemNodes)
            {
                string FeedSyncID = SourceFeedItemNode.SyncNode.ID;
                Microsoft.Samples.FeedSync.FeedItemNode OutputFeedItemNode = SourceFeedItemNode;

                //  Check if item exists in merge feed
                if (i_IncomingFeed.DoesFeedItemNodeExist(FeedSyncID))
                {
                    Microsoft.Samples.FeedSync.FeedItemNode MergeFeedItemNode = i_IncomingFeed.GetFeedItemNode(FeedSyncID);
                    OutputFeedItemNode = Microsoft.Samples.FeedSync.FeedItemNode.MergeFeedItems
                        (
                        OutputFeed,
                        SourceFeedItemNode,
                        MergeFeedItemNode
                        );
                }

                OutputFeed.AddFeedItem(OutputFeedItemNode);
            }

            foreach (Microsoft.Samples.FeedSync.FeedItemNode MergeFeedItemNode in i_IncomingFeed.FeedItemNodes)
            {
                string FeedSyncID = MergeFeedItemNode.SyncNode.ID;

                //  Check if item exists in source feed - if so just
                //  continue loop because we would have processed it above
                if (i_LocalFeed.DoesFeedItemNodeExist(FeedSyncID))
                    continue;

                OutputFeed.AddFeedItem(MergeFeedItemNode);
            }

            return OutputFeed;
        }

        #region constructors

        private Feed(string i_Title, string i_Description, string i_Link, Microsoft.Samples.FeedSync.Feed.FeedTypes i_FeedType)
        {
            string XmlDocumentContents;
            
            if (i_FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
            {
                XmlDocumentContents = System.String.Format
                    (
                    Microsoft.Samples.FeedSync.Constants.ATOM_FEED_TEMPLATE,
                    Microsoft.Samples.FeedSync.MiscHelpers.XMLEncode(i_Title),
                    Microsoft.Samples.FeedSync.MiscHelpers.XMLEncode(i_Description),
                    Microsoft.Samples.FeedSync.MiscHelpers.XMLEncode(i_Link),
                    System.DateTime.Now.ToUniversalTime().ToString(Microsoft.Samples.FeedSync.Constants.DATE_STRING_FORMAT),
                    System.Reflection.Assembly.GetExecutingAssembly().Location,
                    System.Guid.NewGuid().ToString()
                    );
            }
            else
            {
                XmlDocumentContents = System.String.Format
                    (
                    Microsoft.Samples.FeedSync.Constants.RSS_FEED_TEMPLATE,
                    Microsoft.Samples.FeedSync.MiscHelpers.XMLEncode(i_Title),
                    Microsoft.Samples.FeedSync.MiscHelpers.XMLEncode(i_Description),
                    Microsoft.Samples.FeedSync.MiscHelpers.XMLEncode(i_Link)
                    );
            }

            m_XmlDocument = new System.Xml.XmlDocument();
            m_XmlDocument.LoadXml(XmlDocumentContents);

            m_XmlNamespaceManager = new System.Xml.XmlNamespaceManager(m_XmlDocument.NameTable);

            this.InitializeXmlNamespaceManager(false);
 
            m_FeedType = i_FeedType;
            if (m_FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
            {
                m_FeedItemXPathQuery = System.String.Format
                    (
                    "{0}:{1}",
                    m_AtomNamespacePrefix,
                    Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME
                    );

                if (m_XmlNamespaceManager.DefaultNamespace == Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
                    m_FeedItemElementName = Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME;
                else
                    m_FeedItemElementName = m_FeedItemXPathQuery;
            }
            else
            {
                m_FeedItemXPathQuery = Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_ELEMENT_NAME;
                m_FeedItemElementName = m_FeedItemXPathQuery;
            }

            this.Initialize();
        }

        private Feed(System.Xml.XmlDocument i_FeedSyncXmlDocument)
        {
            m_XmlDocument = i_FeedSyncXmlDocument;
            m_XmlNamespaceManager = new System.Xml.XmlNamespaceManager(m_XmlDocument.NameTable);

            if (m_XmlDocument.DocumentElement.LocalName == Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME)
                m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom;
            else
                m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.RSS;

            this.InitializeXmlNamespaceManager(false);

            if (m_FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
            {
                m_FeedItemXPathQuery = System.String.Format
                    (
                    "{0}:{1}",
                    m_AtomNamespacePrefix,
                    Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME
                    );

                if (m_XmlNamespaceManager.DefaultNamespace == Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
                    m_FeedItemElementName = Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME;
                else
                    m_FeedItemElementName = m_FeedItemXPathQuery;
            }
            else
            {
                m_FeedItemXPathQuery = Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_ELEMENT_NAME;
                m_FeedItemElementName = m_FeedItemXPathQuery;
            }

            //  Get reference to 'sx:sharing' element
            string XPathQuery = System.String.Format
                (
                "{0}:{1}",
                m_FeedSyncNamespacePrefix,
                Microsoft.Samples.FeedSync.Constants.SHARING_ELEMENT_NAME
                );

            System.Xml.XmlElement SharingXmlElement = (System.Xml.XmlElement)this.ItemContainerXmlElement.SelectSingleNode
                (
                XPathQuery, 
                m_XmlNamespaceManager
                );

            if (SharingXmlElement != null)
                m_SharingNode = new Microsoft.Samples.FeedSync.SharingNode(this, SharingXmlElement);

            //  Iterate items
            System.Xml.XmlNodeList FeedItemXmlNodeList = this.ItemContainerXmlElement.SelectNodes
                (
                m_FeedItemXPathQuery,
                m_XmlNamespaceManager
                );

            foreach (System.Xml.XmlElement FeedItemXmlElement in FeedItemXmlNodeList)
            {
                Microsoft.Samples.FeedSync.FeedItemNode FeedItemNode = Microsoft.Samples.FeedSync.FeedItemNode.CreateFromXmlElement
                    (
                    this, 
                    FeedItemXmlElement
                    );

                m_FeedItemNodeSortedList[FeedItemNode.SyncNode.ID] = FeedItemNode;
            }

            this.Initialize();
        }

        private Feed(System.Xml.XmlDocument i_FeedXmlDocument, int? i_SequenceIDBase, string i_Since, string i_Until)
        {
            m_XmlDocument = i_FeedXmlDocument;
            m_XmlNamespaceManager = new System.Xml.XmlNamespaceManager(m_XmlDocument.NameTable);

            if (m_XmlDocument.DocumentElement.LocalName == Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME)
                m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom;
            else
                m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.RSS;

            this.InitializeXmlNamespaceManager(true);

            if (m_FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
            {
                m_FeedItemXPathQuery = System.String.Format
                    (
                    "{0}:{1}",
                    m_AtomNamespacePrefix,
                    Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME
                    );

                if (m_XmlNamespaceManager.DefaultNamespace == Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
                    m_FeedItemElementName = Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME;
                else
                    m_FeedItemElementName = m_FeedItemXPathQuery;
            }
            else
            {
                m_FeedItemXPathQuery = Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_ELEMENT_NAME;
                m_FeedItemElementName = m_FeedItemXPathQuery;
            }

            //  Get reference to 'sx:sharing' element
            string XPathQuery = System.String.Format
                (
                "{0}:{1}",
                m_FeedSyncNamespacePrefix,
                Microsoft.Samples.FeedSync.Constants.SHARING_ELEMENT_NAME
                );

            System.Xml.XmlElement SharingXmlElement = (System.Xml.XmlElement)this.ItemContainerXmlElement.SelectSingleNode
                (
                XPathQuery,
                m_XmlNamespaceManager
                );

            XPathQuery = System.String.Format
                (
                "descendant::*[{0}:{1}]",
                m_FeedSyncNamespacePrefix,
                Microsoft.Samples.FeedSync.Constants.SYNC_ELEMENT_NAME
                );

            System.Xml.XmlNodeList SyncXmlNodeList = this.ItemContainerXmlElement.SelectNodes
                (
                XPathQuery,
                m_XmlNamespaceManager
                );

            if ((SharingXmlElement != null) || (SyncXmlNodeList.Count > 0))
                throw new System.Exception("Document is already a valid FeedSync document!");

            System.Xml.XmlNodeList FeedItemXmlNodeList = this.ItemContainerXmlElement.SelectNodes
                (
                m_FeedItemXPathQuery,
                m_XmlNamespaceManager
                );

            //  BIG HONKING NOTE:  Iterate nodes using index instead of enumerator
            for (int Index = 0; Index < FeedItemXmlNodeList.Count; ++Index)
            {
                System.Xml.XmlElement FeedItemXmlElement = (System.Xml.XmlElement)FeedItemXmlNodeList[Index];

                //  Remove item from document
                FeedItemXmlElement.ParentNode.RemoveChild(FeedItemXmlElement);

                string SyncNodeID = null;
                if (i_SequenceIDBase != null)
                {
                    ++i_SequenceIDBase;
                    SyncNodeID = i_SequenceIDBase.ToString();
                }

                //  Create new FeedItemNode and add it to feed
                Microsoft.Samples.FeedSync.FeedItemNode FeedItemNode = Microsoft.Samples.FeedSync.FeedItemNode.CreateNewFromXmlElement
                    (
                    this,
                    FeedItemXmlElement,
                    SyncNodeID
                    );

                this.AddFeedItem(FeedItemNode);
            }

            this.Initialize();
        }

        #endregion

        private void Initialize()
        {
            string XPathQuery;

            #region Feed Title related

            if (m_FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
            {
                XPathQuery = System.String.Format
                    (
                    "/{0}:{1}/{2}:{3}",
                    m_AtomNamespacePrefix,
                    Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME,
                    m_AtomNamespacePrefix,
                    Microsoft.Samples.FeedSync.Constants.ATOM_TITLE_ELEMENT_NAME
                    );
            }
            else
            {
                XPathQuery = System.String.Format
                    (
                    "{0}/{1}",
                    Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_CONTAINER_ELEMENT_NAME,
                    Microsoft.Samples.FeedSync.Constants.RSS_TITLE_ELEMENT_NAME
                    );
            }

            //  Get title
            System.Xml.XmlNode TitleXmlNode = m_XmlDocument.DocumentElement.SelectSingleNode(XPathQuery, m_XmlNamespaceManager);
            if (TitleXmlNode != null)
                m_Title = TitleXmlNode.InnerText;

            #endregion

            #region Feed Description related

            if (m_FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
            {
                XPathQuery = System.String.Format
                    (
                    "{0}/{1}",
                    Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME,
                    Microsoft.Samples.FeedSync.Constants.ATOM_FEED_DESCRIPTION_ELEMENT_NAME
                    );
            }
            else
            {
                XPathQuery = System.String.Format
                    (
                    "{0}/{1}",
                    Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_CONTAINER_ELEMENT_NAME,
                    Microsoft.Samples.FeedSync.Constants.RSS_DESCRIPTION_ELEMENT_NAME
                    );
            }

            //  Get description
            System.Xml.XmlNode DescriptionXmlNode = m_XmlDocument.DocumentElement.SelectSingleNode(XPathQuery);
            if (DescriptionXmlNode != null)
                m_Description = DescriptionXmlNode.InnerText;

            #endregion

            #region Feed Link related

            if (m_FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
            {
                XPathQuery = System.String.Format
                    (
                    "{0}/{1}",
                    Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_CONTAINER_ELEMENT_NAME,
                    Microsoft.Samples.FeedSync.Constants.ATOM_LINK_ELEMENT_NAME
                    );
            }
            else
            {
                XPathQuery = System.String.Format
                    (
                    "{0}/{1}",
                    Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_CONTAINER_ELEMENT_NAME,
                    Microsoft.Samples.FeedSync.Constants.RSS_LINK_ELEMENT_NAME
                    );
            }

            //  Get link
            System.Xml.XmlNode LinkXmlNode = m_XmlDocument.DocumentElement.SelectSingleNode(XPathQuery);
            if (LinkXmlNode != null)
                m_Link = LinkXmlNode.InnerText;

            #endregion
        }

        private void InitializeXmlNamespaceManager(bool i_AddFeedSyncNamespace)
        {
            System.Diagnostics.Debug.Assert(m_XmlDocument != null);
            System.Diagnostics.Debug.Assert(m_XmlNamespaceManager != null);

            if (i_AddFeedSyncNamespace)
            {
                m_XmlDocument.DocumentElement.SetAttribute
                    (
                    "xmlns:" + Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX,
                    Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI
                    );
            }

            #region Populate namespaces into XmlNamespaceManager instance

            System.Xml.XmlReader XmlReader = new System.Xml.XmlNodeReader(m_XmlDocument);

            while (XmlReader.Read())
            {
                XmlReader.MoveToFirstAttribute();
                do
                {
                    if (XmlReader.Name.StartsWith("xmlns:") || (XmlReader.Name == "xmlns"))
                    {
                        string NamespacePrefix = System.String.Empty;
                        string NamespaceValue = System.String.Empty;

                        if (XmlReader.Name.StartsWith("xmlns:"))
                            NamespacePrefix = XmlReader.Name.Substring(XmlReader.Name.IndexOf(':') + 1);

                        NamespaceValue = XmlReader.Value;

                        if (!System.String.IsNullOrEmpty(NamespaceValue))
                            m_XmlNamespaceManager.AddNamespace(NamespacePrefix, NamespaceValue);
                    }
                }
                while (XmlReader.MoveToNextAttribute());
            }

            XmlReader.Close();

            #endregion

            #region Get/set namespace prefix for subsequent xpath queries

            foreach (System.Collections.Generic.KeyValuePair<string, string> KeyValuePair in m_XmlNamespaceManager.GetNamespacesInScope(System.Xml.XmlNamespaceScope.All))
            {
                if (KeyValuePair.Value == Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI)
                {
                    if (KeyValuePair.Key == Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX)
                    {
                        m_FeedSyncNamespacePrefix = Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX;
                        continue;
                    }

                    //  Check for default namespace
                    if (System.String.IsNullOrEmpty(KeyValuePair.Key))
                    {
                        //  See if sse prefix used
                        if (!m_XmlNamespaceManager.HasNamespace(Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX))
                        {
                            m_FeedSyncNamespacePrefix = Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX;
                        }
                        else
                        {
                            //  See if sse prefix is mapped to atom namespace uri
                            string NamespaceURI = m_XmlNamespaceManager.LookupNamespace(Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX);
                            if (NamespaceURI == Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI)
                                m_FeedSyncNamespacePrefix = Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX;
                            else
                            {
                                //  Create a new prefix
                                m_FeedSyncNamespacePrefix = Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_PREFIX + System.Guid.NewGuid().ToString();
                            }
                        }

                        //  Add namespace with mapped prefix
                        m_XmlNamespaceManager.AddNamespace
                            (
                            m_FeedSyncNamespacePrefix,
                            Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI
                            );
                    }

                    if (System.String.IsNullOrEmpty(m_FeedSyncNamespacePrefix))
                        throw new System.Exception("Invalid FeedSync XML document!");
                }
                else if (KeyValuePair.Value == Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
                {
                    if (KeyValuePair.Key == Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX)
                    {
                        m_AtomNamespacePrefix = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX;
                        continue;
                    }

                    //  Check for default namespace
                    if (System.String.IsNullOrEmpty(KeyValuePair.Key))
                    {
                        //  See if atom prefix used
                        if (!m_XmlNamespaceManager.HasNamespace(Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX))
                        {
                            m_AtomNamespacePrefix = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX;
                        }
                        else
                        {
                            //  See if atom prefix is mapped to atom namespace uri
                            string NamespaceURI = m_XmlNamespaceManager.LookupNamespace(Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX);
                            if (NamespaceURI == Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
                                m_AtomNamespacePrefix = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX;
                            else
                            {
                                //  Create a new prefix
                                m_AtomNamespacePrefix = Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_PREFIX + System.Guid.NewGuid().ToString();
                            }
                        }

                        //  Add namespace with mapped prefix
                        m_XmlNamespaceManager.AddNamespace
                            (
                            m_AtomNamespacePrefix,
                            Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI
                            );
                    }

                    if (System.String.IsNullOrEmpty(m_AtomNamespacePrefix))
                        throw new System.Exception("Invalid Atom XML document!");
                }
            }

            #endregion
        }

        public string Title
        {
            get
            {
                return m_Title;
            }
        }

        public string Description
        {
            get
            {
                return m_Description;
            }
        }

        public string Link
        {
            get
            {
                return m_Link;
            }
        }

        public string FeedSyncNamespacePrefix
        {
            get
            {
                return m_FeedSyncNamespacePrefix;
            }
        }

        public string AtomNamespacePrefix
        {
            get
            {
                return m_AtomNamespacePrefix;
            }
        }

        public System.Xml.XmlDocument XmlDocument
        {
            get
            {
                return m_XmlDocument;
            }
        }

        public System.Xml.XmlElement ItemContainerXmlElement
        {
            get
            {
                if (m_ItemContainerXmlElement == null)
                {
                    if (m_FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
                        m_ItemContainerXmlElement = (System.Xml.XmlElement)m_XmlDocument.DocumentElement;
                    else
                        m_ItemContainerXmlElement = (System.Xml.XmlElement)m_XmlDocument.DocumentElement.SelectSingleNode(Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_CONTAINER_ELEMENT_NAME);

                    System.Diagnostics.Debug.Assert(m_ItemContainerXmlElement != null);
                }

                return m_ItemContainerXmlElement;
            }
        }

        public Microsoft.Samples.FeedSync.SharingNode FeedSyncSharingNode
        {
            get
            {
                return m_SharingNode;
            }
        }

        public System.Xml.XmlNamespaceManager XmlNamespaceManager
        {
            get
            {
                return m_XmlNamespaceManager;
            }
        }

        public Microsoft.Samples.FeedSync.Feed.FeedTypes FeedType
        {
            get
            {
                return m_FeedType;
            }
        }

        public void AddFeedItem(Microsoft.Samples.FeedSync.FeedItemNode i_FeedItemNode)
        {
            if (i_FeedItemNode.XmlElement.OwnerDocument != m_XmlDocument)
                i_FeedItemNode = this.ImportFeedItemNode(i_FeedItemNode);

            //  Append feed item element to 'channel' element
            this.ItemContainerXmlElement.AppendChild(i_FeedItemNode.XmlElement);

            //  Cache item in hashtable
            m_FeedItemNodeSortedList[i_FeedItemNode.SyncNode.ID] = i_FeedItemNode;
        }

        #region UpdateFeedItem methods

        public void UpdateFeedItem(Microsoft.Samples.FeedSync.FeedItemNode i_FeedItemNode)
        {
            //  Get item in hashtable
            Microsoft.Samples.FeedSync.FeedItemNode FeedItemNode = m_FeedItemNodeSortedList[i_FeedItemNode.SyncNode.ID];

            //  Get feed item element to 'channel' element
            this.ItemContainerXmlElement.ReplaceChild(i_FeedItemNode.XmlElement, FeedItemNode.XmlElement);

            //  Cache item in hashtable
            m_FeedItemNodeSortedList[i_FeedItemNode.SyncNode.ID] = i_FeedItemNode;
        }

        public bool UpdateFeedItem(string i_Title, string i_Description, string i_SyncNodeID, string i_When, string i_By)
        {
            return this.UpdateFeedItem
                (
                i_Title,
                i_Description,
                i_SyncNodeID,
                System.Convert.ToDateTime(i_When),
                i_By,
                false
                );
        }

        public bool UpdateFeedItem(string i_Title, string i_Description, string i_SyncNodeID, System.DateTime i_WhenDateTime, string i_By)
        {
            return this.UpdateFeedItem
                (
                i_Title,
                i_Description,
                i_SyncNodeID,
                i_WhenDateTime,
                i_By,
                false
                );
        }

        public bool UpdateFeedItem(string i_Title, string i_Description, string i_SyncNodeID, string i_When, string i_By, bool i_ResolveConflicts)
        {
            return this.UpdateFeedItem
                (
                i_Title,
                i_Description,
                i_SyncNodeID,
                System.Convert.ToDateTime(i_When),
                i_By,
                i_ResolveConflicts
                );
        }

        public bool UpdateFeedItem(string i_Title, string i_Description, string i_SyncNodeID, System.DateTime i_WhenDateTime, string i_By, bool i_ResolveConflicts)
        {
            //  Make sure item already exists
            if (!m_FeedItemNodeSortedList.ContainsKey(i_SyncNodeID))
                throw new System.Exception("FeedItem doesn't exist - ID = " + i_SyncNodeID);

            Microsoft.Samples.FeedSync.FeedItemNode FeedItemNode = (Microsoft.Samples.FeedSync.FeedItemNode)m_FeedItemNodeSortedList[i_SyncNodeID];

            bool UpdateSucceeded = FeedItemNode.Update
                (
                i_Title,
                i_Description,
                i_WhenDateTime,
                i_By,
                i_ResolveConflicts
                );

            return UpdateSucceeded;
        }

        #endregion

        #region DeleteFeedItem methods

        public bool DeleteFeedItem(string i_SyncNodeID)
        {
            return this.DeleteFeedItem
                (
                i_SyncNodeID,
                System.DateTime.Now,
                ""
                );
        }
        
        public bool DeleteFeedItem(string i_SyncNodeID, string i_When, string i_By)
        {
            return this.DeleteFeedItem
                (
                i_SyncNodeID,
                System.Convert.ToDateTime(i_When),
                i_By
                );
        }

        public bool DeleteFeedItem(string i_SyncNodeID, System.DateTime i_WhenDateTime, string i_By)
        {
            //  Make sure item already exists
            if (!m_FeedItemNodeSortedList.ContainsKey(i_SyncNodeID))
                throw new System.Exception("FeedItem doesn't exist - ID = " + i_SyncNodeID);

            Microsoft.Samples.FeedSync.FeedItemNode FeedItemNode = (Microsoft.Samples.FeedSync.FeedItemNode)m_FeedItemNodeSortedList[i_SyncNodeID];

            bool DeleteSucceeded = FeedItemNode.Delete
                (
                i_WhenDateTime,
                i_By
                );

            return DeleteSucceeded;
        }

        #endregion

        #region PruneFeedItemNode methods

        public bool PruneFeedItemNode(string i_SyncNodeID)
        {
            if (!m_FeedItemNodeSortedList.ContainsKey(i_SyncNodeID))
                return false;

            Microsoft.Samples.FeedSync.FeedItemNode FeedItemNode = (Microsoft.Samples.FeedSync.FeedItemNode)m_FeedItemNodeSortedList[i_SyncNodeID];
            return this.PruneFeedItemNode(FeedItemNode);
        }

        public bool PruneFeedItemNode(Microsoft.Samples.FeedSync.FeedItemNode i_FeedItemNode)
        {
            m_FeedItemNodeSortedList.Remove(i_FeedItemNode.SyncNode.ID);

            System.Xml.XmlElement FeedItemXmlElement = i_FeedItemNode.XmlElement;
            FeedItemXmlElement.ParentNode.RemoveChild(FeedItemXmlElement);

            return true;
        }

        #endregion

        #region PruneAllFeedItemNodes

        public void PruneAllFeedItemNodes()
        {
            this.PruneAllFeedItemNodes(false);
        }

        public void PruneAllFeedItemNodes(bool i_ExcludeDeletedItems)
        {
            System.Collections.Generic.List<Microsoft.Samples.FeedSync.FeedItemNode> FeedItemNodeList = new System.Collections.Generic.List<FeedItemNode>();
            foreach (Microsoft.Samples.FeedSync.FeedItemNode FeedItemNode in m_FeedItemNodeSortedList.Values)
            {
                if (i_ExcludeDeletedItems && FeedItemNode.SyncNode.Deleted)
                    continue;

                FeedItemNodeList.Add(FeedItemNode);
            }

            foreach (Microsoft.Samples.FeedSync.FeedItemNode FeedItemNode in FeedItemNodeList)
                this.PruneFeedItemNode(FeedItemNode);
        }

        #endregion

        public bool FindFeedItemNode(string i_SyncNodeID, out Microsoft.Samples.FeedSync.FeedItemNode o_FeedItemNode)
        {
            o_FeedItemNode = null;

            if (m_FeedItemNodeSortedList.ContainsKey(i_SyncNodeID))
            {
                o_FeedItemNode = (Microsoft.Samples.FeedSync.FeedItemNode)m_FeedItemNodeSortedList[i_SyncNodeID];
                return true;
            }

            return false;
        }

        public bool DoesFeedItemNodeExist(string i_SyncNodeID)
        {
            return m_FeedItemNodeSortedList.ContainsKey(i_SyncNodeID);
        }

        public Microsoft.Samples.FeedSync.FeedItemNode GetFeedItemNode(string i_SyncNodeID)
        {
            //  Make sure item already exists
            if (!m_FeedItemNodeSortedList.ContainsKey(i_SyncNodeID))
                throw new System.Exception("FeedItem doesn't exist - ID = " + i_SyncNodeID);

            return (Microsoft.Samples.FeedSync.FeedItemNode)m_FeedItemNodeSortedList[i_SyncNodeID];
        }

        public Microsoft.Samples.FeedSync.FeedItemNode[] FeedItemNodes
        {
            get
            {
                System.Collections.Generic.List<Microsoft.Samples.FeedSync.FeedItemNode> FeedItemNodeList = new System.Collections.Generic.List<FeedItemNode>(m_FeedItemNodeSortedList.Values);
                return FeedItemNodeList.ToArray();
            }
        }

        public Microsoft.Samples.FeedSync.FeedItemNode ImportFeedItemNode(Microsoft.Samples.FeedSync.FeedItemNode i_FeedItemNode)
        {
            //  Import the node into the current document
            System.Xml.XmlElement ImportedFeedItemNodeXmlElement = (System.Xml.XmlElement)m_XmlDocument.ImportNode(i_FeedItemNode.XmlElement, true);

            Microsoft.Samples.FeedSync.FeedItemNode ImportedFeedItemNode = Microsoft.Samples.FeedSync.FeedItemNode.CreateFromXmlElement
                (
                this, 
                ImportedFeedItemNodeXmlElement
                );

            return ImportedFeedItemNode;
        }

        internal string FeedItemElementName
        {
            get
            {
                return m_FeedItemElementName;
            }
        }

        internal string FeedItemXPathQuery
        {
            get
            {
                return m_FeedItemXPathQuery;
            }
        }
    }
}
