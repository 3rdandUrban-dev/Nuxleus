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
using System.Security;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.Serialization;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ALAZ.SystemEx.NetEx.SocketsEx
{

    /// <summary>
    /// Connection creator using in BaseSocketConnectionHost.
    /// </summary>
    public abstract class BaseSocketConnectionCreator : BaseDisposable
    {

        #region Fields

        //----- Local endpoint of creator!
        private IPEndPoint FLocalEndPoint;

        //----- Host!
        private BaseSocketConnectionHost FHost;
        private string FName;

        private EncryptType FEncryptType;
        private CompressionType FCompressionType;

        private ICryptoService FCryptoService;

        #endregion

        #region Constructor

        public BaseSocketConnectionCreator(BaseSocketConnectionHost host, string name, IPEndPoint localEndPoint, EncryptType encryptType, CompressionType compressionType, ICryptoService cryptoService)
        {

            FHost = host;
            FName = name;
            FLocalEndPoint = localEndPoint;
            FCompressionType = compressionType;
            FEncryptType = encryptType;

            FCryptoService = cryptoService;

        }

        #endregion

        #region Destructor

        protected override void Free(bool canAccessFinalizable)
        {

            FLocalEndPoint = null;
            FCryptoService = null;
            FHost = null;

            base.Free(canAccessFinalizable);

        }

        #endregion

        #region Methods

        #region InitializeConnection

        /// <summary>
        /// Initializes the connection with encryption.
        /// </summary>
        /// <param name="connection"></param>
        protected virtual void InitializeConnection(object state)
        {

            if (!Disposed)
            {

                BaseSocketConnection connection = (BaseSocketConnection)state;

                if (FCryptoService != null)
                {
                    InitializeCryptService(connection);
                }
                else
                {
                    //----- No encryption!
                    FHost.FireOnConnected(connection);
                }

            }
        }

        #endregion

        #region InitializeCryptService

        protected void InitializeCryptService(BaseSocketConnection connection)
        { 

          //----- None!
          if (connection.EncryptType == EncryptType.etNone || connection.EncryptType == EncryptType.etBase64)
          {
              FHost.FireOnConnected(connection);
          }

          //----- Symmetric!
          if (connection.EncryptType == EncryptType.etRijndael || connection.EncryptType == EncryptType.etTripleDES)
          {

              if (FHost.HostType == HostType.htClient)
              {

                  //----- Get RSA provider!
                  RSACryptoServiceProvider serverPublicKey;
                  RSACryptoServiceProvider clientPrivateKey = new RSACryptoServiceProvider();
                  byte[] signMessage;

                  FCryptoService.OnSymmetricAuthenticate(connection, out serverPublicKey, out signMessage);

                  //----- Generates symmetric algoritm!
                  SymmetricAlgorithm sa = CryptUtils.CreateSymmetricAlgoritm(connection.EncryptType);
                  sa.GenerateIV();
                  sa.GenerateKey();

                  //----- Adjust connection cryptors!
                  connection.Encryptor = sa.CreateEncryptor();
                  connection.Decryptor = sa.CreateDecryptor();

                  //----- Create authenticate structure!
                  AuthMessage am = new AuthMessage();
                  am.SessionIV = serverPublicKey.Encrypt(sa.IV, false);
                  am.SessionKey = serverPublicKey.Encrypt(sa.Key, false);
                  am.SourceKey = CryptUtils.EncryptDataForAuthenticate(sa, Encoding.UTF8.GetBytes(clientPrivateKey.ToXmlString(false)), PaddingMode.ISO10126);

                  //----- Sign message with am.SourceKey, am.SessionKey and signMessage!
                  //----- Need to use PaddingMode.PKCS7 in sign!
                  MemoryStream m = new MemoryStream();
                  m.Write(am.SourceKey, 0, am.SourceKey.Length);
                  m.Write(am.SessionKey, 0, am.SessionKey.Length);
                  m.Write(signMessage, 0, signMessage.Length);
                  
                  am.Sign = clientPrivateKey.SignData(CryptUtils.EncryptDataForAuthenticate(sa, m.ToArray(), PaddingMode.PKCS7), new SHA1CryptoServiceProvider());

                  //----- Serialize authentication message!
                  XmlSerializer xml = new XmlSerializer(typeof(AuthMessage));
                  m.SetLength(0);
                  xml.Serialize(m, am);

                  //----- Send structure!
                  MessageBuffer mb = new MessageBuffer(0);
                  mb.PacketBuffer = Encoding.GetEncoding(1252).GetBytes(Convert.ToBase64String(m.ToArray()));
                  connection.Socket.BeginSend(mb.PacketBuffer, mb.PacketOffSet, mb.PacketRemaining, SocketFlags.None, new AsyncCallback(InitializeConnectionSendCallback), new CallbackData(connection, mb));

                  m.Close();
                  am.SessionIV.Initialize();
                  am.SessionKey.Initialize();
                  serverPublicKey.Clear();
                  clientPrivateKey.Clear();

              }
              else
              {

                  //----- Create empty authenticate structure!
                  MessageBuffer mb = new MessageBuffer(8192);

                  //----- Start receive structure!
                  connection.Socket.BeginReceive(mb.PacketBuffer, mb.PacketOffSet, mb.PacketRemaining, SocketFlags.None, new AsyncCallback(InitializeConnectionReceiveCallback), new CallbackData(connection, mb));

              }

          }

          //----- Asymmetric!
          if (connection.EncryptType == EncryptType.etSSL)
          {

              if (FHost.HostType == HostType.htClient)
              {

                  //----- Get SSL items!
                  X509Certificate2Collection certs = null;
                  string serverName = null;
                  bool checkRevocation = true;

                  FCryptoService.OnSSLClientAuthenticate(connection, out serverName, ref certs, ref checkRevocation);

                  //----- Authneticate SSL!
                  SslStream ssl = new SslStream(new NetworkStream(connection.Socket), true, new RemoteCertificateValidationCallback(ValidateServerCertificateCallback)); 

                  if (certs == null)
                  {
                      ssl.BeginAuthenticateAsClient(serverName, new AsyncCallback(SslAuthenticateCallback), new AuthenticateCallbackData(connection, ssl, HostType.htClient));
                  }
                  else
                  {
                      ssl.BeginAuthenticateAsClient(serverName, certs, System.Security.Authentication.SslProtocols.Default, checkRevocation, new AsyncCallback(SslAuthenticateCallback), new AuthenticateCallbackData(connection, ssl, HostType.htClient));

                  }

              }
              else
              {

                  //----- Get SSL items!
                  X509Certificate2 cert = null;
                  bool clientAuthenticate = false;
                  bool checkRevocation = true;

                  FCryptoService.OnSSLServerAuthenticate(connection, out cert, out clientAuthenticate, ref checkRevocation);

                  //----- Authneticate SSL!
                  SslStream ssl = new SslStream(new NetworkStream(connection.Socket));
                  ssl.BeginAuthenticateAsServer(cert, clientAuthenticate, System.Security.Authentication.SslProtocols.Default, checkRevocation, new AsyncCallback(SslAuthenticateCallback), new AuthenticateCallbackData(connection, ssl, HostType.htServer));

              }

          }

        }

        #endregion

        #region InitializeConnectionSendCallback

        private void InitializeConnectionSendCallback(IAsyncResult ar)
        {

            if (!Disposed)
            {

                BaseSocketConnection connection = null;
                MessageBuffer writeMessage = null;

                try
                {

                    CallbackData callbackData = (CallbackData)ar.AsyncState;

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
                            connection.Socket.BeginSend(writeMessage.PacketBuffer, writeMessage.PacketOffSet, writeMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(InitializeConnectionSendCallback), callbackData);
                        }
                        else
                        {
                          
                          writeMessage = null;
                          callbackData = null;

                          FHost.FireOnConnected(connection);

                        }

                    }

                }
                catch (Exception ex)
                {
                    FHost.FireOnException(connection, ex);
                }

            }


        }

        #endregion

        #region InitializeConnectionReceiveCallback

        private void InitializeConnectionReceiveCallback(IAsyncResult ar)
        {

            if (!Disposed)
            {

                BaseSocketConnection connection = null;
                MessageBuffer readMessage = null;

                try
                {

                    CallbackData callbackData = (CallbackData)ar.AsyncState;

                    connection = callbackData.Connection;
                    readMessage = callbackData.Buffer;

                    if (connection.Active)
                    {

                        bool readSocket = true;
                        bool completed = false;

                        int readBytes = connection.Socket.EndReceive(ar);

                        if (readBytes > 0)
                        {

                            readMessage.PacketOffSet += readBytes;
                            byte[] message = null;

                            try
                            {
                                message = Convert.FromBase64String(Encoding.GetEncoding(1252).GetString(readMessage.PacketBuffer, 0, readMessage.PacketOffSet));
                            }
                            catch (FormatException)
                            {
                                //----- Base64 transformation error!
                            }

                            if ((message != null) && (Encoding.GetEncoding(1252).GetString(message).Contains("</AuthMessage>")))
                            {

                                //----- Get RSA provider!
                                RSACryptoServiceProvider serverPrivateKey;
                                RSACryptoServiceProvider clientPublicKey = new RSACryptoServiceProvider();
                                byte[] signMessage;

                                FCryptoService.OnSymmetricAuthenticate(connection, out serverPrivateKey, out signMessage);

                                //----- Deserialize authentication message!
                                MemoryStream m = new MemoryStream();
                                m.Write(message, 0, message.Length);
                                m.Position = 0;

                                XmlSerializer xml = new XmlSerializer(typeof(AuthMessage));
                                AuthMessage am = (AuthMessage)xml.Deserialize(m);

                                //----- Generates symmetric algoritm!
                                SymmetricAlgorithm sa = CryptUtils.CreateSymmetricAlgoritm(connection.EncryptType);
                                sa.Key = serverPrivateKey.Decrypt(am.SessionKey, false);
                                sa.IV = serverPrivateKey.Decrypt(am.SessionIV, false);

                                //----- Adjust connection cryptors!
                                connection.Encryptor = sa.CreateEncryptor();
                                connection.Decryptor = sa.CreateDecryptor();

                                //----- Verify sign!
                                clientPublicKey.FromXmlString(Encoding.UTF8.GetString(CryptUtils.DecryptDataForAuthenticate(sa, am.SourceKey, PaddingMode.ISO10126)));

                                m.SetLength(0);
                                m.Write(am.SourceKey, 0, am.SourceKey.Length);
                                m.Write(am.SessionKey, 0, am.SessionKey.Length);
                                m.Write(signMessage, 0, signMessage.Length);

                                if (clientPublicKey.VerifyData(CryptUtils.EncryptDataForAuthenticate(sa, m.ToArray(), PaddingMode.PKCS7), new SHA1CryptoServiceProvider(), am.Sign))
                                {
                                    completed = true;
                                }

                                readSocket = false;

                                m.Close();
                                am.SessionIV.Initialize();
                                am.SessionKey.Initialize();
                                serverPrivateKey.Clear();
                                clientPublicKey.Clear();

                                readMessage = null;
                                callbackData = null;

                                if (!completed)
                                {
                                    throw new SymmetricAuthenticationException("Symmetric sign error.");
                                }

                                FHost.FireOnConnected(connection);

                            }

                            if (readSocket)
                            {
                                connection.Socket.BeginReceive(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(InitializeConnectionReceiveCallback), callbackData);
                            }

                        }
                        else
                        {
                            throw new SymmetricAuthenticationException("Symmetric authentication error.");
                        }

                    }

                }
                catch (Exception ex)
                {
                    FHost.FireOnException(connection, ex);
                }

            }


        }

        #endregion

        #region SslAuthenticateCallback

        private void SslAuthenticateCallback(IAsyncResult ar)
        {

            if (!Disposed)
            {

                BaseSocketConnection connection = null;
                SslStream stream = null;
                bool completed = false;

                try
                {

                    AuthenticateCallbackData callbackData = (AuthenticateCallbackData)ar.AsyncState;

                    connection = callbackData.Connection;
                    stream = callbackData.Stream;

                    if (connection.Active)
                    {

                        if (callbackData.HostType == HostType.htClient)
                        {
                            stream.EndAuthenticateAsClient(ar);
                        }
                        else
                        {
                            stream.EndAuthenticateAsServer(ar);
                        }

                        if ((stream.IsSigned && stream.IsEncrypted))
                        {
                            completed = true;
                        }

                        callbackData = null;  
                        connection.Stream = stream;

                        if (!completed)
                        {
                            throw new SSLAuthenticationException("Ssl authenticate is not signed or not encrypted.");
                        }

                        FHost.FireOnConnected(connection);

                    }

                }
                catch (Exception ex)
                {
                    FHost.FireOnException(connection, ex);
                }

            }

        }

        #endregion

        #region ValidateServerCertificateCallback

        private bool ValidateServerCertificateCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {

            bool acceptCertificate = false;
            FCryptoService.OnSSLClientValidateServerCertificate(certificate, chain, sslPolicyErrors, out acceptCertificate);
            
            return acceptCertificate;

        }

        #endregion

        #region Abstract Methods

        public abstract void Start();
        public abstract void Stop();

        #endregion

        #endregion

        #region Properties

        internal BaseSocketConnectionHost Host
        {
            get { return FHost; }
        }

        public string Name
        {
          get { return FName; }
        }

        public ICryptoService CryptoService
        {
            get { return FCryptoService; }
            set { FCryptoService = value; } 
        }

        public EncryptType EncryptType
        {
            get { return FEncryptType; }
            set { FEncryptType = value; }
        }

        internal IPEndPoint InternalLocalEndPoint
        {
            get { return FLocalEndPoint; }
            set { FLocalEndPoint = value; }
        }

        public CompressionType CompressionType
        {
            get { return FCompressionType; }
            set { FCompressionType = value; }
        }

        #endregion

    }

}
