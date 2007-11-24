using System;
using System.Collections;

namespace Nuxleus.Agent
{
    public interface IAgent
    {
        Queue Inbox { get; set; }
        Queue Outbox { get; set; }
        Hashtable Result { get; set; }

        void AuthenticateRequest();
        void ValidateRequest();
        Response MakeRequest(Request request);
        string GetResponse(Guid id);
    }
}
