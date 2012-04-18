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
    public class LLUPRouterService : ServiceBase
    {
        Container components = null;
        MessageClient boundTo = null;
        MessageServer servingOn = null;
        RouterHandler router = null;

        // Using the default filtering, ie. forward all notifications.
        public LLUPRouterService (string ipToBind, int portToBind, int servingPort)
            : this(ipToBind, portToBind, servingPort, null)
        {
        }

        public LLUPRouterService (string ipToBind, int portToBind, int servingPort, IRouterFilter filter)
        {
            // This call is required by the Windows.Forms Component Designer.
            InitializeComponent();

            boundTo = new MessageClient(ipToBind, portToBind, "\r\n");
            servingOn = new MessageServer(servingPort, "\r\n");

            router = new RouterHandler();
            router.ReceiverService = boundTo.Service;
            router.FilterService = servingOn.Service;
            if (filter != null)
                router.Filter = filter;
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
            this.ServiceName = "nuXleus llup router servers";
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
                Log.Write("Starting nuXleus llup router servers...");
                servingOn.Start();
                boundTo.Open();
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
                Log.Write("Stopping nuXleus llup router servers...");
                boundTo.Close();
                servingOn.Stop();
                this.Dispose();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }



    }

}