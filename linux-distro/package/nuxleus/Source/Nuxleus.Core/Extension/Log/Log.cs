using System;
using System.Security.Permissions;
using log4net;
using log4net.Config;

namespace Nuxleus.Extension {

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.AllFlags)]
    public static class Log {

        static bool IsConfigured;

        public static ILog GetLogger(this object obj) {
            if (!IsConfigured)
                Config();
            return LogManager.GetLogger(obj.GetType());
        }

        public static ILog GetLogger<T>() {
            if (!IsConfigured)
                Config();
            return LogManager.GetLogger(typeof(T));
        }

        private static ILog Logger(object obj) {
            if(!IsConfigured) 
                Config();
            return LogManager.GetLogger(obj.GetType());
        }

        private static ILog Logger<T>() {
            if (!IsConfigured)
                Config();
            return LogManager.GetLogger(typeof(T));
        }

        public static void Config(string logFile) {
            XmlConfigurator.Configure(new System.IO.FileInfo((logFile)));
            IsConfigured = true;
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

        public static void LogDebug<T>(object message, Exception exception) {
            Logger<T>().Debug(message, exception);
        }

        public static void LogDebug<T>(object message) {
            Logger<T>().Debug(message);
        }

        public static void LogDebug<T>(IFormatProvider provider, string format, params object[] args) {
            Logger<T>().DebugFormat(provider, format, args);
        }

        public static void LogDebug<T>(string format, params object[] args) {
            Logger<T>().DebugFormat(format, args);
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

        public static void LogError<T>(object message, Exception exception) {
            Logger<T>().Error(message, exception);
        }

        public static void LogError<T>(object message) {
            Logger<T>().Error(message);
        }

        public static void LogError<T>(IFormatProvider provider, string format, params object[] args) {
            Logger<T>().ErrorFormat(provider, format, args);
        }

        public static void LogError<T>(string format, params object[] args) {
            Logger<T>().ErrorFormat(format, args);
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

        public static void LogFatal<T>(object message, Exception exception) {
            Logger<T>().Fatal(message, exception);
        }

        public static void LogFatal<T>(object message) {
            Logger<T>().Fatal(message);
        }

        public static void LogFatal<T>(IFormatProvider provider, string format, params object[] args) {
            Logger<T>().FatalFormat(provider, format, args);
        }

        public static void LogFatal<T>(string format, params object[] args) {
            Logger<T>().FatalFormat(format, args);
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

        public static void LogInfo<T>(object message, Exception exception) {
            Logger<T>().Info(message, exception);
        }

        public static void LogInfo<T>(object message) {
            Logger<T>().Info(message);
        }

        public static void LogInfo<T>(IFormatProvider provider, string format, params object[] args) {
            Logger<T>().InfoFormat(provider, format, args);
        }

        public static void LogInfo<T>(string format, params object[] args) {
            Logger<T>().InfoFormat(format, args);
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

        public static void LogWarn<T>(object message, Exception exception) {
            Logger<T>().Warn(message, exception);
        }

        public static void LogWarn<T>(object message) {
            Logger<T>().Warn(message);
        }

        public static void LogWarn<T>(IFormatProvider provider, string format, params object[] args) {
            Logger<T>().WarnFormat(provider, format, args);
        }

        public static void LogWarn<T>(string format, params object[] args) {
            Logger<T>().WarnFormat(format, args);
        }
    }
}
