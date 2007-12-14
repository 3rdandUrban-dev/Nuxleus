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
using com.amazonaws;

namespace com.amazon.sdb
{
  public class Domain {
    private IAwsConnection connection;
    public string Name;

    public Domain(IAwsConnection connection, string domainName) 
    {
      this.connection = connection;
      Name = domainName;
    }

    public QueryResponse Query(string queryExpression) 
    {
      SortedList parameters = new SortedList();

      parameters.Add("DomainName",Name);

      if (queryExpression != null) {
	parameters.Add("QueryExpression",queryExpression);
      }

      string response = 
	connection.MakeRequest("","Query",parameters);

      return new QueryResponse(connection,Name,response);
    }

    public QueryResponse Query()
    {
      return Query(null);
    }

    public Item GetItem(string itemName)
    {
      return new Item(connection,Name,itemName);
    }
  }
}
