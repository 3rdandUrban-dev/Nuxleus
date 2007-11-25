using System;
using System.Collections;
using Nuxleus.Agent;

namespace Nuxleus.Core
{

    public class InvokeRequest : ICommand
    {
        Request m_request;
        AsyncRequest m_invokeRequest;

        delegate Response AsyncRequest (Request request);

        public InvokeRequest (Agent agent, Request request)
        {
            m_request = request;
            m_invokeRequest = new AsyncRequest(agent.MakeRequest);
        }

        public void Execute ()
        {
            m_invokeRequest.BeginInvoke(m_request, this.CallBack, null);
        }

        private void CallBack (IAsyncResult ar)
        {
            Response response = m_invokeRequest.EndInvoke(ar);

            ///TODO: Process and serialize result.
            ///TODO: Add result to Result Hashtable
        }

    }
}
