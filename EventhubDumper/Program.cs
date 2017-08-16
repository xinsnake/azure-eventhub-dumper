using EventhubDumper.Models;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventhubDumper
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 4)
                {
                    throw new Exception("Usage: .\\EventhubDumper.exe $ConectionString $EventHubName $StartTimestamp $WriteTo");
                }
                _Execute(_Prepare(args));

            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
            }
        }

        private static Configuration _Prepare(string[] args)
        {
            return new Configuration()
            {
                ConnectionString = args[0],
                EventHubName = args[1],
                StartTimestamp = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Double.Parse(args[2])),
                WriteTo = args[3]
            };
        }

        private static void _Execute(Configuration c)
        {
            var client = EventHubClient.CreateFromConnectionString(c.ConnectionString, c.EventHubName);
            var group = client.GetDefaultConsumerGroup();
            var receiver = group.CreateReceiver(client.GetRuntimeInformation().PartitionIds[0], c.StartTimestamp);

            int counter = 0;

            bool receive = true;
            using (var writer = new StreamWriter(c.WriteTo))
            {
                while (receive)
                {
                    var message = receiver.Receive();
                    var body = Encoding.UTF8.GetString(message.GetBytes());
                    var time = message.EnqueuedTimeUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz");

                    writer.WriteLine("MESSAGE " + message.Offset + " @ " + time);
                    writer.WriteLine(body);
                    writer.WriteLine("");

                    Console.Write(".");
                    if (counter % 60 == 59)
                    {
                        Console.WriteLine("");
                    }
                    counter++;
                }
            }
        }
    }
}
