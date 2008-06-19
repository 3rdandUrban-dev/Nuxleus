using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;

namespace Nuxleus.Extension.Aws.SimpleDb {

    [XmlTypeAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public struct ListDomains : ITask {

        string m_maxNumberOfItems;
        string m_nextToken;
        Guid m_taskID;
        IRequest m_request;
        IResponse m_response;

        [XmlElementAttribute(ElementName = "MaxNumberOfItems")]
        public String MaxNumberOfItems {
            get {
                return m_maxNumberOfItems;
            }
            set { m_maxNumberOfItems = value; }
        }

        [XmlElementAttribute(ElementName = "NextToken")]
        public String NextToken {
            get { return m_nextToken; }
            set { m_nextToken = value; }
        }


        #region ITask Members

        public RequestType RequestType {
            get { throw new NotImplementedException(); }
        }

        public IRequest Request {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IResponse Response {
            get {
                return m_response;
            }
        }

        public Guid TaskID {
            get { return m_taskID; }
        }

        public IEnumerable<IAsync> Invoke<T>(Dictionary<IRequest, T> responseList) {
            Init();
            throw new NotImplementedException();
        }

        #endregion

        void Init() {
            m_request = new ListDomainsRequest();
            m_response = new ListDomainsResponse();
            m_taskID = System.Guid.NewGuid();
        }
    }
}
