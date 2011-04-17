using System;
using System.Collections;
using Nuxleus.Core;
using Nuxleus.Agent;

namespace Nuxleus.Messaging {

    public delegate void PostedHandler (Notification n);

    /// <summary>
    /// 
    /// </summary>
    public class PostOffice : IPostOffice {
        Queue _inboxQueue;
        Queue _outboxQueue;
        Hashtable _resultHashtable;

        public PostOffice () { }

        /// <summary>
        /// Register delegates to be notified of a new notification
        /// to process.
        /// </summary>
        public event PostedHandler Mailbox = null;

        /// <summary>
        /// Notify that a notification is ready to be processed.
        /// </summary>
        public void Post (Notification n) {
            if (Mailbox != null) {
                Mailbox(n);
            }
        }

        public Response MakeRequest (Request request) {
            return new Response();
        }

        public String GetResponse (Guid id) {
            return (String)_resultHashtable[id];
        }

        public Queue Inbox {
            get {
                return _inboxQueue;
            }
            set {
                _inboxQueue = value;
            }
        }

        public Queue Outbox {
            get {
                return _outboxQueue;
            }
            set {
                _outboxQueue = value;
            }
        }

        public Hashtable Result {
            get {
                return _resultHashtable;
            }
            set {
                _resultHashtable = value;
            }
        }

    }
}