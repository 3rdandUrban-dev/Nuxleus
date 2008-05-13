using System;
using System.Collections.Generic;
using System.Text;
using Nuxleus.Agent;
using log4net;
using log4net.Config;

namespace Nuxleus.Logging {

    public class Agent : IAgent {

        static readonly ILog m_loggerInstance = LogManager.GetLogger(typeof(Agent));

        #region IAgent Members

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
            BasicConfigurator.Configure();
            return m_loggerInstance;
        }

        #endregion
    }
}
