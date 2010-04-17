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
using Nuxleus.Messaging.QS;
using Nuxleus.Logging;

namespace Nuxleus.Service {

    public class BlipQueueServerService : ServiceBase {
        Container components = null;
        MessageServer server;
        BlipMessageServerHandler blipMessageHandler = null;

        public BlipQueueServerService ( int port, string[] memcachedServers, string topLevelQueueId ) {
            // This call is required by the Windows.Forms Component Designer.
            InitializeComponent();
            server = new MessageServer(port, "\r\n\r\n");
            blipMessageHandler = new BlipMessageServerHandler(memcachedServers, topLevelQueueId);
            blipMessageHandler.Service = server.Service;
        }

        // The main entry point for the process
        public static void Main ( object[] args ) {
            ServiceBase[] ServicesToRun;
            string[] memcachedServers = ((string)args[1]).Split(':');
            string topLevelQueueId = (string)args[2];
            ServicesToRun = new ServiceBase[] { new BlipQueueServerService((int)args[0], 
									     memcachedServers, 
									     topLevelQueueId) };
            ServiceBase.Run(ServicesToRun);
        }


        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            components = new Container();
            this.ServiceName = "nuXleus queue server";
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose ( bool disposing ) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        protected override void OnStart ( string[] args ) {
            try {
                Log.Write("Starting nuXleus queue server...");
                server.Start();
            } catch (Exception ex) {
                Log.Write(ex);
            }
        }

        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop () {
            try {
                Log.Write("Stopping nuXleus queue server...");
                blipMessageHandler.Close();
                server.Stop();
                this.Dispose();
            } catch (Exception ex) {
                Log.Write(ex);
            }
        }



    }

}