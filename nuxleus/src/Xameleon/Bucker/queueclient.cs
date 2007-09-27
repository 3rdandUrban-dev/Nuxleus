//
// queueclient.cs: Client API for the bucker queue system
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Xameleon.Bucker
{
  /// <summary>
  /// Wrapper carrying necessary information when using the asynchronous methods
  /// of the QueueClient class.
  /// </summary>
  internal class MessageState
  {
    private ManualResetEvent ev = null;
    private Socket sock = null;
    private byte[] buffer = new byte[1024];
    public StringBuilder sb = new StringBuilder();

    public MessageState(Socket sock) {
      this.sock = sock;
    }

    public MessageState(ManualResetEvent ev) {
      this.ev = ev;
    }

    public MessageState(Socket sock, ManualResetEvent ev) {
      this.ev = ev;
      this.sock = sock;
    }

    /// <summary>
    /// Gets or sets the connected client socket.
    /// </summary>
    public Socket Sock {
      get {
	return this.sock;
      }
      set {
	this.sock = value;
      }
    }

    /// <summary>
    /// Gets or sets the synchronisation event on which
    /// another thread is waiting on.
    /// </summary>
    public ManualResetEvent Event {
      get {
	return this.ev;
      }
    }

    /// <summary>
    /// Gets or sets the content read from the socket so far.
    /// </summary>
    public StringBuilder Data {
      get {
	return this.sb;
      }
      set {
	this.sb = value;
      }
    }

    /// <summary>
    /// Gets the buffer where the data was read into.
    /// </summary>
    public byte[] Buffer {
      get {
	return this.buffer;
      }
    }
  }

  /// <summary> 
  /// Manages a connection to a bucker queue.
  /// Allows to perform actions on a queue and its messages
  /// </summary>
  public class QueueClient
  {
    private IPEndPoint endpoint = null;
    private Socket _sock = null;

    public QueueClient(IPEndPoint ipe) {
      this.endpoint = ipe;
      this._sock = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    public QueueClient(string ip, int port) {
      this.endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
      this._sock = new Socket(this.endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    /// <summary> 
    /// Connect to the queue server.
    /// </summary>    
    public void Connect() {
      this._sock.Connect(this.endpoint);
    }

    /// <summary> 
    /// Gracefully disconnect from the queue server.
    /// </summary>   
    public void Disconnect() {
      if(this._sock != null){
	this._sock.Shutdown(SocketShutdown.Both);
	this._sock.Close();
	this._sock = null;
      }
    }

    /// <summary> 
    /// Send a message as an XML string.
    /// </summary>   
    /// <param name="xml">Message representation as an XML string.</param>
    public void Send(string xml) {
      byte[] buffer = System.Text.Encoding.UTF8.GetBytes(xml);
      this._sock.Send(buffer, buffer.Length, SocketFlags.None);
    }

    /// <summary> 
    /// Send a message instance.
    /// </summary>   
    /// <param name="m">Message instance.</param>
    public void Send(IMessage m) {
      this.Send(m.Serialize());
    }

    /// <summary> 
    /// Asynchronously send a message as an XML string.
    /// </summary>   
    /// <param name="xml">Message representation as an XML string.</param>
    /// <param name="ev">Synchronisation event used by the calling thread.</param>
    public void AsyncSend(string xml, ManualResetEvent ev) {
      byte[] buffer = System.Text.Encoding.ASCII.GetBytes(xml);
      MessageState ms = new MessageState(this._sock, ev);
      this._sock.BeginSend(buffer, buffer.Length, 0, SocketFlags.None,
			   new AsyncCallback(this.SendCallback), ms);
    }

    /// <summary> 
    /// Asynchronously send a message instance.
    /// </summary> 
    /// <param name="m">Message instance.</param>  
    /// <param name="ev">Synchronisation event used by the calling thread.</param>
    public void AsyncSend(IMessage m, ManualResetEvent ev) {
      this.AsyncSend(m.Serialize(), ev);
    }

    private void SendCallback(IAsyncResult ar) {
      MessageState st = (MessageState)ar.AsyncState;
      st.Sock.EndSend(ar);
      if(st.Event != null){
	st.Event.Set();
      }
    }
    
    /// <summary> 
    /// Read up to 1024 bytes from the queue.
    /// </summary>   
    /// <remarks>Block the caller execution.</remarks>
    /// <returns>Whatever could be read from the queue connection.</returns>
    public string Recv() { 
      return this.Recv(1024);
    }

    /// <summary> 
    /// Read up to the specified number of bytes from the queue.
    /// </summary>   
    /// <remarks>Block the caller execution.</remarks>
    /// <param name="size">Maximum to be read from the queue.</param>  
    /// <returns>Whatever could be read from the queue connection.</returns>
    public string Recv(int size) { 
      byte[] buffer = new byte[size];
  
      int res = this._sock.Receive(buffer, 0, size, SocketFlags.None);

      if(res > 0){
	return Encoding.ASCII.GetString(buffer, 0, res);
      }

      return null;
    }

    /// <summary> 
    /// Asynchronously read from the queue and put the content into 
    /// the provided StringBuilder.
    /// <param name="sb">Hold the read content.</param>  
    /// <param name="ev">Synchronisation event used by the calling thread.</param>  
    /// </summary>   
    public void AsyncRecv(StringBuilder sb, ManualResetEvent ev) {
      MessageState ms = new MessageState(this._sock, ev);
      ms.Data = sb;
      this._sock.BeginReceive(ms.Buffer, 0, 1024, 0,
			      new AsyncCallback(this.ReceiveCallback), ms);
    }

    private void ReceiveCallback(IAsyncResult ar) {
      MessageState ms = (MessageState) ar.AsyncState;
      int bytesRead = ms.Sock.EndReceive(ar);

      if (bytesRead > 0) {
            ms.Data.Append(Encoding.ASCII.GetString(ms.Buffer, 0, bytesRead));
	    ms.Sock.BeginReceive(ms.Buffer, 0, 1024, 0, new AsyncCallback(ReceiveCallback), ms);
      } else {
	if(ms.Event != null){
	  ms.Event.Set();
	}
      }
    }

  }
}