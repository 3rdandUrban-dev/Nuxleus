//
// Message.cs: Implementation of the IMessage interface
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;

namespace Nuxleus.Messaging {
    public class Message : IMessage {
        private string id = null;
        private byte[] data = null;

        public string Id { get { return id; } set { id = value; } }

        public byte[] InnerMessage {
            get { return data; }
            set { data = value; }
        }
    }
}