using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;
using Nuxleus.Core;

namespace Nuxleus.Extension.Aws.SimpleDb
{

    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false, ElementName = "CreateDomain")]
    public class CreateDomainTask : ITask, IXmlSerializable
    {
        public CreateDomainTask()
        {
            Transaction = new Transaction { Request = new CreateDomainRequest(), Response = new CreateDomainResponse() };
            TaskID = System.Guid.NewGuid();
        }

        [XmlElementAttribute(ElementName = "DomainName")]
        public string DomainName
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
            return HttpWebService<CreateDomainTask, CreateDomainResponse>.CallWebServiceAsync(this);
        }

        public IResponse Invoke()
        {
            return HttpWebService<CreateDomainTask, CreateDomainResponse>.CallWebService(this);
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
