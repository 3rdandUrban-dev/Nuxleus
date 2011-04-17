using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;
using Nuxleus.Core;
using System.Net;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false, ElementName = "DeleteAttributes")]
    public class DeleteAttributesTask : ITask, IXmlSerializable
    {
        public DeleteAttributesTask()
        {
            Transaction = new Transaction
            {
                Request = new DeleteAttributesRequest(),
                Response = new DeleteAttributesResponse()
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

        [XmlElementAttribute(ElementName = "Attribute")]
        public List<SimpleDb.Attribute> Attribute
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
            return HttpWebService<DeleteAttributesTask, DeleteAttributesResponse>.CallWebServiceAsync(this);
        }

        public IResponse Invoke()
        {
            return HttpWebService<DeleteAttributesTask, DeleteAttributesResponse>.CallWebService(this);
        }

        #region IXmlSerializable Members

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
            writer.WriteElementString("DomainName", DomainName);
            writer.WriteElementString("ItemName", ItemName);
            foreach (Attribute attribute in Attribute)
            {
                writer.WriteStartElement("Attribute");
                writer.WriteElementString("Name", attribute.Name);
                writer.WriteElementString("Value", attribute.Value);
                writer.WriteEndElement();
            }
            Utilities.GetAuthorizationElements("DeleteAttributes", writer);
        }

        #endregion
    }
}
