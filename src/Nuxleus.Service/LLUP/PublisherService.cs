using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Configuration.Install;
using Nuxleus.Messaging;
using Nuxleus.Messaging.LLUP;
//using Nuxleus.Logging;

namespace Nuxleus.Service
{
    public class LLUPPublisherService : ServiceBase
    {
        Container components = null;
        MessageServer pubServer = null;
        MessageServer busServer = null;
        PublisherHandler pub = null;


        public LLUPPublisherService (int pubPort, int busPort)
        {
            // This call is required by the Windows.Forms Component Designer.
            InitializeComponent();

            pubServer = new MessageServer(pubPort, "\r\n");
            busServer = new MessageServer(busPort, "\r\n");

            pub = new PublisherHandler();

            pub.ReceiverService = pubServer.Service;
            pub.DispatcherService = busServer.Service;
        }

        // The main entry point for the process
        public static void Main (object[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new LLUPPublisherService((int)args[0], (int)args[1]) };
            ServiceBase.Run(ServicesToRun);
        }


        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
        {
            components = new Container();
            this.ServiceName = "nuXleus llup publisher servers";
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose (bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        protected override void OnStart (string[] args)
        {
            try
            {
                Log.Write("Starting nuXleus llup publisher servers...");
                busServer.Start();
                pubServer.Start();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop ()
        {
            try
            {
                Log.Write("Stopping nuXleus llup publisher servers...");
                pubServer.Stop();
                busServer.Stop();
                this.Dispose();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }



    }

}