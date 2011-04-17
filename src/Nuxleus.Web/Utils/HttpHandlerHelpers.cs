using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Nuxleus.Web
{
    public static class Utility
    {
        public static Dictionary<string, string> GetCookieValues(HttpCookieCollection cookieCollection, out bool success, params string[] cookieNames)
        {
            Dictionary<string, string> cookies = new Dictionary<string, string>();
            bool successful = true;
            foreach (string cookieName in cookieNames)
            {
                try
                {
                    cookies.Add(cookieName, cookieCollection[cookieName].Value);
                }
                catch
                {
                    successful = false;
                }
            }
            success = successful;
            return cookies;
        }
    }
}
