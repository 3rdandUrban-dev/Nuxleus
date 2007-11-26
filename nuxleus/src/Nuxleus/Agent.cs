using System;
using System.Collections;
using Nuxleus.Agent;
using Nuxleus.Messaging;

namespace Nuxleus.Core
{

    public struct Agent : IAgent
    {
        PostOffice _postOffice;
        Hashtable _resultHashtable;
        LoadBalancer _loadBalancer;

        public Agent (LoadBalancer loadBalancer)
        {
            _loadBalancer = LoadBalancer.GetLoadBalancer();
            _postOffice = null;
            _resultHashtable = new Hashtable();
        }

        public PostOffice PostOffice { get { return _postOffice; } set { _postOffice = value; } }
        public Hashtable Result { get { return _resultHashtable; } set { _resultHashtable = value; } }

        public Response MakeRequest (Request request) 
        {
            if (_postOffice == null)
            {
                _postOffice = _loadBalancer.GetPostOffice;
            }
            ///TODO: Create the logic for adding a new request to the _postOffice.Inbox.
            ///For now will just return a new Response() so we can compile;
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