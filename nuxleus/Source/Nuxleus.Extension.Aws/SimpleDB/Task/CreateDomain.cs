using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using EeekSoft.Asynchronous;
using Nuxleus.Extension.AWS.SimpleDB.Model;

namespace Nuxleus.Extension.AWS.SimpleDB {

    [XmlTypeAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public struct CreateDomain : ITask {

        string m_domainName;
        List<String> m_attributeNameArray;
        static Guid m_taskID = new Guid();
        static IRequest m_request = new GetAttributesRequest();

        [XmlElementAttribute(ElementName = "DomainName")]
        public string DomainName {
            get { return m_domainName; }
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

        public IResponse Response { get; set; }

        public IEnumerable<IAsync> Invoke<T>(Dictionary<IRequest, T> responseList) {
            return SimpleDBService<CreateDomain>.CallWebService<T>(this, Request, responseList);
        }
    }
}
