using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using Nuxleus.Transform;
using System.IO;
using System.Globalization;

namespace Nuxleus.Web {

    public struct XmlServiceOperationReader {

        HttpContext m_httpContext;
        Context m_context;
        Transform.Context m_transformContext;
        XmlReader m_reader;
        TransformRequest m_request;
        TransformResponse m_response;
        XsltTransformationManager m_xslTransformationManager;
        Transform.Transform m_transform;
        UTF8Encoding m_encoding;

        public XmlServiceOperationReader (HttpContext httpContext, Context context, Transform.Context transformContext, XmlReader reader, TransformRequest request, TransformResponse response, XsltTransformationManager transformationManager) {
            m_reader = reader;
            m_httpContext = httpContext;
            m_context = context;
            m_transformContext = transformContext;
            m_request = request;
            m_response = response;
            m_xslTransformationManager = transformationManager;
            m_transform = transformationManager.Transform;
            m_encoding = new UTF8Encoding();
        }

        public TransformResponse Process () {
            XmlReader reader = m_reader;
            string xmlStylesheetHref = String.Empty;
            bool processWithEmbeddedPIStylsheet = false;

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
                                    Uri baseXsltUri = new Uri(m_httpContext.Request.MapPath(xmlStylesheetHref));
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

                                    TransformContext transformContext = new TransformContext();
                                    transformContext.Context = m_transformContext;
                                    transformContext.HttpContext = m_httpContext;
                                    transformContext.XsltTransformationManager = m_xslTransformationManager;
                                    transformContext.XsltName = baseXslt;
                                    transformContext.Response = m_response;
                                    m_request.TransformContext = transformContext;
                                    m_response = m_transform.BeginTransformProcess(m_request);
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

            return m_response;
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
    }
}
