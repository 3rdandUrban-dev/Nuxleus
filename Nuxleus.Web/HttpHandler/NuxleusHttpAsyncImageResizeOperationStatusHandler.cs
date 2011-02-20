using System;
using System.Web;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Nuxleus.Core;
using Nuxleus.MetaData;
using Nuxleus.Drawing.Utility;

namespace IAct.Web.HttpHandler {

    public class NuxleusHttpAsyncImageResizeOperationStatusHandler : IHttpAsyncHandler
    {

        [Message("Status code and related messages.")]
        public enum StatusCode {
            [Message("Job is complete.")]
            DONE,
            [Message("Job is currently being processed.")]
            PENDING,
            [Message("There was an error processing job.")]
            ERROR
        }

        XmlReader m_reader;
        StatusCode m_statusCode;
        String m_message;
        HttpResponse m_response;
        StringBuilder m_builder;
        String m_id;
        Dictionary<string, string> m_statusQueue;

        public void ProcessRequest ( HttpContext context ) {
            //not called
        }

        public bool IsReusable {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest ( HttpContext context, AsyncCallback cb, object extraData ) {

            HttpRequest request = context.Request;
            m_response = context.Response;
            m_statusCode = StatusCode.PENDING;
            m_builder = new StringBuilder();
            m_id = HttpUtility.UrlDecode(request.QueryString["id"]);
            m_message = MessageAttribute.FromMember(m_statusCode);
            NuxleusAsyncResult nuxleusAsyncResult = new NuxleusAsyncResult(cb, extraData);
            m_builder.AppendFormat("<result time='{0}'>", DateTime.Now);
            m_statusQueue = (Dictionary<string, string>)context.Application["as_statusQueue"];

            try {
                m_reader = XmlReader.Create(new StringReader((string)m_statusQueue[m_id]));
                this.LogInfo("Content of status queue: {0}", (string)m_statusQueue[m_id]);
                nuxleusAsyncResult.CompleteCall();
                return nuxleusAsyncResult;
            } catch (Exception e) {
                m_statusCode = StatusCode.ERROR;
                m_message = e.Message;
                nuxleusAsyncResult.CompleteCall();
                return nuxleusAsyncResult;
            }
        }

        public void EndProcessRequest ( IAsyncResult result ) {

            if (m_reader != null) {
                lock (m_reader) {
                    using (m_reader) {
                        do {
                            if (m_reader.IsStartElement()) {
                                switch (m_reader.Name) {
                                    case "job":
                                        int statusCode;
                                        if (int.TryParse(m_reader.GetAttribute("status"), out statusCode)) {
                                            m_statusCode = (StatusCode)statusCode;
                                            m_message = MessageAttribute.FromMember(m_statusCode);
                                            break;
                                        } else {
                                            m_statusCode = StatusCode.ERROR;
                                            m_message = String.Format("Could not read status code {0} from the status queue.", m_id);
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                        } while (m_reader.Read());
                    }
                }
            } else {
                m_statusCode = StatusCode.ERROR;
                m_message = String.Format("Job ID {0} could not be read from.", m_id);
            }

            m_builder.AppendFormat("<job id='{0}' status='{1}'/>", m_id, (int)m_statusCode);
            m_builder.AppendFormat("<message>{0}</message>", m_message);
            m_builder.Append("</result>");
            m_response.Output.WriteLine(m_builder.ToString());
            m_response.ContentType = "text/xml";
        }
    }
}
