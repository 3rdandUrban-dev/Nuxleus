using System;
using System.Web;
using Xameleon.Bucker;

namespace Xameleon.Function
{
    public static class QueueManager
    {

        public static string Push(HttpContext context, string queueName, string message)
        {
            QueueClient qs = context.Application["queueclient"] as QueueClient;
            if (qs != null)
            {
                return Push(qs, queueName, message);
            }
            else
                return "no queue client available";
        }

        private static string Push(QueueClient qs, string queueName, string message)
        {
            qs.Send(message);
            MessageState ms = new MessageState();
            ms.Dismiss = true; // we are not interested in storing the response
            qs.AsyncRecv(ms);
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