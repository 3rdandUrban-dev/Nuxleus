using System.IO;
using System.Net;
using System.Xml.Serialization;
using Nuxleus.Core;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public class SelectResponse : IResponse, IXmlSerializable
    {
        static readonly string m_sdbNamespace = "http://sdb.amazonaws.com/doc/2007-11-07/";
        static readonly XName m_nextToken = XName.Get("NextToken", m_sdbNamespace);
        static readonly XName m_selectResult = XName.Get("SelectResult", m_sdbNamespace);
        static readonly XName m_item = XName.Get("Item", m_sdbNamespace);
        static readonly XName m_itemName = XName.Get("Name", m_sdbNamespace);
        static readonly XName m_itemAttributes = XName.Get("Attribute", m_sdbNamespace);
        static readonly XName m_attributeName = XName.Get("Name", m_sdbNamespace);
        static readonly XName m_attributeValue = XName.Get("Value", m_sdbNamespace);

        public WebHeaderCollection Headers {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "SelectResult")]
        public IResult Result {
            get;
            set;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new System.NotImplementedException();
        }

        public void ReadXml(XmlReader reader) {
            SelectResult result = new SelectResult();
            XElement elements = XElement.Load(reader);
            XElement selectResult = elements.Element(m_selectResult);

            try {
                result.NextToken = selectResult.Element(m_nextToken).Value;
            }
            catch {
                result.NextToken = String.Empty;
            }

            result.Metadata = Utility.GetSdbResponseMetadata(elements);

            result.Item = new List<Item>();

            IEnumerable<XElement> items = selectResult.Descendants(m_item);


            foreach (XElement item in items) {
                IEnumerable<XElement> itemAttributes = item.Elements(m_itemAttributes);
                List<SimpleDb.Attribute> attributes = new List<SimpleDb.Attribute>();

                foreach (XElement attribute in itemAttributes) {
                    attributes.Add(new Attribute {
                        Name = attribute.Element(m_attributeName).Value,
                        Value = attribute.Element(m_attributeValue).Value
                    });
                }
                result.Item.Add(new Item {
                    ItemName = item.Element(m_itemName).Value,
                    Attribute = attributes
                });
            }

            Result = result;
        }

        public void WriteXml(XmlWriter writer) {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
