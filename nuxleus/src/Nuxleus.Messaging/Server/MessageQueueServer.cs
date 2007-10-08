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
using System.Text;

using ALAZ.SystemEx.NetEx.SocketsEx;
using ALAZ.SystemEx.ThreadingEx;

namespace Nuxleus.Messaging {
  public class MessageQueueServer {
    private EntityQueueService service = null;
    private SocketServer server = null;

    public MessageQueueServer(int port) {
      service = new EntityQueueService();
      server = new SocketServer(service);
      server.Delimiter = Encoding.ASCII.GetBytes("\n");
      server.DelimiterType = DelimiterType.dtMessageTailExcludeOnReceive;
            
      server.SocketBufferSize = 4096;
      server.MessageBufferSize = 4096 * 4;
    
      server.IdleCheckInterval = 60000;
      server.IdleTimeOutValue = 120000;

      SocketListener listener = server.AddListener("Commom Port - 8090", new IPEndPoint(IPAddress.Any, port));

      listener.AcceptThreads = 3;
      listener.BackLog = 50;
    } 

    public void Start() {
      server.Start();
    }

    public void Stop() {
      if(server != null) {
	server.Stop();
	server.Dispose();
	server = null;
      }
    }
  }
}