//
// ReplicationHandler.cs: Replication service implementation
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Nuxleus.Entity;

using ALAZ.SystemEx.NetEx.SocketsEx;

namespace Nuxleus.Messaging.Replication {
    public class ReplicationHandler {
        private MessageService service = null;

        public ReplicationHandler () { }

        public MessageService Service {
            get { return service; }
            set {
                service = value;
                service.Received += new MessageEventHandler(this.EntityMessageReceived);
                //service.Sent += new QueueEventHandler(this.MessageSent);
                //service.Connected += new QueueEventHandler(this.ClientConnected);
                service.Failure += new QueueFailureEventHandler(this.FailureRaised);
            }
        }

        private void FailureRaised ( ISocketConnection sender, Exception ex ) {
            // here we should log the exception

            // we disconnect the faulty client
            sender.BeginDisconnect();
        }

        /// <summary>
        /// This takes care of operations where a file has been stored for the first time
        /// or if it has been replaced.
        /// </summary>
        private void HandleCreateOrReplaceOperation ( IEntity entity ) {

        }

        /// <summary>
        /// This takes care of operations where a file has been deleted.
        /// </summary>
        private void HandleRemoveOperation ( IEntity entity ) {
        }

        private void EntityMessageReceived ( ISocketConnection sender, IMessage message ) {
            ReplicationMessage msg = ReplicationMessage.Deserialize(message.InnerMessage);

            switch (msg.Type) {
                case ReplicationOperationType.CreateOrReplace:
                    HandleCreateOrReplaceOperation(msg.Entity);
                    break;
                case ReplicationOperationType.Remove:
                    HandleRemoveOperation(msg.Entity);
                    break;
            }
        }
    }
}