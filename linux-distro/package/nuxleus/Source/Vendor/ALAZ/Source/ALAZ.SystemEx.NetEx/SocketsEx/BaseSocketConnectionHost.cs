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

using System.Threading;
using System.Xml.Serialization;
using System.Security.Cryptography;

using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.IO;
using System.Text;

using ALAZ.SystemEx.ThreadingEx;

namespace ALAZ.SystemEx.NetEx.SocketsEx
{

    /// <summary>
    /// The connection host.
    /// </summary>
    public abstract class BaseSocketConnectionHost : BaseDisposable
    {

        #region Fields

        private bool FActive;
        private object FSyncActive;

        private HostType FHostType;
        private long FConnectionId;

        //----- Enumerates the connections and creators!
        private ReaderWriterLock FSocketConnectionsSync;
        private Dictionary<long, BaseSocketConnection> FSocketConnections;

        private List<BaseSocketConnectionCreator> FSocketCreators;

        //----- The Socket Service.
        private ISocketService FSocketService;

        //----- Waits for objects removing!
        private ManualResetEvent FWaitCreatorsDisposing;
        private ManualResetEvent FWaitConnectionsDisposing;
        private ManualResetEvent FWaitThreadsDisposing;

        //----- Check idle timer!
        private Timer FIdleTimer;
        private int FIdleCheckInterval;
        private int FIdleTimeOutValue;

        //----- Socket delimiter and buffer size!
        private byte[] FDelimiter;
        private DelimiterType FDelimiterType;

        private int FMessageBufferSize;
        private int FSocketBufferSize;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Base creator for BaseSocketConnectionHost.
        /// </summary>
        /// <param name="hostType">
        /// Host type.
        /// </param>
        /// <param name="socketService">
        /// Socket service.
        /// </param>
        /// <param name="delimiterType">
        /// Delimiter type.
        /// </param>
        /// <param name="delimiter">
        /// Delimiter byte array.
        /// </param>
        /// <param name="socketBufferSize">
        /// Socket buffer size.
        /// </param>
        /// <param name="messageBufferSize">
        /// Max message buffer size.
        /// </param>
        /// <param name="idleCheckInterval">
        /// Idle check interval timeout.
        /// </param>
        /// <param name="idleTimeOutValue">
        /// Idle connection timeout.
        /// </param>
        public BaseSocketConnectionHost(HostType hostType, ISocketService socketService, DelimiterType delimiterType, byte[] delimiter, int socketBufferSize, int messageBufferSize, int idleCheckInterval, int idleTimeOutValue)
        {

            FHostType = hostType;
            FConnectionId = 1000;

            FSocketConnectionsSync = new ReaderWriterLock();

            FSocketConnections = new Dictionary<long, BaseSocketConnection>();
            FSocketCreators = new List<BaseSocketConnectionCreator>();
            FSocketService = socketService;

            FWaitCreatorsDisposing = new ManualResetEvent(false);
            FWaitConnectionsDisposing = new ManualResetEvent(false);
            FWaitThreadsDisposing = new ManualResetEvent(false);

            FIdleCheckInterval = idleCheckInterval;
            FIdleTimeOutValue = idleTimeOutValue;

            if ( (FIdleCheckInterval > 0) && (FIdleTimeOutValue > 0) )
            {
                FIdleTimer = new Timer(new TimerCallback(CheckSocketConnections));
            }

            FDelimiter = delimiter;
            FDelimiterType = delimiterType;

            FMessageBufferSize = messageBufferSize;
            FSocketBufferSize = socketBufferSize;

            FActive = false;
            FSyncActive = new Object();

        }

        #endregion

        #region Destructor

        protected override void Free(bool canAccessFinalizable)
        {

            if (FIdleTimer != null)
            {
                FIdleTimer.Dispose();
                FIdleTimer = null;
            }

            if (FWaitCreatorsDisposing != null)
            {
                FWaitCreatorsDisposing.Set();
                FWaitCreatorsDisposing.Close();
                FWaitCreatorsDisposing = null;
            }

            if (FWaitConnectionsDisposing != null)
            {
                FWaitConnectionsDisposing.Set();
                FWaitConnectionsDisposing.Close();
                FWaitConnectionsDisposing = null;
            }

            if (FWaitThreadsDisposing != null)
            {
                FWaitThreadsDisposing.Set();
                FWaitThreadsDisposing.Close();
                FWaitThreadsDisposing = null;
            }

            if (FSocketConnections != null)
            {
                FSocketConnections.Clear();
                FSocketConnections = null;
            }

            if (FSocketCreators != null)
            {
                FSocketCreators.Clear();
                FSocketCreators = null;
            }

            FSocketConnectionsSync = null;
            FSocketService = null;
            FDelimiter = null;

            base.Free(canAccessFinalizable);

        }

        #endregion

        #region Methods

        #region Start

        /// <summary>
        /// Starts the base host.
        /// </summary>
        public void Start()
        {

            if (!Disposed)
            {

                int loopSleep = 0;
                
                foreach (BaseSocketConnectionCreator creator in FSocketCreators)
                {
                    creator.Start();
                    ThreadEx.LoopSleep(ref loopSleep);
                }

                if (FIdleTimer != null)
                {
                    FIdleTimer.Change(FIdleTimeOutValue, FIdleTimeOutValue);
                }

                Active = true;

            }

        }

        #endregion

        #region Stop

        /// <summary>
        /// Stop the base host.
        /// </summary>
        public virtual void Stop()
        {
            Active = false;
            Dispose();
        }

        #endregion

        #region StopCreators

        /// <summary>
        /// Stop the host creators.
        /// </summary>
        protected void StopCreators()
        {

            //----- Stop Creators!
            BaseSocketConnectionCreator[] creators = GetSocketCreators();

            if (creators != null)
            {

                FWaitCreatorsDisposing.Reset();

                int loopCount = 0;
                
                foreach (BaseSocketConnectionCreator creator in creators)
                {
                
                    try
                    {
                        
                        creator.Stop();
                        
                    }
                    finally
                    {

                        RemoveCreator(creator);
                        creator.Dispose();

                        ThreadEx.LoopSleep(ref loopCount);

                    }

                }

                if (creators.Length > 0)
                {
                    FWaitCreatorsDisposing.WaitOne(10000, false);
                }

            }

        }

        #endregion

        #region StopConnections

        protected void StopConnections()
        {

            if (!Disposed)
            {

                //----- Stop Connections!
                BaseSocketConnection[] connections = GetSocketConnections();

                if (connections != null)
                {

                    FWaitConnectionsDisposing.Reset();
                    
                    int loopSleep = 0;

                    foreach (BaseSocketConnection connection in connections)
                    {
                        connection.BeginDisconnect();
                        ThreadEx.LoopSleep(ref loopSleep);
                    }

                    if (connections.Length > 0)
                    {
                        FWaitConnectionsDisposing.WaitOne(10000, false);
                    }

                }


            }

        }

        #endregion

        #region Fire Methods

        #region FireOnConnected

        internal void FireOnConnected(BaseSocketConnection connection)
        {

            if (connection.Active)
            {
                FSocketService.OnConnected(new ConnectionEventArgs(connection));
            }

        }

        #endregion

        #region FireOnSent

        private void FireOnSent(BaseSocketConnection connection, byte[] buffer, bool sentByServer)
        {
            
            if (connection.Active)
            {
                FSocketService.OnSent(new MessageEventArgs(connection, buffer, sentByServer));
            }

        }

        #endregion

        #region FireOnReceived

        private void FireOnReceived(BaseSocketConnection connection, byte[] buffer, bool readCanEnqueue)
        {

            if (connection.Active)
            {

                if (!readCanEnqueue)
                {

                    lock (connection.SyncReadCount)
                    {
                        connection.ReadCanEnqueue = false;
                    }

                }

                FSocketService.OnReceived(new MessageEventArgs(connection, buffer, false));

                if (!readCanEnqueue)
                {

                    lock (connection.SyncReadCount)
                    {
                        connection.ReadCanEnqueue = true;
                    }

                }

            }

        }

        #endregion

        #region FireOnDisconnected

        private void FireOnDisconnected(ConnectionEventArgs e)
        {
            
          FSocketService.OnDisconnected(e);

        }

        #endregion

        #region FireOnException

        internal void FireOnException(BaseSocketConnection connection, Exception ex, bool forceEvent)
        {
            
          if (forceEvent || connection.Active)
          {
            FSocketService.OnException(new ExceptionEventArgs(connection, ex));
          }

        }

        internal void FireOnException(BaseSocketConnection connection, Exception ex)
        {
          FireOnException(connection, ex, false);
        }

        #endregion

        #endregion

        #region Begin Methods

        #region BeginSend

        /// <summary>
        /// Begin send the data.
        /// </summary>
        internal void BeginSend(BaseSocketConnection connection, byte[] buffer, bool sentByServer)
        {

            if (!Disposed)
            {

                try
                {

                    if (connection.Active)
                    {

                        if (buffer.Length > FMessageBufferSize) 
                        {
                            throw new MessageLengthException("Message length is greater than Host maximum message length.");
                        }

                        connection.LastAction = DateTime.Now;

                        MessageBuffer writeMessage = MessageBuffer.GetPacketMessage(connection, buffer);
                        writeMessage.SentByServer = sentByServer;

                        lock (connection.WriteQueue)
                        {

                            if (connection.WriteQueueHasItems)
                            {
                                //----- If the connection is sending, enqueue the message!
                                connection.WriteQueue.Enqueue(writeMessage);
                            }
                            else
                            {

                                //----- If the connection is not sending, send the message!
                                connection.WriteQueueHasItems = true;

                                if (connection.Stream != null)
                                {
                                    //----- Ssl!
                                    connection.Stream.BeginWrite(writeMessage.PacketBuffer, writeMessage.PacketOffSet, writeMessage.PacketRemaining, new AsyncCallback(BeginSendCallback), new CallbackData(connection, writeMessage));
                                }
                                else
                                {
                                    //----- Socket!
                                    connection.Socket.BeginSend(writeMessage.PacketBuffer, writeMessage.PacketOffSet, writeMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(BeginSendCallback), new CallbackData(connection, writeMessage));
                                }

                            }

                        }

                    }

                }
                catch (Exception ex)
                {
                    FireOnException(connection, ex);
                }

            }

        }

        #endregion

        #region BeginSendCallback

        /// <summary>
        /// Send Callback.
        /// </summary>
        private void BeginSendCallback(IAsyncResult ar)
        {

            if (!Disposed)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(BeginSendCallbackProcessing), ar);
            }

        }

        #endregion

        #region BeginSendCallbackProcessing

        /// <summary>
        /// Send Callback Processing.
        /// </summary>
        private void BeginSendCallbackProcessing(object state)
        {

            if (!Disposed)
            {

                IAsyncResult ar = (IAsyncResult) state;
                BaseSocketConnection connection = null;
                MessageBuffer writeMessage = null;
                bool CanReadQueue = false;

                try
                {

                    CallbackData callbackData = (CallbackData) ar.AsyncState;

                    writeMessage = callbackData.Buffer;
                    connection = callbackData.Connection;

                    if (connection.Active)
                    {

                        if (connection.Stream != null)
                        {

                            //----- Ssl!
                            connection.Stream.EndWrite(ar);
                            FireOnSent(connection, writeMessage.RawBuffer, writeMessage.SentByServer);
                            CanReadQueue = true;

                        }
                        else
                        {

                            //----- Socket!
                            int writeBytes = connection.Socket.EndSend(ar);

                            if (writeBytes < writeMessage.PacketRemaining)
                            {
                                //----- Continue to send until all bytes are sent!
                                writeMessage.PacketOffSet += writeBytes;
                                connection.Socket.BeginSend(writeMessage.PacketBuffer, writeMessage.PacketOffSet, writeMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(BeginSendCallback), callbackData);
                            }
                            else
                            {
                                
                                FireOnSent(connection, writeMessage.RawBuffer, writeMessage.SentByServer);
                                CanReadQueue = true;
                            }

                        }

                        //----- Check Queue!
                        if (CanReadQueue)
                        {

                            callbackData = null;
                            writeMessage = null;

                            lock (connection.WriteQueue)
                            {
                                if (connection.WriteQueue.Count > 0)
                                {

                                    //----- If has items, send it!
                                    MessageBuffer dequeueWriteMessage = connection.WriteQueue.Dequeue();

                                    if (connection.Stream != null)
                                    {
                                        //----- Ssl!
                                        connection.Stream.BeginWrite(dequeueWriteMessage.PacketBuffer, dequeueWriteMessage.PacketOffSet, dequeueWriteMessage.PacketRemaining, new AsyncCallback(BeginSendCallback), new CallbackData(connection, dequeueWriteMessage));
                                    }
                                    else
                                    {
                                        //----- Socket!
                                        connection.Socket.BeginSend(dequeueWriteMessage.PacketBuffer, dequeueWriteMessage.PacketOffSet, dequeueWriteMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(BeginSendCallback), new CallbackData(connection, dequeueWriteMessage));
                                    }

                                }
                                else
                                {
                                    connection.WriteQueueHasItems = false;
                                }

						    }

                        }

                    }

                }
                catch (Exception ex)
                {
                    FireOnException(connection, ex);
                }

            }

        }

        #endregion

        #region BeginReceive

        /// <summary>
        /// Receive data from connetion.
        /// </summary>
        internal void BeginReceive(BaseSocketConnection connection)
        {

            if (!Disposed)
            {

                try
                {

                    if (connection.Active)
                    {

                        lock (connection.SyncReadCount)
                        {

                            if (connection.ReadCanEnqueue)
                            {

                                if (connection.ReadCount == 0)
                                {

                                    //----- if the connection is not receiving, start the receive!
                                    MessageBuffer readMessage = new MessageBuffer(FSocketBufferSize);

                                    if (connection.Stream != null)
                                    {
                                        //----- Ssl!
                                        connection.Stream.BeginRead(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, new AsyncCallback(BeginReadCallback), new CallbackData(connection, readMessage));
                                    }
                                    else
                                    {
                                        //----- Socket!
                                        connection.Socket.BeginReceive(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(BeginReadCallback), new CallbackData(connection, readMessage));
                                    }

                                }

                                //----- Increase the read count!
                                connection.ReadCount++;

                            }

                        }

                    }

                }
                catch (Exception ex)
                {
                    FireOnException(connection, ex);
                }

            }

        }

        #endregion

        #region BeginReadCallback

        private void BeginReadCallback(IAsyncResult ar)
        {

            if (!Disposed)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(BeginReadCallbackProcessing), ar);
            }

        }

        #endregion

        #region BeginReadCallbackProcessing

        private void BeginReadCallbackProcessing(object state)
        {
            
            if (!Disposed)
            {

                IAsyncResult ar = (IAsyncResult)state;

                BaseSocketConnection connection = null;

                try
                {

                    CallbackData callbackData = (CallbackData)ar.AsyncState;

                    connection = callbackData.Connection;

                    if (connection.Active)
                    {

                        int readBytes = 0;

                        if (connection.Stream != null)
                        {
                            //----- Ssl!
                            readBytes = connection.Stream.EndRead(ar);
                        }
                        else
                        {
                            //----- Socket!
                            readBytes = connection.Socket.EndReceive(ar);
                        }

                        if (readBytes > 0)
                        {

                            ReadBytesFromConnection(callbackData, readBytes);

                        }
                        else
                        {
                            //----- Is has no data to read then the connection has been terminated!
                            connection.BeginDisconnect();
                        }

                    }
                }
                catch (Exception ex)
                {
                    FireOnException(connection, ex);
                }

            }

        }

        #endregion

        #region ReadBytesFromConnection

        private void ReadBytesFromConnection(CallbackData callbackData, int readBytes)
        {

            BaseSocketConnection connection = callbackData.Connection;
            MessageBuffer readMessage = callbackData.Buffer;

            //----- Has bytes!
            connection.LastAction = DateTime.Now;

            byte[] rawBuffer = null;
            bool socketWasRead = false;
            
            readMessage.PacketOffSet += readBytes;

            switch (connection.DelimiterType)
            {

                case DelimiterType.dtNone:

                    //----- Message with no delimiter!
                    rawBuffer = readMessage.GetRawBuffer(readBytes, 0);

                    break;

                case DelimiterType.dtPacketHeader:

                    //----- Message with packet header!
                    rawBuffer = ReadMessageWithPacketHeader(connection.Delimiter, callbackData, ref socketWasRead);

                    break;

                case DelimiterType.dtMessageTailExcludeOnReceive:
                case DelimiterType.dtMessageTailIncludeOnReceive:


                    //----- Message with tail!
                    rawBuffer = ReadMessageWithMessageTail(connection.Delimiter, callbackData, ref socketWasRead);

                    break;

            }

            if (rawBuffer != null)
            {

                //----- Decrypt!
                //rawBuffer = CryptUtils.DecryptData(connection, rawBuffer, FMessageBufferSize);

                //----- Fire Event!
                FireOnReceived(connection, rawBuffer, true);
                
            }

            readMessage = null;
            callbackData = null;
            
            if(!socketWasRead)
            {

                //----- Check Queue!
                lock (connection.SyncReadCount)
                {

                    connection.ReadCount--;

                    if (connection.ReadCount > 0)
                    {

                        //----- if it need more receiving, start the receive!
                        MessageBuffer continueReadMessage = new MessageBuffer(FSocketBufferSize);

                        //----- if the read queue has items, start to receive!
                        if (connection.Stream != null)
                        {
                            //----- Ssl!
                            connection.Stream.BeginRead(continueReadMessage.PacketBuffer, continueReadMessage.PacketOffSet, continueReadMessage.PacketRemaining, new AsyncCallback(BeginReadCallback), new CallbackData(connection, continueReadMessage));
                        }
                        else
                        {
                            //----- Socket!
                            connection.Socket.BeginReceive(continueReadMessage.PacketBuffer, continueReadMessage.PacketOffSet, continueReadMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(BeginReadCallback), new CallbackData(connection, continueReadMessage));
                        }

                    }

                }

            }

        }

        #endregion

        #region ReadMessageWithPacketHeader

        private byte[] ReadMessageWithPacketHeader(byte[] connectionDelimiter, CallbackData callbackData, ref bool socketWasRead)
        {

            byte[] rawBuffer = null;

            BaseSocketConnection connection = callbackData.Connection;
            MessageBuffer readMessage = callbackData.Buffer;

            //----- Message with delimiter!
            int delimiterSize = connectionDelimiter.Length + 3;

            bool readPacket = false;
            bool readSocket = false;

            do
            {

                rawBuffer = null;

                if (readMessage.PacketOffSet > delimiterSize)
                {

                    //----- Has Delimiter!
                    for (int i = 0; i < connectionDelimiter.Length; i++)
                    {
                        if (connectionDelimiter[i] != readMessage.PacketBuffer[i])
                        {
                            //----- Bad Delimiter!
                            throw new BadDelimiterException("Message delimiter is different from Host delimiter.");
                        }

                    }

                    //----- Get Length!
                    int messageLength = (readMessage.PacketBuffer[connectionDelimiter.Length] << 16)
                                        + (readMessage.PacketBuffer[connectionDelimiter.Length + 1] << 8)
                                        + (readMessage.PacketBuffer[connectionDelimiter.Length + 2]);

                    if (messageLength > FMessageBufferSize)
                    {
                        throw new MessageLengthException("Message length is greater than Host maximum message length.");
                    }

                    //----- Check Length!
                    if (messageLength == readMessage.PacketOffSet)
                    {

                        //----- Equal -> Get rawBuffer!
                        rawBuffer = readMessage.GetRawBuffer(messageLength, delimiterSize);

                        //----- Decrypt!
                        rawBuffer = CryptUtils.DecryptData(connection, rawBuffer, FMessageBufferSize);

                        readPacket = false;
                        readSocket = false;

                    }
                    else
                    {

                        if (messageLength < readMessage.PacketOffSet)
                        {

                            //----- Less -> Get rawBuffer and fire event!
                            rawBuffer = readMessage.GetRawBuffer(messageLength, delimiterSize);

                            //----- Decrypt!
                            rawBuffer = CryptUtils.DecryptData(connection, rawBuffer, FMessageBufferSize);

                            readPacket = true;
                            readSocket = false;

                            FireOnReceived(connection, rawBuffer, false);

                        }
                        else
                        {

                            if (messageLength > readMessage.PacketOffSet)
                            {

                                //----- Greater -> Read Socket!
                                if (messageLength > readMessage.PacketLength)
                                {
                                    readMessage.Resize(messageLength);
                                }

                                readPacket = false;
                                readSocket = true;

                            }

                        }

                    }

                }
                else
                {

                    if (readMessage.PacketRemaining < delimiterSize)
                    {
                        //----- Adjust room for more! 
                        readMessage.Resize(readMessage.PacketLength + delimiterSize);
                    }

                    readPacket = false;
                    readSocket = true;

                }

            } while (readPacket);

            if (readSocket)
            {

              if (connection.Active)
              {

                //----- Read More!
                if (connection.Stream != null)
                {
                  //----- Ssl!
                  connection.Stream.BeginRead(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, new AsyncCallback(BeginReadCallback), callbackData);
                }
                else
                {
                  //----- Socket!
                  connection.Socket.BeginReceive(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(BeginReadCallback), callbackData);
                }

              }

            }

            socketWasRead = readSocket;
            return rawBuffer;

        }

        #endregion

        #region ReadMessageWithMessageTail

        private byte[] ReadMessageWithMessageTail(byte[] connectionDelimiter, CallbackData callbackData, ref bool socketWasRead)
        {

            string stringDelimiter = Encoding.GetEncoding(1252).GetString(connectionDelimiter);

            byte[] rawBuffer = null;

            BaseSocketConnection connection = callbackData.Connection;
            MessageBuffer readMessage = callbackData.Buffer;

            //----- Message with delimiter!
            int delimiterSize = connectionDelimiter.Length;

            bool readPacket = false;
            bool readSocket = false;

            do
            {

                rawBuffer = null;

                if (readMessage.PacketOffSet > delimiterSize)
                {

                    int index = Encoding.GetEncoding(1252).GetString(readMessage.PacketBuffer).IndexOf(stringDelimiter);

                    if (index >= 0)
                    {

                        rawBuffer = readMessage.GetRawBufferWithTail(connection, index, delimiterSize);

                        //----- Decrypt!
                        rawBuffer = CryptUtils.DecryptData(connection, rawBuffer, FMessageBufferSize);

                        if (readMessage.PacketOffSet == 0)
                        {
                            readPacket = false;
                            readSocket = false;
                        }
                        else
                        {

                            readPacket = true;
                            readSocket = false;

                            FireOnReceived(connection, rawBuffer, false);
                            
                        }

                    }
                    else
                    { 
                    
                        readPacket = false;
                        readSocket = true;
                        
                    }
                    
                }
                else
                {
                
                    readPacket = false;
                    readSocket = (readMessage.PacketOffSet > 0);

                }
                
                    
            } while (readPacket);

            //----- Adjust room for more! 
            readMessage.Resize(FMessageBufferSize);

            if (readSocket)
            {

                if (connection.Active)
                {

                    //----- Read More!
                    if (connection.Stream != null)
                    {
                        //----- Ssl!
                        connection.Stream.BeginRead(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, new AsyncCallback(BeginReadCallback), callbackData);
                    }
                    else
                    {
                        //----- Socket!
                        connection.Socket.BeginReceive(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(BeginReadCallback), callbackData);
                    }

                }

            }

            socketWasRead = readSocket;
            return rawBuffer;

        }

        #endregion

        #region BeginDisconnect

        /// <summary>
        /// Begin disconnect the connection
        /// </summary>
        internal void BeginDisconnect(BaseSocketConnection connection)
        {

            if (!Disposed)
            {

              ConnectionEventArgs e = new ConnectionEventArgs(connection);

              if (connection.Active)
              {

                try
                {

                  if ((Environment.OSVersion.Version.Major == 5)
                      && (Environment.OSVersion.Version.Minor >= 1))
                  {

                    //----- NT5 / WinXP and later!
                    connection.Socket.BeginDisconnect(false, new AsyncCallback(BeginDisconnectCallback), e);

                  }
                  else
                  {

                    //----- NT5 / Win2000!
                    ThreadPool.QueueUserWorkItem(new WaitCallback(BeginDisconnectCallbackProcessing), e);
                    
                  }

                }
                catch (Exception ex)
                {
                  FireOnException(connection, ex);
                }

              }
              else
              {

                RemoveSocketConnection(connection);
                DisposeAndNullConnection(ref connection);

              }

            }

        }

        #endregion

        #region BeginDisconnectCallback

        /// <summary>
        /// Disconnect callback.
        /// </summary>
        private void BeginDisconnectCallback(IAsyncResult ar)
        {

            if (!Disposed)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(BeginDisconnectCallbackProcessing), ar);
            }

        }

        #endregion

        #region BeginDisconnectCallbackProcessing

        private void BeginDisconnectCallbackProcessing(object state)
        {

            if (!Disposed)
            {

                bool calledByBeginDisconnect = false;
                
                IAsyncResult ar = null;
                BaseSocketConnection connection = null;
                ConnectionEventArgs e = null;

                try
                {

                    if (state is ConnectionEventArgs)
                    {

                        //----- NT5 / Win2000!
                        e = (ConnectionEventArgs)state;
                        calledByBeginDisconnect = false;

                    }
                    else
                    {

                        //----- NT5 / WinXP and later!
                        ar = (IAsyncResult)state;
                        e = (ConnectionEventArgs)ar.AsyncState;
                        calledByBeginDisconnect = true;

                    }

                    connection = (BaseSocketConnection)e.Connection;

                    if (connection.Active)
                    {

                        if (calledByBeginDisconnect)
                        {
                            connection.Socket.EndDisconnect(ar);
                        }

                        lock (connection.SyncActive)
                        {
                            connection.Active = false;
                            connection.Socket.Close();
                        }

                        FireOnDisconnected(e);
                        
                    }

                    RemoveSocketConnection(connection);
                    DisposeAndNullConnection(ref connection);

                }
                catch (Exception ex)
                {
                    FireOnException(connection, ex);
                }

            }

        }

        #endregion
        
        #endregion

        #region Abstract Methods

        internal abstract void BeginReconnect(ClientSocketConnection connection);
        internal abstract void BeginSendToAll(ServerSocketConnection connection, byte[] buffer, bool includeMe);
        internal abstract void BeginSendTo(BaseSocketConnection connectionTo, byte[] buffer);
        internal abstract BaseSocketConnection GetConnectionById(long connectionId);
        internal abstract BaseSocketConnection[] GetConnectios();

        #endregion

        #endregion

        #region Connection Methods

        #region GetConnectionId

        internal long GetConnectionId()
        {
            return Interlocked.Increment(ref FConnectionId);
        }

        #endregion

        #region AddSocketConnection

        internal void AddSocketConnection(BaseSocketConnection socketConnection)
        {

            if (!Disposed)
            {

                FSocketConnectionsSync.AcquireWriterLock(Timeout.Infinite);

                try
                {
                    FSocketConnections.Add(socketConnection.ConnectionId, socketConnection);
                }
                finally
                {
                    FSocketConnectionsSync.ReleaseWriterLock();
                }

            }

        }

        #endregion

        #region RemoveSocketConnection

        internal void RemoveSocketConnection(BaseSocketConnection socketConnection)
        {

          if (!Disposed)
          {

              if (socketConnection != null)
              {


                  FSocketConnectionsSync.AcquireWriterLock(Timeout.Infinite);

                  try
                  {

                      FSocketConnections.Remove(socketConnection.ConnectionId);

                  }
                  finally
                  {
                      FSocketConnectionsSync.ReleaseWriterLock();

                      if (FSocketConnections.Count <= 0)
                      {
                          FWaitConnectionsDisposing.Set();
                      }

                  }

              }

        }

        }

        #endregion

        #region DisposeAndNullConnection

        internal void DisposeAndNullConnection(ref BaseSocketConnection connection)
        {

          if (connection != null)
          {
            connection.Dispose();
            connection = null;
          }

        }

        #endregion

        #region GetSocketConnections

        internal BaseSocketConnection[] GetSocketConnections()
        {

            BaseSocketConnection[] items = null;

            if (!Disposed)
            {

                FSocketConnectionsSync.AcquireReaderLock(Timeout.Infinite);

                try
                {
                    items = new BaseSocketConnection[FSocketConnections.Count];
                    FSocketConnections.Values.CopyTo(items, 0);
                }
                finally
                {
                    FSocketConnectionsSync.ReleaseReaderLock();
                }

            }

            return items;

        }

        #endregion

        #region GetSocketConnectionById

        internal BaseSocketConnection GetSocketConnectionById(long connectionId)
        {

            BaseSocketConnection item = null;

            if (!Disposed)
            {

                
                FSocketConnectionsSync.AcquireReaderLock(Timeout.Infinite);

                try
                {
                    item = FSocketConnections[connectionId];
                }
                finally
                {
                    FSocketConnectionsSync.ReleaseReaderLock();
                }

            }

            return item;

        }

        #endregion

        #region CheckSocketConnections

        private void CheckSocketConnections(Object stateInfo)
        {

            if (!Disposed)
            {

                //----- Disable timer event!
                FIdleTimer.Change(Timeout.Infinite, Timeout.Infinite);

                try
                {

                    //----- Get connections!
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

                                if (cnn != null)
                                {

                                    //----- Check the idle timeout!
                                    if (DateTime.Now > (cnn.LastAction.AddMilliseconds(FIdleTimeOutValue)))
                                    {
                                        cnn.BeginDisconnect();
                                    }

                                }

                            }
                            finally
                            {

                                ThreadEx.LoopSleep(ref loopSleep);

                            }

                        }

                    }

                }
                finally
                {
                    
                    if (!Disposed)
                    {
                        //----- Restart the timer event!
                        FIdleTimer.Change(FIdleCheckInterval, FIdleCheckInterval);
                    }

                }

            }

        }

        #endregion

        #region Creators Methods

        #region AddCreator

        protected void AddCreator(BaseSocketConnectionCreator creator)
        {

            if (!Disposed)
            {
                lock (FSocketCreators)
                {
                    FSocketCreators.Add(creator);
                }

            }

        }

        #endregion

        #region RemoveCreator

        protected void RemoveCreator(BaseSocketConnectionCreator creator)
        {
            if (!Disposed)
            {
                lock (FSocketCreators)
                {
                    FSocketCreators.Remove(creator);

                    if (FSocketCreators.Count <= 0)
                    {
                        FWaitCreatorsDisposing.Set();
                    }

                }
            }
        }

        #endregion

        #region GetSocketCreators

        protected BaseSocketConnectionCreator[] GetSocketCreators()
        {

            BaseSocketConnectionCreator[] items = null;

            if (!Disposed)
            {
                lock (FSocketCreators)
                {
                    items = new BaseSocketConnectionCreator[FSocketCreators.Count];
                    FSocketCreators.CopyTo(items, 0);
                }

            }

            return items;

        }

        #endregion

        #endregion

        #endregion

        #region Properties

        public int SocketBufferSize
        {
            get { return FSocketBufferSize; }
            set { FSocketBufferSize = value; }  
        }

        public int MessageBufferSize
        {
            get { return FMessageBufferSize; }
            set { FMessageBufferSize = value; }  
        }

        public byte[] Delimiter
        {
            get { return FDelimiter; }
            set { FDelimiter = value; }
        }

        public DelimiterType DelimiterType
        {
            get { return FDelimiterType; }
            set { FDelimiterType = value; }
        }

        public ISocketService SocketService
        {
            get { return FSocketService; }
        }

        protected Timer CheckTimeOutTimer
        {
            get { return CheckTimeOutTimer; }
        }

        public int IdleCheckInterval
        {
            get { return FIdleCheckInterval; }
            set { FIdleCheckInterval = value; }
        }

        public int IdleTimeOutValue
        {
            get { return FIdleTimeOutValue; }
            set { FIdleTimeOutValue = value; } 
        }

        public HostType HostType
        {
            get { return FHostType; }
        }

        public bool Active
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

            internal set
            {
                lock (FSyncActive)
                {
                    FActive = value;
                }
            }

        }


        #endregion

    }

}
