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
using System.IO;
using System.Reflection;
using System.Threading;

namespace MPAPI
{
    public enum LogLevel
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 4,
        InfoWarningError = 7, //Info, Warning and Error
        Debug = 8,
        DebugCore = 16,
        All = 31
    }

    public enum LogType
    {
        None = 0,
        Console = 1,
        File = 2,
        Both = 3
    }

    public sealed class Log
    {
        private static object _logTypeLock = new object();
        private static object _logLevelLock = new object();
        private static object _fsLock = new object();
        private static object _writerLock = new object();
        private static LogType _logType = LogType.Console;
        private static LogLevel _logLevel = LogLevel.InfoWarningError;
        private static FileStream _fs;
        private static StreamWriter _writer;

        private Log()
        {
            OpenLogFile();
        }

        public static LogType LogType
        {
            get
            {
                lock (_logTypeLock)
                {
                    return _logType;
                }
            }
            set
            {
                lock (_logTypeLock)
                {
                    _logType = value;
                    if ((_logType & LogType.File) != LogType.File)
                        CloseLogFile();
                    else
                        OpenLogFile();
                }
            }
        }

        public static LogLevel LogLevel
        {
            get
            {
                lock (_logLevelLock)
                {
                    return _logLevel;
                }
            }
            set
            {
                lock (_logLevelLock)
                {
                    _logLevel = value;
                }
            }
        }

        public static void Info(string formattedMessage, params object[] arguments)
        {
            lock (_logLevelLock)
            {
                if ((_logLevel & LogLevel.Info) == LogLevel.Info)
                    LogMessage("Info", formattedMessage, arguments);
            }
        }

        public static void Warning(string formattedMessage, params object[] arguments)
        {
            lock (_logLevelLock)
            {
                if ((_logLevel & LogLevel.Warning) == LogLevel.Warning)
                    LogMessage("Warning", formattedMessage, arguments);
            }
        }

        public static void Error(string formattedMessage, params object[] arguments)
        {
            lock (_logLevelLock)
            {
                if ((_logLevel & LogLevel.Error) == LogLevel.Error)
                    LogMessage("Error", formattedMessage, arguments);
            }
        }

        public static void Debug(string formattedMessage, params object[] arguments)
        {
            lock (_logLevelLock)
            {
                if ((_logLevel & LogLevel.Debug) == LogLevel.Debug)
                    LogMessage("Debug", formattedMessage, arguments);
            }
        }

        //internal static void DebugCore(string formattedMessage, params object[] arguments)
        //{
        //    lock (_logLevelLock)
        //    {
        //        if ((_logLevel & LogLevel.DebugCore) == LogLevel.DebugCore)
        //            LogMessage("DebugCore", formattedMessage, arguments);
        //    }
        //}

        private static void LogMessage(string header, string formattedMessage, params object[] arguments)
        {
            DateTime now = DateTime.Now;
            string msg = string.Format("{0}:{1} | {2} | {3}", now.ToString("HH:mm:ss"), now.Millisecond.ToString().PadLeft(3,'0'), header.PadRight(9,' '), string.Format(formattedMessage, arguments));
            lock (_logTypeLock)
            {
                if ((_logType & LogType.Console) == LogType.Console)
                    Console.WriteLine(msg);
                if ((_logType & LogType.File) == LogType.File)
                {
                    lock (_writerLock)
                    {
                        _writer.WriteLine(msg);
                        _writer.Flush();
                    }
                }
            }
        }

        private static void CloseLogFile()
        {
            if (_writer != null)
            {
                lock (_writerLock)
                {
                    _writer.Flush();
                    _writer.Close();
                    _writer = null;
                }
            }
            if (_fs != null)
            {
                lock (_fsLock)
                {
                    _fs.Close();
                    _fs = null;
                }
            }
        }

        private static void OpenLogFile()
        {
            lock (_logTypeLock)
            {
                if ((_logType & LogType.File) == LogType.File)
                {
                        string logFileName = Path.GetFileName(Assembly.GetEntryAssembly().CodeBase) + ".log";
                        string executingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().CodeBase);
                        executingDir = executingDir.Replace(@"file:\", "");
                        lock (_fsLock)
                        {
                            _fs = File.Open(Path.Combine(executingDir, logFileName), FileMode.Append, FileAccess.Write);
                        }
                        lock (_writerLock)
                        {
                            _writer = new StreamWriter(_fs);
                        }
                }
            }
        }
    }
}
