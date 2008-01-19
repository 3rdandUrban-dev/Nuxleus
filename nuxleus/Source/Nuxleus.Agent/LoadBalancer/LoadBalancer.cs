using System;
using System.Collections;

namespace Nuxleus.Agent
{

    public struct LoadBalancer
    {

        static readonly LoadBalancer instance = new LoadBalancer(Environment.ProcessorCount);

        ArrayList _postOfficeArrayList;
        int _loadBalancePostOfficeIndex;

        LoadBalancer (int processors)
        {
            _postOfficeArrayList = new ArrayList();
            _loadBalancePostOfficeIndex = 0;
            for (int p = 0; p < processors; p++)
            {
                PostOffice m_postOffice = new PostOffice();
                _postOfficeArrayList.Add(m_postOffice);
            }
        }

        public static LoadBalancer GetLoadBalancer ()
        {
            return instance;
        }

        public PostOffice GetPostOffice
        {
            get
            {
                int i = _loadBalancePostOfficeIndex;
                _loadBalancePostOfficeIndex++;
                return (PostOffice)_postOfficeArrayList[i];
            }
        }

        public int GetPostOfficeCount { get { return _postOfficeArrayList.Count; } }
    }
}
