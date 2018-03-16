using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Grpc.Contract;
using Grpc.Core;


namespace GrpcService.Services
{
    public class LuckQueryService : Grpc.Contract.GrpcService.GrpcServiceBase
    {
        public override async Task<GrpcResponse> GetCustomer(GrpcRequest request, ServerCallContext context)
        {
            Console.WriteLine("GetCustomer Server Received " + request.Collector);

            return await Task.FromResult(new GrpcResponse
            {
                Collector = request.Collector
            });
        }

        public override async Task<GrpcResponses> GetCustomers(GrpcRequests request, ServerCallContext context)
        {
            Console.WriteLine("GetCustomers Server Received " );

            var response = new GrpcResponses();
            foreach (var grpcRequest in request.Request)
            {
                response.Response.Add(new GrpcResponse
                {
                    Collector = grpcRequest.Collector
                });
            }

            return await Task.FromResult(response);
        }

        public override async Task GetCustomerWithBidirectionalStream(IAsyncStreamReader<GrpcRequest> requestStream,
            IServerStreamWriter<GrpcResponse> responseStream,
            ServerCallContext context)
        {
            while (await requestStream.MoveNext(CancellationToken.None))
            {
                Console.WriteLine("GetCustomerWithBidirectionalStream Server Received " + requestStream.Current.Collector);

                await responseStream.WriteAsync(new GrpcResponse
                {
                    Collector = requestStream.Current.Collector
                });
            }
        }
    }
}
