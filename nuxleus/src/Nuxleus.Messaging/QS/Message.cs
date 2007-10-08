//
// IMessage.cs: interface for the messages
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using Nuxleus.Bucker;

namespace Nuxleus.Messaging.QS {
  public class Message : IMessage {
    private string id = null;
    private Nuxleus.Bucker.Message qsMessage = null;
    
    public string Id { get { return id; } set{ id = value; } }

    public object Payload { 
      get { return qsMessage; } 
      set { qsMessage = (Nuxleus.Bucker.Message)value; } 
    }
    
    public byte[] Serialize() {
      return Nuxleus.Bucker.Message.Serialize((Nuxleus.Bucker.Message)this.Payload);
    }

    public void Deserialize(byte[] data) {
      Nuxleus.Bucker.Message qs = Nuxleus.Bucker.Message.Parse(data);
      this.Id = qs.ResponseId;
      this.Payload = qs;
    }

    public void Deserialize(string data) {
      Nuxleus.Bucker.Message qs = Nuxleus.Bucker.Message.Parse(data);
      this.Id = qs.ResponseId;
      this.Payload = qs;
    }
  }
}