using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.ServiceProcess;
using System.Runtime.Remoting;
using Nuxleus.Service;

namespace Nuxleus.Service
{
    public class Program
    {

        // The main entry point for the process
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;

            ServicesToRun = new ServiceBase[] { new LLUPRouterService("127.0.0.1", 7401, 7402) };
            ServiceBase.Run(ServicesToRun);
        }
    }
}