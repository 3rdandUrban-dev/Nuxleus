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

namespace ALAZ.SystemEx.NetEx.SocketsEx
{

    /// <summary>
    /// Socket client host.
    /// </summary>
    public class SocketClient : BaseSocketConnectionHost
    {

        #region Constructor

        public SocketClient(ISocketService socketService)
            : base(HostType.htClient, socketService, DelimiterType.dtNone, null, 2048, 8192, 0, 0)
        {
            //-----
        }

        public SocketClient(ISocketService socketService, DelimiterType delimiterType, byte[] delimiter)
            : base(HostType.htClient, socketService, delimiterType, delimiter, 2048, 8192, 0, 0)
        {
            //-----
        }

        public SocketClient(ISocketService socketService, DelimiterType delimiterType, byte[] delimiter, int socketBufferSize, int messageBufferSize)
            : base(HostType.htClient, socketService, delimiterType, delimiter, socketBufferSize, messageBufferSize, 0, 0)
        {
            //-----
        }

        public SocketClient(ISocketService socketService, DelimiterType delimiterType, byte[] delimiter, int socketBufferSize, int messageBufferSize, int idleCheckInterval, int idleTimeOutValue)
            : base(HostType.htClient, socketService, delimiterType, delimiter, socketBufferSize, messageBufferSize, idleCheckInterval, idleTimeOutValue)
        {
            //-----
        }

        #endregion

        #region Methods

        #region BeginReconnect

        /// <summary>
        /// Reconnects the connection adjusting the reconnect timer.
        /// </summary>
        /// <param name="connection">
        /// Connection.
        /// </param>
        /// <param name="sleepTimeOutValue">
        /// Sleep timeout before reconnect.
        /// </param>
        internal override void BeginReconnect(ClientSocketConnection connection)
        {
            
            if (!Disposed)
            {

              SocketConnector connector = (SocketConnector) connection.Creator;

              if ( (connector.ProxyInfo == null || connector.ProxyInfo.Completed) )
              {
                connector.Reconnect(true, null, null);
              }

            }

        }

        #endregion

        #region BeginSendToAll

        internal override void BeginSendToAll(ServerSocketConnection connection, byte[] buffer, bool includeMe) { }

        #endregion

        #region BeginSendTo

        internal override void BeginSendTo(BaseSocketConnection connectionTo, byte[] buffer) { }

        #endregion

        #region GetConnectionById

        internal override BaseSocketConnection GetConnectionById(long connectionId) { return null; }
        internal override BaseSocketConnection[] GetConnectios() { return null; }

        #endregion

        #region AddConnector

        /// <summary>
        /// Adds the client connector (SocketConnector).
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        public SocketConnector AddConnector(string name, IPEndPoint remoteEndPoint)
        {
            return AddConnector(name, remoteEndPoint, null, EncryptType.etNone, CompressionType.ctNone, null, 0, 0, new IPEndPoint(IPAddress.Any, 0));
        }

        public SocketConnector AddConnector(string name, IPEndPoint remoteEndPoint, ProxyInfo proxyData)
        {
            return AddConnector(name, remoteEndPoint, proxyData, EncryptType.etNone, CompressionType.ctNone, null, 0, 0, new IPEndPoint(IPAddress.Any, 0));
        }

        public SocketConnector AddConnector(string name, IPEndPoint remoteEndPoint, ProxyInfo proxyData, EncryptType encryptType, CompressionType compressionType, ICryptoService cryptoService)
        {
            return AddConnector(name, remoteEndPoint, proxyData, encryptType, compressionType, cryptoService, 0, 0, new IPEndPoint(IPAddress.Any, 0));
        }

        public SocketConnector AddConnector(string name, IPEndPoint remoteEndPoint, ProxyInfo proxyData, EncryptType encryptType, CompressionType compressionType, ICryptoService cryptoService, int reconnectAttempts, int reconnectAttemptInterval)
        {
            return AddConnector(name, remoteEndPoint, proxyData, encryptType, compressionType, cryptoService, reconnectAttempts, reconnectAttemptInterval, new IPEndPoint(IPAddress.Any, 0));
        }

        public SocketConnector AddConnector(string name, IPEndPoint remoteEndPoint, ProxyInfo proxyData, EncryptType encryptType, CompressionType compressionType, ICryptoService cryptoService, int reconnectAttempts, int reconnectAttemptInterval, IPEndPoint localEndPoint)
        {

            SocketConnector result = null;
            
            if (!Disposed)
            {
            
              result = new SocketConnector(this, name, remoteEndPoint, proxyData, encryptType, compressionType, cryptoService, reconnectAttempts, reconnectAttemptInterval, localEndPoint);
              AddCreator(result);
              
            }

            return result;
            
        }

        #endregion

        #region Stop

        public override void Stop()
        {

            if (!Disposed)
            {

                StopConnections();
                StopCreators();

            }

            base.Stop();

        }

        #endregion

        #endregion

    }

}
