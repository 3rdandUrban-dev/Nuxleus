using System.IO;

namespace Xameleon.Function {

    public static class HttpWebResponseStream {

        ///<summary>
        ///</summary>
        ///<param name="stream"></param>
        ///<returns></returns>
        public static string GetResponseString ( Stream stream ) {
            using (StreamReader reader = new StreamReader(stream)) {
                return reader.ReadToEnd();
            }
        }
    }
}
