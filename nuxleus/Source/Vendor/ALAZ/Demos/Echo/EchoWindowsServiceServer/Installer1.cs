using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace EchoWindowsServiceServer
{
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        
        public Installer1()
        {

            InitializeComponent();

            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.User;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "EchoWindowsServiceServer";

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);

        }
    }
}