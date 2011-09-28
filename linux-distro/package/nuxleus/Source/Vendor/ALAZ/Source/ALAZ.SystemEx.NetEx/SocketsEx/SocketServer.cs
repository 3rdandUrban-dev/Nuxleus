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

using ALAZ.SystemEx.ThreadingEx;

namespace ALAZ.SystemEx.NetEx.SocketsEx
{

    /// <summary>
    /// Server connection host.
    /// </summary>
    public class SocketServer : BaseSocketConnectionHost
    {

        #region Constructor

        public SocketServer(ISocketService socketService)
            : base(HostType.htServer, socketService, DelimiterType.dtNone, null, 4096, 8192, 0, 0)
        {
            //-----
        }

        public SocketServer(ISocketService socketService, DelimiterType delimiterType, byte[] delimiter)
            : base(HostType.htServer, socketService, delimiterType, delimiter, 4096, 8192, 0, 0)
        {
            //-----
        }

        public SocketServer(ISocketService socketService, DelimiterType delimiterType, byte[] delimiter, int socketBufferSize, int messageBufferSize)
            : base(HostType.htServer, socketService, delimiterType, delimiter, socketBufferSize, messageBufferSize, 0, 0)
        {
            //-----
        }

        public SocketServer(ISocketService socketService, DelimiterType delimiterType, byte[] delimiter, int socketBufferSize, int messageBufferSize, int idleCheckInterval, int idleTimeOutValue)
            : base(HostType.htServer, socketService, delimiterType, delimiter, socketBufferSize, messageBufferSize, idleCheckInterval, idleTimeOutValue)
        {
            //-----
        }

        #endregion

        #region Methods

        #region BeginReconnect

        internal override void BeginReconnect(ClientSocketConnection connection) { }

        #endregion

        #region BeginSendToAll

        internal override void BeginSendToAll(ServerSocketConnection connection, byte[] buffer, bool includeMe)
        {

            if (!Disposed)
            {

                BaseSocketConnection[] items = GetSocketConnections();

                if (items != null)
                {

                    int loopSleep = 0;
                    
                    foreach (BaseSocketConnection cnn in items)
                    {

                        if (Disposed)
                        {
                            break;
                        }

                        try
                        {
                            
                            
                            if (includeMe || connection != cnn)
                            {

                                byte[] localBuffer = new byte[buffer.Length];
                                Buffer.BlockCopy(buffer, 0, localBuffer, 0, buffer.Length);
                                
                                BeginSend(cnn, localBuffer, true);

                            }
                            
                        }
                        finally
                        {
                            
                            ThreadEx.LoopSleep(ref loopSleep);
                            
                        }

                    }

                }

            }

        }

        #endregion

        #region BeginSendTo

        internal override void BeginSendTo(BaseSocketConnection connection, byte[] buffer)
        {

            if (!Disposed)
            {
                BeginSend(connection, buffer, true);
            }

        }

        #endregion

        #region GetConnectionById

        internal override BaseSocketConnection GetConnectionById(long connectionId)
        {

            BaseSocketConnection result = null;

            if (!Disposed)
            {
                result = GetSocketConnectionById(connectionId);
            }

            return result;

        }

        internal override BaseSocketConnection[] GetConnectios()
        { 
            
            return GetSocketConnections();
            
        }

        #endregion

        #region AddListener

        /// <summary>
        /// Add the server connector (SocketListener).
        /// </summary>
        /// <param name="localEndPoint"></param>
        public SocketListener AddListener(string name, IPEndPoint localEndPoint)
        {
          return AddListener(name, localEndPoint, EncryptType.etNone, CompressionType.ctNone, null, 5, 2);
        }

        public SocketListener AddListener(string name, IPEndPoint localEndPoint, EncryptType encryptType, CompressionType compressionType, ICryptoService cryptoService)
        {
          return AddListener(name, localEndPoint, encryptType, compressionType, cryptoService, 5, 2);
        }

        public SocketListener AddListener(string name, IPEndPoint localEndPoint, EncryptType encryptType, CompressionType compressionType, ICryptoService cryptoService, byte backLog, byte acceptThreads)
        {

            SocketListener listener = null;
            
            if (!Disposed)
            {
              
              listener = new SocketListener(this, name, localEndPoint, encryptType, compressionType, cryptoService, backLog, acceptThreads);
              AddCreator(listener);
              
            }
            
            return listener;
            
        }

        #endregion

        #region Stop

        public override void Stop()
        {

            if (!Disposed)
            {

                StopCreators();
                StopConnections();

            }

            base.Stop();

        }

        #endregion

        #region GetConnections

        public ISocketConnectionInfo[] GetConnections()
        {
            return GetConnectios();
        }

        #endregion

        #endregion

    }

}
