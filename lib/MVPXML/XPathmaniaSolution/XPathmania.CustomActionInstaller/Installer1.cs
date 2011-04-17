using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace XmlMvp.XPathmania
{
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }

        [SecurityPermissionAttribute(SecurityAction.Demand)]
        public override void Commit(System.Collections.IDictionary savedState)
        {
            base.Commit(savedState);

            Assembly installerAssembly = Assembly.GetExecutingAssembly();
            string installerLocation = installerAssembly.Location;
            string appPath = Path.GetDirectoryName(installerLocation);

            //ProcessStartInfo startInfo = new ProcessStartInfo(System.IO.Path.Combine(appPath, "regpkg.exe"));
            //startInfo.Arguments = @" /codebase " + installerLocation;
            //startInfo.UseShellExecute = true;
            //startInfo.CreateNoWindow = true;
            //Process.Start(startInfo);

            string vs8Path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\8.0\Setup\VS\", "EnvironmentPath", string.Empty);

            ProcessStartInfo startInfo = new ProcessStartInfo(vs8Path);
            startInfo.Arguments = @" /setup";
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
            Process.Start(startInfo);
        }

        [SecurityPermissionAttribute(SecurityAction.Demand)]
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);

            Assembly installerAssembly = Assembly.GetExecutingAssembly();
            string installerLocation = installerAssembly.Location;
            string appPath = Path.GetDirectoryName(installerLocation);

            //ProcessStartInfo startInfo = new ProcessStartInfo(System.IO.Path.Combine(appPath, "regpkg.exe"));
            //startInfo.Arguments = @" /unregister " + installerLocation;
            //startInfo.UseShellExecute = true;
            //startInfo.CreateNoWindow = true;
            //Process.Start(startInfo);

            string vs8Path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\8.0\Setup\VS\", "EnvironmentPath", string.Empty);

            ProcessStartInfo startInfo = new ProcessStartInfo(vs8Path);
            startInfo.Arguments = @" /setup";
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
            Process.Start(startInfo);
        }
    }
}