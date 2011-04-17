using System;
using System.ComponentModel;
using System.ServiceProcess;
using Nuxleus.Messaging;
using Nuxleus.Messaging.Core;

namespace Nuxleus.Service
{
    public class NuxleusCoreService : ServiceBase
    {
        Container components = null;
        MessageServer m_nuxleusCoreMessageServer = null;
        NuxleusCoreHandler m_nuxleusCoreHandler = null;
        static string m_serviceName = "Nuxleus Core Messaging Service";

        public NuxleusCoreService(int port)
        {
            // This call is required by the Windows.Forms Component Designer.
            InitializeComponent();

            m_nuxleusCoreMessageServer = new MessageServer(port, "\r\n");

            m_nuxleusCoreHandler = new NuxleusCoreHandler();
            m_nuxleusCoreHandler.ReceiverService = m_nuxleusCoreMessageServer.Service;

            ///TODO: This needs to be integrated into the core messaging server
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
            this.ServiceName = m_serviceName;
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
                Log.Write("Starting " + m_serviceName);
                m_nuxleusCoreMessageServer.Start();
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
                Log.Write("Stopping " + m_serviceName);
                m_nuxleusCoreMessageServer.Stop();
                this.Dispose();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

    }

}