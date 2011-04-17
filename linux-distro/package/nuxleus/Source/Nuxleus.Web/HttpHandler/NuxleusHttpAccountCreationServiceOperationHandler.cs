// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Nuxleus.Core;
using Nuxleus.Extension.Aws.SimpleDb;
using AwsSdbModel = Nuxleus.Extension.Aws.SimpleDb;

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpAccountCreationServiceOperationHandler : IHttpAsyncHandler
    {
        HttpRequest m_request;
        HttpResponse m_response;
        static Encoding m_encoding = new UTF8Encoding();
        HttpCookieCollection m_cookieCollection;
        NuxleusAsyncResult m_asyncResult;
        string m_returnLocation;


        public void ProcessRequest(HttpContext context) {
            //not called
        }

        public bool IsReusable {
            get {
                return false;
            }
        }

        static NuxleusHttpAccountCreationServiceOperationHandler() {

        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {
            m_request = context.Request;
            m_response = context.Response;
            m_cookieCollection = context.Response.Cookies;
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);
            HttpCookie c_sessionid = new HttpCookie("sessionid");
            HttpCookie c_userid = new HttpCookie("userid");
            HttpCookie c_username = new HttpCookie("username");
            HttpCookie c_name = new HttpCookie("name");
            HttpCookie c_validated = new HttpCookie("uservalidated");
            DateTime expires = DateTime.Now.AddDays(1);

            c_sessionid.Expires = expires;
            c_userid.Expires = expires;
            c_username.Expires = expires;
            c_name.Expires = expires;
            c_validated.Expires = expires;

            string ip = m_request.UserHostAddress.ToString();

            NameValueCollection form = m_request.Form;
            m_returnLocation = form.Get("return_url");
            string name = form.Get("name");
            string userid = form.Get("email").GetHashCode().ToString();
            string username = form.Get("username");
            string password = form.Get("password");
            string email = form.Get("email");
            string user_time_zone = form.Get("user[time_zone]");
            string user_url = form.Get("user[url]");
            string user_description = form.Get("user[description]");
            string user_location = form.Get("user[location]");
            string user_lang = form.Get("user[lang]");
            string sessionid = Guid.NewGuid().ToString();

            string myuserid = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(String.Format("{0}:{1}:{2}", name, Guid.NewGuid(), email).ToCharArray())).ToString();

            c_sessionid.Value = sessionid;
            c_userid.Value = userid;
            c_username.Value = username;
            c_name.Value = name;
            c_validated.Value = "true";

            m_cookieCollection.Add(c_sessionid);
            m_cookieCollection.Add(c_userid);
            m_cookieCollection.Add(c_username);
            m_cookieCollection.Add(c_name);
            m_cookieCollection.Add(c_validated);


            List<AwsSdbModel.Attribute> attributes = new List<AwsSdbModel.Attribute>();
            attributes.Add(new AwsSdbModel.Attribute {
                Name = "name",
                Value = name
            });
            attributes.Add(new AwsSdbModel.Attribute {
                Name = "username",
                Value = username
            });
            attributes.Add(new AwsSdbModel.Attribute {
                Name = "password",
                Value = password
            });
            attributes.Add(new AwsSdbModel.Attribute {
                Name = "email",
                Value = email
            });
            attributes.Add(new AwsSdbModel.Attribute {
                Name = "user_time_zone",
                Value = user_time_zone
            });
            attributes.Add(new AwsSdbModel.Attribute {
                Name = "user_url",
                Value = user_url
            });
            attributes.Add(new AwsSdbModel.Attribute {
                Name = "user_description",
                Value = user_description
            });
            attributes.Add(new AwsSdbModel.Attribute {
                Name = "user_location",
                Value = user_location
            });
            attributes.Add(new AwsSdbModel.Attribute {
                Name = "user_lang",
                Value = user_lang
            });
            attributes.Add(new AwsSdbModel.Attribute {
                Name = "session_id",
                Value = sessionid
            });

            PutAttributesTask task = new PutAttributesTask {
                DomainName = new Domain {
                    Name = "account"
                },
                Item = new Item {
                    ItemName = myuserid,
                    Attribute = attributes
                }
            };

            IResponse response = task.Invoke();

            m_asyncResult.CompleteCall();
            return m_asyncResult;
        }

        public void EndProcessRequest(IAsyncResult result) {
            m_response.Redirect(m_returnLocation);
        }
    }
}
