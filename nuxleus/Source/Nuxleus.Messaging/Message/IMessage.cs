//
// IMessage.cs: interface for the messages
//
// Author:
//   Sylvain Hellegouarch (sh@3rdandurban.com)
//
// Copyright (C) 2007, 3rd&Urban, LLC
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
        /// Actual message depending on the implementation of
        /// this interface.
        /// </summary>
        byte[] InnerMessage { get; set; }
    }
}