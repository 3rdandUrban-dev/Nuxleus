using System;
using System.Xml;
using System.IO;
using Xameleon.Llup;

public class Test 
{

    public static void Main()
    {
      XMPPSubscriber sub = new XMPPSubscriber();
      sub.Connect("127.0.0.1", 9879, "gmail.com", 
		  "talk.google.com", "duhmeee", "azerty00");

      Console.WriteLine("Wait until you get the message and press a key to continue");
      Console.ReadLine();
            
      sub.Disconnect();
    }
}
