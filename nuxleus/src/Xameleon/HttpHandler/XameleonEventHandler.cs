using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Threading;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.SessionState;
using System.Collections;
using System.Text;
using System.Xml;
using Xameleon.Configuration;
using System.Collections.Generic;
using Xameleon.Cryptography;

namespace Xameleon.HttpHandler
{
    public class XameleonEventHandler : IHttpHandler
    {
      public void ProcessRequest(HttpContext context) {
      }

     
      public bool IsReusable {
        get { return false; }
      }

    }
}
