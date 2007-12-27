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
using Nuxleus.Service.FileSystem;

namespace Nuxleus.Service
{
    public class FileSystemWatcherService : ServiceBase
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        Container components = null;
        Watcher _fileSystemWatcher;
        TextWriter _writer;
        StringBuilder _builder;

        public FileSystemWatcherService(string path)
        {
            // This call is required by the Windows.Forms Component Designer.
            InitializeComponent();
            _builder = new StringBuilder();
            _writer = new StringWriter(_builder);
            _fileSystemWatcher = new Watcher(path, "", _writer);
        }

        // The main entry point for the process
        public static void Main(string[] args)
        {
            Console.WriteLine("Start Service...");
            Console.WriteLine(args[0]);
        }

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new Container();
            this.ServiceName = "File System Watcher Service";
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
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
        protected override void OnStart(string[] args)
        {
            try
            {
                Log.Write("Starting Service...");
                _fileSystemWatcher.Watch(true);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                Log.Write(_builder.ToString());
                this.Dispose();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}