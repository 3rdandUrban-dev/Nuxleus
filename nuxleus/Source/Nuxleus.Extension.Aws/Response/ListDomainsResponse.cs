using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuxleus.Extension.AWS.SimpleDB {

    public struct ListDomainsResponse : IResponse {
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
