/* 
 * This software is derived from samples provided by Amazon Web Services.
 * The original copyright notice for this sample code follows.
 **************************************************************************
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
 * *************************************************************************
 * Extensions to this code base are Copyright (c) 2007 by M. David Peterson
 * The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
 * Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.
 */
using System.IO;
using System.Collections;
using Nuxleus.Extension.Aws;
using Nuxleus.Extension.Aws.Sdb;

class BasicSample
{
    static void handleException (SdbException ex)
    {
        System.Console.WriteLine("Failure: {0}: {1} ({2})", ex.ErrorCode,
                     ex.Message, ex.RequestId);
    }

    static void printAttributes (Item item)
    {
        System.Console.WriteLine();
        System.Console.WriteLine("Attributes for '{0}':", item.Name);
        try
        {
            GetAttributesResponse getAttributesResponse = item.GetAttributes();

            foreach (Attribute attribute in getAttributesResponse.Attributes())
            {
                System.Console.WriteLine("{0} => {1}.", attribute.Name,
                             attribute.Value);
            }
        }
        catch (SdbException ex)
        {
            handleException(ex);
        }
    }

    public static void Main (string[] args)
    {
        string awsAccessKey =
          System.Environment.GetEnvironmentVariable("SDB_ACCESS_KEY");
        string awsSecretKey =
          System.Environment.GetEnvironmentVariable("SDB_SECRET_KEY");

        string sample_domain = "sample_domain_1";
        string sample_item = "sample_item";

        // Create a new instance of the SDB class
        HttpQueryConnection connection = new HttpQueryConnection(awsAccessKey, awsSecretKey, "http://sdb.amazonaws.com");
        Sdb sdb = new Sdb(connection);

        // Step 1:
        // Create the domain
        System.Console.WriteLine();
        System.Console.WriteLine("Step 1: Creating the domain.");

        try
        {
            sdb.CreateDomain(sample_domain);
        }
        catch (SdbException ex)
        {
            handleException(ex);
        }

        // Get the sample domain
        Domain domain = sdb.GetDomain(sample_domain);

        // Get the sample item
        Item item = domain.GetItem(sample_item);

        // Step 2:
        // Create a series of attributes to associate with the sample eid.
        ArrayList attributes = new ArrayList();
        attributes.Add(new Attribute("name", "value"));
        attributes.Add(new Attribute("name", "2nd_value"));
        attributes.Add(new Attribute("name", "3rd_value"));
        attributes.Add(new Attribute("2nd_name", "4th_value"));
        attributes.Add(new Attribute("2nd_name", "5th_value"));

        System.Console.WriteLine();
        System.Console.WriteLine("Step 2: Creating initial attributes.");

        try
        {
            item.PutAttributes(attributes);
        }
        catch (SdbException ex)
        {
            handleException(ex);
        }

        // Print out the attributes for the item.
        printAttributes(item);

        // Step 3:
        // Delete the { "name", "3rd_value" } attribute.
        System.Console.WriteLine();
        System.Console.WriteLine("Step 3: Deleting the { \"name\", " +
                     "\"3rd_value\" } attribute.");

        attributes.Clear();
        attributes.Add(new Attribute("name", "3rd_value"));

        try
        {
            item.DeleteAttributes(attributes);
        }
        catch (SdbException ex)
        {
            handleException(ex);
        }

        // Print out the attributes for $item.
        printAttributes(item);

        // Step 4:
        // Delete all attributes with name "2nd_name".
        System.Console.WriteLine();
        System.Console.WriteLine("Step 4: Delete attributes with name " +
                     "\"2nd_name\".");

        attributes.Clear();
        attributes.Add(new Attribute("2nd_name"));

        try
        {
            item.DeleteAttributes(attributes);
        }
        catch (SdbException ex)
        {
            handleException(ex);
        }

        // Print out the attributes for $item.
        printAttributes(item);

        // Step 5:
        // Replace the value of the "name" attribute of the sample 
        // eID with the value "new_value".
        System.Console.WriteLine();
        System.Console.WriteLine("Step 5: Write value \"new_value\" " +
                     "to the \"name\" attribute.");

        attributes.Clear();
        attributes.Add(new Attribute("name", "new_value"));

        try
        {
            item.PutAttributes(attributes);
        }
        catch (SdbException ex)
        {
            handleException(ex);
        }

        // Print out the attributes for the item.
        printAttributes(item);

        // Step 6:
        // Find all of the eIDs which contain the attribute "name"
        // with the value "new_value".
        System.Console.WriteLine();
        System.Console.WriteLine("Step 6: Find all eID which contain" +
                     "the attribute { \"name\", " +
                     "\"new_value\" }.");

        try
        {
            QueryResponse queryResponse =
          domain.Query("[\"name\" = \"new_value\"]");

            // Print them out.
            System.Console.WriteLine();
            System.Console.WriteLine("Found items:");

            foreach (Item curitem in queryResponse.Items())
            {
                System.Console.WriteLine(curitem.Name);
            }
        }
        catch (SdbException ex)
        {
            handleException(ex);
        }

        // Step 7:
        // Delete the sample eID.
        System.Console.WriteLine();
        System.Console.WriteLine("Step 7: Delete the sample item.");

        try
        {
            item.DeleteAttributes();
        }
        catch (SdbException ex)
        {
            handleException(ex);
        }

        // Print out the attributes for $item.
        printAttributes(item);

        // Step 8:
        // Delete the domain
        System.Console.WriteLine();
        System.Console.WriteLine("Step 8: Deleting the domain.");

        try
        {
            sdb.DeleteDomain(sample_domain);
        }
        catch (SdbException ex)
        {
            handleException(ex);
        }
        System.Console.WriteLine();
    }
}