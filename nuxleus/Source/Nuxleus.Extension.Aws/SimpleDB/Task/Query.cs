using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using EeekSoft.Asynchronous;

namespace Nuxleus.Extension.AWS.SimpleDB {

    [XmlTypeAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public struct Query : ITask {

        string m_domainName;
        string m_queryExpression;
        string m_maxNumberOfItems;
        string m_nextToken;
        static Guid m_taskID = new Guid();

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

        public IResponse Response { get; set; }

        public Guid TaskID {
            get { return m_taskID; }
        }

        public IEnumerable<IAsync> Invoke<T>(Dictionary<IRequest, T> responseList) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
