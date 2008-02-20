// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Nuxleus.Agent;

namespace Nuxleus.Web.HttpHandler {

    public struct NuxleusHttpAsyncFormHandler : IHttpAsyncHandler {

        static string m_fileRedirect = "/thanks";
        static int m_statusCode = 303;

        AmazonSimpleDBClient m_amazonSimpleDBClient;

        public void ProcessRequest ( HttpContext context ) {
            //not called
        }

        public bool IsReusable {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest ( HttpContext context, AsyncCallback cb, object extraData ) {

            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            NuxleusAsyncResult nuxleusAsyncResult = new NuxleusAsyncResult(cb, extraData);

            string name = request.Form.Get("name");
            string email = request.Form.Get("email");
            string zip = request.Form.Get("zip");
            string location = request.Form.Get("location");

            //StringBuilder builder = new StringBuilder();

            response.RedirectLocation = m_fileRedirect;
            response.StatusCode = m_statusCode;

            m_amazonSimpleDBClient = (AmazonSimpleDBClient)context.Application["simpledbclient"];

            PutAttributes putAttributes = new PutAttributes();
            putAttributes.DomainName = "4lessig";
            putAttributes.ItemName = email;

            putAttributes.WithAttribute(
                createReplacableAttribute("name", name, false),
                createReplacableAttribute("location", name, false),
                createReplacableAttribute("zip", name, false)
                );

            m_amazonSimpleDBClient.PutAttributes(putAttributes);

            Console.WriteLine("Form Length: {0}", request.Form.Count);
            Console.WriteLine("Name: {0}, Email: {1}, Zip: {2}, Location: {3}", name, email, zip, location);

            nuxleusAsyncResult.CompleteCall();
            return nuxleusAsyncResult;

            //builder.AppendFormat("<request time='{0}' filePath='{1}'>", DateTime.Now, request.FilePath);

            //IEnumerator enumerator = request.Headers.GetEnumerator();
            //for (int i = 0; enumerator.MoveNext(); i++) {
            //    string name = request.Headers.AllKeys[i].ToString();
            //    builder.AppendFormat("<{0}>:{1}</{0}>", name, HttpUtility.HtmlEncode(request.Headers[name]));
            //}

            //builder.Append("</request>\r\n");

            //byte[] output = Encoding.ASCII.GetBytes(builder.ToString());
            //lock (m_lock) {
            //    m_file = new FileStream(request.MapPath("~/App_Data/TrackerLog.xml"), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, 1024, true);
            //    m_file.Seek(m_position, SeekOrigin.Begin);
            //    m_position += output.Length;
            //    return m_file.BeginWrite(output, 0, output.Length, cb, extraData);
            //}
        }

        public void EndProcessRequest ( IAsyncResult result ) {
            //m_file.EndWrite(result);
            //m_file.Close();
        }

        private ReplaceableAttribute createReplacableAttribute ( string name, string value, bool replacable ) {
            ReplaceableAttribute replacableAttribute = new ReplaceableAttribute();
            replacableAttribute.Name = name;
            replacableAttribute.Value = value;
            replacableAttribute.Replace = replacable;
            return replacableAttribute;
        }


    }
}
