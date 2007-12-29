using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Threading;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.SessionState;
using System.Collections;
using System.Text;
using System.Xml;
using Nuxleus.Configuration;
using System.Collections.Generic;
using Nuxleus.Cryptography;

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
