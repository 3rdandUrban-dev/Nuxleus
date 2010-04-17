using System;
using System.Threading;
using System.Web;
using System.IO;

namespace Nuxleus.Core {

    [Serializable]
    public class NuxleusAsyncResult : IAsyncResult {

        Boolean m_isCompleted;
        AsyncCallback m_cb = null;
        Object m_asyncState;
        public HttpContext m_context = null;

        public NuxleusAsyncResult (AsyncCallback cb, Object extraData) {
            this.m_cb = cb;
            m_asyncState = extraData;
            m_isCompleted = false;
        }


        public object AsyncState {
            get {
                return m_asyncState;
            }
        }

        public bool CompletedSynchronously {
            get {
                return false;
            }
        }

        public WaitHandle AsyncWaitHandle {
            get {
                throw new InvalidOperationException(
                          "ASP.Net should never use this property");
            }
        }

        public bool IsCompleted {
            get {
                return m_isCompleted;
            }
        }

        public void CompleteCall () {
            m_isCompleted = true;
            if (m_cb != null) {
                m_cb(this);
            }
        }

    }
}
