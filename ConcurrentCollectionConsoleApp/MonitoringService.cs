using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ConcurrentCollectionConsoleApp
{
    public class MonitoringService
    {
        private readonly BlockingCollection<Sale> _salesQueue = new BlockingCollection<Sale>(new ConcurrentQueue<Sale>());

        private ConcurrentDictionary<string, int> _totalSoldByPerson = new ConcurrentDictionary<string, int>();

        public void AddSaleOperation(Sale sale)
        {
            _salesQueue.Add(sale);
        }

        public void CompleteAdding()
        {
            _salesQueue.CompleteAdding();
        }

        public void MonitorSales()
        {
            while (true)
            {
                try
                {
                    // The "Take" will block the thread until an item is added to the queue.
                    var sale = _salesQueue.Take();
                    Thread.Sleep(300);

                    _totalSoldByPerson.AddOrUpdate(sale.Person, sale.Quantity, (key, oldValue) => oldValue + sale.Quantity);

                    Console.WriteLine($"Processing sale from { sale.Person } for item { sale.Item }.");
                }
                catch (InvalidOperationException ex)
                {
                    // When the BlockingCollection is Completed (CompleteAdding called), the Take method raises an InvalidOperationException
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
        }


        public void DisplayReport(string[] people)
        {
            Console.WriteLine();
            Console.WriteLine("Sale operations by person:");
            foreach (var person in people)
            {
                int sales = _totalSoldByPerson.GetOrAdd(person, 0);
                Console.WriteLine($"{person} sold {sales}");
            }
        }

    }
}
