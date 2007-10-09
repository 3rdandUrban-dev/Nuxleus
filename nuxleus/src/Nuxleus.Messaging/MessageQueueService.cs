//
// QueueService.cs: Defines the service class used by the different clients or servers
// to perform tasks upon socket events.
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using ALAZ.SystemEx.NetEx.SocketsEx;
using ALAZ.SystemEx.ThreadingEx;

namespace Nuxleus.Messaging {
  public delegate void MessageEventHandler(ISocketConnection sender, IMessage m);
  public delegate void QueueEventHandler(object sender);
  public delegate void QueueFailureEventHandler(object sender, Exception ex);

  public class MessageQueueService : BaseSocketService {
    private ISocketConnection connection = null;
    
    public AutoResetEvent ReceivedEvent = new AutoResetEvent(false);
    public AutoResetEvent ExceptionEvent = new AutoResetEvent(false);
    public AutoResetEvent SentEvent = new AutoResetEvent(false);
    public AutoResetEvent ConnectEvent = new AutoResetEvent(false);
    public ManualResetEvent DisconnectEvent = new ManualResetEvent(false);

    public MessageQueueService() {}

    public event MessageEventHandler Received = null;

    public event QueueEventHandler Sent = null;
    public event QueueEventHandler Connected = null;
    public event QueueEventHandler Disconnected = null;
    public event QueueFailureEventHandler Failure = null;

    public ISocketConnection Connection {
      get { return connection; }
    }
    
    public override void OnConnected(ConnectionEventArgs e) {
      connection = e.Connection;
      connection.BeginReceive();
      if(Connected != null) {
	Connected(this);
      }
      ConnectEvent.Set();
    }

    public override void OnSent(MessageEventArgs e) {
      if(Sent != null) {
	Sent(this);
      }
      SentEvent.Set();
    }

    public override void OnReceived(MessageEventArgs e) {
      IMessage m = new Message();
      m.InnerMessage = new byte[e.Buffer.Length];
      Array.Copy(e.Buffer, m.InnerMessage, e.Buffer.Length);
      if(Received != null) {
	Received(e.Connection, m);
      }
      ReceivedEvent.Set();
      connection.BeginReceive();
    }

    public override void OnDisconnected(ConnectionEventArgs e) {
      if(Disconnected != null) {
	Disconnected(this);
      }
      DisconnectEvent.Set();
    }

    public override void OnException(ExceptionEventArgs e) {
      if(Failure != null) {
	Failure(this, e.Exception);
      }
      ExceptionEvent.Set();
    }
  }
}