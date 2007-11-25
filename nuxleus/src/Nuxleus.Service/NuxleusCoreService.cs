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

namespace Nuxleus.Service
{
    public class NuxleusCoreService : ServiceBase
    {
        Container components = null;
        MessageServer _nuxleusCoreServer = null;
        PublisherHandler pub = null;
        static string _serviceName = _serviceName;

        public NuxleusCoreService(int port)
        {
            // This call is required by the Windows.Forms Component Designer.
            InitializeComponent();

            _nuxleusCoreServer = new MessageServer(port, "\r\n");

            pub = new PublisherHandler();
            pub.ReceiverService = _nuxleusCoreServer.Service;

            ///TODO: This needs to be integrating into the core messaging server
            ///and used to dispatch requests based on the number of processors
            ///on the system.  See LoadBalancer folder for more detail.

            //LoadBalancer loadBalancer = LoadBalancer.GetLoadBalancer();

        }

        // The main entry point for the process
        public static void Main(object[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new NuxleusCoreService((int)args[0]) };
            ServiceBase.Run(ServicesToRun);
        }


        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new Container();
            this.ServiceName = _serviceName;
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
                Log.Write("Starting " + _serviceName);
                _nuxleusCoreServer.Start();
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
                Log.Write("Stopping " + _serviceName);
                _nuxleusCoreServer.Stop();
                this.Dispose();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

    }

}