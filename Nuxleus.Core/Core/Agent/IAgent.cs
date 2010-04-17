using System;
using System.Collections;

namespace Nuxleus.Core
{
    public delegate IAsyncResult NuxleusAgentAsyncRequest (IRequest request, AsyncCallback callback, NuxleusAsyncResult asyncResult, object extraData);

    public interface IAgent
    {
        Hashtable Result { get; set; }

        void AuthenticateRequest();
        void ValidateRequest();
        IAsyncResult BeginRequest (IRequest request, AsyncCallback callback, NuxleusAsyncResult asyncResult, Object extraData);
        void EndRequest (IAsyncResult result);
        void Invoke();
        IResponse GetResponse(Guid id);
    }
}
