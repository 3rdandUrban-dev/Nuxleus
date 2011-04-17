using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Permissions;
using System.Xml;

namespace Nuxleus.Process {
    public class ZorbaProcess : System.Diagnostics.Process {

        TextWriter m_logWriter;
        StreamReader m_output;

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public ZorbaProcess() {
            base.StartInfo.FileName = "zorba";

        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public ZorbaProcess(TextWriter logWriter) {
            m_logWriter = logWriter;
            base.StartInfo.FileName = "zorba";

        }

        public TextWriter LogWriter { get { return m_logWriter; } set { m_logWriter = value; } }
        public StreamReader Output { get { return m_output; } set { m_output = value; } }

        public void RunQuery(string args) {
            base.StartInfo.Arguments = args;
            base.StartInfo.RedirectStandardOutput = true;
            base.StartInfo.UseShellExecute = false;
            base.Start();
            m_output = base.StandardOutput;
            base.WaitForExit();
        }

        public XmlReader GetXmlReader() {
            return XmlReader.Create(new StringReader(m_output.ReadToEnd()));
        }

    }
}