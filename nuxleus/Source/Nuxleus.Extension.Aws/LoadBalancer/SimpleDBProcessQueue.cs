using System;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using Nuxleus.Asynchronous;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Security;
using System.Reflection;
using Nuxleus.Extension;
using System.Net;
using System.Configuration;
using System.Web.Configuration;


namespace Nuxleus.Extension.Aws.SimpleDb {

    [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true, Flags = SecurityPermissionFlag.AllFlags)]
    public struct SimpleDBProcessQueue : IDisposable {
        static object m_lock = new object();
        Thread[] workers;
        int m_cpuThrottle;
        static Queue<String> queue = new Queue<String>();
        static Queue<SimpleDBAppDomain> appDomainQueue = new Queue<SimpleDBAppDomain>();
        static AppDomainSetup appDomainSetup = new AppDomainSetup();

        public SimpleDBProcessQueue(int workerCount, int cpuThrottle) {
            workers = new Thread[workerCount];
            m_cpuThrottle = cpuThrottle;
            appDomainSetup.ApplicationBase = System.Environment.CurrentDirectory;
            appDomainSetup.DisallowBindingRedirects = false;
            appDomainSetup.DisallowCodeDownload = true;
            appDomainSetup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            appDomainSetup.LoaderOptimization = LoaderOptimization.MultiDomain;

            // Seed the AppDomain.  Not technically necessary, but Mono throws a key not found error
            // if the appDomainQueue hasn't been seeded with at least one AppDomain
            AppDomain appDomain = AppDomain.CreateDomain("SimpleDB SOAP Worker", null, appDomainSetup);
            appDomainQueue.Enqueue(
                    (SimpleDBAppDomain)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(SimpleDBAppDomain).FullName));
            appDomain.InitializeLifetimeService();

            for (int i = 0; i < workerCount; i++) {
                (workers[i] = new Thread(Consume)).Start();
            }
        }

        public void EnqueueTask(String task) {
            lock (m_lock) {
                queue.Enqueue(task);
                Monitor.PulseAll(m_lock);
            }
        }

        void Consume() {
            while (true) {
                String task;
                lock (m_lock) {
                    while (queue.Count == 0)
                        Monitor.Wait(m_lock);
                    task = queue.Dequeue();
                }
                if (task == null) {
                    return;
                }

                if (m_cpuThrottle > 0) {
                    Decimal cpuThrottle = m_cpuThrottle;
                    while (GetCurrentCpuUsage() >= cpuThrottle) {
                        System.Console.WriteLine("Current CPU Usage is over {0}", cpuThrottle);
                        Thread.Sleep(1000);
                    }
                }
                
                SimpleDBAppDomain simpleDBAppDomain = null;
                lock (m_lock) {
                    if (appDomainQueue.Count > 0) {

                        simpleDBAppDomain = appDomainQueue.Dequeue();
                    } else {
                        AppDomain appDomain = AppDomain.CreateDomain(String.Format("SimpleDB SOAP Worker: {0}", Guid.NewGuid()), null, appDomainSetup);
                        simpleDBAppDomain =
                            (SimpleDBAppDomain)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(SimpleDBAppDomain).FullName);
                        simpleDBAppDomain.InitializeLifetimeService();
                    }
                }
                
                simpleDBAppDomain.Invoke(task, appDomainQueue);
                
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true, Flags = SecurityPermissionFlag.AllFlags)]
        static Decimal GetCurrentCpuUsage() {
            Decimal finalPercentage = 0;
            try {
                var cpuUsage = from process in GetProcesses()
                               let ellapsedTime = DateTime.Now.Subtract(process.StartTime).Ticks
                               let percentCpuUsage = Math.Ceiling(Decimal.Divide((Decimal)process.TotalProcessorTime.Ticks, (Decimal)ellapsedTime) * 100)
                               select new {
                                   PercentCpuUsage = percentCpuUsage
                               };

                Decimal totalPercentage = 0;

                foreach (var percentCpuUsage in cpuUsage.ToArray()) {
                    totalPercentage += (int)percentCpuUsage.PercentCpuUsage;
                }
                finalPercentage = Math.Ceiling(Decimal.Divide(totalPercentage, (Decimal)System.Environment.ProcessorCount));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return finalPercentage;
        }

        static IEnumerable<Process> GetProcesses() {
            foreach (Process process in Process.GetProcesses()) {
                yield return process;
            }
        }

        public void Dispose() {
            foreach (Thread worker in workers) {
                EnqueueTask(null);
            }
            foreach (Thread worker in workers) {
                worker.Join();
            }
        }
    }

    [System.Web.AspNetHostingPermission(SecurityAction.Demand, Level = System.Web.AspNetHostingPermissionLevel.Unrestricted, Unrestricted = true)]
    //[SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true, Flags = SecurityPermissionFlag.AllFlags)]
    public class SimpleDBAppDomain : MarshalByRefObject {
        static object obj = new object();
        bool m_initialized = false;
        public void Invoke(string fileName, Queue<SimpleDBAppDomain> appDomainQueue) {
            NoLoadBalanceAgent agent = new NoLoadBalanceAgent();
            if (!m_initialized) {
                Initialize();
            }
            //Initialize();
            //HttpRuntimeSection configSection = (HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime");
            //this.LogInfo("ServicePointManager Default Connection Limit: {0}", ServicePointManager.DefaultConnectionLimit);
            //this.LogInfo("system.web/httpRuntime/minFreeThreads: {0}", configSection.MinFreeThreads);
            //this.LogInfo("system.web/httpRuntime/minLocalRequestFreeThreads: {0}", configSection.MinLocalRequestFreeThreads);
            //this.LogDebug("Invoking task {0} on thread: {1}", fileName, Thread.CurrentThread.ManagedThreadId);
            agent.Invoke(fileName);
            lock (obj) {
                appDomainQueue.Enqueue(this);
            }
        }

        void Initialize() {
            ServicePointManager.DefaultConnectionLimit = int.Parse(ConfigurationManager.AppSettings["DefaultConnectionLimit"]);
            //EnableDnsRoundRobin is not implemented on Mono
            //ServicePointManager.EnableDnsRoundRobin = true;

            //ServicePointManager.Expect100Continue = true;
            int minWorkerThreads = int.Parse(ConfigurationManager.AppSettings["MinimumWorkerThreads"]);
            int minAsyncIOThreads = int.Parse(ConfigurationManager.AppSettings["MinimumAsyncIOThreds"]);
            int maxWorkerThreads = int.Parse(ConfigurationManager.AppSettings["MaximumWorkerThreads"]);
            int maxAsyncIOThreads = int.Parse(ConfigurationManager.AppSettings["MaximumAsyncIOThreads"]);
            ThreadPool.SetMaxThreads(maxWorkerThreads, maxAsyncIOThreads);
            ThreadPool.SetMinThreads(minWorkerThreads, minAsyncIOThreads);
            m_initialized = true;
        }

    }
}
