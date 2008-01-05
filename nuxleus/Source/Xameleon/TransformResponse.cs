using System;
using Nuxleus.Agent;

namespace Nuxleus.Transform {

    public struct TransformResponse : IResponse {

        Guid m_id;
        string m_transformResult;
        DateTime m_timeStamp;

        public Guid ID { get { return m_id; } }
        public string TransformResult { get { return m_transformResult; } set { m_transformResult = value; } }
        public DateTime Timestamp { get { return m_timeStamp; } set { m_timeStamp = value; } }


        #region IResponse Members

        public object Result {
            get {
                throw new Exception("The method or operation is not implemented.");
            }
            set {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public string Message {
            get {
                throw new Exception("The method or operation is not implemented.");
            }
            set {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}
