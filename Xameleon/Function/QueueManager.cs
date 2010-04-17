using System;
using System.Web;
using System.Text;
using Nuxleus.Atom;
using Nuxleus.Bucker;
using Nuxleus.Messaging.LLUP;
using Nuxleus.PubSub;

namespace Xameleon.Function {

    public static class QueueManager {

        public static string Push (HttpContext context, string queueName, string message) {
            return Push(queueName, message);
        }

        private static string Push (string queueName, string message) {
            // Notification
            Notification n = new Notification();
            n.Action = "publish";
            n.Expires = DateTime.Now.AddHours(1);
            n.Categories = new Category[2];
            n.Categories[0] = new Category();
            n.Categories[0].Term = message;
            n.Categories[1] = new Category();
            n.Categories[1].Term = "indie";

            // The actual Queue Message to send
            Message pm = new Message();
            pm.Op.Type = OperationType.PushMessage;
            pm.QueueId = queueName;

            Console.WriteLine(queueName);
            Console.WriteLine(n.ToString());
            pm.Payload = Convert.ToBase64String(Notification.Serialize(n));

            // An event handler
            MessageEvent me = new MessageEvent();
            me.Message = pm;
            // Let's see what the response of the server was
            me.MessageReceived += new MessageEventHandler(queue_MessageReceived);

            QueueClientPool.Enqueue(me);

            return "message sent";
        }

        private static void queue_MessageReceived (object sender, MessageStateEventArgs e) {
            MessageEvent me = (MessageEvent)sender;
            Message received = e.Message;
            //Console.WriteLine("RESPONSE: {0}", received.ToString());
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