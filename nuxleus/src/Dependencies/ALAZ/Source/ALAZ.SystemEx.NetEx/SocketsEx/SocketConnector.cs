/* ====================================================================
 * Copyright (c) 2007 Andre Luis Azevedo (az.andrel@yahoo.com.br)
 * All rights reserved.
 *                       
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 *    In addition, the source code must keep original namespace names.
 *
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in
 *    the documentation and/or other materials provided with the
 *    distribution. In addition, the binary form must keep the original 
 *    namespace names and original file name.
 * 
 * 3. The name "ALAZ" or "ALAZ Library" must not be used to endorse or promote 
 *    products derived from this software without prior written permission.
 *
 * 4. Products derived from this software may not be called "ALAZ" or
 *    "ALAZ Library" nor may "ALAZ" or "ALAZ Library" appear in their 
 *    names without prior written permission of the author.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY
 * EXPRESSED OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR
 * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE. 
 */

using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ALAZ.SystemEx.NetEx.SocketsEx
{

    /// <summary>
    /// Client socket creator.
    /// </summary>
    public class SocketConnector : BaseSocketConnectionCreator
    {

        #region Fields

        private Socket FSocket;
        private IPEndPoint FRemoteEndPoint;

        private Timer FReconnectTimer;
        private int FReconnectAttempts;
        private int FReconnectAttemptInterval;
        private int FReconnectAttempted;

        private ProxyInfo FProxyInfo;

        #endregion

        #region Constructor

        /// <summary>
        /// Base SocketConnector creator.
        /// </summary>
        /// <param name="host">
        /// Host.
        /// </param>
        /// <param name="remoteEndPoint">
        /// The remote endpoint to connect.
        /// </param>
        /// <param name="encryptType">
        /// Encrypt type.
        /// </param>
        /// <param name="compressionType">
        /// Compression type.
        /// </param>
        /// <param name="cryptoService">
        /// CryptoService. if null, will not be used.
        /// </param>
        /// <param name="localEndPoint">
        /// Local endpoint. if null, will be any address/port.
        /// </param>
        public SocketConnector(BaseSocketConnectionHost host, string name, IPEndPoint remoteEndPoint, ProxyInfo proxyData, EncryptType encryptType, CompressionType compressionType, ICryptoService cryptoService, int reconnectAttempts, int reconnectAttemptInterval, IPEndPoint localEndPoint)
            : base(host, name, localEndPoint, encryptType, compressionType, cryptoService)
        {

            FReconnectTimer = new Timer(new TimerCallback(ReconnectSocketConnection));
            FRemoteEndPoint = remoteEndPoint;

            FReconnectAttempts = reconnectAttempts;
            FReconnectAttemptInterval = reconnectAttemptInterval;

            FReconnectAttempted = -1;

            FProxyInfo = proxyData;

        }

        #endregion

        #region Destructor

        protected override void Free(bool canAccessFinalizable)
        {

            if (FReconnectTimer != null)
            {
                FReconnectTimer.Dispose();
                FReconnectTimer = null;
            }

            if (FSocket != null)
            {
                FSocket.Close();
                FSocket = null;
            }
            
            FRemoteEndPoint = null;
            FProxyInfo = null;

            base.Free(canAccessFinalizable);

        }

        #endregion

        #region Methods

        #region InitializeConnection

        protected override void InitializeConnection(object state)
        {

          BaseSocketConnection connection = (BaseSocketConnection) state;

          if (FProxyInfo != null)
          {
            InitializeProxy(connection);
          }
          else
          {
            base.InitializeConnection(connection);
          }

        }

        #endregion

        #region InitializeProxy

        protected void InitializeProxy(BaseSocketConnection connection)
        {

          if (!Disposed)
          {

            try
            {

              if (connection.Active)
              {

                MessageBuffer mb = new MessageBuffer(0);
                mb.PacketBuffer = FProxyInfo.GetProxyRequestData(FRemoteEndPoint);
                connection.Socket.BeginSend(mb.PacketBuffer, mb.PacketOffSet, mb.PacketRemaining, SocketFlags.None, new AsyncCallback(InitializeProxySendCallback), new CallbackData(connection, mb));
              }

            }
            catch (Exception ex)
            {
              Host.FireOnException(connection, ex);
            }

          }

        }

        #endregion

        #region InitializeProxySendCallback

        private void InitializeProxySendCallback(IAsyncResult ar)
        {

            if (!Disposed)
            {

                BaseSocketConnection connection = null;
                MessageBuffer writeMessage = null;
                CallbackData callbackData = null;

                try
                {

                  callbackData = (CallbackData)ar.AsyncState;
                  connection = callbackData.Connection;
                  writeMessage = callbackData.Buffer;

                  if (connection.Active)
                  {

                    //----- Socket!
                    int writeBytes = connection.Socket.EndSend(ar);

                    if (writeBytes < writeMessage.PacketRemaining)
                    {
                      //----- Continue to send until all bytes are sent!
                      writeMessage.PacketOffSet += writeBytes;
                      connection.Socket.BeginSend(writeMessage.PacketBuffer, writeMessage.PacketOffSet, writeMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(InitializeProxySendCallback), callbackData);
                    }
                    else
                    {

                      writeMessage = null;
                      callbackData = null;

                      MessageBuffer readMessage = new MessageBuffer(4096);
                      connection.Socket.BeginReceive(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(InitializeProxyReceiveCallback), new CallbackData(connection, readMessage));

                    }

                  }

                }
                catch (Exception ex)
                {
                  Host.FireOnException(connection, ex);
                }

            }

        }

        #endregion

        #region InitializeProxyReceiveCallback

        private void InitializeProxyReceiveCallback(IAsyncResult ar)
        {

          if (!Disposed)
          {

            BaseSocketConnection connection = null;
            MessageBuffer readMessage = null;
            CallbackData callbackData = null;

            try
            {

              callbackData = (CallbackData)ar.AsyncState;
              connection = callbackData.Connection;
              readMessage = callbackData.Buffer;

              if (connection.Active)
              {

                int readBytes = connection.Socket.EndReceive(ar);

                if (readBytes > 0)
                {

                  FProxyInfo.GetProxyResponseStatus(readMessage.PacketBuffer);

                  readMessage = null;
                  callbackData = null;

                  if (FProxyInfo.Completed)
                  {
                    base.InitializeConnection(connection);
                  }
                  else
                  {
                    InitializeProxy(connection);
                  }

                }
                else
                {
                  throw new ProxyAuthenticationException(0, "Proxy connection aborted.");
                }

              }

            }
            catch (Exception ex)
            {

                Host.FireOnException(connection, ex);

            }
          
          }

        }

        #endregion

        #region Start

        public override void Start()
        {

            if (!Disposed)
            {
                BeginConnect();
            }

        }

        #endregion

        #region BeginConnect

        /// <summary>
        /// Begin the connection with host.
        /// </summary>
        internal void BeginConnect()
        {

            FSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            FSocket.Bind(InternalLocalEndPoint);

            if (FProxyInfo == null)
            {
              FSocket.BeginConnect(FRemoteEndPoint, new AsyncCallback(BeginConnectCallback), this);
            }
            else
            {
              FProxyInfo.Completed = false;
              FProxyInfo.SOCKS5Phase = SOCKS5Phase.spIdle;
              FSocket.BeginConnect(FProxyInfo.ProxyEndPoint, new AsyncCallback(BeginConnectCallback), this);            
            }

        }

        #endregion

        #region BeginConnectCallback

        /// <summary>
        /// Connect callback!
        /// </summary>
        /// <param name="ar"></param>
        internal void BeginConnectCallback(IAsyncResult ar)
        {

            if (!Disposed)
            {

                BaseSocketConnection connection = null;
                SocketConnector connector = null;

                try
                {

                  connector = (SocketConnector) ar.AsyncState;
                  connection = new ClientSocketConnection(Host, connector, connector.Socket);

                  connector.Socket.EndConnect(ar);

                  //----- Adjust buffer size!
                  connector.Socket.ReceiveBufferSize = Host.SocketBufferSize;
                  connector.Socket.SendBufferSize = Host.SocketBufferSize;
                  
                  connection.Active = true;

                  //----- Initialize!
                  Host.AddSocketConnection(connection);
                  InitializeConnection(connection);

                }
                catch (Exception ex)
                {

                    if (ex is SocketException)
                    {

                      FReconnectAttempted++;
                      Reconnect(false, connection, ex);

                    }
                    else
                    {
                      Host.FireOnException(connection, ex);
                    }

                }

            }

        }

        #endregion

        #region Stop

        public override void Stop()
        {
            Dispose();
        }

        #endregion

        #region Reconnect

        internal void Reconnect(bool resetAttempts, BaseSocketConnection connection, Exception ex)
        {

          if (!Disposed)
          {

              if (resetAttempts)
              {
                FReconnectAttempted = 0;
              }

              if (FReconnectAttempts > 0)
              {

                if (FReconnectAttempted < FReconnectAttempts)
                {

                  Host.RemoveSocketConnection(connection);
                  Host.DisposeAndNullConnection(ref connection);

                  FReconnectTimer.Change(FReconnectAttemptInterval, FReconnectAttemptInterval);

                }
                else
                {
                  Host.FireOnException(connection, new ReconnectAttemptsException("Max reconnect attempts reached"), true);
                }

              }
              else
              {
                
                if ( (connection != null) && (ex != null) )
                {
                  Host.FireOnException(connection, ex, true);
                }

              }

           }

        }

        #endregion

        #region ReconnectSocketConnection

        private void ReconnectSocketConnection(Object stateInfo)
        {

            if (!Disposed)
            {
                FReconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
                BeginConnect();
            }

        }

        #endregion

        #endregion

        #region Properties

        public int ReconnectAttempts
        {
            get { return FReconnectAttempts; }
            set { FReconnectAttempts = value; }
        }

        public int ReconnectAttemptInterval
        {
            get { return FReconnectAttemptInterval; }
            set { FReconnectAttemptInterval = value; }
        }

        public IPEndPoint LocalEndPoint
        {
            get { return InternalLocalEndPoint; }
            set { InternalLocalEndPoint = value; }
        }

        public IPEndPoint RemoteEndPoint
        {
            get { return FRemoteEndPoint; }
        }
        
        public  ProxyInfo ProxyInfo
        {
            get { return FProxyInfo; }
            set { FProxyInfo = value; } 
        }

        internal Socket Socket
        {
            get { return FSocket; }
        }

        #endregion

    }

}
