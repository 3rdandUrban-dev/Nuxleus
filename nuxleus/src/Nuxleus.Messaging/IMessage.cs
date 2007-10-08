//
// IMessage.cs: interface for the messages
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;

namespace Nuxleus.Messaging {
  public interface IMessage {
    /// <summary>
    /// This could be used to track the message within the application.
    /// It is free form and non portable accross different
    /// implementations of this interface.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// The payload really is the message body itself and it is up
    /// to each implementation of this interface to provide any
    /// meaning in the context it is used.
    /// </summary>
    object Payload { get; set; }

    byte[] Serialize();

    void Deserialize(byte[] data); 
    void Deserialize(string data); 
  }
}