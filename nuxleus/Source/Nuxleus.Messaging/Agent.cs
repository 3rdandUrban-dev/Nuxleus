using System;
using System.Collections;
using Nuxleus.Agent;

namespace Nuxleus.Messaging
{
    public struct Agent : IAgent
    {
        Hashtable m_resultHashtable;
        LoadBalancer m_loadBalancer;

        public Agent (LoadBalancer loadBalancer)
        {
            m_loadBalancer = LoadBalancer.GetLoadBalancer();
            m_resultHashtable = new Hashtable();
        }

        public Hashtable Result { get { return m_resultHashtable; } set { m_resultHashtable = value; } }

        public IResponse GetResponse (Guid id)
        {
            return (IResponse)m_resultHashtable[id];
        }
        public void AuthenticateRequest () { }
        public void ValidateRequest () { }

        #region IAgent Members


        public void EndRequest (IAsyncResult result) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void BeginRequest (IRequest request, AsyncCallback callback, IResponse response) {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}