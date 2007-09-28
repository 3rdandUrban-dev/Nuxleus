using System;
using System.Web;
using System.IO;
using System.Threading;
using System.Text;
using Xameleon.Bucker;
using Xameleon.Llup;
using Xameleon.Configuration;


namespace Xameleon.HttpModule
{
    public class AsyncQueueServerModule : IHttpModule
    {
        public void Init(System.Web.HttpApplication application)
        {
            application.AddOnPreRequestHandlerExecuteAsync(
                new BeginEventHandler(BeginPreRequestHandlerExecute),
                new EndEventHandler(EndPreRequestHandlerExecute)
            );
        }

        IAsyncResult BeginPreRequestHandlerExecute(Object source, EventArgs e, AsyncCallback cb, Object state)
        {
            QueueClient qs = (QueueClient)HttpContext.Current.Application["queueclient"];
            Notification n = new Notification();
            n.Published = DateTime.UtcNow;

            PushMessage pm = new PushMessage();
            pm.QueueId = "event";
            pm.Payload = Notification.Xml(n);

            HttpResponse Response = HttpContext.Current.Response;
            Response.Write(pm.Serialize());

            TransmitQueueMessage t = new TransmitQueueMessage(QueueClient.Transmit);
            return t.BeginInvoke(new MessageInfo(qs, pm), null, null);
        }

        void EndPreRequestHandlerExecute(IAsyncResult ar)
        {
            TransmitQueueMessage t = (TransmitQueueMessage)ar.AsyncState;
            t.EndInvoke(ar);
        }

        public void Dispose() { }
    }
}