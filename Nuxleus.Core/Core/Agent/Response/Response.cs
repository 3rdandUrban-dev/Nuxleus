using System;
using System.Collections;

namespace Nuxleus.Agent
{
    public struct Response : IResponse
    {
        Guid m_id;
        string m_message;
        Object m_object;
        DateTime m_timeStamp;

        public Guid ID { get { return m_id; } }
        public string Message { get { return m_message; } set { m_message = value; } }
        public Object Result { get { return m_object; } set { m_object = value; } }
        public DateTime Timestamp { get { return m_timeStamp; } set { m_timeStamp = value; } }
    }
}