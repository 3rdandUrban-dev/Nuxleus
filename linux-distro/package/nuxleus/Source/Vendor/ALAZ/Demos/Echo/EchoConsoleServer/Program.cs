using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Security.Cryptography;
using System.Text;

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

            ThreadPool.SetMinThreads(4, 4);
            ThreadPool.SetMaxThreads(8, 8);

            //----- CspParameters used in CryptService.
            CspParameters param = new CspParameters();
            param.KeyContainerName = "ALAZ_ECHO_SERVICE";
            RSACryptoServiceProvider serverKey = new RSACryptoServiceProvider(param);

            //----- Socket Server!
            OnEventDelegate FEvent = new OnEventDelegate(Event);

            SocketServer echoServer = new SocketServer(new EchoSocketService.EchoSocketService(FEvent));

            echoServer.Delimiter = new byte[] { 0xAA, 0xFF };
            echoServer.DelimiterType = DelimiterType.dtMessageTailExcludeOnReceive;
            
            echoServer.SocketBufferSize = 4096;
            echoServer.MessageBufferSize = 4096 * 4;
            
            echoServer.IdleCheckInterval = 60000;
            echoServer.IdleTimeOutValue = 120000;

            //----- Socket Listener!
            SocketListener listener = echoServer.AddListener("Commom Port - 8090", new IPEndPoint(IPAddress.Any, 8090));

            listener.AcceptThreads = 3;
            listener.BackLog = 50;
            
            listener.CompressionType = CompressionType.ctNone;
            listener.EncryptType = EncryptType.etNone;
            listener.CryptoService = new EchoCryptService.EchoCryptService();
            
            echoServer.Start();
 
            Console.WriteLine("Started!");
            Console.WriteLine("----------------------");

            string s;
            
            do
            {
                int iot = 0;
                int wt = 0;

                s = Console.ReadLine();

                if (s.Equals("g"))
                {

                    ThreadPool.GetAvailableThreads(out wt, out iot);
                    Console.WriteLine("IOT " + iot.ToString());
                    Console.WriteLine("WT " + wt.ToString());

                }

            } while (s.Equals("g"));

            try
            {
                echoServer.Stop();
                echoServer.Dispose();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            echoServer = null;

            Console.WriteLine("Stopped!");
            Console.WriteLine("----------------------");
            Console.ReadLine();

        }

        static void echoServer_OnException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Service Exception! - " + ex.Message);
            Console.WriteLine("------------------------------------------------");
            Console.ResetColor();
        }

        static void Event(string eventMessage)
        {
            if (eventMessage.Contains("Exception"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(eventMessage);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(eventMessage);
            }

        }

    }

}
