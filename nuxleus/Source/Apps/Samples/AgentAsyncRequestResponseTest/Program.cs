using System;
using System.Collections.Generic;
using System.Text;

namespace AgentAsyncRequestResponseTest {

    public class Program {

        public static void Main (string[] args) {

            SumDelegate sumDelegate = Sum;
            sumDelegate.BeginInvoke(100, SumIsDone, sumDelegate);
            //Agent agent = new Agent(new LoadBalancer());
            //agent.BeginRequest(new Request());
            Console.ReadLine();
        }


        internal delegate UInt64 SumDelegate (UInt64 n);

        private static UInt64 Sum (UInt64 n) {
            UInt64 sum = 0;
            for (UInt64 i = 1; i <= n; i++) {
                checked {
                    Console.WriteLine(i);
                    sum += 1;
                }
            }
            return sum;
        }

        public static void SumIsDone (IAsyncResult ar) {
            SumDelegate sumDelegate = (SumDelegate)ar.AsyncState;

            UInt64 sum = sumDelegate.EndInvoke(ar);
            Console.WriteLine("Sum: {0}", sum);
        }
    }
}
