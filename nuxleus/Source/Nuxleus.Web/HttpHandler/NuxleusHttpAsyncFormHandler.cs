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

        FileStream m_file;
        static long m_position = 0;
        static object m_lock = new object();
        static string m_fileRedirect = "/thanks";
        static int m_statusCode = 303;

        AmazonSimpleDBClient m_amazonSimpleDBClient;
        PutAttributes m_putAttributes;

        int m_pledgeCountTotal;
        int m_pledgeCountDistrict;

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
            string ip = context.Request.UserHostAddress.ToString();

            response.RedirectLocation = m_fileRedirect;
            response.StatusCode = m_statusCode;

            m_amazonSimpleDBClient = (AmazonSimpleDBClient)context.Application["simpledbclient"];
            m_pledgeCountDistrict = (int)context.Application["pledgeCountDistrict"];
            m_pledgeCountTotal = (int)context.Application["pledgeCountTotal"];

            m_putAttributes = new PutAttributes();
            m_putAttributes.DomainName = "4lessig-dev";
            m_putAttributes.ItemName = email;

            m_putAttributes.WithAttribute(
                createReplacableAttribute("name", name, false),
                createReplacableAttribute("location", location, false),
                createReplacableAttribute("zip", zip, false),
                createReplacableAttribute("ip", ip, false)
                );

            if (location == "ca12thdistrict") {
                lock (m_lock) {
                    m_pledgeCountDistrict = m_pledgeCountDistrict + 1;
                }
            } else {
                lock (m_lock) {
                    m_pledgeCountTotal = m_pledgeCountTotal + 1;
                }
            }

            //Console.WriteLine("Form Length: {0}", request.Form.Count);
            //Console.WriteLine("Name: {0}, Email: {1}, Zip: {2}, Location: {3}", name, email, zip, location);

            string pledge = String.Format("<pledge time='{0}' email='{1}'><name>{2}</name><location>{3}</location><zip>{4}</zip></pledge>\r\n", DateTime.Now, email, name, location, zip);


            byte[] output = Encoding.ASCII.GetBytes(pledge);
            lock (m_lock) {
                m_file = new FileStream(request.MapPath("~/App_Data/TrackerLog.xml"), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, 1024, true);
                m_file.Seek(m_position, SeekOrigin.Begin);
                m_position += output.Length;
                return m_file.BeginWrite(output, 0, output.Length, cb, extraData);
            }

            nuxleusAsyncResult.CompleteCall();
            return nuxleusAsyncResult;
        }

        public void EndProcessRequest ( IAsyncResult result ) {
            m_amazonSimpleDBClient.PutAttributes(m_putAttributes);
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
