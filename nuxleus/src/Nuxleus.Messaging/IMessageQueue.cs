//
// IMessageQueue.cs: interface for the message queue
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.Collections;
using System.Collections.Generic;

namespace Nuxleus.Messaging {
  public interface IMessageQueue {
    string Id { get; set; }

    void Send(IMessage message);

    /// <summary>
    /// shortcut version to avoid having to create
    /// a full message.
    /// </summary>
    void Send(object payload);

    void Open();
    void Close();
  }
}