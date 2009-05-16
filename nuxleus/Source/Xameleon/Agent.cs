using System;
using System.Collections;
using Nuxleus.Core;
using System.Collections.Generic;
using System.IO;
using Nuxleus.Asynchronous;
using Nuxleus.Messaging;

namespace Nuxleus.Transform {

    public delegate TransformResponse NuxleusTransformAsyncRequestDelegate ( TransformRequest request );
    public delegate TransformResponse BeginAgentAsyncRequest ( TransformRequest request );

    public struct Agent : IAgent {

        PostOffice m_postOffice;
        Hashtable m_resultHashtable;
        LoadBalancer m_loadBalancer;
        Transform m_transform;
        AsyncCallback m_callback;
        //MemoryStream m_memoryStream;

        public Agent ( LoadBalancer loadBalancer ) {
            m_loadBalancer = LoadBalancer.GetLoadBalancer();
            m_postOffice = null;
            m_resultHashtable = new Hashtable();
            m_transform = new Transform();
            m_callback = null;
        }

        public Transform Transform { get { return m_transform; } set { m_transform = value; } }
        public PostOffice PostOffice { get { return m_postOffice; } set { m_postOffice = value; } }
        public Hashtable Result { get { return m_resultHashtable; } set { m_resultHashtable = value; } }
        //public MemoryStream ResultStream { get { return m_memoryStream; } set { m_memoryStream = value; } }

        //public TransformResponse MakeRequest (IRequest request, AsyncCallback callback, Nuxleus.Agent.NuxleusAsyncResult asyncResult, object extraData) {
        //    //if (m_postOffice == null) {
        //    //    m_postOffice = m_loadBalancer.GetPostOffice;
        //    //}
        //    //Console.WriteLine("Transform reached");
        //    //TransformRequest tr = (TransformRequest)request;
        //    //m_transform.BeginTransformProcess((TransformRequest)request, callback, asyncResult);
        //    //m_resultHashtable[tr.ID] = tr.TransformResult;
        //    //Console.WriteLine("TransformIsComplete reached w. GUID: {0}.  Hashtable has: {1} entries.", tr.ID.ToString(), m_resultHashtable.Count);
        //    //Console.WriteLine("End of Invoke Reached");
        //    //return asyncResult;

        //}

        public IAsyncResult BeginRequest(IRequest request, AsyncCallback callback, Nuxleus.Core.NuxleusAsyncResult asyncResult, object extraData) {
            //if (m_postOffice == null) {
            //    m_postOffice = m_loadBalancer.GetPostOffice;
            //}
            Console.WriteLine("Transform reached");
            TransformRequest tr = (TransformRequest)request;
            m_transform.BeginTransformProcess((TransformRequest)request);
            m_resultHashtable[tr.ID] = tr.TransformResult;
            Console.WriteLine("TransformIsComplete reached w. GUID: {0}.  Hashtable has: {1} entries.", tr.ID.ToString(), m_resultHashtable.Count);
            Console.WriteLine("End of Invoke Reached");
            return asyncResult;

        }

        public IAsyncResult MakeRequest ( TransformRequest request, AsyncCallback callback, object extraData ) {
            //if (m_postOffice == null) {
            //    m_postOffice = m_loadBalancer.GetPostOffice;
            //}
            TransformProcessDelegate transformProcessDelegate = m_transform.BeginTransformProcess;
            Console.WriteLine("Make Transform Request Reached");
            return transformProcessDelegate.BeginInvoke(request, callback, transformProcessDelegate);
            //return m_transform.BeginTransformProcess(request);

        }

        public static void EndThisRequest ( IAsyncResult ar ) {
            TransformProcessDelegate transformProcessDelegate = (TransformProcessDelegate)ar.AsyncState;
            TransformResponse response = transformProcessDelegate.EndInvoke(ar);
            Console.WriteLine("Transform Is Complete Reached");
            Console.WriteLine("Result of transformation: {0}", response.TransformResult);
        }


        public void EndRequest ( IAsyncResult ar ) {
            TransformProcessDelegate transformProcessDelegate = (TransformProcessDelegate)ar.AsyncState;
            TransformResponse response = transformProcessDelegate.EndInvoke(ar);
            Console.WriteLine("Transform Is Complete Reached");
            Console.WriteLine("Result of transformation: {0}", response.TransformResult);
        }

        public IResponse GetResponse ( Guid id ) {
            TransformResponse tr = (TransformResponse)m_resultHashtable[id];
            m_resultHashtable.Remove(id);
            return tr;
        }

        public void AuthenticateRequest () { }

        public void ValidateRequest () { }

        #region IAgent Members


        public void Invoke() {
            throw new NotImplementedException();
        }

        #endregion
    }
}
