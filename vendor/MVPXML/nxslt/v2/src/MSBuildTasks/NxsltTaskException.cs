using System;
using System.Collections.Generic;
using System.Text;

namespace XmlLab.NxsltTasks.MSBuild
{
    public class NxsltTaskException : Exception
    {
        public NxsltTaskException(string msg) :base(msg)
        {}

        public NxsltTaskException(string msg, Exception e)
            : base(msg, e)
        { }
    }
}
