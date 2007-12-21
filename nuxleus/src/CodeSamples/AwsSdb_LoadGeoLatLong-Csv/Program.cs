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
        string awsAccessKey =
          System.Environment.GetEnvironmentVariable("SDB_ACCESS_KEY");
        string awsSecretKey =
          System.Environment.GetEnvironmentVariable("SDB_SECRET_KEY");

        string domainName = "place";

        // Create a new instance of the SDB class
        HttpQueryConnection connection = new HttpQueryConnection(awsAccessKey, awsSecretKey, "http://sdb.amazonaws.com/");
        Sdb sdb = new Sdb(connection);

        // Step 1:
        // Create the domain
        System.Console.WriteLine();
        System.Console.WriteLine("Step 1: Creating the domain.");

        try
        {
            sdb.CreateDomain(domainName);
        }
        catch (SdbException ex)
        {
            handleException(ex);
        }

        Domain domain = sdb.GetDomain(domainName);

        Dictionary<int, string> countries = new Dictionary<int, string>();

        using (StreamReader csvReader = new StreamReader("./hip_countries.csv"))
        {
            string inputLine = "";

            while ((inputLine = csvReader.ReadLine()) != null)
            {
                string[] inputArray = inputLine.Split(new char[] { ',' });
                int key = int.Parse((string)inputArray.GetValue(0));
                string value = (string)inputArray.GetValue(1);
                if (!countries.ContainsKey(key))
                {
                    countries.Add(key, value);
                }
            }
        }

        Dictionary<int, int> ipv4_country = new Dictionary<int, int>();

        using (StreamReader csvReader = new StreamReader("./hip_ip4_country.csv"))
        {
            string inputLine = "";

            while ((inputLine = csvReader.ReadLine()) != null)
            {
                string[] inputArray = inputLine.Split(new char[] { ',' });
                int key = int.Parse((string)inputArray.GetValue(0));
                int value = int.Parse((string)inputArray.GetValue(1));
                if(!ipv4_country.ContainsKey(key))
                {
                    ipv4_country.Add(key, value);
                }
            }
        }

        //Dictionary<string, string[]> city = new Dictionary<string, string[]>();

        using (StreamReader csvReader = new StreamReader("./hip_ip4_city_lat_lng.csv"))
        {
            string inputLine = "";

            while ((inputLine = csvReader.ReadLine()) != null)
            {
                //city.Add((string)inputArray.GetValue(1), new string[] {(string)inputArray.GetValue(0), (string)inputArray.GetValue(2), (string)inputArray.GetValue(3)});

                string[] inputArray = inputLine.Split(new char[] { ',' });
                string city = (string)inputArray.GetValue(1);
                string country = "";
                string countryCode = "";
                int m_country;
                string m_countryCode;
                if (ipv4_country.TryGetValue(int.Parse((string)inputArray.GetValue(0)), out m_country))
                {
                    country = m_country.ToString();

                    if (countries.TryGetValue(m_country, out m_countryCode))
                    {
                        countryCode = m_countryCode.ToString();
                    }
                    else
                    {
                        countryCode = "unknown";
                    }

                }
                else
                {
                    country = "unknown";
                }


                string itemName = String.Format("{0}:{1}", city, country);
                Item item = domain.GetItem(itemName.GetHashCode().ToString());

                ArrayList attributes = new ArrayList();
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("place", (string)inputArray.GetValue(1)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("lat", (string)inputArray.GetValue(2)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("long", (string)inputArray.GetValue(3)));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("country", country));
                attributes.Add(new Nuxleus.Extension.Aws.Sdb.Attribute("countryCode", countryCode));

                try
                {
                    item.PutAttributes(attributes);
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
}