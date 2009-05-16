//
// ReceiverHandler.cs: Internal handler that takes used to handle
// notifications received from a remote component (be it router or publisher)
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 
using System;
using System.Collections;
using System.Collections.Generic;

using ALAZ.SystemEx.NetEx.SocketsEx;
using ALAZ.SystemEx.ThreadingEx;

namespace Nuxleus.Messaging.LLUP {
    internal class ReceiverHandler {
        private MessageService service = null;
        private BlipPostOffice postOffice = null;

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
        public MessageService Service {
            get { return service; }
            set {
                service = value;
                service.Received += new MessageEventHandler(this.BlipReceived);
                //service.Sent += new QueueEventHandler(this.MessageSent);
                //service.Connected += new QueueEventHandler(this.ClientConnected);
                service.Failure += new QueueFailureEventHandler(this.FailureRaised);
            }
        }

        /// <summary>
        /// Sets the PostOffice instance used to notify about 
        /// new notifications to be routed.
        /// </summary>
        public BlipPostOffice PostOffice {
            set { postOffice = value; }
        }

        private void FailureRaised ( ISocketConnection sender, Exception ex ) {
            // here we should log the exception

            // we disconnect the faulty client
            sender.BeginDisconnect();
        }

        private void BlipReceived ( ISocketConnection sender, IMessage message ) {
            Notification n = Notification.Parse(message.InnerMessage);
            postOffice.Post(n);
        }
    }
}