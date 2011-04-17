//
// MessageServer.cs: Base TCP/IP server 
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
using System.Text;

using ALAZ.SystemEx.NetEx.SocketsEx;
using ALAZ.SystemEx.ThreadingEx;

namespace Nuxleus.Messaging {
    public class MessageServer {
        private MessageService service = null;
        private SocketServer server = null;

        /// <summary>
        /// Use this constructeur to setup a server listening on a given port for
        /// data delimited as specified by the delimiter argument.
        /// </summary>
        public MessageServer ( int port, string delimiter ) {
            service = new MessageService();
            server = new SocketServer(CallbackThreadType.ctWorkerThread, service);
            server.Delimiter = Encoding.ASCII.GetBytes(delimiter);
            server.DelimiterType = DelimiterType.dtMessageTailExcludeOnReceive;

            server.SocketBufferSize = 4096;
            server.MessageBufferSize = 4096 * 4;

            server.IdleCheckInterval = 60000;
            server.IdleTimeOutValue = 120000;

            SocketListener listener = server.AddListener(String.Format("Nuxleus server: {0}", port),
                                 new IPEndPoint(IPAddress.Any, port));

            listener.AcceptThreads = 3;
            listener.BackLog = 50;
        }

        /// <summary>
        /// Gets the actual server object handling the connections 
        /// and dispatching on socket events.
        /// </summary>
        public SocketServer Server {
            get { return server; }
        }

        /// <summary> 
        /// Gets the underlying service handling the different event 
        /// on the connection. Attach your handlers to that service.
        /// </summary>
        public MessageService Service { get { return service; } }

        /// <summary>
        /// Starts up the server and listen on the port defined.
        /// </summary>
        public void Start () {
            server.Start();
        }

        /// <summary>
        /// Stops the server and disposes of all its resources.
        /// </summary>
        public void Stop () {
            if (server != null) {
                server.Stop();
                server.Dispose();
                server = null;
            }
        }
    }
}