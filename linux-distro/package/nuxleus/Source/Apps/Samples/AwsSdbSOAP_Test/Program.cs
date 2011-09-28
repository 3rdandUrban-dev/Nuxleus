using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Nuxleus.Asynchronous;
using Nuxleus.Core;
using Nuxleus.Extension.Aws;
using Nuxleus.Extension.Aws.SimpleDb;
using AwsSdbModel = Nuxleus.Extension.Aws.SimpleDb;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.XPath;
using System.Xml;
using Nuxleus.Extension;

namespace AwsSdbSOAP_Test
{
    class Program
    {
        static XNamespace r = "http://nuxleus.com/message/response";
        static XmlNameTable table = new NameTable();
        static XmlNamespaceManager manager = new XmlNamespaceManager(table);
        static XPathExpression expression = XPathExpression.Compile("/s:Envelope/s:Body");

        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("AWS_PUBLIC_KEY", ConfigurationManager.AppSettings["AWS_PUBLIC_KEY"]);
            Environment.SetEnvironmentVariable("AWS_PRIVATE_KEY", ConfigurationManager.AppSettings["AWS_PRIVATE_KEY"]);
            Environment.SetEnvironmentVariable("AWS_URI_ENDPOINT", ConfigurationManager.AppSettings["AWS_URI_ENDPOINT"]);
            manager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            manager.AddNamespace("aws", "http://sdb.amazonaws.com/doc/2007-11-07/");
            expression.SetContext(manager);

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            switch (args[0])
            {
                case "select":
                    RunSelectQuery(args[1], args[2]);
                    break;
                case "create-domain":
                    CreateDomain(args[1]);
                    break;
                case "delete-domain":
                    DeleteDomain(args[1]);
                    break;
                case "list-domains":
                    ListDomains();
                    break;
                case "domain-metadata":
                    DomainMetadata(args[1]);
                    break;
                case "get-attributes":
                    List<string> getAttList = new List<string>();
                    int i = 0;
                    foreach (string attribute in args)
                    {
                        if (i > 2)
                        {
                            getAttList.Add(attribute);
                        }
                        i++;
                    }
                    GetAttributes(args[1], args[2], getAttList);
                    break;
                case "delete-attributes":
                    List<AwsSdbModel.Attribute> delAttList = new List<AwsSdbModel.Attribute>();
                    int d = 0;
                    foreach (string attribute in args)
                    {
                        if (d > 2)
                        {
                            AwsSdbModel.Attribute att = new AwsSdbModel.Attribute { Name = attribute, Value = String.Empty };
                            delAttList.Add(att);
                        }
                        d++;
                    }
                    DeleteAttributes(args[1], args[2], delAttList);
                    break;
                case "put-attributes":
                    List<AwsSdbModel.Attribute> putAttList = new List<AwsSdbModel.Attribute>();
                    int p = 0;
                    foreach (string attribute in args)
                    {
                        if (p > 2)
                        {
                            AwsSdbModel.Attribute att = new AwsSdbModel.Attribute { Name = attribute, Value = String.Empty };
                            putAttList.Add(att);
                        }
                        p++;
                    }
                    PutAttributes(args[1], args[2], putAttList);
                    break;
                case "sync":
                    RunSync(args);
                    break;
                case "noload-async":
                    RunAsyncNoLoadBalance(args);
                    break;
                case "procload-async":
                    Nuxleus.Core.Log.LogInfo<Program>("Args Length: {0}", args.Length);
                    List<string> jobs = new List<string>();
                    int j = 0;
                    foreach (string job in args)
                    {
                        if (j > 2)
                        {
                            Nuxleus.Core.Log.LogInfo<Program>("job: {0}", job);
                            jobs.Add(job);
                        }
                        j++;
                    }
                    //RunAsyncProcessLoadBalance(jobs, int.Parse(args[1]), int.Parse(args[2]));
                    break;
                case "batch-async":
                    RunBatchAsync(args);
                    break;
                case "async":
                default:
                    RunAsync(args);
                    break;
            }
            stopwatch.Stop();

            Nuxleus.Core.Log.LogInfo<Program>("Completed all in:\t {0}ms", stopwatch.ElapsedMilliseconds);
        }

        static void RunSelectQuery(string domain, string expression)
        {
            SelectTask selectTask = new SelectTask { DomainName = new Domain { Name = domain }, SelectExpression = expression };
            WriteResponseMetadataToConsole<SelectResult>(selectTask.Invoke());
        }

        static void CreateDomain(string domain)
        {
            CreateDomainTask task = new CreateDomainTask { DomainName = domain };
            WriteResponseMetadataToConsole<CreateDomainResponse>(task.Invoke());
        }

        static void DeleteDomain(string domain)
        {
            DeleteDomainTask task = new DeleteDomainTask { DomainName = domain };
            WriteResponseMetadataToConsole<DeleteDomainResponse>(task.Invoke());
        }

        static void ListDomains()
        {
            ListDomainsTask task = new ListDomainsTask { };
            WriteResponseMetadataToConsole<ListDomainsResponse>(task.Invoke());
        }

        static void DomainMetadata(string domain)
        {
            DomainMetadataTask task = new DomainMetadataTask { DomainName = domain };
            WriteResponseMetadataToConsole<DomainMetadataResponse>(task.Invoke());
        }

        static void PutAttributes(string domain, string item, List<AwsSdbModel.Attribute> attributes)
        {
            PutAttributesTask task = new PutAttributesTask { DomainName = new Domain { Name = domain }, Item = new Item { ItemName = item, Attribute = attributes } };
            WriteResponseMetadataToConsole<PutAttributesResponse>(task.Invoke());
        }

        static void GetAttributes(string domain, string item, List<string> attName)
        {
            GetAttributesTask task = new GetAttributesTask { DomainName = domain, ItemName = item, AttributeName = attName };
            WriteResponseMetadataToConsole<GetAttributesResponse>(task.Invoke());
        }

        static void DeleteAttributes(string domain, string item, List<AwsSdbModel.Attribute> attName)
        {
            DeleteAttributesTask task = new DeleteAttributesTask { DomainName = domain, ItemName = item, Attribute = attName };
            WriteResponseMetadataToConsole<DeleteAttributesResponse>(task.Invoke());
        }

        static void WriteResponseMetadataToConsole<TResult>(IResponse response)
        {
            foreach (string header in response.Headers)
            {
                Nuxleus.Core.Log.LogInfo<Program>("ResponseHeader: {0}: {1}", header, response.Headers[header]);
            }
            //SdbResponseMetadata metadata = ((SelectResult)response.Result).Metadata;
            //Log.LogInfo<Program>("BoxUsage: {0}", metadata.BoxUsage);
            //Log.LogInfo<Program>("RequestId: {0}", metadata.RequestId);
            TResult result = (TResult)response.Result;

            Nuxleus.Core.Log.LogInfo<Program>("TotalCount: {0}", result.ToString());

            //foreach (string header in ((PutAttributesResponse)response).Headers)
            //{
            //    Log.LogInfo<Program>("DomainName: {0}", domainName);
            //}

            //XDocument xDoc = null;
            //XElement xTask = null;
            //XElement xResponseMetadata = null;

            //using (XmlReader reader = XmlReader.Create(response.Result)){
            //    xDoc = XDocument.Load(reader);
            //    xTask = xDoc.XPathSelectElement(
            //            String.Format("//aws:{0}",
            //            typeof(TResult).Name),
            //            manager
            //        );

            //    xResponseMetadata = xDoc.XPathSelectElement(
            //            String.Format("//aws:ResponseMetadata",
            //            typeof(ResponseMetadata)),
            //            manager
            //        );
            //}

            //XmlSerializer xSerializer = new XmlSerializer(typeof(TResult));
            //TResult result = (TResult)xSerializer.Deserialize(xTask.CreateReader());
            //using (Stream stream = new MemoryStream())
            //{
            //    using (XmlWriter writer = XmlWriter.Create(stream))
            //    {
            //        xSerializer.Serialize(writer, result);
            //        stream.Seek(0, 0);
            //        Log.LogInfo<Program>("Response: {0}", new StreamReader(stream).ReadToEnd());
            //    }
            //}

            //XmlSerializer xResponseMetadataSerializer = new XmlSerializer(typeof(ResponseMetadata));
            //ResponseMetadata metadataResult = (ResponseMetadata)xResponseMetadataSerializer.Deserialize(xResponseMetadata.CreateReader());
            //using (Stream stream = new MemoryStream())
            //{
            //    using (XmlWriter writer = XmlWriter.Create(stream))
            //    {
            //        xResponseMetadataSerializer.Serialize(writer, metadataResult);
            //        stream.Seek(0, 0);
            //        Log.LogInfo<Program>("Response: {0}", new StreamReader(stream).ReadToEnd());
            //    }
            //}

        }

        static void RunSync(string[] args)
        {
            //SyncAgent<Program> agent = new SyncAgent<Program>();
            //agent.Invoke(args[1]);
        }

        static void RunAsync(string[] args)
        {
            Agent agent = new Agent();
            agent.Initialize();
            agent.Invoke<XElement>(InvokeOperationAsync(ReadLinesFromFile(args[1]), args[2]), true);
        }

        static void RunBatchAsync(string[] args)
        {
            Agent agent = new Agent();
            agent.Initialize();
            agent.Invoke<XElement>(InvokeBatchOperationAsync(ReadLinesFromFile(args[1]), args[2], 25), true);
        }

        static void RunAsyncNoLoadBalance(string[] args)
        {
            Agent agent = new Agent();
            agent.Initialize();
            agent.Invoke<XElement>(InvokeBatchOperationAsync(ReadLinesFromFile(args[1]), args[2], 25), false);
        }

        //static void RunAsyncProcessLoadBalance(List<string> args, int maxWorkers, int cpuThrottle)
        //{
        //    using (Nuxleus.Extension.Aws.ProcessQueue q = new Nuxleus.Extension.Aws.ProcessQueue(((args.Count <= maxWorkers) ? args.Count : maxWorkers), cpuThrottle))
        //    {
        //        foreach (string job in args)
        //        {
        //            q.EnqueueTask(job);
        //        }
        //    }
        //}

        // From Don Box @ http://www.pluralsight.com/community/blogs/dbox/archive/2007/10/09/48719.aspx
        // LINQ-compatible streaming I/O helper
        static IEnumerable<string> ReadLinesFromFile(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8, true))
            {
                while (true)
                {
                    string s = reader.ReadLine();
                    if (s == null)
                        break;
                    yield return s;
                }
            }
        }

        static IEnumerable<IResponse> InvokeOperation(IEnumerable<string> operation, string domain)
        {
            foreach (string inputLine in operation)
            {
                string[] inputArray = inputLine.Split(new char[] { '\u0009' });
                yield return CreateTask(inputArray, domain).Invoke();
            }

        }

        static IEnumerable<IEnumerable<IAsync>> InvokeBatchOperationAsync(IEnumerable<string> operation, string domain, int groupsOf)
        {

            var lines = from line in operation
                        select line.Split(new char[] { '\u0009' });

            int count = 0;
            while (count < lines.Count())
            {
                yield return CreateBatchTask(lines.Skip(count).Take(groupsOf), domain).AsIAsync();
                count += groupsOf;
            }
        }

        static IEnumerable<IEnumerable<IAsync>> InvokeOperationAsync(IEnumerable<string> operation, string domain)
        {
            return from line in operation
                   let inputArray = line.Split(new char[] { '\u0009' })
                   select CreateTask(inputArray, domain).AsIAsync();
        }

        public static PutAttributesTask CreateTask(string[] inputArray, string domain)
        {
            KeyValuePair<string, string>[] geoNames = GetGeoNames(inputArray);

            IEnumerator attributeArray = geoNames.GetEnumerator();

            List<AwsSdbModel.Attribute> attributes = GetAttributes(attributeArray);

            return new PutAttributesTask { DomainName = new Domain { Name = domain }, Item = new Item { ItemName = (string)inputArray.GetValue(0), Attribute = attributes } };
        }

        public static BatchPutAttributesTask CreateBatchTask(IEnumerable<string[]> inputArrayList, string domain)
        {
            List<Item> items = new List<Item>();

            foreach (string[] inputArray in inputArrayList)
            {
                KeyValuePair<string, string>[] geoNames = GetGeoNames(inputArray);

                IEnumerator attributeArray = geoNames.GetEnumerator();

                items.Add(new Item { ItemName = String.Format("{0}{1}{2}", (string)inputArray.GetValue(0), (string)inputArray.GetValue(1), (string)inputArray.GetValue(2)).GetHashCode().ToString(), Attribute = GetAttributes(attributeArray) });

            }

            return new BatchPutAttributesTask { DomainName = new Domain { Name = domain }, Item = items };
        }

        static KeyValuePair<string, string>[] GetGeoNames(string[] inputArray)
        {

            return
                    //new KeyValuePair<string, string>[] { 
                    //    new KeyValuePair<string,string>("geonamesid",(string)inputArray.GetValue(0)),
                    //    new KeyValuePair<string,string>("names",(string)inputArray.GetValue(1)),
                    //    new KeyValuePair<string,string>("alternatenames",(string)inputArray.GetValue(3)), 
                    //    new KeyValuePair<string,string>("latitude", (string)inputArray.GetValue(4)),
                    //    new KeyValuePair<string,string>("longitude", (string)inputArray.GetValue(5)),
                    //    new KeyValuePair<string,string>("feature_class", (string)inputArray.GetValue(6)),
                    //    new KeyValuePair<string,string>("feature_code",(string)inputArray.GetValue(7)),
                    //    new KeyValuePair<string,string>("country_code",(string)inputArray.GetValue(8)),
                    //    new KeyValuePair<string,string>("cc2",(string)inputArray.GetValue(9)),
                    //    new KeyValuePair<string,string>("admin1_code",(string)inputArray.GetValue(10)),
                    //    new KeyValuePair<string,string>("admin2_code",(string)inputArray.GetValue(11)),
                    //    new KeyValuePair<string,string>("admin3_code",(string)inputArray.GetValue(12)),
                    //    new KeyValuePair<string,string>("admin4_code",(string)inputArray.GetValue(13)),
                    //    new KeyValuePair<string,string>("population",(string)inputArray.GetValue(14)),
                    //    new KeyValuePair<string,string>("elevation",(string)inputArray.GetValue(15)),
                    //    new KeyValuePair<string,string>("gtopo30",(string)inputArray.GetValue(16)),
                    //    new KeyValuePair<string,string>("timezone",(string)inputArray.GetValue(17)),
                    //    new KeyValuePair<string,string>("modification_date",(string)inputArray.GetValue(18)),
                    //};
                    new KeyValuePair<string, string>[] { 
                        new KeyValuePair<string,string>("country_code",(string)inputArray.GetValue(0)),
                        new KeyValuePair<string,string>("postal_code",(string)inputArray.GetValue(1)),
                        new KeyValuePair<string,string>("place_name",(string)inputArray.GetValue(2)),
                        new KeyValuePair<string,string>("admin1_code",(string)inputArray.GetValue(3)),
                        new KeyValuePair<string,string>("admin1_name",(string)inputArray.GetValue(4)),
                        new KeyValuePair<string,string>("admin2_code",(string)inputArray.GetValue(5)),
                        new KeyValuePair<string,string>("admin2_name",(string)inputArray.GetValue(6)),
                        new KeyValuePair<string,string>("admin3_name",(string)inputArray.GetValue(7)),
                        new KeyValuePair<string,string>("latitude", (string)inputArray.GetValue(8)),
                        new KeyValuePair<string,string>("longitude", (string)inputArray.GetValue(9)),
                        new KeyValuePair<string,string>("accuracy",(string)inputArray.GetValue(10)),
                    };
        }

        static List<AwsSdbModel.Attribute> GetAttributes(IEnumerator attributes)
        {

            List<AwsSdbModel.Attribute> attributeList = new List<AwsSdbModel.Attribute>();

            while (attributes.MoveNext())
            {
                KeyValuePair<System.String, System.String> attribute = (KeyValuePair<System.String, System.String>)attributes.Current;
                string title = attribute.Key;
                string current = attribute.Value.Normalize().ToLower();

                if (current.Length > 0)
                {
                    if (current.Contains(","))
                    {
                        IEnumerator csvEnumerator = current.Split(new char[] { ',' }).GetEnumerator();
                        while (csvEnumerator.MoveNext())
                        {
                            attributeList.Add(new AwsSdbModel.Attribute { Name = title, Value = (string)csvEnumerator.Current });
                        }
                    }
                    else
                    {
                        attributeList.Add(new AwsSdbModel.Attribute { Name = title, Value = current });
                    }
                }
            }
            return attributeList;
        }

        private static IEnumerable<AwsSdbModel.Attribute> CreateAttributeList(string[] attributeNames, string[] attributeValues)
        {
            foreach (string attributeName in attributeNames)
            {
                string title = attributeName.SubstringBefore(":");
                int pos = int.Parse(attributeName.SubstringAfter(":"));
                string attributeValue = attributeValues[pos];
                if (attributeValue.Length > 0)
                {
                    if (attributeValue.Contains(","))
                    {
                        IEnumerator csvEnumerator = attributeValue.Split(new char[] { ',' }).GetEnumerator();
                        while (csvEnumerator.MoveNext())
                        {
                            yield return new AwsSdbModel.Attribute { Name = attributeName, Value = (string)csvEnumerator.Current, Replace = true };
                        }
                    }
                    else
                    {
                        yield return new AwsSdbModel.Attribute { Name = title, Value = attributeValue, Replace = true };
                    }
                }
            }
        }
    }
}