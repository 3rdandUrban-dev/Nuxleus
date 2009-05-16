// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Text;
using System.Collections.Specialized;
using System.Collections;

namespace Nuxleus.Web.HttpHandler {

    public class NuxleusHttpAsyncTrackerReaderHandler : IHttpAsyncHandler
    {

        HttpResponse m_response;
        FileStream m_fileStream;
        byte[] m_xmlFragmentByteArray;
        static object m_lock = new object();

        public void ProcessRequest ( HttpContext context ) {
            //not called
        }

        public bool IsReusable {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest ( HttpContext context, AsyncCallback cb, object extraData ) {

            HttpRequest request = context.Request;
            m_response = context.Response;

            lock (m_lock) {
                m_fileStream = new FileStream(request.MapPath("~/App_Data/TrackerLog.xml"), FileMode.Open, FileAccess.Read, FileShare.Read, 1024, true);
                m_xmlFragmentByteArray = new byte[m_fileStream.Length];
                return m_fileStream.BeginRead(m_xmlFragmentByteArray, 0, (int)m_fileStream.Length, cb, extraData);
            }
        }

        public void EndProcessRequest ( IAsyncResult result ) {
            int readComplete = m_fileStream.EndRead(result);
            Encoding encoding = Encoding.UTF8;

            m_response.Output.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
            m_response.Output.WriteLine("<?xml-stylesheet type='text/xsl' href='/transform/base.xsl'?>");
            m_response.Output.WriteLine("<summary>");
            using (m_fileStream) {
                m_response.Output.Write(encoding.GetString(m_xmlFragmentByteArray));
            }
            m_response.Output.WriteLine("</summary>");

            m_response.ContentEncoding = encoding;
            m_response.ContentType = "text/xml";
        }
    }
}
