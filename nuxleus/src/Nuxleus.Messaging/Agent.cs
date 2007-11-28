using System;
using System.Collections;
using Nuxleus.Agent;

namespace Nuxleus.Messaging
{
    public struct Agent : IAgent
    {
        Hashtable _resultHashtable;
        LoadBalancer _loadBalancer;

        public Agent (LoadBalancer loadBalancer)
        {
            _loadBalancer = LoadBalancer.GetLoadBalancer();
            _resultHashtable = new Hashtable();
        }

        public Hashtable Result { get { return _resultHashtable; } set { _resultHashtable = value; } }

        public Response MakeRequest (Request request)
        {
            //if (_postOffice == null)
            //{
            //    _postOffice = _loadBalancer.GetPostOffice;
            //}
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