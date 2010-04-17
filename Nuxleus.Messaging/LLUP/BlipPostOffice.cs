//
// BlipPostOffice.cs: Inter services blip notifier
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 
using System;

namespace Nuxleus.Messaging.LLUP {
    public delegate void BlipPostedHandler ( Notification n );

    /// <summary>
    /// Each component in the LLUP network acts as a client to another component
    /// and server for other components to connect to.
    /// The implementation in Nuxleus leads to the client and server to
    /// be two different services that have no mean to communicate within
    /// the process.
    /// Therefore the BlipPostOffice. An instance of this class is shared
    /// between the client and server sides. When the client received a
    /// new blip, it posts it to the post-office. The server is then
    /// notified of such event and can start processing the notification.
    /// </summary>
    public class BlipPostOffice {
        public BlipPostOffice () { }

        /// <summary>
        /// Register delegates to be notified of a new notification
        /// to process.
        /// </summary>
        public event BlipPostedHandler Mailbox = null;

        /// <summary>
        /// Notify that a notification is ready to be processed.
        /// </summary>
        public void Post ( Notification n ) {
            if (Mailbox != null) {
                Mailbox(n);
            }
        }
    }
}