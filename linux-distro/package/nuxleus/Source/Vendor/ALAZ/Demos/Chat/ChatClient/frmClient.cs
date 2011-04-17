using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using ALAZ.SystemEx;
using ALAZ.SystemEx.NetEx.SocketsEx;

using ChatSocketService;

namespace ChatClient
{
 
    public partial class frmClient : Form
    {

        private const int WM_VSCROLL = 277; // Vertical scroll
        private const int SB_PAGEBOTTOM = 7; // Scrolls to the upper right

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        delegate void UpdateListDel(string text);
        delegate void AddUserDel(UserInfo[] users);
        
        UserInfo userInfo;
        
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

                this.Invoke(new UpdateListDel(
                    delegate(string innerText)
                    {
                    
                        this.UpdateList(innerText);

                    }), text);

            }
            else
            {

                string[] data = text.Split( "\r\n".ToCharArray());

                this.lstStatus.Items.Add(DateTime.Now.ToString("hh:mm:ss.fff") + " - " + data[0]);

                if (data.Length > 1)
                {
                    this.lstStatus.Items.Add(data[2]);
                }

                SendMessage(lstStatus.Handle, WM_VSCROLL, (IntPtr)SB_PAGEBOTTOM, IntPtr.Zero);
                Application.DoEvents();
            }

        }

        private void CheckMsg(string data)
        {

            ChatMessage msg = ChatSocketService.ChatSocketService.DeserializeMessage(Encoding.GetEncoding(1252).GetBytes(data));
            
            switch(msg.MessageType)
            {
                
                case MessageType.mtLogin:
                    
                    userInfo = msg.UserInfo[0];
                    UpdateList(userInfo.UserName + " has entered!");
                    break;

                case MessageType.mtAuthenticated:

                    UpdateList(msg.UserInfo[0].UserName + " has entered!");
                    AddUser(msg.UserInfo);
                    
                    break;

                case MessageType.mtMessage:
                
                    UpdateList(msg.UserInfo[0].UserName + "\r\n" + msg.Message);
                    break;

                case MessageType.mtHello:

                    AddUser(msg.UserInfo);
                    break;

                case MessageType.mtLogout:

                    RemoveUser(msg.UserInfo);
                    break;
                    
                    
            }

        }
        
        private void AddUser(UserInfo[] users)
        {
            
            if (this.InvokeRequired)
            {

                this.Invoke(
                    new AddUserDel(
                        delegate(UserInfo[] innerUsers)
                        {
                            this.AddUser(innerUsers);
                        }
                    ), users);

            }
            else
            {

                foreach (UserInfo u in users)
                {
                    if (u.UserName != null)
                    {
                        this.lstUsers.Items.Add(u);
                    }
                }

            }
        
        }

        private void RemoveUser(UserInfo[] users)
        {

            if (this.InvokeRequired)
            {

                this.Invoke(
                    new AddUserDel(
                        delegate(UserInfo[] innerUsers)
                        {
                            this.RemoveUser(innerUsers);
                        }
                    ), users);

            }
            else
            {

                bool found = false;
                
                for (int j = 0; j < users.Length; j++)
			    {
    			
                    if (users[j].UserName != null)
                    {
                        for (int i = 0; i < this.lstUsers.Items.Count; i++)
                        {
                            
                            UserInfo u = (UserInfo) this.lstUsers.Items[i];
                            
                            if (u.UserId == users[j].UserId)
                            {
                                this.lstUsers.Items.RemoveAt(i);
                                found = true;
                                break;    
                            }

        	            }
        	            
        	        }
        	        
        	        if (found)
        	        {

                        UpdateList(users[0].UserName + " quits!");

        	            break;
        	        }

                }
                
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
                        CheckMsg(read);
                    }

                }

                Thread.Sleep(11);

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (!client.Connected)
            {

                client.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(txtHost.Text), 8090);
                
                client.DelimiterType = DelimiterType.dtMessageTailExcludeOnReceive;
                client.Delimiter = new byte[] {0xAA, 0xFF, 0xAA};

                client.EncryptType = EncryptType.etRijndael;
                client.CompressionType = CompressionType.ctNone;

                client.SocketBufferSize = 1024;
                client.MessageBufferSize = 512;

                client.Connect();
                
                if (client.Connected)
                {

                    this.lstUsers.Items.Clear();
                    this.lstStatus.Items.Clear();

                    read = new Thread(new ParameterizedThreadStart(ThreadExecute));
                    read.Start(client);
                    
                    ChatMessage msg = new ChatMessage();
                    msg.MessageType = MessageType.mtLogin;

                    msg.UserInfo = new UserInfo[1];
                    msg.UserInfo[0].UserName = txtNickName.Text;
                    
                    client.Write(ChatSocketService.ChatSocketService.SerializeMessage(msg));

                }
                else
                { 
                    UpdateList("Not Connected! " + client.LastException != null ? client.LastException.Message : String.Empty);
                }

            }

        }

        private void frmClient_Load(object sender, EventArgs e)
        {

            client = new SocketClientSync(null);
            client.OnSymmetricAuthenticate += new OnSymmetricAuthenticateEvent(client_OnSymmetricAuthenticate);

        }

        static void client_OnSymmetricAuthenticate(ISocketConnection connection, out System.Security.Cryptography.RSACryptoServiceProvider serverKey, out byte[] signMessage)
        {
            
            //----- Sign Message!
            signMessage = new byte[]
            {
                
                0x1E, 0xF0, 0x5F, 0x4D, 0x2B, 0x33, 0xCE, 0xB0, 
                0x12, 0x29, 0x31, 0xCA, 0xEF, 0xAB, 0xEC, 0x97, 
                0xB3, 0x73, 0x2E, 0xDD, 0x2D, 0x58, 0xAC, 0xE9, 
                0xE0, 0xCC, 0x14, 0xDC, 0x14, 0xEF, 0x97, 0x64,
                0x38, 0xC6, 0x1C, 0xD8, 0x87, 0xFC, 0x30, 0xD5,
                0x79, 0xE4, 0x10, 0x2C, 0xFE, 0x98, 0x30, 0x2C, 
                0xFF, 0xAE, 0x51, 0xD5, 0x47, 0x1D, 0x4D, 0xC5, 
                0x43, 0x75, 0x6C, 0x5E, 0x32, 0xF2, 0x9C, 0x22,
                0x51, 0xBE, 0xA2, 0xC5, 0x31, 0x19, 0xAE, 0x21, 
                0x3D, 0x9A, 0xF2, 0x78, 0x90, 0x19, 0xCF, 0x97, 
                0xA5, 0x75, 0x99, 0xB3, 0xFD, 0x31, 0xE6, 0xB5, 
                0x7F, 0xFD, 0xD0, 0x37, 0x26, 0xC2, 0x7B, 0x27, 
                0x18, 0x43, 0xED, 0xD9, 0xC8, 0x5A, 0xF5, 0xE0, 
                0xDA, 0x33, 0x41, 0x3A, 0xC8, 0xE7, 0x4A, 0x5C, 
                0x9D, 0x48, 0x95, 0x22, 0x56, 0x2F, 0x62, 0x20, 
                0xD8, 0xEC, 0x46, 0x52, 0x49, 0x76, 0xFB, 0x7B

            };

            //----- Using string!

            //----- You must get the public key xml from the ALAZ certificate in you server machine.
            //----- Uncomment the following lines to get the public key from certificate.

            //---- Get certificate!
            // X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            // store.Open(OpenFlags.ReadOnly);
            // X509Certificate2 certificate = store.Certificates.Find(X509FindType.FindBySubjectName, "ALAZ Library", true)[0];

            //---- Get public key string!
            // string publicKey = certificate.PrivateKey.ToXmlString(false);

            serverKey = new RSACryptoServiceProvider();
            serverKey.FromXmlString("<RSAKeyValue><Modulus>x66+m1J+bNfaDmUCDl/XEi5tZUSPvtJ1AxsZAR2awHI+xKtB8320oBZiKDuEY0MticpMtfvEkBGDSYXtMCxtpQ+4B6DydwwoQhUc+XDHNnCJ3agCDLQ30Tt/lDLMvEVzyXCgaC7J6KM9PT533b6Khlz96gNIht7e0sSDAVP76Hc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
            
             
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (client.Connected)
            {

                ChatMessage msg = new ChatMessage();
                msg.MessageType = MessageType.mtLogout;

                msg.UserInfo = new UserInfo[1];
                msg.UserInfo[0].UserName = userInfo.UserName;
                msg.UserInfo[0].UserId = userInfo.UserId;

                client.Write(ChatSocketService.ChatSocketService.SerializeMessage(msg));

                UpdateList("Disconnecting!");

                Thread.Sleep(1000);
                
                client.Disconnect();

                if (!client.Connected)
                {
                    UpdateList("Disconnected!");
                }
                else
                {
                    UpdateList("Not Disconnected! " + client.LastException != null ? client.LastException.Message : String.Empty);
                }

                this.lstUsers.Items.Clear();
                
                Application.DoEvents();

                read.Join();
                read = null;

                Application.DoEvents();

            }

        }

        private void frmClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            if(client != null)
            {
            
                if (client.Connected)
                {
                    client.Disconnect();
                }

                client.Dispose();
                
            }
            
        }

        private void Send()
        {

            if (client.Connected)
            {

                ChatMessage msg = new ChatMessage();

                msg.UserInfo = new UserInfo[1];
                msg.UserInfo[0] = userInfo;

                msg.MessageType = MessageType.mtMessage;
                msg.Message = txtMessage.Text;

                client.Write(ChatSocketService.ChatSocketService.SerializeMessage(msg));

                if (client.LastException != null)
                {
                    UpdateList("Write Error! " + client.LastException.Message);
                }
                else
                {
                    txtMessage.Clear();
                    UpdateList(msg.UserInfo[0].UserName + "\r\n" + msg.Message);
                }

            }
        
        }
        
        private void button3_Click_1(object sender, EventArgs e)
        {

            Send();

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
            
        }

        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (txtMessage.Text.Length > 0)
                {
                    Send();
                }
            }
        }

    }

}