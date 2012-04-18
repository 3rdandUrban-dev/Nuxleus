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
    public class LLUPSubscriberService : ServiceBase
    {
        Container components = null;
        MessageClient[] boundTo = null;
        SubscriberHandler sub = null;
        ISubscriber[] subscribers = null;

        public LLUPSubscriberService(string[] bindAddresses, ISubscriber[] subscribers)
        {
            // This call is required by the Windows.Forms Component Designer.
            InitializeComponent();

            boundTo = new MessageClient[bindAddresses.Length];

            sub = new SubscriberHandler();

            int index = 0;
            foreach (string address in bindAddresses)
            {
                string[] ipAndPort = address.Split(':');
                MessageClient routerToBindTo = new MessageClient(ipAndPort[0], Convert.ToInt32(ipAndPort[1]), "\r\n");
                sub.AddService(routerToBindTo.Service);

                boundTo[index] = routerToBindTo;
                index++;
            }

            this.subscribers = subscribers;
            foreach (ISubscriber s in subscribers)
            {
                s.Handler = sub;
            }
        }

        // The main entry point for the process
        public static void Main(object[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new LLUPPublisherService((int)args[0], (int)args[1]) };
            ServiceBase.Run(ServicesToRun);
        }


        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new Container();
            this.ServiceName = "nuXleus llup router servers";
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
                Log.Write("Starting nuXleus llup router servers...");
                foreach (ISubscriber s in subscribers)
                {
                    s.Start();
                }
                foreach (MessageClient client in boundTo)
                {
                    client.Open();
                }
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
                Log.Write("Stopping nuXleus llup router servers...");
                foreach (MessageClient client in boundTo)
                {
                    sub.RemoveService(client.Service);
                    client.Close();
                }
                foreach (ISubscriber s in subscribers)
                {
                    s.Stop();
                }
                this.Dispose();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }



    }

}