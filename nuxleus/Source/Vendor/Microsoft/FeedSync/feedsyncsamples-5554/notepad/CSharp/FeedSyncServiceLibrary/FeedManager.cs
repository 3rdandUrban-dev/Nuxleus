/*****************************************************************************************
   
   Copyright (c) Microsoft Corporation. All rights reserved.

   Use of this code sample is subject to the terms of the Microsoft
   Permissive License, a copy of which should always be distributed with
   this file.  You can also access a copy of this license agreement at:
   http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx

 ****************************************************************************************/

using System;
using System.Net;
using System.IO;

namespace Microsoft.Samples.FeedSyncService
{

    public class FeedManager
    {
        //  Set default timeout for web/http calls to 2 minutes
        static private int s_TimeoutInMilliseconds = 120000;
        static private HttpStatusCode s_LastStatusCode;

        static public HttpStatusCode LastStatusCode { get { return FeedManager.s_LastStatusCode; } }

        static public bool IsRSSFeedURL(string i_FeedURL)
        {
            return (i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.RSS_QUERY_FRAGMENT) != -1);
        }

        static public bool IsAtomFeedURL(string i_FeedURL)
        {
            bool IsAtomFeedURL =
                (i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.ATOM_QUERY_FRAGMENT) != -1) ||
                (i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.RSS_QUERY_FRAGMENT) == -1);

            return IsAtomFeedURL;
        }

        static public string GetRSSFeedURL(string i_FeedURL)
        {
            string RSSFeedURL = i_FeedURL;

            //  Strip off Atom fragment if supplied
            if (i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.ATOM_QUERY_FRAGMENT) != -1)
                RSSFeedURL = i_FeedURL.Substring(0, i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.ATOM_QUERY_FRAGMENT));

            //  Add RSS fragment if necessary
            if (RSSFeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.RSS_QUERY_FRAGMENT) == -1)
                RSSFeedURL += Microsoft.Samples.FeedSyncService.Constants.RSS_QUERY_FRAGMENT;

            return RSSFeedURL;
        }

        static public string GetAtomFeedURL(string i_FeedURL)
        {
            string AtomFeedURL = i_FeedURL;

            //  Strip off RSS fragment if supplied
            if (i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.RSS_QUERY_FRAGMENT) != -1)
                AtomFeedURL = i_FeedURL.Substring(0, i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.RSS_QUERY_FRAGMENT));

            //  Add Atom fragment if necessary
            if (AtomFeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.ATOM_QUERY_FRAGMENT) == -1)
                AtomFeedURL += Microsoft.Samples.FeedSyncService.Constants.ATOM_QUERY_FRAGMENT;

            return AtomFeedURL;
        }

        static public int TimeoutInMilliseconds
        {
            get
            {
                return s_TimeoutInMilliseconds;
            }
            set
            {
                s_TimeoutInMilliseconds = value;
            }
        }

        #region Generic read/write methods

        static public string ReadFeedContents(string i_FeedURL, ref string i_Since)
        {
            string FeedContents = System.String.Empty;

            System.Net.HttpWebRequest HttpWebRequest = 
                CreateWebRequest
                    (
                    i_FeedURL,
                    "GET"
                    );

            HttpWebRequest.KeepAlive = false;
            HttpWebRequest.Timeout = s_TimeoutInMilliseconds;

            if (!System.String.IsNullOrEmpty(i_Since))
            {
                HttpWebRequest.Headers.Add("If-None-Match", i_Since);
                HttpWebRequest.Headers.Add("A-IM", "feed");
            }

            System.Net.HttpWebResponse HttpWebResponse = null;

            try
            {
                HttpWebResponse = (System.Net.HttpWebResponse)HttpWebRequest.GetResponse();

                using (System.IO.Stream ResponseStream = HttpWebResponse.GetResponseStream())
                {
                    System.IO.StreamReader StreamReader = new System.IO.StreamReader
                        (
                        ResponseStream,
                        System.Text.Encoding.UTF8
                        );

                    FeedContents = StreamReader.ReadToEnd();
                    StreamReader.Close();
                }

                i_Since = HttpWebResponse.Headers["ETag"];
            }
            catch (System.Net.WebException WebException)
            {
                //  In the event that there was something returned, explicitly
                //  empty feed contents
                FeedContents = null;

                if (WebException.Response != null)
                {
                    System.Net.HttpWebResponse HttpWebExceptionResponse = (System.Net.HttpWebResponse)WebException.Response;

                    if (HttpWebExceptionResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new System.Exception("Private feeds not supported!");
                    }
                    else if (HttpWebExceptionResponse.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        throw new System.Exception("Not authorized to access feed!");
                    }

                    if (HttpWebExceptionResponse.StatusCode != System.Net.HttpStatusCode.NotModified)
                        throw;
                }
                else
                {
                    throw;
                }
            }

            finally
            {
                if (HttpWebResponse != null)
                    HttpWebResponse.Close();
            }
            return FeedContents;
        }

        static public void UpdateFeedContents(string i_FeedContents, string i_CompleteFeedURL)
        {
            string contentType;

            if (IsAtomFeedURL(i_CompleteFeedURL))
            {
                contentType = "application/atom+xml;type=feed";
            }
            else if (IsRSSFeedURL(i_CompleteFeedURL))
            {
                contentType = "application/rss+xml";
            }
            else
            {
                throw new Exception("Invalid feed url");
            }

            try
            {
                System.Net.HttpWebResponse webResponse;

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                byte[] encodedData = encoding.GetBytes(i_FeedContents);

                CompleteRequest
                    (
                    "POST",
                    encodedData,
                    i_CompleteFeedURL, 
                    contentType, 
                    null,
                    out webResponse
                    );
            }
            catch (System.Net.WebException WebException)
            {
                if (WebException.Response != null)
                {
                    System.Net.HttpWebResponse HttpWebResponse = (System.Net.HttpWebResponse)WebException.Response;
                    
                    if (HttpWebResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new System.Exception("Private feeds are unsupported!");
                    }
                    else if (HttpWebResponse.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        throw new System.Exception("Not authorized to access feed!");
                    }
                }

                throw;
            }
        }


        #endregion

        #region Web request methods

        public static string CompleteRequest(string method, byte[] encodedData, String toFileUrl, string contentType, WebHeaderCollection headers, out HttpWebResponse webResponse)
        {
            HttpWebRequest wr = CreateWebRequest(toFileUrl, method);

            if (headers != null)
                wr.Headers.Add(headers);

            if (encodedData != null)
            {
                wr.ContentLength = encodedData.Length;
                wr.ContentType = contentType;

                using (Stream s = wr.GetRequestStream())
                {
                    s.Write(encodedData, 0, encodedData.Length);
                }
            }

            webResponse = null;
            string response = null;

            try
            {
                using (webResponse = (HttpWebResponse)wr.GetResponse())
                {
                    s_LastStatusCode = webResponse.StatusCode;
                    using (StreamReader sr = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                    {
                        response = sr.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (StreamReader sr = new StreamReader(ex.Response.GetResponseStream(), System.Text.Encoding.UTF8))
                    {
                        response = sr.ReadToEnd();
                        System.Diagnostics.Debug.Write(response);
                    }
                }

                throw;
            }

            return response;
        }

        // Create a webRequest to perform "method" (eg: Copy, Move, Delete etc.) on the specified file/folder.
        public static HttpWebRequest CreateWebRequest(String url, String method)
        {
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);

            wr.Credentials = CredentialCache.DefaultCredentials;
            wr.PreAuthenticate = true;
            wr.Method = method;

            wr.Timeout = 10000; // set timeout to 10 seconds.

            return (wr);
        }

        #endregion
    }
}
