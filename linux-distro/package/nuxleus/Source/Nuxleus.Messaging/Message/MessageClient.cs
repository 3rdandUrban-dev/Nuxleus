//
// MessageClient.cs: Client for any kind of messaging protocol
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

using ALAZ.SystemEx.NetEx.SocketsEx;
using ALAZ.SystemEx.ThreadingEx;

namespace Nuxleus.Messaging
{
    public class MessageClient
    {
        private string id = null;

        private int connectTimeout = 30000; //ms
        private bool connected = false;

        private SocketClient client = null;
        private MessageService service = null;

        public MessageClient(string ip, int port, string delimiter)
        {
            service = new MessageService();
            client = new SocketClient(CallbackThreadType.ctWorkerThread, service);
            client.AddConnector(String.Format("MessageQueue {0}:{1}", ip, port),
                    new IPEndPoint(IPAddress.Parse(ip), port));
            client.Delimiter = Encoding.ASCII.GetBytes(delimiter);
            client.DelimiterType = DelimiterType.dtMessageTailExcludeOnReceive;

            client.SocketBufferSize = 4096;
            client.MessageBufferSize = 4096 * 4;
        }

        /// <summary>
        /// Gets or sets the identifier of the instance.
        /// </summary>
        public string Id { get { return id; } set { id = value; } }

        /// <summary>
        /// Gets or sets the time span within which the connection 
        /// is expected to happen.
        /// </summary>
        public int ConnectionTimeout
        {
            get { return connectTimeout; }
            set { connectTimeout = value; }
        }

        /// <summary> Gets the connection status of the client. </summary>
        public bool Connected { get { return connected; } }

        /// <summary> 
        /// Gets the underlying service handling the different event 
        /// on the connection. 
        /// </summary>
        public MessageService Service { get { return service; } }

        /// <summary>
        /// Sends a message to the server.
        /// </summary>
        public void Send(IMessage message)
        {
            Service.Connection.BeginSend(message.InnerMessage);
        }

        /// <summary>
        /// Sends a message to the server. 
        /// <summary>
        public void Send(byte[] data)
        {
            Service.Connection.BeginSend(data);
        }

        /// <summary>
        /// Opens up the connection to the server.
        /// </summary>
        /// <remark> 
        /// If an error happens during the connection, it won't be raised but
        /// you can trap it setting the Service.Failure handler. 
        /// The Close() method will be called automatically.
        /// </remark>
        /// <remark> 
        /// If the connection times out a SocketException will be thrown.
        /// </remark>
        public void Open()
        {
            WaitHandle[] wait = new WaitHandle[] { Service.ConnectEvent,
					     Service.ExceptionEvent };
            client.Start();

            // Let's wait for the connection to be signaled by
            // the connection handler of the service
            int signal = WaitHandle.WaitAny(wait, ConnectionTimeout, false);

            switch (signal)
            {
                case 0:
                    // connected
                    connected = true;
                    break;
                case 1:
                    // Exception, to trap it you should set the Service.Failure handler
                    Close();
                    break;
                default:
                    // Timeout
                    Close();
                    throw new SocketException(10060);
                    break;
            }
        }

        /// <summary>
        /// Closes the connection to the server.
        /// </summary>
        public void Close()
        {
            if (client != null)
            {
                connected = false;
                client.Stop();
                client.Dispose();
                client = null;
            }
        }
    }
}