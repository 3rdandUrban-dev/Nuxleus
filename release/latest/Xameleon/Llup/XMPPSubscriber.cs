using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using agsXMPP;
using agsXMPP.protocol.client;

namespace Xameleon.Llup
{
  internal class MessageState {

    private ManualResetEvent ev = null;
    private byte[] buffer = new byte[4096];
    private StringBuilder sb = new StringBuilder();

    public MessageState() {}

    public MessageState(ManualResetEvent ev) {
      this.ev = ev;
    }

    public StringBuilder Data {
      get {
	return this.sb;
      }
      set {
	this.sb = value;
      }
    }

    public ManualResetEvent Event {
      get {
	return this.ev;
      }
    }

    public byte[] Buffer {
      get {
	return this.buffer;
      }
    }
  }

  public class XMPPSubscriber{
    private Socket peer = null;
    private XmppClientConnection xmpp = null;
    
    public XMPPSubscriber() {}

    public XmppClientConnection XMPP {
      get {
	return this.xmpp;
      }
      set {
	this.xmpp = value;
      }
    }

    public void ConnectToPeer(IPEndPoint ipe) {
      this.peer = new Socket(ipe.AddressFamily, SocketType.Stream, 
			     ProtocolType.Tcp);
      this.peer.Connect(ipe);
    }

    public void ConnectToPeer(string ip, int port) {
      ConnectToPeer(new IPEndPoint(IPAddress.Parse(ip), port));
    }
  
    public void DisconnectFromPeer() {
      if(this.peer != null) {
	this.peer.Shutdown(SocketShutdown.Both);
	this.peer.Close();
	this.peer = null;
      }
    }
    
    public void Connect(string peerIp, int peerPort, string server, string connectServer, string username, string password) {

      this.xmpp = new  XmppClientConnection();
      this.xmpp.Server = server;
      this.xmpp.ConnectServer = connectServer;
      this.xmpp.Username = username;
      this.xmpp.Password = password;
      this.xmpp.Open();
      xmpp.OnLogin += delegate(object o) { 
	Console.WriteLine("Logged in");
	this.ConnectToPeer(peerIp, peerPort);
	Console.WriteLine("Connected to LLUP");
	this.AsyncRecv();
// 	byte[] buffer = new byte[4096];
  
// 	int res = this.peer.Receive(buffer, 0, 4096, SocketFlags.None);
// 	Console.WriteLine(res);
// 	Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, res));
      };
    }

    public void Disconnect() {
      this.DisconnectFromPeer();
      if(this.xmpp != null) {
	this.xmpp.Close();
      }
    }

    public void AsyncRecv() {
      MessageState ms = new MessageState();
      this.peer.BeginReceive(ms.Buffer, 0, 4096, 0,
			     new AsyncCallback(this.ReceiveCallback), ms);
    }

    private void ReceiveCallback(IAsyncResult ar) {
      MessageState ms = (MessageState) ar.AsyncState;
      int bytesRead = this.peer.EndReceive(ar);

      if (bytesRead > 0) {
	ms.Data.Append(Encoding.ASCII.GetString(ms.Buffer, 0, bytesRead));
	string xml = ms.Data.ToString();
	Notification n =  Notification.Parse(xml);
	Category c = (Category)n.Categories[0];
	this.xmpp.Send(new Message(new Jid("sylvain.hellegouarch@gmail.com"), 
				   MessageType.chat, c.Term));
      }
      this.AsyncRecv();
    }
  }
}
