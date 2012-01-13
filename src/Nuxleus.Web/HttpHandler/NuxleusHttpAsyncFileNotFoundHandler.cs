// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Web;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Nuxleus.Bucker;
using Nuxleus.Core;
using Nuxleus.Cryptography;
using Nuxleus.Memcached;
using Nuxleus.Transform;

namespace Nuxleus.Web.HttpHandler {

    public class NuxleusHttpAsyncFileNotFoundHandler : IHttpAsyncHandler
    {

        XsltTransformationManager m_xslTransformationManager;
        MemcachedClient m_memcachedClient;
        QueueClient m_queueClient;
        Transform.Transform m_transform;
        TextWriter m_writer;
        StringBuilder m_builder;
        HttpContext m_context;
        Hashtable m_xsltParams;
        Hashtable m_namedXsltHashtable;
        NuxleusAsyncResult m_transformAsyncResult;
        AsyncCallback m_callback;
        String m_httpMethod;
        Exception m_exception;
        Transform.Context m_transformContext;
        bool m_CONTENT_IS_MEMCACHED;
        bool m_USE_MEMCACHED;
        static HashAlgorithm m_hashAlgorithm = HashAlgorithm.SHA1;

        public void ProcessRequest (HttpContext context) {
            //not called
        }

        public bool IsReusable {
            get { return false; }
        }

        #region IHttpAsyncHandler Members

        public IAsyncResult BeginProcessRequest (HttpContext context, AsyncCallback cb, object extraData) {
            FileInfo fileInfo = new FileInfo(context.Request.MapPath(context.Request.CurrentExecutionFilePath));

            m_context = context;
            m_httpMethod = m_context.Request.HttpMethod;
            m_memcachedClient = (Client)context.Application["memcached"];
            m_queueClient = (QueueClient)context.Application["queueclient"];
            m_xslTransformationManager = (XsltTransformationManager)context.Application["xslTransformationManager"];
            m_transform = m_xslTransformationManager.Transform;
            m_xsltParams = (Hashtable)context.Application["globalXsltParams"];
            m_namedXsltHashtable = (Hashtable)context.Application["namedXsltHashtable"];
            m_transformContext = new Transform.Context(context, m_hashAlgorithm, (string)context.Application["hashkey"], fileInfo, (Hashtable)m_xsltParams.Clone(), fileInfo.LastWriteTimeUtc, fileInfo.Length);
            m_transformAsyncResult = new NuxleusAsyncResult(cb, extraData);
            m_callback = cb;
            m_transformAsyncResult.m_context = context;
            m_builder = new StringBuilder();
            m_CONTENT_IS_MEMCACHED = false;
            m_USE_MEMCACHED = (bool)context.Application["usememcached"];

            bool hasXmlSourceChanged = m_xslTransformationManager.HasXmlSourceChanged(m_transformContext.RequestXmlETag);
            bool hasBaseXsltSourceChanged = m_xslTransformationManager.HasBaseXsltSourceChanged();

            if (m_USE_MEMCACHED) {

                string obj = (string)m_memcachedClient.Get(m_transformContext.GetRequestHashcode(true));

                if (obj != null && !(hasXmlSourceChanged || hasBaseXsltSourceChanged) && !(m_context.Request.CurrentExecutionFilePath.StartsWith("/service/session"))) {
                    m_builder.Append(obj);
                    m_CONTENT_IS_MEMCACHED = true;
                    if ((bool)context.Application["debug"])
                        context.Response.ContentType = "text";
                    else
                        context.Response.ContentType = "text/xml";
                } else {
                    m_writer = new StringWriter(m_builder);
                    m_CONTENT_IS_MEMCACHED = false;
                }
            } else {
                m_writer = new StringWriter(m_builder);
            }

            try {

                switch (m_httpMethod) {
                    case "GET":
                    case "HEAD": {
                            if (m_CONTENT_IS_MEMCACHED) {
                                m_transformAsyncResult.CompleteCall();
                                return m_transformAsyncResult;
                            } else {
                                try {
                                    string file = m_context.Request.FilePath;
                                    string baseXslt;

                                    if (file.EndsWith("index.page")) {
                                        baseXslt = "precompile-atomictalk";
                                    } else if (file.EndsWith("service.op"))
                                        baseXslt = "base";
                                    else
                                        baseXslt = m_xslTransformationManager.BaseXsltName;

                                    //m_transform.BeginProcess(m_transformContext, context, m_xslTransformationManager, m_writer, baseXslt, m_transformAsyncResult);
                                    return m_transformAsyncResult;
                                } catch (Exception e) {
                                    m_exception = e;
                                    WriteError();
                                    m_transformAsyncResult.CompleteCall();
                                    return m_transformAsyncResult;
                                }
                            }
                        }
                    case "PUT": {
                            return m_transformAsyncResult;
                        }
                    case "POST": {
                            return m_transformAsyncResult;
                        }
                    case "DELETE": {
                            return m_transformAsyncResult;
                        }
                    default: {
                            return m_transformAsyncResult;
                        }
                }

            } catch (Exception ex) {
                m_exception = ex;
                WriteError();
                m_transformAsyncResult.CompleteCall();
                return m_transformAsyncResult;
            }
        }

        public void EndProcessRequest (IAsyncResult result) {
            using (m_writer) {
                string output = m_builder.ToString();
                using (TextWriter writer = m_context.Response.Output) {
                    writer.Write(output);
                }
                m_transformContext.Clear();
                if (!m_CONTENT_IS_MEMCACHED && m_USE_MEMCACHED)
                    m_memcachedClient.Store(StoreMode.Set, m_transformContext.GetRequestHashcode(true), output, DateTime.Now.AddHours(1));
                if ((bool)m_context.Application["debug"])
                    m_context.Response.Write((string)m_context.Application["debugOutput"]);
            }
        }

        private void WriteError () {
            m_context.Response.Output.WriteLine("<html>");
            m_context.Response.Output.WriteLine("<head>");
            m_context.Response.Output.WriteLine("<title>Xameleon Transformation Error</title>");
            m_context.Response.Output.WriteLine("</head>");
            m_context.Response.Output.WriteLine("<body>");
            m_context.Response.Output.WriteLine("<h3>Error Message</h3>");
            m_context.Response.Output.WriteLine("<p>" + m_exception.Message + "</p>");
            m_context.Response.Output.WriteLine("<h3>Error Source</h3>");
            m_context.Response.Output.WriteLine("<p>" + m_exception.Source + "</p>");
            m_context.Response.Output.WriteLine("<h3>Error StackTrace</h3>");
            m_context.Response.Output.WriteLine("<p>" + m_exception.StackTrace + "</p>");
            m_context.Response.Output.WriteLine("</body>");
            m_context.Response.Output.WriteLine("</html>");
        }

        #endregion
    }
}