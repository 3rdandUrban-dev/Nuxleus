using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Nuxleus.Cryptography;

namespace Nuxleus.Extension.Facebook
{
    public static class Facebook
    {
        public static string FACEBOOK_API_URL = "https://api.facebook.com/restserver.php";

        private static string CreateSignature(string secret, SortedDictionary<string, object> parameters)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, object> kvp in parameters)
            {
                string value = null;

                if (kvp.Value is Array)
                {
                    StringBuilder v = new StringBuilder();

                    Array values = (Array)kvp.Value;

                    for (int i = 0; i < values.Length; i++)
                    {
                        v.Append(values.GetValue(i).ToString());
                        v.Append(",");
                    }
                    // Remove any trailing comma
                    v.Remove(v.Length - 1, 1);
                    value = v.ToString();

                }
                else
                {
                    value = kvp.Value.ToString();
                }

                sb.Append(String.Format("{0}={1}", kvp.Key, value));
            }

            if (secret != null)
                sb.Append(secret);

            string hash = Hash.MD5(sb.ToString());

            return hash;
        }

        private static string BuildQueryString(SortedDictionary<string, object> parameters)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, object> kvp in parameters)
            {
                string value = null;

                if (kvp.Value is Array)
                {
                    StringBuilder v = new StringBuilder();

                    Array values = (Array)kvp.Value;

                    for (int i = 0; i < values.Length; i++)
                    {
                        v.Append(values.GetValue(i).ToString());
                        v.Append(",");
                    }
                    // Remove any trailing comma
                    v.Remove(v.Length - 1, 1);
                    value = v.ToString();

                }
                else
                {
                    value = kvp.Value.ToString();
                }

                sb.Append(String.Format("{0}={1}&", HttpUtility.UrlEncode(kvp.Key),
                            HttpUtility.UrlEncode(value)));
            }

            //remove trailing &
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public static string Call(string secret, SortedDictionary<string, object> parameters)
        {
            string sig = CreateSignature(secret, parameters);
            parameters.Add("sig", sig);
            string qs = BuildQueryString(parameters);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(FACEBOOK_API_URL);
            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (compatible)";
            req.ContentType = "application/x-www-form-urlencoded";

            byte[] bytes = Encoding.UTF8.GetBytes(qs);
            req.ContentLength = bytes.Length;
            Stream body = req.GetRequestStream();
            body.Write(bytes, 0, bytes.Length);
            body.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            Stream stream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string content = reader.ReadToEnd();

            resp.Close();

            return content;
        }

        public static string Call(SortedDictionary<string, object> parameters)
        {
            return Call(null, parameters);
        }
    }
}
