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
using Nuxleus.Extension.Aws.SimpleDb;
using System.Security.Permissions;
using System.Security;

namespace AwsSdbSOAP_Test {

    class Program {

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.AllFlags)]
        static void Main(string[] args) {

            System.Environment.SetEnvironmentVariable("AWS_PUBLIC_KEY", ConfigurationManager.AppSettings["AWS_PUBLIC_KEY"]);
            System.Environment.SetEnvironmentVariable("AWS_PRIVATE_KEY", ConfigurationManager.AppSettings["AWS_PRIVATE_KEY"]);
            System.Environment.SetEnvironmentVariable("AWS_URI_ENDPOINT", ConfigurationManager.AppSettings["AWS_URI_ENDPOINT"]);
            int workers = int.Parse(args[1]);
            int cpuThrottle = int.Parse(args[2]);
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            switch (args[0]) {
                case "sync":
                    RunSync(args);
                    break;
                case "noloadasync":
                    RunAsyncNoLoadBalance(args);
                    break;
                case "procloadasync":
                    System.Console.WriteLine("Args Length: {0}", args.Length);
                    List<string> jobs = new List<string>();
                    int i = 0;
                    foreach (string job in args) {
                        if (i > 2) {
                            System.Console.WriteLine("job: {0}", job);
                            jobs.Add(job);
                        }
                        i++;
                    }
                    RunAsyncProcessLoadBalance(jobs, workers, cpuThrottle);
                    break;
                case "async":
                default:
                    RunAsync(args);
                    break;
            }
            stopwatch.Stop();

            Console.WriteLine("Completed all in:\t {0}ms", stopwatch.ElapsedMilliseconds);
        }

        static void RunSync(string[] args) {
            SyncAgent<Program> agent = new SyncAgent<Program>();
            agent.Invoke(args[1]);
        }

        static void RunAsync(string[] args) {
            Agent agent = new Agent();
            agent.Invoke<XElement>(args[1]);
        }

        static void RunAsyncNoLoadBalance(string[] args) {
            NoLoadBalanceAgent agent = new NoLoadBalanceAgent();
            agent.Invoke(args[1]);
        }

        static void RunAsyncProcessLoadBalance(List<string> args, int maxWorkers, int cpuThrottle) {
            using (SimpleDBProcessQueue q = new SimpleDBProcessQueue(((args.Count <= maxWorkers) ? args.Count : maxWorkers), cpuThrottle)) {
                foreach (string job in args) {
                    q.EnqueueTask(job);
                }
            }
        }
    }
}
