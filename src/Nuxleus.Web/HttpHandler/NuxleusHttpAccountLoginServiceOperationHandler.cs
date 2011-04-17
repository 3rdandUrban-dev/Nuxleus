// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Nuxleus.Core;
using Nuxleus.Extension.Aws.SimpleDb;
using AwsSdbModel = Nuxleus.Extension.Aws.SimpleDb;

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpAccountLoginServiceOperationHandler : IHttpAsyncHandler
    {
        HttpRequest m_request;
        HttpResponse m_response;
        static Encoding m_encoding = new UTF8Encoding();
        static Domain m_domain = new Domain
        {
            Name = "account"
        };
        HttpCookieCollection m_cookieCollection;
        NuxleusAsyncResult m_asyncResult;
        string m_returnLocation;
        static XNamespace r = "http://nuxleus.com/message/response";
        SelectTask m_task;
        string email;
        string password;
        string sessionid;
        NuxleusAsyncResult m_iTaskResult;

        public void ProcessRequest(HttpContext context)
        {
            //not called
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        static NuxleusHttpAccountLoginServiceOperationHandler()
        {

        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            m_request = context.Request;
            m_response = context.Response;
            m_cookieCollection = context.Response.Cookies;
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);

            string ip = m_request.UserHostAddress.ToString();

            NameValueCollection form = m_request.Form;
            m_returnLocation = form.Get("return_url");

            email = form.Get("email");
            password = form.Get("password");
            sessionid = Guid.NewGuid().ToString();

            m_task = new SelectTask { DomainName = new Domain { Name = "account" }, SelectExpression = String.Format("select name,email,username,password from account where email = '{0}'", email) };
            m_iTaskResult = new NuxleusAsyncResult(cb, extraData);

            m_task.Transaction.OnSuccessfulTransaction += new OnSuccessfulTransaction(Transaction_OnSuccessfulTransaction);
            m_task.Transaction.OnFailedTransaction += new OnFailedTransaction(Transaction_OnFailedTransaction);
            return m_task.BeginInvoke(m_iTaskResult);

        }

        public void EndProcessRequest(IAsyncResult result)
        {
            //PutAttributesTask putAttributesTask = new PutAttributesTask
            //{
            //    DomainName = m_domain,
            //    Item = new Item
            //    {
            //        ItemName = r_userid,
            //        Attribute = attributes
            //    }
            //};

            m_response.Redirect(m_returnLocation);
        }

        public void Transaction_OnSuccessfulTransaction()
        {
            string r_userid = null;
            string r_name = null;
            string r_email = null;
            string r_username = null;
            string r_password = null;

            foreach (Item item in ((SelectResult)m_task.Transaction.Response.Result).Item)
            {
                r_userid = item.ItemName;
                foreach (AwsSdbModel.Attribute attribute in item.Attribute)
                {
                    switch (attribute.Name)
                    {
                        case "name":
                            r_name = attribute.Value;
                            break;
                        case "username":
                            r_username = attribute.Value;
                            break;
                        case "email":
                            r_email = attribute.Value;
                            break;
                        case "password":
                            r_password = attribute.Value;
                            break;
                        default:
                            break;
                    }
                }
            }

            string r_uservalidated = (password == r_password) ? "true" : "false";

            HttpCookie c_sessionid = new HttpCookie("sessionid", sessionid);
            HttpCookie c_name = new HttpCookie("name", r_name);
            HttpCookie c_userid = new HttpCookie("userid", r_userid);
            HttpCookie c_email = new HttpCookie("email", r_email);
            HttpCookie c_username = new HttpCookie("username", r_username);
            HttpCookie c_validated = new HttpCookie("uservalidated", r_uservalidated);
            DateTime expires = DateTime.Now.AddDays(1);

            c_sessionid.Expires = expires;
            c_name.Expires = expires;
            c_userid.Expires = expires;
            c_email.Expires = expires;
            c_username.Expires = expires;
            c_validated.Expires = expires;

            m_cookieCollection.Add(c_sessionid);
            m_cookieCollection.Add(c_userid);
            m_cookieCollection.Add(c_name);
            m_cookieCollection.Add(c_username);
            m_cookieCollection.Add(c_email);
            m_cookieCollection.Add(c_validated);

            //List<AwsSdbModel.Attribute> attributes = new List<AwsSdbModel.Attribute>();
            //attributes.Add(new AwsSdbModel.Attribute
            //{
            //    Name = "session_id",
            //    Value = sessionid,
            //    Replace = true
            //});
            //WriteDebugXmlToOutputStream(((SelectResult)m_task.Transaction.Response.Result).Item);
            m_iTaskResult.CompleteCall();
        }

        public void Transaction_OnFailedTransaction()
        {
            WriteDebugXmlToOutputStream(((SelectResult)m_task.Transaction.Response.Result).Item);
            m_iTaskResult.CompleteCall();
        }

        void WriteDebugXmlToOutputStream(List<Item> items)
        {
            XDocument doc = new XDocument(
                new XElement(r + "message",
                         GenerateXElementItems(items)
                )
            );
            m_response.ContentType = "text/xml";
            doc.Save(m_response.Output);
        }

        IEnumerable<XElement> GenerateXElementItems(List<Item> items)
        {
            foreach (Item item in items)
            {
                yield return new XElement(r + "Account",
                    new XElement(r + "ItemName", item.ItemName),
                         GenerateXElementArray(item.Attribute)
                    );
            }
        }

        IEnumerable<XElement> GenerateXElementArray(List<AwsSdbModel.Attribute> attributes)
        {
            foreach (AwsSdbModel.Attribute attribute in attributes)
            {
                yield return new XElement(r + attribute.Name, attribute.Value);
            }
        }
    }
}
