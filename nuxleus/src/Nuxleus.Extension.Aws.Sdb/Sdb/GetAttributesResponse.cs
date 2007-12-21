/* -*- Mode: Java; c-basic-offset: 2 -*- */
/*
 * This software code is made available "AS IS" without warranties of any
 * kind.  You may copy, display, modify and redistribute the software
 * code either by itself or as incorporated into your code; provided that
 * you do not remove any proprietary notices.  Your use of this software
 * code is at your own risk and you waive any claim against Amazon
 * Web Services LLC or its affiliates with respect to your use of
 * this software code.
 * 
 * @copyright 2007 Amazon Web Services LLC or its affiliates.
 *            All rights reserved.
 */
using System.Collections;
using System.Xml;

namespace Nuxleus.Extension.Aws.Sdb
{
    public class GetAttributesResponse : Response
    {
        private ArrayList attributes = new ArrayList();
        private string responseName = "GetAttributesResponse";

        public GetAttributesResponse (string response)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);

            CheckErrorResponse(doc);

            processResponse(responseName, doc);

            foreach (XmlNode node in doc.ChildNodes)
            {
                if (node.Name.Equals(responseName))
                {
                    foreach (XmlNode attributesNode in node.ChildNodes)
                    {
                        if (attributesNode.Name.Equals("Attribute"))
                        {
                            string name = "";
                            string value = "";

                            foreach (XmlNode attributeNode in
                                 attributesNode.ChildNodes)
                            {
                                if (attributeNode.Name.Equals("Name"))
                                {
                                    name = attributeNode.InnerText;
                                }
                                if (attributeNode.Name.Equals("Value"))
                                {
                                    value = attributeNode.InnerText;
                                }
                            }

                            attributes.Add(new Attribute(name, value));
                        }
                    }
                }
            }
        }

        public ICollection Attributes ()
        {
            return attributes;
        }
    }
}
