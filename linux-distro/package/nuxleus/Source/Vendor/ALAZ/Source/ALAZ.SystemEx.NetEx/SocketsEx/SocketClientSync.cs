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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ALAZ.SystemEx.NetEx.SocketsEx
{

    #region SocketClientSync

    public class SocketClientSync: BaseDisposable
    {

        #region Fields

        //----- EndPoints!
        private IPEndPoint FLocalEndPoint;
        private IPEndPoint FRemoteEndPoint;

        //----- Message Types!
        private EncryptType FEncryptType;
        private CompressionType FCompressionType;
        private DelimiterType FDelimiterType;

        //----- Proxy!
        private ProxyInfo FProxyInfo;

        //----- Socket delimiter and buffer size!
        private byte[] FDelimiter;

        private int FMessageBufferSize;
        private int FSocketBufferSize;
        
        private event OnSymmetricAuthenticateEvent FOnSymmetricAuthenticateEvent;
        private event OnSSLClientAuthenticateEvent FOnSSLClientAuthenticateEvent;
        private event OnDisconnectEvent FOnDisconnectedEvent;

        private ISocketConnection FSocketConnection;
        private SocketClient FSocketClient;
        private SocketClientSyncSocketService FSocketClientEvents;
        private SocketClientSyncCryptService FCryptClientEvents;

        private AutoResetEvent FExceptionEvent;

        private AutoResetEvent FConnectEvent;
        private int FConnectTimeout;
        private bool FConnected;
        private object FConnectedSync;

        private AutoResetEvent FSentEvent;
        private int FSentTimeout;

        private Queue<string> FReceivedQueue;
        private AutoResetEvent FReceivedEvent;

        private ManualResetEvent FDisconnectEvent;

        private Exception FLastException;

        #endregion
        
        #region Constructor

        public SocketClientSync(IPEndPoint host)
        {

            FReceivedEvent = new AutoResetEvent(false);
            FExceptionEvent = new AutoResetEvent(false);
            FSentEvent = new AutoResetEvent(false);
            FConnectEvent = new AutoResetEvent(false);
            FDisconnectEvent = new ManualResetEvent(false);

            FReceivedQueue = new Queue<string>();

            FConnectTimeout = 10000;
            FSentTimeout = 10000;

            FConnectedSync = new object();
            FConnected = false;

            FSocketClientEvents = new SocketClientSyncSocketService(this);
            FCryptClientEvents = new SocketClientSyncCryptService(this);
            
            FLocalEndPoint = null;
            FRemoteEndPoint = host;

            //----- Message Types!
            FEncryptType = EncryptType.etNone;
            FCompressionType = CompressionType.ctNone;
            FDelimiterType = DelimiterType.dtNone;

            //----- Proxy!
            FProxyInfo = null;

            //----- Socket delimiter and buffer size!
            FDelimiter = null;

            FMessageBufferSize = 4096;
            FSocketBufferSize = 2048;

        }

        #endregion

        #region Destructor

        protected override void Free(bool canAccessFinalizable)
        {

            FSocketConnection = null;
            FSocketClientEvents = null;
            FCryptClientEvents = null;
            FConnectedSync = null;
            FLastException = null;

            if (FReceivedQueue != null)
            {
                FReceivedQueue.Clear();
                FReceivedQueue = null;
            }

            if (FSocketClient != null)
            {
                FSocketClient.Stop();
                FSocketClient.Dispose();
                FSocketClient = null;
            }

            if (FExceptionEvent != null)
            {
                FExceptionEvent.Close();
                FExceptionEvent = null;
            }

            if (FConnectEvent != null)
            {
                FConnectEvent.Close();
                FConnectEvent = null;
            }

            if (FSentEvent != null)
            {
                FSentEvent.Close();
                FSentEvent = null;
            }

            if (FReceivedEvent != null)
            {
                FReceivedEvent.Close();
                FReceivedEvent = null;
            }

            if (FDisconnectEvent != null)
            {
                FDisconnectEvent.Close();
                FDisconnectEvent = null;
            }

            base.Free(canAccessFinalizable);

        }

        #endregion

        #region Methods

        #region DoOnSSLClientAuthenticate

        internal void DoOnSSLClientAuthenticate(ISocketConnection connection, out string serverName, ref X509Certificate2Collection certs, ref bool checkRevocation)
        {

            serverName = String.Empty;

            if (FOnSSLClientAuthenticateEvent != null)
            {
                FOnSSLClientAuthenticateEvent(connection, out serverName, ref certs, ref checkRevocation);
            }

        }


        #endregion

        #region DoOnSymmetricAuthenticate

        internal void DoOnSymmetricAuthenticate(ISocketConnection connection, out RSACryptoServiceProvider serverKey, out byte[] signMessage)
        {

            serverKey = new RSACryptoServiceProvider();
            serverKey.Clear();

            signMessage = new byte[0];

            if (FOnSymmetricAuthenticateEvent != null)
            {
                FOnSymmetricAuthenticateEvent(connection, out serverKey, out signMessage);
            }   

        }

        #endregion

        #region Connect

        public void Connect()
        {

            if (!Disposed)
            {

                FLastException = null;

                if (!Connected)
                {

                    FConnectEvent.Reset();
                    FExceptionEvent.Reset();
                    FDisconnectEvent.Reset();

                    FSocketClient = new SocketClient(FSocketClientEvents, FDelimiterType, FDelimiter, FSocketBufferSize, FMessageBufferSize);
                    
                    SocketConnector connector = FSocketClient.AddConnector("SocketClientSync", FRemoteEndPoint);
                    
                    connector.EncryptType = FEncryptType;
                    connector.CompressionType = FCompressionType;
                    connector.CryptoService = FCryptClientEvents;
                    connector.ProxyInfo = FProxyInfo;

                    WaitHandle[] wait = new WaitHandle[] { FConnectEvent, FExceptionEvent };

                    FSocketClient.Start();

                    int signal = WaitHandle.WaitAny(wait, FConnectTimeout, false);

                    switch (signal)
                    {

                        case 0:

                            //----- Connect!
                            FLastException = null;
                            Connected = true;
                            
                            break;

                        case 1:

                            //----- Exception!
                            Connected = false;
                            FSocketConnection = null;
                            
                            FSocketClient.Stop();
                            FSocketClient.Dispose();
                            FSocketClient = null;

                            break;

                        default:

                            //----- TimeOut!
                            FLastException = new TimeoutException("Connect timeout.");

                            Connected = false;
                            FSocketConnection = null;
                            
                            FSocketClient.Stop();
                            FSocketClient.Dispose();
                            FSocketClient = null;

                            break;

                    }

                }

            }
            
        }

        #endregion

        #region Write
        
        public void Write(string buffer)
        {
            Write(Encoding.GetEncoding(1252).GetBytes(buffer));
        }

        public void Write(byte[] buffer)
        {

            FLastException = null;

            if (!Disposed)
            {

                if (Connected)
                {

                    FSentEvent.Reset();
                    FExceptionEvent.Reset();

                    WaitHandle[] wait = new WaitHandle[] { FSentEvent, FDisconnectEvent, FExceptionEvent };

                    FSocketConnection.BeginSend(buffer);

                    int signaled = WaitHandle.WaitAny(wait, FSentTimeout, false);

                    switch (signaled)
                    {

                        case 0:

                            //----- Sent!
                            FLastException = null;
                            break;

                        case 1:

                            //----- Disconnected!
                            DoDisconnect();
                            break;

                        case 2:

                            //----- Exception!
                            break;

                        default:

                            //----- TimeOut!
                            FLastException = new TimeoutException("Write timeout.");
                            break;

                    }

                }

            }

        }

        #endregion

        #region Enqueue

        internal void Enqueue(string data)
        {

            if (!Disposed)
            {

                lock (FReceivedQueue)
                {
                    FReceivedQueue.Enqueue(data);
                    FReceivedEvent.Set();
                }

            }

        }

        #endregion

        #region Read
        
        public string Read(int timeOut)
        {

            string result = null;

            if (!Disposed)
            {

                FLastException = null;

                if (Connected)
                {

                    lock (FReceivedQueue)
                    {

                        if (FReceivedQueue.Count > 0)
                        {
                            result = FReceivedQueue.Dequeue();
                        }

                    }

                    if (result == null)
                    {

                        WaitHandle[] wait = new WaitHandle[] { FReceivedEvent, FDisconnectEvent, FExceptionEvent };

                        int signaled = WaitHandle.WaitAny(wait, timeOut, false);

                        switch (signaled)
                        {

                            case 0:

                                //----- Received!
                                lock (FReceivedQueue)
                                {

                                    if (FReceivedQueue.Count > 0)
                                    {
                                        result = FReceivedQueue.Dequeue();
                                    }

                                }

                                FLastException = null;

                                break;

                            case 1:

                                //----- Disconnected!
                                DoDisconnect();
                                break;

                            case 2:

                                //----- Exception!
                                break;

                            default:

                                //----- TimeOut!
                                FLastException = new TimeoutException("Read timeout.");
                                break;

                        }

                    }

                }

            }

            return result;

        }

        #endregion

        #region DoDisconnect

        internal void DoDisconnect()
        {

            bool fireEvent = false;
            
            lock (FConnectedSync)
            {

                if (FConnected)
                {

                    //----- Disconnect!
                    FConnected = false;
                    FSocketConnection = null;

                    if (FSocketClient != null)
                    {
                        FSocketClient.Stop();
                        FSocketClient.Dispose();
                        FSocketClient = null;
                    }
                    
                    fireEvent = true;
                    
                }

            }

            if ( (FOnDisconnectedEvent != null) && fireEvent)
            {
                FOnDisconnectedEvent();
            }

        }

        #endregion

        #region Disconnect

        public void Disconnect()
        {

            if (!Disposed)
            {

                FLastException = null;

                if (Connected)
                {

                    FExceptionEvent.Reset();

                    if (FSocketConnection != null)
                    {

                        WaitHandle[] wait = new WaitHandle[] { FDisconnectEvent, FExceptionEvent };

                        FSocketConnection.BeginDisconnect();

                        int signaled = WaitHandle.WaitAny(wait, FConnectTimeout, false);

                        switch (signaled)
                        {

                            case 0:

                                DoDisconnect();
                                break;

                            case 1:

                                //----- Exception!
                                DoDisconnect();
                                break;

                            default:

                                //----- TimeOut!
                                FLastException = new TimeoutException("Disconnect timeout.");
                                break;

                        }

                    }

                }
            }
        }

        #endregion

        #endregion

        #region Properties

        public event OnDisconnectEvent OnDisconnected
        {

                add
                {
                    FOnDisconnectedEvent += value;
                }

                remove
                {
                    FOnDisconnectedEvent -= value;
                }
        
        }

        public event OnSymmetricAuthenticateEvent OnSymmetricAuthenticate
        {

            add 
            {
                FOnSymmetricAuthenticateEvent += value;
            }

            remove 
            {
                FOnSymmetricAuthenticateEvent -= value;
            }

        }

        public event OnSSLClientAuthenticateEvent OnSSLClientAuthenticate
        {

            add
            {
                FOnSSLClientAuthenticateEvent += value;
            }

            remove
            {
                FOnSSLClientAuthenticateEvent -= value;
            }

        }
        
        public IPEndPoint RemoteEndPoint
        {
            get { return FRemoteEndPoint; }
            set { FRemoteEndPoint = value; }
        }

        public IPEndPoint LocalEndPoint
        {
            get { return FLocalEndPoint; }
            set { FLocalEndPoint = value; }
        }
        
        public DelimiterType DelimiterType
        {
            get { return FDelimiterType; }
            set { FDelimiterType = value; }
        }

        public EncryptType EncryptType
        {
            get { return FEncryptType; }
            set { FEncryptType = value; }
        }

        public CompressionType CompressionType
        {
            get { return FCompressionType; }
            set { FCompressionType = value; }
        }
        
        public byte[] Delimiter
        {
            get { return FDelimiter; }
            set { FDelimiter = value; }
        }

        public ProxyInfo ProxyInfo
        {
            get { return FProxyInfo; }
            set { FProxyInfo = value; }
        }
        
        public int MessageBufferSize
        {
            get { return FMessageBufferSize; }
            set { FMessageBufferSize = value; }
        }

        public int SocketBufferSize
        {
            get { return FSocketBufferSize; }
            set { FSocketBufferSize = value; }
        }

        internal ManualResetEvent DisconnectEvent
        {

            get
            {
                return FDisconnectEvent;
            }

        }

        internal AutoResetEvent ConnectEvent
        {

            get
            {
                return FConnectEvent;
            }

        }

        internal AutoResetEvent SentEvent
        {

            get
            {
                return FSentEvent;
            }

        }

        internal AutoResetEvent ExceptionEvent
        {

            get
            {
                return FExceptionEvent;
            }

        }

        internal ISocketConnection SocketConnection
        {

            get
            {
                return FSocketConnection;
            }

            set 
            {
                FSocketConnection = value;
            }

        }

        public bool Connected
        {
            
            get
            {

                bool connected = false;

                lock (FConnectedSync)
                {
                    connected = FConnected;
                }

                return connected;
                
            }

            internal set 
            {

                lock (FConnectedSync)
                {
                    FConnected = value;
                }

            }

        }

        public Exception LastException
        {
            
            get
            {
                return FLastException;
            }

            internal set
            {
                FLastException = value;
            }

        }

        #endregion

    }

    #endregion

    #region SocketClientSyncSocketService
    
    internal class SocketClientSyncSocketService: BaseSocketService
    {

        #region Fields

        private SocketClientSync FSocketClient;

        #endregion

        #region Constructor

        public SocketClientSyncSocketService(SocketClientSync client)
        {
            FSocketClient = client;
        }

        #endregion

        #region Methods

        public override void OnConnected(ConnectionEventArgs e)
        {
            FSocketClient.SocketConnection = e.Connection;
            FSocketClient.SocketConnection.BeginReceive();
            FSocketClient.ConnectEvent.Set();
        }

        public override void OnException(ExceptionEventArgs e)
        {
            FSocketClient.LastException = e.Exception;
            FSocketClient.ExceptionEvent.Set();
        }

        public override void OnSent(MessageEventArgs e)
        {
            FSocketClient.SentEvent.Set();
        }

        public override void OnReceived(MessageEventArgs e)
        {
            FSocketClient.Enqueue(Encoding.GetEncoding(1252).GetString(e.Buffer));
            FSocketClient.SocketConnection.BeginReceive();
        }

        public override void OnDisconnected(ConnectionEventArgs e)
        {
            FSocketClient.DisconnectEvent.Set();
        }

        #endregion

    }

    #endregion

    #region SocketClientSyncCryptService

    internal class SocketClientSyncCryptService : BaseCryptoService
    {

        #region Fields

        private SocketClientSync FSocketClient;

        #endregion

        #region Constructor

        public SocketClientSyncCryptService(SocketClientSync client)
        {
            FSocketClient = client;
        }

        #endregion

        #region Methods

        public override void OnSymmetricAuthenticate(ISocketConnection connection, out RSACryptoServiceProvider serverKey, out byte[] signMessage)
        {
            FSocketClient.DoOnSymmetricAuthenticate(connection, out serverKey, out signMessage);
        }

        public override void OnSSLClientAuthenticate(ISocketConnection connection, out string serverName, ref X509Certificate2Collection certs, ref bool checkRevocation)
        {
            FSocketClient.DoOnSSLClientAuthenticate(connection, out serverName, ref certs, ref checkRevocation);
        }

        #endregion

    }

    #endregion

}
