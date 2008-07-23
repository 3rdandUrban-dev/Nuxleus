using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;
using Nuxleus.Extension.Aws.SimpleDb.Model;

namespace Nuxleus.Extension.Aws.SimpleDb {

    [XmlTypeAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public struct GetAttributes : ITask {

        string m_domainName;
        string m_itemName;
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

        [XmlElementAttribute(ElementName = "ItemName")]
        public string ItemName {
            get { return m_itemName; }
            set { m_itemName = value; }
        }

        [XmlElementAttribute(ElementName = "AttributeName")]
        public List<String> AttributeName {
            get { return m_attributeNameArray; }
            set { m_attributeNameArray = value; }
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
            return HttpWebService<GetAttributes>.CallWebService(this);
        }

        public IResponse Invoke(ITask task) {
            Init();
            return HttpWebService<GetAttributes>.CallWebServiceSync(task);
        }

        void Init() {
            m_request = new GetAttributesRequest();
            m_response = new GetAttributesResponse();
            m_taskID = System.Guid.NewGuid();
        }
    }
}
