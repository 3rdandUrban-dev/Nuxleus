using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;
using Nuxleus.Core;
using System.Net;

namespace Nuxleus.Extension.Aws.SimpleDb
{

    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false, ElementName = "GetAttributes")]
    public class GetAttributesTask : ITask, IXmlSerializable
    {

        public GetAttributesTask()
        {
            Transaction = new Transaction
            {
                Request = new DomainMetadataRequest(),
                Response = new DomainMetadataResponse()
            };
            TaskID = System.Guid.NewGuid();
        }

        [XmlElementAttribute(ElementName = "DomainName")]
        public string DomainName
        {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "ItemName")]
        public string ItemName
        {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "AttributeName")]
        public List<String> AttributeName
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
            return HttpWebService<GetAttributesTask, GetAttributesResponse>.CallWebServiceAsync(this);
        }

        public IResponse Invoke()
        {
            return HttpWebService<GetAttributesTask, GetAttributesResponse>.CallWebService(this);
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
