//
// clientpool.cs: Client API for the bucker queue system
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Nuxleus.Bucker
{
  public class QueueClientPool {
    // see http://www.mono-project.com/Coding_Guidelines#Locking_and_Threading
    static object lockobj = new object ();

    private static Queue<IMessage> messages = new Queue<IMessage>();
    private static Queue<QueueClient>freeClients = new Queue<QueueClient>();
    private static Queue<QueueClient>busyClients = new Queue<QueueClient>();
    private static bool run = false;
    private static int _threshold = 10;
    private static string _ip = null;
    private static int _port = 0;
    private static int initialPoolSize = 5;

    private static Thread runner = null;

    private QueueClientPool() {}

    public static void Init(int poolSize, string ip, int port, int threshold) {
      _ip = ip;
      _port = port;
      initialPoolSize = poolSize;
      _threshold = threshold;

      for(int i=0;i<poolSize;i++) {
	QueueClient qc = new QueueClient(ip, port);
	qc.Connect();
	freeClients.Enqueue(qc);
      }
      run = true;

      runner = new Thread(new ThreadStart(Process));
      runner.Start();
    }

    public static void Shutdown() {
      run = false;

      foreach(QueueClient qc in freeClients) {
	qc.Disconnect();
      }
      freeClients.Clear();

      messages.Clear();
      
      // wee allow busy clientst o end their task
      for(int i=0;i<busyClients.Count;i++) {
	  QueueClient qc = busyClients.Peek();
	  if(qc.IsCompleted == true){
	    qc = busyClients.Dequeue();
	    qc.Disconnect();
	  }
      }

      if(runner != null) {
	runner.Join();
	runner = null;
      }
    }

    public static void Enqueue(IMessage message) {
      if(!run) {
	throw new InvalidOperationException("The pool is not running");
      }

      lock(lockobj) { // should really lock for a very small time
	messages.Enqueue(message);
      }
    }

    public static void Process() {
      while(run) {
	for(int i=0;i<freeClients.Count;i++){
	  IMessage m = null;

	  lock(lockobj) {
	    if(messages.Count > 0) {
	      m = messages.Dequeue();
	    }
	  }

	  if(m != null) {
	    QueueClient qc = freeClients.Dequeue();
	    qc.Send(m);
	    MessageState ms = new MessageState();
            ms.Dismiss = true;
	    qc.AsyncRecv(ms);
	    busyClients.Enqueue(qc);
	  } 
	}

	if(!run) {
	  break;
	}

	// avoid the thread to run like crazy when there is no messages to process
	Thread.Sleep(100);

	if(messages.Count > _threshold) {
	  Init(2, _ip, _port, _threshold);
	} 
	
	for(int i=0;i<busyClients.Count;i++) {
	  QueueClient qc = busyClients.Peek();
	  if(qc.IsCompleted == true){
	    qc = busyClients.Dequeue();
	    freeClients.Enqueue(qc);
	  }
	}
      }
    }
  }
}