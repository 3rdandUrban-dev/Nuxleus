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
using System.Net;
using System.Collections;
using com.amazonaws;

namespace com.amazon.sdb
{
  public class Sdb {
    private IAwsConnection connection;

    public Sdb(IAwsConnection connection) {
      this.connection = connection;
    }

    public Response CreateDomain(string domainName)
    {
      SortedList parameters = new SortedList();
      parameters.Add("DomainName",domainName);

      string response = connection.MakeRequest("","CreateDomain",parameters);

      return new Response("CreateDomainResponse",response);
    }

    public Response DeleteDomain(string domainName)
    {
      SortedList parameters = new SortedList();
      parameters.Add("DomainName",domainName);

      string response = connection.MakeRequest("","DeleteDomain",parameters);

      return new Response("DeleteDomainResponse",response);
    }

    public Domain GetDomain(string domainName) {
      return new Domain(connection,domainName);
    }

    public ListDomainsResponse ListDomains() { 
      return new ListDomainsResponse(connection,
				     connection.MakeRequest("","List"));
    }
  }
}
