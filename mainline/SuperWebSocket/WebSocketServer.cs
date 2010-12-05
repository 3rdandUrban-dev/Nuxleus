﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Config;
using SuperWebSocket.SubProtocol;

namespace SuperWebSocket
{
    public delegate void SessionEventHandler(WebSocketSession session);

    public delegate void SessionClosedEventHandler(WebSocketSession session, CloseReason reason);

    public class WebSocketServer : AppServer<WebSocketSession, WebSocketCommandInfo>
    {
        public WebSocketServer(WebSocketProtocol protocol)
            : base()
        {
            Protocol = protocol;
        }

        public WebSocketServer()
            : this(new WebSocketProtocol())
        {
 
        }

        private ISubProtocol m_SubProtocol;

        public new WebSocketProtocol Protocol
        {
            get
            {
                return (WebSocketProtocol)base.Protocol;
            }
            protected set
            {
                base.Protocol = value;
            }
        }

        public override bool Setup(IServerConfig config, ISocketServerFactory socketServerFactory, object protocol, string consoleBaseAddress, string assembly)
        {
            m_SubProtocol = Protocol.SubProtocol;

            string subProtocolValue = config.Parameters.GetValue("subProtocol");
            if (!string.IsNullOrEmpty(subProtocolValue))
            {
                ISubProtocol subProtocol;
                if (AssemblyUtil.TryCreateInstance<ISubProtocol>(subProtocolValue, out subProtocol))
                    m_SubProtocol = subProtocol;
            }

            return base.Setup(config, socketServerFactory, protocol, consoleBaseAddress, assembly);
        }

        private Dictionary<string, ISubCommand> m_SubProtocolCommandDict = new Dictionary<string, ISubCommand>(StringComparer.OrdinalIgnoreCase);

        private bool m_SubCommandsLoaded = false;

        private SessionEventHandler m_NewSessionConnected;

        public event SessionEventHandler NewSessionConnected
        {
            add { m_NewSessionConnected += value; }
            remove { m_NewSessionConnected -= value; }
        }

        private SessionClosedEventHandler m_SessionClosed;

        public event SessionClosedEventHandler SessionClosed
        {
            add { m_SessionClosed += value; }
            remove { m_SessionClosed -= value; }
        }

        private byte[] GetResponseSecurityKey(string secKey1, string secKey2, byte[] secKey3)
        {
            //Remove all symbols that are not numbers
            string k1 = Regex.Replace(secKey1, "[^0-9]", String.Empty);
            string k2 = Regex.Replace(secKey2, "[^0-9]", String.Empty);

            //Convert received string to 64 bit integer.
            Int64 intK1 = Int64.Parse(k1);
            Int64 intK2 = Int64.Parse(k2);

            //Dividing on number of spaces
            int k1Spaces = secKey1.Count(c => c == ' ');
            int k2Spaces = secKey2.Count(c => c == ' ');
            int k1FinalNum = (int)(intK1 / k1Spaces);
            int k2FinalNum = (int)(intK2 / k2Spaces);

            //Getting byte parts
            byte[] b1 = BitConverter.GetBytes(k1FinalNum).Reverse().ToArray();
            byte[] b2 = BitConverter.GetBytes(k2FinalNum).Reverse().ToArray();
            //byte[] b3 = Encoding.UTF8.GetBytes(secKey3);
            byte[] b3 = secKey3;

            //Concatenating everything into 1 byte array for hashing.
            List<byte> bChallenge = new List<byte>();
            bChallenge.AddRange(b1);
            bChallenge.AddRange(b2);
            bChallenge.AddRange(b3);

            //Hash and return
            byte[] hash = MD5.Create().ComputeHash(bChallenge.ToArray());
            return hash;
        }

        private void SetCookie(WebSocketSession session)
        {
            string cookieValue = session.Context[WebSocketConstant.Cookie];

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
                        cookies.Add(key, value);
                    }
                }                
            }

            session.Cookies = cookies;
        }

        private void ProcessHandshakeRequest(WebSocketSession session)
        {
            SetCookie(session);

            var secKey1 = session.Context.SecWebSocketKey1;
            var secKey2 = session.Context.SecWebSocketKey2;
            var secKey3 = session.Context.SecWebSocketKey3;

            var responseBuilder = new StringBuilder();

            //Common for all websockets editions (v.75 & v.76)
            responseBuilder.AppendLine("HTTP/1.1 101 Web Socket Protocol Handshake");
            responseBuilder.AppendLine("Upgrade: WebSocket");
            responseBuilder.AppendLine("Connection: Upgrade");

            //Check if the client send Sec-WebSocket-Key1 and Sec-WebSocket-Key2
            if (String.IsNullOrEmpty(secKey1) && String.IsNullOrEmpty(secKey2))
            {
                //No keys, v.75
                if (!string.IsNullOrEmpty(session.Context.Origin))
                    responseBuilder.AppendLine(string.Format("WebSocket-Origin: {0}", session.Context.Origin));
                responseBuilder.AppendLine(string.Format("WebSocket-Location: ws://{0}{1}", session.Context.Host, session.Context.Path));
                responseBuilder.AppendLine();
                session.SendRawResponse(responseBuilder.ToString());
            }
            else
            {
                //Have Keys, v.76
                if (!string.IsNullOrEmpty(session.Context.Origin))
                    responseBuilder.AppendLine(string.Format("Sec-WebSocket-Origin: {0}", session.Context.Origin));
                responseBuilder.AppendLine(string.Format("Sec-WebSocket-Location: ws://{0}{1}", session.Context.Host, session.Context.Path));
                responseBuilder.AppendLine();
                session.SendRawResponse(responseBuilder.ToString());
                //Encrypt message
                byte[] secret = GetResponseSecurityKey(secKey1, secKey2, secKey3);
                session.SendResponse(secret);
            }
        }

        internal static void ParseHandshake(WebSocketContext context, TextReader reader)
        {
            string line;
            string firstLine = string.Empty;
            string prevKey = string.Empty;

            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                if (string.IsNullOrEmpty(firstLine))
                {
                    firstLine = line;
                    continue;
                }

                if (line.StartsWith("\t") && !string.IsNullOrEmpty(prevKey))
                {
                    string currentValue = context[prevKey];
                    context[prevKey] = currentValue + line.Trim();
                    continue;
                }

                int pos = line.IndexOf(':');

                string key = line.Substring(0, pos);

                if (!string.IsNullOrEmpty(key))
                    key = key.Trim();

                string value = line.Substring(pos + 1);
                if (!string.IsNullOrEmpty(value))
                    value = value.TrimStart(' ');

                if (string.IsNullOrEmpty(key))
                    continue;

                context[key] = value;
                prevKey = key;
            }

            var metaInfo = firstLine.Split(' ');

            context.Method = metaInfo[0];
            context.Path = metaInfo[1];
            context.HttpVersion = metaInfo[2];
        }

        public override void ExecuteCommand(WebSocketSession session, WebSocketCommandInfo commandInfo)
        {
            if (commandInfo.CommandKey.Equals(WebSocketConstant.CommandHead))
            {
                ProcessHandshakeRequest(session);

                if (m_NewSessionConnected != null)
                    m_NewSessionConnected(session);
            }
            else
            {
                if (m_SubCommandsLoaded)
                {
                    ExecuteSubCommand(session, m_SubProtocol.SubCommandParser.ParseSubCommand(commandInfo));
                }
                else
                {
                    base.ExecuteCommand(session, commandInfo);
                }
            }
        }

        private void ExecuteSubCommand(WebSocketSession session, SubCommandInfo subCommandInfo)
        {
            ISubCommand subCommand;

            if (m_SubProtocolCommandDict.TryGetValue(subCommandInfo.CommandKey, out subCommand))
            {
                session.Context.CurrentCommand = subCommandInfo.CommandKey;
                subCommand.ExecuteCommand(session, subCommandInfo);
                session.Context.PrevCommand = subCommandInfo.CommandKey;

                if (Config.LogCommand)
                    LogUtil.LogInfo(this, string.Format("Command - {0} - {1}", session.IdentityKey, subCommandInfo.CommandKey));
            }
            else
            {
                session.HandleUnknownCommand(subCommandInfo);
            }

            session.LastActiveTime = DateTime.Now;
        }

        protected override bool SetupCommands(Dictionary<string, ICommand<WebSocketSession, WebSocketCommandInfo>> commandDict)
        {
            if (m_SubProtocol != null)
            {
                var commandAssembly = m_SubProtocol.GetSubCommandAssembly;
                if (commandAssembly != null)
                {
                    var commandLoader = new ReflectCommandLoader<ISubCommand>(commandAssembly);

                    foreach (var command in commandLoader.LoadCommands())
                    {
                        if (m_SubProtocolCommandDict.ContainsKey(command.Name))
                        {
                            LogUtil.LogError(this, string.Format("You have defined duplicated command {0} in your command assembly!", command.Name));
                            return false;
                        }

                        m_SubProtocolCommandDict.Add(command.Name, command);
                    }

                    m_SubCommandsLoaded = true;
                }
            }
            else
            {
                base.SetupCommands(commandDict);
            }

            return true;
        }

        protected override void OnSocketSessionClosed(object sender, SocketSessionClosedEventArgs e)
        {
            var session = this.GetAppSessionByIndentityKey(e.IdentityKey);

            base.OnSocketSessionClosed(sender, e);

            if (m_SessionClosed != null)
                m_SessionClosed(session, e.Reason);
        }
    }
}
