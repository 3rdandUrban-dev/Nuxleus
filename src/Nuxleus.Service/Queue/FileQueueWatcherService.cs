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
using Nuxleus.Service.Drawing;
//using Nuxleus.Logging;
using Nuxleus.Service.FileQueue;

namespace Nuxleus.Service {

    public class FileQueueWatcherService : ServiceBase {

        Container components = null;
        static FileQueueWatcher m_fileWatcher;
        static string m_serviceName = "IAct Queue Watcher Service";
        string m_folderName;

        public FileQueueWatcherService ( string queueFolder ) {
            // This call is required by the Windows.Forms Component Designer.
            InitializeComponent();
            string logPath = Path.Combine(
                Path.GetDirectoryName(Environment.CurrentDirectory), "../App_Data/Queue.log");
            FileStream fs = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.Default);
            m_folderName = queueFolder;
            m_fileWatcher = new FileQueueWatcher(queueFolder, "", Console.Out);

        }

        // The main entry point for the process
        public static void Main ( object[] args ) {
            ServiceBase[] ServicesToRun = new ServiceBase[] { new FileQueueWatcherService((string)args[1]) };
            ServiceBase.Run(ServicesToRun);
        }

        private void InitializeComponent () {
            components = new Container();
            this.ServiceName = m_serviceName;
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
                Log.Write("Starting " + m_serviceName);
                Log.Write("Watching " + m_folderName);
                m_fileWatcher.Watch(false);
            } catch (Exception ex) {
                Log.Write(ex);
            }
        }

        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop () {
            try {
                Log.Write("Stopping " + m_serviceName);
                this.Dispose();
            } catch (Exception ex) {
                Log.Write(ex);
            }
        }

    }

}