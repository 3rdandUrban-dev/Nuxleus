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

namespace MPAPI.RegistrationServer
{
    public sealed class RegistrationServer : IRegistrationServer, IDisposable
    {
        #region class ProxyEndPointMap
        private class ProxyEndPointMap
        {
            private IPEndPoint _endPoint;
            private IRegisteredNode _nodeProxy;

            public ProxyEndPointMap(IPEndPoint endPoint, IRegisteredNode nodeProxy)
            {
                _endPoint = endPoint;
                _nodeProxy = nodeProxy;
            }

            public IRegisteredNode NodeProxy
            {
                get { return _nodeProxy; }
            }

            public IPEndPoint EndPoint
            {
                get { return _endPoint; }
            }
        }
        #endregion

        private Dictionary<ushort, ProxyEndPointMap> _nodeProxies = new Dictionary<ushort, ProxyEndPointMap>();

        #region IRegistrationServer Members

        public bool RegisterNode(IPEndPoint nodeEndPoint)
        {
            IRegisteredNode newNode = ProxyFactory.CreateProxy<IRegisteredNode>(nodeEndPoint);

            //we need to lock all this since the generation of node ids is dependent of what is in the
            //dictionary _nodeProxies and the node will only be registered in the dictionary as the last
            //action.
            lock (_nodeProxies)
            {
                //find an id that is available
                ushort newNodeId = 0;
                while (_nodeProxies.ContainsKey(newNodeId))
                {
                    if (newNodeId == ushort.MaxValue)
                    {
                        Log.Error("Unable to register more nodes. There are already {0} nodes registered", _nodeProxies.Count);
                        return false;
                    }
                    newNodeId++;
                }

                newNode.SetId(newNodeId);
                Log.Info("Registered node. Node Id : {0} , Address : {1} , Port : {2}", newNodeId, nodeEndPoint.Address.ToString(), nodeEndPoint.Port);

                //Notify all existing nodes of this new one
                foreach (ProxyEndPointMap proxyEndPointMap in _nodeProxies.Values)
                    proxyEndPointMap.NodeProxy.NodeRegistered(nodeEndPoint);

                _nodeProxies.Add(newNodeId, new ProxyEndPointMap(nodeEndPoint, newNode));
            }
            return true;
        }

        public void UnregisterNode(ushort nodeId)
        {
            lock (_nodeProxies)
            {
                if (_nodeProxies.ContainsKey(nodeId))
                {
                    IRegisteredNode node = _nodeProxies[nodeId].NodeProxy;
                    _nodeProxies.Remove(nodeId);
                    foreach (ProxyEndPointMap proxyEndPointMap in _nodeProxies.Values)
                        proxyEndPointMap.NodeProxy.NodeUnregistered(nodeId);
                    Log.Info("Unregistered node, Node Id : {0}", nodeId);
                    ((IDisposable)node).Dispose(); //not strictly necessary due to Dispose, but nice nonetheless
                }
            }
        }

        public List<IPEndPoint> GetAllNodeEndPoints()
        {
            List<IPEndPoint> endPoints = new List<IPEndPoint>();
            lock (_nodeProxies)
            {
                foreach (ProxyEndPointMap proxyEndPointMap in _nodeProxies.Values)
                    endPoints.Add(proxyEndPointMap.EndPoint);
            }
            return endPoints;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            lock (_nodeProxies)
            {
                foreach (ProxyEndPointMap proxyEndPointMap in _nodeProxies.Values)
                    ((IDisposable)proxyEndPointMap.NodeProxy).Dispose();
                _nodeProxies.Clear();
            }
        }

        #endregion
    }
}
