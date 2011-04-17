using System;
using System.Collections;
using System.Text;
using System.Web;
using System.Xml;
using Nuxleus.Core;
using Nuxleus.Extension.Aws;

namespace Nuxleus.Web.HttpHandler {

    public class NuxleusHttpAsyncQueryEntityHandler : IHttpAsyncHandler
    {

        HttpRequest m_request;
        HttpResponse m_response;
        HttpCookieCollection m_cookieCollection;
        NuxleusAsyncResult m_asyncResult;
        Message m_responseMessage;

        public void ProcessRequest ( HttpContext context ) {

        }

        public bool IsReusable {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest ( HttpContext context, AsyncCallback cb, object extraData ) {
            m_request = context.Request;
            m_response = context.Response;
            m_cookieCollection = context.Response.Cookies;
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);

            int maxResults;

            if (!int.TryParse(m_request.QueryString["maxResults"], out maxResults)) {
                maxResults = 5;
            }

            string awsAccessKey = System.Environment.GetEnvironmentVariable("SDB_ACCESS_KEY");
            string awsSecretKey = System.Environment.GetEnvironmentVariable("SDB_SECRET_KEY");
            //Sdb sdb = new Sdb(new HttpQueryConnection(awsAccessKey, awsSecretKey, "http://sdb.amazonaws.com"));

            //Domain domain = sdb.GetDomain("geonames");

            //QueryResponse response = domain.Query(String.Format("['{0}'starts-with'{1}'OR'{0}'='{1}']", m_request.QueryString["name"], m_request.QueryString["value"]));
            //IEnumerator items = response.Items().GetEnumerator();

            //StringBuilder builder = new StringBuilder();
            //int i = 0;
            //while (items.MoveNext()) {
            //    if (i < maxResults) {
            //        builder.Append(((GetAttributesResponse)((Item)items.Current).GetAttributes()).RawXml.Substring(21));
            //    }
            //    i++;
            //}

            //try {
            //    m_responseMessage = new Message("query", ResponseType.QUERY_RESPONSE);
            //} catch (Exception e) {
            //    m_responseMessage = new Message(e.Message, ErrorType.INVALID_REQUEST, ResponseType.ERROR);
            //}

            //m_responseMessage.WriteResponseMessage(XmlWriter.Create(m_response.Output), builder.ToString(), m_asyncResult);

            return m_asyncResult;
        }

        public void EndProcessRequest ( IAsyncResult result ) {
            m_response.ContentType = "text/xml";
        }
    }
}