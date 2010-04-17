//
// AtomFeed.cs: Subscriber that creates/fetches an atom entry based on the notification add it to an atom feed
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 
using System;
using System.Threading;

namespace Nuxleus.Messaging.LLUP {
    public class AtomFeedSubscriber : ISubscriber {
        private SubscriberHandler handler = null;
        private static Thread runner = null;
        private static bool running = false;

        public AtomFeedSubscriber () { }

        public SubscriberHandler Handler {
            get { return handler; }
            set { handler = value; }
        }

        public void Start () {
            running = true;
            runner = new Thread(new ThreadStart(this.ProcessNotification));
            runner.Start();
        }

        public void Stop () {
            running = false;
            runner.Join();
            runner = null;
        }

        private void ProcessNotification () {
            Notification n = null;

            while (running) {
                // let's not clog the system
                Thread.Sleep(1000);

                if (handler.Pending.Count > 0) {
                    n = (Notification)handler.Pending.Dequeue();
                    if (n != null) {
                        Console.WriteLine(String.Format("Processing {0}", n.Id));
                        // .. do something here ...
                    }
                }
            }
        }
    }
}