/*****************************************************************
 * Registration Server
 * Distributed as part of MPAPI - Message Passing API
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
using MPAPI.RegistrationServer;
using System.Configuration;

namespace RegistrationServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port;
            string sPort = ConfigurationManager.AppSettings["Port"];
            if (!int.TryParse(sPort, out port))
            {
                Console.WriteLine(string.Format("Cannot parse '{0}' as an integer", sPort));
                return;
            }
            RegistrationServerBootstrap registrationServer = new RegistrationServerBootstrap();
            registrationServer.Open(port);
        }
    }
}
