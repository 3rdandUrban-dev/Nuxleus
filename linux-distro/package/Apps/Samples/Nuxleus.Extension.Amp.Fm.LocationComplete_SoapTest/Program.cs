using System;
using System.Xml;
using System.Web.Services;
using System.Collections;
using Amp.Fm.fm.amp.dev;

namespace Amp.Fm.LocationComplete_SoapTest {
    class Program {
        static void Main ( string[] args ) {

            LocationComplete locationComplete = new LocationComplete();
            Entity[] results = locationComplete.GetLocation((args.Length > 0) ? args[0] : String.Empty);

            Console.WriteLine("object result length: {0}", results.Length);


            IEnumerator entityArray = results.GetEnumerator();

            while (entityArray.MoveNext()) {
                Entity entity = (Entity)entityArray.Current;
                Console.WriteLine
                        ("{0}: {1}{2}",
                            entity.Label,
                            entity.Scheme,
                            entity.Term
                        );
            }
        }
    }
}
