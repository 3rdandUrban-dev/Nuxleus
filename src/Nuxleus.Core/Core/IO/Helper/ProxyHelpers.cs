// Lifted directly from http://code.google.com/p/fastajaxproxy/source/browse/trunk/App_Code/ProxyHelpers.cs?r=2
// and thus far has made no changes to its original state except for changing the namespace it which it resides.
// If this proves to be a useful utility I plan to spend time optimizing the current state and extending the
// feature set to inherit the multipeer-2-multipeer message dissemintation and distribution capabilities provided
// by the core Nuxleus messaging sub-system.
//
// M. David Peterson 2010-12-17 at 9:45 P.M. Mountain Standard Time

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;

/// <summary>
/// Summary description for ProxyHelpers
/// </summary>
namespace Nuxleus.Core.IO.Helper.ProxyHelpers
{
    public static class HttpHelper
    {

        public static HttpWebRequest CreateScalableHttpWebRequest(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Headers.Add("Accept-Encoding", "gzip");
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.MaximumAutomaticRedirections = 2;
            request.ReadWriteTimeout = 5000;
            request.Timeout = 3000;
            request.Accept = "*/*";
            request.Headers.Add("Accept-Language", "en-US");
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.8.1.6) Gecko/20070725 Firefox/2.0.0.6";

            return request;
        }

        public static void CacheResponse(HttpContext context, int durationInMinutes)
        {
            TimeSpan duration = TimeSpan.FromMinutes(durationInMinutes);

            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.Now.Add(duration));
            context.Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            context.Response.Cache.SetMaxAge(duration);
        }
        public static void DoNotCacheResponse(HttpContext context)
        {
            context.Response.Cache.SetNoServerCaching();
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetMaxAge(TimeSpan.Zero);
            context.Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            context.Response.Cache.SetExpires(DateTime.Now.AddYears(-1));
        }

    }
    public class CachedContent
    {
        public string ContentType;
        public string ContentEncoding;
        public string ContentLength;
        public MemoryStream Content;
    }

    public class AsyncState
    {
        public string Method;
        public HttpContext Context;
        public string Url;
        public int CacheDuration;
        public HttpWebRequest OutboundRequest;
    }

    public class SyncResult : IAsyncResult
    {
        public CachedContent Content;
        public HttpContext Context;

        #region IAsyncResult Members

        object IAsyncResult.AsyncState
        {
            get { return new object(); }
        }

        WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get { return new ManualResetEvent(true); }
        }

        bool IAsyncResult.CompletedSynchronously
        {
            get { return true; }
        }

        bool IAsyncResult.IsCompleted
        {
            get { return true; }
        }

        #endregion
    }

    public static class Log
    {
        private static StreamWriter logStream;
        private static object lockObject = new object();
        public static void WriteLine(string msg)
        {
            if (logStream == null)
            {
                lock (lockObject)
                {
                    if (logStream == null)
                    {
                        logStream = File.AppendText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\log.txt"));
                    }
                }
            }
            logStream.WriteLine(msg);
            //logStream.Flush();
            
            /*
            lock (lockObject)
            {
                File.AppendAllText(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"),
                    msg
                );
            }
            */
        }
    }

    public class TimedLog : IDisposable
    {
        private string _Message;
        private DateTime _Start;

        public TimedLog(string msg)
        {
            this._Message = msg;
            this._Start = DateTime.Now;
        }
        #region IDisposable Members

        public void Dispose()
        {
            DateTime end = DateTime.Now;
            TimeSpan duration = end  - this._Start;
            /*
            Log.WriteLine(this._Start.Minute + ":" + this._Start.Second + ":" + this._Start.Millisecond
                + "/" + end.Minute + ":" + end.Second + ":" + this._Start.Millisecond
                    + "\t" + duration.TotalMilliseconds
                    + "\t" + _Message + "\n");
            */
            Log.WriteLine(duration.TotalMilliseconds + "\t" + _Message + "\n");
        }

        #endregion
    }
}
