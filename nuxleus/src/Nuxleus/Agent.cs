using System;
using System.Collections;
using Nuxleus.Agent;
using Nuxleus.Messaging;

namespace Nuxleus.Core
{

    public class Agent : IAgent
    {
        PostOffice _postOffice;
        Hashtable _resultHashtable;

        public PostOffice PostOffice { get { return _postOffice; } set { _postOffice = value; } }
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