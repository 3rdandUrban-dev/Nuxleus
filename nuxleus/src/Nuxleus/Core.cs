using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.ServiceProcess;
using System.Runtime.Remoting;
using Nuxleus.Service;
using Nuxleus.Agent;

namespace Nuxleus.Core
{
    public class Program
    {

        //// The main entry point for the process
        //static void Main(string[] args)
        //{
        //    ServiceBase[] ServicesToRun;
        //    ServicesToRun = new ServiceBase[] { new NuxleusCoreService(3369) };
        //    ServiceBase.Run(ServicesToRun);
        //}
        static void Main()
        {
            LoadBalancer loadBalancer = LoadBalancer.GetLoadBalancer();

            int processors = Environment.ProcessorCount;

            for (int i = 0; i < processors; i++)
            {
                Console.WriteLine(processors);
                Console.WriteLine(loadBalancer.GetQueueCount);
            }
        }
    }
}