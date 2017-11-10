using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace ConcurrentCollectionConsoleApp
{
    public class StockService
    {
        ConcurrentDictionary<string, int> _stock = new ConcurrentDictionary<string, int>();
        int _totalAddedToStock;
        int _totalSold;

        public void AddToStock(string item, int quantity)
        {
            _stock.AddOrUpdate(item, quantity, (key, oldValue) => oldValue + quantity);
            Interlocked.Add(ref _totalAddedToStock, quantity);
        }

        public bool TrySellItem(string item)
        {
            // If the item does not exist, put -1, otherwise, decrement the value.
            // Once the operation is done. Increment the stock if the value is -1.
            // Always make sure that the delegate in the update value factory is simple and does not use a variable outside the delegate.

            int newStockLevel = _stock.AddOrUpdate(item, -1, (key, oldValue) => oldValue - 1);
            if (newStockLevel < 0)
            {
                _stock.AddOrUpdate(item, 0, (key, oldValue) => oldValue + 1);
                return false;
            }
            else
            {
                Interlocked.Increment(ref _totalSold);
                return true;
            }
        }

        public void VerifyStockStatus()
        {
            // This can be slow (the call to .Values.Sum()). Avoid calling this method intensively.
            int totalStock = _stock.Values.Sum();
            Console.WriteLine($"Added = {_totalAddedToStock} - Sold = {_totalSold } - Stock = {totalStock}.");

            var expectedTotalStock = _totalAddedToStock - _totalSold;
            if (expectedTotalStock == totalStock)
                Console.WriteLine("Total stock as expected.");
            else
                Console.WriteLine($"Total stock incorrect. Expected: {expectedTotalStock} - actual: {totalStock}.");

            Console.WriteLine("Stock levels by item:");
            foreach (string itemName in _stock.Keys)
            {
                int stockLevel = _stock.GetOrAdd(itemName, 0);
                Console.WriteLine($"- {itemName}: {stockLevel}");
            }
        }

    }
}
