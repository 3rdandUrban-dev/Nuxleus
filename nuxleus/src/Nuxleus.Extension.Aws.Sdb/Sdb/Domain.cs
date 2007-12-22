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
using System.Web;
using Nuxleus.Extension.Aws;

namespace Nuxleus.Extension.Aws.Sdb
{
    public struct Domain
    {
        IAwsConnection m_connection;
        string m_name;


        public Domain (IAwsConnection connection, string domainName)
        {
            m_connection = connection;
            m_name = domainName;
        }

        public QueryResponse Query (string queryExpression)
        {
            SortedList parameters = new SortedList();

            parameters.Add("DomainName", m_name);

            if (queryExpression != null)
            {
                parameters.Add("QueryExpression", queryExpression);
            }

            string response = m_connection.MakeRequest("", "Query", parameters);

            return new QueryResponse(m_connection, m_name, response);
        }

        public QueryResponse Query ()
        {
            return Query(null);
        }

        public Item GetItem (string itemName)
        {
            return new Item(m_connection, m_name, itemName);
        }
    }
}
