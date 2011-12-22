using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Nuxleus.Transform;
using Sgml;

namespace Xameleon.Function
{

    public class HttpSgmlToXml
    {
        static IDictionary<String, String> m_CacheDictionary = new Dictionary<String, String>();

        public HttpSgmlToXml () { }

        public static String GetXmlFromHtmlString (String html)
        {
            using (SgmlReader sr = new SgmlReader())
            {
                sr.InputStream = new StringReader(html);
                return sr.ReadOuterXml();
            }
        }

        public static StringReader GetStreamFromHtmlString (String html)
        {
            return new StringReader(html);
        }

        public static String DecodeHtmlString (String html)
        {
            return HttpUtility.HtmlDecode(html);
        }

        //public static Value GetDocXml (String uri, HttpContext context)
        //{
        //    return getDocXml(uri, "/html", context);
        //}

        //public static Value GetDocXml (String uri, String path, HttpContext context)
        //{
        //    return getDocXml(uri, path, context);
        //}

        public static String GetDocXml (String uri, String path, bool stripNS, HttpContext context)
        {
            try
            {
                return getXmlReader(uri, path, context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "<stacktrace>" + e.StackTrace + "</stacktrace>";
            }
        }

        //private static Value getDocXml (String uri, String path, HttpContext context)
        //{
        //    try
        //    {
        //        return Value.asValue(getXdmNode(uri, path, context).Unwrap());
        //    }
        //    catch (Exception e)
        //    {
        //        throw;
        //    }
        //}

        private static String getXmlReader (String uri, String path, HttpContext context)
        {
            string decodedUri = HttpUtility.UrlDecode(uri);
            string eTag = Context.GenerateETag(decodedUri, Nuxleus.Cryptography.HashAlgorithm.SHA1);
            String xhtml = String.Empty;

            try
            {

                if (m_CacheDictionary.ContainsKey(eTag))
                {
                    xhtml = m_CacheDictionary[eTag];
                }
                else
                {
                    using (SgmlReader sr = new SgmlReader())
                    {
                        try
                        {
                            if ((bool)context.Application["usememcached"])
                            {
                                MemcachedClient m_client = (MemcachedClient)context.Application["memcached"];
                                object keyValue;
                                if (m_client.TryGet(eTag, out keyValue))
                                {
                                    sr.InputStream = GetStreamFromHtmlString((string)keyValue);
                                }

                                else
                                {
                                    sr.Href = decodedUri;
                                    m_client.Store(StoreMode.Add, eTag, sr.ReadOuterXml(), DateTime.Now.AddMinutes(60));
                                }

                            }
                            else
                            {
                                sr.Href = decodedUri;

                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                        }

                        xhtml = sr.ReadOuterXml();
                        m_CacheDictionary.Add(eTag, xhtml);

                    }
                }

                return xhtml;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

            }

            return String.Format(@"<html xmlns='http://www.w3.org/1999/xhtml'>
                                   <head>
                                    <title>No Readable HTML</title>
                                   </head>
                                   <body>
                                    <h1>No readable HTML was located at the specified URL</h1>
                                   </body>
                                 </html>");
        }
    }
}
