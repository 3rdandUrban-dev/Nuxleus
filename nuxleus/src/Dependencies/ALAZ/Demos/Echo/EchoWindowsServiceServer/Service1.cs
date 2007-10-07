using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Net;
using System.IO;

using ALAZ.SystemEx.NetEx.SocketsEx;
using EchoSocketService;
using EchoCryptService;

namespace EchoWindowsServiceServer
{
    public partial class Service1 : ServiceBase
    {

        private SocketServer FServer;
        private OnEventDelegate FEvent;
        private StreamWriter FEchoLog;

        public Service1()
        {
            
            InitializeComponent();

        }

        protected override void OnStart(string[] args)
        {

            FEvent = new OnEventDelegate(Event);
            FEchoLog = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\EchoLog.txt");

            FServer = new SocketServer(new EchoSocketService.EchoSocketService(FEvent), DelimiterType.dtMessageTailExcludeOnReceive, Encoding.GetEncoding(1252).GetBytes("ALAZ"), 1024 * 2, 1024 * 16);
            FServer.AddListener("Commom Port - 8090", new IPEndPoint(IPAddress.Any, 8090), EncryptType.etRijndael, CompressionType.ctNone, new EchoCryptService.EchoCryptService(), 50, 3);
            FServer.Start();

        }

        protected override void OnStop()
        {

            FServer.Stop();
            FServer.Dispose();

        }

        private void Event(string eventMessage)
        {

            lock (FEchoLog)
            {
                FEchoLog.Write(eventMessage);
                FEchoLog.Flush();
            }

        }

    }

}
