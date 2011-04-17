using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ChatServiceServer
{
    [RunInstaller(true)]
    public partial class ChatServiceServerInstaller : Installer
    {
        public ChatServiceServerInstaller()
        {
            
            InitializeComponent();

            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "ChatServiceServer";

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);

        }
    }
}