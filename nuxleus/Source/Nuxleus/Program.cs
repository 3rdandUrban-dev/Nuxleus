using System;
using System.IO;
using System.Threading;
using System.Reflection;
using Nuxleus.Agent;
using Nuxleus.Ssh;

namespace Nuxleus {

    public class Program {

        const int NUX_SSH_REMOTE_EXECUTION = 1;
        const int NUX_VERSION = 2;
        const int NUX_EXIT = 3;

        static void Main(string[] args) {
            while (true) {
                PrintVersion();
                Console.WriteLine();
                Console.WriteLine("Available Commands:");
                Console.WriteLine("=============");
                Console.WriteLine("{0})\tRemote SSH Execution", NUX_SSH_REMOTE_EXECUTION);
                Console.WriteLine("{0})\tVersion", NUX_VERSION);
                Console.WriteLine("{0})\tExit", NUX_EXIT);
                Console.WriteLine();

            INPUT:
                int i = -1;
                Console.Write("Please enter your choice: ");
                try {
                    string input = Console.ReadLine();
                    if (input == "")
                        return;
                    i = int.Parse(input);
                    Console.WriteLine();
                } catch {
                    i = -1;
                }

                switch (i) {
                    case NUX_SSH_REMOTE_EXECUTION:
                        SshRemoteExecution.RunExample();
                        break;
                    case NUX_VERSION:
                        PrintVersion();
                        break;
                    case NUX_EXIT:
                        return;
                    default:
                        Console.Write("Bad input, ");
                        goto INPUT;
                }
            }
        }

        public static string[] GetArgs(string[] args) {
            for (int i = 0; i < args.Length; i++) {
                Console.Write("Enter {0}: ", args[i]);
                args[i] = Console.ReadLine();
            }
            return args;
        }

        private static void PrintVersion() {
            try {
                System.Reflection.Assembly asm
                    = System.Reflection.Assembly.GetAssembly(typeof(Nuxleus.Program));
                Console.WriteLine("Nuxleus-" + asm.GetName().Version);
            } catch {
                Console.WriteLine("Nuxleus v0.2.5.7");
            }
        }
    }
}