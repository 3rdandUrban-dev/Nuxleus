using System;
using System.Collections.Generic;
using Nuxleus.Core;
using Nuxleus.MetaData;

namespace Nuxleus.Extension.Aws.SimpleDb
{

    public class DeleteAttributesRequest : IRequest
    {

        String m_requestBody;

        #region IRequest Members

        public List<KeyValuePair<string, string>> Headers
        {
            get
            {
                return
                    new List<KeyValuePair<string, string>> { };
            }
        }

        public Enum RequestType
        {
            get
            {
                return SdbRequestType.DeleteAttributes;
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
