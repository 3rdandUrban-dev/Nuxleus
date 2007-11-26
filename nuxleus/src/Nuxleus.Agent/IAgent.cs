using System;
using System.Collections;
using Nuxleus.Messaging;

namespace Nuxleus.Agent
{
    public interface IAgent
    {
        PostOffice PostOffice { get; set; }
        Hashtable Result { get; set; }

        void AuthenticateRequest();
        void ValidateRequest();
        Response MakeRequest(Request request);
        string GetResponse(Guid id);
    }
}
