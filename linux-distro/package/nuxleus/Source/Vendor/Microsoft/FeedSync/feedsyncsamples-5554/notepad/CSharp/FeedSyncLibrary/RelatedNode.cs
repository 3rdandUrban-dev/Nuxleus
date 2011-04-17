/*****************************************************************************************
   
   Copyright (c) Microsoft Corporation. All rights reserved.

   Use of this code sample is subject to the terms of the Microsoft
   Permissive License, a copy of which should always be distributed with
   this file.  You can also access a copy of this license agreement at:
   http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx

 ****************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.FeedSync
{
    public class RelatedNode : Microsoft.Samples.FeedSync.Node
    {
        public enum RelatedNodeTypes
        {
            Complete = 0,
            Aggregated = 1
        }

        private string m_Link;
        private string m_Title;

        private Microsoft.Samples.FeedSync.SharingNode m_SharingNode;

        private Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes m_RelatedNodeType;

        static public Microsoft.Samples.FeedSync.RelatedNode CreateNew(Microsoft.Samples.FeedSync.SharingNode i_SharingNode, Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes i_RelatedNodeType, string i_Link, string i_Title)
        {
            Microsoft.Samples.FeedSync.Feed Feed = i_SharingNode.Feed;

            string ElementName = System.String.Format
                (
                "{0}:{1}",
                Feed.FeedSyncNamespacePrefix,
                Microsoft.Samples.FeedSync.Constants.RELATED_ELEMENT_NAME
                );

            System.Xml.XmlElement RelatedNodeXmlElement = Feed.XmlDocument.CreateElement
                (
                ElementName,
                Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI
                );

            string RelatedNodeType = System.String.Empty;
            switch (i_RelatedNodeType)
            {
                case Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes.Aggregated:
                    {
                        RelatedNodeType = Microsoft.Samples.FeedSync.Constants.RELATED_TYPE_AGGREGATED;
                        break;
                    }

                case Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes.Complete:
                    {
                        RelatedNodeType = Microsoft.Samples.FeedSync.Constants.RELATED_TYPE_COMPLETE;
                        break;
                    }

                default:
                    {
                        throw new System.ArgumentException("Unknown related type!");
                    }
            }

            RelatedNodeXmlElement.SetAttribute
                (
                Microsoft.Samples.FeedSync.Constants.TYPE_ATTRIBUTE,
                RelatedNodeType
                );

            RelatedNodeXmlElement.SetAttribute
                (
                Microsoft.Samples.FeedSync.Constants.LINK_ATTRIBUTE,
                i_Link
                );

            if (!System.String.IsNullOrEmpty(i_Title))
            {
                RelatedNodeXmlElement.SetAttribute
                    (
                    Microsoft.Samples.FeedSync.Constants.TITLE_ATTRIBUTE,
                    i_Title
                    );
            }

            Microsoft.Samples.FeedSync.RelatedNode RelatedNode = new Microsoft.Samples.FeedSync.RelatedNode
                (
                null,
                RelatedNodeXmlElement
                );

            return RelatedNode;
        }


        static public Microsoft.Samples.FeedSync.RelatedNode CreateFromXmlElement(Microsoft.Samples.FeedSync.SharingNode i_SharingNode, System.Xml.XmlElement i_RelatedNodeXmlElement)
        {
            if (i_RelatedNodeXmlElement.OwnerDocument != i_SharingNode.Feed.XmlDocument)
                i_RelatedNodeXmlElement = (System.Xml.XmlElement)i_SharingNode.Feed.XmlDocument.ImportNode(i_RelatedNodeXmlElement, true);

            Microsoft.Samples.FeedSync.RelatedNode RelatedNode = new Microsoft.Samples.FeedSync.RelatedNode
                (
                i_SharingNode,
                i_RelatedNodeXmlElement
                );

            return RelatedNode;
        }

        private RelatedNode(Microsoft.Samples.FeedSync.SharingNode i_SharingNode, System.Xml.XmlElement i_RelatedNodeXmlElement)
        {
            bool InvalidXmlElement =
                (i_RelatedNodeXmlElement.LocalName != Microsoft.Samples.FeedSync.Constants.RELATED_ELEMENT_NAME) ||
                (i_RelatedNodeXmlElement.NamespaceURI != Microsoft.Samples.FeedSync.Constants.FEEDSYNC_XML_NAMESPACE_URI);

            if (InvalidXmlElement)
                throw new System.Exception("Invalid xml element!");

            m_SharingNode = i_SharingNode;
            m_XmlElement = i_RelatedNodeXmlElement;

            string RelatedNodeType = i_RelatedNodeXmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.TYPE_ATTRIBUTE);
            switch (RelatedNodeType)
            {
                case Microsoft.Samples.FeedSync.Constants.RELATED_TYPE_AGGREGATED:
                {
                    m_RelatedNodeType = Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes.Aggregated;
                    break;
                }

                case Microsoft.Samples.FeedSync.Constants.RELATED_TYPE_COMPLETE:
                {
                    m_RelatedNodeType = Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes.Complete;
                    break;
                }

                default:
                {
                    throw new System.ArgumentException("Unknown related node type: " + RelatedNodeType);
                }
            }
            
            m_Link = i_RelatedNodeXmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.LINK_ATTRIBUTE);

            if (i_RelatedNodeXmlElement.HasAttribute(Microsoft.Samples.FeedSync.Constants.TITLE_ATTRIBUTE))
                m_Title = i_RelatedNodeXmlElement.GetAttribute(Microsoft.Samples.FeedSync.Constants.TITLE_ATTRIBUTE);
        }

        public Microsoft.Samples.FeedSync.SharingNode SharingNode
        {
            get
            {
                return m_SharingNode;
            }
        }

        public Microsoft.Samples.FeedSync.RelatedNode.RelatedNodeTypes RelatedNodeType
        {
            get
            {
                return m_RelatedNodeType;
            }
        }

        public string Link
        {
            get
            {
                return m_Link;
            }
        }

        public string Title
        {
            get
            {
                return m_Title;
            }
        }
    }
}
