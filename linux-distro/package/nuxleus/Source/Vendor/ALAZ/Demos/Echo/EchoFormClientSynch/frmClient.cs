using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using ALAZ.SystemEx;
using ALAZ.SystemEx.NetEx.SocketsEx;

namespace ClientSynch
{
 
    public partial class frmClient : Form
    {

        delegate void UpdateListDel(string text);
        
        SocketClientSync client;
        Thread read;

        public frmClient()
        {
            InitializeComponent();
        }

        private void UpdateList(string text)
        {

            if (this.InvokeRequired)
            {

                this.BeginInvoke(new UpdateListDel(
                    delegate(string innerText)
                    {
                    
                        this.UpdateList(innerText);

                    }), text);

            }
            else
            {

                this.lstStatus.Items.Insert(0, DateTime.Now.ToString("hh:mm:ss.fff") + " - " + text);

            }

        }

        private void ThreadExecute(object data)
        {

            SocketClientSync client = (SocketClientSync)data;
            string read = null;

            while (client.Connected)
            {
                
                read = client.Read(500);

                if (client.LastException != null)
                {
                    if (!(client.LastException is TimeoutException))
                    {
                        UpdateList("Read Error! " + client.LastException.Message);
                        client.Disconnect();
                    }
                }
                else
                {

                    if (read != null)
                    {
                        UpdateList("Read <- " + read);
                    }

                }

                Thread.Sleep(11);

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (!client.Connected)
            {

                client.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(txtHost.Text), Convert.ToInt32(txtPort.Text));
                
                client.DelimiterType = (DelimiterType)Enum.Parse(typeof(DelimiterType), cboDelimiter.Text);
                client.Delimiter = Encoding.GetEncoding(1252).GetBytes(txtDelimiter.Text);

                client.EncryptType = (EncryptType)Enum.Parse(typeof(EncryptType), cboEncrypt.Text);
                client.CompressionType = (CompressionType)Enum.Parse(typeof(CompressionType), cboCompression.Text);

                client.SocketBufferSize = 256;
                client.MessageBufferSize = 512;
                
                if (cboProxyType.SelectedIndex > -1)
                {
                    
                    client.ProxyInfo = new ProxyInfo((ProxyType)Enum.Parse(typeof(ProxyType), cboProxyType.Text),
                                new IPEndPoint(IPAddress.Parse(txtProxyHost.Text), Convert.ToInt32(txtProxyPort.Text)),
                                new NetworkCredential(txtProxyUser.Text, txtProxyPass.Text, txtProxyDomain.Text));
                                
                }
                
                client.Connect();
                
                if (client.Connected)
                {

                    UpdateList("Connected!");

                    read = new Thread(new ParameterizedThreadStart(ThreadExecute));
                    read.Start(client);

                }
                else
                { 
                    UpdateList("Not Connected! " + client.LastException != null ? client.LastException.Message : String.Empty);
                }

            }

        }

        private void frmClient_Load(object sender, EventArgs e)
        {

            cboCompression.SelectedIndex = 0;
            cboDelimiter.SelectedIndex = 0;
            cboEncrypt.SelectedIndex = 0;
            cboProxyType.SelectedIndex = -1;

            client = new SocketClientSync(null);
            client.OnSymmetricAuthenticate += new OnSymmetricAuthenticateEvent(client_OnSymmetricAuthenticate);
            client.OnSSLClientAuthenticate += new OnSSLClientAuthenticateEvent(client_OnSSLClientAuthenticate);
            client.OnDisconnected += new OnDisconnectEvent(client_OnDisconnected);

        }

        void client_OnDisconnected()
        {
            UpdateList("Disconnected Event!");
        }

        static void client_OnSSLClientAuthenticate(ISocketConnection connection, out string serverName, ref X509Certificate2Collection certs, ref bool checkRevocation)
        {

            serverName = "ALAZ Library";

            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            certs = store.Certificates.Find(X509FindType.FindBySubjectName, serverName, true);
            checkRevocation = false;

            store.Close();

        }

        static void client_OnSymmetricAuthenticate(ISocketConnection connection, out System.Security.Cryptography.RSACryptoServiceProvider serverKey, out byte[] signMessage)
        {
            
            //----- Sign Message!
            signMessage = new byte[]
            {
                
                0x51, 0xBE, 0xA2, 0xC5, 0x31, 0x19, 0xAE, 0x21, 
                0x3D, 0x9A, 0xF2, 0x78, 0x90, 0x19, 0xCF, 0x97, 
                0xA5, 0x75, 0x99, 0xB3, 0xFD, 0x31, 0xE6, 0xB5, 
                0x7F, 0xFD, 0xD0, 0x37, 0x26, 0xC2, 0x7B, 0x27, 
                0x18, 0x43, 0xED, 0xD9, 0xC8, 0x5A, 0xF5, 0xE0, 
                0xDA, 0x33, 0x41, 0x3A, 0xC8, 0xE7, 0x4A, 0x5C, 
                0x9D, 0x48, 0x95, 0x22, 0x56, 0x2F, 0x62, 0x20, 
                0xD8, 0xEC, 0x46, 0x52, 0x49, 0x76, 0xFB, 0x7B, 
                0x1E, 0xF0, 0x5F, 0x4D, 0x2B, 0x33, 0xCE, 0xB0, 
                0x12, 0x29, 0x31, 0xCA, 0xEF, 0xAB, 0xEC, 0x97, 
                0xB3, 0x73, 0x2E, 0xDD, 0x2D, 0x58, 0xAC, 0xE9, 
                0xE0, 0xCC, 0x14, 0xDC, 0x14, 0xEF, 0x97, 0x64,
                0x38, 0xC6, 0x1C, 0xD8, 0x87, 0xFC, 0x30, 0xD5,
                0x79, 0xE4, 0x10, 0x2C, 0xFE, 0x98, 0x30, 0x2C, 
                0xFF, 0xAE, 0x51, 0xD5, 0x47, 0x1D, 0x4D, 0xC5, 
                0x43, 0x75, 0x6C, 0x5E, 0x32, 0xF2, 0x9C, 0x22

            };
 
            //----- Using CspParameters!
            CspParameters param = new CspParameters();
            param.KeyContainerName = "ALAZ_ECHO_SERVICE";
            serverKey = new RSACryptoServiceProvider(param);

        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (client.Connected)
            {

                client.Disconnect();

                if (!client.Connected)
                {
                    UpdateList("Disconnected!");
                }
                else
                {
                    UpdateList("Not Disconnected! " + client.LastException != null ? client.LastException.Message : String.Empty);
                }

                read.Join();
                read = null;

            }

        }

        private void frmClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            client.Dispose();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

            if (client.Connected)
            {

                client.Write(txtMessage.Text);

                if (client.LastException != null)
                {
                    UpdateList("Write Error! " + client.LastException.Message);
                }
                else
                {
                    UpdateList("Write -> " + txtMessage.Text);
                }

            }

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void cboDelimiter_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            Random r = new Random(DateTime.Now.Millisecond);
            
            timer1.Interval = r.Next(5000, 10000);
            timer1.Start();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button3.PerformClick();
        }

    }

}