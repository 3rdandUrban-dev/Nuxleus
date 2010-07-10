using System;
using System.Xml;
using net.sf.saxon.value;
using Saxon.Api;
using Sgml;
using System.Web;
using Xameleon.Memcached;
using Xameleon.Transform;
using System.IO;
using System.Text;

namespace Xameleon.Function
{

    public class HttpSgmlToXml
    {

        public HttpSgmlToXml() { }

        public static Value GetDocXml(String uri, HttpContext context)
        {
            return getDocXml(uri, "/html", context);
        }

        public static Value GetDocXml(String uri, String path, HttpContext context)
        {
            return getDocXml(uri, path, context);
        }

        public static String GetDocXml(String uri, String path, bool stripNS, HttpContext context)
        {
            try
            {
                return getXdmNode(uri, path, context).OuterXml;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static Value getDocXml(String uri, String path, HttpContext context)
        {
            try
            {
                return Value.asValue(getXdmNode(uri, path, context).Unwrap());
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private static XdmNode getXdmNode(String uri, String path, HttpContext context)
        {
            Client memcachedClient = (Client)context.Application["memcached"];
            string decodedUri = HttpUtility.UrlDecode(uri);
            string eTag = Context.GenerateETag(decodedUri, Xameleon.Cryptography.HashAlgorithm.SHA1);
            string xhtmlDocString = (string)memcachedClient.Get(eTag);
            SgmlReader sr = new SgmlReader();
            XmlDocument xDoc = new XmlDocument();

            if (xhtmlDocString != null)
            {
                TextReader stringReader = new StringReader(xhtmlDocString);
                sr.InputStream = stringReader;
                xDoc.Load(sr);
            }
            else
            {
                sr.Href = decodedUri;
                xDoc.Load(sr);
                memcachedClient.Set(eTag, xDoc.OuterXml);
            }

            XmlNode xhtml = xDoc.SelectSingleNode(HttpUtility.UrlDecode(path));
            Processor processor = new Processor();
            return processor.NewDocumentBuilder().Build(xhtml);

        }
    }
}
