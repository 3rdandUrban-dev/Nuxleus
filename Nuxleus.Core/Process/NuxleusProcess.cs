using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Permissions;
using System.Xml;

namespace Nuxleus.Core
{
    public class NuxleusProcess : System.Diagnostics.Process {

        StreamReader m_output;

        //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public NuxleusProcess() {
            base.StartInfo.FileName = "SdbSOAP_Test.exe";

        }

        //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public NuxleusProcess(string processName)
        {
            base.StartInfo.FileName = processName;

        }

        public StreamReader Output { get { return m_output; } set { m_output = value; } }

        public void RunProcess(string args) {
            System.Console.WriteLine("Starting Process with: {0}", args);
            base.StartInfo.Arguments = args;
            //base.StartInfo.RedirectStandardOutput = true;
            //base.StartInfo.UseShellExecute = false;
            base.Start();
            base.WaitForExit();
        }

    }
}