using System;
using System.Collections;

namespace Nuxleus.Messaging {

    public struct LoadBalancer {

        static readonly LoadBalancer instance = new LoadBalancer(Environment.ProcessorCount);

        ArrayList m_postOfficeArrayList;
        int m_loadBalancePostOfficeIndex;

        LoadBalancer(int processors) {
            m_postOfficeArrayList = new ArrayList();
            m_loadBalancePostOfficeIndex = 0;
            for (int p = 0; p < processors; p++) {
                PostOffice m_postOffice = new PostOffice();
                m_postOfficeArrayList.Add(m_postOffice);
            }
        }

        public static LoadBalancer GetLoadBalancer() {
            return instance;
        }

        public PostOffice GetPostOffice {
            get {
                int i = m_loadBalancePostOfficeIndex;
                m_loadBalancePostOfficeIndex++;
                return (PostOffice)m_postOfficeArrayList[i];
            }
        }

        public int GetPostOfficeCount { get { return m_postOfficeArrayList.Count; } }
    }
}
