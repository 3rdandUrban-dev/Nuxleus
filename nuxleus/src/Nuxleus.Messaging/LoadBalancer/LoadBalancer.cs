using System;
using System.Collections;

namespace Nuxleus.Messaging
{

    sealed class LoadBalancer
    {

        static readonly LoadBalancer instance = new LoadBalancer(Environment.ProcessorCount);

        ArrayList _postOfficeArrayList = new ArrayList();
        int _loadBalancePostOfficeIndex = 0;

        LoadBalancer (int processors)
        {
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
