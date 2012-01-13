// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Nuxleus.Bucker;
using Nuxleus.Core;
using Nuxleus.Cryptography;
using Nuxleus.Memcached;
using Nuxleus.Transform;

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpAsyncHandler : IHttpAsyncHandler
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
        XmlWriter m_debugXmlWriter;

        public void ProcessRequest(HttpContext context)
        {
            //not called
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #region IHttpAsyncHandler Members

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            FileInfo fileInfo = new FileInfo(context.Request.MapPath(context.Request.CurrentExecutionFilePath));
            m_CONTENT_IS_MEMCACHED = false;
            m_USE_MEMCACHED = false;
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

            if (m_USE_MEMCACHED)
            {

                string obj = (string)m_memcachedClient.Get(m_transformContext.GetRequestHashcode(true));

                if (obj != null && !(hasXmlSourceChanged || hasBaseXsltSourceChanged) && !(m_context.Request.CurrentExecutionFilePath.StartsWith("/service/session")  && !(m_context.Request.CurrentExecutionFilePath.StartsWith("/service/geo"))))
                {
                    m_builder.Append(obj);
                    m_CONTENT_IS_MEMCACHED = true;
                    if ((bool)context.Application["debug"])
                        context.Response.ContentType = "text";
                    else
                        context.Response.ContentType = "text/xml";
                }
                else
                {
                    m_writer = new StringWriter(m_builder);
                    m_CONTENT_IS_MEMCACHED = false;
                }
            }
            else
            {
                m_writer = new StringWriter(m_builder);
            }

            //if ((bool)context.Application["debug"])
            //{
            //    context.Response.Write("<debug>");
            //    context.Response.Write("<file-info>");
            //    context.Response.Write("Has Xml Changed: " + hasXmlSourceChanged + ":" + m_transformContext.RequestXmlETag + "<br/>");
            //    context.Response.Write("Has Xslt Changed: " + hasBaseXsltSourceChanged + "<br/>");
            //    context.Response.Write("Xml ETag: " + m_transformContext.GetRequestHashcode(true) + "<br/>");
            //    context.Response.Write("XdmNode Count: " + m_xslTransformationManager.GetXdmNodeHashtableCount() + "<br/>");
            //    context.Response.Write("</file-info>");
            //    context.Application["debugOutput"] = (string)("<DebugOutput>" + WriteDebugOutput(m_transformContext, m_xslTransformationManager, new StringBuilder(), m_CONTENT_IS_MEMCACHED).ToString() + "</DebugOutput>");
            //    context.Response.Write("</debug>");
            //}

            try
            {

                switch (m_httpMethod)
                {
                    case "GET":
                    case "HEAD":
                        {
                            if (m_CONTENT_IS_MEMCACHED)
                            {
                                m_transformAsyncResult.CompleteCall();
                                return m_transformAsyncResult;
                            }
                            else
                            {
                                try
                                {
                                    string file = m_context.Request.FilePath;
                                    string baseXslt;

                                    if (file.EndsWith("index.page"))
                                    {
                                        baseXslt = "precompile-atomictalk";
                                    }
                                    else if (file.EndsWith("service.op"))
                                        baseXslt = "base";
                                    else
                                        baseXslt = m_xslTransformationManager.BaseXsltName;

                                    //m_transform.BeginTransformProcess(m_transformContext, context, m_xslTransformationManager, m_writer, baseXslt, m_transformAsyncResult);
                                    return m_transformAsyncResult;
                                }
                                catch (Exception e)
                                {
                                    m_exception = e;
                                    WriteError();
                                    m_transformAsyncResult.CompleteCall();
                                    return m_transformAsyncResult;
                                }
                            }
                        }
                    case "PUT":
                        {
                            
                            return m_transformAsyncResult;
                        }
                    case "POST":
                        {
                            return m_transformAsyncResult;
                        }
                    case "DELETE":
                        {
                            return m_transformAsyncResult;
                        }
                    default:
                        {
                            return m_transformAsyncResult;
                        }
                }

            }
            catch (Exception ex)
            {
                m_exception = ex;
                WriteError();
                m_transformAsyncResult.CompleteCall();
                return m_transformAsyncResult;
            }
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            using (m_writer)
            {
                string output = m_builder.ToString();
                using (TextWriter writer = m_context.Response.Output)
                {
                    writer.Write(output);
                }
                m_transformContext.Clear();
                if (!m_CONTENT_IS_MEMCACHED && m_USE_MEMCACHED)
                    m_memcachedClient.Store(StoreMode.Set, m_transformContext.GetRequestHashcode(true), output, DateTime.Now.AddHours(1));
                //if ((bool)m_context.Application["debug"])
                //    m_context.Response.Write((string)m_context.Application["debugOutput"]);
            }
        }

        private void WriteError ()
        {
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

        //protected StringBuilder WriteDebugOutput(Context context, XsltTransformationManager xsltTransformationManager, StringBuilder builder, bool CONTENT_IS_MEMCACHED)
        //{
        //    builder.Append(CreateNode("Request_File_ETag", context.ETag));
        //    builder.Append(CreateNode("CompilerBaseUri", xsltTransformationManager.Compiler.BaseUri));
        //    builder.Append(CreateNode("Compiler", xsltTransformationManager.Compiler.GetHashCode()));
        //    //foreach(System.Reflection.PropertyInfo t in HttpContext.Current.GetType().GetProperties()){
        //    // 
        //    //}
        //    builder.Append(CreateNode("Serializer", xsltTransformationManager.Serializer.GetHashCode()));
        //    builder.Append(CreateNode("BaseXsltName", xsltTransformationManager.BaseXsltName));
        //    builder.Append(CreateNode("BaseXsltUri", xsltTransformationManager.BaseXsltUri));
        //    builder.Append(CreateNode("BaseXsltUriHash", xsltTransformationManager.BaseXsltUriHash));
        //    builder.Append(CreateNode("UseMemcached", (bool)m_context.Application["appStart_usememcached"]));
        //    builder.Append(CreateNode("Transform", xsltTransformationManager.Transform.GetHashCode()));
        //    builder.Append(CreateNode("Resolver", xsltTransformationManager.Resolver.GetHashCode()));
        //    builder.Append(CreateNode("XslTransformationManager", xsltTransformationManager.GetHashCode()));
        //    builder.Append(CreateNode("GlobalXsltParms", m_xsltParams.GetHashCode()));
        //    builder.Append(CreateNode("Processor", m_xslTransformationManager.Processor.GetHashCode()));
        //    builder.Append(CreateNode("RequestXmlSourceExecutionFilePath", m_context.Request.MapPath(HttpContext.Current.Request.CurrentExecutionFilePath)));
        //    builder.Append(CreateNode("RequestUrl", context.RequestUri, true));
        //    builder.Append(CreateNode("RequestIsMemcached", CONTENT_IS_MEMCACHED));
        //    builder.Append(CreateNode("RequestHashcode", context.GetRequestHashcode(false)));
        //    builder.Append(CreateNode("ContextHashcode", context.GetHashCode()));
        //    builder.Append(CreateNode("ContextUri", context.RequestUri, true));
        //    builder.Append(CreateNode("ContextHttpParamsCount", context.HttpParams.Count));
        //    IEnumerator httpParamsEnum = context.HttpParams.GetEnumerator();
        //    int i = 0;
        //    while (httpParamsEnum.MoveNext())
        //    {
        //        string key = context.HttpParams.AllKeys[i].ToString();
        //        builder.Append("<Param>");
        //        builder.Append(CreateNode("Name", key));
        //        builder.Append(CreateNode("Value", context.HttpParams[key]));
        //        builder.Append("</Param>");
        //        i += 1;
        //    }
        //    Client mc = (Client)HttpContext.Current.Application["appStart_memcached"];
        //    IDictionary stats = mc.Stats();

        //    foreach (string key1 in stats.Keys)
        //    {
        //        builder.Append("<Key>");
        //        builder.Append(CreateNode("Name", key1));
        //        Hashtable values = (Hashtable)stats[key1];
        //        foreach (string key2 in values.Keys)
        //        {
        //            builder.Append(CreateNode(key2, values[key2]));
        //        }
        //        builder.Append("</Key>");
        //    }
        //    builder.Append(CreateNode("ContextXsltParamsCount", context.XsltParams.Count));
        //    foreach (DictionaryEntry entry in context.XsltParams)
        //    {
        //        builder.Append(CreateNode("XsltParamName", (string)entry.Key));
        //        builder.Append(CreateNode("XsltParamValue", (string)entry.Value));
        //    }
        //    return builder;
        //}
        //protected String CreateNode(String name, object value, bool useCDATA)
        //{
        //    return "<" + name + "><![CDATA[" + value.ToString() + "]]></" + name + ">";
        //}
        //protected String CreateNode(String name, object value)
        //{
        //    return "<" + name + ">" + value.ToString() + "</" + name + ">";
        //}

        #endregion
    }
}