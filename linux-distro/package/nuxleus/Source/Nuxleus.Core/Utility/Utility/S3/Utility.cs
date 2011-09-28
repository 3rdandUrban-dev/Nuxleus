using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web.Services;
using System.Web;

namespace Nuxleus.Utility.S3 {

    public class Utility {

        static string mySecretAccessKeyId;
        static string myAWSAccessKeyId;

        [WebMethod]
        public string GetSignature ( string authorize, string content ) {
            Encoding myEncoding = new UTF8Encoding();
            // Create a new Cryptography class using the 
            // Secret Access Key as the key
            HMACSHA1 myCrypto = new HMACSHA1(myEncoding.GetBytes(mySecretAccessKeyId));
            // Calculate the digest 
            byte[] strDigest = myCrypto.ComputeHash(Convert.FromBase64String(content));
            return Convert.ToBase64String(strDigest);
        }

        [WebMethod]
        public string GetAuthenticatedURL ( string authorize, string url ) {

            TimeSpan t = (DateTime.Now.AddMinutes(15)).ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
            int timestamp = (int)t.TotalSeconds;

            string time = timestamp.ToString();
            string content = String.Format(@"GET\n\n\n{0}\n{1}", time, url).Replace("\\n", "\n");
            string signature = GetSignatureHelper(content);
            return String.Format("{0}?AWSAccessKeyId={1}&Signature={2}&Expires={3}",
             url, myAWSAccessKeyId, signature, time);
        }
        string GetSignatureHelper ( string strOperation ) {
            Encoding myEncoding = new UTF8Encoding();
            // Create the source string which is used to create the digest
            string mySource = strOperation;
            // Create a new Cryptography class using the 
            // Secret Access Key as the key
            HMACSHA1 myCrypto = new HMACSHA1(myEncoding.GetBytes(mySecretAccessKeyId));
            // Convert the source string to an array of bytes
            char[] mySourceArray = mySource.ToCharArray();
            // Convert the source to a UTF8 encoded array of bytes
            byte[] myUTF8Bytes = myEncoding.GetBytes(mySourceArray);
            // Calculate the digest 
            byte[] strDigest = myCrypto.ComputeHash(myUTF8Bytes);
            return HttpUtility.UrlEncode(Convert.ToBase64String(strDigest));
        }
        /// <summary>
        /// Formats the provided time as a string limited to millisecond precision
        /// </summary>
        /// <param name="myTime"></param>
        /// <returns></returns>
        string FormatTimeStamp ( DateTime myTime ) {
            DateTime myUniversalTime = myTime.ToUniversalTime();
            return myUniversalTime.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Returns a new DateTime object set to the provided time 
        /// but with precision limited to milliseconds. 
        /// </summary>
        /// <param name="myTime"></param>
        /// <returns></returns>
        DateTime GetTimeStamp ( DateTime myTime ) {
            DateTime myUniversalTime = myTime.ToUniversalTime();
            DateTime myNewTime = new DateTime(myUniversalTime.Year, myUniversalTime.Month, myUniversalTime.Day,
                   myUniversalTime.Hour, myUniversalTime.Minute, myUniversalTime.Second,
                   myUniversalTime.Millisecond);
            return myNewTime;
        }
    }
}
