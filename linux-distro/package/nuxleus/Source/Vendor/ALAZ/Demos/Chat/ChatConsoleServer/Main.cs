using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using ChatSocketService;
using ALAZ.SystemEx.NetEx.SocketsEx;

namespace ChatServer
{

    public class ChatServer
    {

        [STAThread]
        static void Main(string[] args)
        {

            //----- Set Threads!
            ThreadPool.SetMinThreads(4, 4);
            ThreadPool.SetMaxThreads(32, 32);
            
            SocketServer chatServer = new SocketServer(new ChatSocketService.ChatSocketService());

            chatServer.Delimiter = new byte[] { 0xAA, 0xFF, 0xAA };
            chatServer.DelimiterType = DelimiterType.dtMessageTailExcludeOnReceive;

            chatServer.SocketBufferSize = 1024;
            chatServer.MessageBufferSize = 512;
            
            //----- Socket Listener!
            SocketListener listener = chatServer.AddListener("Char Server", new IPEndPoint(IPAddress.Any, 8090));

            listener.AcceptThreads = 3;
            listener.BackLog = 50;

            listener.CompressionType = CompressionType.ctNone;
            listener.EncryptType = EncryptType.etRijndael;
            listener.CryptoService = new ChatCryptService.ChatCryptService();
            
            chatServer.Start();

            Console.WriteLine(" Chat Server Started!");
            Console.WriteLine("--------------------------------------");

            string key;
            int iot = 0;
            int wt = 0;

            do
            {

                Console.WriteLine(" Press T <ENTER> for Threads");
                Console.WriteLine(" Press C <ENTER> for Clients");
                Console.WriteLine(" Press S <ENTER> for Stop Server");
                Console.WriteLine("--------------------------------------");
                Console.Write(" -> ");

                key = Console.ReadLine().ToUpper();

                if (key.Equals("T"))
                {

                    ThreadPool.GetAvailableThreads(out wt, out iot);

                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine(" I/O Threads " + iot.ToString());
                    Console.WriteLine(" Worker Threads " + wt.ToString());
                    Console.WriteLine("--------------------------------------");

                }

                if (key.Equals("C"))
                {

                    ISocketConnectionInfo[] infos = chatServer.GetConnections();

                    Console.WriteLine("\r\n--------------------------------------");
                    Console.WriteLine(" " + infos.Length.ToString() + " user(s)!\r\n");
                    
                    foreach (ISocketConnectionInfo info in infos)
	                {

                        Console.WriteLine(" Connection Id " + info.ConnectionId.ToString());
                        Console.WriteLine(" User Name " + ((ConnectionData) info.CustomData).UserName);
                        Console.WriteLine(" Ip Address " + info.RemoteEndPoint.Address.ToString());
                        
                        Console.WriteLine("--------------------------------------");
                        
	                }

                }

            } while (!key.Equals("S"));

            Console.WriteLine(" Chat Server Stopping!");
            Console.WriteLine("--------------------------------------");

            try
            {
                chatServer.Stop();
                chatServer.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            chatServer = null;

            Console.WriteLine(" Chat Server Stopped!");
            Console.WriteLine("--------------------------------------");

            Console.ReadLine();

        }
        
    }
    
}
