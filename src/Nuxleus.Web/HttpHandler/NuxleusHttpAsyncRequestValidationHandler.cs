using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using Nuxleus.Core;

namespace Nuxleus.Web.HttpHandler {
    public struct NuxleusHttpAsyncRequestValidationHandler : IHttpAsyncHandler {
        NuxleusAsyncResult m_asyncResult;
        int m_pledgeCountDistrict;
        int m_pledgeCountTotal;

        public void ProcessRequest ( HttpContext context ) {

        }

        public bool IsReusable {
            get { return false; }
        }

        #region IHttpAsyncHandler Members

        public IAsyncResult BeginProcessRequest ( HttpContext context, AsyncCallback cb, object extraData ) {
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);

            Queue<string> pledgeQueue = (Queue<string>)context.Application["pledgeQueue"];

            if (pledgeQueue.Count > 0) {
                while (pledgeQueue.Count != 0) {
                    string location = pledgeQueue.Dequeue();
                    if (location == "ca12thdistrict") {
                        m_pledgeCountDistrict++;
                        m_pledgeCountTotal++;
                    } else {
                        m_pledgeCountTotal++;
                    }
                }
            }

            using (XmlWriter writer = XmlWriter.Create(context.Response.Output)) {
                DateTime now = DateTime.Now;

                writer.WriteStartDocument();
                writer.WriteStartElement("message", "http://nuxleus.com/message/response");
                writer.WriteStartElement("session");
                writer.WriteStartAttribute("request-total");
                writer.WriteString(m_pledgeCountTotal.ToString());
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("request-district");
                writer.WriteString(m_pledgeCountDistrict.ToString());
                writer.WriteEndAttribute();
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            m_asyncResult.CompleteCall();

            return m_asyncResult;
        }

        public void EndProcessRequest ( IAsyncResult result ) {

        }

        #endregion
    }
}