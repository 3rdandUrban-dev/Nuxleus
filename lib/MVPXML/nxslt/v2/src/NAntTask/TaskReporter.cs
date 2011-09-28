using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Text;
using NAnt.Core;
using XmlLab.nxslt;

namespace XmlLab.NxsltTasks.NAnt
{
    /// <summary>
    /// nxslt task reporter class.
    /// </summary>
    internal class TaskReporter : Reporter
    {
        private Task task;
        public TaskReporter(Task task) {
            this.task = task;
        }

        /// <summary>
        /// Reports command line parsing error.
        /// </summary>        
        /// <param name="msg">Error message</param>
        public override void ReportCommandLineParsingError(string msg)
        {            
            task.Log(Level.Error, NXsltStrings.ErrorCommandLineParsing + "\n" + msg);                        
        }

        /// <summary>
        /// Reports an error.
        /// </summary>        
        /// <param name="msg">Error message</param>
        public override void ReportError(string msg)
        {            
            task.Log(Level.Error, msg);         
        }

        /// <summary>
        /// Reports an error.
        /// </summary>        
        /// <param name="msg">Error message</param>
        /// <param name="arg">Message argument</param>
        public override void ReportError(string msg, params string[] args)
        {            
            task.Log(Level.Error, string.Format(msg, args));            
        }

        /// <summary>
        /// Reports command line parsing error.
        /// </summary>        
        /// <param name="msg">Error message</param>
        /// <param name="arg">Message argument</param>
        public override void ReportCommandLineParsingError(string msg, params string[] args)
        {            
            task.Log(Level.Error, NXsltStrings.ErrorCommandLineParsing + "\n" + 
                string.Format(msg, args));            
        }

        /// <summary>
        /// Prints nxslt usage info.
        /// </summary>        
        public override void ReportUsage()
        {
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            task.Log(Level.Info, string.Format(NXsltStrings.UsageHeader,
              ver.Major, ver.Minor, ver.Build,
              System.Environment.Version.Major, System.Environment.Version.Minor,
              System.Environment.Version.Build, System.Environment.Version.Revision));            
            task.Log(Level.Info, NXsltStrings.UsageBody);
        }

        /// <summary>
        /// Prints timing info.
        /// </summary>   
        public override void ReportTimings(ref NXsltTimings timings)
        {            
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            task.Log(Level.Info, string.Format(NXsltStrings.UsageHeader,
              ver.Major, ver.Minor, ver.Build,
              System.Environment.Version.Major, System.Environment.Version.Minor,
              System.Environment.Version.Build, System.Environment.Version.Revision));            
            task.Log(Level.Info, string.Format(NXsltStrings.Timings, timings.XsltCompileTime,
              timings.XsltExecutionTime, timings.TotalRunTime));
        }        
    }
}
