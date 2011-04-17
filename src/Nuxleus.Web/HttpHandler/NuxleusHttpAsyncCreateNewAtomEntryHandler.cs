// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Nuxleus.Atom;
using Nuxleus.Core;

namespace Nuxleus.Web.HttpHandler {

    public class NuxleusHttpAsyncCreateNewAtomEntryHandler : IHttpAsyncHandler
    {

        FileStream m_file;
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

        static NuxleusHttpAsyncCreateNewAtomEntryHandler() {
            
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {

            HttpRequest request = context.Request;
            m_response = context.Response;
            m_response.ContentType = "text/xml";

            NameValueCollection form = request.Form;

            string name = form.Get("name");
            string slug = form.Get("slug");
            string base_uri = form.Get("base_uri");
            string base_path = form.Get("base_path");
            string title = form.Get("title");
            string ip = context.Request.UserHostAddress.ToString();

            Entry entry = new Entry { Published = DateTime.Now, Updated = DateTime.Now, Title = title };

            using (MemoryStream stream = new MemoryStream()) {

                m_xSerializer.Serialize(stream, (Entry)entry);
                stream.Seek(0, 0);

                byte[] output = stream.GetBuffer();
                char[] chars = m_encoding.GetChars(output, 0, output.Length);

                using (XmlReader reader = XmlReader.Create(stream)) {
                    using (StreamWriter sWriter = new StreamWriter(m_response.OutputStream, Encoding.UTF8)) {
                        while (reader.Read()) {
                            sWriter.Write(reader.ReadOuterXml());
                        }
                    }
                }

                lock (m_lock) {
                    string basePath = request.MapPath(String.Format("~{0}", base_path));
                    this.LogInfo(basePath);
                    if (!Directory.Exists(basePath)) {
                        Directory.CreateDirectory(basePath);
                    }
                    m_file = new FileStream(String.Format("{0}/{1}.xml", basePath, Guid.NewGuid()), FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 1024, true);
                    m_file.Seek(0, 0);
                    return m_file.BeginWrite(output, 0, output.Length, cb, extraData);
                }
            }
        }

        public void EndProcessRequest(IAsyncResult result) {
            m_file.Close();
            m_response.OutputStream.Close();
        }
    }
}
