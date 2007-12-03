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
using Nuxleus.Messaging.Core;
using Nuxleus.Logging;

namespace Nuxleus.Service
{
    public class NuxleusCoreService : ServiceBase
    {
        Container components = null;
        MessageServer _nuxleusCoreMessageServer = null;
        NuxleusCoreHandler _nuxleusCoreHandler = null;
        static string _serviceName = "Nuxleus Core Messaging Service";

        public NuxleusCoreService(int port)
        {
            // This call is required by the Windows.Forms Component Designer.
            InitializeComponent();

            _nuxleusCoreMessageServer = new MessageServer(port, "\r\n");

            _nuxleusCoreHandler = new NuxleusCoreHandler();
            _nuxleusCoreHandler.ReceiverService = _nuxleusCoreMessageServer.Service;

            ///TODO: This needs to be integrating into the core messaging server
            ///and used to dispatch requests based on the number of processors
            ///on the system.  See LoadBalancer folder for more detail.

            ///LoadBalancer loadBalancer = LoadBalancer.GetLoadBalancer();

        }

        // The main entry point for the process
        public static void Main(object[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new NuxleusCoreService((int)args[0]) };
            ServiceBase.Run(ServicesToRun);
        }

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
                _nuxleusCoreMessageServer.Start();
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
                _nuxleusCoreMessageServer.Stop();
                this.Dispose();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

    }

}