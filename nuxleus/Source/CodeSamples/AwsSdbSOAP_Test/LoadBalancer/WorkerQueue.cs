using System;
using System.Threading;
using System.Collections.Generic;
using Nuxleus.Asynchronous;

namespace AwsSdbSOAP_Test {

    public class WorkerQueue : IDisposable {
        object m_lock = new object();
        Thread[] workers;
        Queue<IEnumerable<IAsync>> queue = new Queue<IEnumerable<IAsync>>();

        public WorkerQueue(int workerCount) {
            workers = new Thread[workerCount];

            for (int i = 0; i < workerCount; i++) {
                (workers[i] = new Thread(Consume)).Start();
            }
        }

        public void EnqueueTask(IEnumerable<IAsync> task) {
            lock (m_lock) {
                queue.Enqueue(task);
                Monitor.PulseAll(m_lock);
            }
        }

        void Consume() {
            while (true) {
                IEnumerable<IAsync> task;
                lock (m_lock) {
                    while (queue.Count == 0)
                        Monitor.Wait(m_lock);
                    task = queue.Dequeue();
                }
                if (task == null) {
                    return;
                }
                Console.WriteLine("Thread is background: {0}, Thread ID: {1}, Thread is managed: {2}", Thread.CurrentThread.IsBackground, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
                task.ExecuteAndWait();
            }
        }

        public void Dispose() {
            foreach (Thread worker in workers) {
                EnqueueTask(null);
            }
            foreach (Thread worker in workers) {
                worker.Join();
            }
        }
    }
}