using System;
using System.Collections.Generic;
using System.Threading;
using Nuxleus.Asynchronous;

namespace Nuxleus.Core
{

    public class IEnumerableTWorkerQueue<T> : IDisposable
    {
        static object m_lock = new object();
        Thread[] workers;
        static Queue<IEnumerable<T>> queue = new Queue<IEnumerable<T>>();

        public IEnumerableTWorkerQueue(int workerCount)
        {
            workers = new Thread[workerCount];

            for (int i = 0; i < workerCount; i++)
            {
                (workers[i] = new Thread(Consume)).Start();
            }
        }

        public void EnqueueTask(IEnumerable<T> task)
        {
            lock (m_lock)
            {
                queue.Enqueue(task);
                Monitor.PulseAll(m_lock);
            }
        }

        void Consume()
        {
            while (true)
            {
                IEnumerable<T> task;
                lock (m_lock)
                {
                    while (queue.Count == 0)
                        Monitor.Wait(m_lock);
                    task = queue.Dequeue();
                }
                if (task == null)
                {
                    return;
                }
                // Temp hack until I decide the best way to handle this from an extension method perspective.
                ((IEnumerable<IAsync>)task).Execute();
            }
        }

        public void Dispose()
        {
            foreach (Thread worker in workers)
            {
                EnqueueTask(null);
            }
            foreach (Thread worker in workers)
            {
                worker.Join();
            }
        }
    }
}