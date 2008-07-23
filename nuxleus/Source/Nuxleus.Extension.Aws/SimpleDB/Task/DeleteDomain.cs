using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;
using Nuxleus.Extension.Aws.SimpleDb.Model;

namespace Nuxleus.Extension.Aws.SimpleDb {

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

        public IEnumerable<IAsync> InvokeAsync() {
            Init();
            return HttpWebService<CreateDomain>.CallWebService(this);
        }

        public IResponse Invoke(ITask task) {
            Init();
            return HttpWebService<DeleteDomain>.CallWebServiceSync(task);
        }

        void Init() {
            m_request = new DeleteDomainRequest();
            m_response = new DeleteDomainResponse();
            m_taskID = System.Guid.NewGuid();
        }
    }
}
