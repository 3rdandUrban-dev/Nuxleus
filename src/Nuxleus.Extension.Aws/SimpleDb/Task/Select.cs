using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;
using Nuxleus.Core;
using ProtoBuf;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    [Serializable, ProtoContract]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false, ElementName = "Select")]
    public class SelectTask : ITask, IXmlSerializable
    {
        static Guid m_taskID;

        static SelectTask()
        {
            m_taskID = System.Guid.NewGuid();
        }
        public SelectTask()
        {
            Transaction = new Transaction
            {
                Request = new PutAttributesRequest(),
                Response = new PutAttributesResponse()
            };
        }

        /// <summary>
        /// gets or sets the domain name to query
        /// </summary>
        [XmlElementAttribute(ElementName = "DomainName")]
        [ProtoMember(1, Name = "DomainName", IsRequired = true)]
        public Domain DomainName
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets the query expression
        /// </summary>
        [XmlElementAttribute(ElementName = "SelectExpression")]
        [ProtoMember(2, Name = "SelectExpression", IsRequired = true)]
        public String SelectExpression
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets the maximum number of items to return in the result
        /// </summary>
        [XmlElementAttribute(ElementName = "MaxNumberOfItems")]
        [ProtoMember(3, Name = "MaxNumberOfItems", IsRequired = false)]
        public String MaxNumberOfItems
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets the token that should be used if more results exist
        /// </summary>
        [XmlElementAttribute(ElementName = "NextToken")]
        [ProtoMember(4, Name = "NextToken", IsRequired = false)]
        public String NextToken
        {
            get;
            set;

        }

        [XmlIgnore]
        public Guid TaskID
        {
            get { return m_taskID; }
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
            return HttpWebService<SelectTask, SelectResponse>.CallWebServiceAsync(this);
        }

        public IAsyncResult BeginInvoke(NuxleusAsyncResult asyncResult)
        {
            return new HttpWebService<SelectTask, SelectResponse>().BeginCallWebService(this, asyncResult);
        }

        public void EndInvoke(IAsyncResult asyncResult)
        {

        }

        public IResponse Invoke()
        {
            return HttpWebService<SelectTask, SelectResponse>.CallWebService(this);
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
            writer.WriteElementString("DomainName", DomainName.Name);
            writer.WriteElementString("SelectExpression", SelectExpression);
            writer.WriteElementString("MaxNumberOfItems", MaxNumberOfItems);
            writer.WriteElementString("NextToken", NextToken);
            Utilities.GetAuthorizationElements("Select", writer);
        }
    }
}
