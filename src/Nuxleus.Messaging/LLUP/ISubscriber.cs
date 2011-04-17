//
// ISubscriber.cs : Base interface to be implemented by LLUP subscriber
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.Threading;

namespace Nuxleus.Messaging.LLUP {
    public interface ISubscriber {
        SubscriberHandler Handler { get; set; }
        void Start ();
        void Stop ();
    }
}