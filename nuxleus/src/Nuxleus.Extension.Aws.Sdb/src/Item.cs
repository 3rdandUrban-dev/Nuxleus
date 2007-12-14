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
using System.Xml;
using com.amazonaws;

namespace com.amazon.sdb
{
  public class Item {
    private IAwsConnection connection;
    public string Name;
    private string domain;

    public Item(IAwsConnection connection, string domainName, 
		string itemName) 
    {
      this.connection = connection;
      domain = domainName;
      Name = itemName;
    }

    public GetAttributesResponse GetAttributes() 
    {
      string uri = "";

      SortedList parameters = new SortedList();

      parameters.Add("ItemName",Name);
      parameters.Add("DomainName",domain);

      string response = connection.MakeRequest(uri,"GetAttributes", parameters);

      return new GetAttributesResponse(response);
    }

    public Response PutAttributes(IList attributes)
    {
      return PutAttributes(attributes,false);
    }

    public Response PutAttributes(IList attributes, bool replace)
    {
      string uri = "";

      SortedList parameters = new SortedList();

      parameters.Add("ItemName",Name);
      parameters.Add("DomainName",domain);

      if (replace) {
	parameters.Add("Replace","true");
      }

      int i = 0;
      foreach (Attribute attribute in attributes) {
	parameters.Add("Attribute."+i+".Name",attribute.Name);
	parameters.Add("Attribute."+i+".Value",attribute.Value);
	++i;
      }

      string response = connection.MakeRequest(uri,"PutAttributes",
					       parameters);
      return new Response("PutAttributesResponse",response);
    }

    public Response DeleteAttributes(IList attributes)
    {
      string uri = "";

      SortedList parameters = new SortedList();

      parameters.Add("ItemName",Name);
      parameters.Add("DomainName",domain);

      if (attributes != null) {
	int i = 0;
	foreach (Attribute attribute in attributes) {
	  parameters.Add("Attribute."+i+".Name",attribute.Name);
	  if (attribute.Value != null) {
	    parameters.Add("Attribute."+i+".Value",attribute.Value);
	  }
	  ++i;
	}
      }

      string response = 
	connection.MakeRequest(uri,"DeleteAttributes",parameters);
      return new Response("DeleteAttributesResponse",response);
    }

    public void DeleteAttributes()
    {
      DeleteAttributes(null);
    }
  }
}
