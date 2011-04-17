using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

using ALAZ.SystemEx;
using ALAZ.SystemEx.NetEx.SocketsEx;

using System.Windows.Forms;

namespace ChatClient
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmClient());
        }

    }

}
