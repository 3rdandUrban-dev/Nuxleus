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
    public class HistoryNode : Microsoft.Samples.FeedSync.Node
    {
        private System.DateTime? m_WhenDateTime;

        private string m_By;
        
        private int m_Sequence;

        private Microsoft.Samples.FeedSync.SyncNode m_SyncNode;

        static public Microsoft.Samples.FeedSync.HistoryNode CreateNew(Microsoft.Samples.FeedSync.SyncNode i_SyncNode, System.DateTime? i_WhenDateTime, string i_By)
        {
            Microsoft.Samples.FeedSync.Feed Feed = i_SyncNode.FeedItemNode.Feed;

            string ElementName = System.String.Format
                (
                "{0}:{1}",
                Feed.FeedSyncNamespacePrefix,
                Microsoft.Samples.FeedSync.Constants.HISTORY_ELEMENT_NAME
                );

            System.Xml.XmlElement HistoryNodeXmlElement = Feed.XmlDocument.CreateElement
                (
                ElementName,
                Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI
                );

            if (i_WhenDateTime != null)
            {
                System.DateTime WhenDateTime = ((System.DateTime)i_WhenDateTime).ToUniversalTime();
                string When = WhenDateTime.ToString(Microsoft.Samples.FeedSync.Constants.DATE_STRING_FORMAT);

                HistoryNodeXmlElement.SetAttribute
                    (
                    Microsoft.Samples.FeedSync.Constants.WHEN_ATTRIBUTE,
                    When
                    );
            }

            int Sequence = i_SyncNode.Updates + 1;

            if (!System.String.IsNullOrEmpty(i_By))
            {
                HistoryNodeXmlElement.SetAttribute
                    (
                    Microsoft.Samples.FeedSync.Constants.BY_ATTRIBUTE,
                    i_By
                    );

                foreach (Microsoft.Samples.FeedSync.HistoryNode ExistingHistoryNode in i_SyncNode.HistoryNodes)
                {
                    if ((ExistingHistoryNode.By == i_By) && (ExistingHistoryNode.Sequence >= Sequence))
                        Sequence = ExistingHistoryNode.Sequence + 1;
                }
            }

            HistoryNodeXmlElement.SetAttribute
                (
                Microsoft.Samples.FeedSync.Constants.SEQUENCE_ATTRIBUTE,
                (i_SyncNode.Updates + 1).ToString()
                );

            Microsoft.Samples.FeedSync.HistoryNode HistoryNode = new Microsoft.Samples.FeedSync.HistoryNode
                (
                null,
                HistoryNodeXmlElement
                );

            return HistoryNode;
        }

        static public Microsoft.Samples.FeedSync.HistoryNode CreateFromXmlElement(Microsoft.Samples.FeedSync.SyncNode i_SyncNode, System.Xml.XmlElement i_HistoryNodeXmlElement)
        {
            if (i_HistoryNodeXmlElement.OwnerDocument != i_SyncNode.FeedItemNode.Feed.XmlDocument)
                i_HistoryNodeXmlElement = (System.Xml.XmlElement)i_SyncNode.FeedItemNode.Feed.XmlDocument.ImportNode(i_HistoryNodeXmlElement, true);

            Microsoft.Samples.FeedSync.HistoryNode HistoryNode = new Microsoft.Samples.FeedSync.HistoryNode
                (
                i_SyncNode,
                i_HistoryNodeXmlElement
                );

            return HistoryNode;
        }

        private HistoryNode(Microsoft.Samples.FeedSync.SyncNode i_SyncNode, System.Xml.XmlElement i_HistoryNodeXmlElement)
        {
            bool InvalidXmlElement =
                (i_HistoryNodeXmlElement.LocalName != Microsoft.Samples.FeedSync.Constants.HISTORY_ELEMENT_NAME) ||
                (i_HistoryNodeXmlElement.NamespaceURI != Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI);

            if (InvalidXmlElement)
                throw new System.Exception("Invalid xml element!");

            m_SyncNode = i_SyncNode;
            m_XmlElement = i_HistoryNodeXmlElement;

            m_Sequence = System.Convert.ToInt32(i_HistoryNodeXmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.SEQUENCE_ATTRIBUTE));

            if (i_HistoryNodeXmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.WHEN_ATTRIBUTE))
            {
                string When = i_HistoryNodeXmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.WHEN_ATTRIBUTE);
                m_WhenDateTime = System.Convert.ToDateTime(When);
            }
            
            if (i_HistoryNodeXmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.BY_ATTRIBUTE))
                m_By = i_HistoryNodeXmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.BY_ATTRIBUTE);

            if ((m_WhenDateTime == null) && (System.String.IsNullOrEmpty(m_By)))
                throw new System.ArgumentException("Must have 'when' or 'by' attribute!");

            if (m_Sequence > (2^32 - 1))
                throw new System.ArgumentException("Invalid value for 'sequence' attribute!");
        }

        public System.DateTime? WhenDateTime
        {
            get
            {
                return m_WhenDateTime;
            }
        }

        public string By
        {
            get
            {
                return m_By;
            }
        }

        public int Sequence
        {
            get
            {
                return m_Sequence;
            }
        }

        public Microsoft.Samples.FeedSync.SyncNode SyncNode
        {
            get
            {
                return m_SyncNode;
            }
        }

        public bool IsSubsumedByHistoryNode(Microsoft.Samples.FeedSync.HistoryNode i_HistoryNode)
        {
            bool Subsumed = false;

            if (!System.String.IsNullOrEmpty(m_By))
            {
                Subsumed =
                    (m_By == i_HistoryNode.By) &&
                    (i_HistoryNode.Sequence >= m_Sequence);
            }
            else
            {
                Subsumed =
                    (m_WhenDateTime == i_HistoryNode.WhenDateTime) &&
                    (m_Sequence == i_HistoryNode.Sequence);
            }

            return Subsumed;
        }
    }
}
