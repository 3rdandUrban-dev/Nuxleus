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
using RemotingLite;

namespace MPAPI.RegistrationServer
{
    public class RegistrationServerBootstrap : IDisposable
    {
        private ServiceHost _host;
        private RegistrationServer _registrationServer;

        public void Open(int port)
        {
            Log.LogLevel = LogLevel.InfoWarningError;
            Log.LogType = LogType.Console;
            _registrationServer = new RegistrationServer();
            
            _host = new ServiceHost(_registrationServer, port);
            _host.Open();
            Console.Title = "Registration server";
            Log.Info("Registration server is running.");
            Console.ReadLine();
            Log.Info("Registration server terminated.");
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_host != null)
                _host.Dispose();

            _registrationServer.Dispose();
        }

        #endregion
    }
}
