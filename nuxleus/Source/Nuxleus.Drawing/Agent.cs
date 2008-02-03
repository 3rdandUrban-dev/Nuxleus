using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuxleus.Agent;
using System.Collections;

namespace Nuxleus.Drawing {
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
