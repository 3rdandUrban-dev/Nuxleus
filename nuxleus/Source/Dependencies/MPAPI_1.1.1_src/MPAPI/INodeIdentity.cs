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
    public interface INodeIdentity
    {
        /// <summary>
        /// Gets the id of the node.
        /// </summary>
        /// <returns></returns>
        ushort GetId();
    }
}
