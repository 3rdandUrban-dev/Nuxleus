using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Permissions;
using System.Diagnostics;
using Nuxleus.Drawing.Utility;
using System.Web;
using System.Drawing;
using System.Xml;
using Nuxleus.Utility.S3;
using System.Globalization;

namespace Nuxleus.Service.Drawing
{

    public struct NuxleusImageResizeProcessManager
    {
        string m_path;
        TextWriter m_logWriter;
        bool m_processInProgress;
        Queue m_processQueue;
        Dictionary<string, JobInfo> m_jobIdIndex;
        String m_s3bucket;
        Dictionary<string, string> m_statusQueue;
        Dictionary<string, string> m_lookupTable;

        [PermissionSet(SecurityAction.Demand, Name="FullTrust")]
        public NuxleusImageResizeProcessManager (string path, TextWriter logWriter)
        {
            m_path = path;
            m_s3bucket = "storage.iact.com";
            m_logWriter = logWriter;
            m_processInProgress = false;
            m_processQueue = new Queue ();
            m_jobIdIndex = new Dictionary<string, JobInfo> ();
            m_statusQueue = (Dictionary<string, string>)HttpContext.Current.Application ["as_statusQueue"];
            m_lookupTable = new Dictionary<string, string> ();
            m_lookupTable.Add ("100", "1");
            m_lookupTable.Add ("150", "2");
            m_lookupTable.Add ("200", "3");
            m_lookupTable.Add ("320", "4");
            m_lookupTable.Add ("500", "5");
        }

        public TextWriter LogWriter { get { return m_logWriter; } set { m_logWriter = value; } }

        public string Folder { get { return m_path; } set { m_path = value; } }

        public void AddFile (string filePath)
        {
            m_logWriter.WriteLine ("Adding {0}", filePath);
            JobInfo jobInfo = new JobInfo ();
            jobInfo.SourceUri = filePath;
            string uri = String.Empty;
            try {
                using (XmlReader reader = XmlReader.Create(filePath)) {
                    lock (reader) {
                        do {
                            if (reader.IsStartElement ()) {
                                switch (reader.Name) {
                                case "request":
                                    uri = reader.GetAttribute ("uri");
                                    jobInfo.ProcessUri = uri;
                                    break;
                                case "bucket":
                                    uri = reader.GetAttribute ("name");
                                    break;
                                case "job":
                                    jobInfo.JobID = reader.GetAttribute ("id");
                                    break;
                                default:
                                    break;
                                }
                            }
                        } while (reader.Read());

                        m_jobIdIndex.Add (uri, jobInfo);
                    }
                }
            } catch (Exception e) {
                m_logWriter.WriteLine (e.Message);
            }

            m_logWriter.WriteLine ("Processing URI {0}", uri);

            if (m_processQueue.Count == 0) {
                // If the m_processQueue is empty, process the job request.
                m_logWriter.WriteLine ("Queue is empty.  Processing URI {0} directly", uri);
                processFile (uri);

            } else if (m_processInProgress) {
                // If a file is being processed or the m_processQueue is currently being processed the m_processInProgress will be set 
                // to true.  As such, add the file to the queue for processing.
                m_logWriter.WriteLine ("Queue is currently being processed.  Adding {0} to queue.", uri);
                m_processQueue.Enqueue (uri);
            } else {
                // If it's not set to true, add it to the queue and then process that queue.
                m_logWriter.WriteLine ("Queue is not empty, but is not currently processing anything.  Starting processing of the queue.", uri);
                m_processQueue.Enqueue (uri);
                processFile (m_processQueue);
            }

        }

        private void processFile (Queue queue)
        {
            m_processInProgress = true;
            m_logWriter.WriteLine ("Processing queue with {0} current entries.", queue.Count);
            ProcessQueue (queue);
            m_processInProgress = false;
        }

        public void ProcessFile (string uri)
        {
            processFile (uri);
        }

        private void processFile (string uri)
        {

            m_logWriter.WriteLine ("Processing file: {0}", uri);

            Uri fileUri = new Uri (uri);

            m_processInProgress = true;

            try {
                HttpWebRequest fileRequest = (HttpWebRequest)WebRequest.Create (fileUri);
                fileRequest.Timeout = 30000;
                HttpWebResponse fileResponse = (HttpWebResponse)fileRequest.GetResponse ();
                m_logWriter.WriteLine ("Response Code: {0}", fileResponse.StatusCode);
                m_logWriter.WriteLine ("Content Length: {0}", fileResponse.ContentLength);
                m_logWriter.WriteLine ("Content Type: {0}", fileResponse.ContentType);
                Stream imageStream = (Stream)fileResponse.GetResponseStream ();
                Dictionary<string, MemoryStream > result = new ConvertImage.JpegImageResize (imageStream).InvokeProcess ();
                m_logWriter.WriteLine ("Total images in result dictionary: {0}", result.Count);

                int status = 0;

                try {
                    putToS3 (result, fileUri.AbsolutePath);
                } catch (Exception e) {
                    m_logWriter.WriteLine ("There was a problem processing the file.  Error message: {0}", e.Message);
                    status = 2;
                }

                JobInfo jobInfo = m_jobIdIndex [uri];

                String statusXml = String.Format ("<request time='{0}' uri='{1}'><job id='{2}' status='{3}'/></request>", DateTime.Now, uri, jobInfo.JobID, status);
                m_logWriter.WriteLine ("Putting value of {0} into status queue with value of: {1}", jobInfo.JobID, statusXml);
                m_statusQueue [jobInfo.JobID] = statusXml;

                m_jobIdIndex.Remove (uri);

            } catch (Exception e) {
                m_logWriter.WriteLine (e.Message);
            }

            m_processInProgress = false;
        }

        private void ProcessQueue (Queue queue)
        {
            do {
                processFile ((string)queue.Dequeue ());
            } while (queue.Count > 0);
        }

        private void putToS3 (Dictionary<string, MemoryStream> imageDictionary, string baseFileName)
        {
            m_logWriter.WriteLine ("Beginning Put Process of {0}", baseFileName);
            //long requestExpires = DateTime.UtcNow.AddMinutes(1).Subtract(new DateTime(1970, 1, 1)).Ticks;
            //QueryStringAuthGenerator authGenerator = new QueryStringAuthGenerator(false, "s3.amazonaws.com", 80, CallingFormat.REGULAR);
            //authGenerator.Expires = requestExpires;

            SortedList headerList = new SortedList ();
            headerList.Add ("Content-Type", "image/jpeg");
            headerList.Add ("X-Amz-acl", "public-read");

            AWSAuthConnection awsConnection = new AWSAuthConnection (Environment.GetEnvironmentVariable ("S3_ACCESS_KEY"), Environment.GetEnvironmentVariable ("S3_SECRET_KEY"), false, "s3.amazonaws.com", 80, CallingFormat.REGULAR);

            string baseFileKey = SubstringAfter (SubstringBefore (baseFileName.ToLower (), "."), "/");

            IEnumerator images = imageDictionary.GetEnumerator ();

            while (images.MoveNext()) {
                KeyValuePair<string, MemoryStream > keyValuePair = (KeyValuePair<string, MemoryStream>)images.Current;
                string fileName = String.Format ("{0}-{1}.jpg", baseFileKey, m_lookupTable [keyValuePair.Key]);
                S3Object s3object = new S3Object (keyValuePair.Value.GetBuffer (), new SortedList ());
                bool keepTrying = true;
                int trys = 0;
                int maxRetryAttempts = 10;
                do {
                    try {
                        Response webRequest = awsConnection.put (m_s3bucket, fileName, s3object, headerList);
                        Console.WriteLine ("PUT {0} to S3 with response: {1}", fileName, webRequest.getResponseMessage ());
                        keepTrying = false;
                    } catch (Exception e) {
                        if (trys == maxRetryAttempts) {
                            keepTrying = false;
                        } else {
                            trys++;
                        }
                    }
                } while (keepTrying);

                //XmlReader webResponseReader = null;
                //try {
                //    webRequest 
                //    //webRequest = AWSAuthConnection.MakeRequest("PUT", "storage.iact.com", fileName, headerList, s3object, signedKey, m_s3PublicKey, requestExpires/1000);
                //} catch (Exception e) {
                //    //webResponseReader = XmlReader.Create(webRequest.GetResponse().GetResponseStream());
                //    Console.WriteLine("Error response from S3: {0} with error: {1}", e.Message, webRequest.getResponseMessage());
                //}
                //webResponseReader = XmlReader.Create(webRequest.GetResponse().GetResponseStream());
            }
        }

        private string getS3Signature (string conanicalString)
        {
            Console.WriteLine (conanicalString);
            XmlReader reader = XmlReader.Create (String.Format ("http://iact.stagegold.com/iactservice.asmx/GetSignature?authorize={0}&content={1}", String.Empty, HttpUtility.UrlEncode (conanicalString)));

            if (reader.ReadToFollowing ("string", "http://tempuri.org/")) {
                return reader.ReadString ();
            } else {
                return "can not read from response";
            }
        }

        public static string SubstringAfter (string source, string value)
        {
            if (string.IsNullOrEmpty (value)) {
                return source;
            }
            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            int index = compareInfo.IndexOf (source, value, CompareOptions.Ordinal);
            if (index < 0) {
                //No such substring
                return string.Empty;
            }
            return source.Substring (index + value.Length);
        }

        public static string SubstringBefore (string source, string value)
        {
            if (string.IsNullOrEmpty (value)) {
                return value;
            }
            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            int index = compareInfo.IndexOf (source, value, CompareOptions.Ordinal);
            if (index < 0) {
                //No such substring
                return string.Empty;
            }
            return source.Substring (0, index);
        }
    }

    public struct JobInfo
    {

        string m_sourceUri;
        string m_jobId;
        string m_processUri;

        public JobInfo (string sourceUri, string jobId, string processUri)
        {
            m_sourceUri = sourceUri;
            m_jobId = jobId;
            m_processUri = processUri;
        }

        public string SourceUri { get { return m_sourceUri; } set { m_sourceUri = value; } }

        public string JobID { get { return m_jobId; } set { m_jobId = value; } }

        public string ProcessUri { get { return m_processUri; } set { m_processUri = value; } }

    }
}