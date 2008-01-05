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
    public class SyncNode : Microsoft.Samples.FeedSync.Node
    {
        private string m_ID;

        private int m_Updates;

        private bool m_NoConflicts = false;
        private bool m_Deleted = false;

        private System.Collections.Generic.SortedList<string, Microsoft.Samples.FeedSync.FeedItemNode> m_ConflictNodeList = new System.Collections.Generic.SortedList<string, Microsoft.Samples.FeedSync.FeedItemNode>();

        private System.Xml.XmlElement m_ConflictsNodeXmlElement = null;

        private System.Collections.Generic.List<Microsoft.Samples.FeedSync.HistoryNode> m_HistoryNodeList = new System.Collections.Generic.List<Microsoft.Samples.FeedSync.HistoryNode>();

        private Microsoft.Samples.FeedSync.FeedItemNode m_FeedItemNode;

        static public Microsoft.Samples.FeedSync.SyncNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, Microsoft.Samples.FeedSync.FeedItemNode i_FeedItemNode)
        {
            return Microsoft.Samples.FeedSync.SyncNode.CreateNew
                (
                i_Feed,
                i_FeedItemNode,
                System.Guid.NewGuid().ToString(),
                0,
                System.DateTime.Now,
                null,
                false,
                false,
                0
                );
        }

        static public Microsoft.Samples.FeedSync.SyncNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, Microsoft.Samples.FeedSync.FeedItemNode i_FeedItemNode, string i_SyncNodeID)
        {
            return Microsoft.Samples.FeedSync.SyncNode.CreateNew
                (
                i_Feed,
                i_FeedItemNode,
                i_SyncNodeID,
                0,
                System.DateTime.Now,
                null,
                false,
                false,
                0
                );
        }

        static public Microsoft.Samples.FeedSync.SyncNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, Microsoft.Samples.FeedSync.FeedItemNode i_FeedItemNode, string i_SyncNodeID, int i_Sequence, System.DateTime? i_WhenDateTime, string i_By, bool i_Deleted, bool i_NoConflicts, int i_Updates)
        {
            string ElementName = System.String.Format
                (
                "{0}:{1}",
                i_Feed.FeedSyncNamespacePrefix,
                Microsoft.Samples.FeedSync.Constants.SYNC_ELEMENT_NAME
                );

            System.Xml.XmlElement SyncNodeXmlElement = i_Feed.XmlDocument.CreateElement
                (
                ElementName, 
                Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI
                );

            SyncNodeXmlElement.SetAttribute
                (
                Microsoft.Samples.FeedSync.Constants.ID_ATTRIBUTE,
                i_SyncNodeID
                );

            SyncNodeXmlElement.SetAttribute
                (
                Microsoft.Samples.FeedSync.Constants.UPDATES_ATTRIBUTE,
                i_Updates.ToString()
                );

            SyncNodeXmlElement.SetAttribute
                (
                Microsoft.Samples.FeedSync.Constants.DELETED_ATTRIBUTE,
                i_Deleted ? "true" : "false"
                );

            SyncNodeXmlElement.SetAttribute
                (
                Microsoft.Samples.FeedSync.Constants.NO_CONFLICTS_ATTRIBUTE,
                i_NoConflicts ? "true" : "false"
                );

            Microsoft.Samples.FeedSync.SyncNode SyncNode = new Microsoft.Samples.FeedSync.SyncNode
                (
                i_FeedItemNode, 
                SyncNodeXmlElement
                );

            Microsoft.Samples.FeedSync.HistoryNode FeedSyncHistoryNode = Microsoft.Samples.FeedSync.HistoryNode.CreateNew
                (
                SyncNode, 
                i_WhenDateTime, 
                i_By
                );

            SyncNode.AddHistoryNode(FeedSyncHistoryNode);

            return SyncNode;
        }

        static public Microsoft.Samples.FeedSync.SyncNode CreateFromXmlElement(Microsoft.Samples.FeedSync.FeedItemNode i_FeedItemNode, System.Xml.XmlElement i_SyncNodeXmlElement)
        {
            if (i_SyncNodeXmlElement.OwnerDocument != i_FeedItemNode.Feed.XmlDocument)
                i_SyncNodeXmlElement = (System.Xml.XmlElement)i_FeedItemNode.Feed.XmlDocument.ImportNode(i_SyncNodeXmlElement, true);

            Microsoft.Samples.FeedSync.SyncNode SyncNode = new Microsoft.Samples.FeedSync.SyncNode(i_FeedItemNode, i_SyncNodeXmlElement);
            return SyncNode;
        }

        private SyncNode(Microsoft.Samples.FeedSync.FeedItemNode i_FeedItemNode, System.Xml.XmlElement i_SyncNodeXmlElement)
        {
            m_FeedItemNode = i_FeedItemNode;
            m_XmlElement = i_SyncNodeXmlElement;

            m_ID = m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.ID_ATTRIBUTE);
            m_Updates = System.Convert.ToInt32(m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.UPDATES_ATTRIBUTE));

            if (m_XmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.DELETED_ATTRIBUTE))
                m_Deleted = (m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.DELETED_ATTRIBUTE) == "true");

            if (m_XmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.NO_CONFLICTS_ATTRIBUTE))
                m_NoConflicts = (m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.NO_CONFLICTS_ATTRIBUTE) == "true");

            string XPathQuery = System.String.Format
                (
                "{0}:{1}",
                m_FeedItemNode.Feed.FeedSyncNamespacePrefix,
                Microsoft.Samples.FeedSync.Constants.HISTORY_ELEMENT_NAME
                );

            System.Xml.XmlNodeList HistoryNodeList = i_SyncNodeXmlElement.SelectNodes
                (
                XPathQuery, 
                i_FeedItemNode.Feed.XmlNamespaceManager
                );

            foreach (System.Xml.XmlElement HistoryNodeXmlElement in HistoryNodeList)
            {
                Microsoft.Samples.FeedSync.HistoryNode HistoryNode = Microsoft.Samples.FeedSync.HistoryNode.CreateFromXmlElement
                    (
                    this,
                    HistoryNodeXmlElement
                    );

                m_HistoryNodeList.Add(HistoryNode);
            }

            if (!m_NoConflicts)
            {
                XPathQuery = System.String.Format
                    (
                    "{0}:{1}",
                    m_FeedItemNode.Feed.FeedSyncNamespacePrefix,
                    Microsoft.Samples.FeedSync.Constants.CONFLICTS_ELEMENT_NAME
                    );

                m_ConflictsNodeXmlElement = (System.Xml.XmlElement)i_SyncNodeXmlElement.SelectSingleNode
                    (
                    XPathQuery,
                    m_FeedItemNode.Feed.XmlNamespaceManager
                    );

                if (m_ConflictsNodeXmlElement != null)
                {
                    System.Xml.XmlNodeList FeedItemXmlNodeList = m_ConflictsNodeXmlElement.SelectNodes
                        (
                        m_FeedItemNode.Feed.FeedItemXPathQuery,
                        m_FeedItemNode.Feed.XmlNamespaceManager
                        );

                    foreach (System.Xml.XmlElement ConflictFeedItemNodeXmlElement in FeedItemXmlNodeList)
                    {
                        Microsoft.Samples.FeedSync.FeedItemNode ConflictFeedItemNode = this.CreateConflictItemNodeFromXmlElement(ConflictFeedItemNodeXmlElement);

                        string Key = System.String.Format
                            (
                            "{0}{1}{2}",
                            ConflictFeedItemNode.SyncNode.Updates,
                            ConflictFeedItemNode.SyncNode.TopMostHistoryNode.Sequence,
                            ConflictFeedItemNode.SyncNode.TopMostHistoryNode.By
                            );

                        if (ConflictFeedItemNode.SyncNode.TopMostHistoryNode.WhenDateTime != null)
                            Key += ((System.DateTime)ConflictFeedItemNode.SyncNode.TopMostHistoryNode.WhenDateTime);

                        if (!m_ConflictNodeList.ContainsKey(Key))
                            m_ConflictNodeList.Add(Key, ConflictFeedItemNode);
                    }
                }
            }
        }

        public void RemoveAllConflictItemNodes()
        {
            if (m_ConflictNodeList.Count == 0)
                return;

            //  Delete "sx:conflicts" element
            m_ConflictsNodeXmlElement.ParentNode.RemoveChild(m_ConflictsNodeXmlElement);
            m_ConflictsNodeXmlElement = null;
            
            //  Empty node list
            m_ConflictNodeList.Clear();
        }

        public Microsoft.Samples.FeedSync.FeedItemNode CreateConflictItemNodeFromXmlElement(System.Xml.XmlElement i_ConflictFeedItemNodeXmlElement)
        {
            Microsoft.Samples.FeedSync.FeedItemNode ConflictFeedItemNode = Microsoft.Samples.FeedSync.FeedItemNode.CreateFromXmlElement
                (
                m_FeedItemNode.Feed,
                i_ConflictFeedItemNodeXmlElement
                );

            return ConflictFeedItemNode;
        }

        public void AddConflictItemNode(Microsoft.Samples.FeedSync.FeedItemNode i_ConflictFeedItemNode)
        {
            //  Create clone of item in case it's from different document
            Microsoft.Samples.FeedSync.FeedItemNode ImportedConflictFeedItemNode = m_FeedItemNode.Feed.ImportFeedItemNode(i_ConflictFeedItemNode);

            string Key = System.String.Format
                (
                "{0}{1}{2}",
                ImportedConflictFeedItemNode.SyncNode.Updates,
                ImportedConflictFeedItemNode.SyncNode.TopMostHistoryNode.Sequence,
                ImportedConflictFeedItemNode.SyncNode.TopMostHistoryNode.By
                );

            if (ImportedConflictFeedItemNode.SyncNode.TopMostHistoryNode.WhenDateTime != null)
                Key += ((System.DateTime)ImportedConflictFeedItemNode.SyncNode.TopMostHistoryNode.WhenDateTime);

            //  Check if if item already exists in either list
            if (m_ConflictNodeList.ContainsKey(Key))
            {
                if (m_ConflictNodeList.ContainsKey(Key))
                    throw new System.ArgumentException("SyncNode::AddConflictItemNode (" + Key + ") - item already exists as conflict");
            }

            //  Create "sx:conflicts" element if necessary
            if (m_ConflictsNodeXmlElement == null)
            {
                string ElementName = System.String.Format
                    (
                    "{0}:{1}",
                    m_FeedItemNode.Feed.FeedSyncNamespacePrefix,
                    Microsoft.Samples.FeedSync.Constants.CONFLICTS_ELEMENT_NAME
                    );

                m_ConflictsNodeXmlElement = m_FeedItemNode.Feed.XmlDocument.CreateElement
                    (
                    ElementName, 
                    Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI
                    );

                m_XmlElement.AppendChild(m_ConflictsNodeXmlElement);
            }

            //  Append conflict node's element to "sx:conflicts" element
            m_ConflictsNodeXmlElement.AppendChild(ImportedConflictFeedItemNode.XmlElement);

            //  Add node to list
            m_ConflictNodeList.Add(Key, ImportedConflictFeedItemNode);
        }

        public bool DoesConflictItemNodeExist(Microsoft.Samples.FeedSync.FeedItemNode i_FeedItemNode)
        {
            string Key = System.String.Format
                (
                "{0}{1}{2}",
                i_FeedItemNode.SyncNode.Updates,
                i_FeedItemNode.SyncNode.TopMostHistoryNode.Sequence,
                i_FeedItemNode.SyncNode.TopMostHistoryNode.By
                );

            if (i_FeedItemNode.SyncNode.TopMostHistoryNode.WhenDateTime != null)
                Key += ((System.DateTime)i_FeedItemNode.SyncNode.TopMostHistoryNode.WhenDateTime);

             return (m_ConflictNodeList.ContainsKey(Key));
        }

        public string ID
        {
            get
            {
                return m_ID;
            }
        }

        public int Updates
        {
            get
            {
                return m_Updates;
            }
            set
            {
                if (value < m_Updates)
                    throw new System.ArgumentException("Invalid value!");

                m_XmlElement.SetAttribute
                    (
                    Microsoft.Samples.FeedSync.Constants.UPDATES_ATTRIBUTE, 
                    value.ToString()
                    );

                m_Updates = value;
            }
        }

        public bool NoConflicts
        {
            get
            {
                return m_NoConflicts;
            }
        }

        public bool Deleted
        {
            get
            {
                return m_Deleted;
            }
            set
            {
                if (value == m_Deleted)
                    return;

                m_XmlElement.SetAttribute
                    (
                    Microsoft.Samples.FeedSync.Constants.DELETED_ATTRIBUTE,
                    value ? "true" : "false"
                    );

                m_Deleted = value;
            }
        }

        public Microsoft.Samples.FeedSync.HistoryNode TopMostHistoryNode
        {
            get
            {
                if (m_HistoryNodeList.Count == 0)
                    return null;

                return m_HistoryNodeList[0];
            }
        }

        public Microsoft.Samples.FeedSync.HistoryNode[] HistoryNodes
        {
            get
            {
                return m_HistoryNodeList.ToArray();
            }
        }

        public Microsoft.Samples.FeedSync.FeedItemNode[] ConflictFeedItemNodes
        {
            get
            {
                System.Collections.Generic.List<Microsoft.Samples.FeedSync.FeedItemNode> ConflictFeedItemNodeList = new System.Collections.Generic.List<Microsoft.Samples.FeedSync.FeedItemNode>(m_ConflictNodeList.Values);
                return ConflictFeedItemNodeList.ToArray();
            }
        }

        public Microsoft.Samples.FeedSync.FeedItemNode FeedItemNode
        {
            get
            {
                return m_FeedItemNode;
            }
        }

        internal void AddHistoryNode(Microsoft.Samples.FeedSync.HistoryNode i_HistoryNode)
        {
            System.Xml.XmlElement ImportedHistoryXmlElement = (System.Xml.XmlElement)m_FeedItemNode.Feed.XmlDocument.ImportNode
                (
                i_HistoryNode.XmlElement, 
                true
                );

            Microsoft.Samples.FeedSync.HistoryNode ImportedHistoryNode = Microsoft.Samples.FeedSync.HistoryNode.CreateFromXmlElement
                (
                this,
                ImportedHistoryXmlElement
                );

            if (m_XmlElement.ChildNodes.Count > 0)
            {
                m_XmlElement.InsertBefore
                    (
                    ImportedHistoryNode.XmlElement,
                    m_XmlElement.ChildNodes[0]
                    );
            }
            else
                m_XmlElement.AppendChild(ImportedHistoryNode.XmlElement);

            //  Make sure new history node is first node in list
            m_HistoryNodeList.Insert
                (
                0, 
                ImportedHistoryNode
                );

            //  Remember not to use m_Updates here because
            //  property accessor modifies xml element
            this.Updates = i_HistoryNode.Sequence;
        }

        internal void AddConflictHistoryNode(Microsoft.Samples.FeedSync.HistoryNode i_ConflictHistoryNode)
        {
            System.Xml.XmlElement ImportedConflictHistoryXmlElement = (System.Xml.XmlElement)m_FeedItemNode.Feed.XmlDocument.ImportNode
                (
                i_ConflictHistoryNode.XmlElement, 
                true
                );

            Microsoft.Samples.FeedSync.HistoryNode ImportedConflictHistoryNode = Microsoft.Samples.FeedSync.HistoryNode.CreateFromXmlElement
                (
                this,
                ImportedConflictHistoryXmlElement
                );

            //  Insert after topmost history element
            m_XmlElement.InsertBefore
                (
                ImportedConflictHistoryNode.XmlElement,
                this.TopMostHistoryNode.XmlElement.NextSibling
                );

            m_HistoryNodeList.Add(ImportedConflictHistoryNode);
        }
    }
}
