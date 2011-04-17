using System;
using System.Diagnostics;
using System.Threading;

namespace Azure.Toolkit.Azure
{
    public abstract class MessageProcessor<T>
        where T : class, new()
    {
        readonly MessageQueue<T> _queue;

        protected MessageProcessor(string queueName)
        {
            _queue = new MessageQueue<T>(queueName);
            StayAlive = true;
        }

        public bool StayAlive { get; set; }

        public abstract void ProcessMessage(T msg);

        public void ProcessMessages()
        {
            while (StayAlive)
            {
                var msg = _queue.GetMessage();
                if (msg == null)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                try
                {
                    ProcessMessage(msg);
                    _queue.DeleteMessage();
                }
                catch (Exception ex)
                {
                    _queue.DeleteMessage();
                    Trace.TraceError(ex.TraceInformation());
                }
            }
        }
    }
}
