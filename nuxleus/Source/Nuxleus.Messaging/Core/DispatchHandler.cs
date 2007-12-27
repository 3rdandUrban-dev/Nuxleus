using System;
using System.Collections;
using System.Collections.Generic;

using ALAZ.SystemEx.NetEx.SocketsEx;
using ALAZ.SystemEx.ThreadingEx;

namespace Nuxleus.Messaging.Core
{

    // Straight dispatcher of notifications to connected clients
    // This does not do any kind of processing on the notification itself
    internal class DispatchHandler
    {
        private MessageService service = null;
        private PostOffice postOffice = null;
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
        public MessageService Service
        {
            get { return service; }
            set
            {
                service = value;
                service.Connected += new QueueEventHandler(ClientConnected);
                service.Disconnected += new QueueEventHandler(ClientDisconnected);
                service.Failure += new QueueFailureEventHandler(FailureRaised);
            }
        }

        /// <summary>
        /// Sets the PostOffice instance used for being notified of 
        /// new notification to process.
        /// </summary>
        public PostOffice PostOffice
        {
            set
            {
                postOffice = value;
                postOffice.Mailbox += new PostedHandler(RequestToDispatch);
            }
        }

        private void ClientConnected (ISocketConnection sender)
        {
            clients.Add(sender);
        }

        private void ClientDisconnected (ISocketConnection sender)
        {
            if (clients.Contains(sender))
            {
                clients.Remove(sender);
            }
        }

        private void SendToAll (Notification n)
        {
            if (clients.Count > 0)
            {
                byte[] blip = Notification.Serialize(n);
                int loopSleep = 0;
                foreach (ISocketConnection client in clients)
                {
                    try
                    {
                        client.BeginSend(blip);
                    }
                    finally
                    {
                        ThreadEx.LoopSleep(ref loopSleep);
                    }
                }
            }
        }

        private void RequestToDispatch (Notification n)
        {
            // The publisher always ensure that each notification has its llup:id 
            // element set so that consumers can decide whether or not
            // they have already processed a notification
            // The form of the id doesn't matter as long as it's unique.
            n.Id = Guid.NewGuid().ToString();
            SendToAll(n);
        }

        private void FailureRaised (ISocketConnection sender, Exception ex)
        {
            // here we should log the exception
            Console.Write(ex.ToString());
            // we disconnect the faulty client
            sender.BeginDisconnect();
        }
    }
}