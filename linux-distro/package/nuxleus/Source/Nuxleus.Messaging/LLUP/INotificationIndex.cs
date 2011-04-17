//
// INotificationIndex.cs : Base interface to be implemented by LLUP indexers
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.Collections.Generic;

namespace Nuxleus.Messaging.LLUP {
    public interface INotificationIndex {
        void Index ( Notification notification );
        void Deindex ( Notification notification );
        IList<Notification> Query ( string query );
    }
}