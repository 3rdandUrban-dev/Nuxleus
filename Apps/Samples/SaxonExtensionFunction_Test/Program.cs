using System;
using System.Diagnostics;
using System.Reflection;

namespace GenericTypeOperation
{

    class Program
    {

        static long directEllapsedTime = 0;
        static long genericsEllapsedTime = 0;

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            int count = 1000;

            //Warming things up...
            TestDirect(10, stopwatch);
            TestGenerics(10, stopwatch);

            //This time for real
            TestDirect(count, stopwatch);
            TestGenerics(count, stopwatch);


            Console.WriteLine("Completed direct test in:\t {0}ms", directEllapsedTime);
            Console.WriteLine("Completed generics test in:\t {0}ms", genericsEllapsedTime);
        }

        public static void TestDirect(int count, Stopwatch stopwatch)
        {
            Console.WriteLine("Starting direct object creation and method invocation test.");
            stopwatch.Start();
            for (int i = 0; i <= count; i++)
            {
                Test obj1 = new Test();
                Test obj2 = new Test("Created with a single string parameter");
                Test obj3 = new Test("Created with two", "string parameters");
                Console.WriteLine("Test.GetOutput(\"test1\") returned: {0}", obj1.GetOutput("test1"));
                Console.WriteLine("Test.GetOutput(\"test2\") returned: {0}", obj2.GetOutput("test2"));
                Console.WriteLine("Test.GetOutput(\"test3\") returned: {0}", obj3.GetOutput("test3"));
            }
            stopwatch.Stop();
            directEllapsedTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Complete!");
            stopwatch.Reset();
        }

        public static void TestGenerics(int count, Stopwatch stopwatch)
        {
            Console.WriteLine("Starting generic object creation and method invocation test.");
            stopwatch.Start();
            for (int i = 0; i <= count; i++)
            {
                Test obj1 = ExtensionFunction<Test>.Create();
                Test obj2 = ExtensionFunction<Test>.Create("Created with a single string parameter");
                Test obj3 = ExtensionFunction<Test>.Create("Created with two", "string parameters");
                //Console.WriteLine("Test.GetOutput(\"test1\") returned: {0}", obj1.GetOutput("test1"));
                //Console.WriteLine("Test.GetOutput(\"test2\") returned: {0}", obj2.GetOutput("test2"));
                //Console.WriteLine("Test.GetOutput(\"test3\") returned: {0}", obj3.GetOutput("test3"));
                Console.WriteLine("Test.GetOutput(\"test1\") returned: {0}", ExtensionFunction<Test>.Invoke<String>(obj1, "GetOutput", "test1"));
                Console.WriteLine("Test.GetOutput(\"test2\") returned: {0}", ExtensionFunction<Test>.Invoke<String>(obj2, "GetOutput", "test2"));
                Console.WriteLine("Test.GetOutput(\"test3\") returned: {0}", ExtensionFunction<Test>.Invoke<String>(obj3, "GetOutput", "test3"));
            }
            stopwatch.Stop();
            genericsEllapsedTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Complete!");
            stopwatch.Reset();
        }
    }
}
