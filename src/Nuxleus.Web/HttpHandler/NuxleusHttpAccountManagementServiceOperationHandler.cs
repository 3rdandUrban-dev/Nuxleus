// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Linq;
using System.Xml.Xsl;
using Nuxleus.Core;
using Nuxleus.Extension.Aws.SimpleDb;
using Sgml;
using AwsSdbModel = Nuxleus.Extension.Aws.SimpleDb;

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpAccountManagementServiceOperationHandler : IHttpAsyncHandler
    {

        FileStream m_file;
        HttpResponse m_response;
        static object m_lock = new object();
        static Encoding m_encoding = new UTF8Encoding();
        static XNamespace h = "http://www.w3.org/1999/xhtml";
        static XslCompiledTransform cleanseHtml = new XslCompiledTransform();
        NuxleusAsyncResult m_asyncResult;
        string m_returnLocation;


        public void ProcessRequest(HttpContext context)
        {
            //not called
        }

        public bool IsReusable
        {
            get { return false; }
        }

        static NuxleusHttpAccountManagementServiceOperationHandler()
        {
            // In the back of my mind there's this nagging thought "There's a reason /NOT/ to use a static
            // constructor to pre-load the XslCompiledTransform", but for life of me I can't remember why that might be.
            // TODO: Talk to somebody who knows better and verify if there are reasons why this might be a bad idea.
            cleanseHtml.Load(HttpContext.Current.Server.MapPath("/service/transform/build-feed.xsl"));
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {

            HttpRequest request = context.Request;
            m_response = context.Response;
            m_response.ContentType = "text/xml";

            m_asyncResult = new NuxleusAsyncResult(cb, extraData);

            NameValueCollection form = request.Form;

            m_returnLocation = form.Get("return_url");
            string name = form.Get("name");
            string slug = form.Get("slug");
            string base_uri = form.Get("base_uri");
            string base_path = form.Get("base_path");
            string title = form.Get("title");
            string ip = context.Request.UserHostAddress.ToString();
            string item = context.Request.Cookies["userid"].Value;
            string username = form.Get("username");
            string email = form.Get("email");
            string user_time_zone = form.Get("user[time_zone]");
            string user_url = form.Get("user[url]");
            string user_description = form.Get("user[description]");
            string user_location = form.Get("user[location]");
            string user_lang = form.Get("user[lang]");


            List<AwsSdbModel.Attribute> attributes = new List<AwsSdbModel.Attribute>();
            attributes.Add(new AwsSdbModel.Attribute { Name = "name", Value = name });
            attributes.Add(new AwsSdbModel.Attribute { Name = "username", Value = username });
            attributes.Add(new AwsSdbModel.Attribute { Name = "email", Value = email });
            attributes.Add(new AwsSdbModel.Attribute { Name = "user_time_zone", Value = user_time_zone });
            attributes.Add(new AwsSdbModel.Attribute { Name = "user_url", Value = user_url });
            attributes.Add(new AwsSdbModel.Attribute { Name = "user_description", Value = user_description });
            attributes.Add(new AwsSdbModel.Attribute { Name = "user_location", Value = user_location });
            attributes.Add(new AwsSdbModel.Attribute { Name = "user_lang", Value = user_lang });

            PutAttributesTask task = new PutAttributesTask { DomainName = new Domain { Name = "account" }, Item = new Item { ItemName = item, Attribute = attributes } };

            IResponse response = task.Invoke();

            //var paragraphs = from line in ReadParagraphsFromContent(HttpUtility.HtmlDecode(form.Get("suggestion")))
            //                 select GetXmlFromHtmlString(line);

            XDocument doc = new XDocument(
                new XElement(h + "div",
                    new XElement(h + "div", name),
                    new XElement(h + "div", username),
                    new XElement(h + "div", email),
                    new XElement(h + "div", user_time_zone),
                    new XElement(h + "div", user_url),
                    new XElement(h + "div", user_description),
                    new XElement(h + "div", user_location),
                    new XElement(h + "div", user_lang),
                    new XElement(h + "div", (PutAttributesResponse)response)
                )
            );
            string xmlOutput = null;
            using (TextWriter writer = new StringWriter())
            {
                doc.Save(writer);
                xmlOutput = writer.ToString();
                m_response.Write(xmlOutput);
            }

            byte[] output = m_encoding.GetBytes(xmlOutput);
            char[] chars = m_encoding.GetChars(output, 0, output.Length);

            lock (m_lock)
            {
                string basePath = request.MapPath(String.Format("~{0}", base_path));

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                m_file = new FileStream(String.Format("{0}/{1}.xml", basePath, Guid.NewGuid()), FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 1024, true);
                m_file.Seek(0, 0);
                return m_file.BeginWrite(output, 0, output.Length, cb, extraData);
            }
            //m_asyncResult.CompleteCall();
            //return m_asyncResult;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            m_response.Redirect(m_returnLocation);
            m_file.Close();
        }


        public static IEnumerable<string> ReadParagraphsFromContent(string content)
        {
            using (StringReader reader = new StringReader(content))
            {
                while (true)
                {
                    string s = reader.ReadLine();
                    if (s == null)
                        break;
                    if (s.Length == 0)
                        continue;
                    yield return String.Format("{0}<br/>", s);
                }
            }
        }

        public static XNode MarkupWithHtmlParagraph(string paragraph)
        {
            return new XElement(h + "p",
                    GetXmlFromHtmlString(paragraph)
                );
        }

        public static XNode GetXmlFromHtmlString(String html)
        {
            using (SgmlReader sr = new SgmlReader())
            {
                string htmlWrapper = String.Format("<div xmlns=\"http://www.w3.org/1999/xhtml\">{0}</div>", html);
                try
                {
                    sr.InputStream = new StringReader(htmlWrapper);
                    return XElement.Parse(sr.ReadOuterXml(), LoadOptions.None);
                }
                catch
                {
                    return new XText(HttpUtility.HtmlEncode(html));
                }
            }
        }

        //string ConvertLinebreaks(String content) {
        //    StringBuilder builder = new StringBuilder();
        //    using (StringReader reader = new StringReader(content)) {
        //        while (true) {
        //            string s = reader.ReadLine();
        //            if (s == null)
        //                break;
        //            if (s.Length == 0)
        //                continue;
        //            builder.Append(String.Format("{0}<br/>", s));
        //        }
        //    }
        //    return builder.ToString();
        //}
    }
}
