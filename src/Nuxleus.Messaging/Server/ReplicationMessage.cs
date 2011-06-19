//
// ReplicationMessage.cs: Define the message format for the Nuxleus replication service
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Nuxleus.Entity;

namespace Nuxleus.Messaging.Replication {
    public enum ReplicationOperationType {
        CreateOrReplace,
        Remove,
    }

    [Serializable]
    public sealed class ReplicationMessage {
        private static byte[] delimiter = Encoding.ASCII.GetBytes("\n");
        private static BinaryFormatter formatter = new BinaryFormatter();
        private IEntity entity = null;
        private ReplicationOperationType opType =  ReplicationOperationType.CreateOrReplace;

        public ReplicationMessage () { }

        public ReplicationMessage ( IEntity entity ) {
            this.entity = entity;
        }

        public ReplicationMessage ( ReplicationOperationType opType, IEntity entity ) {
            this.entity = entity;
            this.opType = opType;
        }

        public IEntity Entity {
            get { return entity; }
            set { entity = value; }
        }

        public ReplicationOperationType Type {
            get { return opType; }
            set { opType = value; }
        }

        public static byte[] Serialize ( ReplicationMessage message ) {
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, message);
            ms.Close();

            byte[] data = ms.ToArray();
            byte[] msg = new byte[data.Length + 1];
            data.CopyTo(msg, 0);
            delimiter.CopyTo(msg, data.Length);

            return msg;
        }

        public static ReplicationMessage Deserialize ( byte[] data ) {
            MemoryStream ms = new MemoryStream(data);
            ReplicationMessage msg = (ReplicationMessage)formatter.Deserialize(ms);
            ms.Close();
            return msg;
        }
    }
}