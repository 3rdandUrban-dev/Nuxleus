using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EeekSoft.Asynchronous;
using System.Xml.Linq;

namespace Nuxleus.Extension.AWS.SimpleDB {

    [XmlTypeAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public struct Query : ITask {

        string m_domainName;
        string m_queryExpression;
        string m_maxNumberOfItems;
        string m_nextToken;
        static System.Guid m_taskID = new System.Guid();

        [XmlElementAttribute(ElementName = "DomainName")]
        public String DomainName {
            get { return m_domainName; }
            set { m_domainName = value; }
        }

        [XmlElementAttribute(ElementName = "QueryExpression")]
        public String QueryExpression {
            get { return m_queryExpression; }
            set { m_queryExpression = value; }
        }

        [XmlElementAttribute(ElementName = "MaxNumberOfItems")]
        public String MaxNumberOfItems {
            get { return m_maxNumberOfItems; }
            set { m_maxNumberOfItems = value; }
        }


        #region ITask Members

        public RequestType RequestType {
            get { throw new NotImplementedException(); }
        }

        public IRequest Request {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public System.Xml.Linq.XElement[] GetXMLBody {
            get { throw new NotImplementedException(); }
        }

        public System.Guid TaskID {
            get { return m_taskID; }
        }

        public IEnumerable<IAsync> Invoke<T>(Dictionary<IRequest, T> responseList) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
