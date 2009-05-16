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
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Nuxleus.Core.Net
{
    public class HttpQueryConnection : IAwsConnection
    {
        string m_awsAccessKey;
        string m_awsSecretKey;
        string m_baseUrl;

        public HttpQueryConnection (string awsAccessKey, string awsSecretKey, string baseUrl)
        {
            m_awsAccessKey = awsAccessKey;
            m_awsSecretKey = awsSecretKey;
            m_baseUrl = baseUrl;
        }

        private string GetSignature (string canonicalString)
        {
            Encoding ae = new UTF8Encoding();
            HMACSHA1 signature = new HMACSHA1(ae.GetBytes(m_awsSecretKey));
            string b64 = Convert.ToBase64String(signature.ComputeHash(ae.GetBytes(canonicalString.ToCharArray())));
            return HttpUtility.UrlEncode(b64);
        }

        public static string slurpInputStream (Stream stream)
        {
            Encoding encode = Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(stream, encode);
            const int stride = 4096;
            Char[] read = new Char[stride];

            int count = readStream.Read(read, 0, stride);
            StringBuilder data = new StringBuilder();
            while (count > 0)
            {
                string str = new string(read, 0, count);
                data.Append(str);
                count = readStream.Read(read, 0, stride);
            }

            return data.ToString();
        }

        public string MakeRequest (string uri, string action,
                      IDictionary parameters)
        {
            string url = m_baseUrl + "/" + uri + "?";
            string canonicalString = "";
            string queryString = "";
            SortedList finalParameters = new SortedList(parameters);

            //string date = System.DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture );
            string date = System.DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz");

            finalParameters.Add("SignatureVersion", "1");
            finalParameters.Add("Timestamp", date);
            finalParameters.Add("AWSAccessKeyId", m_awsAccessKey);
            finalParameters.Add("Version", "2007-02-09");
            finalParameters.Add("Action", action);

            bool first = true;

            foreach (string key in finalParameters.Keys)
            {
                string value = (string)finalParameters[key];
                canonicalString += key + value;
                if (first)
                {
                    first = false;
                }
                else
                {
                    queryString += "&";
                }
                queryString += HttpUtility.UrlEncode(key) + "=" +
                  HttpUtility.UrlEncode(value);
            }

            queryString += "&Signature=" + GetSignature(canonicalString);

            url += queryString;

            //Console.Write("{0}",url);
            //Console.WriteLine();
            //Console.Write("{0}",canonicalString);
            //Console.WriteLine();

            WebRequest request = WebRequest.Create(url);

            if (request is HttpWebRequest)
            {
                HttpWebRequest httpReq = request as HttpWebRequest;
                httpReq.AllowWriteStreamBuffering = false;
            }
            request.Method = "GET";

            string responseString = "";
            try
            {
                WebResponse response = request.GetResponse();
                System.IO.Stream responseStream = response.GetResponseStream();

                responseString = slurpInputStream(responseStream);
            }
            catch (System.Net.WebException ex)
            {
                WebResponse response = ex.Response;
                if (response != null)
                {
                    System.IO.Stream responseStream = response.GetResponseStream();

                    responseString = slurpInputStream(responseStream);
                }
                else
                {
                    responseString = ex.ToString();
                }
            }

            //System.Console.WriteLine(responseString);
            return responseString;
        }

        public string MakeRequest (string uri, string action)
        {
            return this.MakeRequest(uri, action, new Hashtable());
        }
    }
}
