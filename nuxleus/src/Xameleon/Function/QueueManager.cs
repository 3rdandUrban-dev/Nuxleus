using System;
using System.Web;
using Xameleon.Bucker;

namespace Xameleon.Function {
  public class QueueManager {
    public QueueManager() {}

    public static void Push(HttpContext context, string message) {
      QueueClient qs = context.Application["queueclient"] as QueueClient;
      if(qs != null) {
	Push(qs, message);
      }
    }

    public static void Push(QueueClient qs, string message) {
      qs.Send(message);
      MessageState ms = new MessageState();
      ms.Dismiss = true; // we are not interested in storing the response
      qs.AsyncRecv(ms);
    }

    public static void Push(HttpContext context, object message) {
      QueueClient qs = context.Application["queueclient"] as QueueClient;
      if(qs != null) {
	Push(qs, message);
      }
    }

    public static void Push(QueueClient qs, object message) {
      //qs.Send(message);
      //MessageState ms = new MessageState();
      //ms.Dismiss = true; // we are not interested in storing the response
      //qs.AsyncRecv(ms);
    }
  }
}