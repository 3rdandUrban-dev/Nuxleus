using System;
using System.Collections.Generic;
using Nuxleus.Extension.Aws.SimpleDb;
using Nuxleus.MetaData;
using Nuxleus.Core;

namespace Nuxleus.Extension.Aws.SimpleDb
{

    public class PutAttributesRequest : IRequest
    {

        String m_requestBody;
        static List<KeyValuePair<string, string>> m_headers = new List<KeyValuePair<string, string>>();

        #region IRequest Members

        public List<KeyValuePair<string, string>> Headers
        {
            get
            {
                return m_headers;
            }
        }

        public Enum RequestType
        {
            get
            {
                return SdbRequestType.PutAttributes;
            }
        }

        public String RequestMessage
        {
            get
            {
                return m_requestBody;
            }
            set
            {
                m_requestBody = value;
            }
        }

        #endregion
    }
}
