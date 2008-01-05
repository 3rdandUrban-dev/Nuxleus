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
    public class FeedItemNode : Microsoft.Samples.FeedSync.Node
    {
        private string m_Title;
        private string m_Description;
        private string m_ID;

        private Microsoft.Samples.FeedSync.Feed m_Feed;
        private Microsoft.Samples.FeedSync.SyncNode m_SyncNode;

        public enum CompareResult
        {
            ItemNode1Newer = 1,
            ItemNode2Newer = 2,
            ItemNodesEqual = 3,
            ItemNodesEqual_DifferentConflictData = 4
        }

        #region CreateFromXmlElement methods

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateFromXmlElement(Microsoft.Samples.FeedSync.Feed i_Feed, System.Xml.XmlElement i_ItemXmlElement)
        {
            if (i_ItemXmlElement.OwnerDocument != i_Feed.XmlDocument)
                i_ItemXmlElement = (System.Xml.XmlElement)i_Feed.XmlDocument.ImportNode(i_ItemXmlElement, true);

            Microsoft.Samples.FeedSync.FeedItemNode FeedItemNode = new Microsoft.Samples.FeedSync.FeedItemNode
                (
                i_Feed,
                i_ItemXmlElement
                );

            return FeedItemNode;
        }

        #endregion

        #region CreateNewFromXmlElement methods

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNewFromXmlElement(Microsoft.Samples.FeedSync.Feed i_Feed, System.Xml.XmlElement i_ItemXmlElement, string i_SyncNodeID)
        {
            //  First create an element that works in this document
            System.Xml.XmlElement ImportedItemXmlElement = (System.Xml.XmlElement)i_Feed.XmlDocument.ImportNode(i_ItemXmlElement, true);

            Microsoft.Samples.FeedSync.FeedItemNode FeedItemNode = new Microsoft.Samples.FeedSync.FeedItemNode
                (
                i_Feed,
                ImportedItemXmlElement,
                i_SyncNodeID
                );

            return FeedItemNode;
        }

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNewFromXmlElement(Microsoft.Samples.FeedSync.Feed i_Feed, System.Xml.XmlElement i_ItemXmlElement, string i_SyncNodeID, int i_Sequence, System.DateTime? i_WhenDateTime, string i_By, bool i_Deleted, bool i_NoConflicts, int i_Updates)
        {
            //  First create an element that works in this document
            System.Xml.XmlElement ImportedItemXmlElement = (System.Xml.XmlElement)i_Feed.XmlDocument.ImportNode(i_ItemXmlElement, true);

            Microsoft.Samples.FeedSync.FeedItemNode FeedItemNode = new Microsoft.Samples.FeedSync.FeedItemNode
                (
                i_Feed,
                ImportedItemXmlElement,
                i_SyncNodeID,
                i_Sequence,
                i_WhenDateTime,
                i_By,
                i_Deleted, 
                i_NoConflicts,
                i_Updates
                );

            return FeedItemNode;
        }

        #endregion

        #region CreateNew methods

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Title, string i_Description, string i_SyncNodeID, string i_By)
        {
            return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                (
                i_Feed,
                i_Title,
                i_Description,
                null,
                i_SyncNodeID,
                1,
                System.DateTime.Now,
                i_By,
                false,
                false,
                0
                );
        }

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Title, string i_SyncNodeID, int i_Sequence, string i_When, string i_By)
        {
            return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                (
                i_Feed,
                i_Title,
                null,
                null,
                i_SyncNodeID, 
                i_Sequence,
                System.Convert.ToDateTime(i_When), 
                i_By,
                false,
                false,
                0
                );
        }

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Title, string i_SyncNodeID, int i_Sequence, string i_When, string i_By, bool i_Deleted, bool i_NoConflicts)
        {
            return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                (
                i_Feed,
                i_Title,
                null,
                null,
                i_SyncNodeID,
                i_Sequence,
                System.Convert.ToDateTime(i_When),
                i_By,
                i_Deleted,
                i_NoConflicts,
                0
                );
        }

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Title, string i_SyncNodeID, int i_Sequence, System.DateTime? i_WhenDateTime, string i_By)
        {
            return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                (
                i_Feed,
                i_Title,
                null,
                null,
                i_SyncNodeID,
                i_Sequence,
                i_WhenDateTime,
                i_By,
                false,
                false,
                0
                );
        }

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Title, string i_SyncNodeID, int i_Sequence, System.DateTime? i_WhenDateTime, string i_By, bool i_Deleted, bool i_NoConflicts)
        {
            return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                (
                i_Feed,
                i_Title,
                null,
                null,
                i_SyncNodeID,
                i_Sequence,
                i_WhenDateTime,
                i_By,
                i_Deleted,
                i_NoConflicts,
                0
                );
        }

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Title, string i_ID, string i_SyncNodeID, int i_Sequence, string i_When, string i_By)
        {
            return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                (
                i_Feed,
                i_Title,
                null,
                i_ID,
                i_SyncNodeID,
                i_Sequence,
                System.Convert.ToDateTime(i_When),
                i_By,
                false,
                false,
                0
                );
        }

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Title, string i_ID, string i_SyncNodeID, int i_Sequence, string i_When, string i_By, bool i_Deleted, bool i_NoConflicts)
        {
            return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                (
                i_Feed,
                i_Title,
                null,
                i_ID,
                i_SyncNodeID,
                i_Sequence,
                System.Convert.ToDateTime(i_When),
                i_By,
                i_Deleted,
                i_NoConflicts,
                0
                );
        }

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Title, string i_ID, string i_SyncNodeID, int i_Sequence, System.DateTime? i_WhenDateTime, string i_By)
        {
            return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                (
                i_Feed,
                i_Title,
                null,
                i_ID,
                i_SyncNodeID,
                i_Sequence,
                i_WhenDateTime,
                i_By,
                false,
                false,
                0
                );
        }

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Title, string i_ID, string i_SyncNodeID, int i_Sequence, System.DateTime? i_WhenDateTime, string i_By, bool i_Deleted, bool i_NoConflicts)
        {
            return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                (
                i_Feed,
                i_Title,
                null,
                i_ID,
                i_SyncNodeID,
                i_Sequence,
                i_WhenDateTime,
                i_By,
                i_Deleted,
                i_NoConflicts,
                0
                );
        }

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Title, string i_Description, string i_ID, string i_SyncNodeID, int i_Sequence, string i_When, string i_By)
        {
            return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                (
                i_Feed,
                i_Title,
                i_Description,
                i_ID,
                i_SyncNodeID,
                i_Sequence,
                System.Convert.ToDateTime(i_When),
                i_By,
                false,
                false, 
                0
                );
        }

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Title, string i_Description, string i_ID, string i_SyncNodeID, int i_Sequence, string i_When, string i_By, bool i_Deleted, bool i_NoConflicts)
        {
            return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                (
                i_Feed,
                i_Title,
                i_Description,
                i_ID,
                i_SyncNodeID,
                i_Sequence,
                System.Convert.ToDateTime(i_When),
                i_By,
                i_Deleted,
                i_NoConflicts,
                0
                );
        }

        static public Microsoft.Samples.FeedSync.FeedItemNode CreateNew(Microsoft.Samples.FeedSync.Feed i_Feed, string i_Title, string i_Description, string i_ID, string i_SyncNodeID, int i_Sequence, System.DateTime? i_WhenDateTime, string i_By, bool i_Deleted, bool i_NoConflicts, int i_Updates)
        {
            System.Xml.XmlElement FeedItemXmlElement;

            if (i_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
            {
                FeedItemXmlElement = i_Feed.XmlDocument.CreateElement
                    (
                    i_Feed.FeedItemElementName,
                    Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI
                    );
            }
            else
                FeedItemXmlElement = i_Feed.XmlDocument.CreateElement(i_Feed.FeedItemElementName);

            #region Create feed item title element

            System.Xml.XmlElement TitleXmlElement = null;

            if (i_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
            {
                string ElementName;

                if (i_Feed.XmlNamespaceManager.DefaultNamespace == Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
                    ElementName = Microsoft.Samples.FeedSync.Constants.ATOM_TITLE_ELEMENT_NAME;
                else
                {
                    ElementName = System.String.Format
                        (
                        "{0}:{1}",
                        i_Feed.AtomNamespacePrefix,
                        Microsoft.Samples.FeedSync.Constants.ATOM_TITLE_ELEMENT_NAME
                        );
                }
                    
                TitleXmlElement = i_Feed.XmlDocument.CreateElement
                    (
                    ElementName,
                    Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI
                    );
            }
            else
                TitleXmlElement = i_Feed.XmlDocument.CreateElement(Microsoft.Samples.FeedSync.Constants.RSS_TITLE_ELEMENT_NAME);

            FeedItemXmlElement.AppendChild(TitleXmlElement);
            TitleXmlElement.InnerText = i_Title;

            #endregion

            #region Create feed item description element

            System.Xml.XmlElement DescriptionXmlElement = null;

            if (i_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
            {
                string ElementName;

                if (i_Feed.XmlNamespaceManager.DefaultNamespace == Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
                    ElementName = Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_DESCRIPTION_ELEMENT_NAME;
                else
                {
                    ElementName = System.String.Format
                        (
                        "{0}:{1}",
                        i_Feed.AtomNamespacePrefix,
                        Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_DESCRIPTION_ELEMENT_NAME
                        );
                }

                DescriptionXmlElement = i_Feed.XmlDocument.CreateElement
                    (
                    ElementName,
                    Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI
                    );
            }
            else
                DescriptionXmlElement = i_Feed.XmlDocument.CreateElement(Microsoft.Samples.FeedSync.Constants.RSS_DESCRIPTION_ELEMENT_NAME);

            FeedItemXmlElement.AppendChild(DescriptionXmlElement);
            DescriptionXmlElement.InnerText = i_Description;

            #endregion

            #region Create feed item id element

            System.Xml.XmlElement IDXmlElement = null;

            if (!System.String.IsNullOrEmpty(i_ID) || (i_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom))
            {
                if (i_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
                {
                    if (System.String.IsNullOrEmpty(i_ID))
                        i_ID = "urn:uuid:" + System.Guid.NewGuid().ToString();

                    string ElementName;

                    if (i_Feed.XmlNamespaceManager.DefaultNamespace == Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
                        ElementName = Microsoft.Samples.FeedSync.Constants.ATOM_ID_ELEMENT_NAME;
                    else
                    {
                        ElementName = System.String.Format
                            (
                            "{0}:{1}",
                            i_Feed.AtomNamespacePrefix,
                            Microsoft.Samples.FeedSync.Constants.ATOM_ID_ELEMENT_NAME
                            );
                    }

                    IDXmlElement = i_Feed.XmlDocument.CreateElement
                        (
                        ElementName,
                        Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI
                        );
                }
                else
                    IDXmlElement = i_Feed.XmlDocument.CreateElement(Microsoft.Samples.FeedSync.Constants.RSS_ID_ELEMENT_NAME);
            }

            if (IDXmlElement != null)
            {
                FeedItemXmlElement.AppendChild(IDXmlElement);
                IDXmlElement.InnerText = i_ID;
            }

            #endregion

            Microsoft.Samples.FeedSync.FeedItemNode FeedItemNode = Microsoft.Samples.FeedSync.FeedItemNode.CreateNewFromXmlElement
                (
                i_Feed, 
                FeedItemXmlElement,
                i_SyncNodeID,
                i_Sequence,
                i_WhenDateTime, 
                i_By,
                i_Deleted,
                i_NoConflicts, 
                i_Updates
                );

            return FeedItemNode;
        }

        #endregion

        #region MergeFeedItems methods

        static public Microsoft.Samples.FeedSync.FeedItemNode MergeFeedItems(Microsoft.Samples.FeedSync.Feed i_Feed, Microsoft.Samples.FeedSync.FeedItemNode i_LocalFeedItemNode, Microsoft.Samples.FeedSync.FeedItemNode i_IncomingFeedItemNode)
        {
            Microsoft.Samples.FeedSync.FeedItemNode ImportedLocalFeedItemNode = i_Feed.ImportFeedItemNode(i_LocalFeedItemNode);
            Microsoft.Samples.FeedSync.FeedItemNode ImportedIncomingFeedItemNode = i_Feed.ImportFeedItemNode(i_IncomingFeedItemNode);

            System.Collections.Generic.List<Microsoft.Samples.FeedSync.FeedItemNode> LocalFeedItemNodeList = new System.Collections.Generic.List<Microsoft.Samples.FeedSync.FeedItemNode>();
            LocalFeedItemNodeList.Add(ImportedLocalFeedItemNode);

            System.Collections.Generic.List<Microsoft.Samples.FeedSync.FeedItemNode> IncomingFeedItemNodeList = new System.Collections.Generic.List<Microsoft.Samples.FeedSync.FeedItemNode>();
            IncomingFeedItemNodeList.Add(ImportedIncomingFeedItemNode);

            System.Diagnostics.Debug.Assert(ImportedLocalFeedItemNode.SyncNode.NoConflicts == ImportedIncomingFeedItemNode.SyncNode.NoConflicts);

            //  Perform conflict feed item processing (if necessary)
            if (!ImportedLocalFeedItemNode.SyncNode.NoConflicts)
            {
                LocalFeedItemNodeList.AddRange(ImportedLocalFeedItemNode.SyncNode.ConflictFeedItemNodes);
                IncomingFeedItemNodeList.AddRange(ImportedIncomingFeedItemNode.SyncNode.ConflictFeedItemNodes);
            }

            ImportedLocalFeedItemNode.SyncNode.RemoveAllConflictItemNodes();
            ImportedIncomingFeedItemNode.SyncNode.RemoveAllConflictItemNodes();

            Microsoft.Samples.FeedSync.FeedItemNode WinnerFeedItemNode = null;
            System.Collections.Generic.List<Microsoft.Samples.FeedSync.FeedItemNode> MergedFeedItemNodeList = new System.Collections.Generic.List<Microsoft.Samples.FeedSync.FeedItemNode>();

            #region Process local feeditem node list

            //  Iterate local items first, looking for subsumption
            foreach (Microsoft.Samples.FeedSync.FeedItemNode LocalFeedItemNode in LocalFeedItemNodeList.ToArray())
            {
                bool Subsumed = false;

                foreach (Microsoft.Samples.FeedSync.FeedItemNode IncomingFeedItemNode in IncomingFeedItemNodeList.ToArray())
                {
                    if (LocalFeedItemNode.IsSubsumedByFeedItemNode(IncomingFeedItemNode))
                    {
                        Subsumed = true;
                        break;
                    }
                }

                if (Subsumed)
                {
                    LocalFeedItemNodeList.Remove(LocalFeedItemNode);
                    continue;
                }

                MergedFeedItemNodeList.Add(LocalFeedItemNode);

                if (WinnerFeedItemNode == null)
                    WinnerFeedItemNode = LocalFeedItemNode;
                else
                {
                    Microsoft.Samples.FeedSync.FeedItemNode.CompareResult CompareResult = Microsoft.Samples.FeedSync.FeedItemNode.CompareFeedItems
                        (
                        WinnerFeedItemNode, 
                        LocalFeedItemNode
                        );

                    if (Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer != CompareResult)
                        WinnerFeedItemNode = LocalFeedItemNode;
                }
            }

            #endregion

            #region Process incoming feeditem node list

            //  Iterate incoming items next, looking for subsumption
            foreach (Microsoft.Samples.FeedSync.FeedItemNode IncomingFeedItemNode in IncomingFeedItemNodeList.ToArray())
            {
                bool Subsumed = false;

                foreach (Microsoft.Samples.FeedSync.FeedItemNode LocalFeedItemNode in LocalFeedItemNodeList.ToArray())
                {
                    if (IncomingFeedItemNode.IsSubsumedByFeedItemNode(LocalFeedItemNode))
                    {
                        Subsumed = true;
                        break;
                    }
                }

                if (Subsumed)
                {
                    IncomingFeedItemNodeList.Remove(IncomingFeedItemNode);
                    continue;
                }

                MergedFeedItemNodeList.Add(IncomingFeedItemNode);

                if (WinnerFeedItemNode == null)
                    WinnerFeedItemNode = IncomingFeedItemNode;
                else
                {
                    Microsoft.Samples.FeedSync.FeedItemNode.CompareResult CompareResult = Microsoft.Samples.FeedSync.FeedItemNode.CompareFeedItems
                        (
                        WinnerFeedItemNode,
                        IncomingFeedItemNode
                        );

                    if (Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer != CompareResult)
                        WinnerFeedItemNode = IncomingFeedItemNode;
                }
            }

            #endregion

            foreach (Microsoft.Samples.FeedSync.FeedItemNode ConflictFeedItemNode in MergedFeedItemNodeList)
            {
                if (WinnerFeedItemNode != ConflictFeedItemNode)
                    WinnerFeedItemNode.SyncNode.AddConflictItemNode(ConflictFeedItemNode);
            }

            return WinnerFeedItemNode;
        }

        #endregion

        static public Microsoft.Samples.FeedSync.FeedItemNode.CompareResult CompareFeedItems(Microsoft.Samples.FeedSync.FeedItemNode i_FeedItemNode1, Microsoft.Samples.FeedSync.FeedItemNode i_FeedItemNode2)
        {
            Microsoft.Samples.FeedSync.SyncNode SyncNode1 = i_FeedItemNode1.SyncNode;
            Microsoft.Samples.FeedSync.SyncNode SyncNode2 = i_FeedItemNode2.SyncNode;

            //  If SyncNode1 has higher version, it is newer
            if (SyncNode1.Updates> SyncNode2.Updates)
                return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer;

            //  If SyncNode2 has higher version, it is newer
            if (SyncNode2.Updates> SyncNode1.Updates)
                return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode2Newer;

            Microsoft.Samples.FeedSync.HistoryNode FeedSyncHistoryNode1 = SyncNode1.TopMostHistoryNode;
            Microsoft.Samples.FeedSync.HistoryNode FeedSyncHistoryNode2 = SyncNode2.TopMostHistoryNode;

            //  Compare by "when" attribute
            if ((FeedSyncHistoryNode1.WhenDateTime != null) && (FeedSyncHistoryNode2.WhenDateTime == null))
                    return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer;

                if ((FeedSyncHistoryNode1.WhenDateTime != null) && (FeedSyncHistoryNode2.WhenDateTime != null))
            {
                if (FeedSyncHistoryNode1.WhenDateTime > FeedSyncHistoryNode2.WhenDateTime)
                    return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer;

                if (FeedSyncHistoryNode1.WhenDateTime == FeedSyncHistoryNode2.WhenDateTime)
                {
                    if (!System.String.IsNullOrEmpty(FeedSyncHistoryNode1.By) && System.String.IsNullOrEmpty(FeedSyncHistoryNode2.By))
                        return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer;

                    if (!System.String.IsNullOrEmpty(FeedSyncHistoryNode1.By) && !System.String.IsNullOrEmpty(FeedSyncHistoryNode2.By))
                    {
                        if (FeedSyncHistoryNode1.By.CompareTo(FeedSyncHistoryNode2.By) > 0)
                            return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNode1Newer;
                    }
                }
            }

            Microsoft.Samples.FeedSync.FeedItemNode[] ConflictFeedItemNodes1 = i_FeedItemNode1.SyncNode.ConflictFeedItemNodes;
            Microsoft.Samples.FeedSync.FeedItemNode[] ConflictFeedItemNodes2 = i_FeedItemNode2.SyncNode.ConflictFeedItemNodes;

            //  Compare conflict items
            if (ConflictFeedItemNodes1.Length != ConflictFeedItemNodes2.Length)
                return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNodesEqual_DifferentConflictData;

            if (ConflictFeedItemNodes1.Length > 0)
            {
                foreach (Microsoft.Samples.FeedSync.FeedItemNode ConflictFeedItemNode1 in ConflictFeedItemNodes1)
                {
                    bool MatchingConflictItem = false;

                    foreach (Microsoft.Samples.FeedSync.FeedItemNode ConflictFeedItemNode2 in ConflictFeedItemNodes2)
                    {
                        Microsoft.Samples.FeedSync.FeedItemNode.CompareResult CompareResult = Microsoft.Samples.FeedSync.FeedItemNode.CompareFeedItems
                            (
                            ConflictFeedItemNode1,
                            ConflictFeedItemNode2
                            );

                        if (Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNodesEqual == CompareResult)
                        {
                            MatchingConflictItem = true;
                            break;
                        }
                    }

                    if (!MatchingConflictItem)
                        return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNodesEqual_DifferentConflictData;
                }
            }

            return Microsoft.Samples.FeedSync.FeedItemNode.CompareResult.ItemNodesEqual;
        }

        #region constructors

        private FeedItemNode(Microsoft.Samples.FeedSync.Feed i_Feed, System.Xml.XmlElement i_FeedItemXmlElement)
        {
            m_Feed = i_Feed;

            this.GetFeedItemDataFromXmlElement(i_FeedItemXmlElement);

            m_XmlElement = i_FeedItemXmlElement;

            #region Get sx:sync element

            string ElementName = System.String.Format
                (
                "{0}:{1}",
                i_Feed.FeedSyncNamespacePrefix,
                Microsoft.Samples.FeedSync.Constants.SYNC_ELEMENT_NAME
                );

            System.Xml.XmlElement SyncNodeXmlElement = (System.Xml.XmlElement)m_XmlElement.SelectSingleNode
                (
                ElementName, 
                i_Feed.XmlNamespaceManager
                );

            if (SyncNodeXmlElement == null)
                throw new System.ArgumentException("Item is missing 'sx:sync' element!");

            m_SyncNode = Microsoft.Samples.FeedSync.SyncNode.CreateFromXmlElement(this, SyncNodeXmlElement);

            #endregion
        }

        private FeedItemNode(Microsoft.Samples.FeedSync.Feed i_Feed, System.Xml.XmlElement i_FeedItemXmlElement, string i_SyncNodeID)
        {
            m_Feed = i_Feed;

            this.GetFeedItemDataFromXmlElement(i_FeedItemXmlElement);

            m_XmlElement = i_FeedItemXmlElement;

            m_SyncNode = Microsoft.Samples.FeedSync.SyncNode.CreateNew
                (
                m_Feed,
                this,
                i_SyncNodeID
                );

            if (m_XmlElement.ChildNodes.Count > 0)
                m_XmlElement.InsertBefore(m_SyncNode.XmlElement, m_XmlElement.ChildNodes[0]);
            else
                m_XmlElement.AppendChild(m_SyncNode.XmlElement);
        }

        private FeedItemNode(Microsoft.Samples.FeedSync.Feed i_Feed, System.Xml.XmlElement i_FeedItemXmlElement, string i_SyncNodeID, int i_Sequence, System.DateTime? i_WhenDateTime, string i_By, bool i_Deleted, bool i_NoConflicts, int i_Updates)
        {
            m_Feed = i_Feed;

            this.GetFeedItemDataFromXmlElement(i_FeedItemXmlElement);

            m_XmlElement = i_FeedItemXmlElement;

            m_SyncNode = Microsoft.Samples.FeedSync.SyncNode.CreateNew
                (
                m_Feed, 
                this,
                i_SyncNodeID,
                i_Sequence,
                i_WhenDateTime,
                i_By,
                i_Deleted,
                i_NoConflicts,
                i_Updates
                );

            if (m_XmlElement.ChildNodes.Count > 0)
                m_XmlElement.InsertBefore(m_SyncNode.XmlElement, m_XmlElement.ChildNodes[0]);
            else
                m_XmlElement.AppendChild(m_SyncNode.XmlElement);
            if (m_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
                this.AddUpdatedElement();
        }

        #endregion

        private void GetFeedItemDataFromXmlElement(System.Xml.XmlElement i_XmlElement)
        {
            string ElementName;

            #region Validate feed item element

            if (m_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
                ElementName = Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_ELEMENT_NAME;
            else
                ElementName = Microsoft.Samples.FeedSync.Constants.RSS_FEED_ITEM_ELEMENT_NAME;

            if (i_XmlElement.LocalName != ElementName)
                throw new System.ArgumentException("Invalid feed item xml element!");

            #endregion

            #region Get feed item title element

            if (m_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
            {
                ElementName = System.String.Format
                    (
                    "{0}:{1}",
                    m_Feed.AtomNamespacePrefix,
                    Microsoft.Samples.FeedSync.Constants.ATOM_TITLE_ELEMENT_NAME
                    );
            }
            else
                ElementName = Microsoft.Samples.FeedSync.Constants.RSS_TITLE_ELEMENT_NAME;

            System.Xml.XmlNode TitleXmlNode = i_XmlElement.SelectSingleNode
                (
                ElementName,
                m_Feed.XmlNamespaceManager
                );

            if (TitleXmlNode != null)
                m_Title = TitleXmlNode.InnerText;

            #endregion

            #region Get feed item description element

            if (m_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
            {
                ElementName = System.String.Format
                    (
                    "{0}:{1}",
                    m_Feed.AtomNamespacePrefix,
                    Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_DESCRIPTION_ELEMENT_NAME
                    );
            }
            else
                ElementName = Microsoft.Samples.FeedSync.Constants.RSS_DESCRIPTION_ELEMENT_NAME;

            System.Xml.XmlNode DescriptionXmlNode = i_XmlElement.SelectSingleNode
                (
                ElementName,
                m_Feed.XmlNamespaceManager
                );

            if (DescriptionXmlNode != null)
                m_Description = DescriptionXmlNode.InnerText;

            #endregion

            #region Get feed item id element

            if (m_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
            {
                ElementName = System.String.Format
                    (
                    "{0}:{1}",
                    m_Feed.AtomNamespacePrefix,
                    Microsoft.Samples.FeedSync.Constants.ATOM_ID_ELEMENT_NAME
                    );
            }
            else
                ElementName = Microsoft.Samples.FeedSync.Constants.RSS_ID_ELEMENT_NAME;

            System.Xml.XmlNode IDXmlNode = i_XmlElement.SelectSingleNode
                (
                ElementName,
                m_Feed.XmlNamespaceManager
                );

            if (IDXmlNode != null)
                m_ID = IDXmlNode.InnerText;

            #endregion
        }

        public bool Delete(System.DateTime i_WhenDateTime, string i_By)
        {
            Microsoft.Samples.FeedSync.HistoryNode HistoryNode = Microsoft.Samples.FeedSync.HistoryNode.CreateNew
                (
                m_SyncNode,
                i_WhenDateTime,
                i_By
                );

            m_SyncNode.AddHistoryNode(HistoryNode);
            m_SyncNode.Deleted = true;

            if (m_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
                this.AddUpdatedElement();

            //  Indicate successful deletion
            return true;
        }

        public bool Update(string i_Title, string i_Description, System.DateTime i_WhenDateTime, string i_By, bool i_ResolveConflicts)
        {
            Microsoft.Samples.FeedSync.HistoryNode HistoryNode = Microsoft.Samples.FeedSync.HistoryNode.CreateNew
                (
                m_SyncNode,
                i_WhenDateTime,
                i_By
                );

            m_SyncNode.AddHistoryNode(HistoryNode);

            this.Title = i_Title;
            this.Description = i_Description;

            if (m_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
                this.AddUpdatedElement();

            bool IgnoreConflictProcessing = 
                m_SyncNode.NoConflicts || 
                !i_ResolveConflicts ||
                (m_SyncNode.ConflictFeedItemNodes.Length == 0);

            if (IgnoreConflictProcessing)
                return true;

            //  *********************************************************************************
            //  BIG HONKING NOTE:  This code resolves all conflicts and does not accomodate for
            //                     selective conflict resolution
            //  *********************************************************************************

            foreach (Microsoft.Samples.FeedSync.FeedItemNode ConflictFeedItemNode in m_SyncNode.ConflictFeedItemNodes)
            {
                foreach (Microsoft.Samples.FeedSync.HistoryNode ConflictHistoryNode in ConflictFeedItemNode.SyncNode.HistoryNodes)
                {
                    bool Subsumed = false;

                    foreach (Microsoft.Samples.FeedSync.HistoryNode MainHistoryNode in m_SyncNode.HistoryNodes)
                    {
                        if (ConflictHistoryNode.IsSubsumedByHistoryNode(MainHistoryNode))
                        {
                            Subsumed = true;
                            break;
                        }
                    }

                    if (!Subsumed)
                        m_SyncNode.AddConflictHistoryNode(ConflictHistoryNode);
                }
            }

            m_SyncNode.RemoveAllConflictItemNodes();

            //  Indicate successful update
            return true;
        }

        public string Title
        {
            get
            {
                return m_Title;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentException("Must assign non-null value for title");

                string ElementName;

                if (m_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
                {
                    ElementName = System.String.Format
                        (
                        "{0}:{1}",
                        m_Feed.AtomNamespacePrefix,
                        Microsoft.Samples.FeedSync.Constants.ATOM_TITLE_ELEMENT_NAME
                        );
                }
                else
                    ElementName = Microsoft.Samples.FeedSync.Constants.RSS_TITLE_ELEMENT_NAME;

                //  Get reference to title xml node
                System.Xml.XmlNode TitleXmlNode = m_XmlElement.SelectSingleNode
                    (
                    ElementName,
                    m_Feed.XmlNamespaceManager
                    );

                if (TitleXmlNode == null)
                {
                    if (m_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
                    {
                        TitleXmlNode = m_XmlElement.OwnerDocument.CreateElement
                            (
                            ElementName,
                            Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI
                            );
                    }
                    else
                        TitleXmlNode = m_XmlElement.OwnerDocument.CreateElement(ElementName);

                    m_XmlElement.AppendChild(TitleXmlNode);
                }

                TitleXmlNode.InnerText = value;
                m_Title = value;
            }
        }

        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentException("Must assign non-null value for description");

                string ElementName;

                if (m_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
                {
                    ElementName = System.String.Format
                        (
                        "{0}:{1}",
                        m_Feed.AtomNamespacePrefix,
                        Microsoft.Samples.FeedSync.Constants.ATOM_FEED_ITEM_DESCRIPTION_ELEMENT_NAME
                        );
                }
                else
                    ElementName = Microsoft.Samples.FeedSync.Constants.RSS_DESCRIPTION_ELEMENT_NAME;

                //  Get reference to description xml node
                System.Xml.XmlNode DescriptionXmlNode = m_XmlElement.SelectSingleNode
                    (
                    ElementName,
                    m_Feed.XmlNamespaceManager
                    );

                if (DescriptionXmlNode == null)
                {
                    if (m_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
                    {
                        DescriptionXmlNode = m_XmlElement.OwnerDocument.CreateElement
                            (
                            ElementName,
                            Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI
                            );
                    }
                    else
                        DescriptionXmlNode = m_XmlElement.OwnerDocument.CreateElement(ElementName);

                    m_XmlElement.AppendChild(DescriptionXmlNode);
                }


                DescriptionXmlNode.InnerText = value;
                m_Description = value;
            }
        }

        public string ID
        {
            get
            {
                return m_ID;
            }
            set
            {
                string ElementName;

                if (m_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
                {
                    ElementName = System.String.Format
                        (
                        "{0}:{1}",
                        m_Feed.AtomNamespacePrefix,
                        Microsoft.Samples.FeedSync.Constants.ATOM_ID_ELEMENT_NAME
                        );
                }
                else
                    ElementName = Microsoft.Samples.FeedSync.Constants.RSS_ID_ELEMENT_NAME;

                //  Get reference to id xml node
                System.Xml.XmlNode IDXmlNode = m_XmlElement.SelectSingleNode
                    (
                    ElementName,
                    m_Feed.XmlNamespaceManager
                    );

                if (IDXmlNode == null)
                {
                    //  Don't need to do anything if setting null
                    if (System.String.IsNullOrEmpty(value))
                        return;

                    if (m_Feed.FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom)
                    {
                        IDXmlNode = m_Feed.XmlDocument.CreateElement
                            (
                            ElementName,
                            Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI
                            );
                    }
                    else
                        IDXmlNode = m_Feed.XmlDocument.CreateElement(ElementName);

                    m_XmlElement.AppendChild(IDXmlNode);
                }
                else
                {
                    if (System.String.IsNullOrEmpty(value))
                    {
                        IDXmlNode.ParentNode.RemoveChild(IDXmlNode);
                        m_ID = null;
                        return;
                    }
                }

                IDXmlNode.InnerText = value;
                m_ID = value;
            }
        }

        public Microsoft.Samples.FeedSync.SyncNode SyncNode
        {
            get
            {
                return m_SyncNode;
            }
        }

        public Microsoft.Samples.FeedSync.Feed Feed
        {
            get
            {
                return m_Feed;
            }
        }

        public bool IsSubsumedByFeedItemNode(Microsoft.Samples.FeedSync.FeedItemNode i_FeedItemNode)
        {
            foreach (Microsoft.Samples.FeedSync.HistoryNode HistoryNode in i_FeedItemNode.SyncNode.HistoryNodes)
            {
                if (m_SyncNode.TopMostHistoryNode.IsSubsumedByHistoryNode(HistoryNode))
                    return true;
            }

            return false;
        }

        public Microsoft.Samples.FeedSync.FeedItemNode Clone()
        {
            return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                (
                m_Feed,
                m_Title,
                m_Description,
                m_ID,
                m_SyncNode.ID,
                m_SyncNode.TopMostHistoryNode.Sequence,
                m_SyncNode.TopMostHistoryNode.WhenDateTime,
                m_SyncNode.TopMostHistoryNode.By,
                m_SyncNode.Deleted,
                m_SyncNode.NoConflicts,
                m_SyncNode.Updates
                );
        }

        public Microsoft.Samples.FeedSync.FeedItemNode Copy(string i_By)
        {

            return Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                (
                m_Feed,
                m_Title,
                m_Description,
                m_ID,
                System.Guid.NewGuid().ToString(),
                1,
                System.DateTime.Now,
                i_By,
                m_SyncNode.Deleted,
                m_SyncNode.NoConflicts,
                1
                );
        }

        private void AddUpdatedElement()
        {
            string ElementName = System.String.Format
                (
                "{0}:{1}",
                m_Feed.AtomNamespacePrefix,
                Microsoft.Samples.FeedSync.Constants.ATOM_UPDATED_ELEMENT_NAME
                );

            System.Xml.XmlNode UpdatedXmlNode = m_XmlElement.SelectSingleNode
                (
                ElementName,
                m_Feed.XmlNamespaceManager
                );

            //  Remove existing <updated> element if necessary
            if (UpdatedXmlNode != null)
                UpdatedXmlNode.ParentNode.RemoveChild(UpdatedXmlNode);

            //  Strip namespace prefix if using default namespace
            if (m_Feed.XmlNamespaceManager.DefaultNamespace == Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI)
                ElementName = Microsoft.Samples.FeedSync.Constants.ATOM_UPDATED_ELEMENT_NAME;

            UpdatedXmlNode = m_Feed.XmlDocument.CreateElement
                (
                ElementName,
                Microsoft.Samples.FeedSync.Constants.ATOM_XML_NAMESPACE_URI
                );

            if (m_SyncNode.TopMostHistoryNode.WhenDateTime != null)
                UpdatedXmlNode.InnerText = ((System.DateTime)m_SyncNode.TopMostHistoryNode.WhenDateTime).ToString(Microsoft.Samples.FeedSync.Constants.DATE_STRING_FORMAT);
            else
                UpdatedXmlNode.InnerText = System.DateTime.Now.ToString(Microsoft.Samples.FeedSync.Constants.DATE_STRING_FORMAT);

            m_XmlElement.AppendChild(UpdatedXmlNode);
        }
    }
}
