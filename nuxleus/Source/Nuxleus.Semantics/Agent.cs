using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Nuxleus.Agent;

namespace Nuxleus.Semantics {
    public struct Agent : IAgent {
        #region IAgent Members

        public Hashtable Result {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public void AuthenticateRequest () {
            throw new NotImplementedException();
        }

        public void ValidateRequest () {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginRequest ( IRequest request, AsyncCallback callback, NuxleusAsyncResult asyncResult, object extraData ) {
            throw new NotImplementedException();
        }

        public void EndRequest ( IAsyncResult result ) {
            throw new NotImplementedException();
        }

        public IResponse GetResponse ( Guid id ) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
