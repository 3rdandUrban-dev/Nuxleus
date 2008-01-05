using System;
using Nuxleus.Agent;

namespace Nuxleus.Transform {

    public struct TransformRequest : IRequest {

        Guid m_id;
        string m_transformResult;
        string m_message;
        DateTime m_timeStamp;
        TransformContext m_transformContext;
        bool m_isCompleted;

        public Guid ID { get { return m_id; } set { m_id = value; } }
        public string TransformResult { get { return m_transformResult; } set { m_transformResult = value; } }
        public DateTime Timestamp { get { return m_timeStamp; } set { m_timeStamp = value; } }

        public TransformRequest (Guid guid) {
            m_id = guid;
            m_transformResult = null;
            m_message = null;
            m_timeStamp = DateTime.Now;
            m_transformContext = new TransformContext();
            m_isCompleted = false;
        }

        public Object TransformContext {
            get {
                return m_transformContext;
            }
            set {
                m_transformContext = (TransformContext)value;
            }
        }

        public IResponse GetResponse {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsCompleted {
            get { return m_isCompleted; }
            set { m_isCompleted = value; }
        }

        public string Message {
            get { return m_message; }
            set { m_message = value; }
        }
    }
}
