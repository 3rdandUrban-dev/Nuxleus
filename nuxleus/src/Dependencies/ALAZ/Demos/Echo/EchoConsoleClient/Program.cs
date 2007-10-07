using System;
using System.Net;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

using EchoSocketService;
using EchoCryptService;

using ALAZ.SystemEx.NetEx.SocketsEx;

namespace Main
{

    class MainClass
    {

        [STAThread]
        static void Main(string[] args)
        {

            Console.SetWindowSize(60, 25);

            ThreadPool.SetMinThreads(4, 4);
            ThreadPool.SetMaxThreads(32, 32);

            EncryptType et = EncryptType.etNone;
            CompressionType ct = CompressionType.ctNone;
            int port = 8090;
            int connections = 50;

            if (args.Length >= 1)
            {
                port = Convert.ToInt32(args[0]);
            }

            if (args.Length >= 2)
            {
                et = (EncryptType) Enum.Parse(typeof(EncryptType), args[1], true);
            }

            if (args.Length >= 3)
            {
                ct = (CompressionType) Enum.Parse(typeof(CompressionType), args[2], true);
            }

            //----- Socket Client!
            OnEventDelegate FEvent = new OnEventDelegate(Event);

            SocketClient echoClient = new SocketClient(new EchoSocketService.EchoSocketService(FEvent));
            
            echoClient.Delimiter = new byte[] {0xAA, 0xFF};
            echoClient.DelimiterType = DelimiterType.dtMessageTailExcludeOnReceive;
            
            echoClient.SocketBufferSize = 4096;
            echoClient.MessageBufferSize = 4096 * 4;
            
            echoClient.IdleCheckInterval = 60000;
            echoClient.IdleTimeOutValue = 120000;

            //----- Socket Connectors!
            SocketConnector connector = null;
            
            for (int i = 0; i < connections; i++)
            {
                
                connector = echoClient.AddConnector("Connector " + i.ToString(), new IPEndPoint(IPAddress.Loopback, 8090));
                
                /*
                connector.ProxyInfo = new ProxyInfo(
                    ProxyType.ptHTTP, 
                    new IPEndPoint(IPAddress.Loopback, 8000), 
                    new NetworkCredential("user", "pass"));
                */
                
                connector.CryptoService =  new EchoCryptService.EchoCryptService();
                connector.CompressionType = ct;
                connector.EncryptType = et;

                connector.ReconnectAttempts = 2;
                connector.ReconnectAttemptInterval = 5000;
                
            }

            Console.Title = "EchoConsoleClient / " + connections.ToString() + " Connections / " + Enum.GetName(typeof(EncryptType), et) + " / " + Enum.GetName(typeof(CompressionType), ct);
            
            echoClient.Start();

            Console.WriteLine("Started!");
            Console.WriteLine("----------------------");

            Console.ReadLine();

            try
            {
                echoClient.Stop();
                echoClient.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            echoClient = null;

            Console.WriteLine("Stopped!");
            Console.WriteLine("----------------------");
            Console.ReadLine();

        }

        static void Event(string eventMessage)
        {
            Console.WriteLine(eventMessage);
        }

    }

}
