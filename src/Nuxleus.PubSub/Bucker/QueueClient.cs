//
// queueclient.cs: Client API for the bucker queue system
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
// For some of the exceptions we catch
using System.Reflection;
using System.Xml;

namespace Nuxleus.Bucker {

    public delegate void MessageEventHandler ( object sender, MessageStateEventArgs e );

    public class MessageStateEventArgs : EventArgs {

        private Message message = null;

        public MessageStateEventArgs () { }

        public MessageStateEventArgs ( string xml ) {
            try {
                message = Message.Parse(xml);
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

        public MessageStateEventArgs ( Message m ) {
            message = m;
        }

        /// <summary>Represents the message received</summary>
        public Message Message {
            get { return message; }
        }
    }

    /// <summary>
    /// Wrapper carrying necessary information when using the asynchronous methods
    /// of the QueueClient class.
    /// </summary>
    public class MessageEvent {
        private int bufSize = 4096;
        private QueueClient client = null;
        private byte[] buffer = new byte[4096];
        public StringBuilder sb = new StringBuilder();
        private bool dismiss = false;
        private Message message = null;

        public MessageEvent () { }

        public event MessageEventHandler MessageReceived = null;

        public void OnMessageReceived ( MessageStateEventArgs e ) {
            if (MessageReceived != null) {
                MessageReceived(this, e);
            }
        }

        /// <summary>Represents the message sent</summary>
        public Message Message {
            get { return message; }
            set { message = value; }
        }

        /// <summary>
        /// Gets or sets the connected client handling this message event.
        /// </summary>
        public QueueClient Client {
            get { return this.client; }
            set { this.client = value; }
        }

        /// <summary>
        /// Gets or sets the content read from the socket so far.
        /// </summary>
        public StringBuilder Data {
            get { return this.sb; }
            set { this.sb = value; }
        }

        /// <summary>
        /// Gets the buffer where the data was read into.
        /// </summary>
        public byte[] Buffer {
            get { return this.buffer; }
        }

        /// <summary>
        /// Allows for the response to be read from the socket but dismissed immediatly
        /// which is useful when you do not need to look at the response.
        /// Note that this means that the data read won't be stored into the Buffer
        /// and the OnResponseReceived handler will not be called either.
        /// </summary>
        public bool Dismiss {
            get { return dismiss; }
            set { dismiss = value; }
        }

        public int BufferSize {
            get { return bufSize; }
            set {
                bufSize = value;
                buffer = new byte[bufSize];
            }
        }

    }

    public delegate void QueueClientEventHandler ( object sender, QueueClientEventArgs e );

    public class QueueClientEventArgs : EventArgs {
    }

    /// <summary> 
    /// Manages a connection to a bucker queue.
    /// Allows to perform actions on a queue and its messages
    /// </summary>
    public class QueueClient {
        private IPEndPoint endpoint = null;
        private Socket _sock = null;
        private bool completed = false;

        public QueueClient ( IPEndPoint ipe ) {
            this.endpoint = ipe;
            this._sock = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public QueueClient ( string ip, int port ) {
            this.endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            this._sock = new Socket(this.endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public event QueueClientEventHandler ConnectionClosed = null;

        public void OnConnectionClosed ( QueueClientEventArgs e ) {
            if (ConnectionClosed != null) {
                ConnectionClosed(this, e);
            }
        }

        /// <summary> 
        /// Connect to the queue server.
        /// </summary>    
        public void Connect () {
            this._sock.Connect(this.endpoint);
        }

        /// <summary> 
        /// Gracefully disconnect from the queue server.
        /// </summary>   
        public void Disconnect () {
            if (this._sock != null) {
                this._sock.Shutdown(SocketShutdown.Both);
                this._sock.Close();
                this._sock = null;
            }
        }

        public bool IsCompleted {
            get { return completed; }
        }

        public Socket Socket {
            get { return this._sock; }
        }

        /// <summary> 
        /// Send a message as an XML string.
        /// </summary>   
        /// <param name="xml">Message representation as an XML string.</param>
        public void Send ( string xml ) {
            completed = false;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(xml);
            this._sock.Send(buffer, buffer.Length, SocketFlags.None);
        }

        /// <summary> 
        /// Send a message instance.
        /// </summary>   
        /// <param name="m">Message instance.</param>
        public void Send ( Message m ) {
            this.Send(m.ToString());
        }

        public static void AsyncSend ( MessageEvent me ) {
            if (me.Client == null) {
                throw new InvalidOperationException("The 'Client' property must be set");
            }
            string message = String.Format("{0}\r\n\r\n", me.Message.ToString());
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            me.Client.Socket.BeginSend(data, 0, data.Length, 0,
                       new AsyncCallback(SendCallback), me);
        }

        private static void SendCallback ( IAsyncResult ar ) {
            MessageEvent me = (MessageEvent)ar.AsyncState;
            int bytesSent = me.Client.Socket.EndSend(ar);
        }

        /// <summary> 
        /// Read up to 1024 bytes from the queue.
        /// </summary>   
        /// <remarks>Block the caller execution.</remarks>
        /// <returns>Whatever could be read from the queue connection.</returns>
        public string Recv () {
            return this.Recv(1024);
        }

        /// <summary> 
        /// Read up to the specified number of bytes from the queue.
        /// </summary>   
        /// <remarks>Block the caller execution.</remarks>
        /// <param name="size">Maximum to be read from the queue.</param>  
        /// <returns>Whatever could be read from the queue connection.</returns>
        public string Recv ( int size ) {
            byte[] buffer = new byte[size];

            int res = this._sock.Receive(buffer, 0, size, SocketFlags.None);

            completed = true;

            if (res > 0) {
                return Encoding.ASCII.GetString(buffer, 0, res);
            }

            return null;
        }

        /// <summary> 
        /// Asynchronously read from the queue and put the content into 
        /// the provided MessageState.
        /// </summary>   
        public static void AsyncRecv ( MessageEvent ms ) {
            if (ms.Client == null) {
                throw new InvalidOperationException("The 'Client' property must be set");
            }
            ms.Client.Socket.BeginReceive(ms.Buffer, 0, ms.BufferSize, 0,
                          new AsyncCallback(ReceiveCallback), ms);
        }

        private static void ReceiveCallback ( IAsyncResult ar ) {
            MessageEvent ms = (MessageEvent)ar.AsyncState;

            int bytesRead = ms.Client.Socket.EndReceive(ar);

            if (bytesRead > 0) {
                if (!ms.Dismiss) {
                    ms.Data.Append(Encoding.UTF8.GetString(ms.Buffer, 0, bytesRead));
                    try {
                        char[] charsToTrim = { '\r', '\n' };
                        Message m = Message.Parse(ms.Data.ToString().TrimEnd(charsToTrim));
                        ms.OnMessageReceived(new MessageStateEventArgs(m));
                    } catch (XmlException xe) {
                        // Basically this means the data we have read was not yet the full response
                        // so we continue waiting...
                        ms.Client.Socket.BeginReceive(ms.Buffer, 0, ms.BufferSize, 0,
                                      new AsyncCallback(ReceiveCallback), ms);
                    } catch (TargetInvocationException te) {
                        // I'm not entirely sure why this error is raised
                        // but trapping it seems to do just fine
                        ms.Client.Socket.BeginReceive(ms.Buffer, 0, ms.BufferSize, 0,
                                      new AsyncCallback(ReceiveCallback), ms);
                    }
                }
            } else {
                ms.Client.OnConnectionClosed(new QueueClientEventArgs());
            }
        }
    }
}