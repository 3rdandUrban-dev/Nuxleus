//
// BuckerServerHandler.cs: Bucker queue handler implementation
// See: http://trac.defuze.org/wiki/bucker
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 
using System;
using System.Collections;
using System.Collections.Generic;
using Memcached.ClientLibrary;
using Nuxleus.Bucker;

using ALAZ.SystemEx.NetEx.SocketsEx;

namespace Nuxleus.Messaging.QS {

    public class BlipMessageServerHandler {

        private static DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToUniversalTime();
        private static char[] comma = { ',' };

        private MessageService service = null;
        private MemcachedClient mc = null;
        private string rootQueueId = null;
        private IList<IntPtr> clientsToDisconnect = new List<IntPtr>();

        public BlipMessageServerHandler ( string[] servers, string rootQueueId ) {
            this.rootQueueId = rootQueueId;

            SockIOPool pool = SockIOPool.GetInstance("bucker-queue-server");
            pool.SetServers(servers);
            pool.InitConnections = 3;
            pool.MinConnections = 3;
            pool.MaxConnections = 5;

            pool.SocketConnectTimeout = 1000;
            pool.SocketTimeout = 3000;
            pool.MaintenanceSleep = 30;
            pool.Failover = true;
            pool.Nagle = false;
            pool.Initialize();

            mc = new MemcachedClient();
            mc.PoolName = "bucker-queue-server";
            mc.EnableCompression = false;

            mc.Add(rootQueueId, String.Empty);
        }

        public void Close () {
            SockIOPool pool = SockIOPool.GetInstance("bucker-queue-server");
            pool.Shutdown();
        }

        public MessageService Service {
            get { return service; }
            set {
                service = value;
                service.Received += new MessageEventHandler(this.MessageReceived);
                service.Sent += new QueueEventHandler(this.MessageSent);
                //service.Connected += new QueueEventHandler(this.ClientConnected);
                service.Failure += new QueueFailureEventHandler(this.FailureRaised);
            }
        }

        private void FailureRaised ( ISocketConnection sender, Exception ex ) {
            // since the client generated an exceptipon we will disconnect it
            // as soon as the error message below is sent
            clientsToDisconnect.Add(sender.SocketHandle);

            // we warn the client something happened
            Nuxleus.Bucker.Message m = new Nuxleus.Bucker.Message();
            m.Type = "error";
            m.Op = null;
            m.Error = new Nuxleus.Bucker.Error();
            m.Error.Type = "internal-server-error";
            m.Error.Code = 500;
            sender.BeginSend(Nuxleus.Bucker.Message.Serialize(m));
        }

        private void MessageSent ( ISocketConnection sender ) {
            // In case the last message sent to the client was to warn it an error occured
            // we should have its handle in there and just close the connection
            if (clientsToDisconnect.Contains(sender.SocketHandle)) {
                clientsToDisconnect.Remove(sender.SocketHandle);
                sender.BeginDisconnect();
            }
        }

        private Nuxleus.Bucker.Message CheckQueueId ( string queueId ) {
            if ((queueId == null) || (queueId == String.Empty)) {
                Nuxleus.Bucker.Message m = new Nuxleus.Bucker.Message();
                m.Type = "error";
                m.Op = null;
                m.Error = new Nuxleus.Bucker.Error();
                m.Error.Type = "message-invalid";
                m.Error.Code = 400;
                m.Error.Message = "Missing queue id value";
                return m;
            }

            return null;
        }

        private Nuxleus.Bucker.Message CheckMessageId ( string messageId ) {
            if ((messageId == null) || (messageId == String.Empty)) {
                Nuxleus.Bucker.Message m = new Nuxleus.Bucker.Message();
                m.Type = "error";
                m.Op = null;
                m.Error = new Nuxleus.Bucker.Error();
                m.Error.Type = "message-invalid";
                m.Error.Code = 400;
                m.Error.Message = "Missing message id value";
                return m;
            }

            return null;
        }

        private Nuxleus.Bucker.Message CheckQueue ( string queueId ) {
            Nuxleus.Bucker.Message err = CheckQueueId(queueId);
            if (err != null)
                return err;

            string queues = (string)mc.Get(rootQueueId);
            if ((queues == null) || (queues == String.Empty) || !queues.Contains(queueId)) {
                Nuxleus.Bucker.Message m = new Nuxleus.Bucker.Message();
                m.Type = "error";
                m.Op = null;
                m.Error = new Nuxleus.Bucker.Error();
                m.Error.Type = "not-found";
                m.Error.Code = 404;
                m.Error.Message = String.Format("'{0}' is not an existing queue", queueId);
                return m;
            }

            return null;
        }


        private Nuxleus.Bucker.Message HandleNewQueueRequest ( Nuxleus.Bucker.Message m ) {
            Nuxleus.Bucker.Message err = CheckQueueId(m.QueueId);
            if (err != null)
                return err;

            string queues = (string)mc.Get(rootQueueId);
            if (queues == null) { // just in case
                queues = String.Empty;
            }

            if (!queues.Contains(m.QueueId)) {
                mc.Add(m.QueueId, String.Empty);
                mc.Add(String.Format("{0}.new", m.QueueId), String.Empty);
                if (queues == String.Empty) {
                    queues = m.QueueId;
                } else {
                    queues = String.Format("{0},{1}", queues, m.QueueId);
                }
                mc.Set(rootQueueId, queues);
                m.Type = "response";
            } else {
                // Houston we have a conflict!
                string qid = m.QueueId;

                m = new Nuxleus.Bucker.Message();
                m.Type = "error";
                m.Op = null;
                m.Error = new Nuxleus.Bucker.Error();
                m.Error.Type = "conflict";
                m.Error.Code = 409;
                m.Error.Message = String.Format("'{0}' is already an existing queue", qid);
            }

            return m;
        }

        private Nuxleus.Bucker.Message HandleDeleteQueueRequest ( Nuxleus.Bucker.Message m ) {
            Nuxleus.Bucker.Message err = CheckQueueId(m.QueueId);
            if (err != null)
                return err;

            string qid = m.QueueId;
            string keys = (string)mc.Get(qid);

            if ((keys != null) && (keys != String.Empty)) {
                if (m.Op.Force == false) {
                    m = new Nuxleus.Bucker.Message();
                    m.Type = "error";
                    m.Op = null;
                    m.Error = new Nuxleus.Bucker.Error();
                    m.Error.Type = "queue-not-empty";
                    m.Error.Code = 400;
                    m.Error.Message = String.Format("'{0}' is not empty. Either delete all the messages first or set the force attribute on the 'op' element", qid);
                    return m;
                } else {
                    // Deleting all the messages attached to that queue
                    string[] keysList = keys.Split(',');
                    foreach (string key in keysList) {
                        mc.Delete(String.Format("{0}.visibility", key));
                        mc.Delete(key);
                    }
                }
            }

            mc.Delete(qid);
            mc.Delete(String.Format("{0}.new", qid));

            string queues = (string)mc.Get(rootQueueId);
            if (queues == null)
                queues = String.Empty;
            if (queues.Contains(qid)) {
                queues = queues.Replace(qid, "");
                queues = queues.Replace(",,", "");
                queues = queues.Trim(comma);
                mc.Set(rootQueueId, queues);
            }

            m.Type = "response";
            return m;
        }

        private Nuxleus.Bucker.Message HandleListQueuesRequest ( Nuxleus.Bucker.Message m ) {
            string queues = (string)mc.Get(rootQueueId);
            if (queues == null)
                queues = String.Empty;
            string[] queuesList = queues.Split(',');

            Nuxleus.Bucker.Message lm = new Nuxleus.Bucker.Message();
            lm.Type = "response";
            lm.Op.Type = OperationType.ListQueues;
            if (queues == String.Empty) {
                lm.Queues = new string[0];
            } else {
                lm.Queues = new string[queuesList.Length];
                Array.Copy(queuesList, lm.Queues, queuesList.Length);
            }

            return lm;
        }

        private Nuxleus.Bucker.Message HandleListMessagesRequest ( Nuxleus.Bucker.Message m ) {
            Nuxleus.Bucker.Message err = CheckQueue(m.QueueId);
            if (err != null)
                return err;

            string qid = m.QueueId;

            m.Type = "response";
            m.QueueId = m.QueueId;
            m.Op.Type = OperationType.ListMessages;

            string keys = (string)mc.Get(String.Format("{0}.new", m.QueueId));
            if ((keys == null) || (keys == String.Empty)) {
                m.Messages = new string[0];
            } else {
                string[] unread = keys.Split(',');
                int count = 10;
                if (unread.Length < 10) {
                    count = unread.Length;
                }
                int index = 0;
                m.Messages = new string[count];
                foreach (string mid in unread) {
                    m.Messages[index]  = mid;
                    index++;
                    if (index == 10) {
                        break;
                    }
                }
            }

            return m;
        }

        private double UnixTimestampNow {
            get {
                TimeSpan ts = DateTime.UtcNow - origin;
                return ts.TotalSeconds;
            }
        }

        private Nuxleus.Bucker.Message HandlePushMessageRequest ( Nuxleus.Bucker.Message m ) {
            Nuxleus.Bucker.Message err = CheckQueue(m.QueueId);
            if (err != null)
                return err;

            string mid = Guid.NewGuid().ToString();
            string payload = m.Payload;
            if (payload == null)
                payload = String.Empty;
            mc.Set(mid, payload);

            mc.Set(String.Format("{0}.visibility", mid), UnixTimestampNow);

            string keys = (string)mc.Get(m.QueueId);
            if ((keys == null) || (keys == String.Empty)) {
                keys = mid;
            } else {
                keys = String.Format("{0},{1}", keys, mid);
            }
            mc.Set(m.QueueId, keys);

            keys = (string)mc.Get(String.Format("{0}.new", m.QueueId));
            if ((keys == null) || (keys == String.Empty)) {
                keys = mid;
            } else {
                keys = String.Format("{0},{1}", keys, mid);
            }
            mc.Set(String.Format("{0}.new", m.QueueId), keys);

            m.Type = "response";
            m.MessageId = mid;

            return m;
        }

        private Nuxleus.Bucker.Message HandleDeleteMessageRequest ( Nuxleus.Bucker.Message m ) {
            Nuxleus.Bucker.Message err = CheckQueue(m.QueueId);
            if (err != null)
                return err;

            err = CheckMessageId(m.MessageId);
            if (err != null)
                return err;

            string keys = (string)mc.Get(String.Format("{0}", m.QueueId));
            if (keys == null)
                keys = String.Empty;
            if (keys.Contains(m.MessageId)) {
                keys = keys.Replace(m.MessageId, "");
                keys = keys.Replace(",,", "");
                keys = keys.Trim(comma);
                mc.Set(m.QueueId, keys);
            }

            mc.Delete(String.Format("{0}.visibility", m.MessageId));
            mc.Delete(m.MessageId);

            m.Type = "response";
            return m;
        }

        private Nuxleus.Bucker.Message HandleGetMessageRequest ( Nuxleus.Bucker.Message m ) {
            Nuxleus.Bucker.Message err = CheckQueue(m.QueueId);
            if (err != null)
                return err;

            err = CheckMessageId(m.MessageId);
            if (err != null)
                return err;

            string unreadKey = String.Format("{0}.new", m.QueueId);

            string[] keys = { m.QueueId, unreadKey, m.MessageId };
            Hashtable result = mc.GetMultiple(keys);

            if (!mc.KeyExists(m.QueueId)) {
                m.Type = "error";
                m.Op = null;
                m.Error = new Nuxleus.Bucker.Error();
                m.Error.Type = "not-found";
                m.Error.Code = 404;
                m.Error.Message = String.Format("'{0}' is not an existing queue", m.QueueId);
                return m;
            }

            string mids = (string)result[m.QueueId];

            if (mc.KeyExists(m.MessageId)) {
                if (m.Op.Peek == false) {
                    string unread = (string)result[unreadKey];
                    if (unread.Contains(m.MessageId)) {
                        unread = unread.Replace(m.MessageId, "");
                        unread = unread.Replace(",,", "");
                        unread = unread.Trim(comma);
                        mc.Set(unreadKey, unread);
                    }
                }
                string visibilityKey = String.Format("{0}.visibility", m.MessageId);
                double visibilityTimeout = (double)mc.Get(visibilityKey);
                double now = UnixTimestampNow;
                if (now > visibilityTimeout) {
                    if (m.Op.Peek == false)
                        mc.Set(visibilityKey, now + 30);
                    string payload = String.Empty;
                    if (result.ContainsKey(m.MessageId)) {
                        payload = (string)result[m.MessageId];
                        if (payload == null)
                            payload = String.Empty;
                    }
                    m.Type = "response";
                    m.Payload = payload;
                } else {
                    err = new Nuxleus.Bucker.Message();
                    err.Type = "error";
                    err.Op = null;
                    err.Error = new Nuxleus.Bucker.Error();
                    err.Error.Type = "invisible";
                    err.Error.Code = 404;
                    err.Error.Message = String.Format("'{0}' is not visible", m.MessageId);
                }
            } else {
                err = new Nuxleus.Bucker.Message();
                err.Type = "error";
                err.Op = null;
                err.Error = new Nuxleus.Bucker.Error();
                err.Error.Type = "not-found";
                err.Error.Code = 404;
                err.Error.Message = String.Format("'{0}' was not found in the queue '{1}'", m.MessageId, m.QueueId);
            }

            return m;
        }

        private void MessageReceived ( ISocketConnection sender, IMessage message ) {
            if (clientsToDisconnect.Contains(sender.SocketHandle)) {
                // if the client has been scheduled to be closed we don't process any of its
                // incoming data
                return;
            }

            Nuxleus.Bucker.Message m = Nuxleus.Bucker.Message.Parse(message.InnerMessage);
            Nuxleus.Bucker.Message responseToSend = null;

            switch (m.Op.Type) {
                case OperationType.GetMessage:
                    responseToSend = HandleGetMessageRequest(m);
                    break;
                case OperationType.ListMessages:
                    responseToSend = HandleListMessagesRequest(m);
                    break;
                case OperationType.PushMessage:
                    Console.WriteLine(m.ToString());
                    responseToSend = HandlePushMessageRequest(m);
                    break;
                case OperationType.DeleteMessage:
                    responseToSend = HandleDeleteMessageRequest(m);
                    break;
                case OperationType.NewQueue:
                    responseToSend = HandleNewQueueRequest(m);
                    break;
                case OperationType.DeleteQueue:
                    responseToSend = HandleDeleteQueueRequest(m);
                    break;
                case OperationType.ListQueues:
                    responseToSend = HandleListQueuesRequest(m);
                    break;
                default:
                    responseToSend = new Nuxleus.Bucker.Message();
                    responseToSend.Type = "error";
                    responseToSend.Op = null;
                    responseToSend.Error = new Nuxleus.Bucker.Error();
                    responseToSend.Error.Type = "operation-not-allowed";
                    responseToSend.Error.Code = 405;
                    break;
            }

            if (responseToSend != null) {
                sender.BeginSend(Nuxleus.Bucker.Message.Serialize(responseToSend));
                responseToSend = null;
            }
        }
    }
}