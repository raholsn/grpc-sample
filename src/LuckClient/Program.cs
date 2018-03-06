using System;

using Grpc.Core;

using Queries.Contract.protos;

namespace LuckQueryClient
{
    class Program
    {
        static void Main(string[] args)
        {


            var channel = new Channel("localhost:54321", ChannelCredentials.Insecure);

            var client = new GrpcQueryService.GrpcQueryServiceClient(channel);
            var input = "";

            Console.WriteLine("Quit to Exit.");

            Console.WriteLine("Type your name:");

            while ((input = Console.ReadLine()) != "Quit")
            {
                var reply = client.GetLuck(new LuckRequest
                {
                    Name = input
                });
                Console.WriteLine("You luck today will be: " + reply.Procent);
                Console.WriteLine(reply.Notification);
            }
          

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
