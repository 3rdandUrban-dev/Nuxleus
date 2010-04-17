using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

namespace Nuxleus.Messaging
{

    public sealed class LoadBalancer
    {

        static readonly int m_processorCount = Environment.ProcessorCount;
        static readonly LoadBalancer instance = new LoadBalancer(m_processorCount);

        Queue<PostOffice> m_postOfficeQueue;
        int m_loadBalancePostOfficeIndex;

        LoadBalancer() {
            m_postOfficeQueue = new Queue<PostOffice>(m_processorCount);
            m_loadBalancePostOfficeIndex = 0;
            for (int p = 0; p < m_processorCount; p++) {
                PostOffice postOffice = new PostOffice();
                m_postOfficeQueue.Enqueue(postOffice);
            }
        }

        LoadBalancer(int size) {
            m_postOfficeQueue = new Queue<PostOffice>(size);
            m_loadBalancePostOfficeIndex = 0;
            for (int p = 0; p < size; p++) {
                PostOffice postOffice = new PostOffice();
                m_postOfficeQueue.Enqueue(postOffice);
            }
        }

        public static LoadBalancer GetLoadBalancer() {
            return instance;
        }

        public PostOffice GetPostOffice {
            get {
                return m_postOfficeQueue.Dequeue();
            }
        }

        public int GetPostOfficeCount {
            get {
                return m_postOfficeQueue.Count;
            }
        }
    }
}
