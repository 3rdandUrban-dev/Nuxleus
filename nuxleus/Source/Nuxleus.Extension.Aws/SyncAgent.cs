using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Configuration;
using Nuxleus.Core;
using Nuxleus.Extension.Aws.SimpleDb;

namespace Nuxleus.Extension.Aws
{

    public class SyncAgent<T>
    {

        static LoggerScope logger = new LoggerScope();
        static ExceptionHandlerScope exShield = new ExceptionHandlerScope();
        static ProfilerScope profiler = new ProfilerScope();
        static int m_workers = (int.Parse(ConfigurationManager.AppSettings["WorkerQueueMultiplier"]) * System.Environment.ProcessorCount);

        public void Initialize()
        {
            ServicePointManager.DefaultConnectionLimit = int.Parse(ConfigurationManager.AppSettings["DefaultConnectionLimit"]);

            //EnableDnsRoundRobin is not implemented on Mono
            //ServicePointManager.EnableDnsRoundRobin = true;

            //ServicePointManager.Expect100Continue = true;
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            HttpRuntimeSection configSection = (HttpRuntimeSection)config.GetSection("system.web/httpRuntime");
            this.LogInfo("ServicePointManager Default Connection Limit: {0}", ServicePointManager.DefaultConnectionLimit);
            this.LogInfo("system.web/httpRuntime/minFreeThreads: {0}", configSection.MinFreeThreads);
            this.LogInfo("system.web/httpRuntime/minLocalRequestFreeThreads: {0}", configSection.MinLocalRequestFreeThreads);

            int minWorkerThreads = int.Parse(ConfigurationManager.AppSettings["MinimumWorkerThreads"]);
            int minAsyncIOThreads = int.Parse(ConfigurationManager.AppSettings["MinimumAsyncIOThreds"]);
            int maxWorkerThreads = int.Parse(ConfigurationManager.AppSettings["MaximumWorkerThreads"]);
            int maxAsyncIOThreads = int.Parse(ConfigurationManager.AppSettings["MaximumAsyncIOThreads"]);
            ThreadPool.SetMaxThreads(maxWorkerThreads, maxAsyncIOThreads);
            ThreadPool.SetMinThreads(minWorkerThreads, minAsyncIOThreads);
        }

        public void Invoke(string fileName)
        {
            Scope scope = new Scope();
            scope += profiler.Scope;
            scope += logger.Scope;
            scope += exShield.Scope;

            logger.Message = "Processing SOAP requests";

            string[] attributeNames = new string[] {
                "geonamesid:0",
                "names:1",
                "alternatenames:3",
                "latitude:4",
                "longitude:5",
                "feature_class:6",
                "feature_code:7",
                "country_code:8",
                "cc2:9",
                "admin1_code:10",
                "admin2_code:11",
                "admin3_code:12",
                "admin4_code:12",
                "population:14",
                "elevation:15",
                "gtopo30:16",
                "timezone:17",
                "modification_date:18",
            };

            scope.Begin = () =>
            {
                using (TaskWorkerQueue q = new TaskWorkerQueue(m_workers))
                {

                    var tasks = from line in ReadLinesFromFile(fileName)
                                let inputArray = line.Split(new char[] { '\u0009' })
                                let attributes = CreateAttributeList(attributeNames, inputArray)
                                select CreatePutAttributesTasks("foobar", "geonamesid", attributes);

                    System.Console.WriteLine("Total Tasks: {0}", tasks.Count());
                    System.Console.WriteLine("LoadBalance: {0}", m_workers);

                    foreach (var task in tasks)
                    {
                        q.EnqueueTask(task);
                    }

                    System.Console.WriteLine("Total Processing Time: {0}", profiler.EllapsedTime.Milliseconds);
                }
            };
        }

        private static IEnumerable<Attribute> CreateAttributeList(string[] attributeNames, string[] attributeValues)
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
                            yield return new Attribute { Name = attributeName, Value = (string)csvEnumerator.Current, Replace = true };
                        }
                    }
                    else
                    {
                        yield return new Attribute { Name = title, Value = attributeValue, Replace = true };
                    }
                }
            }
        }

        private static PutAttributesTask CreatePutAttributesTasks(string domainName, string matchItemName, IEnumerable<Attribute> attributes)
        {
            return new PutAttributesTask
            {
                DomainName = new Domain { Name = domainName },
                Item = new Item { ItemName = attributes.First(a => a.Name == matchItemName).Value, Attribute = attributes.ToList<Attribute>() },
            };
        }

        // From Don Box @ http://www.pluralsight.com/community/blogs/dbox/archive/2007/10/09/48719.aspx
        // LINQ-compatible streaming I/O helper
        public static IEnumerable<string> ReadLinesFromFile(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
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
    }
}
