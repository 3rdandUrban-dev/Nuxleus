using System;
using System.Collections;

namespace Nuxleus.Core
{

    sealed class LoadBalancer
    {

        static readonly LoadBalancer instance = new LoadBalancer(Environment.ProcessorCount);

        ArrayList queues = new ArrayList();
        int _loadBalanceQueueIndex = 0;

        LoadBalancer (int processors)
        {
            for (int p = 0; p < processors; p++)
            {
                Queue m_queue = new Queue();
                queues.Add(m_queue);
            }
        }

        public static LoadBalancer GetLoadBalancer ()
        {
            return instance;
        }

        public Queue GetQueue
        {
            get
            {
                int i = _loadBalanceQueueIndex;
                _loadBalanceQueueIndex++;
                return (Queue)queues[i];
            }
        }

        public int GetQueueCount { get { return queues.Count; } }
    }
}
