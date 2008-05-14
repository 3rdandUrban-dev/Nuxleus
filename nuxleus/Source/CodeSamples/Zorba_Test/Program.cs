using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuxleus.Process;
using System.IO;
using System.Xml;

namespace Zorba_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ZorbaProcess process = new ZorbaProcess();
            process.RunQuery(args[0]);
            XmlReader xReader = XmlReader.Create(new StringReader(process.Output.ReadToEnd()));
            do
            {
                if(xReader.IsStartElement())
                    Console.WriteLine(xReader.ReadOuterXml());
            } while (xReader.Read());
        }
    }
}
