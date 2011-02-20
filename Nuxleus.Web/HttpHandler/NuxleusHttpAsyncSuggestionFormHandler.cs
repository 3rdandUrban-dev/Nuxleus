//// Copyright (c) 2006 by M. David Peterson
//// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
//// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.IO;
//using System.Text;
//using System.Web;
//using Nuxleus.Web.HttpApplication;
//using Nuxleus.Atom;
//using System.Xml.Serialization;
//using System.Xml;
//using System.Xml.Linq;
//using System.Linq;
//using System.Xml.Xsl;
//using Sgml;

//namespace Nuxleus.Web.HttpHandler {

//    public class NuxleusHttpAsyncSuggestionFormHandler : IHttpAsyncHandler
//    {

//        FileStream m_file;
//        HttpResponse m_response;
//        static object m_lock = new object();
//        static XmlSerializer m_xSerializer = new XmlSerializer(typeof(Entry));
//        static Encoding m_encoding = new UTF8Encoding();
//        static XNamespace h = "http://www.w3.org/1999/xhtml";
//        static XslCompiledTransform cleanseHtml = new XslCompiledTransform();

//        AmazonSimpleDBClient m_amazonSimpleDBClient;
//        PutAttributes m_putAttributes;

//        PledgeCount m_pledgeCount;

//        public void ProcessRequest(HttpContext context) {
//            //not called
//        }

//        public bool IsReusable {
//            get { return false; }
//        }

//        static NuxleusHttpAsyncSuggestionFormHandler() {

//            // In the back of my mind there's this nagging thought "There's a reason /NOT/ to use a static
//            // constructor to pre-load the XslCompiledTransform", but for life of me I can't remember why that might be.
//            // TODO: Talk to somebody who knows better and verify if there are reasons why this might be a bad idea.
//            cleanseHtml.Load(String.Format("{0}/service/transform/cleansehtml.xsl", Environment.CurrentDirectory));
//        }

//        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {

//            HttpRequest request = context.Request;
//            m_response = context.Response;
//            m_response.ContentType = "text/xml";

//            //cleanseHtml.Load(String.Format("{0}/service/transform/cleansehtml.xsl", Environment.CurrentDirectory));

//            NameValueCollection form = request.Form;
//            NameValueCollection headers = request.Headers;

//            string name = form.Get("name");
//            string slug = headers.Get("slug");
//            string base_uri = form.Get("base_uri");
//            string base_path = form.Get("base_path");
//            string title = form.Get("title");
//            string ip = context.Request.UserHostAddress.ToString();

//            this.LogInfo("Slug: {0}", slug);

//            var paragraphs = from line in ReadParagraphsFromContent(HttpUtility.HtmlDecode(form.Get("suggestion")))
//                             select GetXmlFromHtmlString(line);

//            XDocument doc = new XDocument(
//                new XElement(h + "div",
//                    GetXmlFromHtmlString(HttpUtility.HtmlDecode(form.Get("suggestion")))
//                )
//            );

//            using (XmlReader reader = doc.CreateReader()) {
//                while (reader.Read()) {
//                    this.LogInfo(reader.ReadOuterXml());
//                }
//            }

//            // TODO: I need to rewrite Sylvain's Nuxleus.Atom.Feed and Nuxleus.Atom.Entry classes to 
//            // use System.Xml.Linq instead of System.Xml. For now we'll revert back to XmlDocument
//            // which we can then extract the DocumentElement to gain access to the underlying XmlNode
//            // which is required by the XHTMLBody class.  Of course, I'm not really 100% sure switching 
//            // to System.Xml.Linq sure it will save all that much as far as resources are concerned 
//            // which is why I'm being lazy about it ;-)
//            XmlDocument nDoc = new XmlDocument();
//            using (MemoryStream stream = new MemoryStream()) {
//                using (XmlWriter writer = new XmlTextWriter(stream, Encoding.UTF8)) {
//                    cleanseHtml.Transform(doc.CreateReader(), null, writer);
//                    stream.Seek(0, 0);
//                    nDoc.Load(stream);
//                }
//            }

//            //XmlNodeList nodeList = nDoc.GetElementsByTagName("p", "http://www.w3.org/1999/xhtml");
//            //XmlNode[] elements = new XmlNode[nodeList.Count];

//            //int i = 0;
//            //foreach (XmlNode node in nodeList) {
//            //    elements[i] = node;
//            //    i++;
//            //}

//            XHTMLBody body = new XHTMLBody { Elements = new XmlNode[] { nDoc.DocumentElement } };
//            Content content = new Content { Div = body, Type = "xhtml" };
//            Entry entry = new Entry { Content = content, Published = DateTime.Now, Updated = DateTime.Now, Title = title };

//            m_amazonSimpleDBClient = (AmazonSimpleDBClient)context.Application["simpledbclient"];
//            m_pledgeCount = (PledgeCount)context.Application["pledgeCount"];

//            Queue<string> pledgeQueue = (Queue<string>)context.Application["pledgeQueue"];

//            //m_putAttributes = new PutAttributes();
//            //m_putAttributes.DomainName = "whattheyshouldhavesaid";
//            //m_putAttributes.ItemName = slug;

//            //m_putAttributes.WithAttribute(
//            //    createReplacableAttribute("name", name, false),
//            //    createReplacableAttribute("title", title, false),
//            //    createReplacableAttribute("base_uri", base_uri, false),
//            //    createReplacableAttribute("ip", ip, false)
//            //    );

//            //pledgeQueue.Enqueue(title);


//            using (MemoryStream stream = new MemoryStream()) {

//                m_xSerializer.Serialize(stream, (Entry)entry);
//                stream.Seek(0, 0);

//                byte[] output = stream.GetBuffer();
//                char[] chars = m_encoding.GetChars(output, 0, output.Length);

//                using (XmlReader reader = XmlReader.Create(stream)) {
//                    using (StreamWriter sWriter = new StreamWriter(m_response.OutputStream, Encoding.UTF8)) {
//                        while (reader.Read()) {
//                            sWriter.Write(reader.ReadOuterXml());
//                        }
//                    }
//                }

//                lock (m_lock) {
//                    string basePath = request.MapPath(String.Format("~{0}", base_path));
//                    this.LogInfo(basePath);
//                    if (!Directory.Exists(basePath)) {
//                        Directory.CreateDirectory(basePath);
//                    }
//                    m_file = new FileStream(String.Format("{0}/{1}.xml", basePath, Guid.NewGuid()), FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 1024, true);
//                    m_file.Seek(0, 0);
//                    return m_file.BeginWrite(output, 0, output.Length, cb, extraData);
//                }
//            }
//        }

//        public void EndProcessRequest(IAsyncResult result) {

//            m_file.Close();
//            m_response.OutputStream.Close();
//            //m_amazonSimpleDBClient.PutAttributes(m_putAttributes);
//        }

//        private ReplaceableAttribute createReplacableAttribute(string name, string value, bool replacable) {
//            ReplaceableAttribute replacableAttribute = new ReplaceableAttribute();
//            replacableAttribute.Name = name;
//            replacableAttribute.Value = value;
//            replacableAttribute.Replace = replacable;
//            return replacableAttribute;
//        }

//        public static IEnumerable<string> ReadParagraphsFromContent(string content) {
//            using (StringReader reader = new StringReader(content)) {
//                while (true) {
//                    string s = reader.ReadLine();
//                    if (s == null)
//                        break;
//                    if (s.Length == 0)
//                        continue;
//                    yield return String.Format("{0}<br/>", s);
//                }
//            }
//        }

//        public static XNode MarkupWithHtmlParagraph(string paragraph) {
//            return new XElement(h + "p",
//                    GetXmlFromHtmlString(paragraph)
//                );
//        }

//        public static XNode GetXmlFromHtmlString(String html) {
//            using (SgmlReader sr = new SgmlReader()) {
//                string htmlWrapper = String.Format("<div xmlns=\"http://www.w3.org/1999/xhtml\">{0}</div>", html);
//                try {
//                    sr.InputStream = new StringReader(htmlWrapper);
//                    return XElement.Parse(sr.ReadOuterXml(), LoadOptions.None);
//                } catch {
//                    return new XText(HttpUtility.HtmlEncode(html));
//                }
//            }
//        }

//        //string ConvertLinebreaks(String content) {
//        //    StringBuilder builder = new StringBuilder();
//        //    using (StringReader reader = new StringReader(content)) {
//        //        while (true) {
//        //            string s = reader.ReadLine();
//        //            if (s == null)
//        //                break;
//        //            if (s.Length == 0)
//        //                continue;
//        //            builder.Append(String.Format("{0}<br/>", s));
//        //        }
//        //    }
//        //    return builder.ToString();
//        //}
//    }
//}
