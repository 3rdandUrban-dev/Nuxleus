//
// RouterHandler.cs: LLUP base routers handler implementation
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
    public class RouterHandler {
        private ReceiverHandler receiver = null;
        private FilterHandler filter = null;

        public RouterHandler () {
            receiver = new ReceiverHandler();
            filter = new FilterHandler();
            BlipPostOffice po = new BlipPostOffice();
            receiver.PostOffice = po;
            filter.PostOffice = po;
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
        public MessageService FilterService {
            get {
                return filter.Service;
            }
            set {
                filter.Service = value;
            }
        }

        /// <summary>
        /// Sets the per-application specific notification filter.
        /// </summary>
        public IRouterFilter Filter {
            set {
                filter.Filter = value;
            }
        }

        /// <summary>
        /// Sets the index instance that will index any incoming
        /// notifications and de-index expired or discarded ones.
        /// </summary>
        public INotificationIndex Index {
            set {
                filter.Index = value;
            }
        }

        /// <summary>
        /// PostOffice to synchronise receiver and dispatcher.
        /// It is set internally but you may change it to a different
        /// instance.
        /// </summary>
        public BlipPostOffice PostOffice {
            set {
                receiver.PostOffice = value;
                filter.PostOffice = value;
            }
        }
    }

    internal class FilterHandler {
        private MessageService service = null;
        private BlipPostOffice postOffice = null;
        private IList<ISocketConnection> clients = new List<ISocketConnection>();
        private IRouterFilter filter = null;
        private INotificationIndex index = null;

        /// <summary>
        /// The FilterHandler is in charge of deciding whether or not an 
        /// incoming notification can be propagated down the stream or not
        /// based on the filter set.
        /// By default all notifications are forwarded as-is. Your application
        /// shoud implement IRouterFilter and set the Filter property
        /// to an instance of your own filter class.
        /// </summary>
        public FilterHandler () {
            filter = new DefaultRouterFilter();
        }

        /// <summary>
        /// Sets the filter object that will be called to process the notification.
        /// </summary>
        public IRouterFilter Filter {
            set { filter = value; }
        }

        /// <summary>
        /// Sets the index instance that will index any incoming
        /// notifications and de-index expired or discarded ones.
        /// </summary>
        public INotificationIndex Index {
            set {
                index = value;
            }
        }


        /// <summary>
        /// Sets the service instance used by the server to notify
        /// of new events on the connections.
        /// </summary>
        public MessageService Service {
            get { return service; }
            set {
                service = value;
                service.Connected += new QueueEventHandler(this.ClientConnected);
                service.Disconnected += new QueueEventHandler(this.ClientDisconnected);
                service.Failure += new QueueFailureEventHandler(this.FailureRaised);
                //service.Received += new MessageEventHandler(this.BlipReceived);
            }
        }

        /// <summary>
        /// Sets the PostOffice instance used for being notified of 
        /// new notification to process.
        /// </summary>
        public BlipPostOffice PostOffice {
            set {
                postOffice = value;
                postOffice.Mailbox += new BlipPostedHandler(this.BlipReceived);
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

        private void FailureRaised ( ISocketConnection sender, Exception ex ) {
            // here we should log the exception
            Console.WriteLine(ex.ToString());

            // we disconnect the faulty client
            sender.BeginDisconnect();
        }

        /// <summary>
        /// Sends the notification to all connected clients.
        /// </summary>
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

        private void BlipReceived ( Notification n ) {
            if (index != null) {
                index.Index(n);
            }
            // If the Notification is valid then we 
            // we send it to the connected routers.
            n = filter.ProcessNotification(n, index);
            if (n != null) {
                SendToAll(n);
            }
        }
    }

    internal class DefaultRouterFilter : IRouterFilter {
        /// <summary>
        /// Simply returns the passed notification.
        /// </summary>
        /// <return>
        /// Returns the notification as-is.
        /// </return>
        public Notification ProcessNotification ( Notification n, INotificationIndex index ) {
            return n;
        }
    }
}