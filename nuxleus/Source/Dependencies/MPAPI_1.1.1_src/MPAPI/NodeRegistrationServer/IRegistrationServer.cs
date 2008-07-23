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
using System.Net;

namespace MPAPI.RegistrationServer
{
    public interface IRegistrationServer
    {
        bool RegisterNode(IPEndPoint nodeEndPoint);

        void UnregisterNode(ushort nodeId);

        List<IPEndPoint> GetAllNodeEndPoints();
    }
}
