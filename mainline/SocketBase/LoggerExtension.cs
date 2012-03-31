﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.Common;
using SuperSocket.Common.Logging;

namespace SuperSocket.SocketBase
{
    /// <summary>
    /// Logger extension class
    /// </summary>
    public static class LoggerExtension
    {
        private readonly static string m_SessionInfoTemplate = "Session: {0}/{1}";

        /// <summary>
        /// Logs the error
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="session">The session.</param>
        /// <param name="e">The e.</param>
        public static void Error(this ILog logger, ISessionBase session, Exception e)
        {
            logger.Error(string.Format(m_SessionInfoTemplate, session.SessionID, session.RemoteEndPoint), e);
        }

        /// <summary>
        /// Logs the error
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="session">The session.</param>
        /// <param name="title">The title.</param>
        /// <param name="e">The e.</param>
        public static void Error(this ILog logger, ISessionBase session, string title, Exception e)
        {
            logger.Error(string.Format(m_SessionInfoTemplate, session.SessionID, session.RemoteEndPoint) + Environment.NewLine + title, e);
        }

        /// <summary>
        /// Logs the error
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="session">The session.</param>
        /// <param name="message">The message.</param>
        public static void Error(this ILog logger, ISessionBase session, string message)
        {
            logger.Error(string.Format(m_SessionInfoTemplate, session.SessionID, session.RemoteEndPoint) + Environment.NewLine + message);
        }

        /// <summary>
        /// Logs the information
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="session">The session.</param>
        /// <param name="message">The message.</param>
        public static void Info(this ILog logger, ISessionBase session, string message)
        {
            string info = string.Format(m_SessionInfoTemplate, session.SessionID, session.RemoteEndPoint) + Environment.NewLine + message;
            logger.Info(info);
        }

        /// <summary>
        /// Logs the debug message
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="session">The session.</param>
        /// <param name="message">The message.</param>
        public static void Debug(this ILog logger, ISessionBase session, string message)
        {
            if (!logger.IsDebugEnabled)
                return;

            logger.Debug(string.Format(m_SessionInfoTemplate, session.SessionID, session.RemoteEndPoint) + Environment.NewLine + message);
        }

        private const string m_PerfLogName = "Perf";

        /// <summary>
        /// Logs the performance message
        /// </summary>
        /// <param name="appServer">The app server.</param>
        /// <param name="message">The message.</param>
        public static void Perf(this IAppServer appServer, string message)
        {
            var perfLog = LogFactoryProvider.LogFactory.GetLog(m_PerfLogName);

            if (perfLog != null && perfLog.IsInfoEnabled)
                perfLog.Info(message);
        }
    }
}
