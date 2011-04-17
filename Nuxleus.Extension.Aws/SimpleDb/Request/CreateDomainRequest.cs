using System;
using System.Collections.Generic;
using Nuxleus.Extension.Aws.SimpleDb;
using Nuxleus.MetaData;
using Nuxleus.Core;

namespace Nuxleus.Extension.Aws.SimpleDb
{

    public class CreateDomainRequest : IRequest
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
                return SdbRequestType.CreateDomain;
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
