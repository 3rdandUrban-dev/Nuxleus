using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;
using Nuxleus.Core;
using System.Net;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false, ElementName = "BatchPutAttributes")]
    public class BatchPutAttributesTask : ITask, IXmlSerializable
    {
        public BatchPutAttributesTask()
        {
            Transaction = new Transaction
            {
                Request = new BatchPutAttributesRequest(),
                Response = new BatchPutAttributesResponse()
            };
            TaskID = System.Guid.NewGuid();
        }

        [XmlElementAttribute(ElementName = "DomainName")]
        public Domain DomainName
        {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "Item")]
        public List<Item> Item
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
            try
            {
                return HttpWebService<BatchPutAttributesTask, BatchPutAttributesResponse>.CallWebServiceAsync(this);
            }
            catch (InvalidCastException e)
            {
                return null;
            }
            catch
            {
                return null;
                throw;
            }
        }

        public IResponse Invoke()
        {
            try
            {
                return HttpWebService<BatchPutAttributesTask, BatchPutAttributesResponse>.CallWebService(this);
            }
            catch (Exception e)
            {
                return Transaction.Response;
                throw;
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {

        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("DomainName", DomainName.Name);

            foreach (Item item in Item)
            {
                writer.WriteStartElement("Item");
                writer.WriteElementString("ItemName", item.ItemName);
                foreach (Attribute attribute in item.Attribute)
                {
                    writer.WriteStartElement("Attribute");
                    writer.WriteElementString("Name", attribute.Name);
                    writer.WriteElementString("Value", attribute.Value);
                    if (attribute.Replace != null)
                    {
                        writer.WriteElementString("Replace", attribute.Replace.ToString());
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            Utilities.GetAuthorizationElements("BatchPutAttributes", writer);
        }
    }
}
