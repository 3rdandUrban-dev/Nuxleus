using System;
using System.IO;
using System.Threading;
using System.Reflection;
using Nuxleus.Ssh;
using Nuxleus.Core;
using Nuxleus.MetaData;

namespace Nuxleus
{

    public class Program
    {

        const string APP_NAME = "Nuxleus";
        const string VERSION_SERIES = "0.3.1.x";

        enum Command
        {
            [Label("SSH Remote Execution")]
            NUX_SSH_REMOTE_EXECUTION = 1,
            [Label("Start Local Web Server")]
            NUX_START_WEB_SERVER,
            [Label("Manage Master Account")]
            NUX_MANAGE_MASTER_ACCOUNTS,
            [Label("Manage Users")]
            NUX_MANAGE_USERS,
            [Label("Backup System Files")]
            NUX_BACKUP_SYSTEM_FILES,
            [Label("Version")]
            NUX_VERSION,
            [Label("Exit")]
            NUX_EXIT
        }

        static void Main(string[] args)
        {
            PrintVersion(Console.Out);

            while (true)
            {
                PrintCommands();

            INPUT:
                int i = -1;
                Console.Write("Please enter your choice: ");
                try
                {
                    string input = Console.ReadLine();
                    if (input == "")
                        return;
                    i = int.Parse(input);
                    Console.WriteLine();
                }
                catch
                {
                    i = -1;
                }

                switch (i)
                {
                    case (int)Command.NUX_SSH_REMOTE_EXECUTION:
                        SshRemoteExecution.RunExample();
                        break;
                    //case (int)Command.NUX_START_WEB_SERVER:
                    //    WebServer server = new WebServer();
                    //    server.SetPort(9999);
                    //    server.SetRoot("BaseWebApp");
                    //    server.Start();
                    //    break;
                    case (int)Command.NUX_VERSION:
                        PrintVersion(Console.Out);
                        break;
                    case (int)Command.NUX_EXIT:
                        return;
                    default:
                        Console.Write("Bad input, ");
                        goto INPUT;
                }
            }
        }

        public static string[] GetArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Console.Write("Enter {0}: ", args[i]);
                args[i] = Console.ReadLine();
            }
            return args;
        }
        private static void PrintCommands()
        {
            Console.WriteLine();
            Console.WriteLine("Available Commands:");
            Console.WriteLine("=============");
            foreach (Command command in Enum.GetValues(typeof(Command)))
            {
                Console.WriteLine("{0})\t{1}",
                    (int)command,
                    LabelAttribute.FromMember(command)
                    );

            }
            Console.WriteLine();
        }
        private static void PrintVersion(TextWriter writer)
        {
            writer.WriteLine("{0}-{1}", APP_NAME, GetVersion());
        }
        static string GetVersion()
        {
            try
            {
                System.Reflection.Assembly asm
                    = System.Reflection.Assembly.GetAssembly(typeof(Nuxleus.Program));
                return asm.GetName().Version.ToString();
            }
            catch
            {
                return VERSION_SERIES;
            }
        }
    }
}
