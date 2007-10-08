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
	  // The actual Queue Message to send
	  Message pm = new Message();
	  pm.Op.Type = OperationType.PushMessage;
	  pm.QueueId = queueName;
	  pm.Payload = Convert.ToBase64String(Encoding.ASCII.GetBytes(message));

	  // An event handler
	  MessageEvent me = new MessageEvent();
	  me.Message = pm;
	  // Let's see what the response of the server was
	  me.MessageReceived += new MessageEventHandler(queue_MessageReceived);

	  QueueClientPool.Enqueue(me);

	  return "message sent";
        }

      private static void queue_MessageReceived(object sender, MessageStateEventArgs e){
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