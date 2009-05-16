using System;
using System.Collections.Generic;
using Nuxleus.Extension.Aws.SimpleDb;
using Nuxleus.MetaData;
using Nuxleus.Core;

namespace Nuxleus.Extension.Aws.SimpleDb
{

    public class BatchPutAttributesRequest : IRequest
    {
        static List<KeyValuePair<string, string>> m_headers = new List<KeyValuePair<string, string>>();

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
                return SdbRequestType.BatchPutAttributes;
            }
        }

        public String RequestMessage
        {
            get;
            set;
        }
    }
}
