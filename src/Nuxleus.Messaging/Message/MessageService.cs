//
// MessageService.cs: Defines the service class used by 
// the different clients or servers to perform tasks upon socket events.
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using ALAZ.SystemEx.NetEx.SocketsEx;
using ALAZ.SystemEx.ThreadingEx;

namespace Nuxleus.Messaging
{
    public delegate void MessageEventHandler(ISocketConnection sender, IMessage m);
    public delegate void QueueEventHandler(ISocketConnection sender);
    public delegate void QueueFailureEventHandler(ISocketConnection sender, Exception ex);

    /// <summary>
    /// Each server or client instance has an instance of this class.
    /// Your application simply binds event handlers to the events
    /// declared in this class.
    /// Your application can also wait on the thread events.
    /// </summary>
    public class MessageService : BaseSocketService
    {
        private ISocketConnection connection = null;

        public AutoResetEvent ReceivedEvent = new AutoResetEvent(false);
        public AutoResetEvent ExceptionEvent = new AutoResetEvent(false);
        public AutoResetEvent SentEvent = new AutoResetEvent(false);
        public AutoResetEvent ConnectEvent = new AutoResetEvent(false);
        public ManualResetEvent DisconnectEvent = new ManualResetEvent(false);

        public MessageService() { }

        public event MessageEventHandler Received = null;

        public event QueueEventHandler Sent = null;
        public event QueueEventHandler Connected = null;
        public event QueueEventHandler Disconnected = null;
        public event QueueFailureEventHandler Failure = null;

        /// <summary>
        /// Gets the connection socket. This is only returns an
        /// actual instance when used in client context.
        /// </summary>
        public ISocketConnection Connection
        {
            get { return connection; }
            set { connection = value; }
        }

        public override void OnConnected(ConnectionEventArgs e)
        {
            connection = e.Connection;
            connection.BeginReceive();
            if (Connected != null)
            {
                Connected(e.Connection);
            }
            ConnectEvent.Set();
        }

        public override void OnSent(MessageEventArgs e)
        {
            if (Sent != null)
            {
                Sent(e.Connection);
            }
            SentEvent.Set();
        }

        public override void OnReceived(MessageEventArgs e)
        {
            IMessage m = new Message();
            m.InnerMessage = new byte[e.Buffer.Length];
            Array.Copy(e.Buffer, m.InnerMessage, e.Buffer.Length);
            if (Received != null)
            {
                Received(e.Connection, m);
            }
            ReceivedEvent.Set();
            e.Connection.BeginReceive();
        }

        public override void OnDisconnected(ConnectionEventArgs e)
        {
            if (Disconnected != null)
            {
                Disconnected(e.Connection);
            }
            DisconnectEvent.Set();
        }

        public override void OnException(ExceptionEventArgs e)
        {
            if (Failure != null)
            {
                Failure(e.Connection, e.Exception);
            }
            ExceptionEvent.Set();
        }
    }
}