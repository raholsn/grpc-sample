using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Grpc.Contract;
using Grpc.Core;

using GrpcService;

namespace PerformanceTests
{
    public class GrpcPerformanceTest
    {
        private readonly Grpc.Contract.GrpcService.GrpcServiceClient _client;
        private readonly Stopwatch _stopWatch = new Stopwatch();
        public int NrOfRequests { get; private set; }
        public List<GrpcRequest> GrpcRequests { get; private set; }
        public int Type { get; private set; }

        public GrpcPerformanceTest()
        {
            Task.Run(() => Start.Main(null));
            Thread.Sleep(1000);
            var channel = new Channel("localhost:54321", ChannelCredentials.Insecure);
            _client = new Grpc.Contract.GrpcService.GrpcServiceClient(channel);
        }

        public async Task<TimeSpan> Test()
        {
            _stopWatch.Reset();
            switch (Type)
            {
                case (int)Utilities.GrpcTypes.BiDirectionalStream:
                    _stopWatch.Start();
                    using (var call = _client.GetCustomersWithBidirectionalStream())
                    {
                        var result = new ConcurrentBag<GrpcRequest>();

                        var read = Task.Run(async () =>
                        {
                            while (await call.ResponseStream.MoveNext(CancellationToken.None))
                            {
                                result.Add(new GrpcRequest { Value = call.ResponseStream.Current.Value });
                            }
                        });

                        foreach (var grpcRequest in GrpcRequests.ToList())
                        {
                            await call.RequestStream.WriteAsync(grpcRequest);
                        }

                        await call.RequestStream.CompleteAsync();
                        await read;
                    }

                    _stopWatch.Stop();
                    var elapsedTime = _stopWatch.Elapsed;
                    _stopWatch.Reset();

                    return elapsedTime;

                case (int)Utilities.GrpcTypes.RequestResponse:
                    _stopWatch.Start();
                    foreach (var grpcRequest in GrpcRequests.ToList())
                    {
                        await _client.GetCustomerAsync(grpcRequest);
                    }

                    _stopWatch.Stop();
                    break;
                default:
                    throw new ArgumentException("Type not supported");
            }

            return _stopWatch.Elapsed;
        }

        public async Task GrpcRequestResponse(GrpcRequest _sampleRequest)
        {
            await _client.GetCustomerAsync(_sampleRequest);
        }

        public GrpcPerformanceTest WithRequests(int i)
        {
            NrOfRequests = i;

            return this;
        }

        public GrpcPerformanceTest Build()
        {
            if (NrOfRequests == 0)
            {
                throw new ArgumentException("request not specified");
            }

            if (Type == 0)
            {
                throw new ArgumentException("type not specified");
            }

            GrpcRequests = new List<GrpcRequest>();
            for (var i = 0; i < NrOfRequests; i++)
            {
                GrpcRequests.Add(new GrpcRequest
                {
                    Value = i
                });
            }

            return this;
        }

        public GrpcPerformanceTest WithType(Utilities.GrpcTypes type)
        {
            Type = (int)type;

            return this;
        }
    }
}
