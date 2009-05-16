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
    public sealed class WorkerAddress
    {
        private ushort _nodeId;
        private ushort _workerId;

        /// <summary>
        /// Gets the node id part of the address.
        /// </summary>
        public ushort NodeId { get { return _nodeId; } }

        /// <summary>
        /// Gets the worker id part of the address.
        /// </summary>
        public ushort WorkerId { get { return _workerId; } }

        public static bool IsBroadcastAddress(ushort nodeId, ushort workerId)
        {
            return nodeId == ushort.MaxValue && workerId == ushort.MaxValue;
        }

        public static bool IsBroadcastAddress(WorkerAddress address)
        {
            return address.NodeId == ushort.MaxValue && address.WorkerId == ushort.MaxValue;
        }

        public WorkerAddress(ushort nodeId, ushort workerId)
        {
            _nodeId = nodeId;
            _workerId = workerId;
        }

        public static bool operator ==(WorkerAddress addr1, WorkerAddress addr2)
        {
            if ((object)addr1 == null && (object)addr2 == null)
                return true;
            else if ((object)addr1 != null && (object)addr2 != null)
                return (addr1.NodeId == addr2.NodeId) && (addr1.WorkerId == addr2.WorkerId);
            return false;
        }

        public static bool operator !=(WorkerAddress addr1, WorkerAddress addr2)
        {

            if ((object)addr1 == null && (object)addr2 == null)
                return false;
            else if ((object)addr1 != null && (object)addr2 != null)
                return (addr1.NodeId != addr2.NodeId) || (addr1.WorkerId != addr2.WorkerId);
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is WorkerAddress)
                return this == (WorkerAddress)obj;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}@{1}", WorkerId, NodeId);
        }

        //internal byte[] Serialize()
        //{
        //    byte[] data = new byte[4];
        //    Array.Copy(BitConverter.GetBytes(_nodeId), 0, data, 0, 2);
        //    Array.Copy(BitConverter.GetBytes(_workerId), 0, data, 2, 2);
        //    return data;
        //}

        //internal static WorkerAddress Deserialize(byte[] data, int index)
        //{
        //    ushort nodeId = BitConverter.ToUInt16(data, index);
        //    ushort workerId = BitConverter.ToUInt16(data, index + 2);
        //    return new WorkerAddress(nodeId, workerId);
        //}
    }
}
