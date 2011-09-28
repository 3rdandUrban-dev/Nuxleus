using System;
using System.Xml;
using System.Web;
using Nuxleus.Core;
using System.IO;

namespace Nuxleus.Transform {

    public struct TransformContext {

        IAsyncResult m_asyncResult;
        Context m_context;
        HttpContext m_httpContext;
        XsltTransformationManager m_xsltTransformManager;
        TransformResponse m_transformResult;
        String m_xsltName;
        TextWriter m_writer;

        public IAsyncResult AsyncResult { get { return m_asyncResult; } set { m_asyncResult = value; } }
        public TransformResponse Response { get { return m_transformResult; } set { m_transformResult = value; } }
        public Context Context { get { return m_context; } set { m_context = value; } }
        public XsltTransformationManager XsltTransformationManager { get { return m_xsltTransformManager; } set { m_xsltTransformManager = value; } }
        public HttpContext HttpContext { get { return m_httpContext; } set { m_httpContext = value; } }
        public String XsltName { get { return m_xsltName; } set { m_xsltName = value; } }
        public TextWriter Writer { get { return m_writer; } set { m_writer = value; } }

    }
}
