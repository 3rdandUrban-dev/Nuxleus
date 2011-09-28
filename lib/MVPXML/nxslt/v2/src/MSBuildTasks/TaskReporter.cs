using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Text;
using XmlLab.nxslt;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace XmlLab.NxsltTasks.MSBuild
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
            task.Log.LogError(NXsltStrings.ErrorCommandLineParsing + "\n" + msg);                        
        }

        /// <summary>
        /// Reports an error.
        /// </summary>        
        /// <param name="msg">Error message</param>
        public override void ReportError(string msg)
        {            
            task.Log.LogError(msg);         
        }

        /// <summary>
        /// Reports an error.
        /// </summary>        
        /// <param name="msg">Error message</param>
        /// <param name="arg">Message argument</param>
        public override void ReportError(string msg, params string[] args)
        {            
            task.Log.LogError(string.Format(msg, args));            
        }

        /// <summary>
        /// Reports command line parsing error.
        /// </summary>        
        /// <param name="msg">Error message</param>
        /// <param name="arg">Message argument</param>
        public override void ReportCommandLineParsingError(string msg, params string[] args)
        {            
            task.Log.LogError(NXsltStrings.ErrorCommandLineParsing + "\n" + 
                string.Format(msg, args));            
        }

        /// <summary>
        /// Prints nxslt usage info.
        /// </summary>        
        public override void ReportUsage()
        {
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            task.Log.LogMessage(MessageImportance.Normal, string.Format(NXsltStrings.UsageHeader,
              ver.Major, ver.Minor, ver.Build,
              System.Environment.Version.Major, System.Environment.Version.Minor,
              System.Environment.Version.Build, System.Environment.Version.Revision));            
            task.Log.LogMessage(MessageImportance.Normal, NXsltStrings.UsageBody);
        }

        /// <summary>
        /// Prints timing info.
        /// </summary>   
        public override void ReportTimings(ref NXsltTimings timings)
        {            
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            task.Log.LogMessage(MessageImportance.Normal, string.Format(NXsltStrings.UsageHeader,
              ver.Major, ver.Minor, ver.Build,
              System.Environment.Version.Major, System.Environment.Version.Minor,
              System.Environment.Version.Build, System.Environment.Version.Revision));
            task.Log.LogMessage(MessageImportance.Normal, string.Format(NXsltStrings.Timings, timings.XsltCompileTime,
              timings.XsltExecutionTime, timings.TotalRunTime));
        }        
    }
}
