using System;
using System.Xml;
using System.Collections;
using Nuxleus;
using Nuxleus.Amp.Fm;

namespace Nuxleus.Command
{
    public class Command
    {
        static void Main(string[] args)
        {
            Search search = new Search();
            object[] searchResults = search.SearchArtist(args[0]);

            IEnumerator entityArray = searchResults.GetEnumerator();

            while (entityArray.MoveNext())
            {
                object[] entity = (object[])entityArray.Current;
                Console.WriteLine
                        ("{0}: {1}{2}",
                        ((XmlElement)entity[2]).InnerText,
                        ((XmlElement)entity[3]).InnerText,
                        ((XmlElement)entity[1]).InnerText
                        );
            }
        }
    }
}
