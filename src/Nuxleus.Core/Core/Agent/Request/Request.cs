using System;
using System.Collections;

namespace Nuxleus.Agent
{
    public delegate void AsyncRequestCallback(IResponse response);

    public struct Request : IRequest
    {
        Guid m_id;
        string m_message;
        DateTime m_timeStamp;
        bool m_isComplete;
        IResponse m_response;
        Object m_object;

        public Request(string message) 
        {
            m_id = Guid.NewGuid();
            m_message = message;
            m_timeStamp = DateTime.Now;
            m_isComplete = false;
            m_response = new Response();
            m_object = new Object();
        }

        public void BeginRequest()
        {
            ProcessRequest(new AsyncRequestCallback(CompleteRequest), m_object);
        }

        private void ProcessRequest(AsyncRequestCallback callback, object extraData)
        {
            ///TODO: Build out request processing logic
            Response myResponse = new Response();
            myResponse.Message = "<foo>bar</foo>";
            myResponse.Timestamp = DateTime.Now;
            callback(myResponse);
        }

        public void CompleteRequest(IResponse response)
        {
            m_isComplete = true;
            m_response = response;
        }

        public Guid ID { get { return m_id; } set { m_id = value; } }
        public string Message { get { return m_message; } set { m_message = value; } }
        public DateTime Timestamp { get { return m_timeStamp; } set { m_timeStamp = value; } }
        public Object ExtraData { get { return m_object; } set { m_object = value; } }
        public IResponse GetResponse { get { return m_response; } }
        public bool IsCompleted { get { return m_isComplete; } set { m_isComplete = value; } }
    }
}
