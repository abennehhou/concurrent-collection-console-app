using System;

namespace ConcurrentCollectionConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var generator = new SalesGenerator();

            generator.RunTest();

            Console.ReadLine();
        }
    }
}
