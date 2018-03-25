using System;
using System.Threading;
using System.Threading.Tasks;

using Grpc.Contract;
using Grpc.Core;

namespace GrpcService.Services
{
    public class GrpcService : Grpc.Contract.GrpcService.GrpcServiceBase
    {
        public override async Task<GrpcResponse> GetCustomer(GrpcRequest request, ServerCallContext context)
        {
            Console.WriteLine("GetCustomer Server Received " + request.Value);

            return await Task.FromResult(new GrpcResponse
            {
                Value = request.Value
            });
        }

        public override async Task<GrpcResponses> GetCustomers(GrpcRequests request, ServerCallContext context)
        {
            Console.WriteLine("GetCustomers Server Received ");

            var response = new GrpcResponses();
            foreach (var grpcRequest in request.Request)
            {
                response.Response.Add(new GrpcResponse
                {
                    Value = grpcRequest.Value
                });
            }

            return await Task.FromResult(response);
        }

        public override async Task<GrpcResponses> GetCustomersWithClientStream(IAsyncStreamReader<GrpcRequest> requestStream,
            ServerCallContext context)
        {
            var response = new GrpcResponses();
            while (await requestStream.MoveNext(CancellationToken.None))
            {
                response.Response.Add(new GrpcResponse
                {
                    Value = requestStream.Current.Value
                });
            }

            return response;
        }

        public override async Task GetCustomersWithBidirectionalStream(IAsyncStreamReader<GrpcRequest> requestStream,
            IServerStreamWriter<GrpcResponse> responseStream,
            ServerCallContext context)
        {
            while (await requestStream.MoveNext(CancellationToken.None))
            {
                Console.WriteLine("GetCustomerWithBidirectionalStream Server Received " + requestStream.Current.Value);

                await responseStream.WriteAsync(new GrpcResponse
                {
                    Value = requestStream.Current.Value
                });
            }
        }
    }
}
