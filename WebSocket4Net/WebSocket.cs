﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using SuperSocket.ClientEngine;
using SuperSocket.ClientEngine.Protocol;
using WebSocket4Net.Protocol;

namespace WebSocket4Net
{
    public partial class WebSocket
    {
        internal TcpClientSession Client { get; private set; }

        public WebSocketVersion Version { get; private set; }

        public DateTime LastActiveTime { get; internal set; }

        protected const string UserAgentKey = "UserAgent";

        internal IProtocolProcessor ProtocolProcessor { get; private set; }

        public bool SupportBinary
        {
            get { return ProtocolProcessor.SupportBinary; }
        }

        internal Uri TargetUri { get; private set; }

        internal string SubProtocol { get; private set; }

        internal IDictionary<string, object> Items { get; private set; }

        internal List<KeyValuePair<string, string>> Cookies { get; private set; }

        internal List<KeyValuePair<string, string>> CustomHeaderItems { get; private set; }

        public WebSocketState State { get; private set; }

        public bool Handshaked { get; private set; }

        public IProxyConnector Proxy { get; set; }

        protected IClientCommandReader<WebSocketCommandInfo> CommandReader { get; private set; }

        private Dictionary<string, ICommand<WebSocket, WebSocketCommandInfo>> m_CommandDict
            = new Dictionary<string, ICommand<WebSocket, WebSocketCommandInfo>>(StringComparer.OrdinalIgnoreCase);

        private static ProtocolProcessorFactory m_ProtocolProcessorFactory;

        internal bool NotSpecifiedVersion { get; private set; }

        private Timer m_PingTimer;

        private const int m_PingInterval = 1000 * 60;

        internal string LastPongResponse { get; set; }

        private string m_LastPingRequest;

        private const string m_UriScheme = "ws";

        private const string m_UriPrefix = m_UriScheme + "://";

        private const string m_SecureUriScheme = "wss";

        private const string m_SecureUriPrefix = m_SecureUriScheme + "://";

        internal string HandshakeHost { get; private set; }

        internal string Origin { get; private set; }

        static WebSocket()
        {
            m_ProtocolProcessorFactory = new ProtocolProcessorFactory(new Rfc6455Processor(), new DraftHybi10Processor(), new DraftHybi00Processor());
        }

        private EndPoint ResolveUri(string uri)
        {
            TargetUri = new Uri(uri);

            IPAddress ipAddress;

            EndPoint remoteEndPoint;

            if (IPAddress.TryParse(TargetUri.Host, out ipAddress))
                remoteEndPoint = new IPEndPoint(ipAddress, TargetUri.Port);
            else
            {
                if (TargetUri.Port <= 0)
                    remoteEndPoint = new DnsEndPoint(TargetUri.Host, 80);
                else
                    remoteEndPoint = new DnsEndPoint(TargetUri.Host, TargetUri.Port);
            }

            return remoteEndPoint;
        }

        TcpClientSession CreateClient(string uri)
        {
            var targetEndPoint = ResolveUri(uri);

            if (TargetUri.Port == 80)
                HandshakeHost = TargetUri.Host;
            else
                HandshakeHost = TargetUri.Host + ":" + TargetUri.Port;

            return new AsyncTcpSession(targetEndPoint);
        }

#if !SILVERLIGHT

        TcpClientSession CreateSecureClient(string uri)
        {
            int hostPos = uri.IndexOf('/', m_SecureUriPrefix.Length);

            if (hostPos < 0)//wss://localhost
            {
                uri = uri + ":443/";
            }
            else if (hostPos == m_SecureUriPrefix.Length)//wss:///
            {
                throw new ArgumentException("Invalid uri", "uri");
            }
            else//wss://xxx/xxx
            {
                int colonPos = uri.IndexOf(':', m_SecureUriPrefix.Length, hostPos - m_SecureUriPrefix.Length);

                if (colonPos < 0)
                {
                    uri = uri.Substring(0, hostPos) + ":443" + uri.Substring(hostPos);
                }
            }

            var targetEndPoint = ResolveUri(uri);

            if (TargetUri.Port == 443)
                HandshakeHost = TargetUri.Host;
            else
                HandshakeHost = TargetUri.Host + ":" + TargetUri.Port;

            return new SslStreamTcpSession(targetEndPoint);
        }

#endif

        private void Initialize(string uri, string subProtocol, List<KeyValuePair<string, string>> cookies, List<KeyValuePair<string, string>> customHeaderItems, string userAgent, string origin, WebSocketVersion version)
        {
            if (version == WebSocketVersion.None)
            {
                NotSpecifiedVersion = true;
                version = WebSocketVersion.Rfc6455;
            }

            Version = version;
            ProtocolProcessor = GetProtocolProcessor(version);
            CommandReader = ProtocolProcessor.CreateHandshakeReader(this);

            Cookies = cookies;

            Origin = origin;

            if (!string.IsNullOrEmpty(userAgent))
            {
                if (customHeaderItems == null)
                    customHeaderItems = new List<KeyValuePair<string, string>>();

                customHeaderItems.Add(new KeyValuePair<string, string>(UserAgentKey, userAgent));
            }

            if (customHeaderItems != null && customHeaderItems.Count > 0)
                CustomHeaderItems = customHeaderItems;

            var handshakeCmd = new Command.Handshake();
            m_CommandDict.Add(handshakeCmd.Name, handshakeCmd);
            var textCmd = new Command.Text();
            m_CommandDict.Add(textCmd.Name, textCmd);
            var dataCmd = new Command.Binary();
            m_CommandDict.Add(dataCmd.Name, dataCmd);
            var closeCmd = new Command.Close();
            m_CommandDict.Add(closeCmd.Name, closeCmd);
            var pingCmd = new Command.Ping();
            m_CommandDict.Add(pingCmd.Name, pingCmd);
            var pongCmd = new Command.Pong();
            m_CommandDict.Add(pongCmd.Name, pongCmd);
            var badRequestCmd = new Command.BadRequest();
            m_CommandDict.Add(badRequestCmd.Name, badRequestCmd);
            
            State = WebSocketState.None;

            SubProtocol = subProtocol;

            Items = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            TcpClientSession client;

            if (uri.StartsWith(m_UriPrefix, StringComparison.OrdinalIgnoreCase))
            {
                client = CreateClient(uri);
            }
            else if (uri.StartsWith(m_SecureUriPrefix, StringComparison.OrdinalIgnoreCase))
            {
#if SILVERLIGHT
                throw new ArgumentException("WebSocket4Net (Silverlight/WindowsPhone) cannot support wss yet.", "uri");
#else
                client = CreateSecureClient(uri);
#endif
            }
            else
            {
                throw new ArgumentException("Invalid uri", "uri");
            }

            client.Connected += new EventHandler(client_Connected);
            client.Closed += new EventHandler(client_Closed);
            client.Error += new EventHandler<ErrorEventArgs>(client_Error);
            client.DataReceived += new EventHandler<DataEventArgs>(client_DataReceived);

            Client = client;
        }

        void client_DataReceived(object sender, DataEventArgs e)
        {
            OnDataReceived(e.Data, e.Offset, e.Length);
        }

        void client_Error(object sender, ErrorEventArgs e)
        {
            OnError(e);
        }

        void client_Closed(object sender, EventArgs e)
        {
            OnClosed();
        }

        void client_Connected(object sender, EventArgs e)
        {
            OnConnected();
        }

        internal bool GetAvailableProcessor(int[] availableVersions)
        {
            var processor = m_ProtocolProcessorFactory.GetPreferedProcessorFromAvialable(availableVersions);

            if (processor == null)
                return false;

            this.ProtocolProcessor = processor;
            return true;
        }

        public int ReceiveBufferSize
        {
            get { return Client.ReceiveBufferSize; }
            set { Client.ReceiveBufferSize = value; }
        }

        public void Open()
        {
            State = WebSocketState.Connecting;

            if (Proxy != null)
                Client.Proxy = Proxy;

#if SILVERLIGHT
    #if !WINDOWS_PHONE
            Client.ClientAccessPolicyProtocol = ClientAccessPolicyProtocol;
    #endif
#endif
            Client.Connect();
        }

        private static IProtocolProcessor GetProtocolProcessor(WebSocketVersion version)
        {
            var processor = m_ProtocolProcessorFactory.GetProcessorByVersion(version);

            if (processor == null)
                throw new ArgumentException("Invalid websocket version");

            return processor;
        }

        void OnConnected()
        {
            ProtocolProcessor.SendHandshake(this);
        }

        protected internal virtual void OnHandshaked()
        {
            State = WebSocketState.Open;

            Handshaked = true;

            if (m_Opened == null)
                return;

            m_Opened(this, EventArgs.Empty);

            if (ProtocolProcessor.SupportPingPong)
                m_PingTimer = new Timer(OnPingTimerCallback, ProtocolProcessor, m_PingInterval, m_PingInterval);
        }

        private void OnPingTimerCallback(object state)
        {
            if (!string.IsNullOrEmpty(m_LastPingRequest) && !m_LastPingRequest.Equals(LastPongResponse))
            {
                //have not got last response
                return;
            }

            var protocolProcessor = state as IProtocolProcessor;
            m_LastPingRequest = DateTime.Now.ToString();
            protocolProcessor.SendPing(this, m_LastPingRequest);
        }

        private EventHandler m_Opened;

        public event EventHandler Opened
        {
            add { m_Opened += value; }
            remove { m_Opened -= value; }
        }

        private EventHandler<MessageReceivedEventArgs> m_MessageReceived;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived
        {
            add { m_MessageReceived += value; }
            remove { m_MessageReceived -= value; }
        }

        internal void FireMessageReceived(string message)
        {
            if (m_MessageReceived == null)
                return;

            m_MessageReceived(this, new MessageReceivedEventArgs(message));
        }

        private EventHandler<DataReceivedEventArgs> m_DataReceived;

        public event EventHandler<DataReceivedEventArgs> DataReceived
        {
            add { m_DataReceived += value; }
            remove { m_DataReceived -= value; }
        }

        internal void FireDataReceived(byte[] data)
        {
            if (m_DataReceived == null)
                return;

            m_DataReceived(this, new DataReceivedEventArgs(data));
        }

        private const string m_NotOpenSendingMessage = "You must send data by websocket after websocket is opened!";

        private bool EnsureWebSocketOpen()
        {
            if (!Handshaked)
            {
                OnError(new Exception(m_NotOpenSendingMessage));
                return false;
            }

            return true;
        }

        public void Send(string message)
        {
            if (!EnsureWebSocketOpen())
                return;

            ProtocolProcessor.SendMessage(this, message);
        }

        public void Send(byte[] data, int offset, int length)
        {
            if (!EnsureWebSocketOpen())
                return;

            ProtocolProcessor.SendData(this, data, offset, length);
        }

        private void OnClosed()
        {
            var fireBaseClose = false;

            if (State == WebSocketState.Closing || State == WebSocketState.Open)
                fireBaseClose = true;

            State = WebSocketState.Closed;

            if (fireBaseClose)
                FireClosed();
        }

        public void Close()
        {
            //The websocket never be opened
            if (State == WebSocketState.None)
            {
                State = WebSocketState.Closed;
                OnClosed();
                return;
            }

            Close(string.Empty);
        }

        public void Close(string reason)
        {
            Close(ProtocolProcessor.CloseStatusCode.NormalClosure, reason);
        }

        public void Close(int statusCode, string reason)
        {
            State = WebSocketState.Closing;
            ProtocolProcessor.SendCloseHandshake(this, statusCode, reason);
        }

        internal void CloseWithouHandshake()
        {
            Client.Close();
        }

        protected void ExecuteCommand(WebSocketCommandInfo commandInfo)
        {
            ICommand<WebSocket, WebSocketCommandInfo> command;

            if (m_CommandDict.TryGetValue(commandInfo.Key, out command))
            {
                command.ExecuteCommand(this, commandInfo);
            }
        }

        private void OnDataReceived(byte[] data, int offset, int length)
        {
            while (true)
            {
                int left;

                var commandInfo = CommandReader.GetCommandInfo(data, offset, length, out left);

                if (CommandReader.NextCommandReader != null)
                    CommandReader = CommandReader.NextCommandReader;

                if (commandInfo == null)
                    break;

                ExecuteCommand(commandInfo);

                if (left <= 0)
                    break;

                offset = offset + length - left;
                length = left;
            }
        }

        internal void FireError(Exception error)
        {
            OnError(error);
        }

        private EventHandler m_Closed;

        public event EventHandler Closed
        {
            add { m_Closed += value; }
            remove { m_Closed -= value; }
        }

        private void FireClosed()
        {
            if (m_PingTimer != null)
            {
                m_PingTimer.Change(Timeout.Infinite, Timeout.Infinite);
                m_PingTimer.Dispose();
                m_PingTimer = null;
            }

            var handler = m_Closed;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private EventHandler<ErrorEventArgs> m_Error;

        public event EventHandler<ErrorEventArgs> Error
        {
            add { m_Error += value; }
            remove { m_Error -= value; }
        }

        private void OnError(ErrorEventArgs e)
        {
            if (m_Error == null)
                return;

            m_Error(this, e);
        }

        private void OnError(Exception e)
        {
            OnError(new ErrorEventArgs(e));
        }
    }
}
