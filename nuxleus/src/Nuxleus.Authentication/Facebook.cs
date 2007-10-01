using System;
using System.Text;
using System.IO;
using System.Web;
using System.Net;

namespace Nuxleus.Authentication {
  public class Facebook {
    public static void Authenticate(string email, string password) {
      // First we grab the needed cookies to perform the login
      string loginUri = string.Format ("http://www.facebook.com/login.php?api_key={0}&v=1.0", util.ApiKey)
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
      string data = String.Format("version=1.0&auth_token={0}&api_key={1}&email={2}&pass={3}&persistent=1", auth_token, util.ApiKey, HttpUtility.UrlEncode(email), HttpUtility.UrlEncode(password));
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