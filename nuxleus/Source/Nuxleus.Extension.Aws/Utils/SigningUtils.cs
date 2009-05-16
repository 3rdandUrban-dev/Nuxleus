using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;


namespace Nuxleus.Extension.Aws
{

    public static class Utilities
    {

        static readonly string AWS_PUBLIC_KEY = System.Environment.GetEnvironmentVariable("AWS_PUBLIC_KEY");
        static readonly string AWS_PRIVATE_KEY = System.Environment.GetEnvironmentVariable("AWS_PRIVATE_KEY");
        static readonly string AWS_URI_ENDPOINT = System.Environment.GetEnvironmentVariable("AWS_URI_ENDPOINT");
        static readonly XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";
        static readonly XNamespace aws = "http://sdb.amazonaws.com/doc/2007-11-07/";
        static readonly XNamespace i = "http://www.w3.org/2001/XMLSchema-instance";
        static Encoding m_encoding = new UTF8Encoding();


        public static void GetAuthorizationElements(string action, XmlWriter writer)
        {
            string timestamp = GetFormattedTimestamp();
            string awsNSPrefix = aws.NamespaceName;
            writer.WriteElementString("AWSAccessKeyId", awsNSPrefix, AWS_PUBLIC_KEY);
            writer.WriteElementString("Timestamp", awsNSPrefix, timestamp);
            GetSignature(action, timestamp, writer);
        }

        public static void GetSignature(string action, string timestamp, XmlWriter writer)
        {
            writer.WriteElementString("Signature", aws.NamespaceName, Sign(System.String.Format("{0}{1}", action, timestamp), AWS_PRIVATE_KEY));
        }

        public static string Sign(string data, string key)
        {
            HMACSHA1 signature = new HMACSHA1(m_encoding.GetBytes(key));
            return System.Convert.ToBase64String(signature.ComputeHash(m_encoding.GetBytes(data.ToCharArray())));
        }

        public static string GetFormattedTimestamp()
        {
            System.DateTime dateTime = System.DateTime.Now;
            return
                new System.DateTime(
                        dateTime.Year,
                        dateTime.Month,
                        dateTime.Day,
                        dateTime.Hour,
                        dateTime.Minute,
                        dateTime.Second,
                        dateTime.Millisecond,
                        System.DateTimeKind.Local
                    ).ToUniversalTime().ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", CultureInfo.InvariantCulture);
        }

    }
}
