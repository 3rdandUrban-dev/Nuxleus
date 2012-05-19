using System;
using System.Configuration;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;

namespace Nuxleus.Core.Net
{
    internal class ConfigurationUtility
    {
        static ConfigurationUtility() { }

        internal static readonly ConfigurationUtility Instance = SingletonProvider<ConfigurationUtility>.Instance;
        private static readonly HttpRuntimeConfiguration @httpRuntimeConfiguration = SingletonProvider<HttpRuntimeConfiguration>.Instance;
        private static readonly ServicePointManagerConfiguration @servicePointManagerConfiguration = SingletonProvider<ServicePointManagerConfiguration>.Instance;
        private static readonly ThreadPoolConfiguration @threadPoolConfiguration = SingletonProvider<ThreadPoolConfiguration>.Instance;
        private static readonly System.Configuration.Configuration @configurationInstance = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private static readonly HttpRuntimeSection @httpRuntimeSection = (HttpRuntimeSection)ConfigurationInstance.GetSection("system.web/httpRuntime");

        internal static HttpRuntimeConfiguration HttpRuntimeConfigurationInstance { get { return @httpRuntimeConfiguration; } }
        internal static ServicePointManagerConfiguration ServicePointManagerConfigurationInstance { get { return @servicePointManagerConfiguration; } }
        internal static ThreadPoolConfiguration ThreadPoolConfigurationInstance { get { return @threadPoolConfiguration; } }
        internal static System.Configuration.Configuration ConfigurationInstance { get { return @configurationInstance; } }
        internal static HttpRuntimeSection HttpRuntimeSection { get { return @httpRuntimeSection; } }

        private static readonly ConfigurationUtility @this = ConfigurationUtility.Instance;

        internal class HttpRuntimeConfiguration
        {
            private static readonly HttpRuntimeConfiguration @this = HttpRuntimeConfigurationInstance;

            private const int m_executionTimeout = 300;
            private const int m_minimumFreeThreads = 176;
            private const int m_minimumLocalRequestFreeThreads = 152;

            internal int? ExecutionTimeout { get; set; }
            internal int? MinimumFreeThreads { get; set; }
            internal int? MinimumLocalRequestFreeThreads { get; set; }

            static HttpRuntimeConfiguration()
            {
                @this.ExecutionTimeout = ((HttpRuntimeSection.ExecutionTimeout == default(TimeSpan)) ? m_executionTimeout : HttpRuntimeSection.MinFreeThreads);
                @this.MinimumFreeThreads = ((HttpRuntimeSection.MinFreeThreads == default(int)) ? m_minimumFreeThreads : HttpRuntimeSection.MinFreeThreads);
                @this.MinimumLocalRequestFreeThreads = ((HttpRuntimeSection.MinLocalRequestFreeThreads == default(int)) ? m_minimumLocalRequestFreeThreads : HttpRuntimeSection.MinLocalRequestFreeThreads);
            }
        }

        internal class ServicePointManagerConfiguration
        {
            private static readonly ServicePointManagerConfiguration @this = ServicePointManagerConfigurationInstance;

            private const int m_defaultConnectionLimit = 100;
            private const bool m_enableDnsRoundRobin = false;
            private const int m_maxServicePoints = 20;
            private const int m_maxServicePointIdleTime = 10000;

            internal int DefaultConnectionLimit { get; set; }
            internal bool EnableDnsRoundRobin { get; set; }
            internal int MaxServicePoints { get; set; }
            internal int MaxServicePointIdleTime { get; set; }
            internal RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }

            static ServicePointManagerConfiguration()
            {
                @this.DefaultConnectionLimit = (GetIntFromAppSettings("DefaultConnectionLimit", m_defaultConnectionLimit));
                @this.EnableDnsRoundRobin = (GetBoolFromAppSettings("EnableDnsRoundRobin", m_enableDnsRoundRobin));
                @this.MaxServicePoints = (GetIntFromAppSettings("MaxServicePoints", m_maxServicePoints));
                @this.MaxServicePointIdleTime = (GetIntFromAppSettings("MaxServicePointIdleTime", m_maxServicePointIdleTime));
                @this.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
            }
        }

        internal class ThreadPoolConfiguration
        {
            private static readonly ThreadPoolConfiguration @this = ThreadPoolConfigurationInstance;

            private const int m_maximumWorkerThreads = 100;
            private const int m_maximumAsyncIOThreads = 100;
            private const int m_minimumWorkerThreads = 25;
            private const int m_minimumAsyncIOThreads = 25;

            internal int MaxiumWorkerThreads { get; set; }
            internal int MaximumAsyncIOThreads { get; set; }
            internal int MinimumWorkerThreads { get; set; }
            internal int MinimumAsyncIOThreads { get; set; }

            static ThreadPoolConfiguration()
            {
                @this.MaxiumWorkerThreads = (GetIntFromAppSettings("MaximumWorkerThreads", m_maximumWorkerThreads));
                @this.MaximumAsyncIOThreads = (GetIntFromAppSettings("MaximumAsyncIOThreads", m_maximumAsyncIOThreads));
                @this.MinimumWorkerThreads = (GetIntFromAppSettings("MaxServicePoints", m_minimumWorkerThreads));
                @this.MinimumAsyncIOThreads = (GetIntFromAppSettings("MaxServicePointIdleTime", m_minimumAsyncIOThreads));
            }
        }

        internal static bool GetBoolFromAppSettings(string appSetting, bool defaultValue)
        {
            bool resultValue, returnValue = ((bool.TryParse(ConfigurationManager.AppSettings[appSetting], out resultValue)) == true) ? resultValue : defaultValue;
            @this.LogInfo("{0} {1} a configuration file entry. The value has been set to {2}.", appSetting, ((resultValue == default(bool)) ? "did not have" : "had"), resultValue);
            ConfigurationManager.AppSettings[appSetting] = returnValue.ToString();
            return returnValue;
        }

        internal static int GetIntFromAppSettings(string appSetting, int defaultValue)
        {
            int resultValue, returnValue = ((int.TryParse(ConfigurationManager.AppSettings[appSetting], out resultValue)) == true) ? resultValue : defaultValue;
            @this.LogInfo("{0} {1} a configuration file entry. The value has been set to {2}.", appSetting, ((resultValue == default(int)) ? "did not have" : "had"), resultValue);
            ConfigurationManager.AppSettings[appSetting] = returnValue.ToString();
            return returnValue;
        }

        internal static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //Temporary hack
            return true;
        }
    }
}
