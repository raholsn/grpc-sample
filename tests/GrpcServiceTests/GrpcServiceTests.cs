using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using Grpc.Contract;

using Xunit;

namespace GrpcServiceTests
{
    public class GrpcServiceTests : GrpcServiceTestsBase
    {
        public class GetCustomer : GrpcServiceTests
        {
            private readonly IEnumerable<GrpcRequest> _sampleRequest = Fixture.CreateMany<GrpcRequest>();

            [Fact]
            public async Task Should_return_correct_value()
            {
                var result = new List<GrpcRequest>();

                foreach (var grpcRequest in _sampleRequest)
                {
                    var response = await Client.GetCustomerAsync(grpcRequest);

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
            private readonly GrpcRequests _sampleRequest = Fixture.Create<GrpcRequests>();

            [Fact]
            public async Task Should_return_correct_value()
            {
                var result = new GrpcRequests();
                ;
                var responses = await Client.GetCustomersAsync(_sampleRequest);

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
            private readonly IEnumerable<GrpcRequest> _sampleRequest = Fixture.CreateMany<GrpcRequest>();

            [Fact]
            public async Task Should_return_correct_value()
            {
                
                using (var call = Client.GetCustomersWithBidirectionalStream())
                {
                    var result = new ConcurrentBag<GrpcRequest>();
                    
                    var read = Task.Run(async () =>
                    {
                        while (await call.ResponseStream.MoveNext(CancellationToken.None))
                        {
                            result.Add(new GrpcRequest { Value = call.ResponseStream.Current.Value });
                        }
                    });

                    foreach (var grpcRequest in _sampleRequest.ToList())
                    {
                        await call.RequestStream.WriteAsync(grpcRequest);
                    }

                    await call.RequestStream.CompleteAsync();
                    await read;

                    foreach (var grpcRequest in result)
                    {
                        _sampleRequest.Should().Contain(grpcRequest);
                    }
                }
            }

        }
    }
}
