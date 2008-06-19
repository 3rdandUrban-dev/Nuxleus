using System;
using System.Collections.Generic;
using Nuxleus.Extension.Aws.SimpleDb;
using Nuxleus.MetaData;

namespace Nuxleus.Extension.Aws.SimpleDb {

    public struct GetAttributesRequest : IRequest {

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
                return RequestType.GetAttributes;
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
