using System;
using System.Collections;
using System.Collections.Generic;
using ALAZ.SystemEx.NetEx.SocketsEx;
using ALAZ.SystemEx.ThreadingEx;
using Nuxleus.Core;
using Nuxleus.Agent;

namespace Nuxleus.Messaging.Core
{
    internal class ReceiverHandler
    {
        MessageService _service = null;
        PostOffice _postOffice = null;
        Request m_request;
        AsyncRequest m_invokeRequest;

        delegate Response AsyncRequest (Request request);

        /// <summary>
        /// An instance of this class is in charge of recieving 
        /// new notifications and then inform the routing side that a new
        /// notification is ready to be dispatched to routers.
        /// </summary>
        public ReceiverHandler () { }

        /// <summary>
        /// MessageService instance used by the server to notify
        /// of new events on the connections.
        /// </summary>
        public MessageService Service
        {
            get { return _service; }
            set
            {
                _service = value;
                _service.Received += new MessageEventHandler(RequestReceived);
                //service.Sent += new QueueEventHandler(this.MessageSent);
                //service.Connected += new QueueEventHandler(this.ClientConnected);
                _service.Failure += new QueueFailureEventHandler(FailureRaised);
            }
        }

        /// <summary>
        /// Sets the PostOffice instance used to notify about 
        /// new notifications to be routed.
        /// </summary>
        public PostOffice PostOffice
        {
            set { _postOffice = value; }
        }

        private void FailureRaised (ISocketConnection sender, Exception ex)
        {
            // here we should log the exception

            // we disconnect the faulty client
            sender.BeginDisconnect();
        }

        private void RequestReceived (ISocketConnection sender, IMessage message)
        {
            Notification n = Notification.Parse(message.InnerMessage);
            _postOffice.Post(n);
        }


        public void Execute ()
        {
            m_invokeRequest.BeginInvoke(m_request, this.CallBack, null);
        }

        private void CallBack (IAsyncResult ar)
        {
            Response response = m_invokeRequest.EndInvoke(ar);

            ///TODO: Process and serialize result.
            ///TODO: Add result to Result Hashtable
        }

    }
}