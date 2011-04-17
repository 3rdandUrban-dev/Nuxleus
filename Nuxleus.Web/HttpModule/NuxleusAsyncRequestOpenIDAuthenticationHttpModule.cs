using System;
using System.Web;
using Nuxleus.Core;

namespace Nuxleus.Web.HttpModule {

    public delegate void OpenIDAuthenticationDelegate ();

    public class NuxleusAsyncRequestOpenIDAuthenticationHttpModule : IHttpModule
    {

        public void Init ( System.Web.HttpApplication application ) {
            application.AddOnAuthenticateRequestAsync(
                new BeginEventHandler(BeginOpenIDAuthenticationHandlerExecute),
                new EndEventHandler(EndOpenIDAuthenticationHandlerExecute)
            );
        }

        IAsyncResult BeginOpenIDAuthenticationHandlerExecute ( Object source, EventArgs e, AsyncCallback cb, Object state ) {
            System.Web.HttpApplication app = (System.Web.HttpApplication)source;
            OpenIDAuthenticationDelegate doAuthentication = new OpenIDAuthenticationDelegate(DoAuthentication);
            this.LogInfo("Begin Authentication");
            return doAuthentication.BeginInvoke(cb, doAuthentication);
        }

        void EndOpenIDAuthenticationHandlerExecute ( IAsyncResult ar ) {
            OpenIDAuthenticationDelegate del = (OpenIDAuthenticationDelegate)ar.AsyncState;
            del.EndInvoke(ar);
            this.LogInfo("End Authentication");
        }

        void DoAuthentication () {
            this.LogInfo("Doing Authentication...");
        }

        public void Dispose () { }
    }
}