﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.Common.Logging;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using SuperWebSocket.Config;

namespace SuperWebSocket.SubProtocol
{
    /// <summary>
    /// SubProtocol basis
    /// </summary>
    /// <typeparam name="TWebSocketSession">The type of the web socket session.</typeparam>
    public abstract class SubProtocolBase<TWebSocketSession> : ISubProtocol<TWebSocketSession>
        where TWebSocketSession : WebSocketSession<TWebSocketSession>, new()
    {
        private SubProtocolBase()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubProtocolBase&lt;TWebSocketSession&gt;"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SubProtocolBase(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes with the specified config.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="protocolConfig">The protocol config.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public abstract bool Initialize(IServerConfig config, SubProtocolConfig protocolConfig, ILog logger);

        /// <summary>
        /// Gets or sets the sub request parser.
        /// </summary>
        /// <value>
        /// The sub request parser.
        /// </value>
        public IRequestInfoParser<SubRequestInfo> SubRequestParser { get; protected set; }

        /// <summary>
        /// Tries the get command.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public abstract bool TryGetCommand(string name, out ISubCommand<TWebSocketSession> command);
    }
}
