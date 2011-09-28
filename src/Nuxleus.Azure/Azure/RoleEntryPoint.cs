using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Azure.Toolkit.Azure
{
    public abstract class AzureRoleEntryPoint : RoleEntryPoint
    {
        protected AzureRoleEntryPoint()
        {
            StayAlive = true;
        }

        public bool StayAlive { get; set; }

        protected virtual void Configure(DiagnosticMonitorConfiguration config)
        {
            config.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(5.0);
            config.DiagnosticInfrastructureLogs.ScheduledTransferLogLevelFilter = LogLevel.Error;
            config.DiagnosticInfrastructureLogs.ScheduledTransferPeriod = TimeSpan.FromMinutes(5);
            config.Logs.ScheduledTransferLogLevelFilter = LogLevel.Error;
            config.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(5);
            config.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(5);
            config.WindowsEventLog.ScheduledTransferLogLevelFilter = LogLevel.Error;
            config.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromMinutes(5);
            config.WindowsEventLog.DataSources.Add("Application!*");
            config.WindowsEventLog.DataSources.Add("System!*"); 
        }


        public override bool OnStart()
        {
            StartDiagnostics();

            // This code sets up a handler to update CloudStorageAccount instances when their corresponding
            // configuration settings change in the service configuration file.
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                // Provide the configSetter with the initial value
                configSetter(
                    RoleEnvironment.
                        GetConfigurationSettingValue(configName));

                RoleEnvironment.Changed += (sender, arg) =>
                {
                    if (arg.
                        Changes.
                        OfType
                        <
                            RoleEnvironmentConfigurationSettingChange
                            >()
                        .Any(
                        change
                        =>
                        (change.
                             ConfigurationSettingName ==
                         configName)))
                    {
                        // The corresponding configuration setting has changed, propagate the value
                        if (
                            !configSetter
                                 (RoleEnvironment
                                      .
                                      GetConfigurationSettingValue
                                      (configName)))
                        {
                            // In this case, the change to the storage account credentials in the
                            // service configuration is significant enough that the role needs to be
                            // recycled in order to use the latest settings. (for example, the 
                            // endpoint has changed)
                            RoleEnvironment
                                .
                                RequestRecycle
                                ();
                        }
                    }
                };
            });

            return base.OnStart();
        }

        public override void Run()
        {
            base.Run();
            while (StayAlive)
            {
                Thread.Sleep(10000);
            }
        }

        void StartDiagnostics()
        {
            var config = DiagnosticMonitor.GetDefaultInitialConfiguration();
            Configure(config);
            DiagnosticMonitor.AllowInsecureRemoteConnections = true;
            DiagnosticMonitor.Start("DiagnosticsConnectionString", config);
        }

        protected void StartProcessor<T>(MessageProcessor<T> messageProcessor) where T : class, new()
        {
            var messageProcessorThread = new Thread(messageProcessor.ProcessMessages);
            messageProcessorThread.Start();
        }
    }
}
