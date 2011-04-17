using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Nuxleus.Agent;

namespace Nuxleus.Agent
{
    public interface IPostOffice
    {
        Queue Inbox { get; set; }
        Queue Outbox { get; set; }
        Hashtable Result { get; set;}
        event PostedHandler Mailbox;
        Response MakeRequest (Request request);
        String GetResponse (Guid id);
        void Post (Notification notification);
    }
}
