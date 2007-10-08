//
// IMessageQueue.cs: Bucker message queue
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, Sylvain Hellegouarch
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

namespace Nuxleus.Messaging {
  public class MessageQueue : IMessageQueue {
    private string id = null;

    private int connectTimeout = 30000; //ms
    private bool connected = false;
    
    private SocketClient client = null;
    private EntityQueueService service = null;

    private MessageQueue(string ip, int port) {
      service = new EntityQueueService();
      client = new SocketClient(service);
      client.AddConnector("MessageQueue", 
			  new IPEndPoint(IPAddress.Parse(ip), port));
      client.Delimiter = Encoding.ASCII.GetBytes("\n");
      client.DelimiterType = DelimiterType.dtMessageTailExcludeOnReceive;
      
      client.SocketBufferSize = 4096;
      client.MessageBufferSize = 4096 * 4;
    }

    public string Id { get { return id; } set { id = value; } }

    public int ConnectionTimeout { 
      get { return connectTimeout; }
      set { connectTimeout = value; }
    }

    public bool Connected { get { return connected; } }
    public EntityQueueService Service { get { return service; } }

    public void Send(IMessage message) {
      Service.Connection.BeginSend(message.Serialize());
    }

    public void Send(object payload) {
      Message m = new Message();
      m.Payload = payload;
	
      Service.Connection.BeginSend(m.Serialize());
    }

    public static IMessageQueue Create(string name, string ip, int port) {
      MessageQueue mq = new MessageQueue(ip, port);
      mq.Id = name;

      return mq;
    }

    public void Open() {
      WaitHandle[] wait = new WaitHandle[] { Service.ConnectEvent,
					     Service.ExceptionEvent };
      client.Start();
      
      // Let's wait for the connection to be signaled by
      // the connection handler of the service
      int signal = WaitHandle.WaitAny(wait, ConnectionTimeout, false);

      switch(signal) {
      case 0:
	// connected
	connected = true;
	break;
      case 1:
	// Exception
	Close();
	break;
      default:
	// Timeout
	Close();
	break;
      }
    }

    public void Close() {
      if(client != null) {
	connected = false;
	client.Stop();
	client.Dispose();
	client = null;
      }
    }
  }
}