//
// SubscriberHandler.cs: LLUP base subscriber handler implementation
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

namespace Nuxleus.Messaging.LLUP {
    public delegate void NotificationReceived ( Notification n );

    /// <summary>
    /// A subscriber is the a leaf node in a LLUP network in the sense
    /// that it receives notifications from routers but don't forward them
    /// to another component. Instead its purpose is to process the 
    /// notification, fetch the associated resource and process it
    /// to offer a subscription service (say an Atom feed for instance).
    ///
    /// This class offers two ways to handle an incoming a notification.
    /// Either your application register an event handler to the 
    /// Received event, or you setup a thread that polls the Pending
    /// queue for new notifications. The latter solutions offer a nice
    /// way to buffer the input while your processing occur. The former
    /// provides a more reactive solution but may not work well if the 
    /// processing is slower than the rate at which notifications arrive.
    /// </summary>
    public class SubscriberHandler {
        private object processedLock = new object();
        private IList<string> processed = new List<string>();
        private Queue notifications = Queue.Synchronized(new Queue());
        private ReceiverHandler receiver = null;
        public IList<MessageService> services = new List<MessageService>();

        public SubscriberHandler () {
            receiver = new ReceiverHandler();
        }

        public event NotificationReceived Received = null;

        /// <summary>
        /// Gets the queue of pending notifications that are waiting to be processed.
        /// </summary>
        public Queue Pending {
            get { return notifications; }
        }

        /// <summary>
        /// Gets the list of notification id that have been processed.
        /// Keeping this list avoids for one notification with a given id
        /// to be re-processed. Remember to empty this list from time to time
        /// so that it doesn't consume too much memory.
        /// </summary
        public IList<string> Processed {
            get { return processed; }
        }

        public void AddService ( MessageService service ) {
            service.Received += new MessageEventHandler(this.BlipReceived);
            //service.Sent += new QueueEventHandler(this.MessageSent);
            //service.Connected += new QueueEventHandler(this.ClientConnected);
            service.Failure += new QueueFailureEventHandler(this.FailureRaised);

            // not really used for now...
            services.Add(service);
        }

        public void RemoveService ( MessageService service ) {
            services.Remove(service);
        }

        private void FailureRaised ( ISocketConnection sender, Exception ex ) {
            // here we should log the exception
            Console.WriteLine(ex.ToString());

            // we disconnect the faulty client
            sender.BeginDisconnect();
        }

        private void BlipReceived ( ISocketConnection sender, IMessage message ) {
            Notification n = Notification.Parse(message.InnerMessage);
            if ((n.Id != null) && (n.Id != String.Empty)) {

                // Just ensure that we don't run into some kind of race condition
                lock (processedLock) {
                    if (!processed.Contains(n.Id)) {
                        processed.Add(n.Id);
                    } else {
                        // The blip has already been processed 
                        return;
                    }
                }

                if (Received != null) {
                    Received(n);
                } else {
                    notifications.Enqueue(n);
                }
            }
        }
    }
}