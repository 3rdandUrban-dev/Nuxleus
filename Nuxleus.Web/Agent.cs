using System;
using System.Collections.Generic;
using System.Text;
using Nuxleus.Core;
using log4net;
using log4net.Config;
using System.Web;

namespace Nuxleus.Web {

    public struct Agent<T> : IAgent {

        static readonly ILog m_loggerInstance = LogManager.GetLogger(typeof(T));

        public System.Collections.Hashtable Result {
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

        public static ILog GetBasicLogger () {
            XmlConfigurator.Configure(new System.IO.FileInfo(HttpContext.Current.Server.MapPath("~/log4net.config")));
            return m_loggerInstance;
        }

        #region IAgent Members


        public void Invoke() {
            throw new NotImplementedException();
        }

        #endregion
    }
}
