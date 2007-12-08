using System;
using System.Xml;
using net.sf.saxon.value;
using Saxon.Api;
using Sgml;
using System.Web;
using Nuxleus.Memcached;
using Nuxleus.Transform;
using System.IO;
using System.Text;
using System.Collections;

namespace Xameleon.Function
{

    public class HttpSgmlToXml
    {
        static Hashtable _cacheHashtable = new Hashtable();

        public HttpSgmlToXml () { }
        
        public static String GetXmlFromHtmlString(String html)
        {
          using(SgmlReader sr = new SgmlReader())
          {
            sr.InputStream = new StringReader(html);
            return sr.ReadOuterXml();
          }

        }
        
        public static String DecodeHtmlString (String html)
        {
            return HttpUtility.HtmlDecode(html);
        }
        
        public static Value GetDocXml (String uri, HttpContext context)
        {
            return getDocXml(uri, "/html", context);
        }

        public static Value GetDocXml (String uri, String path, HttpContext context)
        {
            return getDocXml(uri, path, context);
        }

        public static String GetDocXml (String uri, String path, bool stripNS, HttpContext context)
        {
            try
            {
                //Console.WriteLine("Uri: " + uri);
                //Console.WriteLine("Path: " + path);
                //string returnXML = getXdmNode(uri, path, context).OuterXml;
                //Console.WriteLine(returnXML);
                return getXdmNode(uri, path, context).OuterXml;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "<stacktrace>" + e.StackTrace + "</stacktrace>";
            }
        }

        private static Value getDocXml (String uri, String path, HttpContext context)
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

        private static XdmNode getXdmNode (String uri, String path, HttpContext context)
        {
            string decodedUri = HttpUtility.UrlDecode(uri);
            string eTag = Context.GenerateETag(decodedUri, Nuxleus.Cryptography.HashAlgorithm.SHA1);
            XmlNode xhtmlNode = (XmlNode)_cacheHashtable[eTag];
            
            using(SgmlReader sr = new SgmlReader())
            {
              if (xhtmlNode == null)
              {
                  try
                  {
                      sr.Href = decodedUri;
                      XmlDocument xDoc = new XmlDocument();
                      xDoc.Load(sr);
                      xhtmlNode = xDoc.FirstChild;
                      _cacheHashtable.Add(eTag, xhtmlNode);
                  }
                  catch (Exception e)
                  {
                      Console.WriteLine(e.Message);
                  }
              }
              
              return new Processor().NewDocumentBuilder().Build(xhtmlNode.SelectSingleNode(HttpUtility.UrlDecode(path)));
              
            }
        }
    }
}
