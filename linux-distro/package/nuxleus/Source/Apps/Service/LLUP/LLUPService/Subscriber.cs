using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.ServiceProcess;
using System.Runtime.Remoting;
using Nuxleus.Service;
using Nuxleus.Messaging.LLUP;

namespace Nuxleus.Service
{
    public class Program
    {

        // The main entry point for the process
        static void Main (string[] args)
        {
            ServiceBase[] ServicesToRun;
            // This implies you have a router listening on port 7403
            string[] routersToBindTo = { "127.0.0.1:7403" };
            ISubscriber[] subs = { new AtomFeedSubscriber() };

            LLUPSubscriberService service = new LLUPSubscriberService(routersToBindTo, subs);
            ServicesToRun = new ServiceBase[] { service };
            ServiceBase.Run(ServicesToRun);
        }
    }
}