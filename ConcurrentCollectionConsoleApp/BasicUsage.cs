using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace ConcurrentCollectionConsoleApp
{
    public class BasicUsage
    {
        private string[] Items = { "Pen", "Pineapple", "Apple", "PPAP" };

        public void TestConcurrentDictionary()
        {
            Console.WriteLine("***** Test concurrent dictionary *****");
            var dic = new ConcurrentDictionary<string, int>();
            var random = new Random();
            Parallel.ForEach(Items, item =>
            {
                bool added = dic.TryAdd(item, random.Next(100));
                Console.WriteLine($"Item {item} added?: {added}");
            });

            // Try add existing item
            bool success = dic.TryAdd("PPAP", 15);
            Console.WriteLine("Try adding already existing item. Added? " + success);

            // Increment or add if doesn't exist
            var testKey = "Pineapple";
            int newValue = dic.AddOrUpdate(testKey, 1, (key, oldValue) => oldValue + 1);
            Console.WriteLine($"New value is {newValue}.");

            Console.WriteLine($"dic[{testKey}] = {dic.GetOrAdd(testKey, 0)}");

            // Try remove
            int existingValue;
            success = dic.TryRemove(testKey, out existingValue);
            if (success)
                Console.WriteLine($"value removed was: {existingValue}.");
            else
                Console.WriteLine($"Could not remove the key {testKey}.");

            Console.WriteLine($"Enumerating: [{string.Join(", ", dic.Select(x => $"{x.Key}: {x.Value}"))}]");
        }

        public void TestConcurrentQueue()
        {
            Console.WriteLine("***** Test concurrent Queue *****");
            var queue = new ConcurrentQueue<string>();
            Parallel.ForEach(Items, item =>
            {
                queue.Enqueue(item);
            });

            Console.WriteLine($"Enumerating: [{string.Join(", ", queue.Select(x => x))}]. Count = {queue.Count}");

            // TryDequeue gets and removes the first item in the queue (FIFO)
            string item1;
            bool success = queue.TryDequeue(out item1);
            if (success)
                Console.WriteLine($"{ item1 } removed.");
            else
                Console.WriteLine("Queue was empty");

            // TryPeek gets the first item in the queue without removing it.
            string item2;
            success = queue.TryPeek(out item2);
            if (success)
                Console.WriteLine($"{ item2 } peeked.");
            else
                Console.WriteLine("Queue was empty");

            Console.WriteLine($"Enumerating: [{string.Join(", ", queue.Select(x => x))}]. Count = {queue.Count}");
        }

        public void TestConcurrentStack()
        {
            Console.WriteLine("***** Test concurrent Stack *****");
            var stack = new ConcurrentStack<string>();
            Parallel.ForEach(Items, item =>
            {
                stack.Push(item);
            });

            Console.WriteLine($"Enumerating: [{string.Join(", ", stack.Select(x => x))}]. Count = {stack.Count}");

            // TryPop gets and removes the first item in the stack (LIFO)
            string item1;
            bool success = stack.TryPop(out item1);
            if (success)
                Console.WriteLine($"{ item1 } removed.");
            else
                Console.WriteLine("Stack was empty");

            // TryPeek gets the first item in the stack without removing it.
            string item2;
            success = stack.TryPeek(out item2);
            if (success)
                Console.WriteLine($"{ item2 } peeked.");
            else
                Console.WriteLine("Stack was empty");

            Console.WriteLine($"Enumerating: [{string.Join(", ", stack.Select(x => x))}]. Count = {stack.Count}");
        }

        public void TestConcurrentBag()
        {
            Console.WriteLine("***** Test concurrent Bag *****");
            var bag = new ConcurrentBag<string>();
            Parallel.ForEach(Items, item =>
            {
                bag.Add(item);
            });

            Console.WriteLine($"Enumerating: [{string.Join(", ", bag.Select(x => x))}]. Count = {bag.Count}");

            // TryTake gets and removes an item in the bag (order not guaranteed)
            string item1;
            bool success = bag.TryTake(out item1);
            if (success)
                Console.WriteLine($"{ item1 } removed.");
            else
                Console.WriteLine("Bag was empty");

            // TryPeek gets an item in the bag without removing it.
            string item2;
            success = bag.TryPeek(out item2);
            if (success)
                Console.WriteLine($"{ item2 } peeked.");
            else
                Console.WriteLine("Bag was empty");

            Console.WriteLine($"Enumerating: [{string.Join(", ", bag.Select(x => x))}]. Count = {bag.Count}");
        }

        public void TestProducerConsumerCollection()
        {
            Console.WriteLine("***** Test producer consumer collection *****");
            IProducerConsumerCollection<string> collection = new ConcurrentQueue<string>();
            Parallel.ForEach(Items, item =>
            {
                collection.TryAdd(item);
            });

            Console.WriteLine($"Enumerating: [{string.Join(", ", collection.Select(x => x))}]. Count = {collection.Count}");

            // TryTake gets and removes the first item in the Producer Consumer Collection
            // The order depends on the collection used. Here a concurrent queue, so FIFO.
            string item1;
            bool success = collection.TryTake(out item1);
            if (success)
                Console.WriteLine($"{ item1 } removed.");
            else
                Console.WriteLine("Collection was empty");

            Console.WriteLine($"Enumerating: [{string.Join(", ", collection.Select(x => x))}]. Count = {collection.Count}");
        }

    }
}
