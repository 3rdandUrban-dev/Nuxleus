// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.

using System;
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
using Xameleon.Configuration;
using Xameleon.Transform;
using System.Collections.Generic;
using Xameleon.Memcached;
using Xameleon.Cryptography;

namespace Xameleon.HttpHandler
{

    public class XameleonHttpAsyncHandler : IHttpAsyncHandler
    {

        XsltTransformationManager _xslTransformationManager;
        MemcachedClient _memcachedClient;
        Transform.Transform _transform;
        TextWriter _writer;
        StringBuilder _builder;
        HttpContext _context;
        Hashtable _xsltParams;
        Hashtable _namedXsltHashtable;
        TransformServiceAsyncResult _transformAsyncResult;
        AsyncCallback _callback;
        String _httpMethod;
        Exception _exception;
        Context _transformContext;
        bool _CONTENT_IS_MEMCACHED = false;
        bool _USE_MEMCACHED = false;
        HashAlgorithm _hashAlgorithm = HashAlgorithm.SHA1;

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

            _context = context;
            _httpMethod = _context.Request.HttpMethod;
            _memcachedClient = (Client)context.Application["memcached"];
            _xslTransformationManager = (XsltTransformationManager)context.Application["xslTransformationManager"];
            _transform = _xslTransformationManager.Transform;
            _xsltParams = (Hashtable)context.Application["globalXsltParams"];
            _namedXsltHashtable = (Hashtable)context.Application["namedXsltHashtable"];
            _transformContext = new Context(context, _hashAlgorithm, (string)context.Application["hashkey"], fileInfo, (Hashtable)_xsltParams.Clone(), fileInfo.LastWriteTimeUtc, fileInfo.Length);
            _transformAsyncResult = new TransformServiceAsyncResult(cb, extraData);
            _callback = cb;
            _transformAsyncResult._context = context;
            _builder = new StringBuilder();
            _CONTENT_IS_MEMCACHED = false;
            _USE_MEMCACHED = (bool)context.Application["usememcached"];

            bool hasXmlSourceChanged = _xslTransformationManager.HasXmlSourceChanged(_transformContext.RequestXmlETag);
            bool hasBaseXsltSourceChanged = _xslTransformationManager.HasBaseXsltSourceChanged();

            if (_USE_MEMCACHED)
            {
                string obj = (string)_memcachedClient.Get(_transformContext.GetRequestHashcode(true));
                if (obj != null && !(hasXmlSourceChanged || hasBaseXsltSourceChanged))
                {
                    _builder.Append(obj);
                    _CONTENT_IS_MEMCACHED = true;
                    if ((bool)context.Application["debug"])
                        context.Response.ContentType = "text";
                    else
                        context.Response.ContentType = "text/xml";
                }
                else
                {
                    _writer = new StringWriter(_builder);
                    _CONTENT_IS_MEMCACHED = false;
                }
            }
            else
            {
                _writer = new StringWriter(_builder);
            }

            if ((bool)context.Application["debug"])
            {
                context.Response.Write("<debug>");
                context.Response.Write("<file-info>");
                context.Response.Write("Has Xml Changed: " + hasXmlSourceChanged + ":" + _transformContext.RequestXmlETag + "<br/>");
                context.Response.Write("Has Xslt Changed: " + hasBaseXsltSourceChanged + "<br/>");
                context.Response.Write("Xml ETag: " + _transformContext.GetRequestHashcode(true) + "<br/>");
                context.Response.Write("XdmNode Count: " + _xslTransformationManager.GetXdmNodeHashtableCount() + "<br/>");
                context.Response.Write("</file-info>");
                context.Application["debugOutput"] = (string)("<DebugOutput>" + WriteDebugOutput(_transformContext, _xslTransformationManager, new StringBuilder(), _CONTENT_IS_MEMCACHED).ToString() + "</DebugOutput>");
                context.Response.Write("</debug>");
            }

            try
            {

                switch (_httpMethod)
                {
                    case "GET":
                        {
                            if (_CONTENT_IS_MEMCACHED)
                            {
                                _transformAsyncResult.CompleteCall();
                                return _transformAsyncResult;
                            }
                            else
                            {
                                try
                                {
                                    _transform.BeginProcess(_transformContext, context, _xslTransformationManager, _writer, _transformAsyncResult);
                                    return _transformAsyncResult;
                                }
                                catch (Exception e)
                                {
                                    _exception = e;
                                    WriteError();
                                    _transformAsyncResult.CompleteCall();
                                    return _transformAsyncResult;
                                }
                            }
                        }
                    case "PUT":
                        {
                            _transform.BeginProcess(_transformContext, context, _xslTransformationManager, _writer, _transformAsyncResult);
                            return _transformAsyncResult;
                        }
                    case "POST":
                        {
                            _transform.BeginProcess(_transformContext, context, _xslTransformationManager, _writer, _transformAsyncResult);
                            return _transformAsyncResult;
                        }
                    case "DELETE":
                        {
                            _transform.BeginProcess(_transformContext, context, _xslTransformationManager, _writer, _transformAsyncResult);
                            return _transformAsyncResult;
                        }
                    default:
                        {
                            _transform.BeginProcess(_transformContext, context, _xslTransformationManager, _writer, _transformAsyncResult);
                            return _transformAsyncResult;
                        }
                }

            }
            catch (Exception ex)
            {
                _exception = ex;
                WriteError();
                _transformAsyncResult.CompleteCall();
                return _transformAsyncResult;
            }
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            using (_writer)
            {
                string output = _builder.ToString();
                using (TextWriter writer = _context.Response.Output)
                {
                    writer.Write(output);
                }
                _transformContext.Clear();
                if (!_CONTENT_IS_MEMCACHED && _USE_MEMCACHED)
                    _memcachedClient.Set(_transformContext.GetRequestHashcode(true), output);
                if ((bool)_context.Application["debug"])
                    _context.Response.Write((string)_context.Application["debugOutput"]);
            }
        }

        private void WriteError()
        {
            _context.Response.Output.WriteLine("<html>");
            _context.Response.Output.WriteLine("<head>");
            _context.Response.Output.WriteLine("<title>Xameleon Transformation Error</title>");
            _context.Response.Output.WriteLine("</head>");
            _context.Response.Output.WriteLine("<body>");
            _context.Response.Output.WriteLine("<h3>Error Message</h3>");
            _context.Response.Output.WriteLine("<p>" + _exception.Message + "</p>");
            _context.Response.Output.WriteLine("<h3>Error Source</h3>");
            _context.Response.Output.WriteLine("<p>" + _exception.Source + "</p>");
            _context.Response.Output.WriteLine("<h3>Error StackTrace</h3>");
            _context.Response.Output.WriteLine("<p>" + _exception.StackTrace + "</p>");
            _context.Response.Output.WriteLine("</body>");
            _context.Response.Output.WriteLine("</html>");
        }

        protected StringBuilder WriteDebugOutput(Context context, XsltTransformationManager xsltTransformationManager, StringBuilder builder, bool CONTENT_IS_MEMCACHED)
        {
            builder.Append(CreateNode("Request_File_ETag", context.ETag));
            builder.Append(CreateNode("CompilerBaseUri", xsltTransformationManager.Compiler.BaseUri));
            builder.Append(CreateNode("Compiler", xsltTransformationManager.Compiler.GetHashCode()));
            //foreach(System.Reflection.PropertyInfo t in HttpContext.Current.GetType().GetProperties()){
            // 
            //}
            builder.Append(CreateNode("Serializer", xsltTransformationManager.Serializer.GetHashCode()));
            builder.Append(CreateNode("BaseXsltName", xsltTransformationManager.BaseXsltName));
            builder.Append(CreateNode("BaseXsltUri", xsltTransformationManager.BaseXsltUri));
            builder.Append(CreateNode("BaseXsltUriHash", xsltTransformationManager.BaseXsltUriHash));
            builder.Append(CreateNode("UseMemcached", (bool)_context.Application["appStart_usememcached"]));
            builder.Append(CreateNode("Transform", xsltTransformationManager.Transform.GetHashCode()));
            builder.Append(CreateNode("Resolver", xsltTransformationManager.Resolver.GetHashCode()));
            builder.Append(CreateNode("XslTransformationManager", xsltTransformationManager.GetHashCode()));
            builder.Append(CreateNode("GlobalXsltParms", _xsltParams.GetHashCode()));
            builder.Append(CreateNode("Processor", _xslTransformationManager.Processor.GetHashCode()));
            builder.Append(CreateNode("RequestXmlSourceExecutionFilePath", _context.Request.MapPath(HttpContext.Current.Request.CurrentExecutionFilePath)));
            builder.Append(CreateNode("RequestUrl", context.RequestUri, true));
            builder.Append(CreateNode("RequestIsMemcached", CONTENT_IS_MEMCACHED));
            builder.Append(CreateNode("RequestHashcode", context.GetRequestHashcode(false)));
            builder.Append(CreateNode("ContextHashcode", context.GetHashCode()));
            builder.Append(CreateNode("ContextUri", context.RequestUri, true));
            builder.Append(CreateNode("ContextHttpParamsCount", context.HttpParams.Count));
            IEnumerator httpParamsEnum = context.HttpParams.GetEnumerator();
            int i = 0;
            while (httpParamsEnum.MoveNext())
            {
                string key = context.HttpParams.AllKeys[i].ToString();
                builder.Append("<Param>");
                builder.Append(CreateNode("Name", key));
                builder.Append(CreateNode("Value", context.HttpParams[key]));
                builder.Append("</Param>");
                i += 1;
            }
            Client mc = (Client)HttpContext.Current.Application["appStart_memcached"];
            IDictionary stats = mc.Stats();

            foreach (string key1 in stats.Keys)
            {
                builder.Append("<Key>");
                builder.Append(CreateNode("Name", key1));
                Hashtable values = (Hashtable)stats[key1];
                foreach (string key2 in values.Keys)
                {
                    builder.Append(CreateNode(key2, values[key2]));
                }
                builder.Append("</Key>");
            }
            builder.Append(CreateNode("ContextXsltParamsCount", context.XsltParams.Count));
            foreach (DictionaryEntry entry in context.XsltParams)
            {
                builder.Append(CreateNode("XsltParamName", (string)entry.Key));
                builder.Append(CreateNode("XsltParamValue", (string)entry.Value));
            }
            return builder;
        }
        protected String CreateNode(String name, object value, bool useCDATA)
        {
            return "<" + name + "><![CDATA[" + value.ToString() + "]]></" + name + ">";
        }
        protected String CreateNode(String name, object value)
        {
            return "<" + name + ">" + value.ToString() + "</" + name + ">";
        }

        #endregion
    }
}