using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Nuxleus.Core;
using System.Xml;
using System.Xml.Linq;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public class SelectResult : IResult, IXmlSerializable
    {
        static readonly string m_sdbNamespace = "http://sdb.amazonaws.com/doc/2007-11-07/";
        static readonly XName m_nextToken = XName.Get("NextToken", m_sdbNamespace);
        static readonly XName m_item = XName.Get("Item", m_sdbNamespace);
        static readonly XName m_itemName = XName.Get("ItemName", m_sdbNamespace);
        static readonly XName m_itemAttributes = XName.Get("Attribute", m_sdbNamespace);
        static readonly XName m_attributeName = XName.Get("Name", m_sdbNamespace);
        static readonly XName m_attributeValue = XName.Get("Value", m_sdbNamespace);

        [XmlElementAttribute(ElementName = "Item")]
        public List<Item> Item {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "NextToken", IsNullable = true)]
        public String NextToken {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "ResponseMetadata")]
        public SdbResponseMetadata Metadata {
            get;
            set;
        }

        public System.Xml.Linq.XElement ToXElement() {
            throw new NotImplementedException();
        }

        public string ToXmlString() {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return Item.Count.ToString();
        }

        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader) {
            //XElement elements = XElement.Load(reader);

            //Console.WriteLine("Elements: {0}", reader.ReadOuterXml());

            //if (elements.Element(m_nextToken).Value != null) {
            //    NextToken = (elements.Element(m_nextToken).Value);
            //}

            //Metadata = new SdbResponseMetadata {
            //    BoxUsage = Metadata.BoxUsage = elements.Element(XName.Get("BoxUsage", m_sdbNamespace)).Value,
            //    RequestId = elements.Element(XName.Get("RequestId", m_sdbNamespace)).Value
            //};

            //IEnumerable<XElement> items = elements.Descendants(m_item);

            //foreach (XElement item in items) {
            //    IEnumerable<XElement> itemAttributes = item.Elements(m_itemAttributes);
            //    List<SimpleDb.Attribute> attributes = new List<SimpleDb.Attribute>();
            //    foreach (XElement attribute in itemAttributes) {
            //        attributes.Add(new Attribute {
            //            Name = attribute.Element(m_attributeName).Value,
            //            Value = attribute.Element(m_attributeValue).Value
            //        });
            //    }
            //    Item.Add(new Item {
            //        ItemName = item.Element(m_itemName).Value,
            //        Attribute = attributes
            //    });
            //}
            throw new NotImplementedException();

        }

        public void WriteXml(System.Xml.XmlWriter writer) {
            throw new NotImplementedException();
        }
    }
}
