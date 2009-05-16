using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Security.Cryptography;

using ALAZ.SystemEx.NetEx.SocketsEx;

namespace EchoFormServer
{
    public partial class frmEchoServer : EchoFormTemplate.frmEchoForm
    {
        
        private SocketServer FEchoServer;

        public frmEchoServer()
        {
            InitializeComponent();
        }

        private void frmEchoServer_Load(object sender, EventArgs e)
        {

            //----- CspParameters used in CryptService.
            CspParameters param = new CspParameters();
            param.KeyContainerName = "ALAZ_ECHO_SERVICE";
            RSACryptoServiceProvider serverKey = new RSACryptoServiceProvider(param);

            FEchoServer = new SocketServer(new EchoSocketService.EchoSocketService(FEvent), DelimiterType.dtMessageTailExcludeOnReceive, Encoding.GetEncoding(1252).GetBytes("ALAZ"), 1024 * 2, 1024 * 16);

        }

        private void AddListener()
        {

            FEchoServer.AddListener(String.Empty, new IPEndPoint(IPAddress.Any, 8092), EncryptType.etNone, CompressionType.ctNone, new EchoCryptService.EchoCryptService(), 50, 3);

        }

        private void cmdStart_Click(object sender, EventArgs e)
        {

            AddListener();
            FEchoServer.Start();

            Event("Started!");
            Event("---------------------------------");

        }

        private void cmdStop_Click(object sender, EventArgs e)
        {
            
            FEchoServer.Stop();

            Event("Stopped!");
            Event("---------------------------------");

        }
    }
}

