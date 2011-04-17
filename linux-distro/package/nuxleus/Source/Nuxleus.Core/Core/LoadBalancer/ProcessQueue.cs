using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;

namespace Nuxleus.Core
{

    [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true, Flags = SecurityPermissionFlag.AllFlags)]
    public class ProcessQueue<T> : IDisposable
    {
        static string m_workerQueueBaseName = "AgentProcessQueueWorker";
        static object m_lock = new object();
        Thread[] workers;
        int m_cpuThrottle;
        static Queue<IAgent> queue = new Queue<IAgent>();
        static Queue<NuxleusAppDomain> appDomainQueue = new Queue<NuxleusAppDomain>();
        static AppDomainSetup appDomainSetup = new AppDomainSetup();

        public ProcessQueue(int workerCount, int cpuThrottle)
        {
            workers = new Thread[workerCount];
            m_cpuThrottle = cpuThrottle;
            appDomainSetup.ApplicationBase = System.Environment.CurrentDirectory;
            appDomainSetup.DisallowBindingRedirects = false;
            appDomainSetup.DisallowCodeDownload = true;
            appDomainSetup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            appDomainSetup.LoaderOptimization = LoaderOptimization.MultiDomain;

            // Seed the AppDomain.  Not technically necessary, but Mono throws a key not found error
            // if the appDomainQueue hasn't been seeded with at least one AppDomain
            AppDomain appDomain = AppDomain.CreateDomain(String.Format("{1}-{0}", m_workerQueueBaseName, Guid.NewGuid()), null, appDomainSetup);
            appDomainQueue.Enqueue(
                    (NuxleusAppDomain)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(NuxleusAppDomain).FullName));
            appDomain.InitializeLifetimeService();

            for (int i = 0; i < workerCount; i++)
            {
                (workers[i] = new Thread(Consume)).Start();
            }
        }

        public void EnqueueTask(IAgent agent)
        {
            lock (m_lock)
            {
                queue.Enqueue(agent);
                Monitor.PulseAll(m_lock);
            }
        }

        void Consume()
        {
            while (true)
            {
                IAgent agent;
                lock (m_lock)
                {
                    while (queue.Count == 0)
                        Monitor.Wait(m_lock);
                    agent = queue.Dequeue();
                }
                if (agent == null)
                {
                    return;
                }

                if (m_cpuThrottle > 0)
                {
                    Decimal cpuThrottle = m_cpuThrottle;
                    while (GetCurrentCpuUsage() >= cpuThrottle)
                    {
                        System.Console.WriteLine("Current CPU Usage is over {0}", cpuThrottle);
                        Thread.Sleep(1000);
                    }
                }

                NuxleusAppDomain nuxleusAppDomain = null;
                lock (m_lock)
                {
                    if (appDomainQueue.Count > 0)
                    {

                        nuxleusAppDomain = appDomainQueue.Dequeue();
                    }
                    else
                    {
                        AppDomain appDomain = AppDomain.CreateDomain(String.Format("{1}-{0}", m_workerQueueBaseName, Guid.NewGuid()), null, appDomainSetup);
                        nuxleusAppDomain =
                            (NuxleusAppDomain)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(NuxleusAppDomain).FullName);
                        nuxleusAppDomain.InitializeLifetimeService();
                    }
                }

                nuxleusAppDomain.Invoke(agent, appDomainQueue);

            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true, Flags = SecurityPermissionFlag.AllFlags)]
        static Decimal GetCurrentCpuUsage()
        {
            Decimal finalPercentage = 0;
            try
            {
                var cpuUsage = from process in GetProcesses()
                               let ellapsedTime = DateTime.Now.Subtract(process.StartTime).Ticks
                               let percentCpuUsage = Math.Ceiling(Decimal.Divide((Decimal)process.TotalProcessorTime.Ticks, (Decimal)ellapsedTime) * 100)
                               select new
                               {
                                   PercentCpuUsage = percentCpuUsage
                               };

                Decimal totalPercentage = 0;

                foreach (var percentCpuUsage in cpuUsage.ToArray())
                {
                    totalPercentage += (int)percentCpuUsage.PercentCpuUsage;
                }
                finalPercentage = Math.Ceiling(Decimal.Divide(totalPercentage, (Decimal)System.Environment.ProcessorCount));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return finalPercentage;
        }

        static IEnumerable<System.Diagnostics.Process> GetProcesses()
        {
            foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses())
            {
                yield return process;
            }
        }

        public void Dispose()
        {
            foreach (Thread worker in workers)
            {
                EnqueueTask(null);
            }
            foreach (Thread worker in workers)
            {
                worker.Join();
            }
        }
    }

    [System.Web.AspNetHostingPermission(SecurityAction.Demand, Level = System.Web.AspNetHostingPermissionLevel.Unrestricted, Unrestricted = true)]
    public class NuxleusAppDomain : MarshalByRefObject
    {
        static object obj = new object();
        bool m_initialized = false;
        public void Invoke(IAgent agent, Queue<NuxleusAppDomain> appDomainQueue)
        {
            if (!m_initialized)
            {
                Initialize();
            }
            agent.Invoke();
            lock (obj)
            {
                appDomainQueue.Enqueue(this);
            }
        }

        void Initialize()
        {
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
