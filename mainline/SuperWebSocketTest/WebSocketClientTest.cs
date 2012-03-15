﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;
using SuperWebSocket;
using SuperWebSocket.SubProtocol;
using WebSocket4Net;

namespace SuperWebSocketTest
{
    [TestFixture]
    public class WebSocketClientTestHybi00 : WebSocketClientTest
    {
        public WebSocketClientTestHybi00()
            : base(WebSocketVersion.DraftHybi00)
        {

        }

        [Test]
        public override void SendDataTest()
        {
            
        }
    }

    [TestFixture]
    public class WebSocketClientTestHybi10 : WebSocketClientTest
    {
        public WebSocketClientTestHybi10()
            : base(WebSocketVersion.DraftHybi10)
        {

        }
    }

    [TestFixture]
    public class WebSocketClientTestRFC6455 : WebSocketClientTest
    {
        public WebSocketClientTestRFC6455()
            : base(WebSocketVersion.Rfc6455)
        {

        }
    }

    public abstract class WebSocketClientTest : WebSocketTestBase
    {
        private readonly WebSocketVersion m_Version;

        public WebSocketClientTest(WebSocketVersion version)
        {
            m_Version = version;
        }

        [TestFixtureSetUp]
        public override void Setup()
        {
            LogUtil.Setup(new ConsoleLogger());

            WebSocketServer = new WebSocketServer(new BasicSubProtocol());
            WebSocketServer.NewDataReceived += new SessionEventHandler<WebSocketSession, byte[]>(WebSocketServer_NewDataReceived);
            WebSocketServer.Setup(new RootConfig(), new ServerConfig
            {
                Port = 2012,
                Ip = "Any",
                MaxConnectionNumber = 100,
                Mode = SocketMode.Async,
                Name = "SuperWebSocket Server"
            }, SocketServerFactory.Instance);
        }

        [Test]
        public void ConnectionTest()
        {
            var webSocketClient = CreateClient(m_Version);

            Assert.AreEqual(WebSocketState.Open, webSocketClient.State);

            webSocketClient.Close();

            if (!CloseEvent.WaitOne(1000))
                Assert.Fail("Failed to close session ontime");

            Assert.AreEqual(WebSocketState.Closed, webSocketClient.State);
        }

        [Test, Repeat(10)]
        public void SendMessageTest()
        {
            var webSocketClient = CreateClient(m_Version);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 10; i++)
            {
                sb.Append(Guid.NewGuid().ToString());
            }

            string messageSource = sb.ToString();

            Random rd = new Random();

            for (int i = 0; i < 100; i++)
            {
                int startPos = rd.Next(0, messageSource.Length - 2);
                int endPos = rd.Next(startPos + 1, messageSource.Length - 1);

                string message = messageSource.Substring(startPos, endPos - startPos);

                webSocketClient.Send("ECHO " + message);

                Console.WriteLine("Client:" + message);

                if (!MessageReceiveEvent.WaitOne(1000))
                    Assert.Fail("Cannot get response in time!");

                Assert.AreEqual(message, CurrentMessage);
            }

            webSocketClient.Close();

            if (!CloseEvent.WaitOne(1000))
                Assert.Fail("Failed to close session ontime");
        }
  
        [Test, Repeat(10)]
        public virtual void SendDataTest()
        {
            var webSocketClient = CreateClient(m_Version);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 10; i++)
            {
                sb.Append(Guid.NewGuid().ToString());
            }

            string messageSource = sb.ToString();

            Random rd = new Random();

            for (int i = 0; i < 100; i++)
            {
                int startPos = rd.Next(0, messageSource.Length - 2);
                int endPos = rd.Next(startPos + 1, messageSource.Length - 1);

                string message = messageSource.Substring(startPos, endPos - startPos);

                Console.WriteLine("Client:" + message);
                var data = Encoding.UTF8.GetBytes(message);
                webSocketClient.Send(data, 0, data.Length);

                if (!this.DataReceiveEvent.WaitOne(1000))
                    Assert.Fail("Cannot get response in time!");

                Assert.AreEqual(message, Encoding.UTF8.GetString(CurrentData));
            }

            webSocketClient.Close();

            if (!CloseEvent.WaitOne(1000))
                Assert.Fail("Failed to close session ontime");
        }
        
        [Test, Repeat(10)]
        public void CloseWebSocketTest()
        {
            var webSocketClient = CreateClient(m_Version);

            Assert.AreEqual(WebSocketState.Open, webSocketClient.State);

            webSocketClient.Send("QUIT");

            if (!CloseEvent.WaitOne())
                Assert.Fail("Failed to close session ontime");

            Assert.AreEqual(WebSocketState.Closed, webSocketClient.State);
        }
    }
}
