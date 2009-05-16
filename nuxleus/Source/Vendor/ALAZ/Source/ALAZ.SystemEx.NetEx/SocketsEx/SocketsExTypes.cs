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
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace ALAZ.SystemEx.NetEx.SocketsEx
{

    //----- SocketsEx declarations!

    #region Delegates

    public delegate void OnDisconnectEvent();
    public delegate void OnSymmetricAuthenticateEvent(ISocketConnection connection, out RSACryptoServiceProvider serverKey, out byte[] signMessage);
    public delegate void OnSSLClientAuthenticateEvent(ISocketConnection connection, out string ServerName, ref X509Certificate2Collection certs, ref bool checkRevocation);
    public delegate void OnSSLServerAuthenticateEvent(ISocketConnection connection, out X509Certificate2 certificate, out bool clientAuthenticate, ref bool checkRevocation);

    #endregion

    #region Exceptions

    /// <summary>
    /// Max reconnect attempts reached.
    /// </summary>
    public class ReconnectAttemptsException : Exception
    {
        public ReconnectAttemptsException(string message) : base(message) { }
    }

    /// <summary>
    /// Bad Delimiter.
    /// </summary>
    public class BadDelimiterException : Exception
    {
        public BadDelimiterException(string message) : base(message) { }
    }

    /// <summary>
    /// Message length is greater than the maximum value.
    /// </summary>
    public class MessageLengthException : Exception
    {
        public MessageLengthException(string message): base(message) { }
    }

    /// <summary>
    /// Symmetric authentication failure.
    /// </summary>
    public class SymmetricAuthenticationException: Exception
    {
        public SymmetricAuthenticationException(string message) : base(message) { } 
    }

    /// <summary>
    /// SSL authentication failure.
    /// </summary>
    public class SSLAuthenticationException : Exception
    {
        public SSLAuthenticationException(string message) : base(message) { }
    }

    /// <summary>
    /// Proxy authentication failure.
    /// </summary>
    public class ProxyAuthenticationException :  HttpException
    {

      public ProxyAuthenticationException(int code, string message) : base(code, message) { }

    }


    #endregion 

    #region Structures

    #region AuthMessage

    public struct AuthMessage
    {
        public byte[] SessionKey;
        public byte[] SessionIV;
        public byte[] SourceKey;
        public byte[] Sign;
    }

    #endregion

    #endregion

    #region Enums

    #region HostType

    /// <summary>
    /// Defines the host type.
    /// </summary>
    public enum HostType
    {
        htServer,
        htClient
    }

    #endregion

    #region EncryptType

    /// <summary>
    /// Defines the encrypt method used.
    /// </summary>
    public enum EncryptType
    {
        etNone,
        etBase64,
        etTripleDES,
        etRijndael,
        etSSL,
    }

    #endregion

    #region CompressionType

    /// <summary>
    /// Defines the compression method used.
    /// </summary>
    public enum CompressionType
    {
        ctNone,
        ctGZIP
    }

    #endregion

    #region DelimiterType

    /// <summary>
    /// Defines message delimiter type.
    /// </summary>
    public enum DelimiterType
    {
        dtNone,
        dtPacketHeader,
        dtMessageTailExcludeOnReceive,
        dtMessageTailIncludeOnReceive
    }

    #endregion

    #region ProxyType

    /// <summary>
    /// Defines the proxy host type.
    /// </summary>
    public enum ProxyType
    {
      ptSOCKS4,
      ptSOCKS4a,
      ptSOCKS5,
      ptHTTP
    }

    #endregion

    #region IpType

    public enum IpType
    {
        itIpV4,
        itIpV6
    }

    #endregion

    #region SOCKS5AuthMode
		 
    /// <summary>
    /// Defines the SOCK5 authentication mode.
    /// </summary>
    internal enum SOCKS5AuthMode
    {
      saNoAuth = 0,
      ssUserPass = 2
    }

  	#endregion

    #region SOCKS5Phase
		 
	  /// <summary>
    /// Defines the SOCKS5 authentication phase
    /// </summary>
    internal enum SOCKS5Phase
    {

      spIdle,
      spGreeting,
      spAuthenticating,
      spConnecting

    }

    #endregion
  
    #endregion

    #region Interfaces

    #region ISocketConnection
    
    #region ISocketConnectionInfo
    
    public interface ISocketConnectionInfo
    {

        /// <summary>
        /// Connection custom data.
        /// </summary>
        object CustomData
        {
            get;
            set;
        }

        /// <summary>
        /// Connection Session Id.
        /// </summary>
        long ConnectionId
        {
            get;
        }

        /// <summary>
        /// Connection Creator object.
        /// </summary>
        BaseSocketConnectionCreator Creator
        {
            get;
        }

        /// <summary>
        /// Connection Host object.
        /// </summary>
        BaseSocketConnectionHost Host
        {
            get;
        }

        /// <summary>
        /// Handle of the OS Socket.
        /// </summary>
        IntPtr SocketHandle
        {
            get;
        }

        /// <summary>
        /// Local socket endpoint.
        /// </summary>
        IPEndPoint LocalEndPoint
        {
            get;
        }

        /// <summary>
        /// Remote socket endpoint.
        /// </summary>
        IPEndPoint RemoteEndPoint
        {
            get;
        }
        
    }
    
    #endregion

    #region ISocketConnection

    /// <summary>
    /// Common connection properties and methods.
    /// </summary>
    public interface ISocketConnection : ISocketConnectionInfo
    {

        /// <summary>
        /// Set Socket Time To Live option
        /// </summary>
        /// <param name="value">
        /// Value for TTL in seconds
        /// </param>
        void SetTTL(short value);
        /// <summary>
        /// Set Socket Linger option.
        /// </summary>
        /// <param name="lo">
        /// LingerOption value to be set
        /// </param>
        void SetLinger(LingerOption lo);
        /// <summary>
        /// Set Socket Nagle algoritm.
        /// </summary>
        /// <param name="value">
        /// Enable/Disable value
        /// </param>
        void SetNagle(bool value);
      
        /// <summary>
        /// Represents the connection as a IClientSocketConnection.
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        IClientSocketConnection AsClientConnection();

        /// <summary>
        /// Represents the connection as a IServerSocketConnection.
        /// </summary>
        /// <returns></returns>
        IServerSocketConnection AsServerConnection();

        /// <summary>
        /// Begin send data.
        /// </summary>
        /// <param name="buffer">
        /// Data to be sent.
        /// </param>
        void BeginSend(byte[] buffer);

        /// <summary>
        /// Begin receive the data.
        /// </summary>
        void BeginReceive();

        /// <summary>
        /// Begin disconnect the connection.
        /// </summary>
        void BeginDisconnect();

    }

    #endregion

    #region IClientSocketConnection

    /// <summary>
    /// Client connection methods.
    /// </summary>
    public interface IClientSocketConnection: ISocketConnection
    {

        /// <summary>
        /// Proxy information.
        /// </summary>
        ProxyInfo ProxyInfo
        {
          get;
        }

        /// <summary>
        /// Begin reconnect the connection.
        /// </summary>
        void BeginReconnect();
    }

    #endregion

    #region IServerSocketConnection

    /// <summary>
    /// Server connection methods.
    /// </summary>
    public interface IServerSocketConnection: ISocketConnection
    {

        /// <summary>
        /// Begin send data to all server connections.
        /// </summary>
        /// <param name="buffer">
        /// Data to be sent.
        /// </param>
        /// <param name="includeMe">
        /// Includes the current connection in send´s loop
        /// </param>
        void BeginSendToAll(byte[] buffer, bool includeMe);

        /// <summary>
        /// Begin send data to the connection.
        /// </summary>
        /// <param name="connection">
        /// The connection that the data will be sent.
        /// </param>
        /// <param name="buffer">
        /// Data to be sent.
        /// </param>
        void BeginSendTo(ISocketConnection connection, byte[] buffer);

        /// <summary>
        /// Get the connection from the connectionId.
        /// </summary>
        /// <param name="connectionId">
        /// The connectionId.
        /// </param>
        /// <returns>
        /// ISocketConnection to use.
        /// </returns>
        ISocketConnection GetConnectionById(long connectionId);

        /// <summary>
        /// Get all the connections.
        /// </summary>
        ISocketConnection[] GetConnections();

    }

    #endregion

    #endregion

    #region ISocketService

    /// <summary>
    /// Socket service methods.
    /// </summary>
    public interface ISocketService
    {
        /// <summary>
        /// Fired when connected.
        /// </summary>
        /// <param name="e">
        /// Information about the connection.
        /// </param>
        void OnConnected(ConnectionEventArgs e);

        /// <summary>
        /// Fired when data arrives.
        /// </summary>
        /// <param name="e">
        /// Information about the Message.
        /// </param>
        void OnReceived(MessageEventArgs e);

        /// <summary>
        /// Fired when data is sent.
        /// </summary>
        /// <param name="e">
        /// Information about the Message.
        /// </param>
        void OnSent(MessageEventArgs e);

        /// <summary>
        /// Fired when disconnected.
        /// </summary>
        /// <param name="e">
        /// Information about the connection.
        /// </param>
        void OnDisconnected(ConnectionEventArgs e);

        /// <summary>
        /// Fired when exception occurs.
        /// </summary>
        /// <param name="e">
        /// Information about the exception and connection.
        /// </param>
        void OnException(ExceptionEventArgs e);

    }

    #endregion

    #region ICryptoService

    /// <summary>
    /// Crypto service methods.
    /// </summary>
    public interface ICryptoService
    {
        
        /// <summary>
        /// Fired when symmetric encryption is used.
        /// </summary>
        /// <param name="serverKey">
        /// The RSA provider used to encrypt symmetric IV and Key.
        /// </param>
        /// 

        void OnSymmetricAuthenticate(ISocketConnection connection, out RSACryptoServiceProvider serverKey, out byte[] signMessage);

        /// <summary>
        /// Fired when SSL encryption is used in client host.
        /// </summary>
        /// <param name="ServerName">
        /// The host name in certificate.
        /// </param>
        /// <param name="certs">
        /// The certification collection to be used (null if not using client certification).
        /// </param>
        /// <param name="checkRevocation">
        /// Indicates if the certificated must be checked for revocation.
        /// </param>
        void OnSSLClientAuthenticate(ISocketConnection connection, out string ServerName, ref X509Certificate2Collection certs, ref bool checkRevocation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="serverCertificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <param name="acceptCertificate"></param>
        void OnSSLClientValidateServerCertificate(X509Certificate serverCertificate, X509Chain chain, SslPolicyErrors sslPolicyErrors, out bool acceptCertificate);

        /// <summary>
        /// Fired when SSL encryption is used in server host.
        /// </summary>
        /// <param name="certificate">
        /// The certificate to be used.
        /// </param>
        /// <param name="clientAuthenticate">
        /// Indicates if client connection will be authenticated (uses certificate).
        /// </param>
        /// <param name="checkRevocation">
        /// Indicates if the certificated must be checked for revocation.
        /// </param>
        void OnSSLServerAuthenticate(ISocketConnection connection, out X509Certificate2 certificate, out bool clientAuthenticate, ref bool checkRevocation);

    }

    #endregion

    #endregion

    #region Classes

    #region BaseSocketService

    /// <summary>
    /// Base class for ISocketServive. Use it overriding the virtual methods.
    /// </summary>
    public abstract class BaseSocketService : ISocketService
    {

        #region ISocketService Members

        public virtual void OnConnected(ConnectionEventArgs e) { }
        public virtual void OnSent(MessageEventArgs e) { }
        public virtual void OnReceived(MessageEventArgs e) { }
        public virtual void OnDisconnected(ConnectionEventArgs e) { }
        public virtual void OnException(ExceptionEventArgs e) { }

        #endregion

    }

    #endregion

    #region BaseCryptoService

    /// <summary>
    /// Base class for ICryptoServive. Use it overriding the virtual methods.
    /// </summary>
    public abstract class BaseCryptoService : ICryptoService
    {

        #region ICryptoService Members

        public virtual void OnSymmetricAuthenticate(ISocketConnection connection, out RSACryptoServiceProvider serverKey, out byte[] signMessage)
        {

            serverKey = new RSACryptoServiceProvider();
            serverKey.Clear();

            signMessage = new byte[0];

        }

        public virtual void OnSSLClientAuthenticate(ISocketConnection connection, out string serverName, ref X509Certificate2Collection certs, ref bool checkRevocation)
        {
            serverName = String.Empty;
        }

        public virtual void OnSSLServerAuthenticate(ISocketConnection connection, out X509Certificate2 certificate, out bool clientAuthenticate, ref bool checkRevocation)
        {
            certificate = new X509Certificate2();
            clientAuthenticate = true;
        }

        public virtual void OnSSLClientValidateServerCertificate(X509Certificate serverCertificate, X509Chain chain, SslPolicyErrors sslPolicyErrors, out bool acceptCertificate)
        {
            acceptCertificate = false;    
        }

        #endregion

    }

    #endregion

    #endregion

}