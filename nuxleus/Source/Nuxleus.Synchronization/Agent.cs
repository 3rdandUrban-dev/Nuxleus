using System;
using System.Collections;
using Nuxleus.Agent;


namespace Nuxleus.Synchronization
{
    public class Agent : IAgent
    {

        #region IAgent Members

        public Hashtable Result {
            get {
                throw new Exception("The method or operation is not implemented.");
            }
            set {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public void AuthenticateRequest () {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ValidateRequest () {
            throw new Exception("The method or operation is not implemented.");
        }

        public IAsyncResult BeginRequest (IRequest request, AsyncCallback callback, NuxleusAsyncResult asyncResult, object extraData) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void EndRequest (IAsyncResult result) {
            throw new Exception("The method or operation is not implemented.");
        }

        public IResponse GetResponse (Guid id) {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
