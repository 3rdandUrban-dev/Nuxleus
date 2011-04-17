using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;
using Nuxleus.Core;
using System.Net;


namespace Nuxleus.Extension.Aws.SimpleDb
{
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false, ElementName = "PutAttributes")]
    public class PutAttributesTask : ITask, IXmlSerializable
    {
        public PutAttributesTask()
        {
            Transaction = new Transaction
            {
                Request = new PutAttributesRequest(),
                Response = new PutAttributesResponse()
            };
            TaskID = System.Guid.NewGuid();
        }

        [XmlElementAttribute(ElementName = "DomainName")]
        public Domain DomainName
        {
            get;
            set;
        }

        public Item Item
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="responseList"></param>
        /// <returns>IEnumerable&lt;IAsync&gt;</returns>
        public IEnumerable<IAsync> InvokeAsync()
        {
            try
            {
                return HttpWebService<PutAttributesTask, PutAttributesResponse>.CallWebServiceAsync(this);
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
                return HttpWebService<PutAttributesTask, PutAttributesResponse>.CallWebService(this);
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
            XElement xElement = XElement.Load(reader);
            Item = new Item { ItemName = xElement.Element(XName.Get("ItemName")).Value, Attribute = new List<Attribute>() };

            foreach (XElement attribute in xElement.Elements(XName.Get("Attribute")))
            {
                Item.Attribute.Add(new Attribute { Name = xElement.Element(XName.Get("ItemName")).Value, Value = xElement.Element(XName.Get("ItemName")).Value });
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("DomainName", DomainName.Name);
            writer.WriteElementString("ItemName", Item.ItemName);
            foreach (Attribute attribute in Item.Attribute)
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
            Utilities.GetAuthorizationElements("PutAttributes", writer);
        }
    }
}
