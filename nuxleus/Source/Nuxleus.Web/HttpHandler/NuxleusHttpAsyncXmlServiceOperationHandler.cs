// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.

using System;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Configuration;
using System.Threading;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.SessionState;
using System.Collections;
using Memcached.ClientLibrary;
using System.Text;
using Saxon.Api;
using System.Xml;
using Mvp.Xml;
using Nuxleus.Configuration;
using Nuxleus.Transform;
using System.Collections.Generic;
using Nuxleus.Memcached;
using Nuxleus.Cryptography;
using Nuxleus.Bucker;
using Nuxleus.Async;
using System.Globalization;
using Nuxleus.Agent;

namespace Nuxleus.Web.HttpHandler {

    public struct NuxleusHttpAsyncXmlServiceOperationHandler : IHttpAsyncHandler {

        XmlServiceOperationManager m_xmlServiceOperationManager;
        XsltTransformationManager m_xslTransformationManager;
        Transform.Context m_transformContext;
        Transform.Transform m_transform;
        Hashtable m_xsltParams;
        Hashtable m_namedXsltHashtable;
        TransformServiceAsyncResult m_transformAsyncResult;
        MemcachedClient m_memcachedClient;
        QueueClient m_queueClient;
        TextWriter m_writer;
        bool m_returnOutput;
        StringBuilder m_builder;
        HttpContext m_httpContext;
        String m_requestHashcode;
        Dictionary<int, int> m_xmlSourceETagDictionary;
        Dictionary<int, XmlReader> m_xmlReaderDictionary;
        Nuxleus.Agent.NuxleusAsyncResult m_nuxleusAsyncResult;
        AsyncCallback m_callback;
        String m_httpMethod;
        Exception m_exception;
        Context m_context;
        String m_hashkey;
        bool m_CONTENT_IS_MEMCACHED;
        bool m_USE_MEMCACHED;
        static HashAlgorithm m_hashAlgorithm = HashAlgorithm.SHA1;
        XmlWriter m_debugXmlWriter;
        string m_lastModifiedKey;
        string m_lastModifiedDate;
        UTF8Encoding m_encoding;
        Stopwatch m_stopwatch;
        TransformRequest m_request;
        TransformResponse m_response;
        NuxleusAgentAsyncRequest m_transformRequest;
        Transform.Agent m_agent;
        IAsyncResult m_asyncResult;

        public void ProcessRequest (HttpContext context) {
            //not called
        }

        public bool IsReusable {
            get { return false; }
        }

        #region IHttpAsyncHandler Members

        public IAsyncResult BeginProcessRequest (HttpContext context, AsyncCallback cb, object extraData) {
            FileInfo fileInfo = new FileInfo(context.Request.MapPath(context.Request.CurrentExecutionFilePath));
            //Console.WriteLine("File Date: {0}; File Length: {1}", fileInfo.LastWriteTimeUtc, fileInfo.Length);
            m_CONTENT_IS_MEMCACHED = false;
            m_USE_MEMCACHED = false;
            m_httpContext = context;
            m_returnOutput = true;
            m_httpMethod = m_httpContext.Request.HttpMethod;
            m_memcachedClient = (Client)context.Application["memcached"];
            m_encoding = (UTF8Encoding)context.Application["encoding"];
            m_queueClient = (QueueClient)context.Application["queueclient"];
            m_hashkey = (string)context.Application["hashkey"];
            m_xmlServiceOperationManager = (XmlServiceOperationManager)context.Application["xmlServiceOperationManager"];
            m_xslTransformationManager = (XsltTransformationManager)context.Application["xslTransformationManager"];
            m_transform = m_xslTransformationManager.Transform;
            m_xsltParams = (Hashtable)context.Application["globalXsltParams"];
            m_transformContext = new Transform.Context(context, m_hashAlgorithm, (string)context.Application["hashkey"], fileInfo, (Hashtable)m_xsltParams.Clone(), fileInfo.LastWriteTimeUtc, fileInfo.Length);
            m_namedXsltHashtable = (Hashtable)context.Application["namedXsltHashtable"];
            m_xmlSourceETagDictionary = m_xmlServiceOperationManager.XmlSourceETagDictionary;
            m_xmlReaderDictionary = m_xmlServiceOperationManager.XmlReaderDictionary;
            m_context = new Context(context, m_hashAlgorithm, m_hashkey, fileInfo, fileInfo.LastWriteTimeUtc, fileInfo.Length);
            //Console.WriteLine("File Date: {0}; File Length: {1}", m_context.RequestXmlFileInfo.LastWriteTimeUtc, m_context.RequestXmlFileInfo.Length);
            m_nuxleusAsyncResult = new Nuxleus.Agent.NuxleusAsyncResult(cb, extraData);
            m_callback = cb;
            m_nuxleusAsyncResult.m_context = context;
            m_builder = new StringBuilder();
            m_CONTENT_IS_MEMCACHED = false;
            m_USE_MEMCACHED = (bool)context.Application["usememcached"];
            Uri requestUri = new Uri(m_context.RequestUri);
            m_requestHashcode = m_context.GetRequestHashcode(true).ToString();
            m_lastModifiedKey = String.Format("LastModified:{0}", m_context.RequestUri.GetHashCode());
            m_lastModifiedDate = String.Empty;
            m_stopwatch = new Stopwatch();
            m_request = new TransformRequest();
            m_response = new TransformResponse();
            Guid requestGuid = Guid.NewGuid();
            m_request.ID = requestGuid;

            m_stopwatch.Start();
            bool hasXmlSourceChanged = m_xmlServiceOperationManager.HasXmlSourceChanged(m_context.RequestXmlETag, requestUri);

            if (m_USE_MEMCACHED) {

                string obj = (string)m_memcachedClient.Get(m_context.GetRequestHashcode(true).ToString());

                if (obj != null && !hasXmlSourceChanged) {
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
                            Console.WriteLine("If-None-Match: {0}, RequestHashCode: {1}", context.Request.Headers["If-None-Match"], m_requestHashcode);
                            if (context.Request.Headers["If-None-Match"] == m_requestHashcode) {
                                Console.WriteLine("They matched.");
                                Console.WriteLine("Use memcached: {0}, KeyExists: {1}, XmlSource Changed: {2}", m_USE_MEMCACHED, m_memcachedClient.KeyExists(m_lastModifiedKey), hasXmlSourceChanged);
                                Console.WriteLine("Last Modified Key Value: {0}", m_lastModifiedKey);
                                if (m_USE_MEMCACHED && m_memcachedClient.KeyExists(m_lastModifiedKey) && !hasXmlSourceChanged) {
                                    m_lastModifiedDate = (string)m_memcachedClient.Get(m_lastModifiedKey);
                                    Console.WriteLine("Last Modified Date: {0}", m_lastModifiedDate);
                                    if (context.Request.Headers["If-Modified-Since"] == m_lastModifiedDate) {
                                        context.Response.StatusCode = 304;
                                        m_returnOutput = false;
                                        goto CompleteCall;
                                    } else {
                                        goto Process;
                                    }
                                }

                            } else if (m_CONTENT_IS_MEMCACHED) {
                                goto CompleteCall;
                            } else {
                                goto Process;
                            }
                            break;
                        }
                    case "PUT": {
                            goto CompleteCall;
                        }
                    case "POST": {
                            goto CompleteCall;
                        }
                    case "DELETE": {
                            goto CompleteCall;
                        }
                    default: {
                            goto CompleteCall;
                        }
                }

            } catch (Exception ex) {
                m_exception = ex;
                WriteError();
                goto CompleteCall;
            }
Process:
            try {
                string xmlStylesheetHref = String.Empty;
                bool processWithEmbeddedPIStylsheet = false;
                XmlReader reader = m_xmlServiceOperationManager.GetXmlReader(m_context.RequestXmlETag, requestUri);
                do {
                    switch (reader.NodeType) {
                        case XmlNodeType.ProcessingInstruction:
                            switch (reader.Name) {
                                case "xml-stylesheet":
                                    string piValue = reader.Value;
                                    if (piValue.Contains("type=\"text/xsl\"") && piValue.Contains("href=")) {
                                        processWithEmbeddedPIStylsheet = true;
                                        xmlStylesheetHref = SubstringBefore(SubstringAfter(piValue, "href=\""), "\"");
                                    }
                                    Console.WriteLine("Stylesheet Href = {0}", xmlStylesheetHref);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.Element:
                            if (reader.IsStartElement()) {
                                switch (reader.Name) {
                                    case "service:operation":
                                    case "my:page":
                                        Uri baseXsltUri = new Uri(context.Request.MapPath(xmlStylesheetHref));
                                        string baseXslt = baseXsltUri.GetHashCode().ToString();

                                        if (!m_xslTransformationManager.NamedXsltHashtable.ContainsKey(baseXslt)) {
                                            m_xslTransformationManager.AddTransformer(baseXslt, baseXsltUri);
                                            m_xslTransformationManager.NamedXsltHashtable.Add(baseXslt, baseXsltUri);
                                        }
                                        if (!m_xslTransformationManager.XmlSourceHashtable.ContainsKey(m_context.RequestXmlETag)) {
                                            using (MemoryStream stream = new MemoryStream(m_encoding.GetBytes(reader.ReadOuterXml().ToCharArray()))) {
                                                m_xslTransformationManager.AddXmlSource(m_context.RequestXmlETag.ToString(), (Stream)stream);
                                            }
                                        }

                                        m_agent = new Transform.Agent();
                                        TransformContext transformContext = new TransformContext();
                                        transformContext.Context = m_transformContext;
                                        transformContext.HttpContext = context;
                                        transformContext.XsltTransformationManager = m_xslTransformationManager;
                                        transformContext.XsltName = baseXslt;
                                        transformContext.Writer = m_writer;
                                        transformContext.Response = m_response;
                                        m_request.TransformContext = transformContext;
                                        m_transform.BeginTransformProcess(m_request, cb, m_nuxleusAsyncResult);
                                        //m_transformRequest = m_agent.BeginRequest;
                                        //AsyncCallback callback = new AsyncCallback(RequestIsComplete);
                                        //m_asyncResult = m_transformRequest.BeginInvoke(m_request, new AsyncCallback(RequestIsComplete), new Nuxleus.Agent.NuxleusAsyncResult(callback, m_transformRequest), m_transformRequest, RequestIsComplete, m_transformRequest);
                                        //Console.WriteLine("BeginInvoke called for method: {0}", m_transformRequest.Method.Name);
                                        //m_writer.WriteLine(reader.ReadOuterXml());


                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case XmlNodeType.EndElement:

                            continue;
                        case XmlNodeType.Text:
                        case XmlNodeType.SignificantWhitespace:
                        case XmlNodeType.Whitespace:

                            break;
                        case XmlNodeType.CDATA:

                            break;
                        case XmlNodeType.Comment:
                            break;
                        case XmlNodeType.DocumentType:
                            break;
                        case XmlNodeType.EntityReference:
                            reader.ResolveEntity();
                            continue;

                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.EndEntity:
                            continue;
                        default:
                            break;
                        //throw new InvalidOperationException();

                    }
                } while (reader.Read());
                goto CompleteCall;
            } catch (Exception e) {
                m_exception = e;
                WriteError();
                goto CompleteCall;
            }

CompleteCall:
            Console.WriteLine("CompleteCall reached");
            if (m_lastModifiedDate == String.Empty) {
                m_lastModifiedDate = DateTime.UtcNow.ToString("r");
            }
            context.Response.AppendHeader("Cache-Control", "max-age=3600");
            context.Response.AddHeader("Last-Modified", m_lastModifiedDate);
            context.Response.AddHeader("ETag", m_requestHashcode);
            m_nuxleusAsyncResult.CompleteCall();
            return m_nuxleusAsyncResult;
        }

        private void RequestIsComplete (IAsyncResult result) {
            NuxleusAgentAsyncRequest response = (NuxleusAgentAsyncRequest)result.AsyncState;
            response.EndInvoke(result);
            Console.WriteLine("End of Invoke Reached");
            Console.WriteLine("result of transform: {0}", ((TransformResponse)m_agent.GetResponse(m_request.ID)).TransformResult);
        }

        public void EndProcessRequest (IAsyncResult result) {
            using (m_writer) {
                string output = m_builder.ToString();
                if (m_returnOutput) {
                    using (TextWriter writer = m_httpContext.Response.Output) {
                        writer.Write(output);
                    }
                }
                if (!m_CONTENT_IS_MEMCACHED && m_USE_MEMCACHED) {
                    Console.WriteLine("Adding Last Modified Key: {0}", m_lastModifiedKey);
                    m_memcachedClient.Set(m_context.GetRequestHashcode(true).ToString(), output, DateTime.Now.AddHours(4));
                    m_memcachedClient.Set(m_lastModifiedKey, m_lastModifiedDate);
                }
            }
            m_stopwatch.Stop();
            Console.WriteLine("Total processing time:\t\t{0} ms", m_stopwatch.ElapsedMilliseconds);
        }

        private void WriteError () {
            m_httpContext.Response.Output.WriteLine("<html>");
            m_httpContext.Response.Output.WriteLine("<head>");
            m_httpContext.Response.Output.WriteLine("<title>Xameleon Transformation Error</title>");
            m_httpContext.Response.Output.WriteLine("</head>");
            m_httpContext.Response.Output.WriteLine("<body>");
            m_httpContext.Response.Output.WriteLine("<h3>Error Message</h3>");
            m_httpContext.Response.Output.WriteLine("<p>" + m_exception.Message + "</p>");
            m_httpContext.Response.Output.WriteLine("<h3>Error Source</h3>");
            m_httpContext.Response.Output.WriteLine("<p>" + m_exception.Source + "</p>");
            m_httpContext.Response.Output.WriteLine("<h3>Error StackTrace</h3>");
            m_httpContext.Response.Output.WriteLine("<p>" + m_exception.StackTrace + "</p>");
            m_httpContext.Response.Output.WriteLine("</body>");
            m_httpContext.Response.Output.WriteLine("</html>");
        }

        /// <summary>
        /// Modified from Oleg Tkachenko's SubstringBefore and SubstringAfter extension functions
        /// @ http://www.tkachenko.com/blog/archives/000684.html
        /// This will be moved into an appropriate class once I have the time.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SubstringAfter (string source, string value) {
            if (string.IsNullOrEmpty(value)) {
                return source;
            }
            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            int index = compareInfo.IndexOf(source, value, CompareOptions.Ordinal);
            if (index < 0) {
                //No such substring
                return string.Empty;
            }
            return source.Substring(index + value.Length);
        }

        public static string SubstringBefore (string source, string value) {
            if (string.IsNullOrEmpty(value)) {
                return value;
            }
            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            int index = compareInfo.IndexOf(source, value, CompareOptions.Ordinal);
            if (index < 0) {
                //No such substring
                return string.Empty;
            }
            return source.Substring(0, index);
        }


        #endregion
    }
}