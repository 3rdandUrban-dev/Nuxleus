using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VVMF.SOA.Common;
using System.Threading;

namespace ScopeTest
{
    static class Program
    {
        static LoggerScope logger = new LoggerScope();

        static ExceptionHandlerScope exShield = new ExceptionHandlerScope();

        static ProfilerScope profiler = new ProfilerScope();
        
        static Scope scope;

        static void Main(string[] args)
        {
            // Aggregate scope handlers to a scope chain:
            scope = new Scope(profiler.Scope + logger.Scope + exShield.Scope);

            #region DoSomething()

            logger.Message = "DoSomething()";

            // Inject code to scope
            scope.Begin = () =>
            {
                DoSomething();
            };

            Console.WriteLine("Time ellapsed {0} ms.", profiler.EllapsedTime.TotalMilliseconds); 
            
            #endregion

            Console.WriteLine("\n---\n");

            #region GetSomething()

            logger.Message = "GetSomething()";

            int value = 0;

            // Inject code to scope
            scope.Begin = () =>
            {
                value = GetSomething();
            };

            Console.WriteLine("Time ellapsed {0} ms.", profiler.EllapsedTime.TotalMilliseconds);

            Console.WriteLine("Value is {0}.", value);

            #endregion

            Console.WriteLine("\n---\n");

            #region ValidateSomething(string value)

            logger.Message = "ValidateSomething(string value)";

            // Inject code to scope
            scope.Begin = () =>
            {
                ValidateSomething(null); // Exception will be handled!
            };

            Console.WriteLine("Time ellapsed {0} ms.", profiler.EllapsedTime.TotalMilliseconds);

            Console.WriteLine();

            logger.Message = "ValidateSomething(string value)";

            // Inject code to scope
            scope.Begin = () =>
            {
                ValidateSomething(""); // Exception will be handled!
            };

            Console.WriteLine("Time ellapsed {0} ms.", profiler.EllapsedTime.TotalMilliseconds);

            #endregion

            Console.WriteLine("\n---\n");

            #region SomethingIsWrong(string message)

            logger.Message = "SomethingIsWrong(string message)";

            // Inject code to scope
            scope.Begin = () =>
            {
                SomethingIsWrong("Unknown error, code: 1."); // Exception will be handled!
            };

            Console.WriteLine("Time ellapsed {0} ms.", profiler.EllapsedTime.TotalMilliseconds);

            Console.WriteLine();

            logger.Message = "SomethingIsWrong(string message)";

            // Disable exception handler
            exShield.Enabled = false;

            try
            {
                // Inject code to scope
                scope.Begin = () =>
                {
                    SomethingIsWrong("Unknown error, code: 2."); // Exception will be unhandled!
                };

                Console.WriteLine("Time ellapsed {0} ms.", profiler.EllapsedTime.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception. Message: {0}.", ex.Message);
            }

            #endregion

            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }

        static void DoSomething()
        {
            Console.WriteLine("Doing something.");
            Thread.Sleep(1000);
        }

        static int GetSomething()
        {
            Console.WriteLine("Getting something."); 
            Thread.Sleep(1000);
            return 1978;
        }

        static void ValidateSomething(string value)
        {
            if (value == null) throw new ArgumentNullException("value", "Value is null.");
            if (value == string.Empty) throw new ArgumentException("Value is empty.", "value");
        }

        static void SomethingIsWrong(string message)
        {
            throw new Exception(message);
        }
    }
}
