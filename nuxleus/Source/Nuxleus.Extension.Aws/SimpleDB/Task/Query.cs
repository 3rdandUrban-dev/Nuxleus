using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;
using Nuxleus.Extension;

namespace Nuxleus.Extension.Aws.SimpleDb {

    [XmlTypeAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public struct Query : ITask {

        string m_domainName;
        string m_queryExpression;
        string m_maxNumberOfItems;
        string m_nextToken;
        Guid m_taskID;
        IRequest m_request;
        IResponse m_response;

        /// <summary>
        /// gets or sets the domain name to query
        /// </summary>
        [XmlElementAttribute(ElementName = "DomainName")]
        public String DomainName {
            get {
                return m_domainName;
            }
            set { m_domainName = value; }
        }

        /// <summary>
        /// gets or sets the query expression
        /// </summary>
        [XmlElementAttribute(ElementName = "QueryExpression")]
        public String QueryExpression {
            get { 
                return m_queryExpression; 
            }
            set { 
                m_queryExpression = value; 
            }
        }

        /// <summary>
        /// gets or sets the maximum number of items to return in the result
        /// </summary>
        [XmlElementAttribute(ElementName = "MaxNumberOfItems")]
        public String MaxNumberOfItems {
            get { return m_maxNumberOfItems; }
            set { m_maxNumberOfItems = value; }
        }

        /// <summary>
        /// gets or sets the token that should be used if more results exist
        /// </summary>
        [XmlElementAttribute(ElementName = "NextToken")]
        public String NextToken {
            get { return m_nextToken; }
            set { m_nextToken = value; }
        }

        #region ITask Members

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

        public Guid TaskID {
            get { return m_taskID; }
        }

        public IEnumerable<IAsync> InvokeAsync() {
            Init();
            return HttpWebService<Query>.CallWebService(this);
        }

        public IResponse Invoke(ITask task) {
            Init();
            return HttpWebService<Query>.CallWebServiceSync(task);
        }

        void Init() {
            m_request = new QueryRequest();
            m_response = new QueryResponse();
            m_taskID = System.Guid.NewGuid();
        }

        #endregion
    }
}
