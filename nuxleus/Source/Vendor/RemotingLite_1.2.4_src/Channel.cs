/*************************************************************************************************
 * RemotingLite
 * ------
 * A light framework for making remote method invocations using TCP/IP. It is based loosely on
 * Windows Communication Foundation, and is meant to provide programmers with the same API
 * regardless of whether they write software for the Microsoft .NET platform or the Mono .NET
 * platform.
 * Consult the documentation and example applications for information about how to use this API.
 * 
 * Author       : Frank Thomsen
 * http         : http://sector0.dk
 * Concact      : http://sector0.dk/?q=contact
 * Information  : http://sector0.dk/?q=node/27
 * Licence      : Free. If you use this, please let me know.
 * 
 *          Please feel free to contact me with ideas, bugs or improvements.
 *************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace RemotingLite
{
    public class Channel : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private BinaryReader _binReader;
        private BinaryWriter _binWriter;
        private BinaryFormatter _formatter;
        private Dictionary<int, int> _methodMaps; //a map of client side hash code / server side hash codes
        private ParameterTransferHelper _parameterTransferHelper = new ParameterTransferHelper();

        /// <summary>
        /// Creates a connection to the concrete object handling method calls on the server side
        /// </summary>
        /// <param name="endpoint"></param>
        public Channel(IPEndPoint endpoint)
        {
            _client = new TcpClient(AddressFamily.InterNetwork);
            _client.Connect(endpoint);
            _client.NoDelay = true;
            _stream = _client.GetStream();
            _binReader = new BinaryReader(_stream);
            _binWriter = new BinaryWriter(_stream);
            _formatter = new BinaryFormatter();
            _methodMaps = new Dictionary<int, int>();
            NegotiateInterface();
        }

        /// <summary>
        /// This method maps the hash code of the method on the client side with the
        /// hash code of the method server side.
        /// </summary>
        private void NegotiateInterface()
        {
            MethodInfo[] methods = GetType().GetMethods();
            List<MethodMap> methodMaps = new List<MethodMap>();
            foreach (MethodInfo method in methods)
            {
                ParameterInfo[] parameterInfos = method.GetParameters();
                Type[] parameterTypes = new Type[parameterInfos.Length];
                for (int i = 0; i < parameterInfos.Length; i++)
                    parameterTypes[i] = parameterInfos[i].ParameterType;
                methodMaps.Add(new MethodMap(method.GetHashCode(), method.Name, parameterTypes));
            }

            _binWriter.Write((int)MessageType.NegotiateInterface);
            //serialize the method maps
            MemoryStream ms = new MemoryStream();
            _formatter.Serialize(ms, methodMaps);
            ms.Seek(0, SeekOrigin.Begin);
            //write the data length
            _binWriter.Write((int)ms.Length);
            //write the data
            _binWriter.Write(ms.ToArray());
            _binWriter.Flush();
            _stream.Flush();

            //read the reply
            ms = new MemoryStream(_binReader.ReadBytes(_binReader.ReadInt32()));
            methodMaps = (List<MethodMap>)_formatter.Deserialize(ms);
            foreach (MethodMap mm in methodMaps)
                _methodMaps.Add(mm.clientSideHashcode, mm.serverSideHashcode);
        }

        /// <summary>
        /// Closes the connection to the server
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Invokes the method with the specified parameters.
        /// </summary>
        /// <param name="methodName">The name of the method</param>
        /// <param name="parameters">Parameters for the method call</param>
        /// <returns>An array of objects containing the return value (index 0) and the parameters used to call
        /// the method, including any marked as "ref" or "out"</returns>
        protected object[] InvokeMethod(params object[] parameters)
        {
            //write the message type
            _binWriter.Write((int)MessageType.MethodInvocation);

            //write the method hash code
            int methodHashcode = (new StackFrame(1)).GetMethod().GetHashCode();
            if(!_methodMaps.ContainsKey(methodHashcode))
                throw new Exception("Unknown method.");
            _binWriter.Write(_methodMaps[methodHashcode]);

            //send the parameters
            _parameterTransferHelper.SendParameters(_binWriter, parameters);

            _binWriter.Flush();
            _stream.Flush();

            // Read the result of the invocation.
            MessageType messageType =(MessageType)_binReader.ReadInt32();
            if (messageType == MessageType.UnknownMethod)
                throw new Exception("Unknown method.");
            return _parameterTransferHelper.ReceiveParameters(_binReader);
        }

        #region IDisposable Members

        public void Dispose()
        {
            _binWriter.Write((int)MessageType.TerminateConnection);
            _binWriter.Flush();
            _binWriter.Close();
            _binReader.Close();
            _stream.Flush();
            _stream.Close();
            _client.Close();
        }

        #endregion
    }
}
