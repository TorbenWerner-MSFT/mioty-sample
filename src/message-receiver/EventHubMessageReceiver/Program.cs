using Azure.Messaging.EventHubs.Consumer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubMessageReceiver
{
    class Program
    {
        private const string _connectionString = "[your-eventhubs-consumer-connection-string]";
        private const string _eventHubName = "[your-eventhubs-name]";
        private const string _consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

        private static EventHubConsumerClient _consumer;

        private static async Task Main()
        {
            _consumer = new EventHubConsumerClient(_consumerGroup, _connectionString, _eventHubName);

            var partitions = await _consumer.GetPartitionIdsAsync().ConfigureAwait(false);

            using var cts = new CancellationTokenSource();
            {
                Console.CancelKeyPress += (s, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();

                    Console.WriteLine("Exiting...");
                };

                var tasks = new List<Task>();

                foreach (var partition in partitions)
                {
                    tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
                }

                Task.WaitAll(tasks.ToArray());
            }
        }

        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            Console.WriteLine("Listening for messages on partition: " + partition);

            while (true)
            {
                if (ct.IsCancellationRequested) break;

                var events = _consumer.ReadEventsFromPartitionAsync(partition, EventPosition.Latest, new ReadEventOptions { }, ct).ConfigureAwait(false);

                await foreach (var eventData in events)
                {
                    var data = Encoding.UTF8.GetString(eventData.Data.Body.ToArray());

                    Console.WriteLine("{0}: Message received on partition {1}:", DateTime.UtcNow, partition);
                    Console.WriteLine();
                    Console.WriteLine("{0}:", data);
                    Console.WriteLine();
                    Console.WriteLine("Application properties (set by device):");
                    Console.WriteLine();

                    foreach (var prop in eventData.Data.Properties)
                    {
                        Console.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                    }

                    Console.WriteLine("System properties (set by IoT Hub):");
                    Console.WriteLine();

                    foreach (var prop in eventData.Data.SystemProperties)
                    {
                        Console.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                    }

                    Console.WriteLine("======================================");
                    Console.WriteLine();
                }
            }
        }
    }
}
