using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Nuxleus.Authentication
{
    public static class Facebook
    {
        /// <summary>
        /// Authenticate a user to Facebook allowing for  usage of the Facebook API.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The authentication uses SSL to encrypt the communication with the 
        ///     Facebook servers.
        ///   </para>
        /// </remarks>
        /// <param name="email">User email address registered on Facebook</param>
        /// <param name="password">Clear text password for the user.</param>
        /// <param name="authToken">Token returned by the call at auth.createToken </param>
        /// <param name="apiKey">Public key to the application the user wishes to access</param>
        /// <returns>Returns nothing.</returns>
        public static void Authenticate(string email, string password, string authToken, string apiKey)
        {
            // First we grab the needed cookies to perform the login
            string loginUri = string.Format("http://www.facebook.com/login.php?api_key={0}&v=1.0&auth_token={1}", apiKey, authToken);
            HttpWebRequest r0 = (HttpWebRequest)WebRequest.Create(loginUri);
            r0.UserAgent = "Mozilla/5.0 (compatible)"; // we have to pretend what we're not otherwise Facebook is not happy 
            r0.CookieContainer = new CookieContainer();
            HttpWebResponse s0 = (HttpWebResponse)r0.GetResponse();

            // Next we perform the actual signing
            HttpWebRequest r1 = (HttpWebRequest)WebRequest.Create("https://login.facebook.com/login.php");
            r1.Method = "POST";
            r1.UserAgent = "Mozilla/5.0 (compatible)";
            r1.ContentType = "application/x-www-form-urlencoded";
            r1.CookieContainer = new CookieContainer();
            r1.CookieContainer.Add(s0.Cookies);
            string data = String.Format("version=1.0&auth_token={0}&api_key={1}&email={2}&pass={3}&persistent=1", authToken, apiKey, HttpUtility.UrlEncode(email), HttpUtility.UrlEncode(password));
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            r1.ContentLength = bytes.Length;
            Stream body = r1.GetRequestStream();
            body.Write(bytes, 0, bytes.Length);
            body.Close();
            HttpWebResponse s1 = (HttpWebResponse)r1.GetResponse();

            s0.Close();
            s1.Close();
        }
    }
}