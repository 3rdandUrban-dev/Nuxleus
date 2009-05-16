//
// BuckerToLLUP.cs: LLUP publication handler implementation with notifications carried 
// in a bucker queue message.
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Nuxleus.Bucker;
using Nuxleus.Messaging.LLUP;

using ALAZ.SystemEx.NetEx.SocketsEx;
using ALAZ.SystemEx.ThreadingEx;

namespace Nuxleus.Messaging.QS {
    public class QSToLLUPHandler {
        private PollHandler poller = null;
        private DispatchHandler dispatcher = null;
        private IList<string> monitoredQueues = new List<string>();
        private Timer pullTimer = null;
        private int freq = 3000;

        public QSToLLUPHandler () {
            poller = new PollHandler();
            dispatcher = new DispatchHandler();
            BlipPostOffice po = new BlipPostOffice();
            poller.PostOffice = po;
            dispatcher.PostOffice = po;
        }

        public int Frequency {
            get { return freq; }
            set { freq = value; }
        }

        public IList<string> MonitoredQueues {
            get { return monitoredQueues; }
        }

        public void StartMonitoring () {
            TimerCallback timerDelegate = new TimerCallback(CheckQueuesForNewMessages);
            pullTimer = new Timer(timerDelegate, null, 1500, freq);
        }

        public void StopMonitoring () {
            pullTimer.Dispose();
            pullTimer = null;
        }

        private void CheckQueuesForNewMessages ( object info ) {
            if (poller.Service.Connection != null) {
                Nuxleus.Bucker.Message lm = new Nuxleus.Bucker.Message();
                lm.Op.Type = OperationType.ListMessages;
                foreach (string queueId in monitoredQueues) {
                    lm.QueueId = queueId;
                    poller.Service.Connection.BeginSend(Nuxleus.Bucker.Message.Serialize(lm));
                }
                lm = null;
            }
        }

        /// <summary>
        /// Gets or sets the service handling events with the queue server
        /// </summary>
        public MessageService PollService {
            get {
                return poller.Service;
            }
            set {
                poller.Service = value;
                poller.Service.Connected += new QueueEventHandler(this.ClientConnected);
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
                poller.PostOffice = value;
                dispatcher.PostOffice = value;
            }
        }

        private void ClientConnected ( ISocketConnection sender ) {
            // we are now connected to the queue server
            // let's ensure the queues we monitor are created too
            Nuxleus.Bucker.Message nq = new Nuxleus.Bucker.Message();
            nq.Op.Type = OperationType.NewQueue;
            foreach (string queueId in monitoredQueues) {
                nq.QueueId = queueId;
                sender.BeginSend(Nuxleus.Bucker.Message.Serialize(nq));
            }

            StartMonitoring();
        }
    }

    internal class PollHandler {
        private MessageService service = null;
        private BlipPostOffice postOffice = null;

        /// <summary>
        /// Extract a LLUP notification from the Payload of a queue message 
        /// and post it to the postoffice
        /// </summary>
        public PollHandler () { }

        /// <summary>
        /// MessageService instance used by the server to notify
        /// of new events on the connections.
        /// </summary>
        public MessageService Service {
            get { return service; }
            set {
                service = value;
                service.Received += new MessageEventHandler(this.QueueMessageReceived);
                service.Failure += new QueueFailureEventHandler(this.FailureRaised);
            }
        }

        /// <summary>
        /// Sets the PostOffice instance used to notify about 
        /// new notifications to be dispatched.
        /// </summary>
        public BlipPostOffice PostOffice {
            set { postOffice = value; }
        }

        private void FailureRaised ( ISocketConnection sender, Exception ex ) {
            // here we should log the exception
            Console.WriteLine(ex.ToString());

            // we disconnect the faulty client
            sender.BeginDisconnect();
        }

        private void HandleListOfNewMessages ( Nuxleus.Bucker.Message m ) {
            if (m.Messages != null) {
                Nuxleus.Bucker.Message gm = new Nuxleus.Bucker.Message();
                gm.Op.Type = OperationType.GetMessage;
                gm.QueueId = m.QueueId;
                foreach (string mid in m.Messages) {
                    gm.MessageId = mid;
                    Service.Connection.BeginSend(Nuxleus.Bucker.Message.Serialize(gm));
                }
            }
        }

        private void HandleMessageReceived ( Nuxleus.Bucker.Message m ) {
            // When a message is received we delete it from the queue as
            // it has no purpose anymore
            Nuxleus.Bucker.Message dm = new Nuxleus.Bucker.Message();
            dm.Op.Type = OperationType.DeleteMessage;
            dm.QueueId = m.QueueId;
            dm.MessageId = m.MessageId;
            Service.Connection.BeginSend(Nuxleus.Bucker.Message.Serialize(dm));

            LLUP.Notification n = LLUP.Notification.Parse(Convert.FromBase64String(m.Payload));
            postOffice.Post(n);
        }

        private void QueueMessageReceived ( ISocketConnection sender, IMessage message ) {
            Nuxleus.Bucker.Message m = Nuxleus.Bucker.Message.Parse(message.InnerMessage);
            switch (m.Op.Type) {
                case OperationType.ListMessages:
                    HandleListOfNewMessages(m);
                    break;
                case OperationType.GetMessage:
                    HandleMessageReceived(m);
                    break;
                default:
                    // we are not interested in processing any other message type
                    break;
            }
        }
    }

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

        private void SendToAll ( LLUP.Notification n ) {
            if (clients.Count > 0) {
                byte[] blip = LLUP.Notification.Serialize(n);
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

        private void BlipToDispatch ( LLUP.Notification n ) {
            // The publisher always ensure that each notification has its llup:id 
            // element set so that consumers can decide whether or not
            // they have already processed a notification
            // The form of the id doesn't matter as long as it's unique.
            n.Id = Guid.NewGuid().ToString();
            SendToAll(n);
        }

        private void FailureRaised ( ISocketConnection sender, Exception ex ) {
            // here we should log the exception

            // we disconnect the faulty client
            sender.BeginDisconnect();
        }
    }
}