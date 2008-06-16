using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Globalization;
using Nuxleus.MetaData;
using System.Collections;

namespace Nuxleus.Extension.AWS.SimpleDB {

    public enum RequestType {
        [Label("Query")]
        Query,
        [Label("CreateDomain")]
        CreateDomain,
        [Label("DeleteDomain")]
        DeleteDomain,
        [Label("ListDomains")]
        ListDomains,
        [Label("PutAttributes")]
        PutAttributes,
        [Label("DeleteAttributes")]
        DeleteAttributes,
        [Label("GetAttributes")] 
        GetAttributes 
    }

    public struct SdbAction {

        static string AWS_PUBLIC_KEY = System.Environment.GetEnvironmentVariable("AWS_PUBLIC_KEY");
        static string AWS_PRIVATE_KEY = System.Environment.GetEnvironmentVariable("AWS_PRIVATE_KEY");
        static XNamespace aws = "http://sdb.amazonaws.com/doc/2007-11-07/";
        static XNamespace i = "http://www.w3.org/2001/XMLSchema-instance";

        public static XElement Query(string domainName, string maxNumberOfTokens, string nextToken, string queryExpression) {
            return 
                new XElement(aws + "Query",
                    new XElement(aws + "DomainName", domainName),
                    new XElement(aws + "MaxNumberOfItems", maxNumberOfTokens),
                    (maxNumberOfTokens == null)
                        ?
                            new XElement(aws + "NextToken",
                                new XAttribute(i + "nil", "true"))
                        :
                            new XElement(aws + "NextToken", nextToken),
                    new XElement(aws + "QueryExpression", queryExpression),
                    GetAuthorizationElements("Query")
                );
        }

        public static XElement CreateDomain(string domainName) {
            return 
                new XElement(aws + "CreateDomain",
                    new XElement(aws + "DomainName", domainName),
                    GetAuthorizationElements("CreateDomain")
                );
        }

        public static XElement DeleteDomain(string domainName) {
            return 
                new XElement(aws + "DeleteDomain",
                    new XElement(aws + "DomainName", domainName),
                    GetAuthorizationElements("DeleteDomain")
                );
        }

        public static XElement ListDomains(string maxNumberOfDomains, string nextToken) {
            return 
                new XElement(aws + "ListDomains",
                    (maxNumberOfDomains == null)
                        ?
                            new XElement(aws + "MaxNumberOfDomains",
                                new XAttribute(i + "nil", "true"))
                        :
                            new XElement(aws + "MaxNumberOfDomains", maxNumberOfDomains),
                    (nextToken == null)
                        ?
                            new XElement(aws + "NextToken",
                                new XAttribute(i + "nil", "true"))
                        :
                            new XElement(aws + "NextToken", nextToken),
                    GetAuthorizationElements("ListDomains")
                );
        }

        public static XElement PutAttributes(string domainName, string itemName, ArrayList attributes) {
            return
                new XElement(aws + "PutAttributesRequest",
                    new XElement(aws + "DomainName", domainName),
                    new XElement(aws + "ItemName", itemName),
                    CreateSdbAttributeElements(AttributeActionType.PUT, attributes),
                    GetAuthorizationElements("PutAttributes")
                );
        }

        public static XElement DeleteAttributes(string domainName, string itemName, ArrayList attributes) {
            return 
                new XElement(aws + "DeleteAttributes",
                    new XElement(aws + "DomainName", domainName),
                    new XElement(aws + "ItemName", itemName),
                    CreateSdbAttributeElements(AttributeActionType.DELETE, attributes),
                    GetAuthorizationElements("DeleteAttributes")
                );
        }

        public static XElement GetAttributes(string domainName, string itemName, params String[] attributeNames) {
            return 
                new XElement(aws + "GetAttributes",
                    new XElement(aws + "DomainName", domainName),
                    new XElement(aws + "ItemName", itemName),
                    CreateSdbAttributeElements(AttributeActionType.GET, attributeNames),
                    GetAuthorizationElements("GetAttributes")
                );
        }

        private static XElement[] GetAuthorizationElements(string action) {
            String timestamp = GetFormattedTimestamp();
            return 
                new XElement[] { 
                    new XElement(aws + "AWSAccessKeyId", AWS_PUBLIC_KEY),
                    new XElement(aws + "Timestamp", timestamp),
                    GetSignature(action, timestamp)
                };

        }

        private static XElement[] CreateSdbAttributeElements(AttributeActionType attributeActionType, ArrayList attributes) {
            XElement[] xElements = new XElement[attributes.Count];
            int i = 0;
            foreach (Nuxleus.Extension.AWS.SimpleDB.Model.Attribute attribute in attributes) {
                switch(attributeActionType){
                    case AttributeActionType.PUT:
                    case AttributeActionType.DELETE:
                        xElements[i] = CreateSdbAttributeElement(attribute);
                        break;
                    case AttributeActionType.GET:
                    default:
                        break;
                }
                i++;
            }
            return xElements;
        }

        private static XElement[] CreateSdbAttributeElements(AttributeActionType attributeActionType, params string[] sdbAttributeNames) {
            XElement[] xElements = new XElement[sdbAttributeNames.Length];
            int i = 0;
            foreach (string attribute in sdbAttributeNames) {
                switch (attributeActionType) {
                    case AttributeActionType.GET:
                        xElements[i] = new XElement(aws + "AttributeName", attribute);
                        break;
                    case AttributeActionType.PUT:
                    case AttributeActionType.DELETE:            
                    default:
                        break;                   
                }
                i++;
            }
            return xElements;
        }

        private static XElement CreateSdbAttributeElement(Nuxleus.Extension.AWS.SimpleDB.Model.Attribute attribute) {
            return new XElement(aws + "Attribute",
                new XElement(aws + "Name", attribute.Name),
                new XElement(aws + "Value", attribute.Value),
                new XElement(aws + "Replace", attribute.Replace)
            );
        }

        private static XElement GetSignature(string action, string timestamp) {
            return new XElement(aws + "Signature", Sign(String.Format("{0}{1}", action, timestamp), AWS_PRIVATE_KEY));
        }

        private static String Sign(String data, String key) {
            Encoding encoding = new UTF8Encoding();
            HMACSHA1 signature = new HMACSHA1(encoding.GetBytes(key));
            return Convert.ToBase64String(signature.ComputeHash(
                encoding.GetBytes(data.ToCharArray())));
        }

        private static String GetFormattedTimestamp() {
            DateTime dateTime = DateTime.Now;
            return 
                new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                             dateTime.Hour, dateTime.Minute, dateTime.Second,
                             dateTime.Millisecond, DateTimeKind.Local)
                            .ToUniversalTime().ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", CultureInfo.InvariantCulture);
        }
    }

    public enum AttributeActionType { GET, PUT, DELETE }

    public struct SdbAttribute {

        string m_name;
        string m_value;
        static bool m_replace = false;

        public string Name { get { return m_name; } set { m_name = value; } }
        public string Value { get { return m_value; } set { m_value = value; } }
        public bool Replace { get { return m_replace; } set { m_replace = value; } }

        public SdbAttribute(string name, string value) {
            m_name = name;
            m_value = value;
        }

        public SdbAttribute(string name, string value, bool replace) {
            m_name = name;
            m_value = value;
            m_replace = replace;
        }
    }
}
