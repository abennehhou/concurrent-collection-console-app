using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentCollectionConsoleApp
{
    public class SalesGenerator
    {
        private static readonly string[] _itemNames = { "Pen", "Pineapple", "Apple" };
        private static readonly string[] _people = { "Luffy", "Nami", "Zoro", "Sanji" };
        private readonly TimeSpan _duration = new TimeSpan(0, 0, 1);

        private readonly StockService _stockService;
        private readonly MonitoringService _monitoringService;

        public SalesGenerator()
        {
            _stockService = new StockService();
            _monitoringService = new MonitoringService();
        }

        public void RunTest()
        {
            var supplyTask = Task.Run(async () => await SupplyStockAsync());
            var tasks = _people
                .Select(person => Task.Run(async () => await WorkAsync(person)));
            tasks.Append(supplyTask);

            Task logger1 = Task.Run(() => _monitoringService.MonitorSales());
            Task logger2 = Task.Run(() => _monitoringService.MonitorSales());

            Task.WaitAll(tasks.ToArray());

            _monitoringService.CompleteAdding();
            Task.WaitAll(logger1, logger2);

            _stockService.VerifyStockStatus();
            _monitoringService.DisplayReport(_people);

            Console.WriteLine();
        }

        public async Task SupplyStockAsync()
        {
            var random = new Random();
            var start = DateTime.UtcNow;
            while (DateTime.UtcNow - start < _duration)
            {
                var quantity = random.Next(9) + 1;
                int threadId = Thread.CurrentThread.ManagedThreadId;
                var itemName = _itemNames[random.Next(_itemNames.Length)];

                _stockService.AddToStock(itemName, quantity);
                Console.WriteLine($"Thread {threadId}: Added to stock {quantity} of {itemName}.");
                await Task.Delay(random.Next(100));
            }
        }

        public async Task WorkAsync(string name)
        {
            var random = new Random(name.GetHashCode());
            var start = DateTime.UtcNow;
            while (DateTime.UtcNow - start < _duration)
            {
                await Task.Delay(random.Next(100));

                string itemName = _itemNames[random.Next(_itemNames.Length)];
                int threadId = Thread.CurrentThread.ManagedThreadId;
                bool success = _stockService.TrySellItem(itemName);
                if (success)
                {
                    Console.WriteLine($"Thread {threadId}: {name} sold {itemName}");
                    _monitoringService.AddSaleOperation(new Sale { Person = name, Item = itemName, Quantity = 1 });
                }
                else
                    Console.WriteLine($"Thread {threadId}: {name}: out of stock of {itemName}");

            }

            Console.WriteLine($"Work complete for {name}.");
        }
    }
}
