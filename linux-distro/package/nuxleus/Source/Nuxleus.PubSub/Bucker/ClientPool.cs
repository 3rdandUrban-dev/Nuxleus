//
// clientpool.cs: Client API for the bucker queue system
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Nuxleus.Bucker {
    public class QueueClientPool {
        private static Queue messages = Queue.Synchronized(new Queue());
        private static Queue clients = Queue.Synchronized(new Queue());
        private static bool run = false;
        private static int _threshold = 10;
        private static string _ip = null;
        private static int _port = 0;
        private static int initialPoolSize = 5;

        private static Thread runner = null;

        private QueueClientPool () { }

        public static void Init ( int poolSize, string ip, int port, int threshold ) {
            _ip = ip;
            _port = port;
            initialPoolSize = poolSize;
            _threshold = threshold;

            for (int i=0; i<poolSize; i++) {
                QueueClient qc = new QueueClient(ip, port);
                qc.ConnectionClosed += queue_ConnectionClosed;
                qc.Connect();
                clients.Enqueue(qc);
            }
            run = true;

            runner = new Thread(new ThreadStart(Process));
            runner.Start();
        }

        public static void Shutdown () {
            run = false;

            foreach (QueueClient qc in clients) {
                qc.Disconnect();
            }
            clients.Clear();

            messages.Clear();

            if (runner != null) {
                runner.Join();
                runner = null;
            }
        }

        public static void Enqueue ( MessageEvent me ) {
            if (!run) {
                throw new InvalidOperationException("The pool is not running");
            }

            messages.Enqueue(me);
        }

        private static void queue_ConnectionClosed ( object sender, QueueClientEventArgs e ) {
            QueueClient qc = (QueueClient)sender;
            // we simply don't re-enqueue that client
            if (clients.Count == 0) {
                run = false;
                // here we have to warn that our pool is now empty
            }
        }

        private static void queue_MessageReceived ( object sender, MessageStateEventArgs e ) {
            MessageEvent me = (MessageEvent)sender;
            clients.Enqueue(me.Client);
        }

        public static void Process () {
            while (run) {
                // if we don't have any messages le'ts just pause and try again
                if (messages.Count == 0) {
                    Thread.Sleep(1000);
                    continue;
                }
                for (int i=0; i<clients.Count; i++) {
                    MessageEvent me = null;

                    if (messages.Count > 0) {
                        me = (MessageEvent)messages.Dequeue();
                        if ((me != null) && (me.Message != null)) {
                            QueueClient qc = null;
                            qc = (QueueClient)clients.Dequeue();
                            me.Client = qc;
                            me.MessageReceived += new MessageEventHandler(queue_MessageReceived);
                            QueueClient.AsyncSend(me);
                            QueueClient.AsyncRecv(me);
                        }
                    }
                }

                if (!run) {
                    break;
                }

                if ((messages.Count > initialPoolSize) && (clients.Count < _threshold)) {
                    // we may need to increase the queue clients pool
                    QueueClient qc = new QueueClient(_ip, _port);
                    qc.ConnectionClosed += queue_ConnectionClosed;
                    try {
                        qc.Connect();
                        clients.Enqueue(qc);
                    } catch (SocketException se) {
                        // we should find a way to warn that we can't create more connections
                        // but this could be one of only so let's try give it a chance to recover
                        Thread.Sleep(5000);
                    }
                } else if ((messages.Count == 0) && (clients.Count > initialPoolSize)) {
                    // or reduce it
                    QueueClient qc = (QueueClient)clients.Dequeue();
                    qc.Disconnect();
                }
            }
        }
    }
}