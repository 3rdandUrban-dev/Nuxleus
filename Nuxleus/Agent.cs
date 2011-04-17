using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Nuxleus.Messaging;

namespace Nuxleus.Core
{

    public delegate int AsyncInvoke();

    public struct Agent : IAgent
    {

        PostOffice m_postOffice;
        Hashtable m_resultHashtable;
        LoadBalancer m_loadBalancer;

        public Agent(LoadBalancer loadBalancer)
        {
            m_loadBalancer = LoadBalancer.GetLoadBalancer();
            m_postOffice = null;
            m_resultHashtable = new Hashtable();
        }

        public PostOffice PostOffice { get { return m_postOffice; } set { m_postOffice = value; } }
        public Hashtable Result { get { return m_resultHashtable; } set { m_resultHashtable = value; } }

        public IResponse GetResponse(Guid id)
        {
            return (IResponse)m_resultHashtable[id];
        }
        public void AuthenticateRequest() { }
        public void ValidateRequest() { }

        public void EndRequest(IAsyncResult result)
        {

        }

        public void BeginRequest(IRequest request)
        {
            if (m_postOffice == null)
            {
                m_postOffice = m_loadBalancer.GetPostOffice;
            }
            AsyncCallback callBack = EndThisRequest;
            AsyncInvoke method1 = TestAsyncInvoke.Method1;
            Console.WriteLine("Calling BeginInvoke on Thread {0}", Thread.CurrentThread.ManagedThreadId);
            IAsyncResult asyncResult = method1.BeginInvoke(callBack, method1);
        }

        public static void EndThisRequest(IAsyncResult result)
        {
            Console.WriteLine("Calling EndThisRequest on Thread {0}", Thread.CurrentThread.ManagedThreadId);
            AsyncResult asyncResult = (AsyncResult)result;
            AsyncInvoke method1 = (AsyncInvoke)asyncResult.AsyncDelegate;

            int retVal = method1.EndInvoke(asyncResult);
            Console.WriteLine("retVal (Callback): {0}", retVal);
        }

        #region IAgent Members


        public IAsyncResult BeginRequest(IRequest request, AsyncCallback callback, NuxleusAsyncResult asyncResult, object extraData)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion


        #region IAgent Members


        public void Invoke()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class TestAsyncInvoke
    {
        public static int Method1()
        {
            Console.WriteLine("Invoked Method1 on Thread {0}", Thread.CurrentThread.ManagedThreadId);
            return 1;
        }
    }
}
