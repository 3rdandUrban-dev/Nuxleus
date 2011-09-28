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
    public class SharingNode : Microsoft.Samples.FeedSync.Node
    {
        private string m_Since;
        private string m_Until;

        private System.DateTime? m_Expires = null;

        private Microsoft.Samples.FeedSync.Feed m_Feed;
        
        private System.Collections.Generic.List<Microsoft.Samples.FeedSync.RelatedNode> m_RelatedNodeList = new System.Collections.Generic.List<Microsoft.Samples.FeedSync.RelatedNode>();

        static public Microsoft.Samples.FeedSync.SharingNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed)
        {
            return Microsoft.Samples.FeedSync.SharingNode.CreateNew
                (
                i_Feed,
                null,
                null,
                null
                );
        }

        static public Microsoft.Samples.FeedSync.SharingNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Since, string i_Until)
        {
            return Microsoft.Samples.FeedSync.SharingNode.CreateNew
                (
                i_Feed,
                i_Since,
                i_Until,
                null
                );
        }

        static public Microsoft.Samples.FeedSync.SharingNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Since, string i_Until, System.DateTime? i_Expires)
        {
            string ElementName = System.String.Format
                (
                "{0}:{1}",
                i_Feed.FeedSyncNamespacePrefix,
                Microsoft.Samples.FeedSync.Constants.SHARING_ELEMENT_NAME
                );

            System.Xml.XmlElement SharingXmlElement = i_Feed.XmlDocument.CreateElement
                (
                ElementName,
                Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI
                );

            if (!System.String.IsNullOrEmpty(i_Since))
                SharingXmlElement.SetAttribute(Microsoft.Samples.FeedSync.Constants.SINCE_ATTRIBUTE, i_Since);

            if (!System.String.IsNullOrEmpty(i_Until))
                SharingXmlElement.SetAttribute(Microsoft.Samples.FeedSync.Constants.UNTIL_ATTRIBUTE, i_Until);

            if (i_Expires != null)
                SharingXmlElement.SetAttribute(Microsoft.Samples.FeedSync.Constants.EXPIRES_ATTRIBUTE, ((System.DateTime)i_Expires).ToString(Microsoft.Samples.FeedSync.Constants.DATE_STRING_FORMAT));

            return new Microsoft.Samples.FeedSync.SharingNode(i_Feed, SharingXmlElement);
        }

        public SharingNode(Microsoft.Samples.FeedSync.Feed i_Feed, System.Xml.XmlElement i_SharingXmlElement)
        {
            m_Feed = i_Feed;

            m_XmlElement = i_SharingXmlElement;

            if (m_XmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.SINCE_ATTRIBUTE))
                m_Since = m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.SINCE_ATTRIBUTE);

            if (m_XmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.UNTIL_ATTRIBUTE))
                m_Until = m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.SINCE_ATTRIBUTE);

            if (m_XmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.EXPIRES_ATTRIBUTE))
            {
                string Expires = m_XmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.EXPIRES_ATTRIBUTE);
                System.DateTime ExpiresDateTime;

                if (System.DateTime.TryParse(Expires, out ExpiresDateTime))
                    m_Expires = ExpiresDateTime;
            }

            string XPathQuery = System.String.Format
                (
                "{0}:{1}",
                m_Feed.FeedSyncNamespacePrefix,
                Microsoft.Samples.FeedSync.Constants.RELATED_ELEMENT_NAME
                );

            System.Xml.XmlNodeList RelatedNodeList = i_SharingXmlElement.SelectNodes
                (
                XPathQuery,
                m_Feed.XmlNamespaceManager
                );

            foreach (System.Xml.XmlElement RelatedNodeXmlElement in RelatedNodeList)
            {
                Microsoft.Samples.FeedSync.RelatedNode RelatedNode = Microsoft.Samples.FeedSync.RelatedNode.CreateFromXmlElement
                    (
                    this,
                    RelatedNodeXmlElement
                    );

                m_RelatedNodeList.Add(RelatedNode);
            }
        }

        public string Since
        {
            get
            {
                return m_Since;
            }
            set
            {
                if (System.String.IsNullOrEmpty(value))
                    throw new System.ArgumentException("Invalid value!");
                
                m_Since = value;
                m_XmlElement.SetAttribute(Microsoft.Samples.FeedSync.Constants.SINCE_ATTRIBUTE, value);
            }
        }

        public string Until
        {
            get
            {
                return m_Until;
            }
            set
            {
                if (System.String.IsNullOrEmpty(value))
                    throw new System.ArgumentException("Invalid value!");

                m_Until = value;
                m_XmlElement.SetAttribute(Microsoft.Samples.FeedSync.Constants.UNTIL_ATTRIBUTE, value);
            }
        }

        public System.DateTime? Expires
        {
            get
            {
                return m_Expires;
            }
        }

        public Microsoft.Samples.FeedSync.Feed Feed
        {
            get
            {
                return m_Feed;
            }
        }

        public void AddRelatedNode(Microsoft.Samples.FeedSync.RelatedNode i_RelatedNode)
        {
            System.Xml.XmlElement ImportedRelatedXmlElement = (System.Xml.XmlElement)m_Feed.XmlDocument.ImportNode
                (
                i_RelatedNode.XmlElement,
                true
                );

            Microsoft.Samples.FeedSync.RelatedNode ImportedRelatedNode = Microsoft.Samples.FeedSync.RelatedNode.CreateFromXmlElement
                (
                this,
                ImportedRelatedXmlElement
                );

            m_XmlElement.AppendChild(ImportedRelatedNode.XmlElement);
            m_RelatedNodeList.Add(ImportedRelatedNode);
        }
    }
}
