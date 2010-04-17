using System;
using System.Collections.Generic;
using System.Web;

namespace Xameleon.Function
{

  public static class Facebook {

    /// <summary>
    /// First function to call in order to get the authentication token used to
    /// sign the user in and open his session.
    /// </summary>
    public static string GetAuthenticationToken(HttpContext context) {
      //Facebook application API key, public, and global to all users
      // Where should this be pulled from? The context?
      // Or should this be passed as parameter of the function?
      string apiKey = String.Empty; 
      string secret = String.Empty; //Facebook application secret key global to all users
      
      SortedDictionary<string, object> parameters = new SortedDictionary<string, object>();
      parameters.Add("api_key", apiKey);
      parameters.Add("method", "auth.createToken");
      parameters.Add("v", "1.0");

      // This performs the actual all to the service
      return Nuxleus.Extension.Facebook.Facebook.Call(secret, parameters);
    }

    /// <summary>
    /// Called to sign the user in and activate the acknowledge the authentication token.
    /// This must be called before OpenUserSession.
    /// </summary>
    public static void SignInUser(HttpContext context) {
      // Again where these should come from?
      // Or rather, how do get we access to them?
      // Should we call memcached?
      string authToken = String.Empty;
      string apiKey = String.Empty; //Facebook application API key, public, and global to all users
      string email = String.Empty;
      string password = String.Empty;
      
      Nuxleus.Authentication.Facebook.Authenticate(email, password, authToken, apiKey);
    }

    public static string OpenUserSession(HttpContext context) {
      string authToken = String.Empty;
      string secret = String.Empty; //Facebook application secret key global to all users
      string apiKey = String.Empty; //Facebook application API key, public, and global to all users
      
      SortedDictionary<string, object> parameters = new SortedDictionary<string, object>();
      parameters.Add("api_key", apiKey);
      parameters.Add("auth_token", authToken);
      parameters.Add("method", "auth.getSession");
      parameters.Add("v", "1.0");

      return Nuxleus.Extension.Facebook.Facebook.Call(secret, parameters);
    }

    public static string GetFriends(HttpContext context) {
      string authToken = String.Empty;
      string secret = String.Empty; //Facebook application secret key global to all users
      string apiKey = String.Empty; //Facebook application API key, public, and global to all users
      string sessionKey = String.Empty; //returned by the OpenUserSession call. It is per user.

      SortedDictionary<string, object> parameters = new SortedDictionary<string, object>();

      parameters.Add("api_key", apiKey);
      parameters.Add("call_id", DateTime.Now.Ticks);
      parameters.Add("method", "friends.get");
      parameters.Add("session_key", sessionKey);
      parameters.Add("v", "1.0");

      return Nuxleus.Extension.Facebook.Facebook.Call(secret, parameters);
    }

    public static string Notify(HttpContext context) {
      string authToken = String.Empty;
      string secret = String.Empty; //Facebook application secret key global to all users
      string apiKey = String.Empty; //Facebook application API key, public, and global to all users
      string sessionKey = String.Empty; //returned by the OpenUserSession call. It is per user.
      string[] uids = null; // uuids of destination
      string message = String.Empty;

      SortedDictionary<string, object> parameters = new SortedDictionary<string, object>();

      parameters.Add("api_key", apiKey);
      parameters.Add("call_id", DateTime.Now.Ticks);
      parameters.Add("method", "notifications.send");
      parameters.Add("notification", message);
      parameters.Add("session_key", sessionKey);
      parameters.Add("to_ids", uids);
      parameters.Add("v", "1.0");

      return Nuxleus.Extension.Facebook.Facebook.Call(secret, parameters);
    }
  }
}