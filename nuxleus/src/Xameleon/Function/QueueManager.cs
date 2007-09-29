using System;
using System.Web;
using System.Text;
using Nuxleus.Bucker;

namespace Xameleon.Function
{
    public static class QueueManager
    {

        public static string Push(HttpContext context, string queueName, string message)
        {
            return Push(queueName, message);
        }

        private static string Push(string queueName, string message)
        {
            PushMessage pm = new PushMessage();
            pm.QueueId = queueName;
            pm.Payload = Convert.ToBase64String(Encoding.ASCII.GetBytes(message));

            QueueClientPool.Enqueue(pm);
            return "message sent";
        }

        //public static void Push(HttpContext context, object message) {
        //  QueueClient qs = context.Application["queueclient"] as QueueClient;
        //  if(qs != null) {
        //Push(qs, message);
        //  }
        //}

        //public static void Push(QueueClient qs, object message) {
        //  //qs.Send(message);
        //  //MessageState ms = new MessageState();
        //  //ms.Dismiss = true; // we are not interested in storing the response
        //  //qs.AsyncRecv(ms);
        //}
    }
}