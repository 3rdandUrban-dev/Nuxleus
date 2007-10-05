using System;
using System.Collections.Generic;
using System.Text;
using Nuxleus.Amp.Fm;
using System.Collections;
using Nuxleus;
using System.Xml;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Search search = new Search();
            object[] searchResults = search.SearchArtist("pearl jam");
            
            IEnumerator entityArray = searchResults.GetEnumerator();

            while (entityArray.MoveNext())
            {
                object[] entity = (object[])entityArray.Current;
                XmlElement term = (XmlElement)entity[1];
                XmlElement label = (XmlElement)entity[2];
                XmlElement scheme = (XmlElement)entity[3];
                Console.WriteLine("{0}: {1}{2}", label.InnerText, scheme.InnerText, term.InnerText);
            }
        }
    }
}
