using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuxleus.Extension.Aws.SimpleDb {

    public struct QueryResponse : IResponse {
        #region IResponse Members

        public KeyValuePair<string, string>[] Headers {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string Response {
            get {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
