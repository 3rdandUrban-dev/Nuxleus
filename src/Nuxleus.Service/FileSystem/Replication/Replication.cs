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
using Nuxleus.Messaging.Replication;

namespace Nuxleus.Service
{
    public class ReplicationService : ServiceBase
    {
        Container components = null;
        MessageServer server;
        ReplicationHandler handler = null;

        public ReplicationService(int port)
        {
            // This call is required by the Windows.Forms Component Designer.
            InitializeComponent();
            server = new MessageServer(port, "\n");
            handler = new ReplicationHandler();
            handler.Service = server.Service;
        }

        // The main entry point for the process
        public static void Main(object[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new ReplicationService((int)args[0]) };
            ServiceBase.Run(ServicesToRun);
        }


        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new Container();
            this.ServiceName = "nuXleus File Replication Service";
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
                Log.Write("Starting nuXleus File Replication Service...");
                server.Start();
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
                Log.Write("Stopping nuXleus File Replication Service...");
                server.Stop();
                this.Dispose();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }



    }

    public sealed class Log
    {
        private static string _logPath;

        static Log()
        {
            _logPath = Path.Combine(
                Path.GetDirectoryName(typeof(Log).Assembly.Location),
                "Test.log");
        }

        public static void Write(string msg)
        {
            using (FileStream fs = new FileStream(_logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter w = new StreamWriter(fs, System.Text.Encoding.Default))
            {
                w.WriteLine("{0:yyyy-MM-dd HH:mm:ss} - {1}", DateTime.Now, msg);
                w.Flush();
            }
        }

        public static void Write(Exception ex)
        {
            StringBuilder message = new StringBuilder();
            StringBuilder stacktrace = new StringBuilder();

            for (Exception e = ex; e != null; e = e.InnerException)
            {
                if (message.Length > 0)
                    message.Append(Environment.NewLine);

                message.Append(e.Message);

                if (stacktrace.Length > 0)
                {
                    stacktrace.Append(Environment.NewLine);
                    stacktrace.Append("----");
                }
                stacktrace.Append(e.StackTrace);
            }

            Write(message.ToString() + Environment.NewLine + stacktrace.ToString());
        }
    }



    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller1;
        private ServiceInstaller serviceInstaller1;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        public ProjectInstaller()
        {
            // This call is required by the Designer.
            InitializeComponent();
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


        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new ServiceProcessInstaller();
            this.serviceInstaller1 = new ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // serviceInstaller1
            // 
            this.serviceInstaller1.DisplayName = "nuXleus File Replication Service";
            this.serviceInstaller1.ServiceName = "Log.Write";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
                                            this.serviceProcessInstaller1,
                                            this.serviceInstaller1});

        }
        #endregion
    }
}