//
// IRouterFilter.cs : Interface to implement per your specific requirements
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 
using System;

namespace Nuxleus.Messaging.LLUP {
    public interface IRouterFilter {
        /// <summary>
        /// Takes a notification and process in order to determine if it should
        /// continue its route or not. 
        /// The index allows filters to query for meta-data and determine
        /// from the already indexed notifications whether or not it's
        /// appropriate to allow the given notification.
        /// Note that the index object may be null if it has not been set
        /// on the router handler.
        /// </summary>
        /// <return>
        /// Returns null to stop the propagation and a notification
        /// object otherwise.
        /// </return>
        Notification ProcessNotification ( Notification n, INotificationIndex index );
    }
}