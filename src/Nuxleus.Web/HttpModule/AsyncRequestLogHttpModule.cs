//PLEASE NOTE: Taken directly from the sample provided @ http://msdn.microsoft.com/msdnmag/issues/07/03/WickedCode/default.aspx?loc=&fig=true#fig6
using System;
using System.Web;
using System.IO;
using System.Threading;
using System.Text;

namespace Nuxleus.Web.HttpModule {

    public class AsyncRequestLogHttpModule : IHttpModule
    {
        FileStream _file;
        static long _position = 0;
        static object _lock = new object();

        public void Init (System.Web.HttpApplication application) {
            application.AddOnPreRequestHandlerExecuteAsync(
                new BeginEventHandler(BeginPreRequestHandlerExecute),
                new EndEventHandler(EndPreRequestHandlerExecute)
            );
        }

        IAsyncResult BeginPreRequestHandlerExecute (Object source, EventArgs e, AsyncCallback cb, Object state) {
            System.Web.HttpApplication app = (System.Web.HttpApplication)source;
            DateTime time = DateTime.Now;

            string line = String.Format(
                "{0,10:d}    {1,11:T}    {2, 32}   {3}\r\n",
                time, time,
                app.User.Identity.IsAuthenticated ?
                app.User.Identity.Name :
                app.Request.UserHostAddress,
                app.Request.Url);

            byte[] output = Encoding.ASCII.GetBytes(line);

            lock (_lock) {
                _file = new FileStream(
                    HttpContext.Current.Server.MapPath("~/App_Data/RequestLog.txt"),
                    FileMode.OpenOrCreate, FileAccess.Write,
                    FileShare.Write, 1024, true);

                _file.Seek(_position, SeekOrigin.Begin);
                _position += output.Length;
                return _file.BeginWrite(output, 0, output.Length, cb, state);
            }
        }

        void EndPreRequestHandlerExecute (IAsyncResult ar) {
            _file.EndWrite(ar);
            _file.Close();
        }

        public void Dispose () {
            
        }
    }
}