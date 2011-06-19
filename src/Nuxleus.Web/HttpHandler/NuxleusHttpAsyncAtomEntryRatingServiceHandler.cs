// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Nuxleus.Core;
using Nuxleus.Atom;

namespace Nuxleus.Web.HttpHandler {

    public class NuxleusHttpAsyncAtomEntryRatingServiceHandler : IHttpAsyncHandler
    {

        HttpResponse m_response;
        static object m_lock = new object();
        static XmlSerializer m_xSerializer = new XmlSerializer(typeof(Entry));
        static Encoding m_encoding = new UTF8Encoding();
        static XNamespace h = "http://www.w3.org/1999/xhtml";
        static XslCompiledTransform cleanseHtml = new XslCompiledTransform();

        public void ProcessRequest(HttpContext context) {
            //not called
        }

        public bool IsReusable {
            get { return false; }
        }

        static NuxleusHttpAsyncAtomEntryRatingServiceHandler() {

        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {

            NuxleusAsyncResult result = new NuxleusAsyncResult(cb, extraData);

            HttpRequest request = context.Request;
            m_response = context.Response;
            m_response.ContentType = "text";

            NameValueCollection form = request.Params;

            string id = form.Get("id");
            string vote = form.Get("vote");
            string ip = context.Request.UserHostAddress.ToString();

            try {
                for (int i = 0; i <= form.Count; i++) {
                    this.LogInfo("Form value: {0}", form[i]);
                }
            } catch (Exception e) {
                this.LogInfo(e.Message);
            }

            using (MemoryStream stream = new MemoryStream()) {
                using (StreamWriter sWriter = new StreamWriter(m_response.OutputStream, Encoding.UTF8)) {
                    sWriter.Write("ID: {0} Vote: {1} IP: {2}", id, vote, ip);
                }
            }

            result.CompleteCall();
            return result;
        }

        public void EndProcessRequest(IAsyncResult result) {
            m_response.OutputStream.Close();
        }
    }
}
