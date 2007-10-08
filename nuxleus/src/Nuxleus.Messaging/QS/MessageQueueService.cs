//
// IMessageQueue.cs: Bucker message queue
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

namespace Nuxleus.Messaging.QS {
  public delegate void MessageEventHandler(IMessage m);
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
      Console.WriteLine("Connected");
      if(Connected != null) {
	Connected(this);
      }
      ConnectEvent.Set();
    }

    public override void OnSent(MessageEventArgs e) {
      Console.WriteLine("Sent");
      if(Sent != null) {
	Sent(this);
      }
      SentEvent.Set();
    }

    public override void OnReceived(MessageEventArgs e) {
      Console.WriteLine("Received");
      IMessage m = new Message();
      m.Deserialize(e.Buffer);
      if(Received != null) {
	Received(m);
      }
      ReceivedEvent.Set();
      connection.BeginReceive();
    }

    public override void OnDisconnected(ConnectionEventArgs e) {
      Console.WriteLine("Disconnected");
      if(Disconnected != null) {
	Disconnected(this);
      }
      DisconnectEvent.Set();
    }

    public override void OnException(ExceptionEventArgs e) {
      Console.WriteLine(String.Format("Error {0}", e.Exception.ToString()));
      if(Failure != null) {
	Failure(this, e.Exception);
      }
      ExceptionEvent.Set();
    }
  }
}