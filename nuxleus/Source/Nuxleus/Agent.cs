using System;
using System.Collections;
using Nuxleus.Agent;
using Nuxleus.Messaging;

namespace Nuxleus.Core {

    public struct Agent : IAgent {

        PostOffice m_postOffice;
        Hashtable m_resultHashtable;
        LoadBalancer m_loadBalancer;

        public Agent (LoadBalancer loadBalancer) {
            m_loadBalancer = LoadBalancer.GetLoadBalancer();
            m_postOffice = null;
            m_resultHashtable = new Hashtable();
        }

        public PostOffice PostOffice { get { return m_postOffice; } set { m_postOffice = value; } }
        public Hashtable Result { get { return m_resultHashtable; } set { m_resultHashtable = value; } }

        public void BeginRequest (IRequest request) {
            if (m_postOffice == null) {
                m_postOffice = m_loadBalancer.GetPostOffice;
            }
            throw new Exception("The method or operation is not implemented.");
        }

        public IResponse GetResponse (Guid id) {
            return (string)m_resultHashtable[id];
        }
        public void AuthenticateRequest () { }
        public void ValidateRequest () { }

        public void EndRequest (IAsyncResult result) {
            throw new Exception("The method or operation is not implemented.");
        }

        public IAsyncResult BeginRequest (IRequest request, AsyncCallback callback, NuxleusAsyncResult asyncResult, object extraData) {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}