//
// IMessage.cs: interface for the messages
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.Text;

namespace Nuxleus.Messaging {
  public class Message : IMessage {
    private string id = null;
    private string entity = null;
    
    public string Id { get { return id; } set{ id = value; } }

    public object Payload { 
      get { return entity; } 
      set { entity = (string)value; } 
    }
    
    public byte[] Serialize() {
      return Encoding.UTF8.GetBytes(entity);
    }

    public void Deserialize(byte[] data) {
      this.Payload = Encoding.UTF8.GetString(data);
    }

    public void Deserialize(string data) {
      this.Payload = data;
    }
  }
}