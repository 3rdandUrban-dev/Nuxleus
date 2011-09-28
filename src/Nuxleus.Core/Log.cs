using System;
using System.IO;
using System.Text;

namespace Nuxleus.Logging
{

    public class Log
    {
        private static string _logPath;

        static Log ()
        {
            _logPath = Path.Combine (
                Path.GetDirectoryName (typeof(Log).Assembly.Location),
                "Test.log");
        }

        public static void Write (string msg)
        {
            using (FileStream fs = new FileStream(_logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter w = new StreamWriter(fs, System.Text.Encoding.Default)) {
                w.WriteLine ("{0:yyyy-MM-dd HH:mm:ss} - {1}", DateTime.Now, msg);
                w.Flush ();
            }
        }

        public static void Write (Exception ex)
        {
            StringBuilder message = new StringBuilder ();
            StringBuilder stacktrace = new StringBuilder ();

            for (Exception e = ex; e != null; e = e.InnerException) {
                if (message.Length > 0)
                    message.Append (Environment.NewLine);

                message.Append (e.Message);

                if (stacktrace.Length > 0) {
                    stacktrace.Append (Environment.NewLine);
                    stacktrace.Append ("----");
                }
                stacktrace.Append (e.StackTrace);
            }

            Write (message.ToString () + Environment.NewLine + stacktrace.ToString ());
        }
    }
}