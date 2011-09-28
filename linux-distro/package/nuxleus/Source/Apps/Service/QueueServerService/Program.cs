using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.ServiceProcess;
using System.Runtime.Remoting;
using Nuxleus.Service;

namespace Nuxleus.Service {
    public class Program {

        // The main entry point for the process
        static void Main ( string[] args ) {
            ServiceBase[] ServicesToRun;
            string[] memcachedServers = { "127.0.0.1:11211" };
            ServicesToRun = new ServiceBase[] { new BlipQueueServerService(9876, 
									     memcachedServers, 
									     "nuXleus-queue") };
            ServiceBase.Run(ServicesToRun);
        }
    }
}