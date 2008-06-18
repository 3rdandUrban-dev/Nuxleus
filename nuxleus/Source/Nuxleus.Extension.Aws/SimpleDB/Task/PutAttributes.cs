using System.Collections.Generic;
using System.Xml.Serialization;
using EeekSoft.Asynchronous;
using Nuxleus.Extension.AWS.SimpleDB.Model;


namespace Nuxleus.Extension.AWS.SimpleDB {

    [XmlTypeAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public struct PutAttributes : ITask {

        string m_domainName;
        string m_itemName;
        List<Attribute> m_attributeArray;
        static System.Guid m_taskID = new System.Guid();
        static IRequest m_request = new PutAttributesRequest();

        [XmlElementAttribute(ElementName = "DomainName")]
        public string DomainName {
            get { return m_domainName; }
            set { m_domainName = value; }
        }

        [XmlElementAttribute(ElementName = "ItemName")]
        public string ItemName {
            get { return m_itemName; }
            set { m_itemName = value; }
        }

        [XmlElementAttribute(ElementName = "Attribute")]
        public List<Attribute> Attribute {
            get { return m_attributeArray; }
            set { m_attributeArray = value; }
        }

        public System.Guid TaskID {
            get { return m_taskID; }
        }

        public IRequest Request {
            get {
                return m_request;
            }
        }

        public IResponse Response { get; set; }

        public IEnumerable<IAsync> Invoke<T>(Dictionary<IRequest, T> responseList) {
            return SimpleDBService<PutAttributes>.CallWebService<T>(this, Request, responseList);
        }
    }
}
