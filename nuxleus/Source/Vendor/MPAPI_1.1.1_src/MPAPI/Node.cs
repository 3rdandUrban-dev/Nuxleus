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
using System.Net;
using RemotingLite;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MPAPI.RegistrationServer;
using System.Reflection;

namespace MPAPI
{
    public sealed class Node : INode, IWorkerNode, IRegisteredNode, IDisposable
    {
        private sealed class ContentTypes
        {
            internal const byte Unknown = 0x00;
            internal const byte Primitive = 0x01;
        }

        public const int WorkerCountWarningLimit = 1000;

        private ManualResetEvent _sendBufferWaitHandle = new ManualResetEvent(false);
        private ServiceHost _host;
        private IRegistrationServer _registrationServerProxy;
        private Dictionary<ushort, Worker> _localWorkers = new Dictionary<ushort, Worker>();
        private Dictionary<ushort, INode> _remoteNodes = new Dictionary<ushort, INode>();
        private ushort _id;
        private object _idLock = new object();
        private LinkedList<Message> _sendBuffer = new LinkedList<Message>();
        private bool _sendBufferThreadTerminate = false;
        private object _sendBufferThreadTerminateLock = new object();
        private Dictionary<Type, byte> _parameterTypes;

        /// <summary>
        /// Thread safe get and set
        /// </summary>
        private bool SendBufferThreadTerminate
        {
            get
            {
                lock (_sendBufferThreadTerminateLock)
                {
                    return _sendBufferThreadTerminate;
                }
            }
            set
            {
                lock (_sendBufferThreadTerminateLock)
                {
                    _sendBufferThreadTerminate = value;
                }
            }
        }

        public Node()
        {
            _parameterTypes = new Dictionary<Type, byte>();
            _parameterTypes.Add(typeof(bool), ContentTypes.Primitive);
            _parameterTypes.Add(typeof(byte), ContentTypes.Primitive);
            _parameterTypes.Add(typeof(sbyte), ContentTypes.Primitive);
            _parameterTypes.Add(typeof(char), ContentTypes.Primitive);
            _parameterTypes.Add(typeof(decimal), ContentTypes.Primitive);
            _parameterTypes.Add(typeof(double), ContentTypes.Primitive);
            _parameterTypes.Add(typeof(float), ContentTypes.Primitive);
            _parameterTypes.Add(typeof(int), ContentTypes.Primitive);
            _parameterTypes.Add(typeof(uint), ContentTypes.Primitive);
            _parameterTypes.Add(typeof(long), ContentTypes.Primitive);
            _parameterTypes.Add(typeof(ulong), ContentTypes.Primitive);
            _parameterTypes.Add(typeof(short), ContentTypes.Primitive);
            _parameterTypes.Add(typeof(ushort), ContentTypes.Primitive);
            _parameterTypes.Add(typeof(string), ContentTypes.Primitive);

            //start the thread that processes outgoing messages
            Thread sendBufferThread = new Thread(SendBufferThreadProc);
            sendBufferThread.IsBackground = true;
            sendBufferThread.Start();
        }

        /// <summary>
        /// Closes this node and calls Dispose().
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Opens this node and connects to the registration server.
        /// Use this when starting slave nodes.
        /// </summary>
        /// <param name="registrationServerNameOrAddress">Name or address of the registration server.</param>
        /// <param name="registrationServerPort">The port number to the registration server.</param>
        /// <param name="listenerPort">The port that this node accepts connections on.</param>
        /// <returns>The worker instance if successfull, otherwise null</returns>
        public void OpenDistributed(string registrationServerNameOrAddress, int registrationServerPort, int listenerPort)
        {
            //Since there is no main worker type here we assume that this is a slave node, and thus has to be online
            OpenAndConnectToRegistrationServer(registrationServerNameOrAddress, registrationServerPort, listenerPort);
        }

        /// <summary>
        /// Opens this node, connects to the registrations server and starts a worker.
        /// </summary>
        /// <typeparam name="TRootWorker">Type of worker to start</typeparam>
        /// <param name="registrationServerNameOrAddress">Name or address of the registration server.</param>
        /// <param name="registrationServerPort">The port number to the registration server.</param>
        /// <param name="listenerPort">The port that this node accepts connections on.</param>
        /// <returns>The worker instance if successfull, otherwise null</returns>
        public TRootWorker OpenDistributed<TRootWorker>(string registrationServerNameOrAddress, int registrationServerPort, int listenerPort) where TRootWorker : Worker
        {
            OpenAndConnectToRegistrationServer(registrationServerNameOrAddress, registrationServerPort, listenerPort);
            return OpenLocal<TRootWorker>();
        }

        /// <summary>
        /// Opens this node in local mode and spawns a worker.
        /// </summary>
        /// <typeparam name="TRootWorker">Type of worker to start</typeparam>
        /// <returns>The worker instance if successfull, otherwise null</returns>
        public TRootWorker OpenLocal<TRootWorker>() where TRootWorker : Worker
        {
            ushort workerId;
            TRootWorker rootWorker = null;
            if (!Spawn(typeof(TRootWorker), out workerId))
                Log.Error("Could not spawn worker");
            else
                rootWorker = (TRootWorker)_localWorkers[workerId];
            return rootWorker;
        }

        /// <summary>
        /// Opens a host to accept incoming connections. Connects to a registration server.
        /// </summary>
        /// <param name="registrationServerNameOrAddress">Name or address of the registration server.</param>
        /// <param name="registrationServerPort">The port number to the registration server.</param>
        /// <param name="listenerPort">The port that this node accepts connections on.</param>
        private void OpenAndConnectToRegistrationServer(string registrationServerNameOrAddress, int registrationServerPort, int listenerPort)
        {
            if (_host == null)
            {
                try
                {
                    //Open a connection to the registration server
                    IPEndPoint registrationServerEndPoint = new IPEndPoint(Dns.GetHostEntry(registrationServerNameOrAddress).AddressList[0], registrationServerPort);
                    _registrationServerProxy = ProxyFactory.CreateProxy<IRegistrationServer>(registrationServerEndPoint);
                    
                    //get the nodes that are already registered
                    List<IPEndPoint> remoteNodeEndPoints = _registrationServerProxy.GetAllNodeEndPoints();
                    _remoteNodes.Clear();
                    foreach (IPEndPoint endpoint in remoteNodeEndPoints)
                    {
                        INode remoteNode = new RemoteNodeProxy(endpoint);
                        _remoteNodes.Add(remoteNode.GetId(), remoteNode);
                    }

                    //open the host that contains this node
                    _host = new ServiceHost(this, listenerPort);
                    _host.Open();

                    //Register the host with the registration server. This causes all other registered nodes to connect to this one
                    _registrationServerProxy.RegisterNode(_host.EndPoint);
                }
                catch (Exception ex)
                {
                    string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    Log.Error("Could not connect to registration server. Message reads : {0}", msg);
                }
            }
        }

        /// <summary>
        /// Queue up the message to be sent. This prevents the calling worker
        /// from being locked for longer than strictly necessary.
        /// </summary>
        /// <param name="msg">The message to be sent.</param>
        internal void QueueMessage(Message msg)
        {
            if (msg.ReceiverAddress.NodeId == _id) //it is a local worker - dispatch it immediately
            {
                lock (_localWorkers)
                {
                    if (_localWorkers.ContainsKey(msg.ReceiverAddress.WorkerId))
                        _localWorkers[msg.ReceiverAddress.WorkerId].PutMessage(msg.MessageLevel, msg.ReceiverAddress.NodeId, msg.ReceiverAddress.WorkerId, msg.SenderAddress.NodeId, msg.SenderAddress.WorkerId, msg.MessageType, CloneContent(msg.Content));
                }
            }
            else
            {
                lock (_sendBuffer)
                {
                    _sendBuffer.AddLast(msg);
                    _sendBufferWaitHandle.Set(); //signal new messages
                }
            }
        }

        /// <summary>
        /// Spawns a new worker at the specified node.
        /// </summary>
        /// <param name="workerType">The type of worker to spawn.</param>
        /// <param name="nodeId">Id of the node to spawn it at. If nodeId==_id the worker is spawned locally.</param>
        /// <param name="workerId">The id of the new worker.</param>
        /// <returns>True if the spawn was successfull, otherwise false.</returns>
        internal bool Spawn(Type workerType, ushort nodeId, out ushort workerId)
        {
            if (nodeId == _id)
                return Spawn(workerType, out workerId);
            lock (_remoteNodes)
            {
                if (_remoteNodes.ContainsKey(nodeId))
                {
                    try
                    {
                        return _remoteNodes[nodeId].Spawn(workerType, out workerId);
                    }
                    catch (Exception)
                    {
                        Log.Error("Node.Spawn({0}, {1}, out workerId) : Remote node {1} appears to be offline", workerType.FullName, nodeId);
                        _remoteNodes.Remove(nodeId);
                        _registrationServerProxy.UnregisterNode(nodeId);
                        workerId = 0;
                        return false;
                    }
                }
            }
            workerId = 0;
            return false;
        }

        /// <summary>
        /// Spawns a new worker at the specified node.
        /// </summary>
        /// <param name="workerTypeName">Fully qualified name of the worker type to spawn.</param>
        /// <param name="nodeId">Id of the node to spawn it at. If nodeId==_id the worker is spawned locally.</param>
        /// <param name="workerId">The id of the new worker.</param>
        /// <returns>True if the spawn was successfull, otherwise false.</returns>
        internal bool Spawn(string workerTypeName, ushort nodeId, out ushort workerId)
        {
            if (nodeId == _id)
                return Spawn(workerTypeName, out workerId);
            lock (_remoteNodes)
            {
                if (_remoteNodes.ContainsKey(nodeId))
                {
                    try
                    {
                        return _remoteNodes[nodeId].Spawn(workerTypeName, out workerId);
                    }
                    catch (Exception)
                    {
                        Log.Error("Node.Spawn(\"{0}\", {1}, out workerId) : Remote node {1} appears to be offline", workerTypeName, nodeId);
                        _remoteNodes.Remove(nodeId);
                        _registrationServerProxy.UnregisterNode(nodeId);
                        workerId = 0;
                        return false;
                    }
                }
            }
            workerId = 0;
            return false;
        }

        /// <summary>
        /// Removes the worker from the local pool of workers. This is called when a worker terminates normally.
        /// </summary>
        /// <param name="workerId"></param>
        internal void WorkerTerminated(ushort workerId)
        {
            lock (_localWorkers)
            {
                _localWorkers.Remove(workerId);
            }
        }

        /// <summary>
        /// This method handles the dispatching of messages in the send buffer.
        /// 
        /// This is done in a separate thread so that local workers do not wait for each other,
        /// or wait for the relatively slow communication over the remoting framework.
        /// </summary>
        /// <param name="state"></param>
        private void SendBufferThreadProc(object state)
        {
            while (!SendBufferThreadTerminate)
            {
                _sendBufferWaitHandle.WaitOne();
                //fetch the first message
                Message msg = null;
                lock (_sendBuffer)
                {
                    LinkedListNode<Message> firstNode = _sendBuffer.First;
                    if (firstNode != null)
                    {
                        msg = firstNode.Value;
                        _sendBuffer.RemoveFirst();
                    }
                    else
                        _sendBufferWaitHandle.Reset();
                }

                /* Keep processing messages if there are any (no sleep - the thread
                 * will be switched out naturally), otherwise force the thread to be switched
                 * out of context. */
                if (msg != null)
                {
                    //is it a broad cast message?
                    if (WorkerAddress.IsBroadcastAddress(msg.ReceiverAddress))
                    {
                        //send it to all local workers (except the sender)
                        lock (_localWorkers)
                        {
                            foreach (Worker worker in _localWorkers.Values)
                                if (msg.SenderAddress.NodeId != _id || worker.Id != msg.SenderAddress.WorkerId)
                                    worker.PutMessage(msg.MessageLevel, msg.ReceiverAddress.NodeId, msg.ReceiverAddress.WorkerId, msg.SenderAddress.NodeId, msg.SenderAddress.WorkerId, msg.MessageType, CloneContent(msg.Content)); //clone to prevent shared state
                        }
                        //broadcast it to all remote nodes
                        lock (_remoteNodes)
                        {
                            foreach (INode remoteNode in _remoteNodes.Values)
                                remoteNode.PutMessage(msg.MessageLevel, msg.ReceiverAddress.NodeId, msg.ReceiverAddress.WorkerId, msg.SenderAddress.NodeId, msg.SenderAddress.WorkerId, msg.MessageType, msg.Content);
                        }
                    }
                    else if (msg.ReceiverAddress.NodeId == _id) //it is a local receiver
                    {
                        lock (_localWorkers)
                        {
                            if (_localWorkers.ContainsKey(msg.ReceiverAddress.WorkerId))
                                _localWorkers[msg.ReceiverAddress.WorkerId].PutMessage(msg.MessageLevel, msg.ReceiverAddress.NodeId, msg.ReceiverAddress.WorkerId, msg.SenderAddress.NodeId, msg.SenderAddress.WorkerId, msg.MessageType, CloneContent(msg.Content));
                        }
                    }
                    else //not a broadcast, not a local receiver, then find the remote node to send it to
                    {
                        lock (_remoteNodes)
                        {
                            ushort nodeId = msg.ReceiverAddress.NodeId;
                            if (_remoteNodes.ContainsKey(nodeId))
                            {
                                try
                                {
                                    _remoteNodes[nodeId].PutMessage(msg.MessageLevel, msg.ReceiverAddress.NodeId, msg.ReceiverAddress.WorkerId, msg.SenderAddress.NodeId, msg.SenderAddress.WorkerId, msg.MessageType, msg.Content);
                                }
                                catch (Exception)
                                {
                                    Log.Error("Node.SendBufferThreadProc : Remote node {0} appears to be offline", nodeId);
                                    _remoteNodes.Remove(nodeId);
                                    _registrationServerProxy.UnregisterNode(nodeId);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clones the contents. If it is a primitive type it is not necessary to clone it to prevent shared state
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private object CloneContent(object content)
        {
            if (content == null)
                return null;
            Type contentType = content.GetType();
            byte contentTypeByte = _parameterTypes.ContainsKey(contentType) ? _parameterTypes[contentType] : ContentTypes.Unknown;
            switch (contentTypeByte)
            {
                case ContentTypes.Primitive:
                    return content;
                default:
                    MemoryStream ms = new MemoryStream();
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(ms, content);
                    ms.Seek(0, SeekOrigin.Begin);
                    return formatter.Deserialize(ms);
            }
        }

        internal void _Monitor(WorkerAddress monitor, WorkerAddress monitoree)
        {
            if (monitoree.NodeId == _id)
                Monitor(monitor, monitoree);
            else
            {
                lock (_remoteNodes)
                {
                    ushort monitoreeNodeId = monitoree.NodeId;
                    if (_remoteNodes.ContainsKey(monitoreeNodeId))
                    {
                        try
                        {
                            _remoteNodes[monitoree.NodeId].Monitor(monitor, monitoree);
                        }
                        catch (Exception)
                        {
                            Log.Error("Node._Monitor({0}, {1}) : Remote node {2} appears to be offline", monitor, monitoree, monitoree.NodeId);
                            _remoteNodes.Remove(monitoreeNodeId);
                            _registrationServerProxy.UnregisterNode(monitoreeNodeId);
                        }
                    }
                    else
                        Log.Error("Node._Monitor({0}, {1}) : Unable to find remote monitoree", monitor, monitoree);
                }
            }
        }

        #region INode Members
        public void PutMessage(MessageLevel messageLevel, ushort receiverNodeId, ushort receiverWorkerId, ushort senderNodeId, ushort senderWorkerId, int messageType, object content)
        {
            //see if it is a local worker that is the receiver
            lock (_localWorkers)
            {
                //Message msg = new Message(messageLevel, new WorkerAddress(receiverNodeId, receiverWorkerId), new WorkerAddress(senderNodeId, senderWorkerId), messageType, content);
                if (WorkerAddress.IsBroadcastAddress(receiverNodeId, receiverWorkerId)) //broadcast address
                {
                    foreach (Worker worker in _localWorkers.Values)
                        worker.PutMessage(messageLevel, receiverNodeId, receiverWorkerId, senderNodeId, senderWorkerId, messageType, CloneContent(content)); //clone the message to prevent shared state
                }
                else if (_localWorkers.ContainsKey(receiverWorkerId))
                    _localWorkers[receiverWorkerId].PutMessage(messageLevel, receiverNodeId, receiverWorkerId, senderNodeId, senderWorkerId, messageType, CloneContent(content));
                else
                    if (!SystemMessages.IsSystemMessageType(messageType)) //we don't care about that
                        Log.Error("Node.PutMessage : Cannot dispatch message");
            }
        }

        public List<ushort> GetWorkerIds()
        {
            List<ushort> workerIds = new List<ushort>();
            lock (_localWorkers)
            {
                foreach (ushort workerId in _localWorkers.Keys)
                    workerIds.Add(workerId);
            }
            return workerIds;
        }

        public bool Spawn(Type workerType, out ushort workerId)
        {
            object oWorker = Activator.CreateInstance(workerType);
            if (!(oWorker is Worker))
                throw new TypeLoadException(String.Format("The type '{0}' does not inherit from MPAPI.Worker", workerType.FullName));

            Worker worker = oWorker as Worker;
            lock (_localWorkers)
            {
                //find the next id
                workerId = 0;
                while (_localWorkers.ContainsKey(workerId))
                {
                    if (workerId == ushort.MaxValue)
                    {
                        Log.Error("Unable to spawn more workers. There are already {0} workers running", _localWorkers.Count);
                        return false;
                    }
                    workerId++;
                }
                worker.Id = workerId;
                worker.SetNode(this);
                _localWorkers.Add(workerId, worker);

                Thread workerThread = new Thread(new ThreadStart(worker._Main)); //we use a ThreadStart since the _Start method has the wrong signature for a thread method
                workerThread.IsBackground = true;
                workerThread.Start();

                if (_localWorkers.Count >= WorkerCountWarningLimit)
                    Log.Warning("There are now {0} workers on this node.", _localWorkers.Count);
            }
            return true;
        }

        public bool Spawn(string workerTypeName, out ushort workerId)
        {
            Type workerType;
            try
            {
                workerType = Type.GetType(workerTypeName);
                return Spawn(workerType, out workerId);
            }
            catch (Exception)
            {
                Log.Error("Node cannot spawn worker with name '{0}'", workerTypeName);
                workerId = 0;
                return false;
            }
        }

        public int GetProcessorCount()
        {
            return Environment.ProcessorCount;
        }

        public int GetWorkerCount()
        {
            lock (_localWorkers)
            {
                return _localWorkers.Count;
            }
        }

        public void Monitor(WorkerAddress monitor, WorkerAddress monitoree)
        {
            if (monitoree.NodeId == _id)
            {
                lock (_localWorkers)
                {
                    if (_localWorkers.ContainsKey(monitoree.WorkerId))
                        _localWorkers[monitoree.WorkerId].SetMonitoringWorker(monitor);
                    else
                        Log.Error("Node.Monitor({0}, {1}) : unable to find monitoree", monitor, monitoree);
                }
            }
        }

        public IPEndPoint GetIPEndPoint()
        {
            return _host.EndPoint;
        }
        #endregion

        #region IRegisteredNode Members
        public void NodeRegistered(IPEndPoint nodeEndPoint)
        {
            lock (_remoteNodes)
            {
                INode node = new RemoteNodeProxy(nodeEndPoint);
                ushort nodeId = node.GetId();
                _remoteNodes.Add(nodeId, node);
                //notify all local workers that a new remote node has entered the grid
                lock (_localWorkers)
                {
                    foreach (Worker worker in _localWorkers.Values)
                        worker.OnRemoteNodeRegistered(nodeId);
                }
            }
        }

        public void NodeUnregistered(ushort nodeId)
        {
            lock (_remoteNodes)
            {
                if (_remoteNodes.ContainsKey(nodeId))
                {
                    INode node = _remoteNodes[nodeId];
                    _remoteNodes.Remove(nodeId);
                    ((IDisposable)node).Dispose(); //not strictly necessary due to Dispose, but nice nonetheless
                }
            }
            //notify all local workers that a remote node has dropped off the grid
            lock (_localWorkers)
            {
                foreach (Worker worker in _localWorkers.Values)
                    worker.OnRemoteNodeUnregistered(nodeId);
            }
        }

        public void SetId(ushort nodeId)
        {
            lock (_idLock)
            {
                _id = nodeId;
            }
        }
        #endregion

        #region IWorkerNode Members
        public List<ushort> GetRemoteNodeIds()
        {
            lock (_remoteNodes)
            {
                List<ushort> remoteNodeIds = new List<ushort>();
                foreach (ushort nodeId in _remoteNodes.Keys)
                    remoteNodeIds.Add(nodeId);
                return remoteNodeIds;
            }
        }

        public int GetProcessorCount(ushort nodeId)
        {
            if (nodeId == _id)
                return GetProcessorCount();

            lock (_remoteNodes)
            {
                if (_remoteNodes.ContainsKey(nodeId))
                {
                    try
                    {
                        return _remoteNodes[nodeId].GetProcessorCount();
                    }
                    catch (Exception)
                    {
                        Log.Error("Node.GetProcessorCount({0}) : Remote node {0} appears to be offline", nodeId);
                        _remoteNodes.Remove(nodeId);
                        _registrationServerProxy.UnregisterNode(nodeId);
                    }

                }
            }
            return -1;
        }

        public int GetWorkerCount(ushort nodeId)
        {
            if (nodeId == _id)
                return GetWorkerCount();
            lock (_remoteNodes)
            {
                if (_remoteNodes.ContainsKey(nodeId))
                {
                    try
                    {
                        return _remoteNodes[nodeId].GetWorkerCount();
                    }
                    catch (Exception)
                    {
                        Log.Error("Node.GetWorkerCount({0}) : Remote node {0} appears to be offline", nodeId);
                        _remoteNodes.Remove(nodeId);
                        _registrationServerProxy.UnregisterNode(nodeId);
                    }
                }
            }
            return -1;
        }

        public IPEndPoint GetIPEndPoint(ushort nodeId)
        {
            if (nodeId == _id)
                return _host.EndPoint;
            else
            {
                lock (_remoteNodes)
                {
                    if (_remoteNodes.ContainsKey(nodeId))
                        return _remoteNodes[nodeId].GetIPEndPoint();
                    else
                        return null;
                }
            }
        }
        #endregion

        #region INodeIdentity Members
        public ushort GetId()
        {
            lock (_idLock)
            {
                return _id;
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Disconnects from the registration server and closes all connections to other nodes.
        /// </summary>
        public void Dispose()
        {
            //kill the send buffer processing thread
            SendBufferThreadTerminate = true;

            //dispose remote nodes
            lock (_remoteNodes)
            {
                foreach (INode nodeProxy in _remoteNodes.Values)
                    ((IDisposable)nodeProxy).Dispose();
                _remoteNodes.Clear();
            }

            //clear local workers
            lock (_localWorkers)
            {
                _localWorkers.Clear();
            }

            //disconnect from the registration server
            if (_registrationServerProxy != null)
            {
                _registrationServerProxy.UnregisterNode(_id);
                ((IDisposable)_registrationServerProxy).Dispose();
            }

            //kill the host
            if (_host != null)
                _host.Dispose();
            _host = null;
        }
        #endregion
    }
}
