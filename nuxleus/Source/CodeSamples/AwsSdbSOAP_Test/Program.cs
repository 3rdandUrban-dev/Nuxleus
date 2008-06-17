using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuxleus.Extension.Aws;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Xml.Linq;
using System.Threading;

namespace AwsSdbSOAP_Test {
    class Program {


        static void Main(string[] args) {

            Agent agent = new Agent();

            System.Environment.SetEnvironmentVariable("AWS_PUBLIC_KEY", ConfigurationManager.AppSettings["AWS_PUBLIC_KEY"]);
            System.Environment.SetEnvironmentVariable("AWS_PRIVATE_KEY", ConfigurationManager.AppSettings["AWS_PRIVATE_KEY"]);

            Stopwatch stopwatch = new Stopwatch();

            Console.WriteLine("Current thread id:\t {0}", Thread.CurrentThread.ManagedThreadId);

            stopwatch.Start();
            agent.Initialize();
            agent.Invoke<XElement>("AD.txt");
            stopwatch.Stop();

            Console.WriteLine("Completed all in:\t {0}ms", stopwatch.ElapsedMilliseconds);
        }
    }
}
