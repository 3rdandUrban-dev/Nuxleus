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

namespace MPAPI
{
    public interface IWorker
    {
        IWorkerNode Node { get;}

        /// <summary>
        /// Sets a filter on the types of messages that will be put into the mail box
        /// </summary>
        /// <param name="filters"></param>
        void SetMessageFilter(params int[] filters);

        /// <summary>
        /// Makes the thread running this worker sleep for a period of time.
        /// </summary>
        /// <param name="ms">The number of milliseconds to sleep.</param>
        void Sleep(int ms);

        /// <summary>
        /// Enables this worker to receive system messages when the monitoree terminates, either
        /// normally or abnormally.
        /// </summary>
        /// <param name="monitoree"></param>
        void Monitor(WorkerAddress monitoree);

        /// <summary>
        /// Spawns a new worker locally.
        /// </summary>
        /// <typeparam name="TWorkerType">The type of worker to spawn. Must inherit from Worker.</typeparam>
        /// <returns>The address of the new worker if successfull, otherwise null.</returns>
        WorkerAddress Spawn<TWorkerType>() where TWorkerType : Worker;

        /// <summary>
        /// Spawns a new worker at the specified node.
        /// </summary>
        /// <typeparam name="TWorkerType">The type of worker to spawn. Must inherit from Worker.</typeparam>
        /// <param name="nodeId">Id of the node to spawn the worker at.</param>
        /// <returns>The address of the new worker if successfull, otherwise null.</returns>
        WorkerAddress Spawn<TWorkerType>(ushort nodeId) where TWorkerType : Worker;

        /// <summary>
        /// Spawns a new worker at the specified node.
        /// </summary>
        /// <param name="workerTypeName">The fully qualified name of the worker to spawn. Must inherit from Worker.</param>
        /// <param name="nodeId">Id of the node to spawn the worker at.</param>
        /// <returns>The address of the new worker if successfull, otherwise null.</returns>
        WorkerAddress Spawn(string workerTypeName, ushort nodeId);

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="receiverAddress">Address of the receicer.</param>
        /// <param name="messageType">Type of message - user specific.</param>
        /// <param name="content">The contents of the message.</param>
        void Send(WorkerAddress receiverAddress, int messageType, object content);

        /// <summary>
        /// Broadcasts a message to all workers.
        /// </summary>
        /// <param name="messageType">Type of message - user specific.</param>
        /// <param name="content">The contents of the message.</param>
        void Broadcast(int messageType, object content);

        /// <summary>
        /// Fetches a message from the message queue.
        /// This method blocks until there are any messages.
        /// </summary>
        /// <returns>The first message in the queue.</returns>
        Message Receive();

        /// <summary>
        /// Fetches a message from the message queue from the specified sender.
        /// This method blocks until there are any messages fullfilling the criteria.
        /// </summary>
        /// <param name="senderAddress">The address of the sender.</param>
        /// <returns>The first message in the queue from the specified sender.</returns>
        Message Receive(WorkerAddress senderAddress);

        /// <summary>
        /// Fetches a message from the message queue with the specified message type.
        /// This method blocks until there are any messages fullfilling the criteria.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <returns>The first message in the queue with the specified message type.</returns>
        Message Receive(int messageType);

        /// <summary>
        /// Fetches a message from the message queue with the specified sender address and message type.
        /// This method blocks until there are any messages fullfilling the criteria.
        /// </summary>
        /// <param name="senderAddress">The sender address.</param>
        /// <param name="messageType">The message type.</param>
        /// <returns>The first message in the queue with the specified sender address and message type</returns>
        Message Receive(WorkerAddress senderAddress, int messageType);

        /// <summary>
        /// Checks if there are any messages in the message queue.
        /// </summary>
        /// <returns></returns>
        bool HasMessages();

        /// <summary>
        /// Checks if there are any messages from the specified sender in the message queue.
        /// </summary>
        /// <param name="senderAddress"></param>
        /// <returns></returns>
        bool HasMessages(WorkerAddress senderAddress);

        /// <summary>
        /// Checks if there are any messages with the specified message type in the message queue.
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        bool HasMessages(int messageType);

        /// <summary>
        /// Checks if there are any message from the specified sender and with the specified message type in the message queue.
        /// </summary>
        /// <param name="senderAddress"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        bool HasMessages(WorkerAddress senderAddress, int messageType);
    }
}
