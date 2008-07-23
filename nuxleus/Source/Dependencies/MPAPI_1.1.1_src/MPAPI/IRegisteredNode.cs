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

namespace MPAPI
{
    /// <summary>
    /// The interface of a node as seen from the registration server
    /// </summary>
    public interface IRegisteredNode : INodeIdentity
    {
        /// <summary>
        /// Called when a new node has registered with the registration server.
        /// </summary>
        /// <param name="nodeEndPoint">The end point of the new node.</param>
        void NodeRegistered(IPEndPoint nodeEndPoint);

        /// <summary>
        /// Called when a registered node unregisters - typically when closing.
        /// </summary>
        /// <param name="nodeId">Id of the node.</param>
        void NodeUnregistered(ushort nodeId);

        /// <summary>
        /// Called by the registration server when a node has registered. The registration server is responsible
        /// for assigning id's to nodes.
        /// </summary>
        /// <param name="nodeId"></param>
        void SetId(ushort nodeId);
    }
}
