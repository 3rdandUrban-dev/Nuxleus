using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;
using Nuxleus.Core;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false, ElementName = "ListDomains")]
    public class ListDomainsTask : ITask, IXmlSerializable
    {
        public ListDomainsTask()
        {
            Transaction = new Transaction
            {
                Request = new ListDomainsRequest(),
                Response = new ListDomainsResponse()
            };
            TaskID = System.Guid.NewGuid();
        }

        [XmlElementAttribute(ElementName = "MaxNumberOfItems")]
        public String MaxNumberOfItems
        {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "NextToken")]
        public String NextToken
        {
            get;
            set;
        }


        [XmlIgnore]
        public Guid TaskID
        {
            get;
            private set;
        }

        [XmlIgnore]
        public ITransaction Transaction
        {
            get;
            private set;
        }

        [XmlIgnore]
        public HttpStatusCode StatusCode
        {
            get;
            set;
        }

        public IEnumerable<IAsync> InvokeAsync()
        {
            return HttpWebService<ListDomainsTask, ListDomainsResponse>.CallWebServiceAsync(this);
        }

        public IResponse Invoke()
        {
            return HttpWebService<ListDomainsTask, ListDomainsResponse>.CallWebService(this);
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
