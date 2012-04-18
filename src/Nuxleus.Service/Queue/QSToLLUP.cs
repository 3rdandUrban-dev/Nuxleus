using System;
using System.ComponentModel;
using System.ServiceProcess;
using Nuxleus.Messaging;
using Nuxleus.Messaging.QS;

namespace Nuxleus.Service
{
    public class QsToLLUPService : ServiceBase
    {
        Container components = null;
        MessageClient qsConnection = null;
        MessageServer pubServer = null;
        QSToLLUPHandler handler = null;


        public QsToLLUPService(string qsIp, int qsPort, int pubPort, string[] monitoredQueues)
        {
            // This call is required by the Windows.Forms Component Designer.
            InitializeComponent();

            qsConnection = new MessageClient(qsIp, qsPort, "\r\n\r\n");
            pubServer = new MessageServer(pubPort, "\r\n");

            handler = new QSToLLUPHandler();

            handler.PollService = qsConnection.Service;
            handler.DispatcherService = pubServer.Service;

            foreach (string queueId in monitoredQueues)
            {
                handler.MonitoredQueues.Add(queueId);
            }
        }

        // The main entry point for the process
        public static void Main(object[] args)
        {
            ServiceBase[] ServicesToRun;
            string[] queues = ((string)args[3]).Split(',');
            ServicesToRun = new ServiceBase[] { new QsToLLUPService((string)args[0], (int)args[1], 
                    (int)args[2], queues) };
            ServiceBase.Run(ServicesToRun);
        }


        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new Container();
            this.ServiceName = "nuXleus llup publisher servers";
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
                Log.Write("Starting nuXleus llup publisher servers...");
                pubServer.Start();
                qsConnection.Open();
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
                Log.Write("Stopping nuXleus llup publisher servers...");
                handler.StopMonitoring();
                qsConnection.Close();
                pubServer.Stop();
                this.Dispose();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }



    }

}