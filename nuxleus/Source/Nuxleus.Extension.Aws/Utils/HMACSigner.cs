using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Nuxleus.Extension.AWS.Utils {
    /// <summary>
    /// Creates the signature used for authentication of an mturk/AWS request
    /// </summary>
    internal class HMACSigner {
        #region Constants
        private static string SERVICE_NAME = "AWSMechanicalTurkRequester";
        private static string TIMESTAMP_FORMAT = "yyyy-MM-ddTHH:mm:ss.fffZ";
        #endregion

        private byte[] rawKey = null;	// secret key data for HMAC		

        /// <summary>
        /// Instantiates the signer with a secret key
        /// </summary>
        /// <param name="key">Key, that matches the AWS access ID</param>
        public HMACSigner(string key) {
            rawKey = Encoding.UTF8.GetBytes(key);
        }

        #region AWS authentication helpers
        //Get a Datetime that can be used with the Amazon Web Services methods
        public DateTime GetAwsDateStamp() {
            DateTime now = DateTime.UtcNow;
            // Important to use DateTimeKind for proper serialization 
            // see http://blogs.msdn.com/mattavis/archive/2005/10/11/479782.aspx
            DateTime ret = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond,
                                        DateTimeKind.Utc);

            // Workaround for issue where valid signatures cannot be produced
            // for timestamps that end with '...0Z', e.g "2007-06-29T15:53:38.770Z"
            //
            // Without the workaround 10% of all requests result in AWS.NotAuthorized
            //
            // Another option would be to simply use a fixed millisecond in the above
            // DateTime constructor. The workaround chosen here favors accuracy over
            // performance
            if (ret.Millisecond % 10 == 0) {
                ret = ret.AddMilliseconds(1);
            }

            return ret;
        }

        //Create signature used for Amazon Web Service method calls
        public string GetAwsSignature(string operation, DateTime timeStamp) {
            string dataToSign = SERVICE_NAME + operation + timeStamp.ToString(TIMESTAMP_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
            HMACSHA1 hmac = new HMACSHA1(rawKey);

            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign.ToCharArray())));
        }
        #endregion

    }
}
