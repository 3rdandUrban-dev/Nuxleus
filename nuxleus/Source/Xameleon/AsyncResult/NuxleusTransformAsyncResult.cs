using System;
using System.Threading;
using System.Web;
using System.IO;

namespace Nuxleus.Transform {

    public struct NuxleusTransformAsyncResult : IAsyncResult {

        Boolean m_isCompleted;
        AsyncCallback m_cb;
        TransformResponse m_transformResponse;

        public NuxleusTransformAsyncResult (AsyncCallback cb, TransformResponse transformResponse) {
            this.m_cb = cb;
            m_transformResponse = transformResponse;
            m_isCompleted = false;
        }


        public object AsyncState {
            get {
                return m_transformResponse;
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
