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
    public partial class EnterFeedForm : Form
    {
        private string m_FeedURL = "";
        private string m_FeedContents = null;
        private string m_Since = null;
        private Microsoft.Samples.FeedSync.Feed.FeedTypes m_FeedType = Microsoft.Samples.FeedSync.Feed.FeedTypes.RSS;
        
        public EnterFeedForm()
        {
            InitializeComponent();
        }

        public string FeedURL
        {
            get
            {
                return m_FeedURL;
            }

            set
            {
                m_FeedURL = value;
            }
        }

        public string FeedContents
        {
            get
            {
                return m_FeedContents;
            }
        }

        public Microsoft.Samples.FeedSync.Feed.FeedTypes FeedType
        {
            get
            {
                return m_FeedType;
            }

            set
            {
                m_FeedType = value;
            }
        }

        public string Since
        {
            get
            {
                return m_Since;
            }
        }

        private void EnterFeedForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = m_FeedURL;
        }

        private void EnterFeedForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                Cursor cursor = this.Cursor;
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    if (m_FeedType == Microsoft.Samples.FeedSync.Feed.FeedTypes.RSS)
                        m_FeedURL = Microsoft.Samples.FeedSyncService.FeedManager.GetRSSFeedURL(textBox1.Text);
                    else
                        m_FeedURL = Microsoft.Samples.FeedSyncService.FeedManager.GetAtomFeedURL(textBox1.Text);
                    
                    // attempt to load the feed in order to validate this form
                    m_FeedContents = Microsoft.Samples.FeedSyncService.FeedManager.ReadFeedContents(m_FeedURL, ref m_Since);

                    e.Cancel = false;
                }
                catch (Exception ex)
                {
                    string err = "Unable to read feed:\r\n\r\n" + ex.Message;
                    MessageBox.Show(this, err, "Error", MessageBoxButtons.OK);
                    e.Cancel = true;
                }
                finally
                {
                    this.Cursor = cursor;
                }
            }
        }
    }
}
