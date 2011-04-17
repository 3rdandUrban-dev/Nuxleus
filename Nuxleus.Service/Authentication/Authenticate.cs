using System;
using System.Collections;
using System.Web;
using System.Web.Services;

namespace Nuxleus.Service
{
    /// <summary>
    /// Summary description for Authenticate service
    /// </summary>
    [WebService(Namespace = "http://nuxleus.com/service")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class Authenticate : WebService
    {

        Hashtable _NonceSessionIDHashtable = new Hashtable();

        public bool CheckAuthentication(string nonce)
        {
            return checkAuthentication(nonce, "SESSIONID");
        }

        public bool CheckAuthentication(string nonce, string sessionID)
        {
            return checkAuthentication(nonce, sessionID);
        }

        [WebMethod(EnableSession = true)]
        private bool checkAuthentication(string nonce, string sessionID)
        {
            if ((string)_NonceSessionIDHashtable[HttpContext.Current.Request.Cookies[sessionID]] == nonce)
                return true;
            else
                return false;
        }

        private string GenerateNonce()
        {
            return new Guid().ToString();
        }
    }
}

