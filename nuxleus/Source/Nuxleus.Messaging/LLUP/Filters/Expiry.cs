//
// Expiry.cs: LLUP Router filter based on the Expires element value of a notification
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 
using System;
using Nuxleus.Messaging;

namespace Nuxleus.Messaging.LLUP
{
    /// </summary>
    /// Simple filter that slect upon the expiry date of a notification to determine
    /// if it can be propagated.
    /// </summary>
    public class FilterByExpireDate : IRouterFilter
    {

        private double hoursToAdd = 0;
        private DateTime limitDate = DateTime.MinValue;

        public Notification ProcessNotification(Notification n, INotificationIndex index)
        {
            DateTime ExpiryLimit = limitDate;

            if (hoursToAdd > 0.0)
            {
                ExpiryLimit = (DateTime.Now).AddHours(hoursToAdd);
            }

            if (n.Expires.Value < ExpiryLimit)
            {
                return n;
            }

            return null;
        }

        /// <summary>
        /// Gets or sets the number of hours to add each time the processing takes place
        /// to determine the current expiry limit.
        /// In other words, this is a moving expiry date that checks that the notification
        /// is in the defined range.
        /// If you do not set this value, this will not be used during the processing.
        /// </summary>
        public double HoursToAdd
        {
            get { return hoursToAdd; }
            set { hoursToAdd = value; }
        }

        /// <summary>
        /// Gets or sets the fixed expiry date. In this case, one the provided date
        /// has passed, this filter will reject all notifications.
        /// </summary>
        public DateTime FixedLimit
        {
            get { return limitDate; }
            set { limitDate = value; }
        }
    }
}
