using System;
using System.Globalization;
using System.IO;
using System.Web.Util;
using System.Web.Hosting;
using System.Web;
using System.Collections;
using System.Collections.Generic;

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpAysncStaticFileHandler : IHttpAsyncHandler
    {

        static bool ValidFileName(string fileName)
        {
            if (fileName == null || fileName.Length == 0)
                return false;

            return (!fileName.EndsWith(" ") && !fileName.EndsWith("."));
        }

        static MimeType m_mimeType = new MimeType { MimeTypeDictionary = new Dictionary<string, string>(), IsInitialized = false };
        Nuxleus.Core.NuxleusAsyncResult m_nuxleusAsyncResult;
        HttpResponse m_response;
        FileInfo m_fi;
        bool m_returnFile;

        public void ProcessRequest(HttpContext context)
        {
            // Never called
        }

        public bool IsReusable
        {
            get { return true; }
        }



        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            HttpRequest request = context.Request;
            m_response = context.Response;
            m_nuxleusAsyncResult = new Nuxleus.Core.NuxleusAsyncResult(cb, extraData);
            m_returnFile = false;

            string fileName = request.PhysicalPath;
            m_fi = new FileInfo(fileName);
            if (!m_fi.Exists || !ValidFileName(fileName))
                throw new HttpException(404, String.Format("Path {0} was not found.", request.FilePath));

            if ((m_fi.Attributes & FileAttributes.Directory) != 0)
            {
                m_response.Redirect(request.Path + '/');
                m_nuxleusAsyncResult.CompleteCall();
                return m_nuxleusAsyncResult;
            }

            string strHeader = request.Headers["If-Modified-Since"];
            try
            {
                if (strHeader != null)
                {
                    DateTime dtIfModifiedSince = DateTime.ParseExact(strHeader, "r", null);
                    DateTime ftime;

                    ftime = m_fi.LastWriteTime.ToUniversalTime();
                    if (ftime <= dtIfModifiedSince)
                    {
                        m_response.StatusCode = 304;
                        m_nuxleusAsyncResult.CompleteCall();
                        return m_nuxleusAsyncResult;
                    }
                }
            }
            catch { }

            try
            {
                
                if (!m_mimeType.IsInitialized) m_mimeType.Init();
                m_returnFile = true;
                m_nuxleusAsyncResult.CompleteCall();
                return m_nuxleusAsyncResult;
            }
            catch (Exception e)
            {
                throw new HttpException(403, e.Message);
            }
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            if (m_returnFile)
            {
                DateTime lastWT = m_fi.LastWriteTime.ToUniversalTime();
                m_response.AddHeader("Last-Modified", lastWT.ToString("r"));
                m_response.ContentType = m_mimeType.GetMimeType(m_fi.Extension);
                m_response.TransmitFile(m_fi.FullName);
            }
        }

    }

    public class MimeType
    {
        Dictionary<string, string> m_mimeType;
        public bool IsInitialized { get; set; }

        public Dictionary<string, string> MimeTypeDictionary { get { return m_mimeType; } set { m_mimeType = value; } }

        public void Init()
        {
            //TODO: Put these into a proper config file, and read from that file to initialize the Dictionary.
            m_mimeType.Add(".xml", "text/xml");
            m_mimeType.Add(".xsl", "text/xml");
            m_mimeType.Add(".xslt", "text/xml");
            m_mimeType.Add(".js", "application/x-javascript");
            m_mimeType.Add(".css", "text/css");
            IsInitialized = true;
        }

        public void AddMimeType(string extension, string mimeType)
        {
            m_mimeType.Add(extension, mimeType);
        }

        public string GetMimeType(string extension)
        {
            return m_mimeType[extension];
        }
    }
}