using System;
using System.Web;

namespace Nuxleus.Web.HttpHandler {
    public class NuxleusEventHandler : IHttpAsyncHandler {
        public void ProcessRequest (HttpContext context) {
        }


        public bool IsReusable {
            get { return false; }
        }


        #region IHttpAsyncHandler Members

        public IAsyncResult BeginProcessRequest (HttpContext context, AsyncCallback cb, object extraData) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void EndProcessRequest (IAsyncResult result) {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
