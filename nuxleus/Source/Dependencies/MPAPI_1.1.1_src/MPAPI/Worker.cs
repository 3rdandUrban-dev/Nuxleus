/*****************************************************************
 * MPAPI - Message Passing API
 * A framework for writing parallel and distributed applications
 * 
 * Author   : Frank Thomsen
 * Web      : http://sector0.dk
 * Contact  : mpapi@sector0.dk
 * License  : New BSD licence
 * 
 * Copyright (c) 2008, Frank Thomsen
 * 
 * Feel free to contact me with bugs and ideas.
 *****************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MPAPI
{
    public abstract class Worker : IWorker
    {
        private const int MessageCountWarningLimit = 10000; //the worker will give a warning if the number of waiting messages exceed this limit.

        #region Static
        private static Dictionary<int, Worker> _threadIdToWorkerMap = new Dictionary<int, Worker>();

        private static void RegisterWorker(Worker worker)
        {
            lock (_threadIdToWorkerMap)
            {
                _threadIdToWorkerMap.Add(Thread.CurrentThread.ManagedThreadId, worker);
            }
        }

        private static void UnregisterWorker()
        {
            lock (_threadIdToWorkerMap)
            {
                _threadIdToWorkerMap.Remove(Thread.CurrentThread.ManagedThreadId);
            }
        }

        public static IWorker Current
        {
            get
            {
                lock (_threadIdToWorkerMap)
                {
                    if (_threadIdToWorkerMap.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                        return _threadIdToWorkerMap[Thread.CurrentThread.ManagedThreadId];
                    return null;
                }
            }
        }
        #endregion

        private Node _node;
        private ushort _id;
        private LinkedList<Message> _messageQueue = new LinkedList<Message>();
        private ManualResetEvent _messageQueueWaitHandle = new ManualResetEvent(false);
        private WorkerAddress _monitoringWorker = null;
        private List<int> _messageFilters = new List<int>();

        #region Properties
        public IWorkerNode Node
        {
            get { return _node; }
        }

        public ushort Id
        {
            get { return _id; }
            set { _id = value; }
        }
        #endregion

        public abstract void Main();

        internal void SetNode(Node node) { _node = node; }

        internal void SetMonitoringWorker(WorkerAddress monitoringWorker)
        {
            _monitoringWorker = monitoringWorker;
        }

        /// <summary>
        /// This method is called by the framework
        /// </summary>
        internal void _Main()
        {
            RegisterWorker(this);
            try
            {
                Main();
                if (_monitoringWorker != null)
                {
                    Message msg = new Message(MessageLevel.System, _monitoringWorker, new WorkerAddress(_node.GetId(), _id), SystemMessages.WorkerTerminated, null);
                    _node.QueueMessage(msg);
                }
                OnWorkerTerminated();
            }
            catch (Exception e)
            {
                WorkerAddress workerAddress = new WorkerAddress(_node.GetId(), _id);
                Log.Error("Worker {0} crashed with exception {1}", workerAddress, e.Message);
                if (_monitoringWorker != null)
                {
                    Message msg = new Message(MessageLevel.System, _monitoringWorker, workerAddress, SystemMessages.WorkerTerminatedAbnormally, e);
                    _node.QueueMessage(msg);
                }
                OnWorkerTerminatedAbnormally(e);
            }
            _node.WorkerTerminated(_id);
            UnregisterWorker();
        }

        /// <summary>
        /// This method is called right before a worker terminates normally.
        /// </summary>
        public virtual void OnWorkerTerminated()
        {
        }

        /// <summary>
        /// This method is called right before a worker terminates
        /// abnormally due to an exception.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnWorkerTerminatedAbnormally(Exception e)
        {
        }

        /// <summary>
        /// This method is called whenever a remote node registers on the registration server.
        /// </summary>
        /// <param name="nodeId"></param>
        public virtual void OnRemoteNodeRegistered(ushort nodeId)
        {
        }

        /// <summary>
        /// This method is called whenever a remote node unregisters from the registration server.
        /// </summary>
        /// <param name="nodeId"></param>
        public virtual void OnRemoteNodeUnregistered(ushort nodeId)
        {
        }

        internal void PutMessage(MessageLevel messageLevel, ushort receiverNodeId, ushort receiverWorkerId, ushort senderNodeId, ushort senderWorkerId, int messageType, object content)
        {
            lock (_messageFilters)
            {
                if (_messageFilters.Count > 0 && !_messageFilters.Contains(messageType))
                    return;
            }
            lock (_messageQueue)
            {
                _messageQueue.AddLast(new Message(messageLevel, new WorkerAddress(receiverNodeId, receiverWorkerId), new WorkerAddress(senderNodeId, senderWorkerId), messageType, content));
                //if (_messageQueue.Count >= MessageCountWarningLimit)
                //    Log.Warning("Mail box contains {0} unread messages. WorkerId:{1}", _messageQueue.Count, _id);
                _messageQueueWaitHandle.Set();
            }
        }

        #region Message passing primitives and other control primitives
        /// <summary>
        /// Sets the message types that this worker is interested in.
        /// </summary>
        /// <param name="filters"></param>
        public void SetMessageFilter(params int[] filters)
        {
            lock (_messageFilters)
            {
                _messageFilters.Clear();
                _messageFilters.AddRange(filters);
            }
        }

        /// <summary>
        /// Makes the thread running this worker sleep for a period of time.
        /// </summary>
        /// <param name="ms">The number of milliseconds to sleep.</param>
        public void Sleep(int ms)
        {
            Thread.Sleep(ms);
        }

        /// <summary>
        /// Enables this worker to receive system messages when the monitoree terminates, either
        /// normally or abnormally.
        /// </summary>
        /// <param name="monitoree"></param>
        public void Monitor(WorkerAddress monitoree)
        {
            _node._Monitor(new WorkerAddress(_node.GetId(), _id), monitoree);
        }

        /// <summary>
        /// Spawns a new worker locally.
        /// </summary>
        /// <typeparam name="TWorkerType">The type of worker to spawn. Must inherit from Worker.</typeparam>
        /// <returns>The address of the new worker if successfull, otherwise null.</returns>
        public WorkerAddress Spawn<TWorkerType>() where TWorkerType : Worker
        {
            ushort workerId;
            WorkerAddress workerAddress = null;
            if (_node.Spawn(typeof(TWorkerType), out workerId))
                workerAddress = new WorkerAddress(_node.GetId(), workerId);
            return workerAddress;
        }

        /// <summary>
        /// Spawns a new worker at the specified node.
        /// </summary>
        /// <typeparam name="TWorkerType">The type of worker to spawn. Must inherit from Worker.</typeparam>
        /// <param name="nodeId">Id of the node to spawn the worker at.</param>
        /// <returns>The address of the new worker if successfull, otherwise null.</returns>
        public WorkerAddress Spawn<TWorkerType>(ushort nodeId) where TWorkerType : Worker
        {
            ushort workerId;
            WorkerAddress workerAddress = null;
            if (_node.Spawn(typeof(TWorkerType), nodeId, out workerId))
                workerAddress = new WorkerAddress(nodeId, workerId);
            return workerAddress;
        }

        /// <summary>
        /// Spawns a new worker at the specified node.
        /// </summary>
        /// <param name="workerTypeName">Fully qualified name of worker type. Must inherit from Worker.</param>
        /// <param name="nodeId">Id of the node to spawn the worker at.</param>
        /// <returns>The address of the new worker if successfull, otherwise null.</returns>
        public WorkerAddress Spawn(string workerTypeName, ushort nodeId)
        {
            ushort workerId;
            WorkerAddress workerAddress = null;
            if (_node.Spawn(workerTypeName, nodeId, out workerId))
                workerAddress = new WorkerAddress(nodeId, workerId);
            return workerAddress;
        }

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="receiverAddress">Address of the receicer.</param>
        /// <param name="messageType">Type of message - user specific.</param>
        /// <param name="content">The contents of the message.</param>
        public void Send(WorkerAddress receiverAddress, int messageType, object content)
        {
            if (SystemMessages.IsSystemMessageType(messageType))
            {
                Log.Error("Message types smaller than 0 are reserved for system messages. Message type : {0}", messageType);
                return;
            }
            Message msg = new Message(MessageLevel.User, receiverAddress, new WorkerAddress(_node.GetId(), _id), messageType, content);
            _node.QueueMessage(msg);
        }

        /// <summary>
        /// Broadcasts a message to all workers.
        /// </summary>
        /// <param name="messageType">Type of message - user specific.</param>
        /// <param name="content">The contents of the message.</param>
        public void Broadcast(int messageType, object content)
        {
            Message msg = new Message(MessageLevel.User, new WorkerAddress(ushort.MaxValue, ushort.MaxValue), new WorkerAddress(_node.GetId(), _id), messageType, content);
            _node.QueueMessage(msg);
        }

        /// <summary>
        /// Fetches a message from the message queue.
        /// This method blocks until there are any messages.
        /// </summary>
        /// <returns>The first message in the queue.</returns>
        public Message Receive()
        {
            Message msg = null;
            LinkedListNode<Message> msgNode;
            do
            {
                _messageQueueWaitHandle.WaitOne();
                lock (_messageQueue)
                {
                    msgNode = _messageQueue.First;
                    if (msgNode != null)
                    {
                        msg = msgNode.Value;
                        _messageQueue.RemoveFirst();
                    }
                    if (msg == null) //nothing yet
                        _messageQueueWaitHandle.Reset();
                }
            }
            while (msg == null);
            return msg;
        }

        /// <summary>
        /// Fetches a message from the message queue from the specified sender.
        /// This method blocks until there are any messages fullfilling the criteria.
        /// </summary>
        /// <param name="senderAddress">The address of the sender.</param>
        /// <returns>The first message in the queue from the specified sender.</returns>
        public Message Receive(WorkerAddress senderAddress)
        {
            Message msg = null;
            LinkedListNode<Message> msgNode;
            do
            {
                _messageQueueWaitHandle.WaitOne();
                lock (_messageQueue)
                {
                    msgNode = _messageQueue.First;
                    //traverse all messages to see if there are any that matches the search criteria
                    while (msgNode != null && msg == null)
                    {
                        if (msgNode.Value.SenderAddress == senderAddress)
                        {
                            msg = msgNode.Value;
                            _messageQueue.Remove(msgNode);
                        }
                        else
                            msgNode = msgNode.Next;
                    }
                    if (msg == null) //nothing yet
                        _messageQueueWaitHandle.Reset();
                }
            }
            while (msg == null);
            return msg;
        }

        /// <summary>
        /// Fetches a message from the message queue with the specified message type.
        /// This method blocks until there are any messages fullfilling the criteria.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <returns>The first message in the queue with the specified message type.</returns>
        public Message Receive(int messageType)
        {
            Message msg = null;
            LinkedListNode<Message> msgNode;
            do
            {
                _messageQueueWaitHandle.WaitOne();
                lock (_messageQueue)
                {
                    msgNode = _messageQueue.First;
                    //traverse all messages to see if there are any that matches the search criteria
                    while (msgNode != null && msg == null)
                    {
                        if (msgNode.Value.MessageType == messageType)
                        {
                            msg = msgNode.Value;
                            _messageQueue.Remove(msgNode);
                        }
                        else
                            msgNode = msgNode.Next;
                    }
                    if (msg == null) //nothing yet
                        _messageQueueWaitHandle.Reset();
                }
            }
            while (msg == null);
            return msg;
        }

        /// <summary>
        /// Fetches a message from the message queue with the specified sender address and message type.
        /// This method blocks until there are any messages fullfilling the criteria.
        /// </summary>
        /// <param name="senderAddress">The sender address.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The first message in the queue with the specified sender address and message type</returns>
        public Message Receive(WorkerAddress senderAddress, int messageType)
        {
            Message msg = null;
            LinkedListNode<Message> msgNode;
            do
            {
                _messageQueueWaitHandle.WaitOne();
                lock (_messageQueue)
                {
                    msgNode = _messageQueue.First;
                    //traverse all messages to see if there are any that matches the search criteria
                    while (msgNode != null && msg == null)
                    {
                        if (msgNode.Value.SenderAddress == senderAddress && msgNode.Value.MessageType == messageType)
                        {
                            msg = msgNode.Value;
                            _messageQueue.Remove(msgNode);
                        }
                        else
                            msgNode = msgNode.Next;
                    }
                    if (msg == null) //nothing yet
                        _messageQueueWaitHandle.Reset();
                }
            }
            while (msg == null);
            return msg;
        }

        /// <summary>
        /// Checks if there are any messages in the message queue.
        /// </summary>
        /// <returns></returns>
        public bool HasMessages()
        {
            lock (_messageQueue)
            {
                return _messageQueue.Count > 0;
            }
        }

        /// <summary>
        /// Checks if there are any messages from the specified sender in the message queue.
        /// </summary>
        /// <param name="senderAddress"></param>
        /// <returns></returns>
        public bool HasMessages(WorkerAddress senderAddress)
        {
            lock (_messageQueue)
            {
                foreach (Message msg in _messageQueue)
                    if (msg.SenderAddress == senderAddress)
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if there are any messages with the specified message type in the message queue.
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public bool HasMessages(int messageType)
        {
            lock (_messageQueue)
            {
                foreach (Message msg in _messageQueue)
                    if (msg.MessageType == messageType)
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if there are any message from the specified sender and with the specified message type in the message queue.
        /// </summary>
        /// <param name="senderAddress"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public bool HasMessages(WorkerAddress senderAddress, int messageType)
        {
            lock (_messageQueue)
            {
                foreach (Message msg in _messageQueue)
                    if (msg.SenderAddress == senderAddress && msg.MessageType == messageType)
                        return true;
            }
            return false;
        }
        #endregion
    }
}
