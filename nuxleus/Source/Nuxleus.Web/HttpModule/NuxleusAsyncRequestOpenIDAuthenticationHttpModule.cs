﻿using System;
using System.Web;
using System.IO;
using System.Threading;
using System.Text;

namespace Nuxleus.Web.HttpModule {

    public delegate void OpenIDAuthenticationDelegate ();

    public struct NuxleusAsyncRequestOpenIDAuthenticationHttpModule : IHttpModule {

        public void Init ( System.Web.HttpApplication application ) {
            application.AddOnAuthenticateRequestAsync(
                new BeginEventHandler(BeginOpenIDAuthenticationHandlerExecute),
                new EndEventHandler(EndOpenIDAuthenticationHandlerExecute)
            );
        }

        IAsyncResult BeginOpenIDAuthenticationHandlerExecute ( Object source, EventArgs e, AsyncCallback cb, Object state ) {
            System.Web.HttpApplication app = (System.Web.HttpApplication)source;
            OpenIDAuthenticationDelegate doAuthentication = new OpenIDAuthenticationDelegate(DoAuthentication);
            Console.WriteLine("Begin Authentication");
            return doAuthentication.BeginInvoke(cb, doAuthentication);
        }

        void EndOpenIDAuthenticationHandlerExecute ( IAsyncResult ar ) {
            OpenIDAuthenticationDelegate del = (OpenIDAuthenticationDelegate)ar.AsyncState;
            del.EndInvoke(ar);
            Console.WriteLine("End Authentication");
        }

        void DoAuthentication () {
            Console.WriteLine("Doing Authentication...");
        }

        public void Dispose () { }
    }
}