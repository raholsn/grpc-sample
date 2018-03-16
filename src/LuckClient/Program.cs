using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Google.Protobuf.Collections;

using Grpc.Contract;
using Grpc.Core;

namespace GrpcClient
{
    class Program
    {
        private const int iterations = 5000;

        static void Main(string[] args)
        {
            var channel = new Channel("localhost:54321", ChannelCredentials.Insecure);

            var client = new GrpcService.GrpcServiceClient(channel);

            var timeTakenGetCustomer = GetCustomer(client, iterations).Result;
            var timeTakenBiStream = GetCustomerWithBidirectionalStream(client, iterations).Result;
            var timeTakenGetCustomers = GetCustomers(client, iterations).Result;

            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("TIMETAKEN:");
            Console.WriteLine("timeTakenBiStream(sec): " + timeTakenBiStream.TotalSeconds);
            Console.WriteLine("timeTakenGetCustomer(sec): " + timeTakenGetCustomer.TotalSeconds);
            Console.WriteLine("timeTakenGetCustomers(sec): " + timeTakenGetCustomers.TotalSeconds);
            Console.WriteLine("-----------------------------------------------------");

            Console.ReadLine();

            channel.ShutdownAsync().Wait();
        }

        private static async Task<TimeSpan> GetCustomers(GrpcService.GrpcServiceClient client, int iterations)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();


            var requests = new GrpcRequests();
            for (var i = 0; i < iterations; i++)
            {
                requests.Request.Add(new GrpcRequest
                {
                    Collector = i
                });

                Console.WriteLine("GetCustomers preparied customer: " + i);

            }

            var result = await client.GetCustomersAsync(requests);

            //Thread.Sleep(10);
            Console.WriteLine("GetCustomer client Received " + result.Response.Count);
            stopWatch.Stop();

            return await Task.FromResult<TimeSpan>(stopWatch.Elapsed);
        }

        private static async Task<TimeSpan> GetCustomer(GrpcService.GrpcServiceClient client, int iterations)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (var i = 0; i < iterations; i++)
            {
                var result = await client.GetCustomerAsync(new GrpcRequest
                {
                    Collector = i
                });
                //Thread.Sleep(10);
                Console.WriteLine("GetCustomer client Received " + result.Collector);
            }

            stopWatch.Stop();

            return await Task.FromResult<TimeSpan>(stopWatch.Elapsed);
        }

        private static async Task<TimeSpan> GetCustomerWithBidirectionalStream(GrpcService.GrpcServiceClient client, int iterations)
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            using (var call = client.GetCustomerWithBidirectionalStream())
            {
                var read = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext(CancellationToken.None))
                    {
                        var grcpResponse = call.ResponseStream.Current;
                        Console.WriteLine("GetCustomerWithBidirectionalStream Client Received " + grcpResponse.Collector);
                    }
                });

                for (var i = 0; i < iterations; i++)
                {
                    await call.RequestStream.WriteAsync(new GrpcRequest { Collector = i });
                    //Thread.Sleep(10);
                }

                await call.RequestStream.CompleteAsync();
                await read;
            }
            stopWatch.Stop();

            return await Task.FromResult<TimeSpan>(stopWatch.Elapsed);
        }
    }
}

