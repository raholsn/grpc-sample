using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.Xunit2;

using FluentAssertions;

using Grpc.Contract;
using Grpc.Core;

using GrpcService;

using Xunit;

namespace GrpcServiceTests
{
    public class GrpcServiceTests
    {
        private static Fixture _fixture = new Fixture();
        private readonly Grpc.Contract.GrpcService.GrpcServiceClient client;
        public GrpcServiceTests()
        {
            Task.Run(() => Start.Main(null));
            Thread.Sleep(1000);

            var channel = new Channel("localhost:54321", ChannelCredentials.Insecure);
            client = new Grpc.Contract.GrpcService.GrpcServiceClient(channel);

        }


        public class GetCustomer : GrpcServiceTests
        {
            private readonly IEnumerable<GrpcRequest> _sampleRequest = _fixture.CreateMany<GrpcRequest>();

            [Fact]
            public async Task Should_return_correct_value()
            {
                var result = new List<GrpcRequest>();

                foreach (var grpcRequest in _sampleRequest)
                {
                    var response = await client.GetCustomerAsync(grpcRequest);

                    result.Add(new GrpcRequest
                    {
                        Value = response.Value
                    });
                }

                _sampleRequest.Should().BeEquivalentTo(result);

            }
        }

        public class GetCustomers : GrpcServiceTests
        {
            private readonly GrpcRequests _sampleRequest = _fixture.Create<GrpcRequests>();

            [Fact]
            public async Task Should_return_correct_value()
            {
                var result = new GrpcRequests();
                ;
                var responses = await client.GetCustomersAsync(_sampleRequest);

                foreach (var grpcResponse in responses.Response)
                {
                    result.Request.Add(new GrpcRequest
                    {
                        Value = grpcResponse.Value
                    });
                }

                _sampleRequest.Should().BeEquivalentTo(result);

            }
        }

        public class GetCustomerWithBiDirectionalStream : GrpcServiceTests
        {
            private readonly GrpcRequests _sampleRequest = _fixture.Create<GrpcRequests>();

            [Fact]
            public async Task Should_return_correct_value()
            {
                using (var call = client.GetCustomersWithBidirectionalStream())
                {
                    var result = new ConcurrentBag<GrpcRequest>();
                    var read = Task.Run(async () =>
                    {
                        while (await call.ResponseStream.MoveNext(CancellationToken.None))
                        {
                            result.Add(new GrpcRequest { Value = call.ResponseStream.Current.Value });
                        }
                    });

                    foreach (var grpcRequest in _sampleRequest.Request)
                    {
                        await call.RequestStream.WriteAsync(grpcRequest);
                    }

                    await call.RequestStream.CompleteAsync();
                    await read;
                }
            }

        }
    }
}
