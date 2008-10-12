// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Nuxleus.Web.HttpApplication;
using Nuxleus.Atom;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Nuxleus.Web.HttpHandler {

    public struct NuxleusHttpAsyncSuggestionFormHandler : IHttpAsyncHandler {

        FileStream m_file;
        HttpResponse m_response;
        static object m_lock = new object();
        static XmlSerializer m_xSerializer = new XmlSerializer(typeof(Entry));
        static Encoding m_encoding = new UTF8Encoding();

        AmazonSimpleDBClient m_amazonSimpleDBClient;
        PutAttributes m_putAttributes;

        PledgeCount m_pledgeCount;

        public void ProcessRequest(HttpContext context) {
            //not called
        }

        public bool IsReusable {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {

            HttpRequest request = context.Request;
            m_response = context.Response;

            NameValueCollection form = request.Form;

            string name = form.Get("name");
            string slug = form.Get("slug");
            string base_uri = form.Get("base_uri");
            string base_path = form.Get("base_path");
            string title = form.Get("title");
            string ip = context.Request.UserHostAddress.ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(String.Format("<p xmlns=\"http://www.w3.org/1999/xhtml\">{0}</p>", form.Get("suggestion")));

            XHTMLBody body = new XHTMLBody { Elements = new XmlNode[] { doc.DocumentElement } };
            Content content = new Content { Div = body, Type = "xhtml" };

            Entry entry = new Entry { Content = content, Published = DateTime.Now, Updated = DateTime.Now, Title = title };

            Console.WriteLine("Published: {0}, Updated: {1}, Title: {2}, Content: {3}", entry.Published, entry.Updated, entry.Title, entry.Content.Text);

            m_amazonSimpleDBClient = (AmazonSimpleDBClient)context.Application["simpledbclient"];
            m_pledgeCount = (PledgeCount)context.Application["pledgeCount"];

            Queue<string> pledgeQueue = (Queue<string>)context.Application["pledgeQueue"];

            //m_putAttributes = new PutAttributes();
            //m_putAttributes.DomainName = "whattheyshouldhavesaid";
            //m_putAttributes.ItemName = slug;

            //m_putAttributes.WithAttribute(
            //    createReplacableAttribute("name", name, false),
            //    createReplacableAttribute("title", title, false),
            //    createReplacableAttribute("base_uri", base_uri, false),
            //    createReplacableAttribute("ip", ip, false)
            //    );

            //pledgeQueue.Enqueue(title);
            
            byte[] output;
            using (MemoryStream stream = new MemoryStream()) {
                stream.Seek(0, 0);
                using (XmlWriter writer = new XmlTextWriter(stream, Encoding.Unicode)) {
                    m_xSerializer.Serialize(writer, (Entry)entry);
                    output = stream.GetBuffer();
                    char[] chars = m_encoding.GetChars(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, output, 0, output.Length));
                    

                    using (TextWriter sWriter = Console.Out) {
                        sWriter.Write(chars, 0, chars.Length);
                    }
                    using (StreamWriter sWriter = new StreamWriter(m_response.OutputStream, Encoding.UTF8)) {
                        sWriter.Write(chars, 0, chars.Length);
                    }

                    lock (m_lock) {
                        string basePath = request.MapPath(String.Format("~{0}", base_path));
                        Console.WriteLine(basePath);
                        if (!Directory.Exists(basePath)) {
                            Directory.CreateDirectory(basePath);
                        }
                        m_file = new FileStream(String.Format("{0}/{1}.xml", basePath, Guid.NewGuid()), FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 1024, true);
                        m_file.Seek(0, 0);
                        return m_file.BeginWrite(output, 0, output.Length, cb, extraData);
                    }
                }
            }
        }

        public void EndProcessRequest(IAsyncResult result) {
            m_response.ContentType = "text/xml";
            m_file.Close();
            m_response.OutputStream.Close();
            //m_amazonSimpleDBClient.PutAttributes(m_putAttributes);
        }

        private ReplaceableAttribute createReplacableAttribute(string name, string value, bool replacable) {
            ReplaceableAttribute replacableAttribute = new ReplaceableAttribute();
            replacableAttribute.Name = name;
            replacableAttribute.Value = value;
            replacableAttribute.Replace = replacable;
            return replacableAttribute;
        }
    }
}
