using System;

namespace GenericTypeOperation
{

    public class Test
    {

        string m_output;

        public Test()
        {
            m_output = "Created with default constructor";
        }

        public Test(string output)
        {
            m_output = output;
        }

        public Test(string output1, string output2)
        {
            m_output = String.Format("{0} {1}", output1, output2);
        }

        public string GetOutput(string foo)
        {
            Console.WriteLine(foo);
            return m_output;
        }

    }
}
