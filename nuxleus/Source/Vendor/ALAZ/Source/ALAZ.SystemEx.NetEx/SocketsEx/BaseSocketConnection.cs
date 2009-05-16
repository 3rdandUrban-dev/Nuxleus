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
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;

namespace ALAZ.SystemEx.NetEx.SocketsEx
{

    /// <summary>
    /// Base socket connection
    /// </summary>
    public abstract class BaseSocketConnection : BaseDisposable, ISocketConnection
    {

        #region Fields

        private object FCustomData;
        private long FId;

        //----- Connection Host and Creator!
        private BaseSocketConnectionHost FHost;
        private BaseSocketConnectionCreator FCreator;

        //----- Socket and Stream items!
        private Socket FSocket;
        private Stream FStream;

        private bool FActive;
        private object FSyncActive;

        //----- Write items!
        private Queue<MessageBuffer> FWriteQueue;
        private bool FWriteQueueHasItems;

        //----- Read items!
        private object FSyncReadCount;
        private int FReadCount;
        private bool FReadCanEnqueue;

        private DateTime FLastAction;

        private ICryptoTransform FDecryptor;
        private ICryptoTransform FEncryptor;

        #endregion

        #region Constructor

        internal BaseSocketConnection(BaseSocketConnectionHost host, BaseSocketConnectionCreator creator, Socket socket)
        {

            //----- Connection Id!
            FId = host.GetConnectionId();
            FHost = host;
            FCreator = creator;
            FSocket = socket;
            FActive = false;
            FSyncActive = new Object();

            FWriteQueue = new Queue<MessageBuffer>();

            FWriteQueueHasItems = false;
            FReadCanEnqueue = true;

            FReadCount = 0;
            FSyncReadCount = new object();

            FLastAction = DateTime.Now;

            FCustomData = null;
            FEncryptor = null;
            FDecryptor = null;

        }

        #endregion

        #region Destructor

        protected override void Free(bool canAccessFinalizable)
        {

            if (FWriteQueue != null)
            {
                FWriteQueue.Clear();
                FWriteQueue = null;
            }

            if (FStream != null)
            {
                FStream.Close();
                FStream = null;
            }

            if (FSocket != null)
            {
                FSocket.Close();
                FSocket = null;
            }

            if (FDecryptor != null)
            {
                FDecryptor.Dispose();
                FDecryptor = null;
            }

            if (FEncryptor != null)
            {
                FEncryptor.Dispose();
                FEncryptor = null;
            }

            FHost = null;
            FCreator = null;
            FSyncReadCount = null;
            FSyncActive = null;

            base.Free(canAccessFinalizable);

        }

        #endregion

        #region Properties

        internal Queue<MessageBuffer> WriteQueue
        {
            get { return FWriteQueue; }
        }

        internal bool WriteQueueHasItems
        {
            get { return FWriteQueueHasItems; }
            set { FWriteQueueHasItems = value; }
        }

        internal bool ReadCanEnqueue
        {
            get { return FReadCanEnqueue; }
            set { FReadCanEnqueue = value; }
        }

        internal int ReadCount
        {
            get { return FReadCount; }
            set { FReadCount = value; }
        }

        internal object SyncReadCount
        {
            get { return FSyncReadCount; }
        }

        internal object SyncActive
        {
            get { return FSyncActive; }
        }

        internal bool Active
        {

            get 
            {
                if (Disposed)
                {
                    return false;
                }

                lock (FSyncActive)
                {
                    return FActive;
                }
            }

            set 
            {
                lock (FSyncActive)
                {
                    FActive = value;    
                }
            }

        }

        internal ICryptoTransform Encryptor
        {
            get { return FEncryptor; }
            set { FEncryptor = value; }
        }

        internal ICryptoTransform Decryptor
        {
            get { return FDecryptor; }
            set { FDecryptor = value; }
        }

        internal Stream Stream
        {
            get { return FStream; }
            set { FStream = value; }
        }

        internal DateTime LastAction
        {
            get { return FLastAction; }
            set { FLastAction = value; }
        }

        internal Socket Socket
        {
            get { return FSocket; }
        }

        internal byte[] Delimiter
        {
            get { return FHost.Delimiter; }
        }

        internal DelimiterType DelimiterType
        {
            get { return FHost.DelimiterType; }
        }

        internal EncryptType EncryptType
        {
            get { return FCreator.EncryptType; }
        }

        internal CompressionType CompressionType
        {
            get { return FCreator.CompressionType; }
        }

        internal HostType HostType
        {
            get { return FHost.HostType; }
        }


        #endregion

        #region ISocketConnection Members

        #region Properties

        public object CustomData
        {
            get { return FCustomData; }
            set { FCustomData = value; }
        }

        public IPEndPoint LocalEndPoint
        {
            get { return (IPEndPoint)FSocket.LocalEndPoint; }
        }

        public IPEndPoint RemoteEndPoint
        {
            get { return (IPEndPoint)FSocket.RemoteEndPoint; }
        }

        public IntPtr SocketHandle
        {
            get { return FSocket.Handle; }
        }

        public long ConnectionId
        {
          get { return FId; }
        }

        public BaseSocketConnectionCreator Creator
        {
          get { return FCreator; }
        }

        public BaseSocketConnectionHost Host
        {
            get { return FHost; }
        }

        #endregion

        #region Socket Options

        public void SetTTL(short value)
        {
          FSocket.Ttl = value;
        }

        public void SetLinger(LingerOption lo)
        {
          FSocket.LingerState = lo;
        }

        public void SetNagle(bool value)
        {
          FSocket.NoDelay = value;
        }

        #endregion

        #region Abstract Methods

        public abstract IClientSocketConnection AsClientConnection();
        public abstract IServerSocketConnection AsServerConnection();

        #endregion

        #region Send Methods

        public void BeginSend(byte[] buffer)
        {
            FHost.BeginSend(this, buffer, false);
        }

        #endregion

        #region Receive Methods

        public void BeginReceive()
        {
            FHost.BeginReceive(this);
        }

        #endregion

        #region Disconnect Methods

        public void BeginDisconnect()
        {
            FHost.BeginDisconnect(this);
        }

        #endregion

        #endregion

    }

}
