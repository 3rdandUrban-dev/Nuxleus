using System;
using System.Collections;
using Nuxleus.Agent;

namespace Nuxleus.Core
{

    public class Agent : IAgent
    {
        Queue _inboxQueue;
        Queue _outboxQueue;
        Hashtable _resultHashtable;

        public Queue Inbox { get { return _inboxQueue; } set { _inboxQueue = value; } }
        public Queue Outbox { get { return _outboxQueue; } set { _outboxQueue = value; } }
        public Hashtable Result { get { return _resultHashtable; } set { _resultHashtable = value; } }

        public Response MakeRequest (Request request) 
        {
            return new Response();
        }

        public string GetResponse (Guid id)
        {
            return (string)_resultHashtable[id];
        }
        public void AuthenticateRequest () { }
        public void ValidateRequest () { }
    }
}