using System;
using System.IO;
using System.Threading;
using System.Reflection;
using Nuxleus.Service;
using System.ServiceProcess;
using System.Runtime.Remoting;

namespace Nuxleus.Service
{
    public class Program
    {

        // The main entry point for the process
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new FileSystemWatcherService("foo") };
            ServiceBase.Run(ServicesToRun);
        }
    }
}