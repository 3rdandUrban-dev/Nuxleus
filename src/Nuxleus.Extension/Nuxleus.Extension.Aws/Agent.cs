using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Configuration;
using Nuxleus.Asynchronous;
using Nuxleus.Core;

namespace Nuxleus.Extension.Aws
{
    public class Agent : IAgent
    {
        static LoggerScope logger = new LoggerScope();
        static ExceptionHandlerScope exShield = new ExceptionHandlerScope();
        static ProfilerScope profiler = new ProfilerScope();
        static int m_workers = (int.Parse(ConfigurationManager.AppSettings["WorkerQueueMultiplier"]) * Environment.ProcessorCount);

        public void Initialize()
        {
            ServicePointManager.DefaultConnectionLimit = int.Parse(ConfigurationManager.AppSettings["DefaultConnectionLimit"]);

            //EnableDnsRoundRobin is not implemented on Mono
#if MS_NET
            //ServicePointManager.EnableDnsRoundRobin = true;
#endif
            //ServicePointManager.Expect100Continue = true;
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            HttpRuntimeSection configSection = (HttpRuntimeSection)config.GetSection("system.web/httpRuntime");
            this.LogInfo("ServicePointManager Default Connection Limit: {0}", ServicePointManager.DefaultConnectionLimit);
            this.LogInfo("ServicePointManager Default Connection Limit: {0}", ServicePointManager.MaxServicePoints);
            this.LogInfo("system.web/httpRuntime/minFreeThreads: {0}", configSection.MinFreeThreads);
            this.LogInfo("system.web/httpRuntime/minLocalRequestFreeThreads: {0}", configSection.MinLocalRequestFreeThreads);

            int minWorkerThreads = int.Parse(ConfigurationManager.AppSettings["MinimumWorkerThreads"]);
            int minAsyncIOThreads = int.Parse(ConfigurationManager.AppSettings["MinimumAsyncIOThreads"]);
            int maxWorkerThreads = int.Parse(ConfigurationManager.AppSettings["MaximumWorkerThreads"]);
            int maxAsyncIOThreads = int.Parse(ConfigurationManager.AppSettings["MaximumAsyncIOThreads"]);
            ThreadPool.SetMaxThreads(maxWorkerThreads, maxAsyncIOThreads);
            ThreadPool.SetMinThreads(minWorkerThreads, minAsyncIOThreads);
        }

        public void Invoke<T>(IEnumerable<IEnumerable<IAsync>> operations, bool loadBalanceOperations)
        {
            Scope scope = new Scope();
            scope += profiler.Scope;
            scope += logger.Scope;
            scope += exShield.Scope;

            logger.Message = "Processing SOAP requests";

            scope.Begin = () =>
            {
                if (loadBalanceOperations)
                {
                    int totalTasksEnqueued = 0;
                    using (IEnumerableTWorkerQueue<IAsync> q = new IEnumerableTWorkerQueue<IAsync>(m_workers))
                    {
                        foreach (IEnumerable<IAsync> operation in operations)
                        {
                            q.EnqueueTask(operation);
                            totalTasksEnqueued += 1;
                        }
                        this.LogInfo("Total Tasks Enqueued: {0}, Total Queues: {1}", totalTasksEnqueued, m_workers);
                    }
                }
                else
                {
                    InvokeAsyncParallelOperation(operations).Execute();
                }

                this.LogInfo("Total Processing Time: {0}", profiler.EllapsedTime.Milliseconds);
            };
        }

        public void InvokeAsyncOperation<T>(IEnumerable<IAsync> operation, bool loadBalanceOperations)
        {

        }

        static IEnumerable<IAsync> InvokeAsyncParallelOperation(IEnumerable<IEnumerable<IAsync>> tasks)
        {
            yield return Async.Parallel(tasks.ToArray());
        }

        void OutputResultList<T>(Dictionary<IRequest, IResponse> responseList)
        {
            int m = 1;
            foreach (KeyValuePair<IRequest, IResponse> responseItem in responseList)
            {
                this.LogInfo(".......................... Begin Message {0} ............................", m);
                this.LogInfo("\n");
                this.LogInfo("[Message {0} Sent]", m);
                this.LogInfo(responseItem.Key.RequestMessage);
                this.LogInfo("\n");
                this.LogInfo("[Message {0} Received]", m);
                this.LogInfo(responseItem.Value.Result);
                this.LogInfo("\n");
                this.LogInfo(".......................... End Message {0} ............................", m);
                m++;
            }
        }

        #region IAgent Members

        public Hashtable Result
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void AuthenticateRequest()
        {
            throw new NotImplementedException();
        }

        public void ValidateRequest()
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginRequest(IRequest request, AsyncCallback callback, NuxleusAsyncResult asyncResult, object extraData)
        {
            throw new NotImplementedException();
        }

        public void EndRequest(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void Invoke()
        {
            throw new NotImplementedException();
        }

        public IResponse GetResponse(Guid id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
