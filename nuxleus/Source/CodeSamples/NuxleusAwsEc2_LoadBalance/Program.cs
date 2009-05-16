using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuxleus.Extension.Aws;
using System.Configuration;
using System.Xml;
using System.IO;

namespace NuxleusAwsEc2_LoadBalance
{
    public class Program
    {


        public static void Main(string[] args)
        {
            string m_AwsPublicKey = ConfigurationManager.AppSettings["AWS_PUBLIC_KEY"];
            string m_AwsPrivateKey = ConfigurationManager.AppSettings["AWS_PRIVATE_KEY"];
            string m_AwsBaseURL = ConfigurationManager.AppSettings["AWS_URI_ENDPOINT"];

            IAwsConnection connection = new HttpQueryConnection(m_AwsPublicKey, m_AwsPrivateKey, m_AwsBaseURL);
            Dictionary<String, String> requestDict = new Dictionary<string, string>();
            requestDict.Add("AccessPointPort", "80");
            requestDict.Add("EndPointPort", "80");
            requestDict.Add("AvailabilityZones.member.1", "us-east-1b");
            requestDict.Add("AvailabilityZones.member.2", "us-east-1c");
            requestDict.Add("RoutingProtocol", "HTTP");
            requestDict.Add("AccessPointName", "nuxleus-core");

            XmlReader reader = XmlReader.Create(new StringReader(connection.MakeRequest("", "DescribeAccessPoints"/*, requestDict*/)));
            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.IsStartElement())
                        {
                            Console.WriteLine("Node: {0}", reader.Name);
                        }
                        break;
                    case XmlNodeType.Text:
                        Console.Write(": {0}", reader.Value);
                        Console.WriteLine();
                        break;
                    default:
                        break;
                }


            } while (reader.Read());

            Console.ReadLine();
        }
    }
}
