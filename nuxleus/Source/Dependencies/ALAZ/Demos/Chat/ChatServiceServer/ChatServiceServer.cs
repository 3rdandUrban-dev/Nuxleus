using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using ALAZ.SystemEx.NetEx.SocketsEx;

namespace ChatServiceServer
{
    public partial class ChatServiceServer : ServiceBase
    {
        
        private SocketServer chatServer;
        
        public ChatServiceServer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            //----- Set Threads!
            ThreadPool.SetMinThreads(4, 4);
            ThreadPool.SetMaxThreads(32, 32);
            
            chatServer = new SocketServer(new ChatSocketService.ChatSocketService());

            chatServer.Delimiter = new byte[] { 0xAA, 0xFF, 0xAA };
            chatServer.DelimiterType = DelimiterType.dtMessageTailExcludeOnReceive;

            chatServer.SocketBufferSize = 1024;
            chatServer.MessageBufferSize = 512;

            //----- Socket Listener!
            SocketListener listener = chatServer.AddListener("Char Server", new IPEndPoint(IPAddress.Any, 8090));

            listener.AcceptThreads = 3;
            listener.BackLog = 50;

            listener.CompressionType = CompressionType.ctNone;
            listener.EncryptType = EncryptType.etRijndael;
            listener.CryptoService = new ChatCryptService.ChatCryptService();

            chatServer.Start();

        }

        protected override void OnStop()
        {

            chatServer.Stop();
            chatServer.Dispose();
            chatServer = null;
            
        }
    }
}
