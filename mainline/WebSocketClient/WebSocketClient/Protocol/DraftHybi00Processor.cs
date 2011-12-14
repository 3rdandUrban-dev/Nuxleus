﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using SuperSocket.ClientEngine;

namespace SuperWebSocket.WebSocketClient.Protocol
{
    class DraftHybi00Processor : ProtocolProcessorBase
    {
        private static List<char> m_CharLib = new List<char>();
        private static List<char> m_DigLib = new List<char>();
        private static Random m_Random = new Random();

        public const byte StartByte = 0x00;
        public const byte EndByte = 0xFF;

        public static byte[] CloseHandshake = new byte[] { 0xFF, 0x00 };

        private byte[] m_ExpectedChallenge;

        static DraftHybi00Processor()
        {
            for (int i = 33; i <= 126; i++)
            {
                char currentChar = (char)i;

                if (char.IsLetter(currentChar))
                    m_CharLib.Add(currentChar);
                else if (char.IsDigit(currentChar))
                    m_DigLib.Add(currentChar);
            }
        }

        public override ReaderBase CreateHandshakeReader()
        {
            return new DraftHybi00HandshakeReader(WebSocket);
        }

        public override bool VerifyHandshake(WebSocketCommandInfo handshakeInfo)
        {
            var challenge = handshakeInfo.Data;

            if (challenge.Length != challenge.Length)
                return false;

            for (var i = 0; i < m_ExpectedChallenge.Length; i++)
            {
                if (challenge[i] != m_ExpectedChallenge[i])
                    return false;
            }

            return true;
        }

        public override void SendMessage(string message)
        {
            var maxByteCount = Encoding.UTF8.GetMaxByteCount(message.Length) + 2;
            var sendBuffer = new byte[maxByteCount];
            sendBuffer[0] = StartByte;
            int bytesCount = Encoding.UTF8.GetBytes(message, 0, message.Length, sendBuffer, 1);
            sendBuffer[1 + bytesCount] = EndByte;

            WebSocket.Send(sendBuffer, 0, bytesCount + 2);
        }

        public override void SendCloseHandshake(string closeReason)
        {
            WebSocket.Send(CloseHandshake, 0, CloseHandshake.Length);
        }

        public override void SendPing(string ping)
        {
            throw new NotImplementedException();
        }

        public override void SendHandshake()
        {
            string secKey1 = Encoding.UTF8.GetString(GenerateSecKey());

            string secKey2 = Encoding.UTF8.GetString(GenerateSecKey());

            byte[] secKey3 = GenerateSecKey(8);

            m_ExpectedChallenge = GetResponseSecurityKey(secKey1, secKey2, secKey3);

            var handshakeBuilder = new StringBuilder();

#if SILVERLIGHT
            handshakeBuilder.AppendLine(string.Format("GET {0} HTTP/1.1", WebSocket.TargetUri.GetPathAndQuery()));
#else
            handshakeBuilder.AppendLine(string.Format("GET {0} HTTP/1.1", WebSocket.TargetUri.PathAndQuery));
#endif

            handshakeBuilder.AppendLine("Upgrade: WebSocket");
            handshakeBuilder.AppendLine("Connection: Upgrade");
            handshakeBuilder.AppendLine(string.Format("Sec-WebSocket-Key1: {0}", secKey1));
            handshakeBuilder.AppendLine(string.Format("Sec-WebSocket-Key2: {0}", secKey2));
            handshakeBuilder.AppendLine(string.Format("Host: {0}", WebSocket.TargetUri.Host));
            handshakeBuilder.AppendLine(string.Format("Origin: {0}", WebSocket.TargetUri.Host));

            if (!string.IsNullOrEmpty(WebSocket.SubProtocol))
                handshakeBuilder.AppendLine(string.Format("Sec-WebSocket-Protocol: {0}", WebSocket.SubProtocol));

            handshakeBuilder.AppendLine();
            handshakeBuilder.Append(Encoding.UTF8.GetString(secKey3, 0, secKey3.Length));

            byte[] handshakeBuffer = Encoding.UTF8.GetBytes(handshakeBuilder.ToString());

            WebSocket.Send(handshakeBuffer, 0, handshakeBuffer.Length);
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
            byte[] b1 = BitConverter.GetBytes(k1FinalNum);
            Array.Reverse(b1);
            byte[] b2 = BitConverter.GetBytes(k2FinalNum);
            Array.Reverse(b2);
            byte[] b3 = secKey3;

            //Concatenating everything into 1 byte array for hashing.
            byte[] bChallenge = new byte[b1.Length + b2.Length + b3.Length];
            Array.Copy(b1, 0, bChallenge, 0, b1.Length);
            Array.Copy(b2, 0, bChallenge, b1.Length, b2.Length);
            Array.Copy(b3, 0, bChallenge, b1.Length + b2.Length, b3.Length);

            //Hash and return
            byte[] hash = MD5.Create().ComputeHash(bChallenge);
            return hash;
        }

        private byte[] GenerateSecKey()
        {
            int totalLen = m_Random.Next(10, 20);
            return GenerateSecKey(totalLen);
        }

        private byte[] GenerateSecKey(int totalLen)
        {
            int spaceLen = m_Random.Next(1, totalLen / 2 + 1);
            int charLen = m_Random.Next(3, totalLen - 1 - spaceLen);
            int digLen = totalLen - spaceLen - charLen;

            byte[] source = new byte[totalLen];

            var pos = 0;

            for (int i = 0; i < spaceLen; i++)
                source[pos++]  = (byte)' ';

            for (int i = 0; i < charLen; i++)
            {
                source[pos++] = (byte)m_CharLib[m_Random.Next(0, m_CharLib.Count - 1)];
            }

            for (int i = 0; i < digLen; i++)
            {
                source[pos++] = (byte)m_DigLib[m_Random.Next(0, m_DigLib.Count - 1)];
            }

            return source.RandomOrder();
        }
    }
}
