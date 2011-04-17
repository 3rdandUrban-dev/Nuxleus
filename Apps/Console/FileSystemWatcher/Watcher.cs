using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Permissions;
using Nuxleus.Service.FileSystem;

namespace Nuxleus
{
    public class FileSystemWatcher
    {
        static Watcher _fileSystemWatcher;

        public static void Main()
        {
            string[] args = System.Environment.GetCommandLineArgs();

            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Watcher.exe (directory)");
                return;
            }
            Console.WriteLine(args[1]);
            Run(_fileSystemWatcher = (new Watcher(args[1], "", Console.Out)));
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Run(Watcher fileSystemWatcher)
        {
            fileSystemWatcher.Watch(true);
            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;
        }

    }
}
