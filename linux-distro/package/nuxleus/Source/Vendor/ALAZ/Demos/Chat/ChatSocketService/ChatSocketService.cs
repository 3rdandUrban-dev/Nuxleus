using System;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using ALAZ.SystemEx;
using ALAZ.SystemEx.NetEx.SocketsEx;
using ALAZ.SystemEx.ThreadingEx;

namespace ChatSocketService
{

    public class ChatSocketService : BaseSocketService
    {

        #region Fields

        private ReaderWriterLock FUsersSync;
        private Dictionary<long, ISocketConnection> FUsers;

        #endregion

        #region Constructor

        public ChatSocketService()
        {
            FUsersSync = new ReaderWriterLock();
            FUsers = new Dictionary<long, ISocketConnection>(100);
        }

        #endregion

        #region Methods

        #region Utils

        #region SerializeMessage

        public static byte[] SerializeMessage(ChatMessage msg)
        {
        
            using(MemoryStream m = new MemoryStream())
            {
            
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(m, msg);
            
                return m.ToArray();
                
            }
        
        }
        
        #endregion

        #region DeserializeMessage

        public static ChatMessage DeserializeMessage(byte[] buffer)
        {

            using(MemoryStream m = new MemoryStream())
            {

                m.Write(buffer, 0, buffer.Length);
                m.Position = 0;

                BinaryFormatter bin = new BinaryFormatter();

                return (ChatMessage) bin.Deserialize(m);
                
            }

        }

        #endregion

        #endregion
        
        #region Service

        #region OnConnected

        public override void OnConnected(ConnectionEventArgs e)
        {

            StringBuilder s = new StringBuilder();

            s.Append("\r\n------------------------------------------------\r\n");
            s.Append("New Client\r\n");
            s.Append(" Connection Id " + e.Connection.ConnectionId + "\r\n");
            s.Append(" Ip Address " + e.Connection.RemoteEndPoint.Address + "\r\n");
            s.Append(" Tcp Port " + e.Connection.RemoteEndPoint.Port + "\r\n");
            
            Console.WriteLine(s.ToString());
            s.Length = 0;
            
            e.Connection.CustomData = new ConnectionData(ConnectionState.csConnected);
            e.Connection.BeginReceive();
            
        }

        #endregion

        #region OnSent

        public override void OnSent(MessageEventArgs e)
        {

            //if (!e.SentByServer)
            //{
            //    e.Connection.BeginReceive();
            //}

        }

        #endregion

        #region OnReceived

        public override void OnReceived(MessageEventArgs e)
        {

            ChatMessage msg = DeserializeMessage(e.Buffer);

            switch (msg.MessageType)
            {

                case MessageType.mtLogin:

                    ((ConnectionData)e.Connection.CustomData).ConnectionState = ConnectionState.csAuthenticated;
                    ((ConnectionData)e.Connection.CustomData).UserName = msg.UserInfo[0].UserName;

                    msg.UserInfo[0].UserId = e.Connection.ConnectionId;
                    e.Connection.BeginSend(SerializeMessage(msg));
                    
                    msg.MessageType = MessageType.mtAuthenticated;
                    e.Connection.AsServerConnection().BeginSendToAll(SerializeMessage(msg), false);
                    
                    ISocketConnection[] cnns = e.Connection.AsServerConnection().GetConnections();
                    
                    if (cnns.Length > 0)
                    {
                        
                        bool send = false;
                        
                        msg.MessageType = MessageType.mtHello;
                        msg.UserInfo = new UserInfo[cnns.Length];
                        
                        for (int i = 0; i < cnns.Length; i++)
        			    {
                            
                            if (cnns[i] != e.Connection)
                            {
                                msg.UserInfo[i].UserName = ((ConnectionData)cnns[i].CustomData).UserName;
                                msg.UserInfo[i].UserId = cnns[i].ConnectionId;
                                send = true;
                            }
                            
		        	    }

                        if (send)
                        {
                            e.Connection.AsServerConnection().BeginSend(SerializeMessage(msg));
                        }
                        
                    }                    
                    
                    break;

                case MessageType.mtMessage:

                    e.Connection.AsServerConnection().BeginSendToAll(e.Buffer, false);

                    break;
                    
                    
                case MessageType.mtLogout:

                    e.Connection.AsServerConnection().BeginSendToAll(SerializeMessage(msg), false);
                    break;    
                
            }

            e.Connection.BeginReceive();

        }

        #endregion

        #region OnDisconnected

        public override void OnDisconnected(ConnectionEventArgs e)
        {

            StringBuilder s = new StringBuilder();

            s.Append("------------------------------------------------" + "\r\n");
            s.Append("Client Disconnected\r\n");
            s.Append(" Connection Id " + e.Connection.ConnectionId + "\r\n");
            
            e.Connection.CustomData = null;
            
            Console.WriteLine(s.ToString());
            
            s.Length = 0;


        }

        #endregion

        #region OnException

        public override void OnException(ExceptionEventArgs e)
        {
            e.Connection.BeginDisconnect();
        }

        #endregion

        #endregion

        #endregion

    }
    
    [Serializable]
    public struct UserInfo
    {

        #region Fields

        private string FUserName;
        private long FUserId;

        #endregion
        
	    #region Properties
	    
	    public string UserName
	    {
            get { return FUserName; }
            set { FUserName = value; }
	    }

        public long UserId
        {
            get { return FUserId; }
            set { FUserId = value; }
        }

	    #endregion   
	    
        #region Methods

        public override string ToString()
        {
            return FUserName;
        }

        #endregion

    }

    [Serializable]
    public class ChatMessage
    {

        #region Fields

        private MessageType FMessageType;
        private UserInfo[] FUsers;
        private string FMessage;

        #endregion
        
        #region Constructor

        public ChatMessage() { }
		 
	    #endregion
	 
	    #region Properties

        public MessageType MessageType
        {
            get { return FMessageType; }
            set { FMessageType = value; }
        }

        public UserInfo[] UserInfo
	    {
            get { return FUsers; }
            set { FUsers = value; }
	    }

        public string Message
        {
            get { return FMessage; }
            set { FMessage = value; }
        }

	    #endregion   

    }

    public class ConnectionData
    {
        
        #region Fields

        private ConnectionState FConnectionState;
        private string FUserName;

        #endregion

        #region Constructor

        public ConnectionData(ConnectionState state)
        {
            FConnectionState = state;
            FUserName = String.Empty;
        }

        #endregion

        #region Properties

        public ConnectionState ConnectionState
        {
            get { return FConnectionState; }
            set { FConnectionState = value; }
        }

        public string UserName
        {
            get { return FUserName; }
            set { FUserName = value; }
        }

        #endregion
        
    }

    public enum MessageType
    {
        mtLogin,
        mtAuthenticated,
        mtHello,
        mtLogout,
        mtMessage
    }

    public enum ConnectionState
    {
        csConnected,
        csAuthenticated,
        csDisconnected
    }


}
