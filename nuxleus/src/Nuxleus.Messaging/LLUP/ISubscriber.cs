//
// ISubscriber.cs : Base interface to be implemented by LLUp subscriber
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.Threading;
using Nuxleus.Llup;

namespace  Nuxleus.Messaging.LLUP {
  public interface ISubscriber {
    SubscriberHandler Handler { get; set; }
    void Start();
    void Stop();
  }
}