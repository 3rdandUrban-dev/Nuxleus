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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Nuxleus.Extension.Aws;
using Nuxleus.Extension.Aws.Sdb;

class BasicSample
{
    
    public static void Main (string[] args)
    {
        bool m_dryRun = false;

        if(args.Length > 0)
        {
            if (args[0] == "dryrun")
            {
                m_dryRun = true;
            }
        }

        string awsAccessKey =
          System.Environment.GetEnvironmentVariable("SDB_ACCESS_KEY");
        string awsSecretKey =
          System.Environment.GetEnvironmentVariable("SDB_SECRET_KEY");

        string domainName = "geonames";

        // Create a new instance of the SDB class
        HttpQueryConnection connection = new HttpQueryConnection(awsAccessKey, awsSecretKey, "http://sdb.amazonaws.com/");
        Sdb sdb = new Sdb(connection);
        Domain domain;

        System.Console.WriteLine();
        System.Console.WriteLine("Step 1: Creating the domain.");

        if (!m_dryRun)
        {
            try
            {
                sdb.CreateDomain(domainName);
                domain = sdb.GetDomain(domainName);
            }
            catch (SdbException ex)
            {
                handleException(ex);
            }
        }
    
        

        System.Console.WriteLine();
        System.Console.WriteLine("Step 2: Loading the GeoNames Data File.");

        using (StreamReader csvReader = new StreamReader("./allCountries.txt"))
        {
            string inputLine = "";

            while ((inputLine = csvReader.ReadLine()) != null)
            {

                string[] inputArray = inputLine.Split(new char[] { '\u0009' });
                IEnumerator alternateNames = ((string)inputArray.GetValue(3)).Split(new char[] { ',' }).GetEnumerator();

                System.Console.WriteLine(String.Format("Loading Item: {0}, with Place Name: {1}", (string)inputArray.GetValue(0), (string)inputArray.GetValue(1)));

                Item item = domain.GetItem((string)inputArray.GetValue(0));

                ArrayList attributes = new ArrayList();
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("name", (string)inputArray.GetValue(1)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("asciiname", (string)inputArray.GetValue(2)));

                while (alternateNames.MoveNext())
                {
                    attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("alternatenames", (string)alternateNames.Current));
                }

                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("latitude", (string)inputArray.GetValue(4)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("longitude", (string)inputArray.GetValue(5)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("feature_class", (string)inputArray.GetValue(6)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("feature_code", (string)inputArray.GetValue(7)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("country_code", (string)inputArray.GetValue(8)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("cc2", (string)inputArray.GetValue(9)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("admin1_code", (string)inputArray.GetValue(10)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("admin2_code", (string)inputArray.GetValue(11)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("admin3_code", (string)inputArray.GetValue(12)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("admin4_code", (string)inputArray.GetValue(13)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("population", (string)inputArray.GetValue(14)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("elevation", (string)inputArray.GetValue(15)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("gtopo30", (string)inputArray.GetValue(16)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("timezone", (string)inputArray.GetValue(17)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("modification_date", (string)inputArray.GetValue(1)));

                try
                {
                    if (!m_dryRun)
                    {
                        item.PutAttributes(attributes);
                    }
                    else
                    {
                        printAttributes(item);
                    }
                }
                catch (SdbException ex)
                {
                    handleException(ex);
                }
            }
        }
    }

    static void handleException (SdbException ex)
    {
        System.Console.WriteLine("Failure: {0}: {1} ({2})", ex.ErrorCode, ex.Message, ex.RequestId);
    }

    static void printAttributes (Item item)
    {
        System.Console.WriteLine();
        System.Console.WriteLine("Attributes for '{0}':", item.Name);
        try
        {
            GetAttributesResponse getAttributesResponse = item.GetAttributes();

            foreach (Nuxleus.Extension.Aws.Sdb.Attribute attribute in getAttributesResponse.Attributes())
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
}