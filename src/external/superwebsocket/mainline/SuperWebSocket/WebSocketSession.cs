﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperWebSocket.Protocol;
using SuperWebSocket.SubProtocol;

namespace SuperWebSocket
{
    public interface IWebSocketSession : IAppSession
    {
        string Method { get; set; }
        string Host { get; }
        string Path { get; set; }
        string HttpVersion { get; set; }
        string SecWebSocketVersion { get; }
        string Origin { get; }
        string UriScheme { get; }
        void SendResponse(string message);
        void SendResponse(byte[] data);
        IWebSocketServer AppServer { get; }
        IProtocolProcessor ProtocolProcessor { get; set; }
        string GetAvailableSubProtocol(string protocol);
        void EnqueueSend(IList<ArraySegment<byte>> data);
        void EnqueueSend(ArraySegment<byte> data);
    }

    public class WebSocketSession : WebSocketSession<WebSocketSession>
    {
        public new WebSocketServer AppServer
        {
            get { return (WebSocketServer)base.AppServer; }
        }
    }

    public class WebSocketSession<TWebSocketSession> : AppSession<TWebSocketSession, IWebSocketFragment>, IWebSocketSession
        where TWebSocketSession : WebSocketSession<TWebSocketSession>, new()
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public string HttpVersion { get; set; }
        public string Host { get { return this.Items.GetValue<string>(WebSocketConstant.Host, string.Empty); } }
        public string Origin { get { return this.Items.GetValue<string>(WebSocketConstant.Origin, string.Empty); } }
        public string Upgrade { get { return this.Items.GetValue<string>(WebSocketConstant.Upgrade, string.Empty); } }
        public string Connection { get { return this.Items.GetValue<string>(WebSocketConstant.Connection, string.Empty); } }
        public string SecWebSocketVersion { get { return this.Items.GetValue<string>(WebSocketConstant.SecWebSocketVersion, string.Empty); } }
        public string SecWebSocketProtocol { get { return this.Items.GetValue<string>(WebSocketConstant.SecWebSocketProtocol, string.Empty); } }

        private Queue<ArraySegment<byte>> m_SendingQueue = new Queue<ArraySegment<byte>>();

        private volatile bool m_InSending = false;

        internal DateTime StartClosingHandshakeTime { get; private set; }

        internal List<WebSocketDataFrame> Frames { get; private set; }

        /// <summary>
        /// Gets or sets the current token. It's only usefull when a command is executing
        /// </summary>
        /// <value>
        /// The current token.
        /// </value>
        public string CurrentToken { get; internal set; }

        public new WebSocketServer<TWebSocketSession> AppServer
        {
            get { return (WebSocketServer<TWebSocketSession>)base.AppServer; }
        }

        IWebSocketServer IWebSocketSession.AppServer
        {
            get { return (IWebSocketServer)base.AppServer; }
        }

        protected override void OnInit()
        {
            Frames = new List<WebSocketDataFrame>();
            base.OnInit();
        }

        string IWebSocketSession.GetAvailableSubProtocol(string protocol)
        {
            if (string.IsNullOrEmpty(protocol))
            {
                SubProtocol = AppServer.DefaultSubProtocol;
                return string.Empty;
            }

            var arrNames = protocol.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach(var name in arrNames)
            {
                var subProtocol = AppServer.GetSubProtocol(name);

                if(subProtocol != null)
                {
                    SubProtocol = subProtocol;
                    return name;
                }
            }

            return string.Empty;
        }

        public string UriScheme
        {
            get { return AppServer.UriScheme; }
        }

        public ISubProtocol<TWebSocketSession> SubProtocol { get; private set; }

        private bool m_Handshaked = false;

        internal bool Handshaked
        {
            get { return m_Handshaked; }
            set
            {
                m_Handshaked = value;
                if (m_Handshaked)
                {
                    SetCookie();
                    OnHandShaked();
                }
            }
        }

        public bool InClosing { get; private set; }

        private void SetCookie()
        {
            string cookieValue = this.Items.GetValue<string>(WebSocketConstant.Cookie, string.Empty);

            var cookies = new StringDictionary();

            if (!string.IsNullOrEmpty(cookieValue))
            {
                string[] pairs = cookieValue.Split(';');

                int pos;
                string key, value;

                foreach (var p in pairs)
                {
                    pos = p.IndexOf('=');
                    if (pos > 0)
                    {
                        key = p.Substring(0, pos).Trim();
                        pos += 1;
                        if (pos < p.Length)
                            value = p.Substring(pos).Trim();
                        else
                            value = string.Empty;

                        cookies[key] = Uri.UnescapeDataString(value);
                    }
                }
            }

            this.Cookies = cookies;
        }

        protected virtual void OnHandShaked()
        {

        }

        public StringDictionary Cookies { get; private set; }

        void IWebSocketSession.EnqueueSend(IList<ArraySegment<byte>> data)
        {
            lock (m_SendingQueue)
            {
                for (var i = 0; i < data.Count; i++)
                {
                    m_SendingQueue.Enqueue(data[i]);
                }
            }

            DequeueSend();
        }

        void IWebSocketSession.EnqueueSend(ArraySegment<byte> data)
        {
            lock (m_SendingQueue)
            {
                m_SendingQueue.Enqueue(data);
            }

            DequeueSend();
        }

        private void DequeueSend()
        {
            if (m_InSending)
                return;

            m_InSending = true;

            while (true)
            {
                if (Status != SessionStatus.Healthy)
                    break;

                ArraySegment<byte> segment;

                lock (m_SendingQueue)
                {
                    if (m_SendingQueue.Count <= 0)
                        break;

                    segment = m_SendingQueue.Dequeue();
                }

                SocketSession.SendResponse(segment.Array, segment.Offset, segment.Count);
            }

            m_InSending = false;
        }

        public override void SendResponse(string message)
        {
            ProtocolProcessor.SendMessage(this, message);
        }

        public override void SendResponse(string message, params object[] paramValues)
        {
            ProtocolProcessor.SendMessage(this, string.Format(message, paramValues));
        }

        public new void SendResponse(byte[] data)
        {
            if (!ProtocolProcessor.CanSendBinaryData)
            {
                Logger.LogError("The websocket of this version cannot used for sending binary data!");
                return;
            }

            ProtocolProcessor.SendData(this, data, 0, data.Length);
        }

        public void SendResponseAsync(string message)
        {
            Async.Run((s) => SendResponse((string)s), message);
        }

        public void SendResponseAsync(string message, params object[] paramValues)
        {
            SendResponseAsync(string.Format(message, paramValues));
        }

        public void CloseWithHandshake(string reasonText)
        {
            this.CloseWithHandshake(ProtocolProcessor.CloseStatusClode.NormalClosure, reasonText);
        }

        public void CloseWithHandshake(int statusCode, string reasonText)
        {
            if (!InClosing)
                InClosing = true;

            ProtocolProcessor.SendCloseHandshake(this, statusCode, reasonText);

            StartClosingHandshakeTime = DateTime.Now;
            AppServer.PushToCloseHandshakeQueue(this);
        }

        public void SendCloseHandshakeResponse(int statusCode)
        {
            if (!InClosing)
                InClosing = true;

            ProtocolProcessor.SendCloseHandshake(this, statusCode, string.Empty);
        }

        public override void Close(CloseReason reason)
        {
            if (reason == CloseReason.TimeOut && ProtocolProcessor != null)
            {
                CloseWithHandshake(ProtocolProcessor.CloseStatusClode.NormalClosure, "Session timeOut");
                return;
            }

            base.Close(reason);
        }

        public IProtocolProcessor ProtocolProcessor { get; set; }

        internal protected virtual void HandleUnknownCommand(SubRequestInfo requestInfo)
        {

        }

        public override void HandleUnknownCommand(IWebSocketFragment cmdInfo)
        {
            base.Close();
        }

        public override void HandleExceptionalError(Exception e)
        {
            Logger.LogError(e);
            this.Close();
        }
    }
}
