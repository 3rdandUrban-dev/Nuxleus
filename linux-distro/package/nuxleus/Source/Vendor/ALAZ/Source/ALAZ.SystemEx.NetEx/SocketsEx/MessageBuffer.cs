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

namespace ALAZ.SystemEx.NetEx.SocketsEx
{

    /// <summary>
    /// Contains original buffer and the read/send buffer and offset.
    /// </summary>
    internal class MessageBuffer
    {

        #region Fields

        private byte[] FRawBuffer;
        private byte[] FPacketBuffer;
        private int FPacketOffSet;
        private bool FSentByServer;
        
        #endregion

        #region Constructor

        public MessageBuffer(int bufferSize)
        {
            
            FPacketBuffer = null;

            if (bufferSize > 0)
            {
                FPacketBuffer = new byte[bufferSize];
            }

            FPacketOffSet = 0;
            FRawBuffer = null;
            FSentByServer = false;

        }

        public MessageBuffer(byte[] rawBuffer, byte[] packetBuffer)
        {
            FRawBuffer = rawBuffer;
            FPacketBuffer = packetBuffer;
            FPacketOffSet = 0;
            FSentByServer = false;
        }

        #endregion

        #region Methods

        #region GetPacketMessage

        /// <summary>
        /// Gets a packet message!
        /// </summary>
        /// <param name="connection">
        /// Socket connection.
        /// </param>
        /// <param name="buffer">
        /// Data.
        /// </param>
        public static MessageBuffer GetPacketMessage(BaseSocketConnection connection, byte[] buffer)
        {

            byte[] workBuffer = null;
            MessageBuffer messageBuffer = null;

            workBuffer = CryptUtils.EncryptData(connection, buffer);

            switch (connection.DelimiterType)
            {

                case DelimiterType.dtNone:

                    //----- No Delimiter!
                    messageBuffer = new MessageBuffer(buffer, workBuffer);
                    break;

                case DelimiterType.dtPacketHeader:

                    if (connection.Delimiter != null && connection.Delimiter.Length >= 0)
                    {

                        //----- Need delimiter!
                        int delimiterSize = connection.Delimiter.Length + 3;
                        byte[] result = new byte[workBuffer.Length + delimiterSize];

                        int messageLength = result.Length;

                        //----- Delimiter!
                        for (int i = 0; i < connection.Delimiter.Length; i++)
                        {
                            result[i] = connection.Delimiter[i];
                        }

                        //----- Length!
                        result[connection.Delimiter.Length] = Convert.ToByte((messageLength & 0xFF0000) >> 16);
                        result[connection.Delimiter.Length + 1] = Convert.ToByte((messageLength & 0xFF00) >> 8);
                        result[connection.Delimiter.Length + 2] = Convert.ToByte(messageLength & 0xFF);

                        Buffer.BlockCopy(workBuffer, 0, result, delimiterSize, workBuffer.Length);
                        
                        messageBuffer = new MessageBuffer(buffer, result);

                    }

                    break;

                case DelimiterType.dtMessageTailExcludeOnReceive:
                case DelimiterType.dtMessageTailIncludeOnReceive:

                    if (connection.Delimiter != null && connection.Delimiter.Length >= 0)
                    {

                        //----- Need delimiter!
                        byte[] result = new byte[workBuffer.Length + connection.Delimiter.Length];

                        Buffer.BlockCopy(workBuffer, 0, result, 0, workBuffer.Length);
                        Buffer.BlockCopy(connection.Delimiter, 0, result, workBuffer.Length, connection.Delimiter.Length);

                        messageBuffer = new MessageBuffer(buffer, result);

                    }

                    break;

            }

            return messageBuffer;
            
        }

        #endregion

        #region GetRawBuffer

        /// <summary>
        /// Get the buffer from packet message!
        /// </summary>
        /// <param name="messageLength">
        /// Message offset.
        /// </param>
        /// <param name="delimiterSize">
        /// Service delimiter size.
        /// </param>
        public byte[] GetRawBuffer(int messageLength, int delimiterSize)
        {

            //----- Get Raw Buffer!
            byte[] result = null;

            result = new byte[messageLength - delimiterSize];
            
            Buffer.BlockCopy(FPacketBuffer, delimiterSize, result, 0, result.Length);

            //----- Adjust Packet Buffer!
            byte[] packetBuffer = new byte[FPacketBuffer.Length - messageLength];
            Buffer.BlockCopy(FPacketBuffer, messageLength, packetBuffer, 0, packetBuffer.Length);

            FPacketBuffer = packetBuffer;
            FPacketOffSet = FPacketOffSet - messageLength;

            return result;

        }

        #endregion

        #region GetRawBufferWithTail

        public byte[] GetRawBufferWithTail(BaseSocketConnection connection, int index, int delimiterSize)
        { 
        
            //----- Get Raw Buffer with Tail!
            byte[] result = null;
            int messageLength = 0;
            int clearLength = index + delimiterSize;

            if (connection.DelimiterType == DelimiterType.dtMessageTailIncludeOnReceive)
            {
                messageLength = index + delimiterSize;
            }
            else
            {
                messageLength = index;
            }

            result = new byte[messageLength];
            Buffer.BlockCopy(FPacketBuffer, 0, result, 0, messageLength);

            //----- Adjust Packet Buffer!
            byte[] packetBuffer = new byte[FPacketBuffer.Length - clearLength];
            Buffer.BlockCopy(FPacketBuffer, clearLength, packetBuffer, 0, packetBuffer.Length);
            
            FPacketBuffer = packetBuffer;
            FPacketOffSet = FPacketOffSet - clearLength;

            return result;

        }

        #endregion

        #region Resize

        /// <summary>
        /// Resize the buffer.
        /// </summary>
        /// <param name="newLength">
        /// The new length of buffer.
        /// </param>
        public void Resize(int newLength)
        {
            Array.Resize(ref FPacketBuffer, newLength);
        }

        #endregion

        #endregion

        #region Properties

        public byte[] RawBuffer
        {
            get { return FRawBuffer; }
            set { FRawBuffer = value; }
        }

        public byte[] PacketBuffer
        {
            get { return FPacketBuffer; }
            set { FPacketBuffer = value; }
        }

        public int PacketOffSet
        {
            get { return FPacketOffSet; }
            set { FPacketOffSet = value; }
        }

        public int PacketRemaining
        {
            get { return FPacketBuffer.Length - FPacketOffSet; }
        }

        public int PacketLength
        {
            get { return FPacketBuffer.Length; }
        }
        
        public bool SentByServer
        {
            get { return FSentByServer; }
            set { FSentByServer = value; }
        }

        #endregion

    }

}
