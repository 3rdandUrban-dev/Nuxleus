using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;
using Nuxleus.Extension.AWS.SimpleDB.Model;

namespace Nuxleus.Extension.AWS.SimpleDB {

    [XmlTypeAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public struct DeleteDomain : ITask {

        string m_domainName;
        List<String> m_attributeNameArray;
        Guid m_taskID;
        IRequest m_request;
        IResponse m_response;

        [XmlElementAttribute(ElementName = "DomainName")]
        public string DomainName {
            get {
                return m_domainName;
            }
            set { m_domainName = value; }
        }

        public Guid TaskID {
            get { return m_taskID; }
        }

        public IRequest Request {
            get {
                return m_request;
            }
        }

        public IResponse Response {
            get {
                return m_response;
            }
        }

        public IEnumerable<IAsync> Invoke<T>(Dictionary<IRequest, T> responseList) {
            Init();
            return SimpleDBService<CreateDomain>.CallWebService<T>(this, Request, responseList);
        }

        void Init() {
            m_request = new DeleteDomainRequest();
            m_response = new DeleteDomainResponse();
            m_taskID = System.Guid.NewGuid();
        }
    }
}
