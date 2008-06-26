using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using log4net.Core;

namespace Nuxleus.Extension.Log {
    public static class Log {

        static bool IsConfigured;

        public static ILog GetLogger(this object obj) {
            if (!IsConfigured)
                Config();
            return LogManager.GetLogger(obj.GetType());
        }

        private static ILog Logger(object obj) {
            if(!IsConfigured) 
                Config();
            return LogManager.GetLogger(obj.GetType());
        }

        private static void Config() {
            XmlConfigurator.Configure(new System.IO.FileInfo(("log4net.config")));
            IsConfigured = true;
        }

        public static void LogDebug(this object obj, object message, Exception exception) {
            Logger(obj).Debug(message, exception);
        }

        public static void LogDebug(this object obj, object message) {
            Logger(obj).Debug(message);
        }

        public static void LogDebug(this object obj, IFormatProvider provider, string format, params object[] args) {
            Logger(obj).DebugFormat(provider, format, args);
        }

        public static void LogDebug(this object obj, string format, params object[] args) {
            Logger(obj).DebugFormat(format, args);
        }

        public static void LogError(this object obj, object message, Exception exception) {
            Logger(obj).Error(message, exception);
        }

        public static void LogError(this object obj, object message) {
            Logger(obj).Error(message);
        }

        public static void LogError(this object obj, IFormatProvider provider, string format, params object[] args) {
            Logger(obj).ErrorFormat(provider, format, args);
        }

        public static void LogError(this object obj, string format, params object[] args) {
            Logger(obj).ErrorFormat(format, args);
        }

        public static void LogFatal(this object obj, object message, Exception exception) {
            Logger(obj).Fatal(message, exception);
        }

        public static void LogFatal(this object obj, object message) {
            Logger(obj).Fatal(message);
        }

        public static void LogFatal(this object obj, IFormatProvider provider, string format, params object[] args) {
            Logger(obj).FatalFormat(provider, format, args);
        }

        public static void LogFatal(this object obj, string format, params object[] args) {
            Logger(obj).FatalFormat(format, args);
        }

        public static void LogInfo(this object obj, object message, Exception exception) {
            Logger(obj).Info(message, exception);
        }

        public static void LogInfo(this object obj, object message) {
            Logger(obj).Info(message);
        }

        public static void LogInfo(this object obj, IFormatProvider provider, string format, params object[] args) {
            Logger(obj).InfoFormat(provider, format, args);
        }

        public static void LogInfo(this object obj, string format, params object[] args) {
            Logger(obj).InfoFormat(format, args);
        }

        public static void LogWarn(this object obj, object message, Exception exception) {
            Logger(obj).Warn(message, exception);
        }

        public static void LogWarn(this object obj, object message) {
            Logger(obj).Warn(message);
        }

        public static void LogWarn(this object obj, IFormatProvider provider, string format, params object[] args) {
            Logger(obj).WarnFormat(provider, format, args);
        }

        public static void LogWarn(this object obj, string format, params object[] args) {
            Logger(obj).WarnFormat(format, args);
        }

    }
}
