/*****************************************************************
 * MPAPI - Message Passing API
 * A framework for writing parallel and distributed applications
 * 
 * Author   : Frank Thomsen
 * Web      : http://sector0.dk
 * Contact  : mpapi@sector0.dk
 * License  : New BSD licence
 * 
 * Copyright (c) 2008, Frank Thomsen
 * 
 * Feel free to contact me with bugs and ideas.
 *****************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace MPAPI
{
    /// <summary>
    /// This class contains constants used when the system is sending messages.
    /// </summary>
    public class SystemMessages
    {
        /// <summary>
        /// A worker thread terminated normally.
        /// </summary>
        public const int WorkerTerminated = -1;

        /// <summary>
        /// A worker terminated abnormally. The message content contains the exception that caused this.
        /// </summary>
        public const int WorkerTerminatedAbnormally = -2;

        public static bool IsSystemMessageType(int messageType)
        {
            return messageType < 0;
        }
    }
}
