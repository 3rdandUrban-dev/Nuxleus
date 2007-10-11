//
// IRouterFilter.cs : Interface to implement per your specific requirements
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 
using System;
using Nuxleus.Llup;

namespace  Nuxleus.Messaging.LLUP {
  public interface IRouterFilter {
    /// <summary>
    /// Takes a notification and process in order to determine if it should
    /// continue its route or not. 
    /// </summary>
    /// <return>
    /// Returns null to stop the propagation and a notification
    /// object otherwise.
    /// </return>
    Notification ProcessNotification(Notification n);
  }
}