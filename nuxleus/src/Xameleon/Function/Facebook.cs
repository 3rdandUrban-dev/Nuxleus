using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Text;
using Nuxleus.Bucker;
using Nuxleus.Authentication;
using Nuxleus.Extension;

namespace Xameleon.Function {
  public static class Facebook {

    /// <summary>
    /// First function to call in order to get the authentication token used to
    /// sign the user in and open his session.
    /// </summary>
    public static string GetAuthenticationToken(HttpContext context) {
      //Facebook application API key, public, and global to all users
      // Where should this be pulled from? The context?
      // Or should this be passed as parameter of the function?
      string apiKey = ""; 
      
      SortedDictionary<string, object> parameters = new SortedDictionary<string, object>();
      parameters.Add("api_key", apiKey);
      parameters.Add("method", "auth.createToken");
      parameters.Add("v", "1.0");

      // This performs the actual all to the service
      return Nuxleus.Extension.Facebook.Call(parameters);
    }

    /// <summary>
    /// Called to sign the user in and activate the acknowledge the authentication token.
    /// This must be called before OpenUserSession.
    /// </summary>
    public static void SignInUser(HttpContext context) {
      // Again where these should come from?
      // Or rather, how do get we access to them?
      // Should we call memcached?
      string authToken = "";
      string apiKey = ""; //Facebook application API key, public, and global to all users
      string email = "";
      string password = "";
      
      Nuxleus.Authentication.Facebook.Authenticate(email, password, authToken, apiKey);
    }

    public static string OpenUserSession(HttpContext context) {
      string authToken = "";
      string secret = ""; //Facebook application secret key global to all users
      string apiKey = ""; //Facebook application API key, public, and global to all users
      
      SortedDictionary<string, object> parameters = new SortedDictionary<string, object>();
      parameters.Add("api_key", apiKey);
      parameters.Add("auth_token", authToken);
      parameters.Add("method", "auth.getSession");
      parameters.Add("v", "1.0");

      return Nuxleus.Extension.Facebook.Call(secret, parameters);
    }

    public static string GetFriends(HttpContext context) {
      string authToken = "";
      string secret = ""; //Facebook application secret key global to all users
      string apiKey = ""; //Facebook application API key, public, and global to all users
      string sessionKey = ""; //returned by the OpenUserSession call. It is per user.

      SortedDictionary<string, object> parameters = new SortedDictionary<string, object>();

      parameters.Add("api_key", apiKey);
      parameters.Add("call_id", DateTime.Now.Ticks);
      parameters.Add("method", "friends.get");
      parameters.Add("session_key", sessionKey);
      parameters.Add("v", "1.0");

      return Nuxleus.Extension.Facebook.Call(secret, parameters);
    }
  }
}