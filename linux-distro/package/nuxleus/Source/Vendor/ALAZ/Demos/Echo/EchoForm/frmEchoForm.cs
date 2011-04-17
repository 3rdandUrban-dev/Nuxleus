using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;

using EchoSocketService;

namespace EchoFormTemplate
{
    
    public partial class frmEchoForm : Form
    {

        public OnEventDelegate FEvent;
        private int FConnectionCount;
        
        public frmEchoForm()
        {
            InitializeComponent();
        }

        public void Event(string eventMessage)
        {
            Event(eventMessage, false);
        }

        public void Event(string eventMessage, bool ex)
        {

            if (lstMessages.InvokeRequired)
            {
                lstMessages.BeginInvoke(new OnEventDelegate(delegate(string s) { Event(s); }), eventMessage);
            }
            else
            {

                if (eventMessage.Contains("Connected"))
                {
                    Interlocked.Increment(ref FConnectionCount);
                }

                if (eventMessage.Contains("Disconnected"))
                {
                    Interlocked.Decrement(ref FConnectionCount);
                }

                this.Text = FConnectionCount.ToString() + " Connections";

                string[] s = eventMessage.Split('\n');

                for (int i = s.Length - 1; i >= 0; i--)
                {
                    lstMessages.BeginUpdate();
                    lstMessages.Items.Insert(0, s[i]);

                    if (lstMessages.Items.Count > 100)
                        lstMessages.Items.RemoveAt(100);

                    lstMessages.EndUpdate();
                }

            }

        }

        private void frmServer_Load(object sender, EventArgs e)
        {
            FEvent = new OnEventDelegate(Event);
            FConnectionCount = 0;
        }

        public void OnException(Exception ex)
        {
            Event("Service Exception! - " + ex.Message, true);
            Event("------------------------------------------------", true);
        }

    }
}