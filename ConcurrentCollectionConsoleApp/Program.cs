using System;

namespace ConcurrentCollectionConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Basic examples
            var basicUsage = new BasicUsage();

            basicUsage.TestConcurrentDictionary();
            Console.WriteLine();

            basicUsage.TestConcurrentQueue();
            Console.WriteLine();

            basicUsage.TestConcurrentStack();
            Console.WriteLine();

            basicUsage.TestConcurrentBag();
            Console.WriteLine();

            basicUsage.TestProducerConsumerCollection();
            Console.WriteLine();

            // Full example
            var generator = new SalesGenerator();
            generator.RunTest();

            Console.ReadLine();
        }
    }
}
