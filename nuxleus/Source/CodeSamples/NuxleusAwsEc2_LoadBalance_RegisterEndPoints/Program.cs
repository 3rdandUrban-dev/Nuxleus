using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using Nuxleus.Extension.Aws;

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
            requestDict.Add("endPoints.member.1.instanceId", "i-db2bacb2");
            requestDict.Add("endPoints.member.2.instanceId", "i-d82bacb1");
            requestDict.Add("endPoints.member.3.instanceId", "i-af2bacc6");
            requestDict.Add("endPoints.member.4.instanceId", "i-ac2bacc5");
            requestDict.Add("endPoints.member.5.instanceId", "i-ad2bacc4");
            requestDict.Add("endPoints.member.6.instanceId", "i-f468ef9d");
            requestDict.Add("endPoints.member.7.instanceId", "i-623abd0b");
            requestDict.Add("endPoints.member.8.instanceId", "i-653abd0c");
            requestDict.Add("accessPointName", "nuxleus-core");

            XmlReader reader = XmlReader.Create(new StringReader(connection.MakeRequest("", "RegisterEndPoints", requestDict)));
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
