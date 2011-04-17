using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Nuxleus.Entity;
using Nuxleus.Utility.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Linq.Expressions;


namespace Buyer_Test {
    public delegate TResult MyDelegate<TA, TB, TResult> ( TA arg1, TB arg2 );
    public delegate TResult MyDelegate<T1, T2, T3, TResult> ( T1 a, T2 b, T3 c );
    public class Program {
        public static void Main ( string[] args ) {
            Expression<Func<int, int, int>> expression = ( x, y ) => x + y;
            Console.WriteLine(expression.Body);
            Console.WriteLine(expression.Compile().Invoke(2, 3));

            XmlSerializer formatter = new XmlSerializer(typeof(Entity));
            Stream stream = new FileStream("myNewEntity.xml",
                         FileMode.Create,
                         FileAccess.Write, FileShare.None);

            Entity entity = new Entity("foobar", "baz", "http://amp.fm");
            Console.WriteLine("Scheme: {0}, Label: {1}, Term: {2}", entity.Scheme, entity.Label, entity.Term);

            using (stream) {
                formatter.Serialize(stream, entity);
            }


            Stream fileStream = new FileStream("myEntity.xml",
                          FileMode.Open,
                          FileAccess.Read,
                          FileShare.Read);

            Entity nEntity = (Entity)formatter.Deserialize(fileStream);
            Console.WriteLine("Scheme: {0}, Label: {1}, Term: {2}", nEntity.Scheme, nEntity.Label, nEntity.Term);

        }
    }
}
