using System;
using System.Web;
using System.IO;
using System.Text;
using Nuxleus.Drawing.Utility;
using Nuxleus.Core;
using System.Collections;
using System.Collections.Generic;

namespace IAct.Web.HttpHandler {

    public class NuxleusHttpAsyncImageResizeOperationHandler : IHttpAsyncHandler
    {

        FileStream m_file;
        static object m_lock = new object();
        Dictionary<string, string> m_statusQueue;

        public void ProcessRequest ( HttpContext context ) {
            //not called
        }

        public bool IsReusable {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest ( HttpContext context, AsyncCallback cb, object extraData ) {

            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            StringBuilder builder = new StringBuilder();
            m_statusQueue = (Dictionary<string, string>)HttpContext.Current.Application["as_statusQueue"];

            string guid = Guid.NewGuid().ToString();

            string stringOutput = String.Format("<request time='{0}' uri='{1}'><job id='{2}' status='1'/></request>", DateTime.Now, request.QueryString["uri"], guid);
            response.Output.WriteLine(stringOutput);

            if (!m_statusQueue.ContainsKey(guid)) {
                m_statusQueue[guid] = stringOutput;
            } else {
                m_statusQueue.Remove(guid);
                m_statusQueue[guid] = stringOutput;
            }

            byte[] output = Encoding.ASCII.GetBytes(stringOutput);
            lock (m_lock) {
                m_file = new FileStream(request.MapPath(String.Format("~/App_Data/process-queue/{0}.xml", guid)), FileMode.Create, FileAccess.Write, FileShare.Write, 1024, true);
                m_file.Seek(0, SeekOrigin.Begin);
                return m_file.BeginWrite(output, 0, output.Length, cb, extraData);
            }
        }

        public void EndProcessRequest ( IAsyncResult result ) {
            m_file.EndWrite(result);
            m_file.Close();
        }
    }
}