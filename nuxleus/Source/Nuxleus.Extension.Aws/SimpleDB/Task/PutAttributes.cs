using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EeekSoft.Asynchronous;
using System.Xml.Linq;
using Nuxleus.Extension.AWS.SimpleDB.Model;
using System.Collections;
using Nuxleus.MetaData;
using System.Net;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Threading;

namespace Nuxleus.Extension.AWS.SimpleDB {

    [XmlTypeAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public struct PutAttributes : ITask {

        static XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";

        string m_domainName;
        string m_itemName;
        List<Attribute> m_attributeArray;
        static System.Guid m_taskID = new System.Guid();
        static IRequest m_request = new PutAttributesRequest();

        static XNamespace aws = "http://sdb.amazonaws.com/doc/2007-11-07/";
        static XNamespace i = "http://www.w3.org/2001/XMLSchema-instance";

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
        public IEnumerable<IAsync> Invoke<T>(Dictionary<IRequest, T> responseList) {
            return SimpleDBService<PutAttributes>.CallWebService<PutAttributes,T>(this, Request, responseList);
        }
    }
}
