/*****************************************************************************************
   
   Copyright (c) Microsoft Corporation. All rights reserved.

   Use of this code sample is subject to the terms of the Microsoft
   Permissive License, a copy of which should always be distributed with
   this file.  You can also access a copy of this license agreement at:
   http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx

 ****************************************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FeedSyncNotesSample
{
    public partial class MainForm : Form
    {
        private const string SHARED_NOTES_LOCAL_FILE_NAME = "notes.xml";
        private const Microsoft.Samples.FeedSync.Feed.FeedTypes SHARED_NOTES_FEED_TYPE = Microsoft.Samples.FeedSync.Feed.FeedTypes.Atom;

        private string m_FeedURL = null;
        private string m_Since = null;
        private string m_Username = null;
        private bool m_Published = false;
        private int m_CurrentItemIndex = 0;
        private Microsoft.Samples.FeedSync.Feed m_Feed = null;

        class FeedItemComparer : IComparer<Microsoft.Samples.FeedSync.FeedItemNode>
        {
            public int Compare(Microsoft.Samples.FeedSync.FeedItemNode x, Microsoft.Samples.FeedSync.FeedItemNode y)
            {
                // compare first time stamps for the items:
                DateTime? xCreated = x.SyncNode.HistoryNodes[x.SyncNode.HistoryNodes.Length - 1].WhenDateTime;
                DateTime? yCreated = y.SyncNode.HistoryNodes[y.SyncNode.HistoryNodes.Length - 1].WhenDateTime;

                int res = xCreated.Value.CompareTo(yCreated.Value);

                if (res != 0)
                {
                    return res;
                }

                // both where created at exactly the same time! Use the item id to break the tie
                // ideally an application would use a custom data model to specify ordering
                string xID = x.SyncNode.ID;
                string yID = y.SyncNode.ID;

                return xID.CompareTo(yID);
            }
        }

        // A class to maintain a reasonable ordering of feed items. This is as simplistic implementation for sample
        // purposes. A more robust implementation should maintain a data model within each feed item to specify
        // logical ordering within an application.
        class SortedFeedItemList : SortedList<Microsoft.Samples.FeedSync.FeedItemNode, Microsoft.Samples.FeedSync.FeedItemNode>
        {
            public SortedFeedItemList() : base(new FeedItemComparer()) { }

            public void AddFeedItemNode(Microsoft.Samples.FeedSync.FeedItemNode feedItemNode)
            {
                Add(feedItemNode, feedItemNode);
            }
        }

        private SortedFeedItemList m_ItemNodes = new SortedFeedItemList();

        public MainForm()
        {
            InitializeComponent();

            m_FeedURL = Properties.Settings.Default.FeedURL;

            m_Username = Environment.UserName + "@" + Environment.MachineName;
        }

        private void CreateOrOpenFeed()
        {
            Microsoft.Samples.FeedSync.Feed incomingFeed = null;

            // if there is a url to use, attempt to read the feed from that url
            if (!string.IsNullOrEmpty(m_FeedURL))
            {
                try
                {
                    // ideally this would happen asynchronously while the app displays
                    // a progress bar, or loading message. Doing this here will slow down the
                    // app start up.
                    incomingFeed = ReadFeedFromURL(m_FeedURL);
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to read feed from " + m_FeedURL + " : " + ex.Message);
                }
            }

            // see if there is a local file to use
            if (System.IO.File.Exists(SHARED_NOTES_LOCAL_FILE_NAME))
            {
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.Load(SHARED_NOTES_LOCAL_FILE_NAME);

                m_Feed = Microsoft.Samples.FeedSync.Feed.Create(xmlDoc);

                LoadFeedContents();

                // there is a local file and a feed from the web so they have to be synchronized
                // now to merge any changes that occured since the app last ran
                if (incomingFeed != null)
                {
                    SyncToIncomingFeed(incomingFeed);
                }
            }
            else
            {
                // there was no local file. If there is a feed from the web use that
                // otherwise create a new feed in memory

                if (incomingFeed != null)
                {
                    m_Feed = incomingFeed;
                    LoadFeedContents();
                }
                else
                {
                    m_Feed = Microsoft.Samples.FeedSync.Feed.Create
                            (
                            "Shared Notes",
                            "Shared Notes sample feed",
                            SHARED_NOTES_LOCAL_FILE_NAME,
                            SHARED_NOTES_FEED_TYPE
                            );
                }
            }

            // display the first note
            DisplayCurrentNote();
        }

        private void SaveLocalFeed()
        {
            // serialize changes to disk:
            m_Feed.XmlDocument.Save(SHARED_NOTES_LOCAL_FILE_NAME);
        }

        private void LoadFeedContents()
        {
            m_ItemNodes.Clear();

            // load up the item nodes into the list, skipping items that are deleted:
            foreach (Microsoft.Samples.FeedSync.FeedItemNode itemNode in m_Feed.FeedItemNodes)
            {
                if (!itemNode.SyncNode.Deleted)
                {
                    m_ItemNodes.AddFeedItemNode(itemNode);
                }
            }

            this.Text = m_Feed.Title;
        }

        bool IsLocalFeedEmpty()
        {
            // Test to see if the user hasn't actually created any notes yet
            return (m_Feed.FeedItemNodes.Length == 1 && CurrentItemNode.Description.Length == 0);
        }

        private Microsoft.Samples.FeedSync.FeedItemNode CurrentItemNode
        {
            get
            {
                return m_ItemNodes.Values[m_CurrentItemIndex];
            }
        }

        private void DisplayConflictStatus()
        {
            // Display conflict button if there are any conflicts on this node
            bool haveConflicts = (CurrentItemNode.SyncNode.ConflictFeedItemNodes.Length > 0);
            toolStripButtonConflict.Visible = haveConflicts;
            toolStripStatusLabelConflict.Visible = haveConflicts;
        }

        private void DisplayCurrentNote()
        {
            // if there are no notes available to display, create one now:
            if (m_ItemNodes.Count == 0)
            {
                CreateNote();
            }
            else
            {
                // Display current item content. Assume these are plain text
                textBox1.Text = CurrentItemNode.Description;

                // Move caret to the end of the control
                // for some reason this control wants to select all the text when changed programatically
                textBox1.Select(textBox1.Text.Length, 0);

                // Enable or Disable appropriate navigation commands
                toolStripButtonPreviousNote.Enabled = (m_CurrentItemIndex != 0);
                toolStripButtonNextNote.Enabled = (m_CurrentItemIndex != m_ItemNodes.Count - 1);
                toolStripButtonDeleteNote.Enabled = m_ItemNodes.Count > 1;

                DisplayConflictStatus();

                // Update status bar with current item index (1 based index) and the total number of items
                toolStripStatusLabel1.Text = string.Format("Note {0}/{1}", m_CurrentItemIndex + 1, m_ItemNodes.Count);
            }
        }

        private void DisplayNoteByID(string syncID)
        {
            // default to first item in case the item is gone
            m_CurrentItemIndex = 0;

            Microsoft.Samples.FeedSync.FeedItemNode feedItemNode = null;

            // get the feed item with this id:
            if (m_Feed.FindFeedItemNode(syncID, out feedItemNode))
            {
                // get index in local list of nodes
                m_CurrentItemIndex = m_ItemNodes.IndexOfValue(feedItemNode);

                if (m_CurrentItemIndex == -1)
                {
                    // not found. default to first item
                    m_CurrentItemIndex = 0;
                }
            }

            DisplayCurrentNote();
        }

        private void DisplayNextNote()
        {
            // This should be the case because otherwise the toolbar button is disabled:
            System.Diagnostics.Debug.Assert(m_CurrentItemIndex < m_ItemNodes.Count - 1);

            UpdateNote();

            ++m_CurrentItemIndex;
            
            DisplayCurrentNote();
        }

        private void DisplayPreviousNote()
        {
            System.Diagnostics.Debug.Assert(m_CurrentItemIndex > 0);

            UpdateNote();
            
            --m_CurrentItemIndex;
            
            DisplayCurrentNote();
        }

        private Microsoft.Samples.FeedSync.Feed ReadFeedFromURL(string url)
        {
            // Get the feed from URL. Passing in a string for the "i_Since" parameter will generate a 304
            // response from the GET request if the feed hasn't changed since the last time it was read.
            // If this is the case, then the incoming feed will be empty and does not need to be merged
            string strFeed = Microsoft.Samples.FeedSyncService.FeedManager.ReadFeedContents(url, ref m_Since);

            if (strFeed == null)
            {
                return null;
            }

            // Create a new XML document instance for the incoming feed
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(strFeed);

            // Create a Feed object to read and manipulate feed items
            return Microsoft.Samples.FeedSync.Feed.Create(xmlDoc);
        }

        private void SyncToIncomingFeed(Microsoft.Samples.FeedSync.Feed incomingFeed)
        {
            bool refreshView = false;

            if (incomingFeed != null)
            {
                // small usability hack: if there is only one item and it is empty, this
                // will replace the current feed with the incoming feed since the user
                // hasn't done any real work yet.
                if (IsLocalFeedEmpty())
                {
                    m_Feed = incomingFeed;
                    // no need to publish anything. This effectively replaces the empty local feed with the incoming feed
                    m_Published = true;
                }
                else
                {
                    m_Feed = Microsoft.Samples.FeedSync.Feed.MergeFeeds(m_Feed, incomingFeed);
                }
                refreshView = true;
            }

            // See if the feed needs to be published (have local changes)
            if (!m_Published)
            {
                // publish the modified feed back up to the service
                Microsoft.Samples.FeedSyncService.FeedManager.UpdateFeedContents
                    (
                    m_Feed.XmlDocument.OuterXml,
                    m_FeedURL
                    );

                m_Published = true;
            }

            if (refreshView)
            {
                // get the node id of the current node to perserve current view
                string currentNodeID = CurrentItemNode.SyncNode.ID;

                // load new feed
                LoadFeedContents();

                // display the note that was previously being displayed
                DisplayNoteByID(currentNodeID);
            }
        }

        private void Synchronize()
        {
            try
            {
                // save any new note text in the current note
                UpdateNote();

                if (m_FeedURL != null)
                {
                    // read feed from the service
                    Microsoft.Samples.FeedSync.Feed incomingFeed = ReadFeedFromURL(m_FeedURL);
                    SyncToIncomingFeed(incomingFeed);
                }

                // write changes to disk
                SaveLocalFeed();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Synchronization Error");
            }
        }

        private void CreateNote()
        {
            int itemIndex = m_Feed.FeedItemNodes.Length;

            string title = string.Format("Item #{0}", itemIndex);
            string ID = string.Format("Item_{0}.{1}", itemIndex, System.Guid.NewGuid().ToString());

            //  Create new FeedItemNode
            Microsoft.Samples.FeedSync.FeedItemNode feedItemNode =
                Microsoft.Samples.FeedSync.FeedItemNode.CreateNew
                    (
                    m_Feed,
                    title,
                    "",
                    ID,
                    m_Username
                    );

            //  Add FeedItemNode to feed
            m_Feed.AddFeedItem(feedItemNode);

            // And to our own list of nodes
            Microsoft.Samples.FeedSync.FeedItemNode itemNode = m_Feed.GetFeedItemNode(ID);
            m_ItemNodes.AddFeedItemNode(itemNode);

            // Remember that the feed has to be published
            m_Published = false;

            // Display this new item, which is now the last one
            m_CurrentItemIndex = m_ItemNodes.Count - 1;

            DisplayCurrentNote();
        }

        private void DeleteNote()
        {
            // Delete the current item from the feed
            m_Feed.DeleteFeedItem(CurrentItemNode.SyncNode.ID, DateTime.Now, m_Username);

            // and from local list
            m_ItemNodes.RemoveAt(m_CurrentItemIndex);

            // remember that this change needs to be published
            m_Published = false;

            // If the current note was the last one, rewind to the previous
            if (m_CurrentItemIndex == m_ItemNodes.Count - 1 && m_CurrentItemIndex != 0)
            {
                --m_CurrentItemIndex;
            }

            DisplayCurrentNote();
        }

        private void UpdateNote(bool force)
        {
            // see if the note actually changed contents
            if (force || textBox1.Text != CurrentItemNode.Description)
            {
                // update the feed with new data
                m_Feed.UpdateFeedItem
                    (
                    CurrentItemNode.Title,
                    textBox1.Text,
                    CurrentItemNode.SyncNode.ID,
                    System.DateTime.Now,
                    m_Username,
                    true
                    );

                DisplayConflictStatus();

                // remember that the feed needs to be published now
                m_Published = false;
            }
        }

        private void UpdateNote()
        {
            UpdateNote(false);
        }

        #region Event handlers

        private void MainForm_Load(object sender, EventArgs e)
        {
            CreateOrOpenFeed();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // serialize changes to disk:
            SaveLocalFeed();

            // save settings:
            if (!string.IsNullOrEmpty(m_FeedURL))
            {
                Properties.Settings.Default.FeedURL = m_FeedURL;
            }

            Properties.Settings.Default.Save();
        }

        private void toolStripButtonDeleteNote_Click(object sender, EventArgs e)
        {
            DeleteNote();
        }

        private void toolStripButtonPreviousNote_Click(object sender, EventArgs e)
        {
            DisplayPreviousNote();
        }

        private void toolStripButtonNextNote_Click(object sender, EventArgs e)
        {
            DisplayNextNote();
        }

        private void toolStripButtonCreateNote_Click(object sender, EventArgs e)
        {
            UpdateNote();
            CreateNote();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // reset the timer to wait before updating, since the user
            // is now typing in the current note
            timer1.Stop();

            if (!string.IsNullOrEmpty(m_FeedURL))
            {
                timer1.Start();
            }
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            // this event will be fired when the focus changes
            // if the text has been altered, update the feed
            UpdateNote();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Synchronize();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Perform a manual refresh, and restart the timer
            timer1.Stop();

            Synchronize();

            timer1.Start();
        }

        private void toolStripButtonConflict_Click(object sender, EventArgs e)
        {
            ResolveConflictsForm resolveConflictsForm = new ResolveConflictsForm();
            resolveConflictsForm.ConflictItem = CurrentItemNode;

            // Turn off syncing while displaying the dialog:
            timer1.Stop();

            if (resolveConflictsForm.ShowDialog(this) == DialogResult.OK)
            {
                Microsoft.Samples.FeedSync.FeedItemNode resolvedItemNode = resolveConflictsForm.ResolvedItem;

                // Update view of current item. This will be updated the next time synchronization happens
                textBox1.Text = resolvedItemNode.Description;

                // force update so that the conflict is resolved even if the user picks
                // the current item
                UpdateNote(true);
            }

            timer1.Start();
        }

        private void syncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnterFeedForm enterFeedForm = new EnterFeedForm();
            enterFeedForm.FeedURL = m_FeedURL;
            enterFeedForm.FeedType = SHARED_NOTES_FEED_TYPE;

            if (enterFeedForm.ShowDialog(this) == DialogResult.OK)
            {
                // Store the feed url that was entered
                m_FeedURL = enterFeedForm.FeedURL;
                m_Since = enterFeedForm.Since;

                // update any current item before synchronizing
                UpdateNote();

                // Synchronize with the feed that was entered
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(enterFeedForm.FeedContents);
                SyncToIncomingFeed(Microsoft.Samples.FeedSync.Feed.Create(xmlDoc));

                // now that there is a feed to synchronize with, start a timer to do this periodically
                timer1.Start();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}