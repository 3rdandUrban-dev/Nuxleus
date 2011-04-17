/*****************************************************************************************
   
   Copyright (c) Microsoft Corporation. All rights reserved.

   Use of this code sample is subject to the terms of the Microsoft
   Permissive License, a copy of which should always be distributed with
   this file.  You can also access a copy of this license agreement at:
   http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx

 ****************************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FeedSyncNotesSample
{
    public partial class ResolveConflictsForm : Form
    {
        private Microsoft.Samples.FeedSync.FeedItemNode m_ConflictItemNode = null;
        private int m_CurrentConflictIndex = 0;

        public ResolveConflictsForm()
        {
            InitializeComponent();
        }

        // Set the conflict item node that this form will display
        // This property must be set before running the form
        public Microsoft.Samples.FeedSync.FeedItemNode ConflictItem
        {
            set
            {
                m_ConflictItemNode = value;
            }
        }

        public Microsoft.Samples.FeedSync.FeedItemNode ResolvedItem
        {
            get
            {
                // Return either the original node, or any of the conflicts.
                // The item and index 0 will represent the original, and above that as an 
                // index into the array of conflict nodes
                if (m_CurrentConflictIndex == 0)
                {
                    return m_ConflictItemNode;
                }
                else
                {
                    return m_ConflictItemNode.SyncNode.ConflictFeedItemNodes[m_CurrentConflictIndex - 1];
                }
            }
        }

        void DisplayCurrentConflictItem()
        {
            Microsoft.Samples.FeedSync.FeedItemNode node = ResolvedItem;

            label1.Text = node.Description;

            int total = m_ConflictItemNode.SyncNode.ConflictFeedItemNodes.Length;

            // enable/disable buttons appropriately
            toolStripButtonPrevConflict.Enabled = m_CurrentConflictIndex > 0;
            toolStripButtonNextConflict.Enabled = m_CurrentConflictIndex < total;

            // Update current and total number of conflicts
            toolStripLabel1.Text = string.Format("{0}/{1}", m_CurrentConflictIndex + 1, total+1);
        }

        private void ResolveConflictsForm_Load(object sender, EventArgs e)
        {
            DisplayCurrentConflictItem();
        }

        private void toolStripButtonPrevConflict_Click(object sender, EventArgs e)
        {
            if (m_CurrentConflictIndex > 0)
            {
                --m_CurrentConflictIndex;
                DisplayCurrentConflictItem();
            }
        }

        private void toolStripButtonNextConflict_Click(object sender, EventArgs e)
        {
            if (m_CurrentConflictIndex < m_ConflictItemNode.SyncNode.ConflictFeedItemNodes.Length)
            {
                ++m_CurrentConflictIndex;
                DisplayCurrentConflictItem();
            }
        }
    }
}