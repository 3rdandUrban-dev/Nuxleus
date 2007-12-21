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
using System;
using System.Collections;
using System.Xml;
using Nuxleus.Extension.Aws;

namespace Nuxleus.Extension.Aws.Sdb
{
    public class ListDomainsResponse : ListResponse
    {
        private ArrayList domains = new ArrayList();

        public ListDomainsResponse (IAwsConnection connection, string response)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);

            CheckErrorResponse(doc);

            foreach (XmlNode node in doc.ChildNodes)
            {
                if (node.Name.Equals("ListDomainsResponse"))
                {
                    foreach (XmlNode domainsNode in node.ChildNodes)
                    {
                        if (domainsNode.Name.Equals("Domains"))
                        {
                            foreach (XmlNode domainNode in domainsNode.ChildNodes)
                            {
                                if (domainNode.Name.Equals("Domain"))
                                {
                                    foreach (XmlNode nameNode in domainNode.ChildNodes)
                                    {
                                        if (nameNode.Name.Equals("Name"))
                                        {
                                            domains.Add(new Domain(connection, nameNode.InnerText));
                                        }
                                    }
                                }
                            }
                        }
                        if (domainsNode.Name.Equals("MoreToken"))
                        {
                            MoreToken = domainsNode.InnerText;
                        }
                    }
                }
            }
        }

        public IEnumerator Domains ()
        {
            return domains.GetEnumerator();
        }
    }
}
