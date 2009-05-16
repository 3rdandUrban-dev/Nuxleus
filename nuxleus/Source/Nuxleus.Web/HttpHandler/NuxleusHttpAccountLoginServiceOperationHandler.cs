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
using System.Xml;
using System.IO;
using System.Xml.XPath;
using System.Xml.Serialization;

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpAccountLoginServiceOperationHandler : IHttpAsyncHandler
    {
        HttpRequest m_request;
        HttpResponse m_response;
        static Encoding m_encoding = new UTF8Encoding();
        static Domain m_domain = new Domain {
            Name = "account"
        };
        HttpCookieCollection m_cookieCollection;
        NuxleusAsyncResult m_asyncResult;
        string m_returnLocation;
        static XNamespace r = "http://nuxleus.com/message/response";

        public void ProcessRequest(HttpContext context) {
            //not called
        }

        public bool IsReusable {
            get {
                return false;
            }
        }

        static NuxleusHttpAccountLoginServiceOperationHandler() {

        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {
            m_request = context.Request;
            m_response = context.Response;
            m_cookieCollection = context.Response.Cookies;
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);

            string ip = m_request.UserHostAddress.ToString();

            NameValueCollection form = m_request.Form;
            m_returnLocation = form.Get("return_url");

            string username = form.Get("username");
            string password = form.Get("password");
            string sessionid = Guid.NewGuid().ToString();


            SelectTask task = new SelectTask {
                DomainName = new Domain { Name = "account" },
                SelectExpression = String.Format("select name,username,password from account where username = '{0}'", username)
            };

            IResponse response = task.Invoke();


            SelectResult result = (SelectResult)response.Result;

            string r_userid = null;
            string r_name = null;
            string r_username = null;
            string r_password = null;

            foreach (Item item in result.Item) {
                r_userid = item.ItemName;
                foreach (AwsSdbModel.Attribute attribute in item.Attribute) {
                    switch (attribute.Name) {
                        case "name":
                            r_name = attribute.Value;
                            break;
                        case "username":
                            r_username = attribute.Value;
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
            HttpCookie c_username = new HttpCookie("username", r_username);
            HttpCookie c_validated = new HttpCookie("uservalidated", r_uservalidated);
            DateTime expires = DateTime.Now.AddDays(1);

            c_sessionid.Expires = expires;
            c_name.Expires = expires;
            c_userid.Expires = expires;
            c_username.Expires = expires;
            c_validated.Expires = expires;

            m_cookieCollection.Add(c_sessionid);
            m_cookieCollection.Add(c_userid);
            m_cookieCollection.Add(c_name);
            m_cookieCollection.Add(c_username);
            m_cookieCollection.Add(c_validated);


            List<AwsSdbModel.Attribute> attributes = new List<AwsSdbModel.Attribute>();
            attributes.Add(new AwsSdbModel.Attribute {
                Name = "session_id",
                Value = sessionid,
                Replace = true
            });

            PutAttributesTask putAttributesTask = new PutAttributesTask {
                DomainName = m_domain,
                Item = new Item {
                    ItemName = r_userid,
                    Attribute = attributes
                }
            };

            XDocument doc = new XDocument(
                    new XElement(r + "session",
                        new XAttribute("id", sessionid),
                        new XElement(r + "username", r_username),
                        new XElement(r + "userid", r_userid),
                        new XElement(r + "password", r_password),
                        new XElement(r + "uservalidated", r_uservalidated)
                    )
            );

            string xmlOutput = null;

            using (TextWriter writer = new StringWriter()) {
                doc.Save(writer);
                xmlOutput = writer.ToString();
                m_response.Write(xmlOutput);
            }

            m_asyncResult.CompleteCall();
            return m_asyncResult;
        }

        public void EndProcessRequest(IAsyncResult result) {
            m_response.Redirect("/account");
        }
    }
}
