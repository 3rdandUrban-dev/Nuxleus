//
// PublisherHandler.cs: LLUP publication handler implementation
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
    public class PublisherHandler {
        private ReceiverHandler receiver = null;
        private DispatchHandler dispatcher = null;

        public PublisherHandler () {
            receiver = new ReceiverHandler();
            dispatcher = new DispatchHandler();
            BlipPostOffice po = new BlipPostOffice();
            receiver.PostOffice = po;
            dispatcher.PostOffice = po;
        }

        /// <summary>
        /// Gets or sets the service handling events on the connections
        /// between clients to the publisher and the publisher handler.
        /// </summary>
        public MessageService ReceiverService {
            get {
                return receiver.Service;
            }
            set {
                receiver.Service = value;
            }
        }

        /// <summary>
        /// Gets or sets the service handling events on the connections
        /// between the publisher and routers connected to it.
        /// </summary>
        public MessageService DispatcherService {
            get {
                return dispatcher.Service;
            }
            set {
                dispatcher.Service = value;
            }
        }

        /// <summary>
        /// PostOffice to synchronise receiver and dispatcher.
        /// Set internally by the constructor but can be changed to 
        /// different instance.
        /// </summary>
        public BlipPostOffice PostOffice {
            set {
                receiver.PostOffice = value;
                dispatcher.PostOffice = value;
            }
        }
    }

    // Straight dispatcher of notifications to connected clients
    // This does not do any kind of processing on the notification itself
    internal class DispatchHandler {
        private MessageService service = null;
        private BlipPostOffice postOffice = null;
        // Router connections
        private IList<ISocketConnection> clients = new List<ISocketConnection>();

        /// <summary>
        /// Routing side of a LLUP publisher. Its task is to accept connections
        /// from notification routers and propagate to them published notification,
        /// without any kind of processing.
        /// </summary>
        public DispatchHandler () { }

        /// <summary>
        /// MessageService instance used by the server to notify
        /// of new events on the connections.
        /// </summary>
        public MessageService Service {
            get { return service; }
            set {
                service = value;
                service.Connected += new QueueEventHandler(this.ClientConnected);
                service.Disconnected += new QueueEventHandler(this.ClientDisconnected);
                service.Failure += new QueueFailureEventHandler(this.FailureRaised);
            }
        }

        /// <summary>
        /// Sets the PostOffice instance used for being notified of 
        /// new notification to process.
        /// </summary>
        public BlipPostOffice PostOffice {
            set {
                postOffice = value;
                postOffice.Mailbox += new BlipPostedHandler(this.BlipToDispatch);
            }
        }

        private void ClientConnected ( ISocketConnection sender ) {
            clients.Add(sender);
        }

        private void ClientDisconnected ( ISocketConnection sender ) {
            if (clients.Contains(sender)) {
                clients.Remove(sender);
            }
        }

        private void SendToAll ( Notification n ) {
            if (clients.Count > 0) {
                byte[] blip = Notification.Serialize(n);
                int loopSleep = 0;
                foreach (ISocketConnection client in clients) {
                    try {
                        client.BeginSend(blip);
                    } finally {
                        ThreadEx.LoopSleep(ref loopSleep);
                    }
                }
            }
        }

        private void BlipToDispatch ( Notification n ) {
            // The publisher always ensure that each notification has its llup:id 
            // element set so that consumers can decide whether or not
            // they have already processed a notification
            // The form of the id doesn't matter as long as it's unique.
            n.Id = Guid.NewGuid().ToString();
            SendToAll(n);
        }

        private void FailureRaised ( ISocketConnection sender, Exception ex ) {
            // here we should log the exception
            Console.Write(ex.ToString());
            // we disconnect the faulty client
            sender.BeginDisconnect();
        }
    }
}