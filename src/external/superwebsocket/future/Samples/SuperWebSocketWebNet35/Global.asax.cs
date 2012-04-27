﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;
using SuperSocket.SocketEngine.Configuration;
using SuperWebSocket;
using SuperSocket.Common.Logging;

namespace SuperWebSocketWeb
{
    public class Global : System.Web.HttpApplication
    {
        private List<WebSocketSession> m_Sessions = new List<WebSocketSession>();
        private List<WebSocketSession> m_SecureSessions = new List<WebSocketSession>();
        private object m_SessionSyncRoot = new object();
        private object m_SecureSessionSyncRoot = new object();
        private Timer m_SecureSocketPushTimer;
        private IBootstrap m_Bootstrap;

        void Application_Start(object sender, EventArgs e)
        {
            LogFactoryProvider.Initialize();
            StartSuperWebSocketByConfig();
            //StartSuperWebSocketByProgramming();
            var ts = new TimeSpan(0, 0, 5);
            m_SecureSocketPushTimer = new Timer(OnSecureSocketPushTimerCallback, new object(), ts, ts);
        }

        void OnSecureSocketPushTimerCallback(object state)
        {
            lock (m_SecureSessionSyncRoot)
            {
                m_SecureSessions.ForEach(s => s.SendResponseAsync("Push data from SecureWebSocket. Current Time: " + DateTime.Now));
            }
        }

        void StartSuperWebSocketByConfig()
        {
            m_Bootstrap = new DefaultBootstrap();

            var serverConfig = ConfigurationManager.GetSection("socketServer") as SocketServiceConfig;
            if (!m_Bootstrap.Initialize(serverConfig))
                return;

            var socketServer = m_Bootstrap.AppServers.FirstOrDefault(s => s.Name.Equals("SuperWebSocket")) as WebSocketServer;
            var secureSocketServer = m_Bootstrap.AppServers.FirstOrDefault(s => s.Name.Equals("SecureSuperWebSocket")) as WebSocketServer;

            Application["WebSocketPort"] = socketServer.Config.Port;
            Application["SecureWebSocketPort"] = secureSocketServer.Config.Port;

            socketServer.NewMessageReceived += new SessionEventHandler<WebSocketSession, string>(socketServer_NewMessageReceived);
            socketServer.NewSessionConnected += socketServer_NewSessionConnected;
            socketServer.SessionClosed += socketServer_SessionClosed;

            secureSocketServer.NewSessionConnected += secureSocketServer_NewSessionConnected;
            secureSocketServer.SessionClosed += secureSocketServer_SessionClosed;

            m_Bootstrap.Start();
        }

        void socketServer_NewMessageReceived(WebSocketSession session, string e)
        {
            SendToAll(session.Cookies["name"] + ": " + e);
        }

        void secureSocketServer_SessionClosed(WebSocketSession session, CloseReason reason)
        {
            lock (m_SecureSessionSyncRoot)
            {
                m_SecureSessions.Remove(session);
            }
        }

        void secureSocketServer_NewSessionConnected(WebSocketSession session)
        {
            lock (m_SecureSessionSyncRoot)
            {
                m_SecureSessions.Add(session);
            }
        }

        void StartSuperWebSocketByProgramming()
        {
            m_Bootstrap = new DefaultBootstrap();

            var socketServer = new WebSocketServer();

            socketServer.NewMessageReceived += new SessionEventHandler<WebSocketSession, string>(socketServer_NewMessageReceived);
            socketServer.NewSessionConnected += socketServer_NewSessionConnected;
            socketServer.SessionClosed += socketServer_SessionClosed;

            var secureSocketServer = new WebSocketServer();

            m_Bootstrap.Initialize(new RootConfig(), new IAppServer[] { socketServer, secureSocketServer },
                new IServerConfig[]
                {
                    new ServerConfig
                    {
                        Name = "SuperWebSocket",
                        Ip = "Any",
                        Port = 2011,
                        Mode = SocketMode.Tcp
                    },
                    new ServerConfig
                    {
                        Name = "SecureSuperWebSocket",
                        Ip = "Any",
                        Port = 2012,
                        Mode = SocketMode.Tcp,
                        Security = "tls",
                        Certificate = new SuperSocket.SocketBase.Config.CertificateConfig
                        {
                            FilePath = Server.MapPath("~/localhost.pfx"),
                            Password = "supersocket",
                            IsEnabled = true
                        }
                    }
                });

            secureSocketServer.NewSessionConnected += secureSocketServer_NewSessionConnected;
            secureSocketServer.SessionClosed += secureSocketServer_SessionClosed;

            Application["WebSocketPort"] = socketServer.Config.Port;
            Application["SecureWebSocketPort"] = secureSocketServer.Config.Port;

            m_Bootstrap.Start();
        }

        void socketServer_NewSessionConnected(WebSocketSession session)
        {
            lock (m_SessionSyncRoot)
                m_Sessions.Add(session);

            SendToAll("System: " + session.Cookies["name"] + " connected");
        }

        void socketServer_SessionClosed(WebSocketSession session, CloseReason reason)
        {
            lock (m_SessionSyncRoot)
                m_Sessions.Remove(session);

            if (reason == CloseReason.ServerShutdown)
                return;

            SendToAll("System: " + session.Cookies["name"] + " disconnected");
        }

        void SendToAll(string message)
        {
            lock (m_SessionSyncRoot)
            {
                foreach (var s in m_Sessions)
                {
                    //s.SendResponse(message);
                    s.SendResponseAsync(message);
                }
            }
        }

        void Application_End(object sender, EventArgs e)
        {
            m_SecureSocketPushTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_SecureSocketPushTimer.Dispose();

            if (m_Bootstrap != null)
                m_Bootstrap.Stop();
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
