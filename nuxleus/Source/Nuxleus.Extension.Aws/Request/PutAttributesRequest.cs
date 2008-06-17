using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuxleus.Extension.AWS.SimpleDB;
using System.Xml.Linq;
using Nuxleus.MetaData;

namespace Nuxleus.Extension.AWS.SimpleDB {

    public struct PutAttributesRequest : IRequest {

        String m_requestBody;

        #region IRequest Members

        public KeyValuePair<string, string>[] Headers {
            get {
                return 
                    new KeyValuePair<string, string>[] {
                        new KeyValuePair<string,string>("SOAPAction", LabelAttribute.FromMember(RequestType)),
                    };
            }
        }

        public RequestType RequestType {
            get {
                return RequestType.PutAttributes;
            }
        }

        public String RequestMessage {
            get {
                return m_requestBody;
            }
            set {
                m_requestBody = value;
            }
        }

        #endregion
    }
}
