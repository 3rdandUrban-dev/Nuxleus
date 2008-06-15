using System;

namespace GenericTypeOperation {
    class Program {
        static void Main(string[] args) {
            object obj1 = ExtensionFunction<Test>.Create();
            object obj2 = ExtensionFunction<Test>.Create("Created with a single string parameter");
            object obj3 = ExtensionFunction<Test>.Create("Created with two", "string parameters");
            Console.WriteLine("Test.GetOutput(\"test1\") returned: {0}", ExtensionFunction<Test>.Invoke<String>(obj1, "GetOutput", "test1"));
            Console.WriteLine("Test.GetOutput(\"test2\") returned: {0}", ExtensionFunction<Test>.Invoke<String>(obj2, "GetOutput", "test2"));
            Console.WriteLine("Test.GetOutput(\"test3\") returned: {0}", ExtensionFunction<Test>.Invoke<String>(obj3, "GetOutput", "test3"));
        }
    }
}
