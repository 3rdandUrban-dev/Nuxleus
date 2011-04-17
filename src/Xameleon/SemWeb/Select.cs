//using System;
//using System.IO;
//using System.Text;
//using SemWeb;

//namespace Xameleon.SemWeb {
//    public class Select {
//        const string RDF = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
//        const string FOAF = "http://xmlns.com/foaf/0.1/";

//        static readonly Entity rdftype = RDF + "type";
//        static readonly Entity foafPerson = FOAF + "Person";
//        static readonly Entity foafknows = FOAF + "knows";
//        static readonly Entity foafname = FOAF + "name";

//        public Select() { }

//        public static string Process(String foafURI) {

//            StringBuilder builder = new StringBuilder();
//            Uri uri = new Uri(foafURI);

//            Store store = new MemoryStore();
//            RdfReader reader = RdfReader.LoadFromUri(uri);
//            reader.BaseUri = uri.OriginalString;
//            store.Import(reader);

//            builder.AppendLine("<statement>These are the people in the file: </statement>");
//            foreach (Statement s in store.Select(new Statement(null, rdftype, foafPerson))) {
//                foreach (Resource r in store.SelectObjects(s.Subject, foafname))
//                    builder.AppendLine("<entry>" + r + "</entry>");
//            }

//            builder.AppendLine("<statement>And here's RDF/XML just for some of the file: </statement>");

//            using (RdfWriter w = new RdfXmlWriter(new StringWriter(builder))) {
//                store.Select(new Statement(null, foafname, null), w);
//                store.Select(new Statement(null, foafknows, null), w);
//            }

//            return builder.ToString();
//        }

//    }

//}
