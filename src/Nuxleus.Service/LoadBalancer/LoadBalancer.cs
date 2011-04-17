using System;
using System.Collections;

namespace Nuxleus.Service
{
    sealed class LoadBalancer
    {
        static readonly LoadBalancer instance = new LoadBalancer(Environment.ProcessorCount);
        ArrayList queues = new ArrayList();
        int m_loadBalanceQueueIndex = 0;

        LoadBalancer(int processors)
        {
            for (int p = 0; p < processors; p++)
            {
                Queue m_queue = new Queue();
                queues.Add(m_queue);
            }
        }

        public static LoadBalancer GetLoadBalancer()
        {
            return instance;
        }

        public Queue GetQueue
        {
            get
            {
                int i = m_loadBalanceQueueIndex;
                m_loadBalanceQueueIndex++;
                return (Queue)queues[i];
            }
        }

        public int GetQueueCount { get { return queues.Count; } }
    }
}
