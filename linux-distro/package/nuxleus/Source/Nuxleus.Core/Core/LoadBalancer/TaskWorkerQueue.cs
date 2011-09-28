using System;
using System.Collections.Generic;
using System.Threading;

namespace Nuxleus.Core
{

    public class TaskWorkerQueue : IDisposable
    {
        static object m_lock = new object();
        Thread[] workers;
        static Queue<ITask> queue = new Queue<ITask>();

        public TaskWorkerQueue(int workerCount)
        {
            workers = new Thread[workerCount];

            for (int i = 0; i < workerCount; i++)
            {
                (workers[i] = new Thread(Consume)).Start();
            }
        }

        public void EnqueueTask(ITask task)
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
                ITask task;
                lock (m_lock)
                {
                    while (queue.Count == 0)
                        Monitor.Wait(m_lock);
                    task = queue.Dequeue();
                    if (task != null)
                    {
                        task.Invoke();
                    }
                }
                if (task == null)
                {
                    return;
                }
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