using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuxleus.Extension.AWS.SimpleDB;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using EeekSoft.Asynchronous;
using System.Threading;
using System.Xml.Linq;
using VVMF.SOA.Common;

namespace AwsSdbSOAP_Test {
    class Program {

        static LoggerScope logger = new LoggerScope();
        static ExceptionHandlerScope exShield = new ExceptionHandlerScope();
        static ProfilerScope profiler = new ProfilerScope();
        static Scope scope;

        static void Main(string[] args) {

            System.Environment.SetEnvironmentVariable("AWS_PUBLIC_KEY", ConfigurationManager.AppSettings["AWS_PUBLIC_KEY"]);
            System.Environment.SetEnvironmentVariable("AWS_PRIVATE_KEY", ConfigurationManager.AppSettings["AWS_PRIVATE_KEY"]);

            scope = new Scope();
            scope += profiler.Scope;
            scope += logger.Scope;
            scope += exShield.Scope;

            logger.Message = "DoSomething()";

            // Inject code to scope
            scope.Begin = () => {
                PutAttributes().ExecuteAndWait();
            };

            Console.WriteLine("Time ellapsed {0} ms.", profiler.EllapsedTime.TotalMilliseconds); 

            

            //RequestType.Query, "geonames", "100", null, String.Format("['{0}' starts-with '{1}' OR '{0}' = '{1}']", "names", "seattle")))
            //RequestType.GetAttributes, "geonames", "5750997", "timezone", "latitude", "longitude"
            //RequestType.PutAttributes, "foobar", "foobarbaz", "foo=bar", "bar=baz", "baz=foo"
            //RequestType.Query, "foobar", "100", null, ""
            //RequestType.CreateDomain, "testfoo"
            //RequestType.ListDomains, "100", null
            //RequestType.PutAttributes, "foobar", "foobarbaz", "foo=bar", "bar=baz", "baz=foo"
            //RequestType.GetAttributes, "testfoo", "foobarbaz"
            //RequestType.PutAttributes, "testfoo", "foobarbaz", "foo=bar", "bar=baz", "baz=foo"
            //SimpleDBService service = new SimpleDBService();

            //using (StreamReader reader = service.MakeRequest(RequestType.PutAttributes, service.GetMessage(RequestType.GetAttributes, "testfoo", "foobar1"))) {
            //    Console.WriteLine(reader.ReadToEnd());
            //}
        }

        static IEnumerable<IAsync> PutAttributes() {

            List<XElement> responseList = new List<XElement>();

            SimpleDBService service = new SimpleDBService();

            IEnumerable<IAsync>[] requestOperations = new IEnumerable<IAsync>[] {
                SimpleDBService.MakeSoapRequestAsync<XElement>(RequestType.PutAttributes, service.GetMessage(RequestType.PutAttributes, "testfoobazzle", "test1", "foo=bar", "bar=baz", "baz=foo"), responseList),
                SimpleDBService.MakeSoapRequestAsync<XElement>(RequestType.PutAttributes, service.GetMessage(RequestType.PutAttributes, "testfoo555", "test2", "foo=bar", "bar=baz", "baz=foo"), responseList),
                SimpleDBService.MakeSoapRequestAsync<XElement>(RequestType.PutAttributes, service.GetMessage(RequestType.PutAttributes, "testfoo", "test3", "foo=bar", "bar=baz", "baz=foo"), responseList),
                SimpleDBService.MakeSoapRequestAsync<XElement>(RequestType.PutAttributes, service.GetMessage(RequestType.PutAttributes, "testfoo", "test4", "foo=bar", "bar=baz", "baz=foo"), responseList),
                SimpleDBService.MakeSoapRequestAsync<XElement>(RequestType.PutAttributes, service.GetMessage(RequestType.PutAttributes, "testfoo", "test5", "foo=bar", "bar=baz", "baz=foo"), responseList),
                SimpleDBService.MakeSoapRequestAsync<XElement>(RequestType.PutAttributes, service.GetMessage(RequestType.PutAttributes, "testfoo", "test6", "foo=bar", "bar=baz", "baz=foo"), responseList),
            };

            Stopwatch stopwatch = new Stopwatch();

            Console.WriteLine("Current thread id:\t {0}", Thread.CurrentThread.ManagedThreadId);

            stopwatch.Start();

            yield return Async.Parallel(requestOperations);

            stopwatch.Stop();

            Console.WriteLine("Completed all in:\t {0}ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("There are a total of {0} XElement objects in the result list", responseList.Count);
        }
    }
}
