using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Nuxleus.Utility;
using Nuxleus.Core;

namespace Nuxleus.Web.HttpHandler
{
    public class NuxleusHttpNotFoundHandler : IHttpAsyncHandler
    {
        NuxleusAsyncResult m_asyncResult;
 
        public void ProcessRequest(HttpContext context)
        {
            // Never called
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #region IHttpAsyncHandler Members

        public IAsyncResult BeginProcessRequest (HttpContext context, AsyncCallback cb, object extraData)
        {
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);

            HttpRequest req = context.Request;
            HttpResponse resp = context.Response;

            IList<string> matches = new List<string>();
            string[] tokens = req.Path.Trim('/').ToLower().Split('/');
            string current = Directory.GetCurrentDirectory();
            FindMatch(current, matches, tokens, 0);

            StringBuilder sb = new StringBuilder(String.Format("<html><head/><body><p>Could not find {0}.</p>", HttpUtility.HtmlEncode(req.Path)));

            if (matches.Count != 0)
            {
                sb.Append("<p>We have however found potential match:</p><ul>");
                foreach (string m in matches)
                {
                    sb.Append(String.Format("<li>{0}</li>", HttpUtility.HtmlEncode(m)));
                }

                sb.Append("</ul>");
            }
            sb.Append("<p>Or maybe you'd rather create that resource instead?</p></body></html>");

            resp.Write(sb.ToString());

            // Even if we build a friendly page, this is still a Page Not Found
            // let's not break HTTP
            resp.StatusCode = 404;
            resp.StatusDescription = "Not Found";
            m_asyncResult.CompleteCall();
            return m_asyncResult;
        }

        public void EndProcessRequest (IAsyncResult result)
        {
            
        }

        #endregion

        private void FindMatch (string current, IList<string> matches, string[] tokens, int index)
        {
            if (index == tokens.Length)
            {
                return;
            }

            FileSystemInfo[] directories = (new DirectoryInfo(current)).GetFileSystemInfos();

            string token = tokens[index].ToLower();
            foreach (FileSystemInfo fsi in directories)
            {
                string name = fsi.Name.ToLower();
                if (fsi is DirectoryInfo)
                {
                    DirectoryInfo dir = (DirectoryInfo)fsi;
                    if (name.Contains(token))
                    {
                        if (matches.Contains(dir.Parent.FullName))
                            matches.Remove(dir.Parent.FullName);
                        matches.Add(dir.FullName);
                        FindMatch(dir.FullName, matches, tokens, index + 1);
                    }
                    else if (StringMatching.AreNeighbors(token, name, 2))
                    {
                        if (matches.Contains(dir.Parent.FullName))
                            matches.Remove(dir.Parent.FullName);
                        matches.Add(dir.FullName);
                        FindMatch(dir.FullName, matches, tokens, index + 1);
                    }
                }
                else if (fsi is FileInfo)
                {
                    FileInfo fi = (FileInfo)fsi;
                    if (name.Contains(token))
                    {
                        if (matches.Contains(fi.DirectoryName))
                            matches.Remove(fi.DirectoryName);
                        matches.Add(fi.FullName);
                    }
                    else if (StringMatching.AreNeighbors(token, name, 2))
                    {
                        if (matches.Contains(fi.DirectoryName))
                            matches.Remove(fi.DirectoryName);
                        matches.Add(fi.FullName);
                    }
                }
            }
        }
    }
}