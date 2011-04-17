// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.

using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Xml.Linq;
using System.Xml.Xsl;
using Nuxleus.Core;

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpUpcomingDotOrgEventListingServiceOperationHandler : IHttpAsyncHandler
    {
        HttpRequest m_request;
        HttpResponse m_response;
        static Encoding m_encoding = new UTF8Encoding();
        static XslCompiledTransform xslt = new XslCompiledTransform();
        static string m_yahooApiKey = (string)HttpContext.Current.Application["yahooApiKey"];
        XsltArgumentList m_argList = new XsltArgumentList();
        NameValueCollection m_queryStringCollection;
        NuxleusAsyncResult m_asyncResult;

        public void ProcessRequest(HttpContext context)
        {
            //not called
        }

        public bool IsReusable
        {
            get { return false; }
        }

        static NuxleusHttpUpcomingDotOrgEventListingServiceOperationHandler()
        {
            xslt.Load(HttpContext.Current.Server.MapPath("/service/transform/xslt10/upcoming-org.xsl"));
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            m_request = context.Request;
            m_response = context.Response;
            m_queryStringCollection = m_request.QueryString;
            string location = m_queryStringCollection.Get("location");
            string quickDate = m_queryStringCollection.Get("time_scope");
            int categoryId = 1;
            string flags = "PT";
            string sort = "distance-asc";
            string backfill = "further";
            string upcomingEventQueryUri =
                Uri.EscapeUriString(
                    String.Format("http://upcoming.yahooapis.com/services/rest/?method=event.search&api_key={0}&location={1}&quick_date={2}&category_id={3}&flags={4}&sort={5}&backfill={6}",
                    m_yahooApiKey,
                    location,
                    quickDate,
                    categoryId,
                    flags,
                    sort,
                    backfill)
                );

            //XElement upcomingEventsXmlResult = XElement.Load(upcomingEventQueryUri);

            m_argList.AddParam("location", String.Empty, location);
            m_argList.AddParam("format", String.Empty, m_queryStringCollection.Get("format"));
            m_argList.AddParam("current-dateTime", String.Empty, DateTime.UtcNow);
            xslt.Transform(upcomingEventQueryUri, m_argList, m_response.Output);
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);
            m_asyncResult.CompleteCall();
            return m_asyncResult;

        }

        public void EndProcessRequest(IAsyncResult result)
        {


        }

        public void EndGetResponse(IAsyncResult result)
        {


        }
    }
}
