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

            ServicesToRun = new ServiceBase[] { new LLUPPublisherService(7400, 7401) };
            ServiceBase.Run(ServicesToRun);
        }
    }
}