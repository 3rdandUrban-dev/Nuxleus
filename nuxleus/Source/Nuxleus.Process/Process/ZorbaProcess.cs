using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Permissions;

namespace Nuxleus.Process
{
    public class ZorbaProcess : System.Diagnostics.Process
    {
        TextWriter m_logWriter;
        ZorbaProcess m_proc;
        StreamReader m_output;

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public ZorbaProcess()
        {
            base.StartInfo.FileName = "zorba";

        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public ZorbaProcess(string parameters, TextWriter logWriter)
        {
            m_logWriter = logWriter;
            m_proc = new ZorbaProcess();
            m_proc.StartInfo.FileName = "zorba";

        }

        public TextWriter LogWriter { get { return m_logWriter; } set { m_logWriter = value; } }


        public void RunQuery(string fullPath)
        {
            this.StartInfo.Arguments = "to be added";
            this.Start();
            m_output = this.StandardOutput;
            this.WaitForExit();
        }
    }
}